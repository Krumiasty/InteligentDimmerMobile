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
using InteligentDimmerMobile.Adapters;
using InteligentDimmerMobile.Configuration;
using InteligentDimmerMobile.Model;
using InteligentDimmerMobile.Services;
using Java.Lang;
using Java.Util;

namespace InteligentDimmerMobile.Activities
{
    [Activity(Label = "Dimmer", Icon = "@drawable/sun", Theme = "@style/AppTheme", 
        ScreenOrientation = ScreenOrientation.Portrait, NoHistory = true)]
    public class MainActivity : Activity
    {
        private Button _scanButton;
        private ListView _devicesList;
        private Button _connectButton;

        private BluetoothAdapter btAdapter;
        private BluetoothDevice pickedDevice = null;
        private BluetoothSocket _socket;

        ObservableCollection<BluetoothDevice> bluetoothDevices =
            new ObservableCollection<BluetoothDevice>();

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            SetupBindings();

            #region testing
            var controlActivity = new Intent(this, typeof(ControlActivity));
            controlActivity.PutExtra("DeviceName", "Test");
            //  controlActivity.PutExtra("DeviceName", pickedDevice.Name);
            //  controlActivity.PutExtra("DeviceMac", pickedDevice.Address);
            StartActivity(controlActivity);
            #endregion

            FirstScan();
        }

        private void SetupBindings()
        {
            _scanButton = FindViewById<Button>(Resource.Id.searchButton);
            _devicesList = FindViewById<ListView>(Resource.Id.devicesList);
            _connectButton = FindViewById<Button>(Resource.Id.connectButton);

            _scanButton.Click += ScanOnClick;
            _devicesList.ItemClick += OnItemClick;
            _connectButton.Click += OnConnectButtonOnClick;
        }

        private async void OnConnectButtonOnClick(object sender, EventArgs args)
        {
            if (pickedDevice != null)
            {
                try
                {
                    _socket =
                        // pickedDevice.CreateInsecureRfcommSocketToServiceRecord(
                        //     UUID.FromString(Constants.BluetoothUUID));
                        pickedDevice.CreateRfcommSocketToServiceRecord(UUID.FromString(Constants.BluetoothUUID));
                    //CreateRfcommSocketToServiceRecord(Java.Util.UUID.FromString("00001101-0000-1000-8000-00805f9b34fb"));
                }
                catch (Java.Lang.Exception e)
                {
                    Toast.MakeText(this, "Error opening port\nPlease try again", ToastLength.Short).Show();
                    return;
                }
                try
                {
                    System.Threading.Thread.Sleep(1000);
                    await _socket.ConnectAsync();
                    System.Threading.Thread.Sleep(1000);
                }
                catch (Java.Lang.Exception e)
                {
                    Toast.MakeText(this, "Error connecting with device\nPlease try again", ToastLength.Short).Show();
                    return;
                    ;
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
                        ControlData.StartByte, ControlData.CommandByte, ControlData.SeparatorByte1, ControlData.DataByte1, ControlData.SeparatorByte2, ControlData.DataByte2, ControlData.EndByte
                    }, 0, 7);
                }
                catch (Java.Lang.Exception e)
                {
                }

                //   byte[] response = new[] { (byte)0 };
                //   await _socket.InputStream.ReadAsync(response, 0, 1);
                try
                {
                    _socket.Close();
                }
                catch (Java.Lang.Exception e)
                {
                    Toast.MakeText(this, "Error closing socket\nPlease try again", ToastLength.Short).Show();
                    return;
                }

                #region release

                //var controlActivity = new Intent(this, typeof(ControlActivity));
                //controlActivity.PutExtra("DeviceName", pickedDevice.Name);
                //controlActivity.PutExtra("DeviceMac", pickedDevice.Address);
                //StartActivity(controlActivity);

                #endregion


                //// _socket.OutputStream.Close();


                //    byte[] response = new []{ (byte)0 };
                //    if (response[0] == (byte)0)
                //    {
                //_socket.OutputStream.Close();
                //_socket.InputStream.Close();
                //var controlActivity = new Intent(this, typeof(ControlActivity));
                //controlActivity.PutExtra("DeviceName", pickedDevice.Name);
                //controlActivity.PutExtra("DeviceMac", pickedDevice.Address);
                //StartActivity(controlActivity);
                //    }
            }
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

        private void FirstScan()
        {
            btAdapter = BluetoothAdapter.DefaultAdapter;
            if (btAdapter == null)
            {
                return;
            }
            if (!btAdapter.IsEnabled)
            {
                btAdapter.Enable();
            }
            var pairedDevices = btAdapter.BondedDevices;
            foreach (var pairedDevice in pairedDevices)
            {
                bluetoothDevices.Add(pairedDevice);
            }
            _devicesList.Adapter = new BluetoothsAdapter(bluetoothDevices);

            pickedDevice = pairedDevices.First(x => x.Name == "HC-05");
            if (pickedDevice != null)
            {
                var controlActivity = new Intent(this, typeof(ControlActivity));
                controlActivity.PutExtra("DeviceName", pickedDevice.Name);
                controlActivity.PutExtra("DeviceMac", pickedDevice.Address);
                StartActivity(controlActivity);
            }
        }

        private void OnItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            pickedDevice = bluetoothDevices[e.Position];
            //pickedDevice = bluetoothNames[e.Position];

            Toast.MakeText(this, pickedDevice.Name, ToastLength.Short).Show();
        }

        private void ScanOnClick(object sender, EventArgs eventArgs)
        {
            bluetoothDevices = new ObservableCollection<BluetoothDevice>();
            //bluetoothNames = new ObservableCollection<string>();
            pickedDevice = null;

            btAdapter = BluetoothAdapter.DefaultAdapter;
            if (btAdapter == null)
            {
                Toast.MakeText(this, "Wrong device", ToastLength.Short).Show();
                return;
            }
            if (!btAdapter.IsEnabled)
            {
                btAdapter.Enable();
            }

            btAdapter.StartDiscovery();

            Task.Run(async () =>
            {
                await Task.Delay(2000);
                btAdapter.CancelDiscovery();
            });

            var pairedDevices = btAdapter.BondedDevices;
            foreach (var pairedDevice in pairedDevices)
            {
                bluetoothDevices.Add(pairedDevice);
                //bluetoothNames.Add(pairedDevice.Name);
            }
            _devicesList.Adapter = new BluetoothsAdapter(bluetoothDevices);
        }
    }
}

