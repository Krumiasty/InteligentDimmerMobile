using Android.App;
using Android.OS;
using Android.Widget;

namespace InteligentDimmerMobile
{
    [Activity(Label = "ControlActivity")]
    public class ControlActivity : Activity
    {
        private TextView _pickedDeviceTextView;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.ControlLayout);

            _pickedDeviceTextView = FindViewById<TextView>(Resource.Id.pickedDeviceNameTextView);

            _pickedDeviceTextView.Text = Intent.GetStringExtra("DeviceName");

        }
    }
}