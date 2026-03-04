using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TracNghiemLaiXe.Models;
using TracNghiemLaiXe.Views;

namespace TracNghiemLaiXe.ViewModels
{
    [QueryProperty(nameof(CorrectCount), "correct")]
    [QueryProperty(nameof(TotalCount), "total")]
    [QueryProperty(nameof(FailedParalysis), "paralysis")]
    [QueryProperty(nameof(Passed), "passed")]
    public partial class ResultViewModel : ObservableObject
    {
        [ObservableProperty]
        private int _correctCount;

        [ObservableProperty]
        private int _totalCount;

        [ObservableProperty]
        private bool _failedParalysis;

        [ObservableProperty]
        private bool _passed;

        public string ResultIcon => Passed ? "✅" : "❌";
        
        public string ResultTitle => Passed ? "ĐẠT" : "KHÔNG ĐẠT";
        
        public Color ResultTitleColor => Passed 
            ? Color.FromArgb("#10B981") 
            : Color.FromArgb("#EF4444");

        public Color ResultBackgroundColor => Passed 
            ? Color.FromArgb("#D1FAE5") 
            : Color.FromArgb("#FEE2E2");

        public string ResultSubtitle => Passed 
            ? "Chúc mừng bạn đã hoàn thành bài thi!" 
            : FailedParalysis 
                ? "Bạn đã trả lời sai câu hỏi điểm liệt." 
                : "Bạn chưa đạt điểm tối thiểu 80%.";

        public string PercentageDisplay => $"{(TotalCount > 0 ? (CorrectCount * 100 / TotalCount) : 0)}%";

        public bool ShowParalysisStatus => FailedParalysis;

        public string ParalysisStatusText => FailedParalysis ? "Sai" : "Đúng";

        public Color ParalysisStatusColor => FailedParalysis 
            ? Color.FromArgb("#EF4444") 
            : Color.FromArgb("#10B981");

        partial void OnCorrectCountChanged(int value)
        {
            NotifyResultProperties();
        }

        partial void OnTotalCountChanged(int value)
        {
            NotifyResultProperties();
        }

        partial void OnPassedChanged(bool value)
        {
            NotifyResultProperties();
        }

        partial void OnFailedParalysisChanged(bool value)
        {
            NotifyResultProperties();
        }

        private void NotifyResultProperties()
        {
            OnPropertyChanged(nameof(ResultIcon));
            OnPropertyChanged(nameof(ResultTitle));
            OnPropertyChanged(nameof(ResultTitleColor));
            OnPropertyChanged(nameof(ResultBackgroundColor));
            OnPropertyChanged(nameof(ResultSubtitle));
            OnPropertyChanged(nameof(PercentageDisplay));
            OnPropertyChanged(nameof(ShowParalysisStatus));
            OnPropertyChanged(nameof(ParalysisStatusText));
            OnPropertyChanged(nameof(ParalysisStatusColor));
        }

        [RelayCommand]
        private async Task ReviewExam()
        {
            // Navigate to Review Page (ListView style) - Push Navigation
            await Shell.Current.GoToAsync(nameof(ReviewPage)); 
        }

        [RelayCommand]
        private async Task RetryExam()
        {
            // Reset and restart same exam type - Push Navigation
            await Shell.Current.GoToAsync($"{nameof(QuizPage)}?action=reset");
        }

        [RelayCommand]
        private async Task GoHome()
        {
            // Return to Main Menu - Root Navigation
            await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
        }
    }
}
