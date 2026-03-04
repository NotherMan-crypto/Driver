using System;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TracNghiemLaiXe.Models;
using TracNghiemLaiXe.Services;

namespace TracNghiemLaiXe.ViewModels
{
    public partial class QuizViewModel : ObservableObject, IQueryAttributable
    {
        private IDispatcherTimer? _timer;
        private readonly IDataService _dataService;
        private readonly IExamGeneratorService _examGeneratorService;
        private TimeSpan _remainingTime;
        private TimeSpan _initialTime;
        // private const int ExamDurationMinutes = 20; // Deprecated


        [ObservableProperty]
        private Question? _currentQuestion;

        [ObservableProperty]
        private int _currentQuestionIndex;

        [ObservableProperty]
        private AnswerViewModel? _selectedAnswer;

        [ObservableProperty]
        private string _timerDisplay = "20:00";

        [ObservableProperty]
        private Color _timerBackgroundColor = Color.FromArgb("#DBEAFE");

        [ObservableProperty]
        private Color _timerTextColor = Color.FromArgb("#2563EB");

        [ObservableProperty]
        private string _nextButtonText = "Câu tiếp →";

        [ObservableProperty]
        private bool _isReviewMode;

        [ObservableProperty]
        private ObservableCollection<AnswerViewModel> _currentAnswers = new();

        public ObservableCollection<Question> Questions { get; } = new();
        public ObservableCollection<QuestionNavigatorItem> QuestionNavigators { get; } = new();
        public Dictionary<int, int> UserAnswers { get; } = new();

        public int TotalQuestions => Questions.Count;
        public int CurrentQuestionNumber => CurrentQuestionIndex + 1;
        public int AnsweredCount => UserAnswers.Count;
        public bool CanGoPrevious => CurrentQuestionIndex > 0;
        public bool HasQuestionImage => !string.IsNullOrEmpty(CurrentQuestion?.ImagePath);
        private LicenseType _currentLicenseType;


        public QuizViewModel(IDataService dataService, IExamGeneratorService examGeneratorService)
        {
            _dataService = dataService;
            _examGeneratorService = examGeneratorService;
            _currentLicenseType = LicenseType.B2; // Default
            
            // Timer will be lazy-initialized in StartTimer()
            // This avoids potential null reference if Application.Current is not ready during DI
            
            _initialTime = TimeSpan.FromMinutes(LicenseConfig.Get(_currentLicenseType).ExamDurationMinutes);
            _remainingTime = _initialTime;

            // DO NOT call LoadQuestionsAsync() here!
            // It will be called from ApplyQueryAttributes when page navigates
        }

        private async Task LoadQuestionsAsync()
        {
            try
            {
                await _dataService.InitializeAsync();
                
                // Generate a random exam for selected license
                var quizQuestions = await _examGeneratorService.GenerateExamAsync(_currentLicenseType);
                
                if (quizQuestions == null || quizQuestions.Count == 0) return;

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Questions.Clear();
                    foreach (var q in quizQuestions)
                    {
                        var mappedQ = new Question 
                        { 
                            Id = q.Id, 
                            QuestionText = q.QuestionText,
                            ImagePath = q.ImagePath,
                            IsParalysis = q.IsFatal,
                            Explanation = q.Explanation,
                            CorrectAnswerId = q.Answers.FirstOrDefault(a => a.IsCorrect)?.Id ?? 0,
                            Answers = q.Answers.Select(a => new Answer { Id = a.Id, Text = a.Text }).ToList()
                        };
                        Questions.Add(mappedQ);
                    }
                    
                    InitializeNavigators();
                    UpdateCurrentQuestion();
                    
                    OnPropertyChanged(nameof(TotalQuestions));
                    OnPropertyChanged(nameof(CurrentQuestionNumber));
                });
            }
            catch (Exception ex)
            {
                // Simple logging or alert
                System.Diagnostics.Debug.WriteLine($"Error generating exam: {ex.Message}");
            }
        }

        private void InitializeNavigators()
        {
            QuestionNavigators.Clear();
            for (int i = 0; i < Questions.Count; i++)
            {
                QuestionNavigators.Add(new QuestionNavigatorItem
                {
                    QuestionIndex = i,
                    DisplayNumber = (i + 1).ToString(),
                    IsAnswered = false,
                    IsCurrent = i == 0
                });
            }
        }

        private void UpdateCurrentQuestion()
        {
            if (Questions.Count == 0) return;

            CurrentQuestion = Questions[CurrentQuestionIndex];
            
            // Map Answers to ViewModels
            CurrentAnswers.Clear();
            int savedAnswerId = UserAnswers.TryGetValue(CurrentQuestion.Id, out int val) ? val : -1;
            
            foreach (var answer in CurrentQuestion.Answers)
            {
                var vm = CreateAnswerViewModel(answer, answer.Id == savedAnswerId, answer.Id == CurrentQuestion.CorrectAnswerId);
                CurrentAnswers.Add(vm);
                
                if (vm.IsSelected)
                {
                    SelectedAnswer = vm;
                }
            }
            
            if (savedAnswerId == -1)
            {
                SelectedAnswer = null;
            }

            // Update navigation state
            UpdateNavigators();
            UpdateNextButtonText();

            OnPropertyChanged(nameof(CurrentQuestionNumber));
            OnPropertyChanged(nameof(CanGoPrevious));
            OnPropertyChanged(nameof(HasQuestionImage));
            OnPropertyChanged(nameof(AnsweredCount));
        }

        private void UpdateNavigators()
        {
            foreach (var nav in QuestionNavigators)
            {
                nav.IsCurrent = nav.QuestionIndex == CurrentQuestionIndex;
                nav.IsAnswered = UserAnswers.ContainsKey(Questions[nav.QuestionIndex].Id);
                nav.UpdateColors();
            }
        }

        private void UpdateNextButtonText()
        {
            if (IsReviewMode && CurrentQuestionIndex == Questions.Count - 1)
            {
                NextButtonText = "Hoàn thành";
            }
            else
            {
                NextButtonText = CurrentQuestionIndex == Questions.Count - 1 
                    ? "Nộp bài" 
                    : "Câu tiếp →";
            }
        }

        private AnswerViewModel CreateAnswerViewModel(Answer answer, bool isSelected, bool isCorrect)
        {
            return new AnswerViewModel
            {
                Id = answer.Id,
                Text = answer.Text,
                AnswerLetter = GetAnswerLetter(answer.Id),
                IsSelected = isSelected,
                IsCorrect = isCorrect,
                ShowResult = IsReviewMode
            };
        }

        private static string GetAnswerLetter(int id)
        {
            return id switch
            {
                1 => "A",
                2 => "B",
                3 => "C",
                4 => "D",
                _ => id.ToString()
            };
        }

        #region Timer

        public void StartTimer()
        {
            // Lazy initialize timer on first use
            if (_timer == null)
            {
                _timer = Application.Current!.Dispatcher.CreateTimer();
                _timer.Interval = TimeSpan.FromSeconds(1);
                _timer.Tick += OnTimerTick;
            }
            
            _timer.Start();
        }

        public void StopTimer()
        {
            _timer?.Stop();
        }

        private void OnTimerTick(object? sender, EventArgs e)
        {
            _remainingTime = _remainingTime.Subtract(TimeSpan.FromSeconds(1));

            if (_remainingTime <= TimeSpan.Zero)
            {
                StopTimer();
                AutoSubmitExam();
                return;
            }

            UpdateTimerDisplay();
        }

        private void UpdateTimerDisplay()
        {
            TimerDisplay = $"{(int)_remainingTime.TotalMinutes:D2}:{_remainingTime.Seconds:D2}";

            // Update timer colors based on remaining time
            if (_remainingTime.TotalMinutes <= 2)
            {
                // Critical - Red
                TimerBackgroundColor = Color.FromArgb("#FEE2E2");
                TimerTextColor = Color.FromArgb("#DC2626");
            }
            else if (_remainingTime.TotalMinutes <= 5)
            {
                // Warning - Orange
                TimerBackgroundColor = Color.FromArgb("#FEF3C7");
                TimerTextColor = Color.FromArgb("#D97706");
            }
            else
            {
                // Normal - Blue
                TimerBackgroundColor = Color.FromArgb("#DBEAFE");
                TimerTextColor = Color.FromArgb("#2563EB");
            }
        }

        private async void AutoSubmitExam()
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await Shell.Current.DisplayAlert(
                    "Hết giờ!",
                    "Thời gian làm bài đã hết. Bài thi sẽ được nộp tự động.",
                    "OK");
                
                SubmitExam();
            });
        }

        #endregion

        #region Commands

        [RelayCommand]
        private void SelectAnswer(AnswerViewModel? answer)
        {
            if (answer == null || CurrentQuestion == null) return;
            SelectAnswerInternal(answer);
        }

        [RelayCommand]
        private void SelectAnswerById(int answerId)
        {
            if (CurrentQuestion == null) return;
            
            var answerVm = CurrentAnswers.FirstOrDefault(a => a.Id == answerId);
            if (answerVm != null)
            {
                SelectAnswerInternal(answerVm);
            }
        }
        
        
        private void SelectAnswerInternal(AnswerViewModel answer)
        {
            if (CurrentQuestion == null || IsReviewMode) return;

            // Update selection state visually
            foreach (var a in CurrentAnswers)
            {
                a.IsSelected = (a.Id == answer.Id);
            }

            SelectedAnswer = answer;
            
            // Save answer
            UserAnswers[CurrentQuestion.Id] = answer.Id;

            // Update navigator
            UpdateNavigators();
            OnPropertyChanged(nameof(AnsweredCount));
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            bool shouldLoadQuestions = false;
            
            if (query.TryGetValue("action", out var action))
            {
                string actionStr = action.ToString() ?? "";
                if (actionStr == "review")
                {
                    // Restore backup if empty (New instance scenario)
                    // Ensure DataService lists are not null before accessing
                    var lastQuestions = _dataService.LastExamQuestions;
                    var lastAnswers = _dataService.LastUserAnswers;

                    if (Questions.Count == 0 && lastQuestions != null && lastQuestions.Count > 0)
                    {
                        Questions.Clear();
                        foreach (var q in lastQuestions) Questions.Add(q);
                        
                        UserAnswers.Clear();
                        if (lastAnswers != null)
                        {
                            foreach (var ua in lastAnswers) UserAnswers.Add(ua.Key, ua.Value);
                        }
                        
                        // Force UI Refresh
                        UpdateNavigators();
                    }
                    EnterReviewMode();
                }
                else if (actionStr == "reset")
                {
                    // Action: RETRY SAME EXAM
                    // If instance is new (Questions empty), try to restore from LastExamQuestions
                    if (Questions.Count == 0 && _dataService.LastExamQuestions != null && _dataService.LastExamQuestions.Count > 0)
                    {
                        Questions.Clear();
                        foreach (var q in _dataService.LastExamQuestions) Questions.Add(q);
                        
                        InitializeNavigators();
                        UpdateCurrentQuestion();
                        OnPropertyChanged(nameof(TotalQuestions));
                        OnPropertyChanged(nameof(CurrentQuestionNumber));
                    }
                    
                    // IF we still have no questions (e.g. app restart), then we MUST load new ones
                    if (Questions.Count == 0)
                    {
                        shouldLoadQuestions = true;
                    }
                    
                    ResetExam();
                }
            }

            
            if (query.TryGetValue("type", out var typeObj))
            {
                if (Enum.TryParse<LicenseType>(typeObj.ToString(), out var type))
                {
                    if (_currentLicenseType != type || Questions.Count == 0)
                    {
                        _currentLicenseType = type;
                        _initialTime = TimeSpan.FromMinutes(LicenseConfig.Get(type).ExamDurationMinutes);
                        _remainingTime = _initialTime; // Update Time
                        shouldLoadQuestions = true;
                    }
                }
            }
            
            // Fallback: Load questions if none exist and no action specified
            if (shouldLoadQuestions || (Questions.Count == 0 && !query.ContainsKey("action")))
            {
                MainThread.BeginInvokeOnMainThread(async () => await LoadQuestionsAsync());
            }
        }

        private void EnterReviewMode()
        {
            IsReviewMode = true;
            StopTimer();
            // Start review from first question
            JumpToQuestion(0);
        }

        private void ResetExam()
        {
            IsReviewMode = false;
            UserAnswers.Clear();
            // Reset timer
            _initialTime = TimeSpan.FromMinutes(LicenseConfig.Get(_currentLicenseType).ExamDurationMinutes);
            _remainingTime = _initialTime;
            StartTimer();
            
            JumpToQuestion(0);
        }

        [RelayCommand]
        private void PreviousQuestion()
        {
            if (CurrentQuestionIndex > 0)
            {
                CurrentQuestionIndex--;
                UpdateCurrentQuestion();
            }
        }

        [RelayCommand]
        private void NextQuestion()
        {
            if (CurrentQuestionIndex < Questions.Count - 1)
            {
                CurrentQuestionIndex++;
                UpdateCurrentQuestion();
            }
            else
            {
                if (IsReviewMode)
                {
                    Shell.Current.GoToAsync("..");
                }
                else
                {
                    ConfirmSubmitExam();
                }
            }
        }

        [RelayCommand]
        private void JumpToQuestion(int index)
        {
            if (index >= 0 && index < Questions.Count)
            {
                CurrentQuestionIndex = index;
                UpdateCurrentQuestion();
            }
        }

        private async void ConfirmSubmitExam()
        {
            var unanswered = Questions.Count - UserAnswers.Count;
            string message = unanswered > 0
                ? $"Bạn còn {unanswered} câu chưa trả lời. Bạn có chắc muốn nộp bài?"
                : "Bạn có chắc muốn nộp bài?";

            bool confirm = await Shell.Current.DisplayAlert(
                "Xác nhận nộp bài",
                message,
                "Nộp bài",
                "Tiếp tục làm");

            if (confirm)
            {
                await SubmitExam();
            }
        }

        private async Task SubmitExam()
        {
            try
            {
                StopTimer();

                var result = CalculateResult();

                // Save state for Review Page via DataService
                _dataService.LastExamResult = result;
                _dataService.LastExamQuestions = new List<Question>(Questions);
                _dataService.LastUserAnswers = new Dictionary<int, int>(UserAnswers);

                // Navigate to result page (Relative Route)
                await Shell.Current.GoToAsync($"ResultPage?correct={result.CorrectCount}&total={result.TotalCount}&paralysis={result.FailedParalysis}&passed={result.Passed}", true);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Lỗi nộp bài: {ex.Message}", "OK");
            }
        }

        private ExamResult CalculateResult()
        {
            int correctCount = 0;
            bool failedParalysis = false;
            var questionResults = new List<QuestionResult>();

            foreach (var question in Questions)
            {
                bool isCorrect = false;
                int selectedAnswerId = 0;
                
                if (UserAnswers.TryGetValue(question.Id, out int userAnswer))
                {
                    selectedAnswerId = userAnswer;
                    if (userAnswer == question.CorrectAnswerId)
                    {
                        correctCount++;
                        isCorrect = true;
                    }
                    else if (question.IsParalysis)
                    {
                        // Failed a paralysis question - immediate failure
                        failedParalysis = true;
                    }
                }
                else if (question.IsParalysis)
                {
                    // Unanswered paralysis question counts as wrong
                    failedParalysis = true;
                }

                // Add to question results for review
                questionResults.Add(new QuestionResult
                {
                    QuestionId = question.Id,
                    SelectedAnswerId = selectedAnswerId,
                    IsCorrect = isCorrect,
                    IsFlagged = false // TODO: Implement flagging feature if needed
                });
            }

            // Passing criteria: Correct >= PassingScore AND no failed paralysis questions
            var config = LicenseConfig.Get(_currentLicenseType);
            bool passed = !failedParalysis && (correctCount >= config.PassingScore);

            return new ExamResult
            {
                CorrectCount = correctCount,
                TotalCount = Questions.Count,
                FailedParalysis = failedParalysis,
                Passed = passed,
                QuestionResults = questionResults,
                TimeTaken = _initialTime - _remainingTime,
                CompletedAt = DateTime.Now
            };
        }

        #endregion
    }

    #region Supporting ViewModels

    public partial class AnswerViewModel : ObservableObject
    {
        [ObservableProperty]
        private int _id;

        [ObservableProperty]
        private string _text = string.Empty;

        [ObservableProperty]
        private string _answerLetter = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(BackgroundColor))]
        [NotifyPropertyChangedFor(nameof(BorderColor))]
        [NotifyPropertyChangedFor(nameof(BadgeBackgroundColor))]
        [NotifyPropertyChangedFor(nameof(BadgeBorderColor))]
        [NotifyPropertyChangedFor(nameof(BadgeTextColor))]
        private bool _isSelected;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(BackgroundColor))]
        [NotifyPropertyChangedFor(nameof(BorderColor))]
        [NotifyPropertyChangedFor(nameof(BadgeBackgroundColor))]
        [NotifyPropertyChangedFor(nameof(BadgeBorderColor))]
        [NotifyPropertyChangedFor(nameof(BadgeTextColor))]
        private bool _isCorrect;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(BackgroundColor))]
        [NotifyPropertyChangedFor(nameof(BorderColor))]
        [NotifyPropertyChangedFor(nameof(BadgeBackgroundColor))]
        [NotifyPropertyChangedFor(nameof(BadgeBorderColor))]
        [NotifyPropertyChangedFor(nameof(BadgeTextColor))]
        private bool _showResult;

        public Color BackgroundColor 
        {
            get
            {
                if (ShowResult)
                {
                    if (IsCorrect) return Color.FromArgb("#D1FAE5"); // SuccessLight
                    if (IsSelected && !IsCorrect) return Color.FromArgb("#FEE2E2"); // DangerLight
                    return Color.FromArgb("#FFFFFF"); // Surface
                }
                return IsSelected ? Color.FromArgb("#EFF6FF") : Color.FromArgb("#FFFFFF"); // PrimaryLighter / Surface
            }
        }

        public Color BorderColor
        {
            get
            {
                if (ShowResult)
                {
                    if (IsCorrect) return Color.FromArgb("#10B981"); // Success
                    if (IsSelected && !IsCorrect) return Color.FromArgb("#EF4444"); // Danger
                    return Color.FromArgb("#E2E8F0"); // Border
                }
                return IsSelected ? Color.FromArgb("#2563EB") : Color.FromArgb("#E2E8F0"); // Primary / Border
            }
        }

        public Color BadgeBackgroundColor 
        {
            get
            {
                if (ShowResult)
                {
                    if (IsCorrect) return Color.FromArgb("#10B981"); // Success
                    if (IsSelected && !IsCorrect) return Color.FromArgb("#EF4444"); // Danger
                    return Color.FromArgb("#F1F5F9"); // BorderLight
                }
                return IsSelected ? Color.FromArgb("#2563EB") : Color.FromArgb("#F1F5F9"); // Primary / BorderLight
            }
        }

        public Color BadgeBorderColor => BadgeBackgroundColor;

        public Color BadgeTextColor 
        {
            get
            {
                if (ShowResult)
                {
                    if (IsCorrect) return Colors.White;
                    if (IsSelected && !IsCorrect) return Colors.White;
                    return Color.FromArgb("#64748B"); // TextSecondary
                }
                return IsSelected ? Colors.White : Color.FromArgb("#64748B"); // TextSecondary
            }
        }

        partial void OnIsSelectedChanged(bool value) => NotifyColorsChanged();
        partial void OnIsCorrectChanged(bool value) => NotifyColorsChanged();
        partial void OnShowResultChanged(bool value) => NotifyColorsChanged();

        private void NotifyColorsChanged()
        {
            OnPropertyChanged(nameof(BackgroundColor));
            OnPropertyChanged(nameof(BorderColor));
            OnPropertyChanged(nameof(BadgeBackgroundColor));
            OnPropertyChanged(nameof(BadgeBorderColor));
            OnPropertyChanged(nameof(BadgeTextColor));
        }
    }

    public partial class QuestionNavigatorItem : ObservableObject
    {
        [ObservableProperty]
        private int _questionIndex;

        [ObservableProperty]
        private string _displayNumber = string.Empty;

        [ObservableProperty]
        private bool _isAnswered;

        [ObservableProperty]
        private bool _isCurrent;

        [ObservableProperty]
        private Color _backgroundColor = Color.FromArgb("#F1F5F9");

        [ObservableProperty]
        private Color _borderColor = Color.FromArgb("#E2E8F0");

        [ObservableProperty]
        private Color _textColor = Color.FromArgb("#64748B");

        public void UpdateColors()
        {
            if (IsCurrent)
            {
                // Current question - Primary blue with border
                BackgroundColor = Color.FromArgb("#2563EB");
                BorderColor = Color.FromArgb("#1D4ED8");
                TextColor = Colors.White;
            }
            else if (IsAnswered)
            {
                // Answered - Light blue
                BackgroundColor = Color.FromArgb("#DBEAFE");
                BorderColor = Color.FromArgb("#93C5FD");
                TextColor = Color.FromArgb("#1E40AF");
            }
            else
            {
                // Unanswered - Gray
                BackgroundColor = Color.FromArgb("#F1F5F9");
                BorderColor = Color.FromArgb("#E2E8F0");
                TextColor = Color.FromArgb("#64748B");
            }
        }
    }

    #endregion
}
