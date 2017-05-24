using System;
using System.Linq;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using InteligentDimmerMobile.Configuration;
using InteligentDimmerMobile.Services;
using Java.Util;

namespace InteligentDimmerMobile
{
    // Theme = "@android:style/Theme.Holo.Light.NoActionBar")  Theme.AppCompat.Light.NoActionBar
    [Activity(Label = "Control Panel", Theme = "@style/MyTheme")]
    public class ControlActivity : Activity
    {
        private LinearLayout _rootLayout;
        private TextView _pickedDeviceTextView;
        private Switch _onOffSwitch;
        private SeekBar _sliderSeekBar;
        private EditText _powerValuEditText;
        private EditText _startTimeHoursEditText;
        private EditText _startTimeMinutesEditText;

        private BluetoothAdapter btAdapter;
        private BluetoothSocket _socket;

        private NumberPicker _numberPicker;

        private int SliderValue = 0;
        private int ValueToSend = 0; // other than Slider becaues it causes problem with switch

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.ControlLayout);

            _rootLayout = FindViewById<LinearLayout>(Resource.Id.rootLayout);
            _pickedDeviceTextView = FindViewById<TextView>(Resource.Id.pickedDeviceNameTextView);
            _onOffSwitch = FindViewById<Switch>(Resource.Id.onOffSwitch);
            _sliderSeekBar = FindViewById<SeekBar>(Resource.Id.seekBar);
            _powerValuEditText = FindViewById<EditText>(Resource.Id.powerValueEditText);
            _startTimeHoursEditText = FindViewById<EditText>(Resource.Id.startTimeHoursEditText);
            _startTimeMinutesEditText = FindViewById<EditText>(Resource.Id.startTimeMinutesEditText);

            //    _numberPicker = FindViewById<NumberPicker>(Resource.Id.numberPicker1);

            SliderValue = 0;
            ValueToSend = 0;
            _powerValuEditText.Text = 0.ToString();

            _pickedDeviceTextView.Text = "Device: " + Intent.GetStringExtra("DeviceName");
            _onOffSwitch.CheckedChange += _onOffSwitch_CheckedChange;
            _sliderSeekBar.ProgressChanged += _sliderSeekBar_ProgressChanged;
            _powerValuEditText.TextChanged += _powerValuEditText_TextChanged;
       
            var macAddress = Intent.GetStringExtra("DeviceMac");

            btAdapter = BluetoothAdapter.DefaultAdapter;
            //     var pickedDevice = btAdapter.BondedDevices.FirstOrDefault(x => x.Address == macAddress);

            //     _socket = pickedDevice.CreateRfcommSocketToServiceRecord(UUID.FromString(Constants.BluetoothUUID));
            //     _socket.Connect();

            //if (_socket.IsConnected)
            //{
            //    Toast.MakeText(this, "Success!", ToastLength.Short).Show();
            //}

            imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
            //imm.HideSoftInputFromWindow(_powerValuEditText.WindowToken, 0);


            _powerValuEditText.KeyPress += (object sender, View.KeyEventArgs e) =>
            {
                var et = sender as EditText;
                et.SetCursorVisible(true);
                et.SetSelectAllOnFocus(true);
                e.Handled = false;
                if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
                {
                    int tmpVal;
                    if (Int32.TryParse(_powerValuEditText.Text, out tmpVal))
                    {
                        if (tmpVal > 100)
                        {
                            _powerValuEditText.Text = 100.ToString();
                            _sliderSeekBar.Progress = 100;
                        }
                        else if (tmpVal < 0)
                        {
                            _powerValuEditText.Text = 0.ToString();
                            _sliderSeekBar.Progress = 0;
                        }
                    }

                    Toast.MakeText(this, _powerValuEditText.Text, ToastLength.Short).Show();
                    e.Handled = true;
                    et.SetCursorVisible(false);
                }
            };

            _powerValuEditText.EditorAction += MyEditText_EditorAction;
            _powerValuEditText.Click += MyEditText_Click;

            _rootLayout.RequestFocus();
            _rootLayout.Click += _rootLayout_Click;
        }

        private InputMethodManager imm;

        private void _rootLayout_Click(object sender, EventArgs e)
        {
            _rootLayout.RequestFocus();
            imm.HideSoftInputFromWindow(_powerValuEditText.WindowToken, HideSoftInputFlags.None);
            imm.HideSoftInputFromInputMethod(_powerValuEditText.WindowToken, HideSoftInputFlags.None);
            imm.HideSoftInputFromWindow(_startTimeHoursEditText.WindowToken, HideSoftInputFlags.None);
            imm.HideSoftInputFromInputMethod(_startTimeHoursEditText.WindowToken, HideSoftInputFlags.None);
        }

        //public override bool DispatchTouchEvent(MotionEvent ev)
        //{
        //    if (ev.Action == MotionEventActions.Down)
        //    {
        //        View v = Window.CurrentFocus;
        //        if (v is EditText)
        //        {
        //            Rect outRect = new Rect();
        //            v.GetGlobalVisibleRect(outRect);
        //            if (!outRect.Contains((int) ev.GetX(), (int) ev.GetY()))
        //            {
        //                v.ClearFocus();
        //                InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
        //                imm.HideSoftInputFromInputMethod(v.WindowToken, 0);
        //            }
        //        }
        //    }
        //    return base.DispatchTouchEvent(ev);
        //}

        private void MyEditText_EditorAction(object sender, TextView.EditorActionEventArgs e)
        {
            if (e.ActionId == ImeAction.Done)
            {
                _powerValuEditText.SetCursorVisible(false);
                
                var et = sender as EditText;
                et.SetSelection(et.Text.Length);
                if (Int32.TryParse(et.Text, out SliderValue))
                {
                    if (SliderValue <= 0)
                    {
                        SliderValue = 0;
                        _onOffSwitch.Checked = false;
                    }
                    else
                    {
                        _onOffSwitch.Checked = true;
                        if (SliderValue >= 100)
                        {
                            SliderValue = 100;
                        }
                        else
                        {
                            _sliderSeekBar.Progress = SliderValue;
                        }

                    }
                    ValueToSend = SliderValue;
                    PrepareDataService.PrepareData(0x01, 0x00);
                    // Send();
                }
            }
            e.Handled = false;
        }

        private void MyEditText_Click(object sender, EventArgs e)
        {
            _powerValuEditText.SetCursorVisible(true);
        }

        private void _powerValuEditText_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            var et = sender as EditText;
            et.SetSelection(et.Text.Length);
            if (Int32.TryParse(e.Text.ToString(), out SliderValue))
            {
                if (SliderValue <= 0)
                {
                    SliderValue = 0;
                    _onOffSwitch.Checked = false;
                }
                else
                {
                    _onOffSwitch.Checked = true;
                    if (SliderValue >= 100)
                    {
                        SliderValue = 100;
                    }
                    else
                    {
                        _sliderSeekBar.Progress = SliderValue;
                    }

                }
                ValueToSend = SliderValue;
                PrepareDataService.PrepareData(0x01, 0x00);
                // Send();
            }
        }

        private void _sliderSeekBar_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            _powerValuEditText.Text = e.Progress.ToString();
            
        }

        private void _onOffSwitch_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (!e.IsChecked)
            {

                SliderValue = 0;
                PrepareDataService.PrepareData(0x00, 0x00);
                _sliderSeekBar.Progress = SliderValue;
                // Send();
            }
            else
            {
                
                if(Int32.TryParse(_powerValuEditText.Text, out ValueToSend))
                {
                    if(ValueToSend > 0 && ValueToSend < 100)
                    {
                        SliderValue = ValueToSend;
                    }
                    else
                    {
                        SliderValue = 100;
                    }
                }
              
                _sliderSeekBar.Progress = SliderValue;
                PrepareDataService.PrepareData(0x01, (byte)SliderValue);
                // Send();
            }
        }


        public override void OnBackPressed()
        {
            base.OnBackPressed();
         //   _socket.Close();
        }
    }

    public class NumberPickerDialogFragment : DialogFragment
    {
        private readonly Context _context;
        private readonly int _min, _max, _current;
        private readonly NumberPicker.IOnValueChangeListener _listener;

        public NumberPickerDialogFragment(Context context, int min, int max, int current, NumberPicker.IOnValueChangeListener listener)
        {
            _context = context;
            _min = min;
            _max = max;
            _current = current;
            _listener = listener;
        }

        public override Dialog OnCreateDialog(Bundle savedState)
        {
            var inflater = (LayoutInflater)_context.GetSystemService((Context.LayoutInflaterService));
            var view = inflater.Inflate(Resource.Layout.NumberPickerDialog, null);
            var numberPicker = view.FindViewById<NumberPicker>(Resource.Id.numberPicker);
            numberPicker.MaxValue = _max;
            numberPicker.MinValue = _min;
            numberPicker.Value = _current;
            numberPicker.SetOnValueChangedListener(_listener);

            var dialog = new AlertDialog.Builder(_context);
            dialog.SetTitle("Power");
            dialog.SetView(view);
            dialog.SetNegativeButton("Cancel", (s, a) => { });
            dialog.SetPositiveButton("Ok", (s, a) => { });
            return dialog.Create();
        }
    }
}