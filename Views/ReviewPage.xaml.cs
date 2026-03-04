using Microsoft.Maui.Controls;
using TracNghiemLaiXe.ViewModels;

namespace TracNghiemLaiXe.Views
{
    public partial class ReviewPage : ContentPage
    {
        private readonly ReviewViewModel _viewModel;

        public ReviewPage(ReviewViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);
            // ViewModel listening to Messenger handles the initialization if message sent.
            // But relying on DataService state is safer for direct navigation.
            _ = _viewModel.InitializeAsync();
        }

        protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
        {
            base.OnNavigatedFrom(args);
            // Trigger cleanup when leaving the page to free memory
            _viewModel.Cleanup();
        }
    }
}
