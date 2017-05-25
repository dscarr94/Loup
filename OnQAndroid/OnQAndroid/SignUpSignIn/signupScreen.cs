using System;
using System.IO;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using SQLite;
using Firebase.Xamarin.Database;
using OnQAndroid.FirebaseObjects;
using Android.Views;

namespace OnQAndroid
{
    [Activity(Label = "signupScreen")]
    public class signupScreen : Activity
    {
        Button finishButton; // initialize global buttons and text fields
        Button facebookButton;
        EditText newName;
        EditText newEmailAddress;
        EditText newPassword;
        EditText confirmNewPassword;

        private const string FirebaseURL = "https://onqfirebase.firebaseio.com/";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.signupScreen); // set view to sign up screen
            ActionBar.Hide(); // hide the action bar

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
        private async void FinishButton_Click(object sender, EventArgs e)
        {
            ProgressBar progressBar = FindViewById<ProgressBar>(Resource.Id.circularProgress);
            progressBar.Visibility = ViewStates.Visible;

            int numExistingLogins = 0;
            try
            {
                var firebase = new FirebaseClient(FirebaseURL);
                var allLogins = await firebase.Child("users").OnceAsync<User>();
                foreach (var login in allLogins)
                {
                    string email = login.Object.email;
                    if (email == newEmailAddress.Text)
                    {
                        numExistingLogins = numExistingLogins + 1;                       
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.ToString(), ToastLength.Long).Show();
            }

            string dbPath_attributes = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "attributes.db3");
            var db_attributes = new SQLiteConnection(dbPath_attributes);
            db_attributes.CreateTable<MyAttributes>();
            
            // Make sure passwords match
            if (newPassword.Text != confirmNewPassword.Text)
            {
                Toast.MakeText(this, "Passwords Do Not Match", ToastLength.Short).Show();
                progressBar.Visibility = ViewStates.Invisible;
            }

            // Make sure all fields are full
            else if (string.IsNullOrWhiteSpace(newName.Text) || string.IsNullOrWhiteSpace(newEmailAddress.Text) || string.IsNullOrWhiteSpace(newPassword.Text) || string.IsNullOrWhiteSpace(confirmNewPassword.Text))
            {
                Toast.MakeText(this, "Please Complete All Fields", ToastLength.Short).Show();
                progressBar.Visibility = ViewStates.Invisible;
            }
            // Make sure email address is not already in use
            else if (numExistingLogins != 0)
            {
                Toast.MakeText(this, "Email is Already in Use", ToastLength.Short).Show();
                progressBar.Visibility = ViewStates.Invisible;
            }
            else
            {
                try // make sure email address is correctly formatted
                {
                    string username = newEmailAddress.Text.Split('@')[0];
                    string domain = newEmailAddress.Text.Split('@')[1];
                    string provider = domain.Split('.')[0];
                    string extension = domain.Split('.')[1];
                    try
                    {
                        // make a myattributes object and populate it
                        MyAttributes tbl = new MyAttributes();
                        tbl.id = 1;
                        tbl.name = newName.Text;
                        tbl.email = newEmailAddress.Text;
                        tbl.password = newPassword.Text;
                        db_attributes.InsertOrReplace(tbl);
                        Toast.MakeText(this, "Success!", ToastLength.Short).Show();
                        progressBar.Visibility = ViewStates.Invisible;
                        var finishIntent = new Intent(this, typeof(IdentifyUserType));
                        StartActivity(finishIntent); // go to identify user type
                    }
                    catch (Exception ex)
                    {
                        Toast.MakeText(this, ex.ToString(), ToastLength.Short).Show();
                        progressBar.Visibility = ViewStates.Invisible;
                    }
                }
                catch
                {
                    Toast.MakeText(this, "Invalid Email Address", ToastLength.Short).Show();
                    progressBar.Visibility = ViewStates.Invisible;
                }
            }                             
        }
    }
}