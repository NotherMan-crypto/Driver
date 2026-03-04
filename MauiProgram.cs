using Microsoft.Extensions.Logging;
using TracNghiemLaiXe.ViewModels;
using TracNghiemLaiXe.Views;
using TracNghiemLaiXe.Services;

namespace TracNghiemLaiXe
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<IExamGeneratorService, ExamGeneratorService>();
            builder.Services.AddSingleton<IDataService, DataService>();
            builder.Services.AddSingleton<ImageService>();

            // Register ViewModels
            builder.Services.AddTransient<QuizViewModel>();
            builder.Services.AddTransient<ReviewViewModel>();
            builder.Services.AddTransient<ResultViewModel>();

            // Register Views
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<QuizPage>();
            builder.Services.AddTransient<ResultPage>();
            builder.Services.AddTransient<ReviewPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
