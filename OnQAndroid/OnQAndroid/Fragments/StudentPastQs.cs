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
    public class StudentPastQs : Android.Support.V4.App.Fragment
    {
        public StudentPastQs()
        {
            //required empty public constructor
        }

        public static StudentPastQs newInstance()
        {
            StudentPastQs fragment = new StudentPastQs();
            return fragment;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        private const string FirebaseURL = "https://onqfirebase.firebaseio.com/";
        ListView lv_pastqs;
        ProgressBar progressBar;
        MyAttributes myAttributes;
        ViewGroup mContainer;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            mContainer = container;
            string dbPath_attributes = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "attributes.db3");
            SQLiteConnection db_attributes = new SQLiteConnection(dbPath_attributes);
            myAttributes = db_attributes.Get<MyAttributes>(1);

            View view = inflater.Inflate(Resource.Layout.PastQs, container, false);
            ImageView backButton = view.FindViewById<ImageView>(Resource.Id.backButton);
            backButton.Click += (sender, e) =>
            {
                Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();
                trans.Replace(Resource.Id.qs_root_frame, new Fragments.CurrentPastQs());
                trans.Commit();
            };

            lv_pastqs = view.FindViewById<ListView>(Resource.Id.pastqslv);
            progressBar = view.FindViewById<ProgressBar>(Resource.Id.circularProgress);

            PopulateList();

            return view;
        }

        private async void PopulateList()
        {
            progressBar.Visibility = ViewStates.Visible;

            var firebase = new FirebaseClient(FirebaseURL);
            string fileName_pastQs = "pastqs_" + myAttributes.typeid.ToString();
            string fileName_favs = "fav_" + myAttributes.cfid.ToString() + "_" + myAttributes.typeid.ToString();

            var pastQs = await firebase.Child("pastqs").Child(fileName_pastQs).OnceAsync<StudentQ>();
            var myFavs = await firebase.Child("favorites").Child(fileName_favs).OnceAsync<Favorite>();
            List<string> companies = new List<string>();
            List<bool> favorites = new List<bool>();
            List<int> companyIds = new List<int>();
            List<string> positionBlank = new List<string>();
            List<string> timeBlank = new List<string>();

            foreach (var pastq in pastQs)
            {
                companies.Add(pastq.Object.company);
                foreach (var fav in myFavs)
                {
                    if (fav.Object.name == pastq.Object.company)
                    {
                        favorites.Add(fav.Object.isFavorite);
                        companyIds.Add(Convert.ToInt32(fav.Object.companyid));
                    }
                }
            }

            QsListViewAdapter adapter = new QsListViewAdapter(mContainer.Context, companies, "PastQs", favorites, companyIds, timeBlank, positionBlank);
            lv_pastqs.Adapter = adapter;
            progressBar.Visibility = ViewStates.Invisible;
        }
    }
}