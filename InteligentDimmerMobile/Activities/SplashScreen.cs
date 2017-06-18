using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;

namespace InteligentDimmerMobile.Activities
{
    [Activity(Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true, 
        ScreenOrientation = ScreenOrientation.Portrait)]
    public class SplashScreen : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SplashScreen);
        
            var newActivity = new Handler();
            newActivity.PostDelayed(StartMainActivity, 500);
        }

        private void StartMainActivity()
        {
            StartActivity(new Intent(Application.Context, typeof(ConnectionActivity)));
        }

        protected override void OnPause()
        {
            base.OnPause();
            SetContentView(Resource.Layout.SplashScreen);
        }
    }
}