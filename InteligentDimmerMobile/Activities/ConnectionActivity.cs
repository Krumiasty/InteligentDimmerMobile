using Android.App;
using Android.Widget;
using Android.OS;
using System;
using Android.Content;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Android.Bluetooth;
using Android.Content.PM;
using Android.Content.Res;
using Android.Transitions;
using Android.Views;
using InteligentDimmerMobile.Adapters;
using InteligentDimmerMobile.Configuration;
using InteligentDimmerMobile.Model;
using InteligentDimmerMobile.Services;
using Java.Lang;
using Java.Util;

namespace InteligentDimmerMobile.Activities
{
    [Activity(Theme = "@android:style/Theme.Material.Light.NoActionBar",
        ScreenOrientation = ScreenOrientation.Portrait)]
    public class ConnectionActivity : Activity
    {
        private bool _wasBluetoothEnabled = true;
        private bool _isFirstRun = true;

        private Button _scanButton;
        private ListView _devicesList;
        private Button _connectButton;
        private ProgressBar _busyIndicator;
        private LinearLayout _busyIndicatorLinearLayout;

        private BluetoothAdapter _bluetoothAdapter;
        private BluetoothDevice _pickedDevice;
        private BluetoothSocket _socket;

        ObservableCollection<BluetoothDevice> _bluetoothDevices =
            new ObservableCollection<BluetoothDevice>();

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.ConnectionLayout);
            SetupBindings();

            #region testing
            //var controlActivity = new Intent(this, typeof(ControlActivity));
            //controlActivity.PutExtra("DeviceName", "Test");
            ////  controlActivity.PutExtra("DeviceName", _pickedDevice.Name);
            ////  controlActivity.PutExtra("DeviceMac", _pickedDevice.Address);
            //StartActivity(controlActivity);
            #endregion

        }

        protected override void OnStart()
        {
            base.OnStart();
            if (_isFirstRun)
            {
                FirstScan();
                HideLoadingIndicator();
                _isFirstRun = false;
            }
        }

        private void SetupBindings()
        {
            _busyIndicatorLinearLayout = FindViewById<LinearLayout>(Resource.Id.busyIndicatorLinearLayout);
            _busyIndicator = FindViewById<ProgressBar>(Resource.Id.busyIndicator);
            _scanButton = FindViewById<Button>(Resource.Id.searchButton);
            _devicesList = FindViewById<ListView>(Resource.Id.devicesList);
            _connectButton = FindViewById<Button>(Resource.Id.connectButton);

            _scanButton.Click += ScanOnClick;
            _devicesList.ItemClick += OnItemClick;
            _connectButton.Click += OnConnectButtonClick;
        }

        private async void OnConnectButtonClick(object sender, EventArgs args)
        {
            _devicesList.ClearChoices();
            _connectButton.Clickable = false;
            _busyIndicator.Visibility = ViewStates.Visible;
            _busyIndicatorLinearLayout.Visibility = ViewStates.Visible;

            await Task.Run( () =>
            {
                Task.Delay(3000);
            });

            if (_pickedDevice != null)
            {
                try
                {
#if DEBUG
                    //HideLoadingIndicator();
                    //var controlActivity = new Intent(this, typeof(ControlActivity));
                    //controlActivity.PutExtra("DeviceName", _pickedDevice.Name);
                    //controlActivity.PutExtra("DeviceMac", _pickedDevice.Address);
                    //StartActivity(controlActivity);
#endif

                    _socket =
                         _pickedDevice.CreateInsecureRfcommSocketToServiceRecord(
                             UUID.FromString(Constants.BluetoothUUID));
                    //_pickedDevice.CreateRfcommSocketToServiceRecord(UUID.FromString(Constants.BluetoothUUID));
                    //CreateRfcommSocketToServiceRecord(Java.Util.UUID.FromString("00001101-0000-1000-8000-00805f9b34fb"));
                }
                catch (Java.Lang.Exception e)
                {
                    Toast.MakeText(this, "Error opening port\nPlease try again", ToastLength.Short).Show();
                    HideLoadingIndicator();
                    return;
                }
                try
                {
                    //await _socket.ConnectAsync();
                    await Task.Run(async () =>
                    {
                        await _socket.ConnectAsync();
                    });
                }
                catch (Java.Lang.Exception e)
                {
                    Toast.MakeText(this, "Error connecting with device\nPlease try again", ToastLength.Short).Show();
                    HideLoadingIndicator();
                    return;
                }
                // Write data to the device
                // SendData();
                // PrepareData(0x00, 0x00);
                //await _socket.OutputStream.WriteAsync()
                PrepareDataService.PrepareData(0x01, 0x00, 0x00);
                try
                {
                    await _socket.OutputStream.WriteAsync(new byte[]
                    {
                        ControlData.StartByte,
                        ControlData.CommandByte,
                        ControlData.SeparatorByte1,
                        ControlData.DataByte1,
                        ControlData.SeparatorByte2,
                        ControlData.DataByte2,
                        ControlData.EndByte
                    }, 0, Constants.BytesNumber);
                }
                catch (Java.Lang.Exception e)
                {
                    HideLoadingIndicator();
                }

                //   byte[] response = new[] { (byte)0 };
                //   await _socket.InputStream.ReadAsync(response, 0, 1);
                try
                {
                    await Task.Delay(2000);
                    _socket.Close();
                    await Task.Delay(1000);
                }
                catch (Java.Lang.Exception e)
                {
                    Toast.MakeText(this, "Error closing socket\nPlease try again", ToastLength.Short).Show();
                    HideLoadingIndicator();
                    return;
                }

                #region release
                var controlActivity = new Intent(this, typeof(ControlActivity));
                controlActivity.PutExtra("DeviceName", _pickedDevice.Name);
                controlActivity.PutExtra("DeviceMac", _pickedDevice.Address);
                StartActivity(controlActivity);
                #endregion

                //// _socket.OutputStream.Close();

                //    byte[] response = new []{ (byte)0 };
                //    if (response[0] == (byte)0)
                //    {
                //_socket.InputStream.Close();
                //var controlActivity = new Intent(this, typeof(ControlActivity));
                //controlActivity.PutExtra("DeviceName", _pickedDevice.Name);
                //controlActivity.PutExtra("DeviceMac", _pickedDevice.Address);
                //StartActivity(controlActivity);
                //    }
            }
            HideLoadingIndicator();
        }

        private void HideLoadingIndicator()
        {
            _busyIndicator.Visibility = ViewStates.Gone;
            _busyIndicatorLinearLayout.Visibility = ViewStates.Gone;
            _connectButton.Clickable = true;
        }

        private void SendData()
        {
        //        Response = null;
            _socket.OutputStream.Write(new byte[]
             {
                    ControlData.StartByte,
                    ControlData.CommandByte,
                    ControlData.SeparatorByte1,
                    ControlData.DataByte1,
                    ControlData.SeparatorByte2,
                    ControlData.DataByte2,
                    ControlData.EndByte
             }, 0, 7);
        }

        private async Task FirstScan()
        {
            _busyIndicator.Visibility = ViewStates.Visible;
            _busyIndicatorLinearLayout.Visibility = ViewStates.Visible;

            await Task.Delay(500);

            _bluetoothAdapter = BluetoothAdapter.DefaultAdapter;

            if (_bluetoothAdapter == null)
            {
                return;
            }

            if (!_bluetoothAdapter.IsEnabled)
            {
                _wasBluetoothEnabled = false;
                await Task.Run(async () =>
                {
                    _bluetoothAdapter.Enable();
                    await Task.Delay(2000);
                });
            }
            var pairedDevices = _bluetoothAdapter.BondedDevices;

            //await Task.Run(() =>
            //{
            //    _pickedDevice = _bluetoothAdapter.BondedDevices
            //                                    .FirstOrDefault(x => x.Address == "20:16:09:06:39:89");
            //});

            //if (_pickedDevice != null)
            //{
            //    HideLoadingIndicator();
            //    var controlActivity = new Intent(this, typeof(ControlActivity));
            //    var options = ActivityOptions.MakeSceneTransitionAnimation(this);

            //    controlActivity.PutExtra("DeviceName", _pickedDevice.Name);
            //    controlActivity.PutExtra("DeviceMac", _pickedDevice.Address);
            //    StartActivity(controlActivity, options.ToBundle());
            //}

            foreach (var pairedDevice in pairedDevices)
            {
                _bluetoothDevices.Add(pairedDevice);
            }
            _devicesList.Adapter = new BluetoothsAdapter(_bluetoothDevices);
            _busyIndicator.Visibility = ViewStates.Gone;
        }

        private void OnItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            _pickedDevice = _bluetoothDevices[e.Position];

            Toast.MakeText(this, _pickedDevice.Name, ToastLength.Short).Show();
        }

        private async void ScanOnClick(object sender, EventArgs eventArgs)
        {
            _bluetoothDevices = new ObservableCollection<BluetoothDevice>();
            _pickedDevice = null;

            _bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
            if (_bluetoothAdapter == null)
            {
                Toast.MakeText(this, "Wrong device", ToastLength.Short).Show();
                return;
            }
            if (!_bluetoothAdapter.IsEnabled)
            {
                _bluetoothAdapter.Enable();
            }

            _bluetoothAdapter.StartDiscovery();

            await Task.Run(() =>
            {
                 Task.Delay(2000);
                _bluetoothAdapter.CancelDiscovery();
            });

            var pairedDevices = _bluetoothAdapter.BondedDevices;
            foreach (var pairedDevice in pairedDevices)
            {
                _bluetoothDevices.Add(pairedDevice);
            }
            _devicesList.Adapter = new BluetoothsAdapter(_bluetoothDevices);
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            if (!_wasBluetoothEnabled)
            {
                _bluetoothAdapter.Disable();
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            _devicesList.Adapter = new BluetoothsAdapter(_bluetoothDevices);
            _pickedDevice = null;
        }
    }
}
