using System;
using System.IO;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using SQLite;

namespace OnQAndroid
{
    [Activity(Label = "selectSchool")]
    public class selectSchool : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.SelectSchool);
            ActionBar.Hide();

            Button advanceButton = FindViewById<Button>(Resource.Id.nextButton);
            Spinner schoolSpinner = FindViewById<Spinner>(Resource.Id.schoolSpinner);

            // Give Functionality to spinner
            schoolSpinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(schoolSpinner_ItemSelected);
            var schooladapter = ArrayAdapter.CreateFromResource(this, Resource.Array.school_array, Android.Resource.Layout.SimpleSpinnerItem);
            schooladapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            schoolSpinner.Adapter = schooladapter;

            // Give functionality to button
            advanceButton.Click += AdvanceButton_Click;
        }

        private void AdvanceButton_Click(object sender, EventArgs e)
        {
            // Connect to database, get user attributes
            string dbPath_attributes = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "attributes.db3");
            var db_attributes = new SQLiteConnection(dbPath_attributes);
            var myAttributes = db_attributes.Get<MyAttributes>(1);
            Spinner schoolSpinner = FindViewById<Spinner>(Resource.Id.schoolSpinner);

            string school = string.Format("{0}", schoolSpinner.SelectedItem);

            MyAttributes tbl = new MyAttributes();

            tbl.id = 1;
            string name = myAttributes.name;
            tbl.name = name;
            string email = myAttributes.email;
            tbl.email = email;
            string password = myAttributes.password; // May not be necessary to store again, but good to keep all the info together
            tbl.password = password;
            string type = myAttributes.type;
            tbl.type = type;
            
            tbl.attribute1 = school;

            db_attributes.InsertOrReplace(tbl);

            var advanceIntent = new Intent(this, typeof(studentBuildProfile));
            StartActivity(advanceIntent);
            Finish();
        }

        private void schoolSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner schoolSpinner = (Spinner)sender;
            string school = schoolSpinner.GetItemAtPosition(e.Position).ToString();
            ImageView schoolLogo = FindViewById<ImageView>(Resource.Id.schoolLogo);
            string fileName = school.ToLower().Replace(" ", "");
            int resourceId = (int)typeof(Resource.Drawable).GetField(fileName).GetValue(null);
            schoolLogo.SetImageResource(resourceId);
        }
    }
}