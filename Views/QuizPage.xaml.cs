namespace TracNghiemLaiXe.Views;

public partial class QuizPage : ContentPage
{
    public QuizPage(ViewModels.QuizViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ViewModels.QuizViewModel vm)
        {
            vm.StartTimer();
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        if (BindingContext is ViewModels.QuizViewModel vm)
        {
            vm.StopTimer();
        }
    }
}
