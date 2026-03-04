using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TracNghiemLaiXe.Models
{
    public class QuizAnswer
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
    }

    public partial class QuizQuestion : ObservableObject
    {
        public int Id { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasImage))]
        private string? _imagePath;

        public bool IsFatal { get; set; } // "Câu điểm liệt" - Immediate fail if incorrect
        public string Explanation { get; set; } = string.Empty;
        public List<QuizAnswer> Answers { get; set; } = new();
        
        public bool HasImage => !string.IsNullOrWhiteSpace(ImagePath);
    }

    public class QuestionResult
    {
        public int QuestionId { get; set; }
        public int SelectedAnswerId { get; set; } // 0 or -1 if unanswered
        public bool IsFlagged { get; set; }
        public bool IsCorrect { get; set; } 
    }

    public class ExamResult
    {
        public int CorrectCount { get; set; }
        public int TotalCount { get; set; }
        public bool FailedParalysis { get; set; }
        public bool Passed { get; set; }
        public TimeSpan TimeTaken { get; set; }
        public List<QuestionResult> QuestionResults { get; set; } = new();
        public DateTime CompletedAt { get; set; } = DateTime.Now;

        // Backward compatibility properties if needed
        public int TotalScore => CorrectCount; // Alias
        public bool IsFailedByFatal => FailedParalysis; // Alias
    }

    /// <summary>
    /// Lightweight model specifically for the Review UI using ObservableCollection.
    /// </summary>
    public class ReviewItemDisplay
    {
        public int QuestionNumber { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string UserAnswer { get; set; } = string.Empty;
        public string CorrectAnswer { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public bool IsFatal { get; set; }
        public bool IsFlagged { get; set; }
        public string? ImagePath { get; set; } // Added for Image Optimization
        
        // UI Helpers
        public string StatusColor => IsCorrect ? "#4CAF50" : (IsFatal ? "#F44336" : "#FF5722"); // Green, Red (Fatal), Orange (Wrong)
        public string UserAnswerColor => IsCorrect ? "Black" : "Red";
    }

    public enum ReviewFilterType
    {
        All,
        Incorrect,
        Flagged
    }

    public class ReviewNavigationMessage
    {
        public ReviewFilterType FilterType { get; set; }
        public ReviewNavigationMessage(ReviewFilterType filterType) { FilterType = filterType; }
    }
}
