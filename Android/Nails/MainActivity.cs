using System;
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using Android.Content.PM;
using Plugin.Permissions;
using System.Threading.Tasks;
using Android.Views;

namespace Nails
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true
        , ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity
    {
        readonly MainConroller mainConroller = new MainConroller();

        Button processNewPhotoButton;
        private Button ProcessNewPhotoButton => processNewPhotoButton ?? (processNewPhotoButton = FindViewById<Button>(Resource.Id.process_new_photo_button));

        Button sendPhotoButton;
        private Button SendPhotoButton => sendPhotoButton ?? (sendPhotoButton = FindViewById<Button>(Resource.Id.send_photo_button));

        ImageView photoView;
        private ImageView PhotoView => photoView ?? (photoView = FindViewById<ImageView>(Resource.Id.photo));

        ProgressBar progressBar;
        private ProgressBar ProgressBar => progressBar ?? (progressBar = FindViewById<ProgressBar>(Resource.Id.progressbar));

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            ProcessNewPhotoButton.Click += ProcessNewPhotoButton_Click;
            SendPhotoButton.Click += SendPhotoButton_Click;
            SendPhotoButton.Enabled = !mainConroller.ImageIsSent;
            CheckVersions();
        }

        private async Task CheckVersions()
        {
            //await CheckNewVersion();
            await CheckModel();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults) => PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);

        async void ProcessNewPhotoButton_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessNewPhotoButton.Enabled = false;
                SendPhotoButton.Enabled = false;
                ProgressBar.Visibility = ViewStates.Visible;

                mainConroller.ImageWidth = PhotoView.Width;
                var result = await Task.Run(() => mainConroller.ProcessNewImage());
                PhotoView.SetImageBitmap(result);
                Toast.MakeText(this
                    , "PreprocessTime: " + mainConroller.PreprocessTime.TotalSeconds
                    + System.Environment.NewLine + "SegmentationTime: " + mainConroller.SegmentationTime.TotalSeconds
                    + System.Environment.NewLine + "PostprocessTime: " + mainConroller.PostprocessTime.TotalSeconds
                    , ToastLength.Long).Show();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }
            finally
            {
                ProcessNewPhotoButton.Enabled = true;
                ProgressBar.Visibility = ViewStates.Invisible;
                SendPhotoButton.Enabled = !mainConroller.ImageIsSent;
            }
        }

        async void SendPhotoButton_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessNewPhotoButton.Enabled = false;
                SendPhotoButton.Enabled = false;
                ProgressBar.Visibility = ViewStates.Visible;

                if (mainConroller.ImageIsSent)
                {
                    return;
                }
                await Task.Run(() => mainConroller.SendImage());
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }
            finally
            {
                ProcessNewPhotoButton.Enabled = true;
                ProgressBar.Visibility = ViewStates.Invisible;
                SendPhotoButton.Enabled = !mainConroller.ImageIsSent;
            }
        }

        private async Task CheckNewVersion()
        {
            try
            {
                ProcessNewPhotoButton.Enabled = false;
                SendPhotoButton.Enabled = false;
                ProgressBar.Visibility = ViewStates.Visible;

                if (await mainConroller.IsNewVersionAvailable())
                {
                    AskForDownloadNewVersion();
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }
            finally
            {
                ProcessNewPhotoButton.Enabled = true;
                SendPhotoButton.Enabled = !mainConroller.ImageIsSent;
                ProgressBar.Visibility = ViewStates.Invisible;
            }
        }


        private async void DownloadNewVersion()
        {
            try
            {
                ProcessNewPhotoButton.Enabled = false;
                SendPhotoButton.Enabled = false;
                ProgressBar.Visibility = ViewStates.Visible;
                await mainConroller.DownloadNewVersion();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }
            finally
            {
                ProcessNewPhotoButton.Enabled = true;
                SendPhotoButton.Enabled = !mainConroller.ImageIsSent;
                ProgressBar.Visibility = ViewStates.Invisible;
            }
        }

        private void AskForDownloadNewVersion()
        {
            var dialog = new Android.App.AlertDialog.Builder(this);
            Android.App.AlertDialog alert = dialog.Create();
            alert.SetMessage("New version available.");
            alert.SetButton("Cancel", (c, ev) => { });
            alert.SetButton2("Download", (c, ev) => { DownloadNewVersion(); });
            alert.Show();
        }

        private async Task CheckModel()
        {
            try
            {
                ProcessNewPhotoButton.Enabled = false;
                SendPhotoButton.Enabled = false;
                ProgressBar.Visibility = ViewStates.Visible;

                if (await mainConroller.IsNewModelAvailable())
                {
                    AskForDownloadModel();
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }
            finally
            {
                ProcessNewPhotoButton.Enabled = true;
                SendPhotoButton.Enabled = !mainConroller.ImageIsSent;
                ProgressBar.Visibility = ViewStates.Invisible;
            }
        }
        
        private async void DownloadModel()
        {
            try
            {
                ProcessNewPhotoButton.Enabled = false;
                SendPhotoButton.Enabled = false;
                ProgressBar.Visibility = ViewStates.Visible;
                await mainConroller.DownloadNewModel();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }
            finally
            {
                ProcessNewPhotoButton.Enabled = true;
                SendPhotoButton.Enabled = !mainConroller.ImageIsSent;
                ProgressBar.Visibility = ViewStates.Invisible;
            }
        }

        private void AskForDownloadModel()
        {
            var dialog = new Android.App.AlertDialog.Builder(this);
            Android.App.AlertDialog alert = dialog.Create();
            alert.SetMessage("New model available.");
            alert.SetButton("Cancel", (c, ev) => { });
            alert.SetButton2("Download", (c, ev) => { DownloadModel(); });
            alert.Show();
        }
    }
}