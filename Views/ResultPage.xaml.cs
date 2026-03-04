using TracNghiemLaiXe.ViewModels;

namespace TracNghiemLaiXe.Views;

public partial class ResultPage : ContentPage
{
    public ResultPage(ResultViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
