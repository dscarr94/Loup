using System;
using System.IO;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using SQLite;
using Android.Views;
using Firebase.Xamarin.Database;
using OnQAndroid.FirebaseObjects;

namespace OnQAndroid
{
    [Activity(Label = "RecruiterScreen")]
    public class recruiterBuildProfile : Activity
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
            SetContentView(Resource.Layout.RecruiterBuildProfile);
            ActionBar.Hide();

            // Select profile picture
            imageButton = FindViewById<ImageButton>(Resource.Id.cameraButton);
            addPhoto = FindViewById<Button>(Resource.Id.photoButton2);
            addPhoto.Click += AddPhotoClick;
            imageButton.Click += AddPhotoClick;


            // Load name from before, and set it as default in the name slot            
            // attributes database
            string dbPath_attributes = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "attributes.db3");
            var db_attributes = new SQLiteConnection(dbPath_attributes);
            var myAttributes = db_attributes.Get<MyAttributes>(1);
            string myName = myAttributes.name;
            string myEmail = myAttributes.email;

            EditText editName = FindViewById<EditText>(Resource.Id.nameField);
            editName.Text = myName;
            EditText editEmail = FindViewById<EditText>(Resource.Id.emailField);
            editEmail.Text = myEmail;

            // Company spinner
            Spinner companySpinner = FindViewById<Spinner>(Resource.Id.companyspinner);

            // make a company spinner adapter and assign it to the company spinner
            companySpinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(companySpinner_ItemSelected);
            var companyadapter = ArrayAdapter.CreateFromResource(this, Resource.Array.company_array, Android.Resource.Layout.SimpleSpinnerItem);
            companyadapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            companySpinner.Adapter = companyadapter;

            // Next Button
            Button nextButton = FindViewById<Button>(Resource.Id.NextButton);
            nextButton.Click += NextButton_Click;
        }

        private async void NextButton_Click(object sender, EventArgs e)
        {
            // Call UI Controls
            ProgressBar progressBar = FindViewById<ProgressBar>(Resource.Id.circularProgress);
            EditText editName = FindViewById<EditText>(Resource.Id.nameField);
            EditText editEmail = FindViewById<EditText>(Resource.Id.emailField);
            Spinner companySpinner = FindViewById<Spinner>(Resource.Id.companyspinner);
            EditText rakField = FindViewById<EditText>(Resource.Id.rak); // rak = recruiter access key

            progressBar.Visibility = ViewStates.Visible;
            string company = string.Format("{0}", companySpinner.SelectedItem); // get value of company spinner

            var firebase = new FirebaseClient(FirebaseURL);
            var companyItems = await firebase.Child("companies").OnceAsync<Company>();
            bool fieldsMatch = false;

            foreach (var item in companyItems)
            {
                string thiscompany = item.Object.name;
                string rak = item.Object.rak;
                if (company == thiscompany && rakField.Text == rak)
                {
                    fieldsMatch = true;
                    break;
                }
            }

            if (fieldsMatch == true) // if query returned results
            {
                // Get user data (attributes)
                string dbPath_attributes = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "attributes.db3");
                var db_attributes = new SQLiteConnection(dbPath_attributes);
                MyAttributes userAttributes = db_attributes.Get<MyAttributes>(1);

                MyAttributes updateAttributes = new MyAttributes();

                updateAttributes.id = 1;
                string name = editName.Text;
                updateAttributes.name = name;
                string email = editEmail.Text;
                updateAttributes.email = email;
                string password = userAttributes.password;
                updateAttributes.password = password;
                updateAttributes.attribute1 = company;
                updateAttributes.type = userAttributes.type;

                // Put Recruiter info into attributes database
                db_attributes.InsertOrReplace(updateAttributes);

                progressBar.Visibility = ViewStates.Invisible;
                // Next
                var advanceIntent = new Intent(this, typeof(successScreen));
                StartActivity(advanceIntent); // go to successScreen
                Finish(); // finish the activity
            }
            else // if query did not return results
            {
                progressBar.Visibility = ViewStates.Invisible;
                Toast.MakeText(this, "Invalid Recruiter Access Key", ToastLength.Short).Show();
            }
            
        }

        private void companySpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e) // not really necessary
        {
            Spinner companySpinner = (Spinner)sender;
        }

        private void AddPhotoClick(object sender, EventArgs eventArgs)
        {
            Intent = new Intent(); // create intent to choose a picture from file
            Intent.SetType("image/*");
            Intent.SetAction(Intent.ActionGetContent);
            StartActivityForResult(Intent.CreateChooser(Intent, "Select Picture"), PickImageId);

        }
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data) // when picture has been chosen
        {
            if ((requestCode == PickImageId) && (resultCode == Result.Ok) && (data != null))
            {
                Android.Net.Uri uri = data.Data;
                imageButton.SetImageURI(uri); // set image uri to selected image uri
                addPhoto = FindViewById<Button>(Resource.Id.photoButton2); 
                addPhoto.Text = "Edit Photo"; // set text to "Edit Photo" instead of "Add Photo"
            }
        }

    }
}
