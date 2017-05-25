using Android.App;
using Android.OS;
using Android.Views;
using Android.Support.V4.App;

namespace OnQAndroid
{
    public class RegisterRootFragment : Android.Support.V4.App.Fragment
    {
        public RegisterRootFragment()
        {
            // required empty public constructor
        }

        public static  RegisterRootFragment newInstance()
        {
            RegisterRootFragment fragment = new RegisterRootFragment();
            return fragment;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.register_root_fragment, container, false);

            Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();

            trans.Replace(Resource.Id.register_root_frame, new RegisterFragment());

            trans.Commit();

            return view;
        }
    }
}