using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using System.IO;
using SQLite;

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

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            string dbPath_attributes = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "attributes.db3");
            var db_attributes = new SQLiteConnection(dbPath_attributes);

            var myAttributes = db_attributes.Get<MyAttributes>(1);

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
                ListView favListView = view.FindViewById<ListView>(Resource.Id.favoritesList);

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
                    List<int> mItems = new List<int>();
                    // Connect to myFavorites database
                    string dbPath_login = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "user.db3");
                    var db_login = new SQLiteConnection(dbPath_login);

                    int myLogInID = db_login.Query<LoginTable>("SELECT * FROM LoginTable WHERE email = ?", myAttributes.email).First().id;

                    string fileName = "fav_" + myCFID.ToString() + "_" + myLogInID.ToString() + ".db3";
                    string dbPath_favorites = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), fileName);
                    var db_favorites = new SQLiteConnection(dbPath_favorites);

                    // Get number of rows in myCFID Companies table
                    int rows = db_favorites.Table<SQLite_Tables.MyFavorites>().Count();

                    // Populate list items
                    for (int i = 1; i <= rows; i = i + 1)
                    {
                        var newRow = db_favorites.Get<SQLite_Tables.MyFavorites>(i);
                        if (newRow.isFavorite == true)
                        {
                            int newItem = newRow.id;
                            mItems.Add(newItem);
                        }
                    }
                    // Provide list items to list view adapter
                    FavoriteCompaniesListViewAdapter adapter = new FavoriteCompaniesListViewAdapter(container.Context, mItems);
                    favListView.Adapter = adapter;
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

                name.Text = myAttributes.name;
                email.Text = myAttributes.email;
                company.Text = myAttributes.attribute1;

                editProfile.Click += EditProfile_Click;

                int myCFID = myAttributes.cfid;

                if (myCFID != 0)
                {
                    string fileName_pastqs = "pastqs_" + myAttributes.attribute1 + ".db3";
                    string dbPath_pastqs = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), fileName_pastqs);
                    SQLiteConnection db_pastqs = new SQLiteConnection(dbPath_pastqs);
                    int numpastqs = db_pastqs.Table<SQLite_Tables.PastQueue>().Count();

                    string dbPath_login = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "user.db3");
                    SQLiteConnection db_login = new SQLiteConnection(dbPath_login);

                    ListView lv_favorites = view.FindViewById<ListView>(Resource.Id.favoritesList);
                    List<int> students = new List<int>();

                    for (int i = 1; i <= numpastqs; i++)
                    {
                        SQLite_Tables.PastQueue thisPastQueue = db_pastqs.Get<SQLite_Tables.PastQueue>(i);
                        if (thisPastQueue.rating != 0)
                        {
                            int newStudentid = thisPastQueue.studentid;
                            students.Add(newStudentid);
                        }

                        PastQueuesListViewAdapter adapter = new PastQueuesListViewAdapter(container.Context, students, "Profile");
                        lv_favorites.Adapter = adapter;
                    }
                }
                return view;
            }
            else
            {
                throw new NotImplementedException();
            }

        }

        private void EditProfile_Click(object sender, EventArgs e)
        {
            Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();
            trans.Replace(Resource.Id.profile_root_frame, new EditProfileFragment());
            //trans.AddToBackStack(null);
            trans.Commit();
        }
    }
}