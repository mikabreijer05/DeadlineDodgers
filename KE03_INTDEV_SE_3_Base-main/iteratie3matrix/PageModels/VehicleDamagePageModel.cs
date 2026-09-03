using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using iteratie3matrix.Models;

namespace iteratie3matrix.PageModels;

public partial class VehicleDamagePageModel : ObservableObject
{
    private readonly DeliverySession _session;

    public VehicleDamagePageModel(DeliverySession session)
    {
        _session = session;

        ReportDate = DateTime.Now.ToString("dd-MM-yyyy");
        ReportTime = DateTime.Now.ToString("HH:mm");

        if (_session.SelectedVan != null)
        {
            VehicleRegistration =
                _session.SelectedVan.LicensePlate;
        }
    }

    [ObservableProperty]
    private string reportDate = string.Empty;

    [ObservableProperty]
    private string reportTime = string.Empty;

    [ObservableProperty]
    private string employeeName = string.Empty;

    [ObservableProperty]
    private string vehicleRegistration = string.Empty;

    [ObservableProperty]
    private string damageDescription = string.Empty;

    [ObservableProperty]
    private string photoPath = string.Empty;

    [RelayCommand]
    private async Task TakePhoto()
    {
        try
        {
            var photo = await MediaPicker.Default.CapturePhotoAsync();

            if (photo == null)
                return;

            var localFile =
                Path.Combine(
                    FileSystem.CacheDirectory,
                    photo.FileName);

            using var sourceStream =
                await photo.OpenReadAsync();

            using var localStream =
                File.OpenWrite(localFile);

            await sourceStream.CopyToAsync(localStream);

            PhotoPath = localFile;
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Camerafout",
                ex.Message,
                "OK");
        }
    }

    [RelayCommand]
    private async Task GoBack()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task SaveReport()
    {
        await Shell.Current.DisplayAlert(
            "Opgeslagen",
            "Schaderapport is opgeslagen.",
            "OK");

        await Shell.Current.GoToAsync("//vehicleinspection");
    }
}