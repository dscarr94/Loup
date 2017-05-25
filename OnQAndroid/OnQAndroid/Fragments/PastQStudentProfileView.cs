using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using SQLite;
using Android.Views.InputMethods;

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
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Bundle arguments = Arguments;
            studentid = arguments.GetInt("StudentId");
            source = arguments.GetString("Sender");
        }
        View view;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            string dbPath_login = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "user.db3");
            SQLiteConnection db_login = new SQLiteConnection(dbPath_login);

            string dbPath_attributes = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "attributes.db3");
            SQLiteConnection db_attributes = new SQLiteConnection(dbPath_attributes);
            MyAttributes myAttributes = db_attributes.Get<MyAttributes>(1);

            string fileName_pastQs = "pastqs_" + myAttributes.attribute1 + ".db3";
            string dbPath_pastQs = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), fileName_pastQs);
            SQLiteConnection db_pastQs = new SQLiteConnection(dbPath_pastQs);

            SQLite_Tables.PastQueue thisQueue = db_pastQs.Query<SQLite_Tables.PastQueue>("SELECT * FROM PastQueue WHERE studentid = ?", studentid).First();

            StudentTable thisStudent = db_login.Get<StudentTable>(studentid);

            view = inflater.Inflate(Resource.Layout.StudentProfileRecView, container, false);

            TextView candidateName = view.FindViewById<TextView>(Resource.Id.candidateName);
            TextView candidateEmail = view.FindViewById<TextView>(Resource.Id.candidateEmail);
            TextView school = view.FindViewById<TextView>(Resource.Id.schoolText);
            TextView major = view.FindViewById<TextView>(Resource.Id.majorText);
            TextView gradterm = view.FindViewById<TextView>(Resource.Id.gradtermText);
            TextView gpa = view.FindViewById<TextView>(Resource.Id.gpaText);
            EditText notes = view.FindViewById<EditText>(Resource.Id.notes);
            ImageView star = view.FindViewById<ImageView>(Resource.Id.star);
            ImageView heart = view.FindViewById<ImageView>(Resource.Id.heart);
            Button nextButton = view.FindViewById<Button>(Resource.Id.nextButton);
            ImageView backButton = view.FindViewById<ImageView>(Resource.Id.backButton);
            Button hideKeyboard = view.FindViewById<Button>(Resource.Id.hideKeyboard);
            LinearLayout rootLayout = view.FindViewById<LinearLayout>(Resource.Id.rootLayout);

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

            candidateName.Text = thisStudent.name;
            candidateEmail.Text = thisStudent.email;
            school.Text = thisStudent.school;
            major.Text = thisStudent.major;
            gradterm.Text = thisStudent.gradterm;
            gpa.Text = thisStudent.gpa;
            notes.Text = thisQueue.notes;

            int newRating = thisQueue.rating;

            if (thisQueue.rating == 1)
            {
                star.SetImageResource(Resource.Drawable.starfilled);
            }
            else if (thisQueue.rating == 2)
            {
                heart.SetImageResource(Resource.Drawable.heartfilled);
            }

            nextButton.Visibility = ViewStates.Gone;

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
                SQLite_Tables.PastQueue newQueue = new SQLite_Tables.PastQueue();
                newQueue.id = thisQueue.id;
                newQueue.studentid = thisQueue.studentid;
                newQueue.rating = newRating;
                newQueue.notes = notes.Text;

                db_pastQs.InsertOrReplace(newQueue);

                Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();
                if (source == "Profile")
                {
                    trans.Replace(Resource.Id.profile_root_frame, new ProfileFragment());
                }
                else if (source == "PastQs")
                {
                    trans.Replace(Resource.Id.qs_root_frame, new PastQs());
                }
                //trans.Replace(Resource.Id.qs_root_frame, new PastQs());
                trans.Commit();
            };
            return view;
        }
    }
}