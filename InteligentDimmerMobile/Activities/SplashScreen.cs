using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using Java.Lang;

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
            newActivity.PostDelayed(StartNewActivity, 500);
        }

        private void StartNewActivity()
        {
            StartActivity(new Intent(Application.Context, typeof(MainActivity)));
        }

        protected override void OnPause()
        {
            base.OnPause();
            SetContentView(Resource.Layout.SplashScreen);
        }
    }
}