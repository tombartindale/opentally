using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.AppCompat.App;

namespace ObsTallyLight
{
    [Activity(Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : AppCompatActivity
    {

        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);
            
        }

        public override void OnBackPressed() { }

        // Launches the startup task
        protected async override void OnResume()
        {
            base.OnResume();
            await Task.Delay(1000);
            StartActivity(new Intent(Application.Context, typeof(MainActivity)));
        }

    }
}
