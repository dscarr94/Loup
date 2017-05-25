using System;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using SQLite;
using Android.Support.V4.View;

namespace OnQAndroid
{
    public class confirmCF : Android.Support.V4.App.Fragment
    {
        string myCFID;
        public confirmCF()
        {
            // Empty constructor
        }

        public static confirmCF newInstance()
        {
            confirmCF fragment = new confirmCF();
            return fragment;
        }

        ViewPager viewPager;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Bundle arguments = Arguments;
            myCFID = arguments.GetString("CFID");
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.ConfirmCF, container, false);
            TextView thisCareerFair = view.FindViewById<TextView>(Resource.Id.thisCareerFair);
            ImageView cfLogo = view.FindViewById<ImageView>(Resource.Id.cfImage);
            Button cancelBtn = view.FindViewById<Button>(Resource.Id.cancelCF);
            Button confirmBtn = view.FindViewById<Button>(Resource.Id.yes);
            viewPager = this.Activity.FindViewById<ViewPager>(Resource.Id.viewpager);

            //string dbPath_attributes = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "attributes.db3");
            //var db_attributes = new SQLiteConnection(dbPath_attributes);

            string dbPath_cfids = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "CFIDs.db3");
            var db_cfids = new SQLiteConnection(dbPath_cfids);

            var cfid_queryResults = db_cfids.Query<Cfids>("SELECT * FROM Cfids WHERE cfid = ?", myCFID);
            Cfids cfid = cfid_queryResults.First();
            thisCareerFair.Text = cfid.name;

            string imageName = "img" + myCFID;
            int resourceId = (int)typeof(Resource.Drawable).GetField(imageName).GetValue(null);
            cfLogo.SetImageResource(resourceId);

            cancelBtn.Click += CancelBtn_Click;
            confirmBtn.Click += ConfirmBtn_Click;

            return view;
        }

        private void ConfirmBtn_Click(object sender, EventArgs e)
        {
            string dbPath_attributes = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "attributes.db3");
            var db_attributes = new SQLiteConnection(dbPath_attributes);

            string dbPath_user = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "user.db3");
            var db_user = new SQLiteConnection(dbPath_user);

            string dbPath_myCF = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), myCFID + ".db3");
            var db_myCF = new SQLiteConnection(dbPath_myCF);

            MyAttributes myAttributes = db_attributes.Get<MyAttributes>(1);
            var loginQueryResults = db_user.Query<LoginTable>("SELECT * FROM LoginTable WHERE email = ?", myAttributes.email);
            LoginTable myLogInInfo = loginQueryResults.First();
            myAttributes.cfid = Convert.ToInt32(myCFID);
            myLogInInfo.cfid = myAttributes.cfid;

            db_user.Update(myLogInInfo);
            db_attributes.Update(myAttributes);

            if (myAttributes.type == "Student")
            {
                string favorites_fileName = "fav_" + myCFID + "_" + myLogInInfo.id.ToString() + ".db3";
                string dbPath_favorites = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), favorites_fileName);
                var db_favorites = new SQLiteConnection(dbPath_favorites);
                db_favorites.CreateTable<OnQAndroid.SQLite_Tables.MyFavorites>();

                int numRows = db_myCF.Table<Companies>().Count();
                SQLite_Tables.MyFavorites item = new SQLite_Tables.MyFavorites();

                for (int i = 1; i <= numRows; i++)
                {
                    item.id = i;
                    item.isFavorite = false;
                    db_favorites.InsertOrReplace(item);
                }

                Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();
                trans.Replace(Resource.Id.register_root_frame, new RegisterFragment());
                trans.Commit();

                viewPager.SetCurrentItem(0, true);
            }

            else if (myAttributes.type == "Recruiter")
            {
                string mPreferences_fileName = "mp_" + myCFID + "_" + myAttributes.attribute1 + ".db3";
                string dbPath_mPreferences = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), mPreferences_fileName);
                var db_mPreferences = new SQLiteConnection(dbPath_mPreferences);
                db_mPreferences.CreateTable<OnQAndroid.SQLite_Tables.MajorPreferences>();
                db_mPreferences.CreateTable<OnQAndroid.SQLite_Tables.GradTermPreferences>();
                db_mPreferences.CreateTable<OnQAndroid.SQLite_Tables.GPAPreferences>();

                string myCompanyQFilename = "qs_" + myCFID + "_" + myAttributes.attribute1 + ".db3";
                string dbPath_myCompanyQ = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), myCompanyQFilename);
                var db_myCompanyQ = new SQLiteConnection(dbPath_myCompanyQ);
                db_myCompanyQ.CreateTable<OnQAndroid.SQLite_Tables.Queue>();

                string myCompanyPastQsFilename = "pastqs_" + myAttributes.attribute1 + ".db3";
                string dbPath_pastqs = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), myCompanyPastQsFilename);
                var db_pastqs = new SQLiteConnection(dbPath_pastqs);
                db_pastqs.CreateTable<OnQAndroid.SQLite_Tables.PastQueue>();

                Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();
                trans.Replace(Resource.Id.register_root_frame, new RegisterFragment());
                trans.Commit();

                viewPager.SetCurrentItem(0, true);
            }
        }

        private void CancelBtn_Click(object sender, EventArgs e)
        {
            /*string dbPath_attributes = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "attributes.db3");
            var db_attributes = new SQLiteConnection(dbPath_attributes);

            string dbPath_user = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "user.db3");
            var db_user = new SQLiteConnection(dbPath_user);

            MyAttributes myAttributes = db_attributes.Get<MyAttributes>(1);
            var loginQueryResults = db_user.Query<LoginTable>("SELECT * FROM LoginTable WHERE email = ?", myAttributes.email);
            LoginTable myLogInInfo = loginQueryResults.First();
            myLogInInfo.cfid = 0;
            myAttributes.cfid = 0;

            db_attributes.Update(myAttributes);
            db_user.Update(myLogInInfo);*/
            
            Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();
            trans.Replace(Resource.Id.register_root_frame, new RegisterFragment());
            trans.Commit();

            //viewPager.SetCurrentItem(0, true);
        }
    }
}