using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Firebase.Xamarin.Database;
using SQLite;
using OnQAndroid.FirebaseObjects;
using System;
using Firebase.Xamarin.Database.Query;

namespace OnQAndroid.Fragments
{
    public class ListQs : Android.Support.V4.App.Fragment
    {
        public ListQs()
        {
            //required empty public constructor
        }

        public static ListQs NewInstance()
        {
            ListQs fragment = new ListQs();
            return fragment;
        }

        //int numStudents;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        ListView queue;
        TextView candidateName;
        private const string FirebaseURL = "https://onqfirebase.firebaseio.com/";
        MyAttributes myAttributes;
        ViewGroup mContainer;
        string student_id1;
        ProgressBar progressBar;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            mContainer = container;

            View view = inflater.Inflate(Resource.Layout.RecruiterQsPresent, container, false);
            progressBar = view.FindViewById<ProgressBar>(Resource.Id.circularProgress);
            queue = view.FindViewById<ListView>(Resource.Id.queuelist);
            candidateName = view.FindViewById<TextView>(Resource.Id.candidateName1);
            Button drop = view.FindViewById<Button>(Resource.Id.dropbutton);
            Button pull = view.FindViewById<Button>(Resource.Id.pullbutton);
            ImageView backButton = view.FindViewById<ImageView>(Resource.Id.backButton);
            backButton.Click += (sender, e) =>
            {
                Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();
                trans.Replace(Resource.Id.qs_root_frame, new Fragments.CurrentPastQs());
                trans.Commit();
            };

            string dbPath_attributes = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "attributes.db3");
            SQLiteConnection db_attributes = new SQLiteConnection(dbPath_attributes);
            myAttributes = db_attributes.Get<MyAttributes>(1);

            LoadQ();
            drop.Click += Drop_Click;
            pull.Click += Pull_Click;

            return view;
        }

        private async void LoadQ()
        {
            progressBar.Visibility = ViewStates.Visible;
            var firebase = new FirebaseClient(FirebaseURL);

            var thisQ = await firebase.Child("qs_" + myAttributes.cfid.ToString() + "_" + myAttributes.attribute1).OnceAsync<Queue>();

            List<string> mItems = new List<string>();
            foreach (var q in thisQ)
            {
                if (q.Object.position == "1")
                {
                    candidateName.Text = q.Object.studentname;
                    student_id1 = q.Object.studentid;
                }
                else
                {
                    mItems.Add(q.Object.studentname);
                }
            }
            QueueListViewAdapter adapter = new QueueListViewAdapter(mContainer.Context, mItems);
            queue.Adapter = adapter;
            progressBar.Visibility = ViewStates.Invisible;
        }

        private async void Drop_Click(object sender, EventArgs e)
        {
            progressBar.Visibility = ViewStates.Visible;
            string fileName_companyQ = "qs_" + myAttributes.cfid.ToString() + "_" + myAttributes.attribute1;
            string fileName_studentQ = "myqs_" + myAttributes.cfid.ToString() + "_" + student_id1;

            var firebase = new FirebaseClient(FirebaseURL);

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

            string fileName_careerFair = myAttributes.cfid.ToString();

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
                    newCompanyInfo.waittime = company.Object.waittime;
                    newCompanyInfo.numstudents = (numStudentsInQ - 1).ToString();
                }
            }

            await firebase.Child(fileName_careerFair).Child(thisCompanyKey).PutAsync(newCompanyInfo);

            progressBar.Visibility = ViewStates.Invisible;
            Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();
            trans.Replace(Resource.Id.qs_root_frame, new QsFragment());
            trans.Commit();
        }

        private void Pull_Click(object sender, EventArgs e)
        {
            Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();
            Bundle arguments = new Bundle();
            arguments.PutString("StudentId", student_id1);
            Android.Support.V4.App.Fragment fragment = new Fragments.StudentProfileRecViewFragment();
            fragment.Arguments = arguments;
            trans.Replace(Resource.Id.qs_root_frame, fragment);
            trans.Commit();
        }
    }
}