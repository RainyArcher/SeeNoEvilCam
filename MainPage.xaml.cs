using Camera.MAUI;
using ImageFormat = Camera.MAUI.ImageFormat;

namespace SeeNoEvilCam;

public partial class MainPage : ContentPage
{
	private readonly string _directoryLocation = "/storage/emulated/0/DCIM/NoEvilArchive";
	
	public MainPage()
	{
		InitializeComponent();
		cameraView.Opacity = 0.0;
		cameraView.CamerasLoaded += CameraView_CamerasLoaded;
	}
	
	private void CameraView_CamerasLoaded(object sender, EventArgs e)
	{
		if (cameraView.Cameras.Count > 0)
		{
			cameraView.Camera = cameraView.Cameras.First();
			try
			{
				LoadCamera();
			}
			catch
			{
				Shell.Current.DisplayAlert("Unable to start the camera", "Please, check the camera permission and try again", "OK",
					FlowDirection.LeftToRight);
			}
		}
	}
	
	private void LoadCamera()
	{
		MainThread.BeginInvokeOnMainThread(async () =>
		{
			if (await cameraView.StartCameraAsync() != CameraResult.Success)
			{
				throw new Exception("Unable to start the camera");
			}
		});
	}
	
	private async void SnapAction(object sender, EventArgs e)
	{
		try
		{
			CreateDirectoryIfNeeded();
			await cameraView.SaveSnapShot(ImageFormat.PNG, Path.Combine(_directoryLocation, $"NoEvilSnap_{DateTime.Now.Ticks}.png"));
		}
		catch
		{
			Shell.Current.DisplayAlert("Permission Denied", "Failed to save the snap", "OK",
				FlowDirection.LeftToRight);
		}
	}

	private void ToggleCamera(object sender, EventArgs e)
	{
		if (cameraView.Cameras.Count>1)
		{
			cameraView.Camera = cameraView.Camera == cameraView.Cameras.First() ? cameraView.Cameras[1] : cameraView.Cameras.First();
			try
			{
				LoadCamera();
			}
			catch
			{
				Shell.Current.DisplayAlert("Unable to start the camera", "Please, check the camera permission and try again", "OK",
					FlowDirection.LeftToRight);
			}
		}
	}
	private void TogglePreviewVisibility(object sender, EventArgs e)
	{
		cameraView.Opacity = cameraView.Opacity == 1 ? 0 : 1;
	}

	private void CreateDirectoryIfNeeded()
	{
		if (!Directory.Exists(_directoryLocation))
		{
			Directory.CreateDirectory(_directoryLocation);
		}
	}
}

