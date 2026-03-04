using System.Collections.Generic;
using System.Threading.Tasks;
using TracNghiemLaiXe.Models;

namespace TracNghiemLaiXe.Services
{
    public interface IDataService
    {
        Task InitializeAsync();
        Task<List<QuizQuestion>> GetQuestionsByCategoryAsync(string category);
        Task<List<QuizQuestion>> GetAllQuestionsAsync();
        Task<QuizQuestion?> GetQuestionByIdAsync(int id);
        
        /// <summary>
        /// Generates a random exam set for the specified license type.
        /// Delegates to ExamGeneratorService internally.
        /// </summary>
        Task<IReadOnlyList<QuizQuestion>> GetExamAsync(LicenseType licenseType);
        
        // State management for current session
        ExamResult? LastExamResult { get; set; }
        List<Question> LastExamQuestions { get; set; }
        Dictionary<int, int> LastUserAnswers { get; set; }
        
        // Potential future expansion
        // Task SaveResultAsync(ExamResult result);
        // Task<List<ExamResult>> GetHistoryAsync();
    }
}
