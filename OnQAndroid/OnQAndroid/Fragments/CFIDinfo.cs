using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace OnQAndroid
{
    public class CFIDinfo : Android.Support.V4.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.CFIDinfo, container, false);
            Button backButton = view.FindViewById<Button>(Resource.Id.backButton);

            backButton.Click += BackButton_Click;
            return view;
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();
            trans.Replace(Resource.Id.register_root_frame, new RegisterFragment());
            trans.Commit();
        }
    }
}