using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace OnQAndroid.Fragments
{
    public class NoQsPresent : Android.Support.V4.App.Fragment
    {
        public NoQsPresent()
        {
            //required empty public constructor
        }

        public static NoQsPresent newInstance()
        {
            NoQsPresent fragment = new NoQsPresent();
            return fragment;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.MyQsTab, container, false);
            ImageView backButton = view.FindViewById<ImageView>(Resource.Id.backButton);
            backButton.Click += (sender, e) =>
            {
                Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();
                trans.Replace(Resource.Id.qs_root_frame, new CurrentPastQs());
                trans.Commit();
            };
            return view;
        }
    }
}