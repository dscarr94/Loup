using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using SQLite;
using OnQAndroid.FirebaseObjects;
using Firebase.Xamarin.Database;

namespace OnQAndroid
{
    public class CompanyInfoFragment : Android.Support.V4.App.Fragment
    {
        int companyInt;
        private const string FirebaseURL = "https://onqfirebase.firebaseio.com/";

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
        Company thisCompanyInfo = new Company();
        TextView companyName;
        ImageView companyLogo;
        TextView description;
        TextView website;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.CompanyInfo, container, false);
            companyName = view.FindViewById<TextView>(Resource.Id.companyName);
            companyLogo = view.FindViewById<ImageView>(Resource.Id.bigCompanyLogo);
            description = view.FindViewById<TextView>(Resource.Id.companyDescription);
            website = view.FindViewById<TextView>(Resource.Id.companyWebsite);
            Button backButton = view.FindViewById<Button>(Resource.Id.backButton);

            //fileName = myCFID.ToString() + ".db3";

            //string dbPath_myCF = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), fileName);
            //var db_myCF = new SQLiteConnection(dbPath_myCF)

            GetCompanyInfo();

            backButton.Click += BackButton_Click;

            return view;
        }

        private async void GetCompanyInfo()
        {
            string dbPath_attributes = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "attributes.db3");
            var db_attributes = new SQLiteConnection(dbPath_attributes);
            MyAttributes myAttributes = db_attributes.Get<MyAttributes>(1);

            var firebase = new FirebaseClient(FirebaseURL);
            var allCompanies = await firebase.Child(myAttributes.cfid.ToString()).OnceAsync<Company>();
            foreach (var company in allCompanies)
            {
                if (company.Object.companyid == companyInt.ToString())
                {
                    thisCompanyInfo.name = company.Object.name;
                    thisCompanyInfo.description = company.Object.description;
                    thisCompanyInfo.website = company.Object.website;
                }
            }

            companyName.Text = thisCompanyInfo.name;

            string imageName = thisCompanyInfo.name.ToLower().Replace(" ", "");
            int resourceId = (int)typeof(Resource.Drawable).GetField(imageName).GetValue(null);
            companyLogo.SetImageResource(resourceId);

            description.Text = thisCompanyInfo.description;

            website.Text = thisCompanyInfo.website;
            website.Click += Website_Click;
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
            //string dbPath_myCF = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), fileName);
            //var db_myCF = new SQLiteConnection(dbPath_myCF);

            Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(thisCompanyInfo.website));
            StartActivity(intent);
        }
    }
}