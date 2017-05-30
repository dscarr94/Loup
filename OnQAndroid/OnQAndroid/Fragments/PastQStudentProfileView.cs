using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using SQLite;
using Android.Views.InputMethods;
using System;
using Firebase.Xamarin.Database;
using OnQAndroid.FirebaseObjects;
using Firebase.Xamarin.Database.Query;

namespace OnQAndroid.Fragments
{
    public class PastQStudentProfileView : Android.Support.V4.App.Fragment
    {
        public PastQStudentProfileView()
        {
            // Required empty public constructor
        }

        public static PastQStudentProfileView newInstance()
        {
            PastQStudentProfileView fragment = new PastQStudentProfileView();
            return fragment;
        }

        int studentid;
        string source;
        MyAttributes myAttributes;
        TextView candidateName;
        TextView candidateEmail;
        TextView school;
        TextView major;
        TextView gradterm;
        TextView gpa;
        EditText notes;
        ImageView star;
        ImageView heart;
        ProgressBar progressBar;
        private const string FirebaseURL = "https://onqfirebase.firebaseio.com/";
        int newRating;
        string pastQkey;
        View view;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Bundle arguments = Arguments;
            studentid = arguments.GetInt("StudentId");
            source = arguments.GetString("Sender");
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            string dbPath_attributes = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "attributes.db3");
            SQLiteConnection db_attributes = new SQLiteConnection(dbPath_attributes);
            myAttributes = db_attributes.Get<MyAttributes>(1);

            view = inflater.Inflate(Resource.Layout.StudentProfileRecView, container, false);

            candidateName = view.FindViewById<TextView>(Resource.Id.candidateName);
            candidateEmail = view.FindViewById<TextView>(Resource.Id.candidateEmail);
            school = view.FindViewById<TextView>(Resource.Id.schoolText);
            major = view.FindViewById<TextView>(Resource.Id.majorText);
            gradterm = view.FindViewById<TextView>(Resource.Id.gradtermText);
            gpa = view.FindViewById<TextView>(Resource.Id.gpaText);
            notes = view.FindViewById<EditText>(Resource.Id.notes);
            star = view.FindViewById<ImageView>(Resource.Id.star);
            heart = view.FindViewById<ImageView>(Resource.Id.heart);
            Button nextButton = view.FindViewById<Button>(Resource.Id.nextButton);
            ImageView backButton = view.FindViewById<ImageView>(Resource.Id.backButton);
            Button hideKeyboard = view.FindViewById<Button>(Resource.Id.hideKeyboard);
            LinearLayout rootLayout = view.FindViewById<LinearLayout>(Resource.Id.rootLayout);
            progressBar = view.FindViewById<ProgressBar>(Resource.Id.circularProgress);

            LoadStudentData();

            hideKeyboard.Visibility = ViewStates.Invisible;
            hideKeyboard.Click += (sender, e) =>
            {
                InputMethodManager imm = (InputMethodManager)this.Activity.GetSystemService(Context.InputMethodService);
                imm.HideSoftInputFromWindow(notes.WindowToken, 0);
                rootLayout.RequestFocus();
                hideKeyboard.Visibility = ViewStates.Invisible;
            };
            notes.FocusChange += (sender, e) =>
            {
                hideKeyboard.Visibility = ViewStates.Visible;
            };

            nextButton.Text = "Save Changes";
            nextButton.Click += NextButton_Click;

            heart.Click += (sender, e) =>
            {
                if (newRating == 2)
                {
                    heart.SetImageResource(Resource.Drawable.heartunfilled);
                    newRating = 0;
                }
                else
                {
                    heart.SetImageResource(Resource.Drawable.heartfilled);
                    star.SetImageResource(Resource.Drawable.starunfilled);
                    newRating = 2;
                }
            };
            star.Click += (sender, e) =>
            {
                if (newRating == 1)
                {
                    star.SetImageResource(Resource.Drawable.starunfilled);
                    newRating = 0;
                }
                else
                {
                    star.SetImageResource(Resource.Drawable.starfilled);
                    heart.SetImageResource(Resource.Drawable.heartunfilled);
                    newRating = 1;
                }
            };

            backButton.Click += (sender, e) =>
            {
                Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();
                if (source == "Profile")
                {
                    trans.Replace(Resource.Id.profile_root_frame, new ProfileFragment());
                }
                else if (source == "PastQs")
                {
                    trans.Replace(Resource.Id.qs_root_frame, new PastQs());
                }
                trans.Commit();
            };
            return view;
        }

        private async void NextButton_Click(object sender, EventArgs e)
        {
            progressBar.Visibility = ViewStates.Visible;
            var firebase = new FirebaseClient(FirebaseURL);

            string fileName_pastQs = "pastqs_" + myAttributes.attribute1;

            var pastQs = await firebase.Child(fileName_pastQs).OnceAsync<PastQ>();
            string time = "";

            foreach (var q in pastQs)
            {
                if (q.Object.studentid == studentid.ToString())
                {
                    time = q.Object.time;
                }
            }

            PastQ updatePastQ = new PastQ();

            updatePastQ.studentid = studentid.ToString();
            updatePastQ.name = candidateName.Text;
            updatePastQ.notes = notes.Text;
            updatePastQ.rating = newRating.ToString();
            updatePastQ.time = time;

            await firebase.Child(fileName_pastQs).Child(pastQkey).PutAsync(updatePastQ);

            Toast.MakeText(this.Activity, "Changes Saved", ToastLength.Short).Show();
            progressBar.Visibility = ViewStates.Invisible;
        }

        private async void LoadStudentData()
        {
            progressBar.Visibility = ViewStates.Visible;
            var firebase = new FirebaseClient(FirebaseURL);

            var allStudents = await firebase.Child("students").OnceAsync<Student>();

            foreach (var student in allStudents)
            {
                if (student.Object.studentid == studentid.ToString())
                {
                    candidateName.Text = student.Object.name;
                    candidateEmail.Text = student.Object.email;
                    school.Text = student.Object.school;
                    major.Text = student.Object.major;
                    gradterm.Text = student.Object.gradterm;
                    gpa.Text = student.Object.gpa;
                }
            }

            string fileName_pastQs = "pastqs_" + myAttributes.attribute1;
            var pastQs = await firebase.Child(fileName_pastQs).OnceAsync<PastQ>();

            foreach (var pastq in pastQs)
            {
                if (pastq.Object.studentid == studentid.ToString())
                {
                    if (pastq.Object.rating == "1")
                    {
                        star.SetImageResource(Resource.Drawable.starfilled);
                    }
                    else if (pastq.Object.rating == "2")
                    {
                        heart.SetImageResource(Resource.Drawable.heartfilled);
                    }
                    newRating = Convert.ToInt32(pastq.Object.rating);
                    notes.Text = pastq.Object.notes;
                    pastQkey = pastq.Key;
                }
            }
            progressBar.Visibility = ViewStates.Invisible;
        }
    }
}