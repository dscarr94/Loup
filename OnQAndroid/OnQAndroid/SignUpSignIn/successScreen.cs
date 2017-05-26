using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using SQLite;
using OnQAndroid.FirebaseObjects;
using Firebase.Xamarin.Database;
using Android.Views;

namespace OnQAndroid
{
    [Activity(Label = "successScreen")]
    public class successScreen : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            // Create your application here
            SetContentView(Resource.Layout.SuccessScreen); // set view to SuccessScreen.axml
            ActionBar.Hide(); // hide the action bar

            Button advanceButton = FindViewById<Button>(Resource.Id.AdvanceButton); // get UI control for advanceButton

            advanceButton.Click += AdvanceButton_Click; // on button click
        }

        private const string FirebaseURL = "https://onqfirebase.firebaseio.com/";

        private void AdvanceButton_Click(object sender, EventArgs e)
        {            
            // connect to attributes database, get first row
            string dbPath_attributes = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "attributes.db3");
            var db_attributes = new SQLiteConnection(dbPath_attributes);
            MyAttributes userAttributes = db_attributes.Get<MyAttributes>(1);

            // connect to user database
            string dbPath_user = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "user.db3");
            var db_user = new SQLiteConnection(dbPath_user);

            // Update User Attributes
            userAttributes.cfid = 0; // no career fair id
            userAttributes.rememberme = false; // force log in on next use

            db_attributes.Update(userAttributes); // update attributes row
            
            // Create new LoginTable entry - converted to firebase
            LoginTable logintbl = new OnQAndroid.LoginTable();
            logintbl.name = userAttributes.name;
            logintbl.email = userAttributes.email;
            logintbl.password = userAttributes.password;
            logintbl.type = userAttributes.type;

            db_user.Insert(logintbl); // insert login information

            // Create new typeTable entry

            if (userAttributes.type == "Student")
            {
                StudentTable studenttbl = new StudentTable();
                studenttbl.name = userAttributes.name;
                studenttbl.email = userAttributes.email;
                studenttbl.password = userAttributes.password;
                studenttbl.school = userAttributes.attribute1;
                studenttbl.gradterm = userAttributes.attribute2;
                studenttbl.major = userAttributes.attribute3;
                studenttbl.gpa = userAttributes.attribute4;

                db_user.Insert(studenttbl); // insert student information
            }

            else if (userAttributes.type == "Recruiter")
            {
                RecruiterTable rectbl = new OnQAndroid.RecruiterTable();
                rectbl.name = userAttributes.name;
                rectbl.email = userAttributes.email;
                rectbl.password = userAttributes.password;
                rectbl.company = userAttributes.attribute1;

                db_user.Insert(rectbl); // insert recruiter attributes
            }

            CreateUser();
        }

        private async void CreateUser()
        {
            ProgressBar progressBar = FindViewById<ProgressBar>(Resource.Id.circularProgress);
            progressBar.Visibility = ViewStates.Visible;
            try
            {
                string dbPath_attributes = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "attributes.db3");
                var db_attributes = new SQLiteConnection(dbPath_attributes);
                MyAttributes userAttributes = db_attributes.Get<MyAttributes>(1);

                // Create user object
                User newUser = new User();
                newUser.name = userAttributes.name;
                newUser.email = userAttributes.email;
                newUser.password = userAttributes.password;
                newUser.type = userAttributes.type;
                newUser.cfid = "0";

                // Query user database for number of users, assign user id
                var firebase = new FirebaseClient(FirebaseURL);
                var items = await firebase.Child("users").OnceAsync<User>();
                int numUsers = items.Count();
                newUser.uid = (numUsers + 1).ToString();

                userAttributes.loginid = Convert.ToInt32(newUser.uid);
                db_attributes.Update(userAttributes);

                // insert object into user database
                var item = await firebase.Child("users").PostAsync(newUser);

                // create type object
                if (userAttributes.type == "Student")
                {
                    Student newStudent = new Student();
                    newStudent.name = userAttributes.name;
                    newStudent.email = userAttributes.email;
                    newStudent.password = userAttributes.password;
                    newStudent.school = userAttributes.attribute1;
                    newStudent.gradterm = userAttributes.attribute2;
                    newStudent.major = userAttributes.attribute3;
                    newStudent.gpa = userAttributes.attribute4;

                    // Query student database for number of students, assign student id
                    var studentItems = await firebase.Child("students").OnceAsync<Student>();
                    int numStudents = studentItems.Count();
                    newStudent.studentid = (numStudents + 1).ToString();
                    userAttributes.typeid = numStudents + 1;
                    db_attributes.Update(userAttributes);
                    // insert object into student database
                    var studentItem = await firebase.Child("students").PostAsync(newStudent);
                }
                else if (userAttributes.type == "Recruiter")
                {
                    Recruiter newRecruiter = new Recruiter();
                    newRecruiter.name = userAttributes.name;
                    newRecruiter.email = userAttributes.email;
                    newRecruiter.password = userAttributes.password;
                    newRecruiter.company = userAttributes.attribute1;

                    // Query recruiter database for number of students, assign recruiter id
                    var recruiterItems = await firebase.Child("recruiters").OnceAsync<Recruiter>();
                    int numRecruiters = recruiterItems.Count();
                    newRecruiter.recruiterid = (numRecruiters + 1).ToString();
                    userAttributes.typeid = numRecruiters + 1;
                    db_attributes.Update(userAttributes);
                    // insert object into recruiter database
                    var recruiterItem = await firebase.Child("recruiters").PostAsync(newRecruiter);
                }

                progressBar.Visibility = ViewStates.Invisible;
                var advanceIntent = new Intent(this, typeof(homeScreen2)).PutExtra("UserId", newUser.uid);
                StartActivity(advanceIntent);
                Finish();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.ToString(), ToastLength.Long).Show();
                progressBar.Visibility = ViewStates.Invisible;
            }
        }
    }
}