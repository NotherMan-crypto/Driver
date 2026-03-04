using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using System.Collections.ObjectModel;
using System.Threading.Tasks; 
using System.Linq;
using TracNghiemLaiXe.Models;
using TracNghiemLaiXe.Services;

namespace TracNghiemLaiXe.ViewModels
{
    // Message to pass the result to this VM
    public class ExamCompletedMessage : ValueChangedMessage<ExamResult>
    {
        public ExamCompletedMessage(ExamResult result) : base(result) { }
    }

    public partial class ReviewViewModel : ObservableObject, IQueryAttributable
    {
        private readonly IDataService _dataService;
        private readonly ImageService _imageService;

        // Internal backing store - kept as List for performance (faster filtering)
        private List<ReviewItemDisplay> _allItems = new();

        [ObservableProperty]
        private ObservableCollection<ReviewItemDisplay> _reviewItems = new();

        [ObservableProperty]
        private string _scoreText = string.Empty;

        [ObservableProperty]
        private string _statusText = string.Empty;

        [ObservableProperty]
        private string _statusColor = "#000000";

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _selectedFilter = "All";

        public ReviewViewModel(IDataService dataService, ImageService imageService)
        {
            _dataService = dataService;
            _imageService = imageService;
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            // Always initialize when navigated to
            Task.Run(async () => await InitializeAsync());
        }

        public void Receive(ExamCompletedMessage message)
        {
            // Update the stored result in DataService if message received
            _dataService.LastExamResult = message.Value;
            Task.Run(async () => await InitializeAsync());
        }

        public void Cleanup()
        {
            ReviewItems.Clear();
            _allItems.Clear();
            _imageService.ReleaseMemory();
            WeakReferenceMessenger.Default.Unregister<ExamCompletedMessage>(this);
        }

        public async Task InitializeAsync()
        {
            IsBusy = true;
            try
            {
                await _dataService.InitializeAsync();
                
                var result = _dataService.LastExamResult;
                if (result == null) return; // Or show error / go back

                // 1. Calculate Score & Status (Business Logic)
                CalculateScore(result);

                // 2. Prepare Display Items (Transformation)
                var displayItems = await Task.Run(async () => await PrepareDisplayItemsAsync(result));
                
                _allItems = displayItems;

                // 3. Initial Filter (All)
                await ApplyFilterAsync("All");

                // 4. Pre-fetch images for better scrolling performance
                var imagesToPrefetch = displayItems
                    .Where(i => !string.IsNullOrEmpty(i.ImagePath))
                    .Select(i => i.ImagePath!) // Non-null validated
                    .Take(20) // Pre-fetch first 20 images
                    .ToList();
                
                await _imageService.PreFetchImagesAsync(imagesToPrefetch);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void CalculateScore(ExamResult result)
        {
            int correctCount = result.CorrectCount;
            int totalQuestions = result.TotalCount;
            
            bool passedScore = result.Passed;
            bool failedFatal = result.FailedParalysis;

            if (failedFatal)
            {
                StatusText = "FAILED (Fatal Error)";
                StatusColor = "#F44336"; // Red
            }
            else if (passedScore)
            {
                StatusText = "PASSED";
                StatusColor = "#4CAF50"; // Green
            }
            else
            {
                StatusText = "FAILED";
                StatusColor = "#FF5722"; // Orange
            }

            ScoreText = $"{correctCount}/{totalQuestions}";
        }

        private async Task<List<ReviewItemDisplay>> PrepareDisplayItemsAsync(ExamResult result)
        {
            var list = new List<ReviewItemDisplay>(result.QuestionResults.Count);
            int index = 1;

            foreach (var r in result.QuestionResults)
            {
                // Fetch cached/db question data efficiently
                var question = await _dataService.GetQuestionByIdAsync(r.QuestionId);
                
                string questionText = question?.QuestionText ?? $"Question {r.QuestionId}";
                string correctAnswerText = question?.Answers.FirstOrDefault(a => a.IsCorrect)?.Text ?? "Unknown";
                string userAnswerText = r.SelectedAnswerId > 0 
                    ? (question?.Answers.FirstOrDefault(a => a.Id == r.SelectedAnswerId)?.Text ?? $"Answer {r.SelectedAnswerId}")
                    : "Chưa trả lời";

                // Resolve optimized image path
                string? imagePath = _imageService.GetImagePath(r.QuestionId, question?.ImagePath);

                list.Add(new ReviewItemDisplay
                {
                    QuestionNumber = index++,
                    QuestionText = questionText,
                    UserAnswer = userAnswerText,
                    CorrectAnswer = correctAnswerText,
                    IsCorrect = r.IsCorrect,
                    IsFlagged = r.IsFlagged,
                    IsFatal = question?.IsFatal ?? false,
                    ImagePath = imagePath
                });
            }
            return list;
        }

        [RelayCommand]
        public async Task ApplyFilterAsync(string filterMode)
        {
            SelectedFilter = filterMode;
            
            var filtered = await Task.Run(() => 
            {
                if (filterMode == "Incorrect")
                    return _allItems.Where(x => !x.IsCorrect).ToList();
                if (filterMode == "Flagged")
                    return _allItems.Where(x => x.IsFlagged).ToList();
                
                return _allItems; // "All"
            });

            await MainThread.InvokeOnMainThreadAsync(() => 
            {
                ReviewItems = new ObservableCollection<ReviewItemDisplay>(filtered);
            });
        }

        [RelayCommand]
        public async Task GoHomeAsync()
        {
            Cleanup();
            await Microsoft.Maui.Controls.Shell.Current.GoToAsync("//MainPage");
        }
    }
}
