using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using CsvHelper;
using Xamarin.Essentials;

namespace ReadSensors
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;
        }

        private AccelerometerTest _accelerometerTest;
        private GyroscopeTest _gyroscopeTest;
        private MagnetometerTest _magnetometerTest;
        private CameraTest _cameraTest;
        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            base.OnPostCreate(savedInstanceState);
            CheckPermissions();

            _accelerometerTest = new AccelerometerTest();
            var error = _accelerometerTest.Toggle();
            if (error != null)
            {
                ShowError(error.Message);
            }

            _gyroscopeTest = new GyroscopeTest();
            error = _gyroscopeTest.Toggle();
            if (error != null)
            {
                ShowError(error.Message);
            }

            _magnetometerTest = new MagnetometerTest();
            error = _magnetometerTest.Toggle();
            if (error != null)
            {
                ShowError(error.Message);
            }

            _cameraTest = new CameraTest();
        }

        private void ShowError(string message)
        {
            var alert = new Android.App.AlertDialog.Builder(this);
            alert.SetTitle("Error");
            alert.SetMessage(message);
            Dialog dialog = alert.Create();
            dialog.Show();
        }
        private async void CheckPermissions()
        {
            var permissionStatus = await Permissions.CheckStatusAsync<Permissions.Microphone>();
            if (permissionStatus != PermissionStatus.Granted)
            {
                permissionStatus = await Permissions.RequestAsync<Permissions.Microphone>();
                if (permissionStatus != PermissionStatus.Granted)
                {
                    ShowError("Permission Microphone status denied");
                }
            }

            permissionStatus = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (permissionStatus != PermissionStatus.Granted)
            {
                permissionStatus = await Permissions.RequestAsync<Permissions.Camera>();
                if (permissionStatus != PermissionStatus.Granted)
                {
                    ShowError("Permission Camera status denied");
                }
            }

            permissionStatus = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
            if (permissionStatus != PermissionStatus.Granted)
            {
                permissionStatus = await Permissions.RequestAsync<Permissions.StorageWrite>();
                if (permissionStatus != PermissionStatus.Granted)
                {
                    ShowError("Permission StorageWrite status denied");
                }
            }

            permissionStatus = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
            if (permissionStatus != PermissionStatus.Granted)
            {
                permissionStatus = await Permissions.RequestAsync<Permissions.StorageRead>();
                if (permissionStatus != PermissionStatus.Granted)
                {
                    ShowError("Permission StorageRead status denied");
                }
            }

            permissionStatus = await Permissions.CheckStatusAsync<Permissions.Media>();
            if (permissionStatus != PermissionStatus.Granted)
            {
                permissionStatus = await Permissions.RequestAsync<Permissions.Media>();
                if (permissionStatus != PermissionStatus.Granted)
                {
                    ShowError("Permission Media status denied");
                }
            }

            permissionStatus = await Permissions.CheckStatusAsync<Permissions.Sensors>();
            if (permissionStatus != PermissionStatus.Granted)
            {
                permissionStatus = await Permissions.RequestAsync<Permissions.Sensors>();
                if (permissionStatus != PermissionStatus.Granted)
                {
                    ShowError("Permission Sensors status denied");
                }
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        public class Coords 
        {
            public long Time { get; set; }
            public float AccelerometerX { get; set; }
            public float AccelerometerY { get; set; }
            public float AccelerometerZ { get; set; }
            public float GyroscopeX { get; set; }
            public float GyroscopeY { get; set; }
            public float GyroscopeZ { get; set; }
            public float MagnetometerX { get; set; }
            public float MagnetometerY { get; set; }
            public float MagnetometerZ { get; set; }
        }
        
        private bool _stop = true;
        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            if (_stop)
            {
                _stop = !_stop;
                SurfaceView surfaceView = FindViewById<SurfaceView>(Resource.Id.surfaceView);
                _cameraTest.Init(surfaceView.Holder.Surface);
                var time = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeMilliseconds();
                _cameraTest.Start(time.ToString() + ".3gp");

                Task.Run(() =>
                {
                    var accelerometer = _accelerometerTest.Data;
                    var gyroscope = _gyroscopeTest.Data;
                    var magnetometer = _magnetometerTest.Data;
                    string path = Path.Combine(_cameraTest.GetDir(), time.ToString() + ".csv");
                    using var streamWriter = new StreamWriter(path);
                    using var cvsWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);
                    while (!_stop)
                    {                        
                        Coords coord = new Coords()
                        {
                            Time = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeMilliseconds(),

                            AccelerometerX = accelerometer.X,
                            AccelerometerY = accelerometer.Y,
                            AccelerometerZ = accelerometer.Z,

                            GyroscopeX = gyroscope.X,
                            GyroscopeY = gyroscope.Y,
                            GyroscopeZ = gyroscope.Z,

                            MagnetometerX = magnetometer.X,
                            MagnetometerY = magnetometer.Y,
                            MagnetometerZ = magnetometer.Z,
                        };
                        cvsWriter.WriteRecord(coord);
                    }
                    cvsWriter.Flush();
                });

            } 
            else
            {
                _stop = !_stop;
                _cameraTest.Stop();
            }
            
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
	}
}
