namespace TracNghiemLaiXe;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Register routes for navigation
        Routing.RegisterRoute("MainPage", typeof(Views.MainPage));
        Routing.RegisterRoute("ResultPage", typeof(Views.ResultPage));
        Routing.RegisterRoute("QuizPage", typeof(Views.QuizPage));
        Routing.RegisterRoute("ReviewPage", typeof(Views.ReviewPage));
    }
}
