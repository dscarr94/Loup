using System;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using SQLite;

namespace OnQAndroid.Fragments
{
    public class SettingsFragment : Android.Support.V4.App.Fragment
    {
        public SettingsFragment()
        {
            // Required empty public constructor
        }

        public static SettingsFragment newInstance()
        {
            SettingsFragment fragment = new SettingsFragment();
            return fragment;
        }
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        View view;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            view = inflater.Inflate(Resource.Layout.SettingsTab, container, false);

            Button logOutButton = view.FindViewById<Button>(Resource.Id.logOutButton);
            Button devTools = view.FindViewById<Button>(Resource.Id.devToolsButton);

            logOutButton.Click += LogOutButton_Click;

            string dbPath_attributes = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "attributes.db3");
            SQLiteConnection db_attributes = new SQLiteConnection(dbPath_attributes);

            MyAttributes myAttributes = db_attributes.Get<MyAttributes>(1);

            if (myAttributes.email == "joshlaz22@hotmail.com")
            {
                devTools.Visibility = ViewStates.Visible;
            }

            devTools.Click += DevTools_Click;
            return view;
        }

        private void DevTools_Click(object sender, EventArgs e)
        {
            Intent devToolsIntent = new Intent(Activity, typeof(devTools));
            StartActivity(devToolsIntent);            
        }

        private void LogOutButton_Click(object sender, EventArgs e)
        {
            string dbPath_attributes = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "attributes.db3");
            SQLiteConnection db_attributes = new SQLiteConnection(dbPath_attributes);

            MyAttributes myAttributes = db_attributes.Get<MyAttributes>(1);

            myAttributes.rememberme = false;

            db_attributes.Update(myAttributes);

            Intent logOutIntent = new Intent(Activity, typeof(LoginSignup));
            StartActivity(logOutIntent);
            Activity.Finish();
        }
    }
}