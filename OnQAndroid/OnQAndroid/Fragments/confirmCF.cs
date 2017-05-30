using System;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using SQLite;
using Android.Support.V4.View;
using Firebase.Xamarin.Database;
using Firebase.Xamarin.Database.Query;
using OnQAndroid.FirebaseObjects;

namespace OnQAndroid
{
    public class confirmCF : Android.Support.V4.App.Fragment
    {
        string myCFID;
        string myCFName;
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
        private const string FirebaseURL = "https://onqfirebase.firebaseio.com/";
        ProgressBar progressBar;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Bundle arguments = Arguments;
            myCFID = arguments.GetString("CFID");
            myCFName = arguments.GetString("cfName");
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.ConfirmCF, container, false);
            TextView thisCareerFair = view.FindViewById<TextView>(Resource.Id.thisCareerFair);
            ImageView cfLogo = view.FindViewById<ImageView>(Resource.Id.cfImage);
            Button cancelBtn = view.FindViewById<Button>(Resource.Id.cancelCF);
            Button confirmBtn = view.FindViewById<Button>(Resource.Id.yes);
            progressBar = view.FindViewById<ProgressBar>(Resource.Id.circularProgress);
            viewPager = this.Activity.FindViewById<ViewPager>(Resource.Id.viewpager);

            thisCareerFair.Text = myCFName;

            string imageName = "img" + myCFID;
            int resourceId = (int)typeof(Resource.Drawable).GetField(imageName).GetValue(null);
            cfLogo.SetImageResource(resourceId);

            cancelBtn.Click += CancelBtn_Click;
            confirmBtn.Click += ConfirmBtn_Click;

            return view;
        }

        private async void ConfirmBtn_Click(object sender, EventArgs e)
        {
            progressBar.Visibility = ViewStates.Visible;
            string dbPath_attributes = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "attributes.db3");
            var db_attributes = new SQLiteConnection(dbPath_attributes);

            MyAttributes myAttributes = db_attributes.Get<MyAttributes>(1);
            myAttributes.cfid = Convert.ToInt32(myCFID);
            db_attributes.Update(myAttributes);

            var firebase = new FirebaseClient(FirebaseURL);
            var allLogins = await firebase.Child("users").OnceAsync<User>();
            string key = "";

            foreach (var login in allLogins)
            {
                if (login.Object.email == myAttributes.email)
                {
                    key = login.Key;
                }
            }

            User updateUser = new User();
            updateUser.uid = myAttributes.loginid.ToString();
            updateUser.name = myAttributes.name;
            updateUser.email = myAttributes.email;
            updateUser.password = myAttributes.password;
            updateUser.type = myAttributes.type;
            updateUser.cfid = myAttributes.cfid.ToString();
            await firebase.Child("users").Child(key).PutAsync(updateUser);

            if (myAttributes.type == "Student")
            {
                string favorites_fileName = "fav_" + myCFID + "_" + myAttributes.typeid.ToString();

                var allCompanies = await firebase.Child(myAttributes.cfid.ToString()).OnceAsync<Company>();

                int numCompanies = allCompanies.Count();
                foreach (var company in allCompanies)
                {
                    Favorite item = new Favorite();
                    item.companyid = company.Object.companyid;
                    item.name = company.Object.name;
                    item.isFavorite = false;
                    await firebase.Child(favorites_fileName).PostAsync(item);
                }

                Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();
                trans.Replace(Resource.Id.register_root_frame, new RegisterFragment());
                trans.Commit();

                viewPager.SetCurrentItem(0, true);
            }

            else if (myAttributes.type == "Recruiter")
            {
                var allCompanies = await firebase.Child(myAttributes.cfid.ToString()).OnceAsync<Company>();

                Company newCompany = new Company();
                string companyKey = "";

                foreach (var company in allCompanies)
                {
                    if (company.Object.name == myAttributes.attribute1)
                    {
                        companyKey = company.Key;
                        newCompany.companyid = company.Object.companyid;
                        newCompany.name = company.Object.name;
                        newCompany.description = company.Object.description;
                        newCompany.website = company.Object.website;
                        newCompany.rak = company.Object.rak;
                        newCompany.checkedIn = true;
                        newCompany.waittime = company.Object.waittime;
                        newCompany.numstudents = company.Object.numstudents;
                    }
                }

                await firebase.Child(myAttributes.cfid.ToString()).Child(companyKey).PutAsync(newCompany);

                Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();
                trans.Replace(Resource.Id.register_root_frame, new RegisterFragment());
                trans.Commit();

                viewPager.SetCurrentItem(0, true);
            }
            progressBar.Visibility = ViewStates.Invisible;
        }

        private void CancelBtn_Click(object sender, EventArgs e)
        {            
            Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();
            trans.Replace(Resource.Id.register_root_frame, new RegisterFragment());
            trans.Commit();
        }
    }
}