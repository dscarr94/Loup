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
        int numpastqs;
        private const string FirebaseURL = "https://onqfirebase.firebaseio.com/";
        MyAttributes myAttributes;

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
                    GetRecruiterView();
                    // in the meantime
                    view = inflater.Inflate(Resource.Layout.HomeTab, container, false);
                    ProgressBar progressBar = view.FindViewById<ProgressBar>(Resource.Id.circularProgress);
                    progressBar.Visibility = ViewStates.Visible;
                    return view;
                }
                else if (myAttributes.type == "Student")
                {
                    GetStudentView();
                    // in the meantime
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
            await GetStudentNumPastQs();

            if (numpastqs == 0)
            {
                NoQsPresent fragment = new NoQsPresent();
                Bundle arguments = new Bundle();
                arguments.PutString("Sender", "PastQs");
                fragment.Arguments = arguments;

                Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();
                trans.Replace(Resource.Id.qs_root_frame, fragment);
                trans.Commit();
            }
            else
            {
                Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();
                trans.Replace(Resource.Id.qs_root_frame, new Fragments.StudentPastQs());
                trans.Commit();
            }
        }

        private async Task GetStudentNumPastQs()
        {
            var firebase = new FirebaseClient(FirebaseURL);

            string fileName_pastQs = "pastqs_" + myAttributes.typeid.ToString();
            var pastQs = await firebase.Child("pastqs").Child(fileName_pastQs).OnceAsync<StudentQ>();

            numpastqs = pastQs.Count;
        }

        private async void GetRecruiterView()
        {
            await GetRecruiterNumPastQs();

            if (numpastqs == 0)
            {
                NoQsPresent fragment = new NoQsPresent();
                Bundle arguments = new Bundle();
                arguments.PutString("Sender", "PastQs");
                fragment.Arguments = arguments;

                Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();
                trans.Replace(Resource.Id.qs_root_frame, fragment);
                trans.Commit();
            }
            else
            {
                Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();
                trans.Replace(Resource.Id.qs_root_frame, new RecruiterPastQs());
                trans.Commit();
            }
        }

        private async Task GetRecruiterNumPastQs()
        {
            var firebase = new FirebaseClient(FirebaseURL);

            string fileName_pastQs = "pastqs_" + myAttributes.attribute1;
            var pastQs = await firebase.Child("pastqs").Child(fileName_pastQs).OnceAsync<PastQ>();

            numpastqs = pastQs.Count;
        }
    }
}