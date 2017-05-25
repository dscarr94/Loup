using Android.App;
using Android.OS;
using Android.Views;
using Android.Support.V4.App;

namespace OnQAndroid
{
    public class SettingsRootFragment : Android.Support.V4.App.Fragment
    {
        public SettingsRootFragment()
        {
            // required empty public constructor
        }

        public static SettingsRootFragment newInstance()
        {
            SettingsRootFragment fragment = new SettingsRootFragment();
            return fragment;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.settings_root_fragment, container, false);

            Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();

            trans.Replace(Resource.Id.settings_root_frame, new Fragments.SettingsFragment());

            trans.Commit();

            return view;
        }
    }
}