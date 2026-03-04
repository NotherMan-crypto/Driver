using System;
using Microsoft.Maui.Controls;

namespace TracNghiemLaiXe.Views;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

    private async void OnStartClicked(object sender, EventArgs e)
    {
        if (LicensePicker.SelectedItem == null)
        {
            await DisplayAlert("Chú ý", "Vui lòng chọn hạng bằng lái!", "OK");
            return;
        }

        try
        {
            var selectedItem = LicensePicker.SelectedItem.ToString();
            await Shell.Current.GoToAsync($"QuizPage?type={selectedItem}");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Lỗi", $"Không thể bắt đầu: {ex.Message}", "OK");
        }
    }
}
