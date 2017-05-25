using System;
using System.IO;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Firebase.Xamarin.Database;
using OnQAndroid.FirebaseObjects;
using SQLite;
// convert this to firebase!!
namespace OnQAndroid
{
    [Activity(Label = "OnQ")]
    public class studentBuildProfile : Activity
    {
        public static readonly int PickImageId = 1000;
        private ImageButton imageButton;
        private Button addPhoto;
        private const string FirebaseURL = "https://onqfirebase.firebaseio.com/";
        //LoginTable newRecord = new LoginTable();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.StudentBuildProfile);
            ActionBar.Hide();

            // Select profile picture
            imageButton = FindViewById<ImageButton>(Resource.Id.cameraButton);
            addPhoto = FindViewById<Button>(Resource.Id.photoButton2);
            addPhoto.Click += AddPhotoClick;
            imageButton.Click += AddPhotoClick;
            

            // Load name from before, and set it as default in the name slot
            string dbPath_attributes = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "attributes.db3");
            var db_attributes = new SQLiteConnection(dbPath_attributes);
            var myAttributes = db_attributes.Get<MyAttributes>(1);
            string myName = myAttributes.name;
            string myEmail = myAttributes.email;

            //Toast.MakeText(this, id.ToString(), ToastLength.Short).Show();
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

        private async void NextButton_Click(object sender, EventArgs e)
        {
            // Call UI Controls
            EditText editName = FindViewById<EditText>(Resource.Id.nameField);
            EditText editEmail = FindViewById<EditText>(Resource.Id.emailField);
            Spinner yearSpinner = FindViewById<Spinner>(Resource.Id.yearspinner);
            Spinner majorSpinner = FindViewById<Spinner>(Resource.Id.majorspinner);
            Spinner gpaSpinner = FindViewById<Spinner>(Resource.Id.gpaspinner);
            ProgressBar progressBar = FindViewById<ProgressBar>(Resource.Id.circularProgress);

            progressBar.Visibility = ViewStates.Visible;

            // Get user data (attributes)
            string dbPath_attributes = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "attributes.db3");
            var db_attributes = new SQLiteConnection(dbPath_attributes);
            var userAttributes = db_attributes.Get<MyAttributes>(1);

            // Make a new table for students and populate it
            MyAttributes myStudentAttributes = db_attributes.Get<MyAttributes>(1);
            MyAttributes tbl = new MyAttributes();

            int numExistingLogins = 0;
            try
            {
                var firebase = new FirebaseClient(FirebaseURL);
                var allLogins = await firebase.Child("users").OnceAsync<User>();
                foreach (var login in allLogins)
                {
                    string email = login.Object.email;
                    if (email == editEmail.Text)
                    {
                        numExistingLogins = numExistingLogins + 1;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.ToString(), ToastLength.Long).Show();
                progressBar.Visibility = ViewStates.Invisible;
            }

            if (numExistingLogins != 0)
            {
                Toast.MakeText(this, "Email Is Already In Use", ToastLength.Short).Show();
                progressBar.Visibility = ViewStates.Invisible;
            }
            else
            {
                try // check to see if email is correctly formatted
                {
                    string user = editEmail.Text.Split('@')[0];
                    string domain = editEmail.Text.Split('@')[1];
                    string provider = domain.Split('.')[0];
                    string extension = domain.Split('.')[1];

                    tbl.id = 1;
                    string name = editName.Text;
                    tbl.name = name;
                    string email = editEmail.Text;
                    tbl.email = email;
                    string password = userAttributes.password; // May not be necessary to store again, but good to keep all the info together
                    tbl.password = password;
                    tbl.type = userAttributes.type;
                    string school = myStudentAttributes.attribute1;
                    tbl.attribute1 = school;
                    string grad_term = string.Format("{0}", yearSpinner.SelectedItem);
                    tbl.attribute2 = grad_term;
                    string major = string.Format("{0}", majorSpinner.SelectedItem);
                    tbl.attribute3 = major;
                    string GPA = string.Format("{0}", gpaSpinner.SelectedItem);
                    tbl.attribute4 = GPA;

                    // Put Student info into MyAttributes table in attributes database
                    db_attributes.InsertOrReplace(tbl);

                    // Next
                    progressBar.Visibility = ViewStates.Invisible;
                    var advanceIntent = new Intent(this, typeof(uploadResume));
                    StartActivity(advanceIntent); // go to upload resume
                    Finish(); // finish activity
                }
                catch // if email address is improperly formatted
                {
                    Toast.MakeText(this, "Invalid Email Address", ToastLength.Short).Show();
                    progressBar.Visibility = ViewStates.Invisible;
                }
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
            Intent = new Intent();
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