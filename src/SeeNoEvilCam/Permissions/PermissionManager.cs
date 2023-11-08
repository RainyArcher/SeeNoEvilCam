using SeeNoEvilCam;

public static class PermissionManager
{
    public static async Task CheckAndRequestCameraPermissions()
    {
        PermissionStatus status = await Permissions.CheckStatusAsync<CameraPermissions>();
        if (status == PermissionStatus.Granted)
            return;
        status = await Permissions.RequestAsync<CameraPermissions>();
        if (status != PermissionStatus.Granted)
        {
            var answer = await Shell.Current.DisplayAlert("Permissions are required", "Grant all required permissions, otherwise the camera will not work", "Grant", "Quit");
            if (answer)
            {
                status = await Permissions.RequestAsync<CameraPermissions>();
                if (status != PermissionStatus.Granted)
                {
                    await Shell.Current.DisplayAlert("Permissions are denied","Go to the SeeNoEvilCam application settings to grant all required permissions. See you then", "OK");
                    App.Current.Quit();
                }
            }
            else
            {
                App.Current.Quit();
            }
        }
    }
}