using Android.App;
using Android.Widget;
using Android.OS;
using System;
using Android.Hardware.Usb;
using Android.Content;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO.Ports;
using System.Threading.Tasks;
using Android.Bluetooth;
using Android.Bluetooth.LE;
using Android.Runtime;
using Java.IO;

namespace InteligentDimmerMobile
{
    [Activity(Label = "InteligentDimmerMobile", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private Button _scanButton;
        private ListView _devicesList;
        private Button _connectButton;

        private BluetoothAdapter btAdapter;
        private BluetoothDevice pickedDevice = null;
        //private string pickedDevice = null;


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

            FirstScan();

       //     _devicesList.Adapter = new BluetoothsAdapter(bluetoothNames);
            _devicesList.ItemClick += OnItemClick;

            _connectButton.Click += (sender, args) =>
            {
                if (pickedDevice != null)
                {
                    var controlActivity = new Intent(this, typeof(ControlActivity));
                    controlActivity.PutExtra("DeviceName", pickedDevice.Name);
                    StartActivity(controlActivity);
                }
            };
        }

        void FirstScan()
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

        void OnItemClick(object sender, AdapterView.ItemClickEventArgs e)
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

