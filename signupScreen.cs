using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
using System.IO;

using Android.App;
using Android.Content;
using Android.OS;
//using Android.Runtime;
//using Android.Views;
using Android.Widget;

using SQLite;
//using SQLitePCL;

//using Amazon;
//using Amazon.CognitoIdentity;

namespace OnQAndroid
{
    [Activity(Label = "signupScreen")]
    public class signupScreen : Activity
    {
        Button finishButton;
        Button facebookButton;
        EditText newName;
        EditText newEmailAddress;
        EditText newPassword;
        EditText confirmNewPassword;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.signupScreen);

            // Get UI Controls
            finishButton = FindViewById<Button>(Resource.Id.finish);
            facebookButton = FindViewById<Button>(Resource.Id.facebook);
            newName = FindViewById<EditText>(Resource.Id.FullName);
            newEmailAddress = FindViewById<EditText>(Resource.Id.NewEmail);
            newPassword = FindViewById<EditText>(Resource.Id.NewPassword);
            confirmNewPassword = FindViewById<EditText>(Resource.Id.ConfirmNewPassword);

            // On Button Clicks
            finishButton.Click += FinishButton_Click;
        }
        private void FinishButton_Click(object sender, EventArgs e)
        {
            string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "user.db3");
            var db = new SQLiteConnection(dbPath);
            db.CreateTable<LoginTable>();
            var existingLogin = db.Query<LoginTable>("SELECT * FROM LoginTable WHERE email = ?", newEmailAddress.Text);
            
            // Make sure passwords match
            if (newPassword.Text != confirmNewPassword.Text)
            {
                Toast.MakeText(this, "Passwords Do Not Match", ToastLength.Short).Show();
            }
            // Make sure all fields are full
            else if (string.IsNullOrWhiteSpace(newName.Text) || string.IsNullOrWhiteSpace(newEmailAddress.Text) || string.IsNullOrWhiteSpace(newPassword.Text) || string.IsNullOrWhiteSpace(confirmNewPassword.Text))
            {
                Toast.MakeText(this, "Please Complete All Fields", ToastLength.Short).Show();
            }
            // Make sure email address is not already in use
            else if (existingLogin.Count != 0)
            {
                Toast.MakeText(this, "Email is Already in Use", ToastLength.Short).Show();
                // Admin
                if (newName.Text == "Admin")
                    {
                        var finishIntent = new Intent(this, typeof(IdentifyUserType)).PutExtra("MyId", 1); ;
                        StartActivity(finishIntent);
                    }
            }
            // Now put information in database
            else
            {
                try
                {
                    LoginTable tbl = new LoginTable();                   
                    tbl.name = newName.Text;
                    tbl.email = newEmailAddress.Text;
                    tbl.password = newPassword.Text;
                    db.Insert(tbl);
                    int id = tbl.id;
                    Toast.MakeText(this, "Success!", ToastLength.Short).Show();
                    var finishIntent = new Intent(this, typeof(IdentifyUserType)).PutExtra("MyId", id);
                    StartActivity(finishIntent);
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, ex.ToString(), ToastLength.Short).Show();
                }
            }
            /*else
            {
                // Obtain Credentials
                CognitoAWSCredentials credentials = new CognitoAWSCredentials("us-west-2:1c6a55f3-cae3-4590-b1f4-62f8cf269921", RegionEndpoint.USWest2);

                // Create Instance
                // finishButton.Click += 

            }*/
        }
    }
}