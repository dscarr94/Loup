using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using SQLite;
using Firebase.Xamarin.Database;
using OnQAndroid.FirebaseObjects;
using System;
using Firebase.Xamarin.Database.Query;

namespace OnQAndroid.Fragments
{
    public class StudentQsPresent : Android.Support.V4.App.Fragment
    {
        public StudentQsPresent()
        {
            // required empty public constructor
        }

        public static StudentQsPresent newInstance()
        {
            StudentQsPresent fragment = new StudentQsPresent();
            return fragment;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        MyAttributes myAttributes;
        private const string FirebaseURL = "https://onqfirebase.firebaseio.com/";
        List<string> mItems;
        ListView mListView;
        ViewGroup mContainer;
        ProgressBar progressBar;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            mContainer = container;
            View view = inflater.Inflate(Resource.Layout.StudentQsPresent, container, false);
            mItems = new List<string>();
            progressBar = view.FindViewById<ProgressBar>(Resource.Id.circularProgress);
            mListView = view.FindViewById<ListView>(Resource.Id.myQsListView);
            ImageView backButton = view.FindViewById<ImageView>(Resource.Id.backButton);
            backButton.Click += (sender, e) =>
            {
                Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();
                trans.Replace(Resource.Id.qs_root_frame, new Fragments.CurrentPastQs());
                trans.Commit();
            };

            string dbPath_attributes = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "attributes.db3");
            SQLiteConnection db_attributes = new SQLiteConnection(dbPath_attributes);
            myAttributes = db_attributes.Get<MyAttributes>(1);

            PopulateList();

            return view;
        }

        private async void PopulateList()
        {
            progressBar.Visibility = ViewStates.Visible;
            string fileName_myQs = "myqs_" + myAttributes.cfid.ToString() + "_" + myAttributes.typeid.ToString();
            string fileName_myCareerFair = myAttributes.cfid.ToString();

            var firebase = new FirebaseClient(FirebaseURL);

            var myQs = await firebase.Child("qs").Child(fileName_myQs).OnceAsync<StudentQ>();
            var myCareerFair = await firebase.Child("careerfairs").Child(fileName_myCareerFair).OnceAsync<Company>();

            List<int> companyIds = new List<int>();
            List<string> mPositions = new List<string>();
            List<string> mWaitTimes = new List<string>();
            int position = -1;

            foreach (var q in myQs)
            {
                position = position + 1;
                mItems.Add(q.Object.company);
                mPositions.Add(q.Object.position);
                
                foreach (var company in myCareerFair)
                {
                    if (company.Object.name == mItems[position])
                    {
                        long partialWaitTime = Convert.ToInt64(company.Object.waittime);
                        long totalWaitTime = partialWaitTime * (Convert.ToInt32(mPositions[position]) - 1); // -1 because they are already in the line
                        TimeSpan ts = TimeSpan.FromTicks(totalWaitTime);
                        string waittime = String.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
                        mWaitTimes.Add(waittime);
                    }
                }
            }

            string fileName_favorites = "fav_" + myAttributes.cfid.ToString() + "_" + myAttributes.typeid.ToString();
            List <bool> favs = new List<bool>();
            var myFavs = await firebase.Child("favorites").Child(fileName_favorites).OnceAsync<Favorite>();

            for (int i = 0; i <= mItems.Count - 1; i++)
            {
                foreach (var fav in myFavs)
                {
                    if (mItems[i] == fav.Object.name)
                    {
                        favs.Add(fav.Object.isFavorite);
                        companyIds.Add(Convert.ToInt32(fav.Object.companyid));                    
                    }
                }
            }

            QsListViewAdapter adapter = new QsListViewAdapter(mContainer.Context, mItems, "CurrentQs", favs, companyIds, mWaitTimes, mPositions);
            mListView.Adapter = adapter;
            progressBar.Visibility = ViewStates.Invisible;
        }
    }
}