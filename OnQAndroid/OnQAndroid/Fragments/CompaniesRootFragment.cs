using Android.App;
using Android.OS;
using Android.Views;
using Android.Support.V4.App;

namespace OnQAndroid
{
    public class CompaniesRootFragment : Android.Support.V4.App.Fragment
    {
        public CompaniesRootFragment()
        {
            // required empty public constructor
        }

        public static CompaniesRootFragment newInstance()
        {
            CompaniesRootFragment fragment = new CompaniesRootFragment();
            return fragment;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.companies_root_fragment, container, false);

            Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();

            trans.Replace(Resource.Id.companies_root_frame, new CompaniesFragment());

            trans.Commit();

            return view;
        }
    }
}