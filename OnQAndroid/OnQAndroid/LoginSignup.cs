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
using Android.Text;

using SQLite;

namespace OnQAndroid
{
    [Activity(Label = "LoginSignup")]
    public class LoginSignup : Activity
    {
        Button loginButton;
        Button signupButton;
        EditText tfEmail;
        EditText tfPassword;
        TextView forgotPassword;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.LoginSignup);
            loginButton = FindViewById<Button>(Resource.Id.LoginButton);
            signupButton = FindViewById<Button>(Resource.Id.SignupButton);
            tfEmail = FindViewById<EditText>(Resource.Id.TFemail);
            tfPassword = FindViewById<EditText>(Resource.Id.TFpassword);
            forgotPassword = FindViewById<TextView>(Resource.Id.forgotpassword);
            CreateDB();

            // Disable login button until username and password are entered
            loginButton.Enabled = false;
            tfEmail.TextChanged += (sender, e) =>
            {
                string username = tfEmail.Text;
                string password = tfPassword.Text;
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                    loginButton.Enabled = false;
                else
                    loginButton.Enabled = true;
            };
            tfPassword.TextChanged += (sender, e) =>
            {
                string username = tfEmail.Text;
                string password = tfPassword.Text;
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                    loginButton.Enabled = false;
                else
                    loginButton.Enabled = true;
            };

            // Define what happens on "login" click
            loginButton.Click += LoginButton_Click;

            // Define what happens on "forgot password" click
            forgotPassword.Click += (s, e) =>
            {
                var forgotPasswordIntent = new Intent(this, typeof(passwordRecover));
                StartActivity(forgotPasswordIntent);
            };

            // Define what happens on "sign up" click
            signupButton.Click += SignupButton_Click;

        }
        private void LoginButton_Click(object sender, EventArgs e)
        {
            try
            {
                string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "user.db3"); // Call Database
                var db = new SQLiteConnection(dbPath);
                var data = db.Table<LoginTable>(); // Call Table
                var data1 = data.Where(x => x.email == tfEmail.Text && x.password == tfPassword.Text).FirstOrDefault();
                if (data1 != null)
                {
                    Toast.MakeText(this, "Login Successful!", ToastLength.Short).Show();
                    var loginIntent = new Intent(this, typeof(homeScreen));
                    StartActivity(loginIntent);
                }
                else
                {
                    Toast.MakeText(this, "Username or Password Invalid", ToastLength.Short).Show();
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.ToString(), ToastLength.Short).Show();
            }
        }
        private void SignupButton_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(signupScreen));
        }
        public string CreateDB()
        {
            var output = "";
            output += "Creating Database if it doesn't exists";
            string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "user.db3"); //Create New Database  
            var db = new SQLiteConnection(dbPath);
            output += "\n Database Created....";
            return output;
        }
    }
}