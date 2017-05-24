using System.Collections.Generic;
using System.Collections.ObjectModel;
using Android.App;
using Android.Bluetooth;
using Android.Views;
using Android.Widget;

namespace InteligentDimmerMobile.Adapters
{
    //public class BluetoothsAdapter : BaseAdapter<string>
    //{
        //    ObservableCollection<string> bluetooths;
        //    public BluetoothsAdapter(ObservableCollection<string> bluetooths) : base()
        //    {
        //        this.bluetooths = bluetooths;
        //    }

        //    public override long GetItemId(int position)
        //    {
        //        return position;
        //    }

        //    public override string this[int position]
        //    {
        //        get { return bluetooths[position]; }
        //    }

        //    public override int Count
        //    {
        //        get { return bluetooths.Count; }
        //    }

        //    public override View GetView(int position, View convertView, ViewGroup parent)
        //    {
        //        var inflater = LayoutInflater.From(parent.Context);
        //        var view = inflater.Inflate(Resource.Layout.BluetoothRow, parent, false);

        //        var id = view.FindViewById<TextView>(Resource.Id.bluetoothNumberTextView);
        //        var name = view.FindViewById<TextView>(Resource.Id.bluetoothNameTextView);

        //        id.Text = position.ToString();
        //        name.Text = bluetooths[position];

        //        return view;
        //    }
        //}

        #region BluetoothDeviceType



        public class BluetoothsAdapter : BaseAdapter<BluetoothDevice>
        {
            ObservableCollection<BluetoothDevice> bluetooths;
            public BluetoothsAdapter(ObservableCollection<BluetoothDevice> bluetooths) : base()
            {
                this.bluetooths = bluetooths;
            }

            public override long GetItemId(int position)
            {
                return position;
            }

            public override BluetoothDevice this[int position]
            {
                get { return bluetooths[position]; }
            }

            public override int Count
            {
                get { return bluetooths.Count; }
            }

            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                var inflater = LayoutInflater.From(parent.Context);
                var view = inflater.Inflate(Resource.Layout.BluetoothRow, parent, false);

                var id = view.FindViewById<TextView>(Resource.Id.bluetoothNumberTextView);
                var name = view.FindViewById<TextView>(Resource.Id.bluetoothNameTextView);

                id.Text = position.ToString();
                name.Text = bluetooths[position].Name;

                return view;
            }
        }
        #endregion
    }