using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Text;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using InteligentDimmerMobile.Configuration;
using InteligentDimmerMobile.Services;
using Java.Lang;
using Java.Util;

namespace InteligentDimmerMobile.Activities
{
    // Theme = "@android:style/Theme.Holo.Light.NoActionBar"  "Theme.AppCompat.Light.NoActionBar" "@style/MyTheme"
    [Activity(Label = "Control Panel", Theme = "@style/AppTheme")]
    public class ControlActivity : Activity
    {
        private LinearLayout _rootLayout;

        private LinearLayout _wrapperLayout;
        private TextView _pickedDeviceTextView;
        private Switch _onOffSwitch;
        private SeekBar _sliderSeekBar;
        private EditText _brightnessEditText;
        private EditText _startTimeHoursEditText;
        private EditText _startTimeMinutesEditText;
        private EditText _endTimeHoursEditText;
        private EditText _endTimeMinutesEditText;
        private EditText _brightnessTimerEditText;
        private Button _setTimerButton;

        private BluetoothAdapter btAdapter;
        private BluetoothSocket _socket;

        private InputMethodManager _inputMethodManager;

        private int _sliderValue = 0;
        private int _valueToSend = 0; // other than Slider becaues it causes problem with switch

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.ControlLayout);

            SetupBindings();
            SetupDefaultValues();
            ValidateWhenLostFocusBindings();
            SetupBrightnessPanelBindings();
            ClearFocusWhenEditionFinishedBindings();

            _inputMethodManager = (InputMethodManager)GetSystemService(Context.InputMethodService);

            _pickedDeviceTextView.Text = "Device: " + Intent.GetStringExtra("DeviceName");
       
            var macAddress = Intent.GetStringExtra("DeviceMac");

            btAdapter = BluetoothAdapter.DefaultAdapter;

            #region codeWithRealDevice
            //     var pickedDevice = btAdapter.BondedDevices.FirstOrDefault(x => x.Address == macAddress);

            //     _socket = pickedDevice.CreateRfcommSocketToServiceRecord(UUID.FromString(Constants.BluetoothUUID));
            //     _socket.Connect();

            //if (_socket.IsConnected)
            //{
            //    Toast.MakeText(this, "Success!", ToastLength.Short).Show();
            //}
#endregion

            _wrapperLayout.Touch += OnWrapperLayoutTouch;

            _setTimerButton.Click += OnSetTimerButtonClick;

        }
       
        private void ClearFocusWhenEditionFinishedBindings()
        {
            _startTimeHoursEditText.EditorAction += OnStartTimeHoursEditTextEditorAction;
            _startTimeMinutesEditText.EditorAction += OnStartTimeMinutesEditTextEditorAction;
            _endTimeHoursEditText.EditorAction += OnEndTimeHoursEditTextEditorAction;
            _endTimeMinutesEditText.EditorAction += OnEndTimeMinutesEditTextEditorAction;
            _brightnessTimerEditText.EditorAction += OnBrightnessTimerEditTextEditorAction;
        }

        private void SetupBrightnessPanelBindings()
        {
            _onOffSwitch.CheckedChange += OnOnOffSwitchCheckedChange;
            _sliderSeekBar.ProgressChanged += OnSliderSeekBarProgressChanged;
            _brightnessEditText.TextChanged += OnBrightnessEditTextTextChanged;

            _brightnessEditText.EditorAction += OnBrightnessEditTextEditorAction;
            _brightnessEditText.Click += OnBrightnessEditTextClick;
        }

        private bool? ValidateTimerBrightness(EditText editText)
        {
            if (!string.IsNullOrEmpty(editText.Text))
            {
                int brightnessValue;
                if (int.TryParse(editText.Text, out brightnessValue))
                {
                    if (brightnessValue >= 0 && brightnessValue <= 100)
                    {
                        return true;
                    }
                    return false;
                }
                return false;
            }
            return null;
        }

        private bool? ValidateTimerHours(EditText field)
        {
            if (!string.IsNullOrEmpty(field.Text))
            {
                var text = field.Text;
                int number;
                if (int.TryParse(text, out number))
                {
                    if (number >= 0 && number <= 23)
                    {
                        return true;
                    }
                    return false;
                }
                return false;
            }
            return null;
        }

        private bool? ValidateTimerMinutes(EditText field)
        {
            if (!string.IsNullOrEmpty(field.Text))
            {
                var text = field.Text;
                int number;
                if (int.TryParse(text, out number))
                {
                    if (number >= 0 && number <= 59)
                    {
                        return true;
                    }
                    return false;
                }
                return false;
            }
            return null;
        }

        private void OnWrapperLayoutTouch(object sender, View.TouchEventArgs e)
        {
            if (e.Event.Action == MotionEventActions.Up)
            {
                ClearFocusFromAllEditTexts();
            }
        }

        private bool Validate()
        {
            var result1 = ValidateTimerHours(_startTimeHoursEditText);
            if (result1 == false)
            {
                _startTimeHoursEditText.SetError("Invalid hour value\nMust be between 0 and 23", null);
                _startTimeHoursEditText.SetTextColor(Color.Red);
            }
            else
            {
                _startTimeHoursEditText.SetTextColor(Color.Black);
            }

            var result2 = ValidateTimerMinutes(_startTimeMinutesEditText);
            if (result2 == false)
            {
                _startTimeMinutesEditText.SetError("Invalid minute value\nMust be between 0 and 59", null);
                _startTimeMinutesEditText.SetTextColor(Color.Red);
            }
            else
            {
                _startTimeMinutesEditText.SetTextColor(Color.Black);
            }

            var result3 = ValidateTimerHours(_endTimeHoursEditText);
            if (result3 == false)
            {
                _endTimeHoursEditText.SetError("Invalid hour value\nMust be between 0 and 23", null);
                _endTimeHoursEditText.SetTextColor(Color.Red);
            }
            else
            {
                _endTimeHoursEditText.SetTextColor(Color.Black);
            }

            var result4 = ValidateTimerMinutes(_endTimeMinutesEditText);
            if (result4 == false)
            {
                _endTimeMinutesEditText.SetError("Invalid minute value\nMust be between 0 and 59", null);
                _endTimeMinutesEditText.SetTextColor(Color.Red);
            }
            else
            {
                _endTimeMinutesEditText.SetTextColor(Color.Black);
            }

            var result5 = ValidateTimerBrightness(_brightnessTimerEditText);
            if (result5 == false)
            {
                _brightnessTimerEditText.SetError("Invalid brightness value\nMust be between 0 and 100", null);
                _brightnessTimerEditText.SetTextColor(Color.Red);
            }
            else
            {
                _brightnessTimerEditText.SetTextColor(Color.Black);
            }

            if (result1 == true && result2 == true && result3 == true
                && result4 == true && result5 == true)
            {
                return true;
            }
            return false;
        }

        private void ClearFocusFromAllEditTexts(EditText currentEditText = null)
        {
            _wrapperLayout.ClearFocus();
            if (currentEditText != null)
            {
                _inputMethodManager.HideSoftInputFromWindow(currentEditText.WindowToken,
                    HideSoftInputFlags.None);
            }
            else
            {
                _inputMethodManager.HideSoftInputFromWindow(_brightnessEditText.WindowToken, 
                    HideSoftInputFlags.None);
                _inputMethodManager.HideSoftInputFromWindow(_startTimeHoursEditText.WindowToken,
                    HideSoftInputFlags.None);
                _inputMethodManager.HideSoftInputFromWindow(_startTimeMinutesEditText.WindowToken,
                    HideSoftInputFlags.None);
                _inputMethodManager.HideSoftInputFromWindow(_endTimeHoursEditText.WindowToken, 
                    HideSoftInputFlags.None);
                _inputMethodManager.HideSoftInputFromWindow(_endTimeMinutesEditText.WindowToken,
                    HideSoftInputFlags.None);
                _inputMethodManager.HideSoftInputFromWindow(_brightnessTimerEditText.WindowToken,
                    HideSoftInputFlags.None);
            }
            Validate();
        }

        private void SetupBindings()
        {
            _wrapperLayout = FindViewById<LinearLayout>(Resource.Id.wrapperLayout);
            _pickedDeviceTextView = FindViewById<TextView>(Resource.Id.pickedDeviceNameTextView);
            _onOffSwitch = FindViewById<Switch>(Resource.Id.onOffSwitch);
            _sliderSeekBar = FindViewById<SeekBar>(Resource.Id.seekBar);
            _brightnessEditText = FindViewById<EditText>(Resource.Id.brightnessEditText);
            _startTimeHoursEditText = FindViewById<EditText>(Resource.Id.startTimeHoursEditText);
            _startTimeMinutesEditText = FindViewById<EditText>(Resource.Id.startTimeMinutesEditText);
            _endTimeHoursEditText = FindViewById<EditText>(Resource.Id.endTimeHoursEditText);
            _endTimeMinutesEditText = FindViewById<EditText>(Resource.Id.endTimeMinutesEditText);
            _brightnessTimerEditText = FindViewById<EditText>(Resource.Id.brightnessTimerEditText);
            _setTimerButton = FindViewById<Button>(Resource.Id.setTimerButton);
        }

        private void SetupDefaultValues()
        {
            _sliderValue = 0;
            _valueToSend = 0;
            _brightnessEditText.Text = 0.ToString();
        }

        private void OnBrightnessEditTextEditorAction(object sender, TextView.EditorActionEventArgs e)
        {
            if (e.ActionId == ImeAction.Done)
            {
                var editText = sender as EditText;
                
                var validationResult = ValidateTimerBrightness(editText);

                if (validationResult == true)
                {
                    _brightnessEditText.SetTextColor(Color.Black);
                    _sliderValue = int.Parse(editText.Text);
                    _valueToSend = _sliderValue;
                    _onOffSwitch.Checked = _sliderValue != 0;
                    _sliderSeekBar.Progress = _sliderValue;
                    PrepareDataService.PrepareData(0x01, 0x00);
                    // Send();
                }
                else
                {
                    _brightnessEditText.SetError("Invalid brightness value\nMust be between 0 and 100", null);
                    _brightnessEditText.SetTextColor(Color.Red);
                }
            }
            e.Handled = false;
            ClearFocusFromAllEditTexts(sender as EditText);
        }

        private void OnBrightnessEditTextClick(object sender, EventArgs e)
        {
            _brightnessEditText.SetCursorVisible(true);
        }

        private void OnBrightnessEditTextTextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            var editText = sender as EditText;
            editText.SetSelection(editText.Text.Length);

            var validationResult = ValidateTimerBrightness(editText);

            if (validationResult == true)
            {
                _brightnessEditText.SetTextColor(Color.Black);
                _sliderValue = int.Parse(editText.Text);
                _valueToSend = _sliderValue;
                _onOffSwitch.Checked = _sliderValue != 0;
                _sliderSeekBar.Progress = _sliderValue;
                PrepareDataService.PrepareData(0x01, 0x00);
                // Send();
            }
            else
            {
                _brightnessEditText.SetError("Invalid brightness value\nMust be between 0 and 100", null);
                _brightnessEditText.SetTextColor(Color.Red);
            }
        }

        private void OnSliderSeekBarProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            _brightnessEditText.Text = e.Progress.ToString();
        }

        private void OnOnOffSwitchCheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (!e.IsChecked)
            {
                _sliderValue = 0;
                PrepareDataService.PrepareData(0x00, 0x00);
                _sliderSeekBar.Progress = _sliderValue;
                // Send();
            }
            else
            {
                if(int.TryParse(_brightnessEditText.Text, out _valueToSend))
                {
                    if(_valueToSend > 0 && _valueToSend < 100)
                    {
                        _sliderValue = _valueToSend;
                    }
                    else
                    {
                        _sliderValue = 100;
                    }
                }
              
                _sliderSeekBar.Progress = _sliderValue;
                PrepareDataService.PrepareData(0x01, (byte)_sliderValue);
                // Send();
            }
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
         //   _socket.Close();
        }

        private void OnSetTimerButtonClick(object sender, EventArgs e)
        {
            _wrapperLayout.ClearFocus();
            if (Validate())
            {
                //var startTimeHour = int.Parse(_startTimeHoursEditText.Text);
                //var startTimeMinute = int.Parse(_startTimeMinutesEditText.Text);
                //var endTimeHour = int.Parse(_endTimeHoursEditText.Text);
                //var endTimeinute = int.Parse(_endTimeMinutesEditText.Text);
                //var brightness = int.Parse(_brightnessTimerEditText.Text);
                //Send();

                Toast.MakeText(this, "Sent", ToastLength.Short).Show();
            }
        }

        private void ValidateWhenLostFocusBindings()
        {
            _brightnessEditText.FocusChange += OnBrightnessEditTextFocusChange;
            _startTimeHoursEditText.FocusChange += OnStartTimeHoursEditTextFocusChange;
            _startTimeMinutesEditText.FocusChange += OnStartTimeMinutesEditTextFocusChange;
            _endTimeHoursEditText.FocusChange += OnEndTimeHoursEditTextFocusChange;
            _endTimeMinutesEditText.FocusChange += OnEndTimeMinutesEditTextFocusChange;
            _brightnessTimerEditText.FocusChange += OnBrightnessTimerEditTextFocusChange;
        }

        #region OnFocusLostValidation
        private void OnBrightnessEditTextFocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (!e.HasFocus)
            {
                var validationResult = ValidateTimerBrightness(sender as EditText);
                if (validationResult == false)
                {
                    _brightnessEditText.SetError("Invalid brightness value\nMust be between 0 and 100", null);
                    _brightnessEditText.SetTextColor(Color.Red);
                }
                else
                {
                    _brightnessEditText.SetTextColor(Color.Black);
                }
            }
        }

        private void OnStartTimeHoursEditTextFocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (!e.HasFocus)
            {
                ClearFocusFromAllEditTexts((sender as EditText));
            }
        }

        private void OnStartTimeMinutesEditTextFocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (!e.HasFocus)
            {
                ClearFocusFromAllEditTexts((sender as EditText));
            }
        }

        private void OnEndTimeHoursEditTextFocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (!e.HasFocus)
            {
                ClearFocusFromAllEditTexts((sender as EditText));
            }
        }

        private void OnEndTimeMinutesEditTextFocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (!e.HasFocus)
            {
                ClearFocusFromAllEditTexts((sender as EditText));
            }
        }

        private void OnBrightnessTimerEditTextFocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (!e.HasFocus)
            {
                ClearFocusFromAllEditTexts((sender as EditText));
            }
        }
        #endregion

        #region OnTextEditionAction 
        private void OnStartTimeHoursEditTextEditorAction(object sender, TextView.EditorActionEventArgs e)
        {
            if (e.ActionId == ImeAction.Next)
            {
                ClearFocusFromAllEditTexts((sender as EditText));
            }
        }
        private void OnStartTimeMinutesEditTextEditorAction(object sender, TextView.EditorActionEventArgs e)
        {
            if (e.ActionId == ImeAction.Done)
            {
                ClearFocusFromAllEditTexts((sender as EditText));

            }
        }

        private void OnEndTimeHoursEditTextEditorAction(object sender, TextView.EditorActionEventArgs e)
        {
            if (e.ActionId == ImeAction.Next)
            {
                ClearFocusFromAllEditTexts((sender as EditText));
            }
        }

        private void OnEndTimeMinutesEditTextEditorAction(object sender, TextView.EditorActionEventArgs e)
        {
            if (e.ActionId == ImeAction.Done)
            {
                ClearFocusFromAllEditTexts((sender as EditText));
            }
        }

        private void OnBrightnessTimerEditTextEditorAction(object sender, TextView.EditorActionEventArgs e)
        {
            if (e.ActionId == ImeAction.Done)
            {
                ClearFocusFromAllEditTexts((sender as EditText));
            }
        }
        #endregion
    }
}