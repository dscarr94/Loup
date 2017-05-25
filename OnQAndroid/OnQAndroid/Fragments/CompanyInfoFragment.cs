using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using SQLite;

namespace OnQAndroid
{
    public class CompanyInfoFragment : Android.Support.V4.App.Fragment
    {
        int companyInt;

        public CompanyInfoFragment()
        {
            // Required empty public constructor
        }

        public static CompanyInfoFragment newInstance()
        {
            CompanyInfoFragment fragment = new OnQAndroid.CompanyInfoFragment();
            return fragment;
        }
        string location;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Bundle arguments = Arguments;
            companyInt = arguments.GetInt("CompanyInt");
            location = arguments.GetString("Sender");
        }
        string fileName;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.CompanyInfo, container, false);
            TextView companyName = view.FindViewById<TextView>(Resource.Id.companyName);
            ImageView companyLogo = view.FindViewById<ImageView>(Resource.Id.bigCompanyLogo);
            TextView description = view.FindViewById<TextView>(Resource.Id.companyDescription);
            TextView website = view.FindViewById<TextView>(Resource.Id.companyWebsite);
            Button backButton = view.FindViewById<Button>(Resource.Id.backButton);

            string dbPath_attributes = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "attributes.db3");
            var db_attributes = new SQLiteConnection(dbPath_attributes);

            int myCFID = db_attributes.Get<MyAttributes>(1).cfid;
            fileName = myCFID.ToString() + ".db3";

            string dbPath_myCF = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), fileName);
            var db_myCF = new SQLiteConnection(dbPath_myCF);

            Companies thisCompanyInfo = db_myCF.Get<Companies>(companyInt);
            companyName.Text = thisCompanyInfo.name;

            string imageName = thisCompanyInfo.name.ToLower().Replace(" ", "");
            int resourceId = (int)typeof(Resource.Drawable).GetField(imageName).GetValue(null);
            companyLogo.SetImageResource(resourceId);

            description.Text = thisCompanyInfo.description;

            website.Text = thisCompanyInfo.website;
            website.Click += Website_Click;

            backButton.Click += BackButton_Click;

            return view;
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();
            if (location == "Companies")
            {
                trans.Replace(Resource.Id.companies_root_frame, new CompaniesFragment());
            }
            else if (location == "Profile")
            {
                trans.Replace(Resource.Id.profile_root_frame, new ProfileFragment());
            }
            else if (location == "CurrentQs")
            {
                trans.Replace(Resource.Id.qs_root_frame, new QsFragment());
            }
            else if (location == "PastQs")
            {
                trans.Replace(Resource.Id.qs_root_frame, new Fragments.PastQs());
            }
            trans.Commit();
        }

        private void Website_Click(object sender, EventArgs e)
        {
            string dbPath_myCF = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), fileName);
            var db_myCF = new SQLiteConnection(dbPath_myCF);

            Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(db_myCF.Get<Companies>(companyInt).website));
            StartActivity(intent);
        }
    }
}