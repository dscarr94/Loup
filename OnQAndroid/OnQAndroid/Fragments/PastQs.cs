using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using SQLite;

namespace OnQAndroid.Fragments
{
    public class PastQs : Android.Support.V4.App.Fragment
    {
        public PastQs()
        {
            // Required empty public constructor
        }

        public static PastQs newInstance()
        {
            PastQs fragment = new PastQs();
            return fragment;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        View view;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            string dbPath_attributes = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "attributes.db3");
            SQLiteConnection db_attributes = new SQLiteConnection(dbPath_attributes);
            MyAttributes myAttributes = db_attributes.Get<MyAttributes>(1);

            string dbPath_login = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "user.db3");
            SQLiteConnection db_login = new SQLiteConnection(dbPath_login);

            if (myAttributes.cfid == 0)
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
                if (myAttributes.type == "Recruiter")
                {
                    string myCompanyPastQsFilename = "pastqs_" + myAttributes.attribute1 + ".db3";
                    string dbPath_pastqs = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), myCompanyPastQsFilename);
                    var db_pastqs = new SQLiteConnection(dbPath_pastqs);
                    List<int> studentIDs = new List<int>();
                    int numpastqs = db_pastqs.Table<SQLite_Tables.PastQueue>().Count();

                    //Toast.MakeText(this.Activity, numpastqs.ToString(), ToastLength.Short).Show();
                    //view = inflater.Inflate(Resource.Layout.MyQsTab, container, false);
                    //return view;

                    if (numpastqs == 0)
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
                        view = inflater.Inflate(Resource.Layout.PastQs, container, false);
                        ImageView backButton = view.FindViewById<ImageView>(Resource.Id.backButton);
                        backButton.Click += (sender, e) =>
                        {
                            Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();
                            trans.Replace(Resource.Id.qs_root_frame, new Fragments.CurrentPastQs());
                            trans.Commit();
                        };

                        ListView lv_pastqs = view.FindViewById<ListView>(Resource.Id.pastqslv);
                        for (int i = 1; i <= numpastqs; i++)
                        {
                            int newStudentID = db_pastqs.Get<SQLite_Tables.PastQueue>(i).studentid;
                            studentIDs.Add(newStudentID);
                        }

                        PastQueuesListViewAdapter adapter = new PastQueuesListViewAdapter(container.Context, studentIDs, "PastQs");
                        lv_pastqs.Adapter = adapter;

                        return view;
                    }
                }
                else if (myAttributes.type == "Student")
                {
                    string myPastQsFileName = "pastqs_" + myAttributes.loginid.ToString() + ".db3";
                    string dbPath_myPastQs = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), myPastQsFileName);
                    SQLiteConnection db_myPastQs = new SQLiteConnection(dbPath_myPastQs);
                    db_myPastQs.CreateTable<SQLite_Tables.MyPastQueue>();

                    List<string> companies = new List<string>();
                    int numpastqs = db_myPastQs.Table<SQLite_Tables.MyPastQueue>().Count();

                    if (numpastqs == 0)
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
                        view = inflater.Inflate(Resource.Layout.PastQs, container, false);
                        ImageView backButton = view.FindViewById<ImageView>(Resource.Id.backButton);
                        backButton.Click += (sender, e) =>
                        {
                            Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();
                            trans.Replace(Resource.Id.qs_root_frame, new Fragments.CurrentPastQs());
                            trans.Commit();
                        };

                        ListView lv_pastqs = view.FindViewById<ListView>(Resource.Id.pastqslv);
                        for (int i = 1; i <= numpastqs; i++)
                        {
                            string newCompany = db_myPastQs.Get<SQLite_Tables.MyPastQueue>(i).company;
                            companies.Add(newCompany);
                        }
                        QsListViewAdapter adapter = new QsListViewAdapter(container.Context, companies, "PastQs");
                        lv_pastqs.Adapter = adapter;

                        return view;
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
        }
    }
}