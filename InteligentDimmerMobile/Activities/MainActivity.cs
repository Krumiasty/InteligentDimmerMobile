using Android.App;
using Android.Widget;
using Android.OS;
using System;
using Android.Content;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Android.Bluetooth;
using Android.Content.Res;
using InteligentDimmerMobile.Adapters;
using InteligentDimmerMobile.Model;

namespace InteligentDimmerMobile.Activities
{
    [Activity(Label = "Dimmer", Icon = "@drawable/sun", Theme = "@style/AppTheme")]
    public class MainActivity : Activity
    {
        private Button _scanButton;
        private ListView _devicesList;
        private Button _connectButton;

        private BluetoothAdapter btAdapter;
        private BluetoothDevice pickedDevice = null;
        //private string pickedDevice = null;
        private BluetoothSocket _socket;

        ObservableCollection<BluetoothDevice> bluetoothDevices =
            new ObservableCollection<BluetoothDevice>();

        //private ObservableCollection<string> bluetoothNames =
        //    new ObservableCollection<string>() { "one", "two" };

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            _scanButton = FindViewById<Button>(Resource.Id.searchButton);
            _devicesList = FindViewById<ListView>(Resource.Id.devicesList);
            _connectButton = FindViewById<Button>(Resource.Id.connectButton);

            _scanButton.Click += ScanOnClick;

            var controlActivity = new Intent(this, typeof(ControlActivity));
            controlActivity.PutExtra("DeviceName", "Test");
            //    controlActivity.PutExtra("DeviceName", pickedDevice.Name);
            //     controlActivity.PutExtra("DeviceMac", pickedDevice.Address);
            StartActivity(controlActivity);

            FirstScan();



       //     _devicesList.Adapter = new BluetoothsAdapter(bluetoothNames);
            _devicesList.ItemClick += OnItemClick;

            _connectButton.Click += /*async*/ (sender, args) =>
            {
                if (pickedDevice != null)
                {
                    #region alternative
                    //_socket = pickedDevice.CreateRfcommSocketToServiceRecord(UUID.FromString(Constants.BluetoothUUID));
                    //await _socket.ConnectAsync();

                    //// Write data to the device
                    //// SendData();
                    //// PrepareData(0x00, 0x00);
                    ////await _socket.OutputStream.WriteAsync()
                    //await _socket.InputStream.WriteAsync(new byte[]
                    //{
                    //    ControlData.StartByte,
                    //    ControlData.CommandByte,
                    //    ControlData.SeparatorByte,
                    //    ControlData.DataByte,
                    //    ControlData.EndByte
                    // }, 0, 5);

                    //byte[] response = new []{ (byte)0 };
                    //await _socket.OutputStream.ReadAsync(response, 0, 1);

                    //// _socket.OutputStream.Close();

                    ////await _socket.InputStream.ReadAsync(byte[] { }, 0, 0);
                    /// 
                    /// 
                    
                    #endregion

                    byte[] response = new []{ (byte)0 };
                    if (response[0] == (byte)0)
                    {
                        //_socket.OutputStream.Close();
                        //_socket.InputStream.Close();
                        //var controlActivity = new Intent(this, typeof(ControlActivity));
                        //controlActivity.PutExtra("DeviceName", pickedDevice.Name);
                        //controlActivity.PutExtra("DeviceMac", pickedDevice.Address);
                        //StartActivity(controlActivity);
                    }
                }
            };
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            _socket.Close();
        }

        private void SendData()
        {
        //        Response = null;
            _socket.OutputStream.Write(new byte[]
             {
                    ControlData.StartByte,
                    ControlData.CommandByte,
                    ControlData.SeparatorByte,
                    ControlData.DataByte,
                    ControlData.EndByte
             }, 0, 5);
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

