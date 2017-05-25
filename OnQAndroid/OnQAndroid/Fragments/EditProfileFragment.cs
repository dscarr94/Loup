using System;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using System.IO;
using SQLite;

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
            EditProfileFragment fragment = new OnQAndroid.EditProfileFragment();
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

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            string dbPath_attributes = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "attributes.db3");
            var db_attributes = new SQLiteConnection(dbPath_attributes);

            var myAttributes = db_attributes.Get<MyAttributes>(1);

            string type = myAttributes.type;

            if (type == "Student")
            {
                View view = inflater.Inflate(Resource.Layout.StudentEditProfileTab, container, false);
                updateName = view.FindViewById<EditText>(Resource.Id.updateName);
                updateEmail = view.FindViewById<EditText>(Resource.Id.updateEmail);

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

        private void ConfirmChanges_Click(object sender, EventArgs e)
        {
            string dbPath_user = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "user.db3");
            var db_user = new SQLiteConnection(dbPath_user);

            string dbPath_attributes = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "attributes.db3");
            var db_attributes = new SQLiteConnection(dbPath_attributes);

            MyAttributes myAttributes = db_attributes.Get<MyAttributes>(1);            

            // Get Login ID
            int id_login = db_user.Query<LoginTable>("SELECT * FROM LoginTable WHERE email = ?", myAttributes.email).First().id;
            LoginTable myLogInInfo = db_user.Get<LoginTable>(id_login);

            if (myAttributes.type == "Student")
            {
                // Get Student ID
                int id_student = db_user.Query<StudentTable>("SELECT * FROM StudentTable WHERE email = ?", myAttributes.email).First().id;

                var existingLogin = db_user.Query<LoginTable>("SELECT * FROM LoginTable WHERE email = ?", updateEmail.Text);

                if (existingLogin.Count != 0 && existingLogin.First().id != id_login)
                {
                    Toast.MakeText(this.Activity, "Email Is Already In Use", ToastLength.Short).Show();
                }
                else
                {
                    UpdateStudent();
                }
            }
            
            else if (myAttributes.type == "Recruiter")
            {                
                var existingLogin = db_user.Query<LoginTable>("SELECT * FROM LoginTable WHERE email = ?", updateEmail.Text);

                if (existingLogin.Count != 0 && existingLogin.First().id != id_login)
                {
                    Toast.MakeText(this.Activity, "Email Is Already In Use", ToastLength.Short).Show();
                }
                else if (company.SelectedItem.ToString() != myAttributes.attribute1)
                {
                    string dbPath_companies = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "allcompanies.db3");
                    var db_companies = new SQLiteConnection(dbPath_companies);
                    var queryResults = db_companies.Query<Companies>("SELECT * FROM Companies WHERE name = ? AND rak = ?", company.SelectedItem.ToString(), rakField.Text);

                    if (queryResults.Count != 0)
                    {
                        UpdateRecruiter(true);
                    }

                    else
                    {
                        Toast.MakeText(this.Activity, "Invalid Recruiter Access Key", ToastLength.Short).Show();
                    }
                }
                else
                {
                    UpdateRecruiter(false);
                }
            }            
        }

        private void UpdateStudent()
        {
            try
            {
                string dbPath_user = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "user.db3");
                var db_user = new SQLiteConnection(dbPath_user);

                string dbPath_attributes = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "attributes.db3");
                var db_attributes = new SQLiteConnection(dbPath_attributes);

                MyAttributes myAttributes = db_attributes.Get<MyAttributes>(1);

                // Get Login ID
                int id_login = db_user.Query<LoginTable>("SELECT * FROM LoginTable WHERE email = ?", myAttributes.email).First().id;
                LoginTable myLogInInfo = db_user.Get<LoginTable>(id_login);
                int id_student = db_user.Query<StudentTable>("SELECT * FROM StudentTable WHERE email = ?", myAttributes.email).First().id;

                string username = updateEmail.Text.Split('@')[0];
                string domain = updateEmail.Text.Split('@')[1];
                string provider = domain.Split('.')[0];
                string extension = domain.Split('.')[1];

                // Update LoginTable
                LoginTable updateLogin = new LoginTable();
                updateLogin.id = id_login;
                updateLogin.name = updateName.Text;
                updateLogin.email = updateEmail.Text;
                updateLogin.password = myLogInInfo.password;
                updateLogin.cfid = myLogInInfo.cfid;
                updateLogin.type = "Student";

                db_user.InsertOrReplace(updateLogin);

                // Update StudentTable
                StudentTable updateStudent = new StudentTable();
                updateStudent.id = id_student;
                updateStudent.name = updateName.Text;
                updateStudent.email = updateEmail.Text;
                updateStudent.password = myLogInInfo.password;
                updateStudent.school = string.Format("{0}", school.SelectedItem);
                updateStudent.gradterm = string.Format("{0}", gradterm.SelectedItem);
                updateStudent.major = string.Format("{0}", major.SelectedItem);
                updateStudent.gpa = string.Format("{0}", gpa.SelectedItem);

                db_user.InsertOrReplace(updateStudent);

                // Update MyAttributes
                MyAttributes updateMyAttributes = new MyAttributes();
                updateMyAttributes.id = 1;
                updateMyAttributes.name = updateStudent.name;
                updateMyAttributes.email = updateStudent.email;
                updateMyAttributes.type = "Student";
                updateMyAttributes.attribute1 = updateStudent.school;
                updateMyAttributes.attribute2 = updateStudent.gradterm;
                updateMyAttributes.attribute3 = updateStudent.major;
                updateMyAttributes.attribute4 = updateStudent.gpa;
                updateMyAttributes.cfid = updateLogin.cfid;
                updateMyAttributes.loginid = myLogInInfo.id;
                updateMyAttributes.typeid = id_student;
                updateMyAttributes.rememberme = myAttributes.rememberme;

                db_attributes.InsertOrReplace(updateMyAttributes);

                Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();
                trans.Replace(Resource.Id.profile_root_frame, new ProfileFragment());
                trans.Commit();

                /*Android.Support.V4.App.FragmentTransaction trans2 = FragmentManager.BeginTransaction();
                trans2.Replace(Resource.Id.qs_root_frame, new QsFragment());
                trans2.Commit();*/
            }
            catch
            {
                Toast.MakeText(this.Activity, "Invalid Email Address", ToastLength.Short).Show();
            }
        }

        private void UpdateRecruiter(bool isCompanyChanged)
        {
            try
            {
                string dbPath_user = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "user.db3");
                var db_user = new SQLiteConnection(dbPath_user);

                string dbPath_attributes = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "attributes.db3");
                var db_attributes = new SQLiteConnection(dbPath_attributes);

                MyAttributes myAttributes = db_attributes.Get<MyAttributes>(1);

                // Get Login ID
                int id_login = db_user.Query<LoginTable>("SELECT * FROM LoginTable WHERE email = ?", myAttributes.email).First().id;
                int id_recruiter = db_user.Query<RecruiterTable>("SELECT * FROM RecruiterTable WHERE email = ?", myAttributes.email).First().id;

                LoginTable myLogInInfo = db_user.Get<LoginTable>(id_login);
                string username = updateEmail.Text.Split('@')[0];
                string domain = updateEmail.Text.Split('@')[1];
                string provider = domain.Split('.')[0];
                string extension = domain.Split('.')[1];

                // Update LoginTable
                LoginTable updateLogin = new LoginTable();
                updateLogin.id = id_login;
                updateLogin.name = updateName.Text;
                updateLogin.email = updateEmail.Text;
                updateLogin.password = myLogInInfo.password;
                if (isCompanyChanged == true)
                {
                    updateLogin.cfid = 0;
                }
                else if (isCompanyChanged == false)
                {
                    updateLogin.cfid = myAttributes.cfid;
                }
                updateLogin.type = "Recruiter";

                db_user.InsertOrReplace(updateLogin);

                // Update RecruiterTable
                RecruiterTable updateRecruiter = new RecruiterTable();
                updateRecruiter.id = id_recruiter;
                updateRecruiter.name = updateName.Text;
                updateRecruiter.email = updateEmail.Text;
                updateRecruiter.password = myLogInInfo.password;
                updateRecruiter.company = string.Format("{0}", company.SelectedItem);

                db_user.InsertOrReplace(updateRecruiter);

                // Update MyAttributes
                MyAttributes updateMyAttributes = new MyAttributes();
                updateMyAttributes.id = 1;
                updateMyAttributes.name = updateRecruiter.name;
                updateMyAttributes.email = updateRecruiter.email;
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

                Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();
                trans.Replace(Resource.Id.profile_root_frame, new ProfileFragment());
                trans.Commit();
            }
            catch
            {
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