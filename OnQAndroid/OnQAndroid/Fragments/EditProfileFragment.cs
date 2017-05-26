using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using System.IO;
using SQLite;
using Firebase.Xamarin.Database;
using OnQAndroid.FirebaseObjects;
using Firebase.Xamarin.Database.Query;

namespace OnQAndroid
{
    public class EditProfileFragment : Android.Support.V4.App.Fragment
    {
        public EditProfileFragment()
        {
            // Required empty public constructor
        }

        public static EditProfileFragment newInstance()
        {
            EditProfileFragment fragment = new EditProfileFragment();
            return fragment;
        }

        EditText updateName;
        EditText updateEmail;
        EditText rakField;
        Spinner school;
        Spinner major;
        Spinner gpa;
        Spinner gradterm;
        Spinner company;
        public bool loginExists;
        string existingId;
        MyAttributes myAttributes;
        ProgressBar progressBar;
        public string key;

        private const string FirebaseURL = "https://onqfirebase.firebaseio.com/";

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            string dbPath_attributes = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "attributes.db3");
            var db_attributes = new SQLiteConnection(dbPath_attributes);

            myAttributes = db_attributes.Get<MyAttributes>(1);

            string type = myAttributes.type;

            if (type == "Student")
            {
                View view = inflater.Inflate(Resource.Layout.StudentEditProfileTab, container, false);
                updateName = view.FindViewById<EditText>(Resource.Id.updateName);
                updateEmail = view.FindViewById<EditText>(Resource.Id.updateEmail);

                progressBar = view.FindViewById<ProgressBar>(Resource.Id.circularProgress);
                school = view.FindViewById<Spinner>(Resource.Id.updateSchool);
                major = view.FindViewById<Spinner>(Resource.Id.updateMajor);
                gpa = view.FindViewById<Spinner>(Resource.Id.updateGPA);
                gradterm = view.FindViewById<Spinner>(Resource.Id.updateGradTerm);
                Button confirmChanges = view.FindViewById<Button>(Resource.Id.saveChangesEditProfile);
                Button cancel = view.FindViewById<Button>(Resource.Id.cancelEditProfile);

                updateName.Text = myAttributes.name;
                updateEmail.Text = myAttributes.email;

                confirmChanges.Click += ConfirmChanges_Click;
                cancel.Click += Cancel_Click;

                school.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(school_ItemSelected);
                var schooladapter = ArrayAdapter.CreateFromResource(this.Activity, Resource.Array.school_array, Android.Resource.Layout.SimpleSpinnerItem);
                schooladapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                school.Adapter = schooladapter;
                string defaultSchool = myAttributes.attribute1;

                if (!defaultSchool.Equals(null))
                {
                    int spinnerPosition = schooladapter.GetPosition(defaultSchool);
                    school.SetSelection(spinnerPosition);
                }

                major.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(major_ItemSelected);
                var majoradapter = ArrayAdapter.CreateFromResource(this.Activity, Resource.Array.major_array, Android.Resource.Layout.SimpleSpinnerItem);
                majoradapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                major.Adapter = majoradapter;
                string defaultMajor = myAttributes.attribute3;

                if (!defaultMajor.Equals(null))
                {
                    int spinnerPosition = majoradapter.GetPosition(defaultMajor);
                    major.SetSelection(spinnerPosition);
                }

                gpa.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(gpa_ItemSelected);
                var gpaadapter = ArrayAdapter.CreateFromResource(this.Activity, Resource.Array.gpa_array, Android.Resource.Layout.SimpleSpinnerItem);
                gpaadapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                gpa.Adapter = gpaadapter;
                string defaultGPA = myAttributes.attribute4;

                if (!defaultGPA.Equals(null))
                {
                    int spinnerPosition = gpaadapter.GetPosition(defaultGPA);
                    gpa.SetSelection(spinnerPosition);
                }

                gradterm.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(gradterm_ItemSelected);
                var gradtermadapter = ArrayAdapter.CreateFromResource(this.Activity, Resource.Array.year_array, Android.Resource.Layout.SimpleSpinnerItem);
                gradtermadapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                gradterm.Adapter = gradtermadapter;
                string defaultGradTerm = myAttributes.attribute2;

                if (!defaultGradTerm.Equals(null))
                {
                    int spinnerPosition = gradtermadapter.GetPosition(defaultGradTerm);
                    gradterm.SetSelection(spinnerPosition);
                }

                return view;
            }

            else if (type == "Recruiter")
            {
                View view = inflater.Inflate(Resource.Layout.RecruiterEditProfileTab, container, false);
                updateName = view.FindViewById<EditText>(Resource.Id.updateName);
                updateEmail = view.FindViewById<EditText>(Resource.Id.updateEmail);
                company = view.FindViewById<Spinner>(Resource.Id.updateMyCompany);
                rakField = view.FindViewById<EditText>(Resource.Id.rak);
                progressBar = view.FindViewById<ProgressBar>(Resource.Id.circularProgress);

                Button confirmChanges = view.FindViewById<Button>(Resource.Id.saveChangesEditProfile);
                Button cancel = view.FindViewById<Button>(Resource.Id.cancelEditProfile);

                updateName.Text = myAttributes.name;
                updateEmail.Text = myAttributes.email;

                confirmChanges.Click += ConfirmChanges_Click;
                cancel.Click += Cancel_Click;

                company.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(company_ItemSelected);
                var companyadapter = ArrayAdapter.CreateFromResource(this.Activity, Resource.Array.company_array, Android.Resource.Layout.SimpleSpinnerItem);
                companyadapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                company.Adapter = companyadapter;
                string defaultCompany = myAttributes.attribute1;

                if (!defaultCompany.Equals(null))
                {
                    int spinnerPosition = companyadapter.GetPosition(defaultCompany);
                    company.SetSelection(spinnerPosition);
                }

                return view;
            }

            else
            {
                throw new NotImplementedException();
            }
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();
            trans.Replace(Resource.Id.profile_root_frame, new ProfileFragment());
            trans.Commit();
        }

        private async void ConfirmChanges_Click(object sender, EventArgs e)
        {
            string dbPath_attributes = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "attributes.db3");
            var db_attributes = new SQLiteConnection(dbPath_attributes);

            MyAttributes myAttributes = db_attributes.Get<MyAttributes>(1);

            int id_login = myAttributes.loginid;

            if (myAttributes.type == "Student")
            {
                // Get Student ID
                int id_student = myAttributes.typeid;

                loginExists = false;
                existingId = "";

                progressBar.Visibility = ViewStates.Visible;
                var firebase = new FirebaseClient(FirebaseURL);
                var allLogins = await firebase.Child("users").OnceAsync<User>();

                foreach (var login in allLogins)
                {
                    if (login.Object.email == updateEmail.Text)
                    {
                        loginExists = true;
                        break;
                    }
                }

                foreach (var login in allLogins)
                {
                    if (login.Object.email == myAttributes.email)
                    {
                        existingId = login.Object.uid;
                        key = login.Key;
                        break;
                    }
                }

                if (loginExists == true && existingId != id_login.ToString())
                {
                    Toast.MakeText(this.Activity, "Email Is Already In Use", ToastLength.Short).Show();
                    progressBar.Visibility = ViewStates.Invisible;
                }
                else
                {
                    UpdateStudent();
                }
            }
            
            else if (myAttributes.type == "Recruiter")
            {
                loginExists = false;
                existingId = "";
                string myId = "";

                progressBar.Visibility = ViewStates.Visible;
                var firebase = new FirebaseClient(FirebaseURL);
                var allLogins = await firebase.Child("users").OnceAsync<User>();

                foreach (var login in allLogins)
                {
                    if (login.Object.email == updateEmail.Text)
                    {
                        loginExists = true;
                        existingId = login.Object.uid;
                        break;
                    }
                }

                foreach (var login in allLogins)
                {
                    if (login.Object.email == myAttributes.email)
                    {
                        myId = login.Object.uid;
                        key = login.Key;
                        break;
                    }
                }

                if (loginExists == true && existingId != id_login.ToString())
                {
                    progressBar.Visibility = ViewStates.Invisible;
                    Toast.MakeText(this.Activity, "Email Is Already In Use", ToastLength.Short).Show();
                }

                else if (company.SelectedItem.ToString() != myAttributes.attribute1)
                {
                    bool match = false;
                    var allCompanies = await firebase.Child("companies").OnceAsync<Company>();

                    foreach (var item in allCompanies)
                    {
                        if (item.Object.name == company.SelectedItem.ToString() && item.Object.rak == rakField.Text)
                        {
                            match = true;
                        }
                    }

                    if (match == true)
                    {
                        UpdateRecruiter(true);
                    }

                    else
                    {
                        progressBar.Visibility = ViewStates.Invisible;
                        Toast.MakeText(this.Activity, "Invalid Recruiter Access Key", ToastLength.Short).Show();
                    }
                }
                else
                {
                    UpdateRecruiter(false);
                }
            }            
        }

        private async void UpdateStudent()
        {
            try
            {
                string dbPath_attributes = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "attributes.db3");
                var db_attributes = new SQLiteConnection(dbPath_attributes);

                MyAttributes myAttributes = db_attributes.Get<MyAttributes>(1);

                // Get Login ID
                int id_login = myAttributes.loginid;

                string username = updateEmail.Text.Split('@')[0];
                string domain = updateEmail.Text.Split('@')[1];
                string provider = domain.Split('.')[0];
                string extension = domain.Split('.')[1];

                // Update Login
                User updateLogin = new User();
                updateLogin.uid = id_login.ToString();
                updateLogin.name = updateName.Text;
                updateLogin.email = updateEmail.Text;
                updateLogin.password = myAttributes.password;
                updateLogin.cfid = myAttributes.cfid.ToString();
                updateLogin.type = "Student";

                var firebase = new FirebaseClient(FirebaseURL);
                await firebase.Child("users").Child(key).PutAsync(updateLogin);

                // Update Student
                Student updateStudent = new Student();
                updateStudent.studentid = myAttributes.typeid.ToString();
                updateStudent.name = updateName.Text;
                updateStudent.email = updateEmail.Text;
                updateStudent.password = myAttributes.password;
                updateStudent.school = string.Format("{0}", school.SelectedItem);
                updateStudent.gradterm = string.Format("{0}", gradterm.SelectedItem);
                updateStudent.major = string.Format("{0}", major.SelectedItem);
                updateStudent.gpa = string.Format("{0}", gpa.SelectedItem);

                var allStudents = await firebase.Child("students").OnceAsync<Student>();
                string studentKey = "";

                foreach (var student in allStudents)
                {
                    if (student.Object.email == myAttributes.email)
                    {
                        studentKey = student.Key;
                    }
                }
                await firebase.Child("students").Child(studentKey).PutAsync(updateStudent);

                // Update MyAttributes
                MyAttributes updateMyAttributes = new MyAttributes();
                updateMyAttributes.id = 1;
                updateMyAttributes.name = updateStudent.name;
                updateMyAttributes.email = updateStudent.email;
                updateMyAttributes.password = updateStudent.password;
                updateMyAttributes.type = "Student";
                updateMyAttributes.attribute1 = updateStudent.school;
                updateMyAttributes.attribute2 = updateStudent.gradterm;
                updateMyAttributes.attribute3 = updateStudent.major;
                updateMyAttributes.attribute4 = updateStudent.gpa;
                updateMyAttributes.cfid = Convert.ToInt32(updateLogin.cfid);
                updateMyAttributes.loginid = myAttributes.loginid;
                updateMyAttributes.typeid = myAttributes.typeid;
                updateMyAttributes.rememberme = myAttributes.rememberme;

                db_attributes.InsertOrReplace(updateMyAttributes);

                progressBar.Visibility = ViewStates.Invisible;

                Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();
                trans.Replace(Resource.Id.profile_root_frame, new ProfileFragment());
                trans.Commit();
            }
            catch
            {
                progressBar.Visibility = ViewStates.Invisible;
                Toast.MakeText(this.Activity, "Invalid Email Address", ToastLength.Short).Show();
            }
        }

        private async void UpdateRecruiter(bool isCompanyChanged)
        {
            try
            {
                string dbPath_attributes = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "attributes.db3");
                var db_attributes = new SQLiteConnection(dbPath_attributes);

                MyAttributes myAttributes = db_attributes.Get<MyAttributes>(1);

                int id_login = myAttributes.loginid;
                int id_recruiter = myAttributes.typeid;

                string username = updateEmail.Text.Split('@')[0];
                string domain = updateEmail.Text.Split('@')[1];
                string provider = domain.Split('.')[0];
                string extension = domain.Split('.')[1];

                // Update Login
                User updateLogin = new User();
                updateLogin.uid = id_login.ToString();
                updateLogin.name = updateName.Text;
                updateLogin.email = updateEmail.Text;
                updateLogin.password = myAttributes.password;
                if (isCompanyChanged == true)
                {
                    updateLogin.cfid = "0";
                }
                else if (isCompanyChanged == false)
                {
                    updateLogin.cfid = myAttributes.cfid.ToString();
                }
                updateLogin.type = "Recruiter";
                var firebase = new FirebaseClient(FirebaseURL);
                await firebase.Child("users").Child(key).PutAsync(updateLogin);

                // Update Recruiter
                Recruiter updateRecruiter = new Recruiter();
                updateRecruiter.recruiterid = id_recruiter.ToString();
                updateRecruiter.name = updateName.Text;
                updateRecruiter.email = updateEmail.Text;
                updateRecruiter.password = myAttributes.password;
                updateRecruiter.company = string.Format("{0}", company.SelectedItem);

                var allRecruiters = await firebase.Child("recruiters").OnceAsync<Recruiter>();
                string recruiterKey = "";

                foreach (var recruiter in allRecruiters)
                {
                    if (recruiter.Object.email == myAttributes.email)
                    {
                        recruiterKey = recruiter.Key;
                    }
                }
                await firebase.Child("recruiters").Child(recruiterKey).PutAsync(updateRecruiter);

                // Update MyAttributes
                MyAttributes updateMyAttributes = new MyAttributes();
                updateMyAttributes.id = 1;
                updateMyAttributes.name = updateRecruiter.name;
                updateMyAttributes.email = updateRecruiter.email;
                updateMyAttributes.password = myAttributes.password;
                updateMyAttributes.type = "Recruiter";
                updateMyAttributes.attribute1 = updateRecruiter.company;
                updateMyAttributes.attribute2 = "";
                updateMyAttributes.attribute3 = "";
                updateMyAttributes.attribute4 = "";

                if (isCompanyChanged == true)
                {
                    updateMyAttributes.cfid = 0;
                }
                else if (isCompanyChanged == false)
                {
                    updateMyAttributes.cfid = myAttributes.cfid;
                }
                updateMyAttributes.loginid = id_login;
                updateMyAttributes.typeid = id_recruiter;
                updateMyAttributes.rememberme = myAttributes.rememberme;

                db_attributes.InsertOrReplace(updateMyAttributes);

                progressBar.Visibility = ViewStates.Invisible;

                Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();
                trans.Replace(Resource.Id.profile_root_frame, new ProfileFragment());
                trans.Commit();
            }
            catch
            {
                progressBar.Visibility = ViewStates.Invisible;
                Toast.MakeText(this.Activity, "Invalid Email Address", ToastLength.Short).Show();
            }
        }

        private void gradterm_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner gradterm = (Spinner)sender;
        }

        private void gpa_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner gpa = (Spinner)sender;
        }

        private void major_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner major = (Spinner)sender;
        }

        private void school_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner school = (Spinner)sender;
        }

        private void company_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner company = (Spinner)sender;
        }
    }
}