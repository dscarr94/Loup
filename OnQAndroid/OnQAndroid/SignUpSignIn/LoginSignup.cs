using System;
using System.Linq;
using System.IO;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using SQLite;
using Firebase.Xamarin.Database;
using OnQAndroid.FirebaseObjects;
using Android.Views;
using Android.Views.InputMethods;
// converted to firebase vvv
namespace OnQAndroid
{
    [Activity(Label = "LoginSignup")]
    public class LoginSignup : Activity
    {
        Button loginButton; // initialize login button
        Button signupButton; // initialize sign up button
        EditText tfEmail; // initialize email text field
        EditText tfPassword; // initialize password text field
        TextView forgotPassword; // initialize forgot password button
        ImageView rememberMe;
        bool shouldRemember;

        private const string FirebaseURL = "https://onqfirebase.firebaseio.com/";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            ActionBar.Hide(); // hides the action bar

            // Create your application here
            SetContentView(Resource.Layout.LoginSignup); // sets the view to LoginSignup axml file
            loginButton = FindViewById<Button>(Resource.Id.LoginButton); // gets login button from view
            signupButton = FindViewById<Button>(Resource.Id.SignupButton);
            tfEmail = FindViewById<EditText>(Resource.Id.TFemail);
            tfPassword = FindViewById<EditText>(Resource.Id.TFpassword);
            forgotPassword = FindViewById<TextView>(Resource.Id.forgotpassword);
            Button devTools = FindViewById<Button>(Resource.Id.devTools); // initializes and gets button from view
            LinearLayout ll_main = FindViewById<LinearLayout>(Resource.Id.rootLayout);
            rememberMe = FindViewById<ImageView>(Resource.Id.rememberMe);
            TextView rememberMeText = FindViewById<TextView>(Resource.Id.rememberMeText);
            shouldRemember = false;


            ll_main.RequestFocus();
            //InputMethodManager imm = (InputMethodManager)this.GetSystemService(Context.InputMethodService);
            //imm.HideSoftInputFromInputMethod(tfEmail.WindowToken, 0);

            rememberMe.Click  += (sender, e) =>
            {
                if (shouldRemember == false)
                {
                    shouldRemember = true;
                    rememberMe.SetImageResource(Resource.Drawable.radio_checked);
                }
                else if (shouldRemember == true)
                {
                    shouldRemember = false;
                    rememberMe.SetImageResource(Resource.Drawable.radio_unchecked);
                }
            };

            rememberMeText.Click += (sender, e) =>
            {
                if (shouldRemember == false)
                {
                    shouldRemember = true;
                    rememberMe.SetImageResource(Resource.Drawable.radio_checked);
                }
                else if (shouldRemember == true)
                {
                    shouldRemember = false;
                    rememberMe.SetImageResource(Resource.Drawable.radio_unchecked);
                }
            };

            // Disable login button until username and password are entered
            loginButton.Enabled = false; // disables login button
            tfEmail.TextChanged += (sender, e) => // when you change the text of email text field
            {
                string username = tfEmail.Text; // new string called username is what is in the email text field
                string password = tfPassword.Text; // new string called password is what is in the password text field
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password)) // if either field is blank
                    loginButton.Enabled = false; // disable login button
                else // otherwise
                    loginButton.Enabled = true; // enable login button
            };

            tfPassword.TextChanged += (sender, e) => // when you change the text of password text field
            {
                string username = tfEmail.Text;
                string password = tfPassword.Text;
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                    loginButton.Enabled = false;
                else
                    loginButton.Enabled = true;
            };

            // Define what happens on "login" click
            loginButton.Click += LoginButton_Click; // calls login button click event

            // Define what happens on "forgot password" click
            forgotPassword.Click += (s, e) => // anonymous function
            {
                var forgotPasswordIntent = new Intent(this, typeof(passwordRecover)); // create new intent of type password recover
                StartActivity(forgotPasswordIntent); // start activity using intent
            };

            // Define what happens on "sign up" click
            signupButton.Click += SignupButton_Click; // call sign up button click event

            devTools.Visibility = Android.Views.ViewStates.Invisible;
            devTools.Click += (s, e) => // anonymous function
            {
                var devToolsIntent = new Intent(this, typeof(devTools)); // create an intent of type devTools
                StartActivity(devToolsIntent); // start activity using intent
            };

        }
        private async void LoginButton_Click(object sender, EventArgs e) // login button click event
        {
            ProgressBar progressBar = FindViewById<ProgressBar>(Resource.Id.circularProgress);
            progressBar.Visibility = ViewStates.Visible;

            string dbPath_attributes = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "attributes.db3");
            SQLiteConnection db_attributes = new SQLiteConnection(dbPath_attributes);

            var firebase = new FirebaseClient(FirebaseURL);
            var allLogins = await firebase.Child("users").OnceAsync<User>();
            int numLogins = allLogins.Count();

            if (allLogins.Count() == 0) // if there are no rows in the data table
            {
                Toast.MakeText(this, "Username or Password Invalid", ToastLength.Short).Show(); // display invalid 
                progressBar.Visibility = ViewStates.Invisible;
            }
            else
            {
                try
                {
                    // query our data 
                    bool loginExists = false;
                    string uid = "0";
                    string name = "";
                    string email = "";
                    string password = "";
                    string type = "";
                    string attribute1 = "";
                    string attribute2 = "";
                    string attribute3 = "";
                    string attribute4 = "";
                    int cfid = 0;
                    int typeid = 0;

                    foreach (var login in allLogins)
                    {
                        email = login.Object.email;
                        password = login.Object.password;
                        if (tfEmail.Text == email && tfPassword.Text == password)
                        {
                            loginExists = true;
                            uid = login.Object.uid;
                            name = login.Object.name;
                            type = login.Object.type;
                            cfid = Convert.ToInt32(login.Object.cfid);
                            break;
                        }
                    }

                    if (loginExists == true)
                    {
                        if (type == "Student")
                        {
                            var allStudents = await firebase.Child("students").OnceAsync<Student>();

                            foreach (var student in allStudents)
                            {
                                if (student.Object.email == email)
                                {
                                    attribute1 = student.Object.school;
                                    attribute2 = student.Object.gradterm;
                                    attribute3 = student.Object.major;
                                    attribute4 = student.Object.gpa;
                                    typeid = Convert.ToInt32(student.Object.studentid);
                                    break;
                                }
                            }
                        }
                        else if (type == "Recruiter")
                        {
                            var allRecruiters = await firebase.Child("recruiters").OnceAsync<Recruiter>();

                            foreach (var recruiter in allRecruiters)
                            {
                                if (recruiter.Object.email == email)
                                {
                                    attribute1 = recruiter.Object.company;
                                    attribute2 = "";
                                    attribute3 = "";
                                    attribute4 = "";
                                    typeid = Convert.ToInt32(recruiter.Object.recruiterid);
                                }
                            }
                        }

                        MyAttributes newAttributes = new MyAttributes();
                        newAttributes.id = 1;
                        newAttributes.name = name;
                        newAttributes.email = email;
                        newAttributes.password = password;
                        newAttributes.type = type;
                        newAttributes.attribute1 = attribute1;
                        newAttributes.attribute2 = attribute2;
                        newAttributes.attribute3 = attribute3;
                        newAttributes.attribute4 = attribute4;
                        newAttributes.cfid = cfid;
                        newAttributes.loginid = Convert.ToInt32(uid);
                        newAttributes.typeid = Convert.ToInt32(typeid);
                        newAttributes.rememberme = shouldRemember;

                        db_attributes.InsertOrReplace(newAttributes);
                        //LoginTable myAttributes = queryResults.First();

                        Toast.MakeText(this, "Login Successful!", ToastLength.Short).Show(); // show login successful
                        progressBar.Visibility = ViewStates.Invisible;
                        var loginIntent = new Intent(this, typeof(homeScreen2)).PutExtra("UserId", uid); // advance to home screen, send my id
                        StartActivity(loginIntent); // start activity using intent
                        Finish();
                    }
                    else
                    {
                        Toast.MakeText(this, "Username or Password Invalid", ToastLength.Short).Show();
                        progressBar.Visibility = ViewStates.Invisible;
                    }
                }
                catch
                {
                    Toast.MakeText(this, "Username or Password Invalid", ToastLength.Short).Show(); // otherwise display text that says username or password invalid
                    progressBar.Visibility = ViewStates.Invisible;
                }
            }
        }

        private void SignupButton_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(signupScreen)); // start sign up screen
        }
    }
}