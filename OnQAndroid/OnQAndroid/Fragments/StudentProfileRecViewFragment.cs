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
using System.Diagnostics;
using System.Collections.Generic;

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
        Stopwatch stopwatch;

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

            stopwatch = new Stopwatch();
            stopwatch.Start();

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
            stopwatch.Stop();

            string dbPath_attributes = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "attributes.db3");
            SQLiteConnection db_attributes = new SQLiteConnection(dbPath_attributes);
            MyAttributes myAttributes = db_attributes.Get<MyAttributes>(1);

            var firebase = new FirebaseClient(FirebaseURL);

            // move q to student's past qs, and move student to company pastqs
            string fileName_companyPastQs = "pastqs_" + myAttributes.attribute1;
            string fileName_studentPastQs = "pastqs_" + student_id.ToString();
            string fileName_careerFair = myAttributes.cfid.ToString();

            StudentQ pastStudentQ = new StudentQ();
            pastStudentQ.company = myAttributes.attribute1;

            PastQ pastCompanyQ = new PastQ();
            pastCompanyQ.name = candidateName.Text;
            pastCompanyQ.studentid = student_id.ToString();
            pastCompanyQ.notes = notes.Text;
            pastCompanyQ.rating = rating.ToString();
            pastCompanyQ.time = stopwatch.ElapsedTicks.ToString();

            // check to see if student is already in past q's
            var companyPastQs = await firebase.Child(fileName_companyPastQs).OnceAsync<PastQ>();
            bool studentExists = false;

            List<string> times = new List<string>();

            foreach (var pastq in companyPastQs)
            {
                if (pastq.Object.studentid == student_id.ToString())
                {
                    studentExists = true;
                    break;
                }
                times.Add(pastq.Object.time);
            }

            times.Add(stopwatch.ElapsedTicks.ToString());

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

            // Calculate average time, and update value in database
            long fiveMinsInTicks = 3000000000;
            long numTimes = times.Count;
            long fiveMinsWeight = 10;
            long otherWeight = 100 - fiveMinsWeight;
            long indWeight = otherWeight / numTimes;

            long sum = (fiveMinsInTicks * fiveMinsWeight)/100;
            foreach (var time in times)
            {
                long timeContribution = (indWeight * Convert.ToInt64(time))/100;
                sum = sum + timeContribution;
            }

            var thisCareerFair = await firebase.Child(fileName_careerFair).OnceAsync<Company>();
            Company newCompanyInfo = new Company();
            string thisCompanyKey = "";

            foreach (var company in thisCareerFair)
            {
                if (company.Object.name == myAttributes.attribute1)
                {
                    thisCompanyKey = company.Key;
                    newCompanyInfo.companyid = company.Object.companyid;
                    newCompanyInfo.name = company.Object.name;
                    newCompanyInfo.description = company.Object.description;
                    newCompanyInfo.website = company.Object.website;
                    newCompanyInfo.rak = company.Object.rak;
                    newCompanyInfo.checkedIn = company.Object.checkedIn;
                    newCompanyInfo.waittime = sum.ToString();
                    newCompanyInfo.numstudents = (numStudentsInQ - 1).ToString();
                }
            }

            await firebase.Child(fileName_careerFair).Child(thisCompanyKey).PutAsync(newCompanyInfo);

            progressBar.Visibility = ViewStates.Invisible;
            Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();
            trans.Replace(Resource.Id.qs_root_frame, new QsFragment());
            trans.Commit();
        }
    }
}