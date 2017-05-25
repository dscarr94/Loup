using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using SQLite;
using Android.Support.V4.View;

namespace OnQAndroid.Fragments
{
    public class confirmQ : Android.Support.V4.App.Fragment
    {
        int companyInt;
        public confirmQ()
        {
            // Empty constructor
        }

        public static confirmQ newInstance()
        {
            confirmQ fragment = new Fragments.confirmQ();
            return fragment;
        }
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Bundle arguments = Arguments;
            companyInt = arguments.GetInt("CompanyInt");
        }

        SQLiteConnection db_attributes;
        SQLiteConnection db_myCF;
        ViewPager viewPager;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.ConfirmQ, container, false);

            viewPager = Activity.FindViewById<ViewPager>(Resource.Id.viewpager);
            string dbpath_attributes = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "attributes.db3");
            db_attributes = new SQLiteConnection(dbpath_attributes);
            MyAttributes myAttributes = db_attributes.Get<MyAttributes>(1);

            string myCFID = myAttributes.cfid.ToString();

            string dbpath_myCF = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), myCFID + ".db3");
            db_myCF = new SQLiteConnection(dbpath_myCF);

            Companies thisCompany = db_myCF.Get<Companies>(companyInt);

            TextView companyName = view.FindViewById<TextView>(Resource.Id.companyName);
            companyName.Text = thisCompany.name + "?";

            ImageView companyLogo = view.FindViewById<ImageView>(Resource.Id.companyLogo);
            string imageName = thisCompany.name.ToLower().Replace(" ", "");
            int resourceId = (int)typeof(Resource.Drawable).GetField(imageName).GetValue(null);
            companyLogo.SetImageResource(resourceId);

            Button cancelq = view.FindViewById<Button>(Resource.Id.cancelqbutton);
            Button confirmq = view.FindViewById<Button>(Resource.Id.confirmqbutton);

            cancelq.Click += Cancelq_Click;

            confirmq.Click += Confirmq_Click;
            return view;
        }

        private void Confirmq_Click(object sender, EventArgs e)
        {
            MyAttributes myAttributes = db_attributes.Get<MyAttributes>(1);
            Companies thisCompany = db_myCF.Get<Companies>(companyInt);

            string fileName_Q = "qs_" + myAttributes.cfid.ToString() + "_" + thisCompany.name + ".db3";
            string dbPath_Q = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), fileName_Q);
            SQLiteConnection db_Q = new SQLiteConnection(dbPath_Q);

            string fileName_myQs = "myqs_" + myAttributes.cfid.ToString() + "_" + myAttributes.loginid + ".db3";
            string dbPath_myQs = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), fileName_myQs);
            SQLiteConnection db_myQs = new SQLiteConnection(dbPath_myQs);
            db_myQs.CreateTable<SQLite_Tables.MyQueue>();

            try
            {
                var queryResults = db_Q.Query<SQLite_Tables.Queue>("SELECT * FROM Queue WHERE studentid = ?", myAttributes.typeid);
                if (queryResults.Count != 0)
                {
                    Toast.MakeText(this.Activity, "You are already onQ with this company", ToastLength.Short).Show();
                }
                else
                {
                    int numQs = db_Q.Table<SQLite_Tables.Queue>().Count();

                    SQLite_Tables.Queue newQ = new SQLite_Tables.Queue();
                    newQ.id = numQs + 1;
                    newQ.studentid = myAttributes.typeid;

                    db_Q.Insert(newQ);

                    SQLite_Tables.MyQueue myQ = new SQLite_Tables.MyQueue();
                    int numMyQs = db_myQs.Table<SQLite_Tables.MyQueue>().Count();
                    myQ.id = numMyQs + 1;
                    myQ.company = thisCompany.name;
                    myQ.position = newQ.id;

                    db_myQs.Insert(myQ);

                    Toast.MakeText(this.Activity, "You are onQ in Position " + (numQs + 1).ToString() + "!", ToastLength.Short).Show();

                    Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();
                    trans.Replace(Resource.Id.companies_root_frame, new CompaniesFragment());
                    trans.Commit();

                    Android.Support.V4.App.FragmentTransaction trans2 = FragmentManager.BeginTransaction();
                    trans.Replace(Resource.Id.qs_root_frame, new QsFragment());
                    trans2.Commit();

                    viewPager.SetCurrentItem(1, true);
                }
            }
            catch
            {
                Toast.MakeText(this.Activity, "No Recruiters have checked in with this company yet...", ToastLength.Short).Show();
                //this.Activity.DeleteDatabase(dbPath_Q);
            }
        }

        private void Cancelq_Click(object sender, EventArgs e)
        {
            Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();
            trans.Replace(Resource.Id.companies_root_frame, new CompaniesFragment());
            trans.Commit();
        }
    }
}