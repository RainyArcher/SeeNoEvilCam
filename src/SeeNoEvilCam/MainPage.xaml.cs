using Camera.MAUI;
using ImageFormat = Camera.MAUI.ImageFormat;

namespace SeeNoEvilCam;

public partial class MainPage : ContentPage
{
	private readonly string _directoryLocation;
	private bool _isRecording;
	
	public MainPage()
	{
		InitializeComponent();
		cameraView.Opacity = 0.0;
		cameraView.CamerasLoaded += CameraView_CamerasLoaded;
		_directoryLocation = "/storage/emulated/0/DCIM/NoEvilArchive";
		_isRecording = false;
	}
	
	private void CameraView_CamerasLoaded(object sender, EventArgs e)
	{
		if (cameraView.Cameras.Count > 0)
		{
			cameraView.Camera = cameraView.Cameras.First();
			if (cameraView.NumMicrophonesDetected > 0)
				cameraView.Microphone = cameraView.Microphones.First();
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
			await PermissionManager.CheckAndRequestCameraPermissions();
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
	private async void ToggleRecordAction(object sender, EventArgs e)
	{
		if (!_isRecording)
		{
			try
			{
				CreateDirectoryIfNeeded();
				_isRecording = true;
				var result = await cameraView.StartRecordingAsync(Path.Combine(_directoryLocation, $"NoEvilRecord_{DateTime.Now.Ticks}.mp4"), new Size(1088, 1088));
				if (!(result == CameraResult.Success))
				{
					Shell.Current.DisplayAlert("Permission Denied", "Failed to save the record - permission is denied", "OK",
						FlowDirection.LeftToRight);
				}
			}
			catch
			{
				StopRecordingIfNeeded();
				Shell.Current.DisplayAlert("Permission Denied", "Failed to save the record - permission is denied", "OK",
					FlowDirection.LeftToRight);
			}
		}
		else
		{
			StopRecordingIfNeeded();
		}
	}
	
	private async void ToggleCamera(object sender, EventArgs e)
	{
		StopRecordingIfNeeded();
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
	
	private async void StopRecordingIfNeeded()
	{
		if (_isRecording)
		{
			_isRecording = false;
			await cameraView.StopRecordingAsync();
		}
	}
	private void CreateDirectoryIfNeeded()
	{
		if (!Directory.Exists(_directoryLocation))
		{
			Directory.CreateDirectory(_directoryLocation);
		}
	}
}

