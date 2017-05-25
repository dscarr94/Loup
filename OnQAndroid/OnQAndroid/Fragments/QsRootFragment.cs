using Android.App;
using Android.OS;
using Android.Views;
using Android.Support.V4.App;

namespace OnQAndroid
{
    public class QsRootFragment : Android.Support.V4.App.Fragment
    {
        public QsRootFragment()
        {
            // required empty public constructor
        }

        public static QsRootFragment newInstance()
        {
            QsRootFragment fragment = new QsRootFragment();
            return fragment;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.qs_root_fragment, container, false);

            Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();

            trans.Replace(Resource.Id.qs_root_frame, new Fragments.CurrentPastQs());

            trans.Commit();

            return view;
        }
    }
}