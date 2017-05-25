using System;
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
    public class StudentProfileRecViewFragment : Android.Support.V4.App.Fragment
    {
        public StudentProfileRecViewFragment()
        {
            // Required empty public constructor
        }

        public static StudentProfileRecViewFragment newInstance()
        {
            StudentProfileRecViewFragment fragment = new StudentProfileRecViewFragment();
            return fragment;
        }

        int student_id;
        View view;
        EditText notes;
        int rating = 0;
        ImageView star;
        ImageView heart;

        //bool doneIsVisible;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Bundle arguments = Arguments;
            student_id = arguments.GetInt("StudentId");
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.StudentProfileRecView, container, false);
            string dbPath_login = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "user.db3");
            SQLiteConnection db_login = new SQLiteConnection(dbPath_login);

            StudentTable studentAttributes = db_login.Get<StudentTable>(student_id);

            TextView candidateName = view.FindViewById<TextView>(Resource.Id.candidateName);
            TextView candidateEmail = view.FindViewById<TextView>(Resource.Id.candidateEmail);
            TextView candidateSchool = view.FindViewById<TextView>(Resource.Id.schoolText);
            TextView candidateMajor = view.FindViewById<TextView>(Resource.Id.majorText);
            TextView candidateGT = view.FindViewById<TextView>(Resource.Id.gradtermText);
            TextView candidateGPA = view.FindViewById<TextView>(Resource.Id.gpaText);
            notes = view.FindViewById<EditText>(Resource.Id.notes);
            Button nextButton = view.FindViewById<Button>(Resource.Id.nextButton);
            star = view.FindViewById<ImageView>(Resource.Id.star);
            heart = view.FindViewById<ImageView>(Resource.Id.heart);
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


            backButton.Click += (sender, e) =>
            {
                Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();
                trans.Replace(Resource.Id.qs_root_frame, new QsFragment());
                trans.Commit();
            };

            candidateName.Text = studentAttributes.name;
            candidateEmail.Text = studentAttributes.email;
            candidateSchool.Text = studentAttributes.school;
            candidateMajor.Text = studentAttributes.major;
            candidateGT.Text = studentAttributes.gradterm;
            candidateGPA.Text = studentAttributes.gpa;

            nextButton.Click += NextButton_Click;
            star.Click += Star_Click;
            heart.Click += Heart_Click;

            return view;
        }

        private void Heart_Click(object sender, EventArgs e)
        {
            if (rating == 1 || rating == 0)
            {
                rating = 2;
                heart.SetImageResource(Resource.Drawable.heartfilled);
                star.SetImageResource(Resource.Drawable.starunfilled);
            }
            else if (rating == 2)
            {
                rating = 0;
                heart.SetImageResource(Resource.Drawable.heartunfilled);
                star.SetImageResource(Resource.Drawable.starunfilled);
            }
        }

        private void Star_Click(object sender, EventArgs e)
        {
            if (rating == 2 || rating == 0)
            {
                rating = 1;
                heart.SetImageResource(Resource.Drawable.heartunfilled);
                star.SetImageResource(Resource.Drawable.starfilled);
            }
            else if (rating == 1)
            {
                rating = 0;
                heart.SetImageResource(Resource.Drawable.heartunfilled);
                star.SetImageResource(Resource.Drawable.starunfilled);
            }
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            string dbPath_attributes = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "attributes.db3");
            SQLiteConnection db_attributes = new SQLiteConnection(dbPath_attributes);
            MyAttributes myAttributes = db_attributes.Get<MyAttributes>(1);

            string dbPath_login = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "user.db3");
            SQLiteConnection db_login = new SQLiteConnection(dbPath_login);

            string myCompanyQFilename = "qs_" + myAttributes.cfid.ToString() + "_" + myAttributes.attribute1 + ".db3";
            string dbPath_myCompanyQ = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), myCompanyQFilename);
            var db_myCompanyQ = new SQLiteConnection(dbPath_myCompanyQ);

            int studentid = db_myCompanyQ.Get<SQLite_Tables.Queue>(1).studentid;
            StudentTable thisStudentAttributes = db_login.Get<StudentTable>(studentid);
            LoginTable thisStudentLogin = db_login.Query<LoginTable>("SELECT * FROM LoginTable WHERE email = ?", thisStudentAttributes.email).First();

            string fileName_studentQ = "myqs_" + thisStudentLogin.cfid + "_" + thisStudentLogin.id.ToString() + ".db3";
            string dbPath_studentQ = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), fileName_studentQ);
            var db_studentQ = new SQLiteConnection(dbPath_studentQ);

            SQLite_Tables.MyQueue thisQ = db_studentQ.Query<SQLite_Tables.MyQueue>("SELECT * FROM MyQueue WHERE company = ?", myAttributes.attribute1).First();
            int thisQid = thisQ.id;
            int numStudentQs = db_studentQ.Table<SQLite_Tables.MyQueue>().Count();

            // Check to see if this student is already in past Q's

            string fileName_pastQs = "pastqs_" + myAttributes.attribute1 + ".db3";
            string dbPath_pastQs = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), fileName_pastQs);
            var db_pastQs = new SQLiteConnection(dbPath_pastQs);

            string fileName_studentpastQs = "pastqs_" + thisStudentLogin.id.ToString() + ".db3";
            string dbPath_studentpastQs = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), fileName_studentpastQs);
            var db_studentpastQs = new SQLiteConnection(dbPath_studentpastQs);

            var queryResults = db_pastQs.Query<SQLite_Tables.PastQueue>("SELECT * FROM PastQueue WHERE studentid = ?", studentid);

            if (queryResults.Count == 0)
            {
                SQLite_Tables.PastQueue pastQ = new SQLite_Tables.PastQueue();
                pastQ.studentid = studentid;
                pastQ.notes = notes.Text;
                pastQ.rating = rating;

                db_pastQs.Insert(pastQ);

                SQLite_Tables.MyPastQueue studentpastQ = new SQLite_Tables.MyPastQueue();
                studentpastQ.company = myAttributes.attribute1;

                db_studentpastQs.Insert(studentpastQ);
            }

            db_studentQ.Delete<SQLite_Tables.MyQueue>(thisQ.id);
            for (int i = thisQid; i <= numStudentQs - 1; i++)
            {
                SQLite_Tables.MyQueue currentEntry = db_studentQ.Get<SQLite_Tables.MyQueue>(i + 1);
                SQLite_Tables.MyQueue newEntry = new SQLite_Tables.MyQueue();
                newEntry.id = i;
                newEntry.company = currentEntry.company;
                newEntry.position = currentEntry.position;

                db_studentQ.InsertOrReplace(newEntry);
            }
            db_studentQ.Delete<SQLite_Tables.MyQueue>(numStudentQs);

            int numStudents = db_myCompanyQ.Table<SQLite_Tables.Queue>().Count();

            db_myCompanyQ.Delete<SQLite_Tables.Queue>(1);
            for (int i = 1; i <= numStudents - 1; i++)
            {
                SQLite_Tables.Queue newStudent = new SQLite_Tables.Queue();
                newStudent.id = i;
                newStudent.studentid = db_myCompanyQ.Get<SQLite_Tables.Queue>(i + 1).studentid;

                db_myCompanyQ.InsertOrReplace(newStudent);
            }
            db_myCompanyQ.Delete<SQLite_Tables.Queue>(numStudents);

            Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();
            trans.Replace(Resource.Id.qs_root_frame, new QsFragment());
            trans.Commit();
        }

        /*private void Notes_Touch(object sender, View.TouchEventArgs e)
        {
            Button doneButton = view.FindViewById<Button>(Resource.Id.doneButton);
            EditText notes = view.FindViewById<EditText>(Resource.Id.notes);
            //InputMethodManager imm = (InputMethodManager)this.Activity.GetSystemService(Context.InputMethodService);
            //imm.ShowSoftInputFromInputMethod(notes.WindowToken, 0);
            doneButton.Visibility = ViewStates.Visible;
            notes.RequestFocus();
        }*/

        /*private void Notes_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            
            Button doneButton = view.FindViewById<Button>(Resource.Id.doneButton);
            if (doneIsVisible == false)
            {
                doneButton.Visibility = ViewStates.Visible;
                doneIsVisible = true;
            }
            else if (doneIsVisible == true)
            {
                doneButton.Visibility = ViewStates.Gone;
                doneIsVisible = false;
            }
        }*/

        /*private void DoneButton_Click(object sender, EventArgs e)
        {
            EditText notes = view.FindViewById<EditText>(Resource.Id.notes);
            InputMethodManager imm = (InputMethodManager)this.Activity.GetSystemService(Context.InputMethodService);
            imm.HideSoftInputFromWindow(notes.WindowToken, 0);
            Button doneButton = view.FindViewById<Button>(Resource.Id.doneButton);
            doneButton.Visibility = ViewStates.Gone;
            doneIsVisible = false;
            notes.ClearFocus();
            //LinearLayout root = view.FindViewById<LinearLayout>(Resource.Id.rootLayout);
            //root.RequestFocus();           
        }*/

        /*private void Notes_Click(object sender, EventArgs e)
        {
            Button doneButton = view.FindViewById<Button>(Resource.Id.doneButton);
            doneButton.Visibility = ViewStates.Visible;
        }*/
    }
}