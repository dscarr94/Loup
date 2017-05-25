using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace OnQAndroid.Fragments
{
    public class CurrentPastQs : Android.Support.V4.App.Fragment
    {
        public CurrentPastQs()
        {
            // Required empty public constructor
        }

        public static CurrentPastQs newInstance()
        {
            CurrentPastQs fragment = new CurrentPastQs();
            return fragment;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.CurrentPastQs, container, false);

            Button currentQs = view.FindViewById<Button>(Resource.Id.currentQs);
            Button pastQs = view.FindViewById<Button>(Resource.Id.pastQs);

            currentQs.Click += CurrentQs_Click;
            pastQs.Click += PastQs_Click;
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            return view;
        }

        private void PastQs_Click(object sender, EventArgs e)
        {
            Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();
            trans.Replace(Resource.Id.qs_root_frame, new PastQs());
            trans.Commit();
        }

        private void CurrentQs_Click(object sender, EventArgs e)
        {
            Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();
            trans.Replace(Resource.Id.qs_root_frame, new QsFragment());
            trans.Commit();
        }
    }
}