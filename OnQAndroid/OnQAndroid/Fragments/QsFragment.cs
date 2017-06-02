using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using SQLite;
using Firebase.Xamarin.Database;
using OnQAndroid.FirebaseObjects;
using System.Threading.Tasks;
using Firebase.Xamarin.Database.Query;

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

        public int numStudents;
        public int numQs;
        private const string FirebaseURL = "https://onqfirebase.firebaseio.com/";
        MyAttributes myAttributes;
        View view;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            string dbPath_attributes = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "attributes.db3");
            SQLiteConnection db_attributes = new SQLiteConnection(dbPath_attributes);
            myAttributes = db_attributes.Get<MyAttributes>(1);

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
                    // get the view
                    GetRecruiterView();
                    // in the meantime, display loading
                    view = inflater.Inflate(Resource.Layout.HomeTab, container, false);
                    ProgressBar progressBar = view.FindViewById<ProgressBar>(Resource.Id.circularProgress);
                    progressBar.Visibility = ViewStates.Visible;
                    return view;
                }
                else if (myAttributes.type == "Student")
                {
                    // get the view
                    GetStudentView();
                    // in the meantime, display loading
                    view = inflater.Inflate(Resource.Layout.HomeTab, container, false);
                    ProgressBar progressBar = view.FindViewById<ProgressBar>(Resource.Id.circularProgress);
                    progressBar.Visibility = ViewStates.Visible;

                    return view;
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
        }

        private async void GetStudentView()
        {
            await GetNumQs();

            if (numQs == 0)
            {
                Fragments.NoQsPresent fragment = new Fragments.NoQsPresent();
                Bundle arguments = new Bundle();
                arguments.PutString("Sender", "CurrentQs");
                fragment.Arguments = arguments;

                Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();
                trans.Replace(Resource.Id.qs_root_frame, fragment);
                trans.Commit();
            }
            else
            {
                Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();
                trans.Replace(Resource.Id.qs_root_frame, new Fragments.StudentQsPresent());
                trans.Commit();
            }
        }

        private async Task GetNumQs()
        {
            var firebase = new FirebaseClient(FirebaseURL);

            string myQsFilename = "myqs_" + myAttributes.cfid.ToString() + "_" + myAttributes.typeid.ToString();
            var myQs = await firebase.Child("qs").Child(myQsFilename).OnceAsync<StudentQ>();

            numQs = myQs.Count;
        }

        private async void GetRecruiterView()
        {
            await GetNumStudents();

            if (numStudents == 0)
            {
                Fragments.NoQsPresent fragment = new Fragments.NoQsPresent();
                Bundle arguments = new Bundle();
                arguments.PutString("Sender", "CurrentQs");
                fragment.Arguments = arguments;

                Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();
                trans.Replace(Resource.Id.qs_root_frame, fragment);
                trans.Commit();
            }
            else
            {
                Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();
                trans.Replace(Resource.Id.qs_root_frame, new Fragments.ListQs());
                trans.Commit();
            }
        }

        private async Task GetNumStudents()
        {
            var firebase = new FirebaseClient(FirebaseURL);

            string myCompanyQFilename = "qs_" + myAttributes.cfid.ToString() + "_" + myAttributes.attribute1;
            var myCompanyQ = await firebase.Child("qs").Child(myCompanyQFilename).OnceAsync<Queue>();

            numStudents = myCompanyQ.Count;
        }
    }
}