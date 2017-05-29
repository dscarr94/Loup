using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using System.IO;
using SQLite;
using Firebase.Xamarin.Database;
using OnQAndroid.FirebaseObjects;

namespace OnQAndroid
{
    public class ProfileFragment : Android.Support.V4.App.Fragment
    {
        public ProfileFragment()
        {
            // Required empty public constructor
        }

        public static ProfileFragment newInstance()
        {
            ProfileFragment fragment = new OnQAndroid.ProfileFragment();
            return fragment;
        }

        private const string FirebaseURL = "https://onqfirebase.firebaseio.com/";
        ViewGroup mContainer;
        ListView favListView;
        MyAttributes myAttributes;
        ProgressBar progressBar;
        ListView lv_favorites;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            mContainer = container;
            string dbPath_attributes = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "attributes.db3");
            var db_attributes = new SQLiteConnection(dbPath_attributes);

            myAttributes = db_attributes.Get<MyAttributes>(1);

            string type = myAttributes.type;

            if (type == "Student")
            {
                View view = inflater.Inflate(Resource.Layout.StudentProfileTab, container, false);
                TextView name = view.FindViewById<TextView>(Resource.Id.myName);
                TextView email = view.FindViewById<TextView>(Resource.Id.myEmail);
                TextView school = view.FindViewById<TextView>(Resource.Id.myUniversity);
                TextView major = view.FindViewById<TextView>(Resource.Id.myMajor);
                TextView gpa = view.FindViewById<TextView>(Resource.Id.myGPA);
                TextView gradterm = view.FindViewById<TextView>(Resource.Id.myGradterm);
                TextView editProfile = view.FindViewById<TextView>(Resource.Id.editProfile);
                favListView = view.FindViewById<ListView>(Resource.Id.favoritesList);
                progressBar = view.FindViewById<ProgressBar>(Resource.Id.circularProgress);

                name.Text = myAttributes.name;
                email.Text = myAttributes.email;
                school.Text = myAttributes.attribute1;
                gradterm.Text = myAttributes.attribute2;
                major.Text = myAttributes.attribute3;
                gpa.Text = myAttributes.attribute4;

                editProfile.Click += EditProfile_Click;

                int myCFID = myAttributes.cfid;
                if (myCFID != 0)
                {
                    PopulateList();
                }

                return view;
            }

            else if (type == "Recruiter")
            {
                View view = inflater.Inflate(Resource.Layout.RecruiterProfileTab, container, false);
                TextView name = view.FindViewById<TextView>(Resource.Id.myName);
                TextView email = view.FindViewById<TextView>(Resource.Id.myEmail);
                TextView company = view.FindViewById<TextView>(Resource.Id.myCompany);
                TextView editProfile = view.FindViewById<TextView>(Resource.Id.editProfile);
                progressBar = view.FindViewById<ProgressBar>(Resource.Id.circularProgress);
                lv_favorites = view.FindViewById<ListView>(Resource.Id.favoritesList);

                name.Text = myAttributes.name;
                email.Text = myAttributes.email;
                company.Text = myAttributes.attribute1;

                editProfile.Click += EditProfile_Click;

                int myCFID = myAttributes.cfid;

                if (myCFID != 0)
                {
                    PopulateRecruiterList();
                }
                return view;
            }
            else
            {
                throw new NotImplementedException();
            }

        }

        private async void PopulateRecruiterList()
        {
            progressBar.Visibility = ViewStates.Visible;
            string fileName_pastQs = "pastqs_" + myAttributes.attribute1;
            var firebase = new FirebaseClient(FirebaseURL);
            List<int> studentIds = new List<int>();
            List<string> studentNames = new List<string>();
            List<string> studentRatings = new List<string>();

            var pastQs = await firebase.Child(fileName_pastQs).OnceAsync<PastQ>();

            foreach (var pastq in pastQs)
            {
                if (pastq.Object.rating != "0")
                {
                    studentIds.Add(Convert.ToInt32(pastq.Object.studentid));
                    studentNames.Add(pastq.Object.name);
                    studentRatings.Add(pastq.Object.rating);
                }
            }

            PastQueuesListViewAdapter adapter = new PastQueuesListViewAdapter(mContainer.Context, studentIds, "Profile", studentNames, studentRatings);
            lv_favorites.Adapter = adapter;
            progressBar.Visibility = ViewStates.Invisible;
        }

        private async void PopulateList()
        {
            progressBar.Visibility = ViewStates.Visible;
            var firebase = new FirebaseClient(FirebaseURL);
            List<int> mItems = new List<int>();
            List<string> mCompanies = new List<string>();
            string fileName = "fav_" + myAttributes.cfid.ToString() + "_" + myAttributes.typeid.ToString();

            var favItems = await firebase.Child(fileName).OnceAsync<Favorite>();

            foreach (var item in favItems)
            {
                if (item.Object.isFavorite == true)
                {
                    mItems.Add(Convert.ToInt32(item.Object.companyid));
                    mCompanies.Add(item.Object.name);
                }
            }

            // Provide list items to list view adapter
            FavoriteCompaniesListViewAdapter adapter = new FavoriteCompaniesListViewAdapter(mContainer.Context, mItems, mCompanies);
            favListView.Adapter = adapter;
            progressBar.Visibility = ViewStates.Invisible;
        }

        private void EditProfile_Click(object sender, EventArgs e)
        {
            Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();
            trans.Replace(Resource.Id.profile_root_frame, new EditProfileFragment());
            trans.Commit();
        }
    }
}