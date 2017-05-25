using System;
using System.IO;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using SQLite;

namespace OnQAndroid
{
    [Activity(Label = "IdentifyUserType")]
    public class IdentifyUserType : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.IdentifyUserType); // set the view to IdentifyUserType.axml
            ActionBar.Hide(); // hide the action bar

            // Get UI Controls
            Button studentIdentifier = FindViewById<Button>(Resource.Id.studentIdentify);
            Button recruiterIdentifier = FindViewById<Button>(Resource.Id.recruiterIdentify);
            Button csIdentifier = FindViewById<Button>(Resource.Id.CSIdentify);

            // SQLite Connection to attributes database
            string dbPath_attributes = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "attributes.db3");
            var db_attributes = new SQLiteConnection(dbPath_attributes);
                      
            var myAttributes = db_attributes.Get<MyAttributes>(1);
            string myName = myAttributes.name;
            string myEmail = myAttributes.email;
            string myPassword = myAttributes.password;

            // Add Code to buttons
            studentIdentifier.Click += (object sender, EventArgs e) => // anonymous function
            {
                var newStudent = new MyAttributes();
                newStudent.id = 1;
                newStudent.name = myName;
                newStudent.email = myEmail;
                newStudent.password = myPassword;
                newStudent.type = "Student"; // new data
                db_attributes.InsertOrReplace(newStudent);
                var advanceIntent = new Intent(this, typeof(selectSchool));
                StartActivity(advanceIntent); // go to select school if student selected
                Finish(); // finish this activity
            };
            recruiterIdentifier.Click += (object sender, EventArgs e) =>
            {
                var newRecruiter = new MyAttributes();
                newRecruiter.id = 1;
                newRecruiter.name = myName;
                newRecruiter.email = myEmail;
                newRecruiter.password = myPassword;
                newRecruiter.type = "Recruiter"; // new data
                db_attributes.InsertOrReplace(newRecruiter);
                var advanceIntent = new Intent(this, typeof(recruiterBuildProfile));
                StartActivity(advanceIntent); // go to recruiterBuildProfile if recruiter selected
                Finish(); // finish this activity
            };
            csIdentifier.Click += (object sender, EventArgs e) =>
            {
                var newCS = new MyAttributes();
                newCS.id = 1;
                newCS.name = myName;
                newCS.email = myEmail;
                newCS.password = myPassword;
                newCS.type = "Career Services"; // new data
                db_attributes.InsertOrReplace(newCS);
                var advanceIntent = new Intent(this, typeof(csBuildProfile));
                StartActivity(advanceIntent); // go to csBuildProfile
                Finish(); // finish this activity
            };
        }
    }
}