using CommunityToolkit.Mvvm.ComponentModel;

namespace TracNghiemLaiXe.ViewModels
{
    /// <summary>
    /// Base class for all ViewModels providing common functionality.
    /// </summary>
    public abstract partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        private bool _isBusy;

        [ObservableProperty]
        private string _title = string.Empty;

        public bool IsNotBusy => !IsBusy;

        /// <summary>
        /// Shows an alert dialog.
        /// </summary>
        protected static Task ShowAlertAsync(string title, string message, string cancel = "OK")
        {
            return Shell.Current.DisplayAlert(title, message, cancel);
        }

        /// <summary>
        /// Shows a confirmation dialog.
        /// </summary>
        protected static Task<bool> ShowConfirmAsync(string title, string message, string accept = "OK", string cancel = "Hủy")
        {
            return Shell.Current.DisplayAlert(title, message, accept, cancel);
        }

        /// <summary>
        /// Navigates to a route.
        /// </summary>
        protected static Task GoToAsync(string route)
        {
            return Shell.Current.GoToAsync(route);
        }

        /// <summary>
        /// Navigates back.
        /// </summary>
        protected static Task GoBackAsync()
        {
            return Shell.Current.GoToAsync("..");
        }
    }
}
