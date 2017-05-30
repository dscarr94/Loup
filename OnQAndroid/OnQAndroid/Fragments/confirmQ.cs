using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using SQLite;
using Android.Support.V4.View;
using Firebase.Xamarin.Database;
using OnQAndroid.FirebaseObjects;
using Firebase.Xamarin.Database.Query;

namespace OnQAndroid.Fragments
{
    public class confirmQ : Android.Support.V4.App.Fragment
    {
        int companyInt;
        ViewPager viewPager;
        MyAttributes myAttributes;
        TextView companyName;
        ImageView companyLogo;
        ProgressBar progressBar;
        private const string FirebaseURL = "https://onqfirebase.firebaseio.com/";
        bool checkedIn;
        string thisCompany;

        public confirmQ()
        {
            // Empty constructor
        }

        public static confirmQ newInstance()
        {
            confirmQ fragment = new confirmQ();
            return fragment;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Bundle arguments = Arguments;
            companyInt = arguments.GetInt("CompanyInt");
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.ConfirmQ, container, false);

            progressBar = view.FindViewById<ProgressBar>(Resource.Id.circularProgress);
            viewPager = Activity.FindViewById<ViewPager>(Resource.Id.viewpager);
            companyName = view.FindViewById<TextView>(Resource.Id.companyName);
            companyLogo = view.FindViewById<ImageView>(Resource.Id.companyLogo);
            Button cancelq = view.FindViewById<Button>(Resource.Id.cancelqbutton);
            Button confirmq = view.FindViewById<Button>(Resource.Id.confirmqbutton);

            string dbpath_attributes = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "attributes.db3");
            SQLiteConnection db_attributes = new SQLiteConnection(dbpath_attributes);
            myAttributes = db_attributes.Get<MyAttributes>(1);

            LoadCompanyInfo();          

            cancelq.Click += Cancelq_Click;

            confirmq.Click += Confirmq_Click;

            return view;
        }

        private async void LoadCompanyInfo()
        {
            progressBar.Visibility = ViewStates.Visible;
            var firebase = new FirebaseClient(FirebaseURL);
            var companyItems = await firebase.Child(myAttributes.cfid.ToString()).OnceAsync<Company>();

            foreach (var company in companyItems)
            {
                if (company.Object.companyid == companyInt.ToString())
                {
                    companyName.Text = company.Object.name + "?";

                    thisCompany = company.Object.name;

                    string imageName = company.Object.name.ToLower().Replace(" ", "");
                    int resourceId = (int)typeof(Resource.Drawable).GetField(imageName).GetValue(null);
                    companyLogo.SetImageResource(resourceId);

                    checkedIn = company.Object.checkedIn;
                }
            }
            progressBar.Visibility = ViewStates.Invisible;
        }

        private async void Confirmq_Click(object sender, EventArgs e)
        {
            progressBar.Visibility = ViewStates.Visible;            
            string fileName_myQs = "myqs_" + myAttributes.cfid.ToString() + "_" + myAttributes.typeid;
            string fileName_Q = "qs_" + myAttributes.cfid.ToString() + "_" + thisCompany;
            string fileName_careerFair = myAttributes.cfid.ToString();

            var firebase = new FirebaseClient(FirebaseURL);

            if (checkedIn == true)
            {
                bool qExists = false;
                var myQs = await firebase.Child(fileName_myQs).OnceAsync<StudentQ>();

                foreach (var q in myQs)
                {
                    if (q.Object.company == thisCompany)
                    {
                        qExists = true;
                        break;
                    }
                }

                if (qExists == true)
                {
                    progressBar.Visibility = ViewStates.Invisible;
                    Toast.MakeText(this.Activity, "You are already onQ with this company", ToastLength.Short).Show();
                }
                else
                {
                    var Qs = await firebase.Child(fileName_Q).OnceAsync<Queue>();
                    var thisCareerFair = await firebase.Child(fileName_careerFair).OnceAsync<Company>();

                    int numQs = Qs.Count; // number of people in the queue
                    int numMyQs = myQs.Count; // number of q's a student has, may want to limit???

                    Queue newQ = new Queue();
                    newQ.position = (numQs + 1).ToString();
                    newQ.studentid = myAttributes.typeid.ToString();
                    newQ.studentname = myAttributes.name;

                    StudentQ newMyQ = new StudentQ();
                    newMyQ.position = (numQs + 1).ToString();
                    newMyQ.company = thisCompany;

                    Company newCompanyInfo = new Company();
                    string companyKey = "";

                    foreach (var company in thisCareerFair)
                    {
                        if (company.Object.name == thisCompany)
                        {
                            companyKey = company.Key;
                            newCompanyInfo.companyid = company.Object.companyid;
                            newCompanyInfo.name = company.Object.name;
                            newCompanyInfo.description = company.Object.description;
                            newCompanyInfo.website = company.Object.website;
                            newCompanyInfo.rak = company.Object.rak;
                            newCompanyInfo.checkedIn = company.Object.checkedIn;
                            newCompanyInfo.waittime = company.Object.waittime;
                            int newNumStudents = Convert.ToInt32(company.Object.numstudents) + 1;
                            newCompanyInfo.numstudents = newNumStudents.ToString();
                            break;
                        }
                    }

                    await firebase.Child(fileName_Q).PostAsync(newQ);
                    await firebase.Child(fileName_myQs).PostAsync(newMyQ);
                    await firebase.Child(fileName_careerFair).Child(companyKey).PutAsync(newCompanyInfo);

                    Toast.MakeText(this.Activity, "You are onQ in Position " + (numQs + 1).ToString() + "!", ToastLength.Short).Show();
                    progressBar.Visibility = ViewStates.Invisible;

                    Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();
                    trans.Replace(Resource.Id.companies_root_frame, new CompaniesFragment());
                    trans.Commit();

                    Android.Support.V4.App.FragmentTransaction trans2 = FragmentManager.BeginTransaction();
                    trans.Replace(Resource.Id.qs_root_frame, new QsFragment());
                    trans2.Commit();

                    viewPager.SetCurrentItem(1, true);
                }
            }
            else
            {
                progressBar.Visibility = ViewStates.Invisible;
                Toast.MakeText(this.Activity, "No Recruiters have checked in with this company yet...", ToastLength.Short).Show();
            }
        }

        private void Cancelq_Click(object sender, EventArgs e)
        {
            Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();
            trans.Replace(Resource.Id.companies_root_frame, new CompaniesFragment());
            trans.Commit();
        }
    }
}