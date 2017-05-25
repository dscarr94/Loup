using Android.App;
using Android.OS;
using Android.Views;

using Android.Support.V4.App;

namespace OnQAndroid
{
    public class ProfileRootFragment : Android.Support.V4.App.Fragment
    {
        public ProfileRootFragment()
        {
            // required empty public constructor
        }

        public static ProfileRootFragment newInstance()
        {
            ProfileRootFragment fragment = new ProfileRootFragment();
            return fragment;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.profile_root_fragment, container, false);

            Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();

            trans.Replace(Resource.Id.profile_root_frame, new ProfileFragment());

            trans.Commit();

            return view;
        }
    }
}