using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using SQLite;
using Android.Views.InputMethods;
using Firebase.Xamarin.Database;
using OnQAndroid.FirebaseObjects;
using Firebase.Xamarin.Database.Query;

namespace OnQAndroid.Fragments
{
    public class StudentProfileRecViewFragment : Android.Support.V4.App.Fragment
    {
        public StudentProfileRecViewFragment()
        {
            // Required empty public constructor
        }

        public static StudentProfileRecViewFragment NewInstance()
        {
            StudentProfileRecViewFragment fragment = new StudentProfileRecViewFragment();
            return fragment;
        }

        string student_id;
        View view;
        EditText notes;
        int rating = 0;
        ImageView star;
        ImageView heart;
        private const string FirebaseURL = "https://onqfirebase.firebaseio.com/";
        TextView candidateName;
        TextView candidateEmail;
        TextView candidateSchool;
        TextView candidateMajor;
        TextView candidateGT;
        TextView candidateGPA;
        ProgressBar progressBar;

        //bool doneIsVisible;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Bundle arguments = Arguments;
            student_id = arguments.GetString("StudentId");
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.StudentProfileRecView, container, false);

            progressBar = view.FindViewById<ProgressBar>(Resource.Id.circularProgress);
            candidateName = view.FindViewById<TextView>(Resource.Id.candidateName);
            candidateEmail = view.FindViewById<TextView>(Resource.Id.candidateEmail);
            candidateSchool = view.FindViewById<TextView>(Resource.Id.schoolText);
            candidateMajor = view.FindViewById<TextView>(Resource.Id.majorText);
            candidateGT = view.FindViewById<TextView>(Resource.Id.gradtermText);
            candidateGPA = view.FindViewById<TextView>(Resource.Id.gpaText);
            notes = view.FindViewById<EditText>(Resource.Id.notes);
            Button nextButton = view.FindViewById<Button>(Resource.Id.nextButton);
            star = view.FindViewById<ImageView>(Resource.Id.star);
            heart = view.FindViewById<ImageView>(Resource.Id.heart);
            ImageView backButton = view.FindViewById<ImageView>(Resource.Id.backButton);
            Button hideKeyboard = view.FindViewById<Button>(Resource.Id.hideKeyboard);
            LinearLayout rootLayout = view.FindViewById<LinearLayout>(Resource.Id.rootLayout);

            LoadCandidateInfo();

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

            nextButton.Click += NextButton_Click;
            star.Click += Star_Click;
            heart.Click += Heart_Click;

            return view;
        }

        private async void LoadCandidateInfo()
        {
            progressBar.Visibility = ViewStates.Visible;
            var firebase = new FirebaseClient(FirebaseURL);

            var allStudents = await firebase.Child("students").OnceAsync<Student>();

            foreach (var student in allStudents)
            {
                if (student.Object.studentid == student_id)
                {
                    candidateName.Text = student.Object.name;
                    candidateEmail.Text = student.Object.email;
                    candidateSchool.Text = student.Object.school;
                    candidateMajor.Text = student.Object.major;
                    candidateGT.Text = student.Object.gradterm;
                    candidateGPA.Text = student.Object.gpa;
                }
            }
            progressBar.Visibility = ViewStates.Invisible;
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

        private async void NextButton_Click(object sender, EventArgs e)
        {
            progressBar.Visibility = ViewStates.Visible;
            string dbPath_attributes = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "attributes.db3");
            SQLiteConnection db_attributes = new SQLiteConnection(dbPath_attributes);
            MyAttributes myAttributes = db_attributes.Get<MyAttributes>(1);

            var firebase = new FirebaseClient(FirebaseURL);

            // move q to student's past qs, and move student to company pastqs
            string fileName_companyPastQs = "pastqs_" + myAttributes.attribute1;
            string fileName_studentPastQs = "pastqs_" + student_id.ToString();

            StudentQ pastStudentQ = new StudentQ();
            pastStudentQ.company = myAttributes.attribute1;

            PastQ pastCompanyQ = new PastQ();
            pastCompanyQ.name = candidateName.Text;
            pastCompanyQ.studentid = student_id.ToString();
            pastCompanyQ.notes = notes.Text;
            pastCompanyQ.rating = rating.ToString();

            // check to see if student is already in past q's
            var companyPastQs = await firebase.Child(fileName_companyPastQs).OnceAsync<PastQ>();
            bool studentExists = false;

            foreach (var pastq in companyPastQs)
            {
                if (pastq.Object.studentid == student_id.ToString())
                {
                    studentExists = true;
                    break;
                }
            }

            if (studentExists == false)
            {
                await firebase.Child(fileName_companyPastQs).PostAsync(pastCompanyQ);
                await firebase.Child(fileName_studentPastQs).PostAsync(pastStudentQ);
            }

            // remove student from q, and remove q from student's current qs
            string fileName_companyQ = "qs_" + myAttributes.cfid.ToString() + "_" + myAttributes.attribute1;
            string fileName_studentQ = "myqs_" + myAttributes.cfid.ToString() + "_" + student_id;

            var companyQ = await firebase.Child(fileName_companyQ).OnceAsync<Queue>();
            var studentQ = await firebase.Child(fileName_studentQ).OnceAsync<StudentQ>();

            int numStudentsInQ = companyQ.Count;

            string key1 = "";
            foreach (var q in companyQ)
            {
                if (q.Object.position != "1")
                {
                    Queue newQ = new Queue();
                    int currentPos = Convert.ToInt32(q.Object.position);
                    int newPos = currentPos - 1;
                    newQ.position = newPos.ToString();
                    newQ.studentid = q.Object.studentid;
                    newQ.studentname = q.Object.studentname;
                    string thisKey = q.Key;

                    await firebase.Child(fileName_companyQ).Child(thisKey).PutAsync(newQ);

                    string fileName_thisStudentQ = "myqs_" + myAttributes.cfid.ToString() + "_" + q.Object.studentid;

                    var thisStudentQ = await firebase.Child(fileName_thisStudentQ).OnceAsync<StudentQ>();
                    foreach (var p in thisStudentQ)
                    {
                        if (p.Object.company == myAttributes.attribute1)
                        {
                            string companyKey = p.Key;
                            StudentQ newStudentQ = new StudentQ();
                            newStudentQ.position = newPos.ToString();
                            newStudentQ.company = myAttributes.attribute1;
                            await firebase.Child(fileName_thisStudentQ).Child(companyKey).PutAsync(newStudentQ);
                        }
                    }
                }
                else
                {
                    key1 = q.Key;
                    await firebase.Child(fileName_companyQ).Child(key1).DeleteAsync();
                }
            }

            foreach (var q in studentQ)
            {
                if (q.Object.company == myAttributes.attribute1)
                {
                    string thisKey = q.Key;
                    await firebase.Child(fileName_studentQ).Child(thisKey).DeleteAsync();
                }
            }

            progressBar.Visibility = ViewStates.Invisible;
            Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();
            trans.Replace(Resource.Id.qs_root_frame, new QsFragment());
            trans.Commit();
        }
    }
}