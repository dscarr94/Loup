using Android.App;
using Android.OS;

namespace OnQAndroid
{
    [Activity(Label = "passwordRecover")]
    public class passwordRecover : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.passwordRecover);
        }
    }
}