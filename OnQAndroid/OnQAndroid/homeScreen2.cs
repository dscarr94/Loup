using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.View;
using com.refractored;
// converted to firebase vvv
namespace OnQAndroid
{
    [Activity(ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)] // lock screen orientation
    public class homeScreen2 : Android.Support.V4.App.FragmentActivity // Inherit FragmentActivity methods
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
          
            SetContentView(Resource.Layout.HomeScreen2); // set the view to HomeScreen2.axml
            ActionBar.Hide(); // hide the actionbar

            // Get user id
            string uid = Intent.GetStringExtra("UserId");

            ViewPager viewpager = FindViewById<ViewPager>(Resource.Id.viewpager); // get viewpager from layout

            // Add Adapter to the ViewPager
            HomeScreenAdapter adapter = new HomeScreenAdapter(SupportFragmentManager); // call homescreenadapter, feed it SupportFragmentManager (internal to FragmentActivity)
            viewpager.Adapter = adapter; // assign the adapter to the viewpager

            // Add Tabs to ViewPager
            PagerSlidingTabStrip tabs = FindViewById<PagerSlidingTabStrip>(Resource.Id.tabs); // get tabs from layout
            tabs.SetViewPager(viewpager); // assign the tabs to the viewpager  
        }
    }
}