using Android.App;
using Android.OS;

namespace OnQAndroid
{
    [Activity(Label = "CSScreen1")]
    public class csBuildProfile : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.CSBuildProfile);
        }
    }
}