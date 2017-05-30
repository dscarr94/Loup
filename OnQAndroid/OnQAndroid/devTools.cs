using System;
using Android.App;
using Android.OS;
using Android.Widget;
using SQLite;
using OnQAndroid.FirebaseObjects;
using Firebase.Xamarin.Database;
using Android.Views;
using Firebase.Xamarin.Database.Query;

namespace OnQAndroid
{
    [Activity(Label = "devTools")]
    public class devTools : Activity
    {
        private const string FirebaseURL = "https://onqfirebase.firebaseio.com/";
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.DevTools);
            Button generateCF = FindViewById<Button>(Resource.Id.cfGenerator);
            Button deleteUserData = FindViewById<Button>(Resource.Id.deleteUserData);
            Button deleteCF = FindViewById<Button>(Resource.Id.deleteCF);
            Button cleanUp = FindViewById<Button>(Resource.Id.cleanUp);
            Button fbCompanies = FindViewById<Button>(Resource.Id.fbCompanies);
            Button generateFBCF = FindViewById<Button>(Resource.Id.generatefbcf);

            generateCF.Click += GenerateCF_Click;
            deleteUserData.Click += DeleteUserData_Click;
            deleteCF.Click += DeleteCF_Click;
            cleanUp.Click += CleanUp_Click;
            fbCompanies.Click += FbCompanies_Click;
            generateFBCF.Click += GenerateFBCF_Click;
        }

        private async void GenerateFBCF_Click(object sender, EventArgs e)
        {
            ProgressBar progressBar = FindViewById<ProgressBar>(Resource.Id.circularProgress);
            progressBar.Visibility = ViewStates.Visible;
            Company newCompany1 = new Company();
            newCompany1.companyid = "1";
            newCompany1.name = "Facebook";
            newCompany1.description = "Description 1";
            newCompany1.website = "https://newsroom.fb.com/";
            newCompany1.rak = "facebook123";
            newCompany1.checkedIn = false;
            newCompany1.waittime = "3000000000";
            newCompany1.numstudents = "0";

            Company newCompany2 = new Company();
            newCompany2.companyid = "2";
            newCompany2.name = "Google";
            newCompany2.description = "Description 2";
            newCompany2.website = "https://www.google.com/intl/en/about/products/";
            newCompany2.rak = "google123";
            newCompany2.checkedIn = false;
            newCompany2.waittime = "3000000000";
            newCompany2.numstudents = "0";

            Company newCompany3 = new Company();
            newCompany3.companyid = "3";
            newCompany3.name = "Stryker";
            newCompany3.description = "Description 3";
            newCompany3.website = "http://www.stryker.com/en-us/index.htm";
            newCompany3.rak = "stryker123";
            newCompany3.checkedIn = false;
            newCompany3.waittime = "3000000000";
            newCompany3.numstudents = "0";

            Company newCompany4 = new Company();
            newCompany4.companyid = "4";
            newCompany4.name = "Aerojet Rocketdyne";
            newCompany4.description = "Description 4";
            newCompany4.website = "http://www.rocket.com/";
            newCompany4.rak = "ar123";
            newCompany4.checkedIn = false;
            newCompany4.waittime = "3000000000";
            newCompany4.numstudents = "0";

            Company newCompany5 = new Company();
            newCompany5.companyid = "5";
            newCompany5.name = "Boeing";
            newCompany5.description = "Description 5";
            newCompany5.website = "http://www.boeing.com/";
            newCompany5.rak = "boeing123";
            newCompany5.checkedIn = false;
            newCompany5.waittime = "3000000000";
            newCompany5.numstudents = "0";

            Company newCompany6 = new Company();
            newCompany6.companyid = "6";
            newCompany6.name = "Amazon";
            newCompany6.description = "Description 6";
            newCompany6.website = "https://www.amazon.com/p/feature/rzekmvyjojcp6uc";
            newCompany6.rak = "amazon123";
            newCompany6.checkedIn = false;
            newCompany6.waittime = "3000000000";
            newCompany6.numstudents = "0";

            var firebase = new FirebaseClient(FirebaseURL);
            var item1 = await firebase.Child("careerfairs").Child("12345678").PostAsync(newCompany1);
            var item2 = await firebase.Child("careerfairs").Child("12345678").PostAsync(newCompany2);
            var item3 = await firebase.Child("careerfairs").Child("12345678").PostAsync(newCompany3);
            var item4 = await firebase.Child("careerfairs").Child("12345678").PostAsync(newCompany4);
            var item5 = await firebase.Child("careerfairs").Child("12345678").PostAsync(newCompany5);
            var item6 = await firebase.Child("careerfairs").Child("12345678").PostAsync(newCompany6);

            Cfid newcfid = new Cfid();
            newcfid.id = "1";
            newcfid.cfid = "12345678";
            newcfid.name = "Cal Poly Spring Career Fair 2017";
            var cfid1 = await firebase.Child("cfids").PostAsync(newcfid);

            progressBar.Visibility = ViewStates.Invisible;
            Toast.MakeText(this, "Firebase Career Fair Generated!", ToastLength.Short).Show();
        }

        private async void FbCompanies_Click(object sender, EventArgs e)
        {
            ProgressBar progressBar = FindViewById<ProgressBar>(Resource.Id.circularProgress);
            progressBar.Visibility = ViewStates.Visible;
            Company newCompany1 = new Company();
            newCompany1.companyid = "1";
            newCompany1.name = "Facebook";
            newCompany1.description = "Description 1";
            newCompany1.website = "https://newsroom.fb.com/";
            newCompany1.rak = "facebook123";

            Company newCompany2 = new Company();
            newCompany2.companyid = "2";
            newCompany2.name = "Google";
            newCompany2.description = "Description 2";
            newCompany2.website = "https://www.google.com/intl/en/about/products/";
            newCompany2.rak = "google123";

            Company newCompany3 = new Company();
            newCompany3.companyid = "3";
            newCompany3.name = "Stryker";
            newCompany3.description = "Description 3";
            newCompany3.website = "http://www.stryker.com/en-us/index.htm";
            newCompany3.rak = "stryker123";

            Company newCompany4 = new Company();
            newCompany4.companyid = "4";
            newCompany4.name = "Aerojet Rocketdyne";
            newCompany4.description = "Description 4";
            newCompany4.website = "http://www.rocket.com/";
            newCompany4.rak = "ar123";

            Company newCompany5 = new Company();
            newCompany5.companyid = "5";
            newCompany5.name = "Boeing";
            newCompany5.description = "Description 5";
            newCompany5.website = "http://www.boeing.com/";
            newCompany5.rak = "boeing123";

            Company newCompany6 = new Company();
            newCompany6.companyid = "6";
            newCompany6.name = "Amazon";
            newCompany6.description = "Description 6";
            newCompany6.website = "https://www.amazon.com/p/feature/rzekmvyjojcp6uc";
            newCompany6.rak = "amazon123";

            var firebase = new FirebaseClient(FirebaseURL);
            var item1 = await firebase.Child("companies").PostAsync(newCompany1);
            var item2 = await firebase.Child("companies").PostAsync(newCompany2);
            var item3 = await firebase.Child("companies").PostAsync(newCompany3);
            var item4 = await firebase.Child("companies").PostAsync(newCompany4);
            var item5 = await firebase.Child("companies").PostAsync(newCompany5);
            var item6 = await firebase.Child("companies").PostAsync(newCompany6);
            progressBar.Visibility = ViewStates.Invisible;
            Toast.MakeText(this, "Firebase Companies Generated!", ToastLength.Short).Show();
        }

        private void CleanUp_Click(object sender, EventArgs e)
        {
            /*string dbpath_user = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "user.db3");
            var db_user = new SQLiteConnection(dbpath_user);

            int numUsers = db_user.Table<LoginTable>().Count();

            for (int i = 1; i <= numUsers; i++)
            {
                string fileName1 = "fav_12345678_" + i.ToString() + ".db3";
                string dbpath_fav = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), fileName1);
                var db_fav = new SQLiteConnection(dbpath_fav);

                string fileName2 = "myqs_12345678_" + i.ToString() + ".db3";
                string dbpath_myqs = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), fileName2);
                var db_myqs = new SQLiteConnection(dbpath_myqs);

                string fileName3 = "pastqs_" + i.ToString() + ".db3";
                string dbpath_pastqs = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), fileName3);
                var db_pastsqs = new SQLiteConnection(dbpath_pastqs);

                this.DeleteDatabase(dbpath_myqs);
                this.DeleteDatabase(dbpath_fav);
                this.DeleteDatabase(dbpath_pastqs);
            }

            string dbpath_allcompanies = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "allcompanies.db3");
            var db_allcompanies = new SQLiteConnection(dbpath_allcompanies);

            int numComps = db_allcompanies.Table<Companies>().Count();

            for (int i = 1; i<= numComps; i++)
            {
                string fileName1 = "mp_12345678" + "_" + db_allcompanies.Get<Companies>(i).name + ".db3";
                string dbpath_prefs = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), fileName1);
                string fileName2 = "qs_12345678" + "_" + db_allcompanies.Get<Companies>(i).name + ".db3";
                string dbpath_qs = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), fileName2);
                //var db_prefs = new SQLiteConnection(dbpath_prefs);
                string fileName3 = "pastqs_" + db_allcompanies.Get<Companies>(i).name + ".db3";
                string dbpath_pastqs = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), fileName3);
                this.DeleteDatabase(dbpath_prefs);
                this.DeleteDatabase(dbpath_qs);
                this.DeleteDatabase(dbpath_pastqs);
            }*/
            Toast.MakeText(this, "Nothing Happened", ToastLength.Short).Show();
        }

        private void DeleteCF_Click(object sender, EventArgs e)
        {
            /*string dbpath_myCF = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "12345678.db3");
            var db_myCF = new SQLiteConnection(dbpath_myCF);

            string dbpath_cfids = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "CFIDs.db3");
            var db_cfids = new SQLiteConnection(dbpath_cfids);

            db_cfids.DeleteAll<Cfids>();
            db_myCF.DeleteAll<Companies>();*/

            Toast.MakeText(this, "Nothing Happned", ToastLength.Short).Show();
        }

        private void DeleteUserData_Click(object sender, EventArgs e)
        {
            /*string dbpath_user = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "user.db3");
            var db_user = new SQLiteConnection(dbpath_user);

            db_user.DeleteAll<LoginTable>();
            db_user.DeleteAll<StudentTable>();
            db_user.DeleteAll<RecruiterTable>();

            this.DeleteDatabase(dbpath_user);

            string dbpath_attributes = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "attributes.db3");
            SQLiteConnection db_attributes = new SQLiteConnection(dbpath_attributes);

            db_attributes.DeleteAll<MyAttributes>();

            this.DeleteDatabase(dbpath_attributes);

            var db_usernew = new SQLiteConnection(dbpath_user);
            db_usernew.CreateTable<LoginTable>();
            db_usernew.CreateTable<StudentTable>();
            db_usernew.CreateTable<RecruiterTable>();*/

            Toast.MakeText(this, "Nothing Happned", ToastLength.Short).Show();
        }

        private void GenerateCF_Click(object sender, EventArgs e)
        {
            /*string dbpath_cfids = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "CFIDs.db3");
            string dbpath_myCF = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "12345678.db3");
            string dbpath_allcompanies = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "allcompanies.db3");

            var db_cfids = new SQLiteConnection(dbpath_cfids);
            var db_myCF = new SQLiteConnection(dbpath_myCF);
            var db_allcompanies = new SQLiteConnection(dbpath_allcompanies);

            db_cfids.CreateTable<Cfids>();
            db_myCF.CreateTable<Companies>();
            db_allcompanies.CreateTable<Companies>();

            Cfids newCfids = new Cfids();
            newCfids.id = 1;
            newCfids.cfid = "12345678";
            newCfids.name = "Cal Poly Spring Career Fair 2017";

            db_cfids.InsertOrReplace(newCfids);

            Companies newCompany1 = new Companies();
            newCompany1.id = 1;
            newCompany1.name = "Facebook";
            newCompany1.description = "Description 1";
            newCompany1.website = "https://newsroom.fb.com/";
            newCompany1.rak = "facebook123";

            Companies newCompany2 = new Companies();
            newCompany2.id = 2;
            newCompany2.name = "Google";
            newCompany2.description = "Description 2";
            newCompany2.website = "https://www.google.com/intl/en/about/products/";
            newCompany2.rak = "google123";

            Companies newCompany3 = new Companies();
            newCompany3.id = 3;
            newCompany3.name = "Stryker";
            newCompany3.description = "Description 3";
            newCompany3.website = "http://www.stryker.com/en-us/index.htm";
            newCompany3.rak = "stryker123";

            Companies newCompany4 = new Companies();
            newCompany4.id = 4;
            newCompany4.name = "Aerojet Rocketdyne";
            newCompany4.description = "Description 4";
            newCompany4.website = "http://www.rocket.com/";
            newCompany4.rak = "ar123";

            Companies newCompany5 = new Companies();
            newCompany5.id = 5;
            newCompany5.name = "Boeing";
            newCompany5.description = "Description 5";
            newCompany5.website = "http://www.boeing.com/";
            newCompany5.rak = "boeing123";

            Companies newCompany6 = new Companies();
            newCompany6.id = 6;
            newCompany6.name = "Amazon";
            newCompany6.description = "Description 6";
            newCompany6.website = "https://www.amazon.com/p/feature/rzekmvyjojcp6uc";
            newCompany6.rak = "amazon123";

            db_myCF.InsertOrReplace(newCompany1);
            db_myCF.InsertOrReplace(newCompany2);
            db_myCF.InsertOrReplace(newCompany3);
            db_myCF.InsertOrReplace(newCompany4);
            db_myCF.InsertOrReplace(newCompany5);
            db_myCF.InsertOrReplace(newCompany6);

            db_allcompanies.InsertOrReplace(newCompany1);
            db_allcompanies.InsertOrReplace(newCompany2);
            db_allcompanies.InsertOrReplace(newCompany3);
            db_allcompanies.InsertOrReplace(newCompany4);
            db_allcompanies.InsertOrReplace(newCompany5);
            db_allcompanies.InsertOrReplace(newCompany6);*/

            Toast.MakeText(this, "Nothing Happened", ToastLength.Short).Show();
        }
    }
}