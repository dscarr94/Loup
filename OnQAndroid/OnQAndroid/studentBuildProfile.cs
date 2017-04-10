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
    [Activity(Label = "OnQ")]
    public class studentBuildProfile : Activity
    {
        public static readonly int PickImageId = 1000;
        private ImageButton imageButton;
        private Button addPhoto;
        LoginTable newRecord = new LoginTable();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.StudentBuildProfile);

            // Select profile picture
            imageButton = FindViewById<ImageButton>(Resource.Id.cameraButton);
            addPhoto = FindViewById<Button>(Resource.Id.photoButton2);
            addPhoto.Click += AddPhotoClick;
            imageButton.Click += AddPhotoClick;
            

            // Load name from before, and set it as default in the name slot
            int id = Intent.GetIntExtra("MyId", -1);
            string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "user.db3");
            var db = new SQLiteConnection(dbPath);
            var myAttributes = db.Get<LoginTable>(id);
            string myName = myAttributes.name;
            string myEmail = myAttributes.email;


            EditText editName = FindViewById<EditText>(Resource.Id.nameField);
            editName.Text = myName;
            EditText editEmail = FindViewById<EditText>(Resource.Id.emailField);
            editEmail.Text = myEmail;

            // Year spinner
            Spinner yearSpinner = FindViewById<Spinner> (Resource.Id.yearspinner);

            yearSpinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(yearSpinner_ItemSelected);
            var yearadapter = ArrayAdapter.CreateFromResource(this, Resource.Array.year_array, Android.Resource.Layout.SimpleSpinnerItem);
            yearadapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            yearSpinner.Adapter = yearadapter;

            // Major Spinner
            Spinner majorSpinner = FindViewById<Spinner>(Resource.Id.majorspinner);

            majorSpinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(majorSpinner_ItemSelected);
            var majoradapter = ArrayAdapter.CreateFromResource(this, Resource.Array.major_array, Android.Resource.Layout.SimpleSpinnerItem);
            majoradapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            majorSpinner.Adapter = majoradapter;

            // GPA Spinner
            Spinner gpaSpinner = FindViewById<Spinner>(Resource.Id.gpaspinner);

            gpaSpinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(gpaSpinner_ItemSelected);
            var gpaadapter = ArrayAdapter.CreateFromResource(this, Resource.Array.gpa_array, Android.Resource.Layout.SimpleSpinnerItem);
            gpaadapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            gpaSpinner.Adapter = gpaadapter;

            // Next Button
            Button nextButton = FindViewById<Button>(Resource.Id.NextButton);
            nextButton.Click += NextButton_Click;
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            // Call UI Controls
            EditText editName = FindViewById<EditText>(Resource.Id.nameField);
            EditText editEmail = FindViewById<EditText>(Resource.Id.emailField);
            Spinner yearSpinner = FindViewById<Spinner>(Resource.Id.yearspinner);
            Spinner majorSpinner = FindViewById<Spinner>(Resource.Id.majorspinner);
            Spinner gpaSpinner = FindViewById<Spinner>(Resource.Id.gpaspinner);

            // Get user data (attributes)
            int id = Intent.GetIntExtra("MyId", -1);
            string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "user.db3");
            var db = new SQLiteConnection(dbPath);
            var userAttributes = db.Get<LoginTable>(id);

            if (userAttributes.name == "Admin")
            {
                var advanceIntent = new Intent(this, typeof(uploadResume));
                StartActivity(advanceIntent);
            }
            else
            {
                // Make a new table for students and populate it
                StudentTable tbl = new StudentTable();

                string name = editName.Text;
                tbl.name = name;
                string email = editEmail.Text;
                tbl.email = email;
                string password = userAttributes.password; // May not be necessary to store again, but good to keep all the info together
                tbl.password = password;
                string grad_term = string.Format("{0}", yearSpinner.SelectedItem);
                tbl.gradterm = grad_term;
                string major = string.Format("{0}", majorSpinner.SelectedItem);
                tbl.major = major;
                string GPA = string.Format("{0}", gpaSpinner.SelectedItem);
                tbl.gpa = GPA;

                // Put Student info into StudentTable
                db.CreateTable<StudentTable>();
                db.Insert(tbl);

                // Update LoginTable
                var updateStudent = new LoginTable();
                updateStudent.id = id;
                updateStudent.name = name;
                updateStudent.email = email;
                updateStudent.password = password;
                updateStudent.type = "Student";
                db.Update(updateStudent);

                // Next
                var advanceIntent = new Intent(this, typeof(uploadResume));
                StartActivity(advanceIntent);
            }
        }

        private void yearSpinner_ItemSelected (object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner yearSpinner = (Spinner)sender;
        }
        private void majorSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner majorSpinner = (Spinner)sender;
        }
        private void gpaSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner gpaSpinner = (Spinner)sender;
        }
        private void AddPhotoClick(object sender, EventArgs eventArgs)
        {
            int id = Intent.GetIntExtra("MyId", -1);
            Intent = new Intent().PutExtra("MyId", id);
            Intent.SetType("image/*");
            Intent.SetAction(Intent.ActionGetContent);
            StartActivityForResult(Intent.CreateChooser(Intent, "Select Picture"), PickImageId);

        }
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if ((requestCode == PickImageId) && (resultCode == Result.Ok) && (data != null))
            {
                Android.Net.Uri uri = data.Data;
                imageButton.SetImageURI(uri);
                addPhoto = FindViewById<Button>(Resource.Id.photoButton2);
                addPhoto.Text = "Edit Photo";
            }
        }
    }
}