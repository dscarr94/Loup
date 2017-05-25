using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using SQLite;

namespace OnQAndroid
{
    public class QsFragment : Android.Support.V4.App.Fragment
    {
        public QsFragment()
        {
            // Required empty public constructor
        }

        public static QsFragment newInstance()
        {
            QsFragment fragment = new OnQAndroid.QsFragment();
            return fragment;
        }

        int student_id1;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            string dbPath_attributes = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "attributes.db3");
            SQLiteConnection db_attributes = new SQLiteConnection(dbPath_attributes);
            MyAttributes myAttributes = db_attributes.Get<MyAttributes>(1);

            if (myAttributes.cfid == 0)
            {
                View view = inflater.Inflate(Resource.Layout.MyQsTab, container, false);
                ImageView backButton = view.FindViewById<ImageView>(Resource.Id.backButton);
                backButton.Click += (sender, e) =>
                {
                    Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();
                    trans.Replace(Resource.Id.qs_root_frame, new Fragments.CurrentPastQs());
                    trans.Commit();
                };
                return view;
            }
            else
            {
                if (myAttributes.type == "Recruiter")
                {
                    string myCompanyQFilename = "qs_" + myAttributes.cfid.ToString() + "_" + myAttributes.attribute1 + ".db3";
                    string dbPath_myCompanyQ = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), myCompanyQFilename);
                    var db_myCompanyQ = new SQLiteConnection(dbPath_myCompanyQ);

                    string dbPath_user = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "user.db3");
                    SQLiteConnection db_user = new SQLiteConnection(dbPath_user);

                    int numStudents = db_myCompanyQ.Table<SQLite_Tables.Queue>().Count();
                    //Toast.MakeText(this.Activity, numStudents.ToString(), ToastLength.Short).Show();
                    View view;
                    //view = inflater.Inflate(Resource.Layout.MyQsTab, container, false);
                    //return view;

                    //}
                    if (numStudents == 0)
                    {
                        view = inflater.Inflate(Resource.Layout.MyQsTab, container, false);
                        ImageView backButton = view.FindViewById<ImageView>(Resource.Id.backButton);
                        backButton.Click += (sender, e) =>
                        {
                            Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();
                            trans.Replace(Resource.Id.qs_root_frame, new Fragments.CurrentPastQs());
                            trans.Commit();
                        };
                        return view;
                    }
                
                    else if (numStudents == 1)
                    {
                        view = inflater.Inflate(Resource.Layout.RecruiterQsPresent, container, false);
                        TextView candidateName = view.FindViewById<TextView>(Resource.Id.candidateName1);
                        Button drop = view.FindViewById<Button>(Resource.Id.dropbutton);
                        Button pull = view.FindViewById<Button>(Resource.Id.pullbutton);
                        ImageView backButton = view.FindViewById<ImageView>(Resource.Id.backButton);
                        backButton.Click += (sender, e) =>
                        {
                            Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();
                            trans.Replace(Resource.Id.qs_root_frame, new Fragments.CurrentPastQs());
                            trans.Commit();
                        };

                        drop.Click += Drop_Click;
                        pull.Click += Pull_Click;

                        student_id1 = db_myCompanyQ.Get<SQLite_Tables.Queue>(1).studentid;
                        candidateName.Text = db_user.Get<StudentTable>(student_id1).name;
                    }

                    else if (numStudents >= 2)
                    {
                        view = inflater.Inflate(Resource.Layout.RecruiterQsPresent, container, false);
                        ListView queue = view.FindViewById<ListView>(Resource.Id.queuelist);
                        //queue.Enabled = false;
                        TextView candidateName = view.FindViewById<TextView>(Resource.Id.candidateName1);
                        Button drop = view.FindViewById<Button>(Resource.Id.dropbutton);
                        Button pull = view.FindViewById<Button>(Resource.Id.pullbutton);
                        ImageView backButton = view.FindViewById<ImageView>(Resource.Id.backButton);
                        backButton.Click += (sender, e) =>
                        {
                            Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();
                            trans.Replace(Resource.Id.qs_root_frame, new Fragments.CurrentPastQs());
                            trans.Commit();
                        };

                        drop.Click += Drop_Click;
                        pull.Click += Pull_Click;

                        student_id1 = db_myCompanyQ.Get<SQLite_Tables.Queue>(1).studentid;
                        candidateName.Text = db_user.Get<StudentTable>(student_id1).name;

                        List<string> mItems = new List<string>();
                        for (int i = 2; i <= numStudents; i++)
                        {
                            int student_id = db_myCompanyQ.Get<SQLite_Tables.Queue>(i).studentid;
                            StudentTable studentInfo = db_user.Get<StudentTable>(student_id);
                            string newItem = studentInfo.name;
                            //Toast.MakeText(this.Activity, newItem, ToastLength.Short).Show();
                            mItems.Add(newItem);
                        }

                        QueueListViewAdapter adapter = new QueueListViewAdapter(container.Context, mItems);
                        queue.Adapter = adapter;
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                    return view;
                }
                else if (myAttributes.type == "Student")
                {
                    string fileName_myQs = "myqs_" + myAttributes.cfid.ToString() + "_" + myAttributes.loginid + ".db3";
                    string dbPath_myQs = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), fileName_myQs);
                    SQLiteConnection db_myQs = new SQLiteConnection(dbPath_myQs);
                    db_myQs.CreateTable<SQLite_Tables.MyQueue>();

                    string fileName_myPastQs = "pastqs_" + myAttributes.loginid.ToString() + ".db3";
                    string dbPath_myPastQs = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), fileName_myPastQs);
                    SQLiteConnection db_myPastQs = new SQLiteConnection(dbPath_myPastQs);
                    db_myPastQs.CreateTable<SQLite_Tables.MyPastQueue>();

                    int numMyQs = db_myQs.Table<SQLite_Tables.MyQueue>().Count();
                    View view;
                    if (numMyQs == 0)
                    {
                        view = inflater.Inflate(Resource.Layout.MyQsTab, container, false);
                        ImageView backButton = view.FindViewById<ImageView>(Resource.Id.backButton);
                        backButton.Click += (sender, e) =>
                        {
                            Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();
                            trans.Replace(Resource.Id.qs_root_frame, new Fragments.CurrentPastQs());
                            trans.Commit();
                        };
                        return view;
                    }
                    else
                    {
                        view = inflater.Inflate(Resource.Layout.StudentQsPresent, container, false);
                        List<string> mItems = new List<string>();
                        ListView mListView = view.FindViewById<ListView>(Resource.Id.myQsListView);
                        ImageView backButton = view.FindViewById<ImageView>(Resource.Id.backButton);
                        backButton.Click += (sender, e) =>
                        {
                            Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();
                            trans.Replace(Resource.Id.qs_root_frame, new Fragments.CurrentPastQs());
                            trans.Commit();
                        };

                        for (int i = 1; i <= numMyQs; i++)
                        {
                            SQLite_Tables.MyQueue newRow = db_myQs.Get<SQLite_Tables.MyQueue>(i);
                            string newItem = newRow.company;
                            mItems.Add(newItem);
                        }

                        QsListViewAdapter adapter = new QsListViewAdapter(container.Context, mItems, "CurrentQs");
                        mListView.Adapter = adapter;

                        return view;
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
        }

        private void Pull_Click(object sender, EventArgs e)
        {
            Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();
            Bundle arguments = new Bundle();
            arguments.PutInt("StudentId", student_id1);
            Android.Support.V4.App.Fragment fragment = new Fragments.StudentProfileRecViewFragment();
            fragment.Arguments = arguments;
            trans.Replace(Resource.Id.qs_root_frame, fragment);
            trans.Commit();
        }

        private void Drop_Click(object sender, EventArgs e)
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

            string fileName_studentQ = "myqs_" + thisStudentLogin.cfid + "_" + thisStudentLogin.id + ".db3";
            string dbPath_studentQ = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), fileName_studentQ);
            var db_studentQ = new SQLiteConnection(dbPath_studentQ);

            SQLite_Tables.MyQueue thisQ = db_studentQ.Query<SQLite_Tables.MyQueue>("SELECT * FROM MyQueue WHERE company = ?", myAttributes.attribute1).First();
            int thisQid = thisQ.id;
            int numStudentQs = db_studentQ.Table<SQLite_Tables.MyQueue>().Count();

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
            for (int i = 1; i <= numStudents - 1; i ++)
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
    }
}