using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using SQLite;
using TracNghiemLaiXe.Models;
using Microsoft.Maui.Storage;
using System.Linq;
using System.Text.RegularExpressions;

namespace TracNghiemLaiXe.Services
{
    public class DataService : IDataService
    {
        private SQLiteAsyncConnection? _db;
        private const string DbName = "driving_quiz.db3";
        private const string CurrentDbVersion = "CSGT-2025-v3"; // Force Reset to Apply Fixes
        private bool _isInitialized;
        public ExamResult? LastExamResult { get; set; }
        public List<Question> LastExamQuestions { get; set; } = new();
        public Dictionary<int, int> LastUserAnswers { get; set; } = new();
        
        private readonly Dictionary<string, List<QuizQuestion>> _categoryCache = new();

        private static string CleanText(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return input;
            // Trim trailing digits
            return Regex.Replace(input.Trim(), @"\s+\d+$", "");
        }

        public DataService() { }

        public async Task InitializeAsync()
        {
            if (_isInitialized) return;

            var dbPath = Path.Combine(FileSystem.AppDataDirectory, DbName);
            _db = new SQLiteAsyncConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);

            // HARD RESET: Drop tables at start to ensure clean state as user requested for forensic clean.
            // This guarantees no stale data exists.
            // Hard Reset Logic
            try {
                // await _db.DropTableAsync<QuestionEntity>(); // Only if needed explicitly
            } catch { }

            try {
                await _db.CreateTableAsync<DatabaseVersionEntity>();
                await _db.CreateTableAsync<QuestionEntity>();
                await _db.CreateTableAsync<AnswerEntity>();

                bool needReseed = await NeedsReseedAsync();
                if (needReseed)
                {
                    await ReseedDatabaseAsync();
                }

                _isInitialized = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DB Init Failed: {ex.Message}");
            }
        }

        private async Task<bool> NeedsReseedAsync()
        {
            if (_db == null) return true;
            try
            {
                var version = await _db.Table<DatabaseVersionEntity>().FirstOrDefaultAsync();
                if (version == null || version.Version != CurrentDbVersion) return true;
                
                var count = await _db.Table<QuestionEntity>().CountAsync();
                if (count < 600) return true; // Enforce 600 questions
                
                return false;
            }
            catch { return true; }
        }

        private async Task ReseedDatabaseAsync()
        {
            if (_db == null) return;
            
            System.Diagnostics.Debug.WriteLine("ReseedDatabaseAsync: Starting...");
            _categoryCache.Clear();
            
            // Explicit drop again just in case
            await _db.DropTableAsync<QuestionEntity>();
            await _db.DropTableAsync<AnswerEntity>();
            
            await _db.CreateTableAsync<QuestionEntity>();
            await _db.CreateTableAsync<AnswerEntity>();
            
            await SeedDatabaseAsync();
            
            await _db.InsertAsync(new DatabaseVersionEntity { Version = CurrentDbVersion });
            System.Diagnostics.Debug.WriteLine($"ReseedDatabaseAsync: Complete. Version {CurrentDbVersion}");
        }

        private async Task SeedDatabaseAsync()
        {
            try
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync("question_bank.json");
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = false };
                var questionBank = await JsonSerializer.DeserializeAsync<QuestionBank>(stream, options);

                if (questionBank?.Questions != null)
                {
                    await BatchInsertQuestionsAsync(questionBank.Questions);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error seeding: {ex.Message}");
            }
        }

        private async Task BatchInsertQuestionsAsync(List<Question> sourceQuestions)
        {
            if (_db == null) return;

            var questionEntities = new List<QuestionEntity>(sourceQuestions.Count);
            var answerEntities = new List<AnswerEntity>(sourceQuestions.Count * 4);

            foreach (var q in sourceQuestions)
            {
                // Smart Path Correction Logic
                // Note: We access q.ImagePath because we expect the model to be updated.
                // However, for safety if model isn't updated yet, we use ImagePath (renamed).
                // Assuming QuestionModel has ImagePath now.
                string? imagePath = q.ImagePath; 

                if (q.Id >= 301)
                {
                    // Force path if null/empty
                    if (string.IsNullOrWhiteSpace(imagePath))
                    {
                        imagePath = $"q{q.Id}.jpeg";
                    }
                    else
                    {
                        // Normalize png -> jpeg
                        imagePath = imagePath.Replace(".png", ".jpeg", StringComparison.OrdinalIgnoreCase);
                        
                        // Force ext if missing
                        if (!imagePath.Contains('.'))
                        {
                            imagePath += ".jpeg";
                        }
                    }
                }
                else
                {
                    // Normalization for < 301
                    if (!string.IsNullOrWhiteSpace(imagePath))
                    {
                        imagePath = imagePath.Replace(".png", ".jpeg", StringComparison.OrdinalIgnoreCase);
                    }
                }

                questionEntities.Add(new QuestionEntity
                {
                    Id = q.Id,
                    QuestionText = q.QuestionText,
                    ImageUrl = imagePath,
                    IsFatal = q.IsParalysis,
                    CorrectAnswerId = q.CorrectIndex,
                    Category = q.CategoryRaw,
                    Explanation = q.Explanation
                });

                foreach (var a in q.Answers)
                {
                    answerEntities.Add(new AnswerEntity
                    {
                        Id = a.Id,
                        QuestionId = q.Id,
                        Text = a.Text,
                        IsCorrect = (a.Id == q.CorrectIndex)
                    });
                }
            }

            await _db.RunInTransactionAsync(trans =>
            {
                trans.InsertAll(questionEntities);
                trans.InsertAll(answerEntities);
            });
        }

        public async Task<List<QuizQuestion>> GetQuestionsByCategoryAsync(string category)
        {
            if (!_isInitialized) await InitializeAsync();
            if (_db == null) return new List<QuizQuestion>();

            if (_categoryCache.TryGetValue(category, out var cached)) return cached;

            var entities = await _db.Table<QuestionEntity>().Where(q => q.Category == category).ToListAsync();
            if (entities.Count == 0) return new List<QuizQuestion>();

            var questionIds = entities.Select(q => q.Id).ToList();
            var answers = await _db.Table<AnswerEntity>().Where(a => questionIds.Contains(a.QuestionId)).ToListAsync();
            var answerLookup = answers.GroupBy(a => a.QuestionId).ToDictionary(g => g.Key, g => g.ToList());

            var result = entities.Select(e => MapToModel(e, answerLookup)).ToList();
            _categoryCache[category] = result;
            return result;
        }

        public async Task<List<QuizQuestion>> GetAllQuestionsAsync()
        {
            await InitializeAsync();
            if (_db == null) return new List<QuizQuestion>();

            var entities = await _db.Table<QuestionEntity>().ToListAsync();
            var answers = await _db.Table<AnswerEntity>().ToListAsync();
            var answerLookup = answers.GroupBy(a => a.QuestionId).ToDictionary(g => g.Key, g => g.ToList());

            return entities.Select(e => MapToModel(e, answerLookup)).ToList();
        }

        public async Task<QuizQuestion?> GetQuestionByIdAsync(int id)
        {
            if (!_isInitialized) await InitializeAsync();
            if (_db == null) return null;

            var entity = await _db.FindWithQueryAsync<QuestionEntity>("SELECT * FROM Questions WHERE Id = ?", id);
            if (entity == null) return null;

            var answers = await _db.Table<AnswerEntity>().Where(a => a.QuestionId == id).ToListAsync();
            return new QuizQuestion
            {
                Id = entity.Id,
                QuestionText = entity.QuestionText,
                ImagePath = entity.ImageUrl,
                IsFatal = entity.IsFatal,
                Explanation = entity.Explanation,
                Answers = answers.Select(a => new QuizAnswer { Id = a.Id, Text = a.Text, IsCorrect = a.IsCorrect }).ToList()
            };
        }

        public async Task<IReadOnlyList<QuizQuestion>> GetExamAsync(LicenseType licenseType)
        {
             await InitializeAsync();
             var structure = LicenseStructureConfig.GetStructure(licenseType);
             var allQuestions = await GetAllQuestionsAsync();
             
             var fatalPool = allQuestions.Where(q => q.IsFatal).ToList();
             var normalPool = allQuestions.Where(q => !q.IsFatal).ToList();
             
             var random = new Random();
             Shuffle(fatalPool, random);
             Shuffle(normalPool, random);
             
             var examQuestions = new List<QuizQuestion>();
             examQuestions.AddRange(fatalPool.Take(Math.Min(structure.FatalQuestionCount, fatalPool.Count)));
             examQuestions.AddRange(normalPool.Take(Math.Min(structure.TotalQuestions - examQuestions.Count, normalPool.Count)));
             
             Shuffle(examQuestions, random);
             return examQuestions.AsReadOnly();
        }

        private QuizQuestion MapToModel(QuestionEntity e, Dictionary<int, List<AnswerEntity>> lookup)
        {
            var q = new QuizQuestion
            {
                Id = e.Id,
                QuestionText = e.QuestionText,
                ImagePath = e.ImageUrl,
                IsFatal = e.IsFatal,
                Explanation = e.Explanation,
                Answers = new List<QuizAnswer>()
            };

            if (lookup.TryGetValue(e.Id, out var entityAnswers))
            {
                q.Answers = entityAnswers.Select(a => new QuizAnswer 
                { 
                    Id = a.Id, Text = a.Text, IsCorrect = a.IsCorrect 
                }).ToList();
            }
            return q;
        }

        private static void Shuffle<T>(IList<T> list, Random random)
        {
            int n = list.Count;
            while (n > 1) { n--; int k = random.Next(n + 1); (list[k], list[n]) = (list[n], list[k]); }
        }
    }

    [Table("Questions")]
    internal class QuestionEntity
    {
        [PrimaryKey] public int Id { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public bool IsFatal { get; set; }
        public int CorrectAnswerId { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Explanation { get; set; } = string.Empty;
    }

    [Table("Answers")]
    internal class AnswerEntity
    {
        [PrimaryKey, AutoIncrement] public int DbId { get; set; }
        [Indexed] public int QuestionId { get; set; }
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
    }

    [Table("DatabaseVersion")]
    internal class DatabaseVersionEntity
    {
        [PrimaryKey, AutoIncrement] public int Id { get; set; }
        public string Version { get; set; } = string.Empty;
    }
}
