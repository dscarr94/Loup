using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using SQLite;

namespace OnQAndroid
{
    [Activity(Label = "Home")]
    public class IdentifyUserType : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.IdentifyUserType);

            // Get UI Controls
            Button studentIdentifier = FindViewById<Button>(Resource.Id.studentIdentify);
            Button recruiterIdentifier = FindViewById<Button>(Resource.Id.recruiterIdentify);
            Button csIdentifier = FindViewById<Button>(Resource.Id.CSIdentify);

            // SQLite Connection
            string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "user.db3");
            var db = new SQLiteConnection(dbPath);
            
            // Receive ID
            int id = Intent.GetIntExtra("MyId", -1);
            var myAttributes = db.Get<LoginTable>(id);
            string myName = myAttributes.name;
            string myEmail = myAttributes.email;
            string myPassword = myAttributes.password;

            // Add Code to buttons
            studentIdentifier.Click += (object sender, EventArgs e) =>
            {
                var newStudent = new LoginTable();
                newStudent.id = id;
                newStudent.name = myName;
                newStudent.email = myEmail;
                newStudent.password = myPassword;
                newStudent.type = "Student";
                db.Update(newStudent);
                var advanceIntent = new Intent(this, typeof(studentBuildProfile)).PutExtra("MyId", id);
                StartActivity(advanceIntent);
            };
            recruiterIdentifier.Click += (object sender, EventArgs e) =>
            {
                var newRecruiter = new LoginTable();
                newRecruiter.id = id;
                newRecruiter.name = myName;
                newRecruiter.email = myEmail;
                newRecruiter.password = myPassword;
                newRecruiter.type = "Recruiter";
                db.Update(newRecruiter);
                var advanceIntent = new Intent(this, typeof(recruiterBuildProfile)).PutExtra("MyId", id);
                StartActivity(advanceIntent);
            };
            csIdentifier.Click += (object sender, EventArgs e) =>
            {
                var newCS = new LoginTable();
                newCS.id = id;
                newCS.name = myName;
                newCS.email = myEmail;
                newCS.password = myPassword;
                newCS.type = "Career Services";
                db.Update(newCS);
                var advanceIntent = new Intent(this, typeof(csBuildProfile)).PutExtra("MyId", id);
                StartActivity(advanceIntent);
            };
        }
    }
}