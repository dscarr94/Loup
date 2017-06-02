using Android.App;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using Firebase.Xamarin.Database;
using Firebase.Xamarin.Database.Query;
using OnQAndroid.FirebaseObjects;
using SQLite;
using System;
using System.Threading.Tasks;

namespace OnQAndroid.Fragments
{
    public class NoQsPresent : Android.Support.V4.App.Fragment
    {
        SwipeRefreshLayout swipeContainer;
        MyAttributes myAttributes;
        private const string FirebaseURL = "https://onqfirebase.firebaseio.com/";
        string mSender;

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
            Bundle arguments = Arguments;
            mSender = arguments.GetString("Sender");

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            string dbPath_attributes = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "attributes.db3");
            SQLiteConnection db_attributes = new SQLiteConnection(dbPath_attributes);
            myAttributes = db_attributes.Get<MyAttributes>(1);

            View view = inflater.Inflate(Resource.Layout.MyQsTab, container, false);
            ImageView backButton = view.FindViewById<ImageView>(Resource.Id.backButton);
            backButton.Click += (sender, e) =>
            {
                Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();
                trans.Replace(Resource.Id.qs_root_frame, new CurrentPastQs());
                trans.Commit();
            };

            swipeContainer = view.FindViewById<SwipeRefreshLayout>(Resource.Id.swipeLayout);
            swipeContainer.SetColorSchemeResources(Android.Resource.Color.HoloBlueLight, Android.Resource.Color.HoloGreenLight,
                                                   Android.Resource.Color.HoloOrangeLight, Android.Resource.Color.HoloRedLight);
            swipeContainer.Refresh += RefreshOnSwipe;
            return view;
        }

        private async void RefreshOnSwipe(object sender, EventArgs e)
        {
            (sender as SwipeRefreshLayout).Refreshing = false;
            await GetNumThings();
        }

        private async Task GetNumThings()
        {
            if (mSender == "CurrentQs" && myAttributes.type == "Recruiter")
            {
                var firebase = new FirebaseClient(FirebaseURL);

                string myCompanyQFilename = "qs_" + myAttributes.cfid.ToString() + "_" + myAttributes.attribute1;
                var myCompanyQ = await firebase.Child("qs").Child(myCompanyQFilename).OnceAsync<Queue>();

                int numStudents = myCompanyQ.Count;

                if (numStudents > 0)
                {
                    Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();
                    trans.Replace(Resource.Id.qs_root_frame, new ListQs());
                    trans.Commit();
                }
            }
            else if (mSender == "CurrentQs" && myAttributes.type == "Student")
            {
                var firebase = new FirebaseClient(FirebaseURL);

                string myQsFilename = "myqs_" + myAttributes.cfid.ToString() + "_" + myAttributes.typeid.ToString();
                var myQs = await firebase.Child("qs").Child(myQsFilename).OnceAsync<StudentQ>();

                int numQs = myQs.Count;

                if (numQs > 0)
                {
                    Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();
                    trans.Replace(Resource.Id.qs_root_frame, new StudentQsPresent());
                    trans.Commit();
                }
            }
        }
    }
}