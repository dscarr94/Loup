using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Firebase.Xamarin.Database;
using SQLite;
using OnQAndroid.FirebaseObjects;
using Firebase.Xamarin.Database.Query;

namespace OnQAndroid.Fragments
{
    public class RecruiterPastQs : Android.Support.V4.App.Fragment
    {
        public RecruiterPastQs()
        {
            //required empty public constructor
        }
        public static RecruiterPastQs newInstance()
        {
            RecruiterPastQs fragment = new RecruiterPastQs();
            return fragment;
        }
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        private const string FirebaseURL = "https://onqfirebase.firebaseio.com/";
        ListView lv_pastqs;
        MyAttributes myAttributes;
        ViewGroup mContainer;
        ProgressBar progressBar;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            mContainer = container;
            string dbPath_attributes = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "attributes.db3");
            SQLiteConnection db_attributes = new SQLiteConnection(dbPath_attributes);
            myAttributes = db_attributes.Get<MyAttributes>(1);

            View view = inflater.Inflate(Resource.Layout.PastQs, container, false);
            ImageView backButton = view.FindViewById<ImageView>(Resource.Id.backButton);
            progressBar = view.FindViewById<ProgressBar>(Resource.Id.circularProgress);

            backButton.Click += (sender, e) =>
            {
                Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();
                trans.Replace(Resource.Id.qs_root_frame, new Fragments.CurrentPastQs());
                trans.Commit();
            };

            lv_pastqs = view.FindViewById<ListView>(Resource.Id.pastqslv);
            PopulateListView();

            return view;
        }

        private async void PopulateListView()
        {
            progressBar.Visibility = ViewStates.Visible;
            var firebase = new FirebaseClient(FirebaseURL);
            string fileName_pastQs = "pastqs_" + myAttributes.attribute1;

            var pastQs = await firebase.Child("pastqs").Child(fileName_pastQs).OnceAsync<PastQ>();

            List<int> studentIds = new List<int>();
            List<string> studentNames = new List<string>();
            List<string> studentRatings = new List<string>();

            foreach (var pastq in pastQs)
            {
                studentIds.Add(Convert.ToInt32(pastq.Object.studentid));
                studentNames.Add(pastq.Object.name);
                studentRatings.Add(pastq.Object.rating);
            }

            PastQueuesListViewAdapter adapter = new PastQueuesListViewAdapter(mContainer.Context, studentIds, "PastQs", studentNames, studentRatings);
            lv_pastqs.Adapter = adapter;
            progressBar.Visibility = ViewStates.Invisible;
        }
    }
}