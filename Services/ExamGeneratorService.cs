using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TracNghiemLaiXe.Models;

namespace TracNghiemLaiXe.Services
{
    public class ExamGeneratorService : IExamGeneratorService
    {
        private readonly IDataService _dataService;
        private readonly Random _random = new Random();

        public ExamGeneratorService(IDataService dataService)
        {
            _dataService = dataService;
        }

        public async Task<IReadOnlyList<QuizQuestion>> GenerateExamAsync(LicenseType licenseType)
        {
            // 1. Get Official Config
            var config = LicenseConfig.Get(licenseType);

            // 2. Fetch All Candidate Questions
            // In a real app, filtering by category/license here would be better.
            // But for this 600 question bank, we assume shared pool or handle filtering later.
            var allQuestions = await _dataService.GetAllQuestionsAsync();

            // 3. Separate Pools
            var fatalPool = allQuestions.Where(q => q.IsFatal).ToList();
            var normalPool = allQuestions.Where(q => !q.IsFatal).ToList();

            var examQuestions = new List<QuizQuestion>();

            // 4. Pick Mandatory Fatal Questions
            Shuffle(fatalPool);
            int fatalNeeded = Math.Min(config.FatalQuestionCount, fatalPool.Count);
            examQuestions.AddRange(fatalPool.Take(fatalNeeded));

            // 5. Fill Remainder with Normal Questions
            Shuffle(normalPool);
            int currentCount = examQuestions.Count;
            int normalNeeded = config.TotalQuestions - currentCount;

            if (normalNeeded > 0)
            {
                examQuestions.AddRange(normalPool.Take(Math.Min(normalNeeded, normalPool.Count)));
            }

            // 6. Final Shuffle to mix fatal questions
            Shuffle(examQuestions);

            return examQuestions.AsReadOnly();
        }

        private void Shuffle<T>(IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = _random.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }
    }
}
