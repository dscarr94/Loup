using System;
using System.Collections.Generic;
using System.Linq;
using Android.OS;
using Android.Views;
using Android.Widget;
using SQLite;
using Firebase.Xamarin.Database;
using OnQAndroid.FirebaseObjects;
using Firebase.Xamarin.Database.Query;

namespace OnQAndroid
{
    public class CompaniesFragment : Android.Support.V4.App.Fragment
    {
        public CompaniesFragment()
        {
            // Required empty public constructor
        }

        public static CompaniesFragment newInstance()
        {
            CompaniesFragment fragment = new OnQAndroid.CompaniesFragment();
            return fragment;
        }

        Button plus_major;
        Button minus_major;
        Spinner hmspinner1;
        Spinner hmspinner2;
        Spinner hmspinner3;
        Spinner hmspinner4;
        Spinner hmspinner5;
        Button HMAllRadio;
        Button HGTAllRadio;
        Button plus_gt;
        Button minus_gt;
        Spinner hgtspinner1;
        Spinner hgtspinner2;
        Spinner hgtspinner3;
        Button HGPANoneRadio;
        Spinner minGPAspinner;

        bool isHGTAll;
        bool isHMAll;
        bool isMinGPANone;

        int numMPs;
        int numGTPs;
        int numGPAs;

        MyAttributes myAttributes;
        string myCFName;
        List<string> mItems;
        TextView cfName;
        ListView mListView;
        ViewGroup mContainer;
        ProgressBar progressBar;

        private const string FirebaseURL = "https://onqfirebase.firebaseio.com/";

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            mContainer = container;
            string dbPath_attributes = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "attributes.db3");
            var db_attributes = new SQLiteConnection(dbPath_attributes);

            myAttributes = db_attributes.Get<MyAttributes>(1);
            int myCFID = myAttributes.cfid;

            if (myCFID == 0) // If not registered for a career fair
            {
                View view = inflater.Inflate(Resource.Layout.GoToRegister, container, false);
                return view;
            }

            else
            {
                if (myAttributes.type == "Student")
                {
                    // Inflate View
                    View view = inflater.Inflate(Resource.Layout.HomeTab, container, false);

                    // Call UI Elements
                    mListView = view.FindViewById<ListView>(Resource.Id.listView1);
                    cfName = view.FindViewById<TextView>(Resource.Id.cfName);
                    progressBar = view.FindViewById<ProgressBar>(Resource.Id.circularProgress);

                    mItems = new List<string>();

                    LoadCF();

                    return view;
                }
                else if (myAttributes.type == "Recruiter")
                {
                    View view = inflater.Inflate(Resource.Layout.RecruiterHomeTab, container, false);

                    progressBar = view.FindViewById<ProgressBar>(Resource.Id.circularProgress);

                    HMAllRadio = view.FindViewById<Button>(Resource.Id.HMAllRadio);
                    HGTAllRadio = view.FindViewById<Button>(Resource.Id.HGTAllRadio);
                    HGPANoneRadio = view.FindViewById<Button>(Resource.Id.HGPANoneRadio);
                    plus_major = view.FindViewById<Button>(Resource.Id.plus_majors);
                    hmspinner1 = view.FindViewById<Spinner>(Resource.Id.hmspinner1);
                    hmspinner2 = view.FindViewById<Spinner>(Resource.Id.hmspinner2);
                    hmspinner3 = view.FindViewById<Spinner>(Resource.Id.hmspinner3);
                    hmspinner4 = view.FindViewById<Spinner>(Resource.Id.hmspinner4);
                    hmspinner5 = view.FindViewById<Spinner>(Resource.Id.hmspinner5);
                    minus_major = view.FindViewById<Button>(Resource.Id.minus_majors);

                    hgtspinner1 = view.FindViewById<Spinner>(Resource.Id.hgtspinner1);
                    hgtspinner2 = view.FindViewById<Spinner>(Resource.Id.hgtspinner2);
                    hgtspinner3 = view.FindViewById<Spinner>(Resource.Id.hgtspinner3);
                    plus_gt = view.FindViewById<Button>(Resource.Id.plus_gt);
                    minus_gt = view.FindViewById<Button>(Resource.Id.minus_gradterms);

                    minGPAspinner = view.FindViewById<Spinner>(Resource.Id.minGPAspinner);

                    Button saveChanges = view.FindViewById<Button>(Resource.Id.saveChangesButton);
                    cfName = view.FindViewById<TextView>(Resource.Id.cfName);
                    TextView HMAllText = view.FindViewById<TextView>(Resource.Id.HMAllText);
                    TextView HGTAllText = view.FindViewById<TextView>(Resource.Id.HGTAllText);
                    TextView MinGPANoneText = view.FindViewById<TextView>(Resource.Id.MinGPANoneText);
                    Space plus1extender = view.FindViewById<Space>(Resource.Id.plusspace1);
                    Space plus2extender = view.FindViewById<Space>(Resource.Id.plusspace2);

                    // Change name to my CF name
                    PopulateName();                   

                    // Click methods
                    HMAllRadio.Click += HMAllRadio_Click;
                    HMAllText.Click += HMAllRadio_Click;
                    HGTAllRadio.Click += HGTAllRadio_Click;
                    HGTAllText.Click += HGTAllRadio_Click;
                    HGPANoneRadio.Click += HGPANoneRadio_Click;
                    MinGPANoneText.Click += HGPANoneRadio_Click;
                    saveChanges.Click += SaveChanges_Click;

                    // connect to firebase preferences database
                    PopulatePreferences();                   

                    // On plus major click
                    plus_major.Click += Plus_major_Click;
                    plus1extender.Click += Plus_major_Click;

                    // On plus gt click
                    plus_gt.Click += Plus_gt_Click;
                    plus2extender.Click += Plus_gt_Click;

                    // on minus major click
                    minus_major.Click += (sender, e) =>
                    {
                        numMPs = numMPs - 1;
                        if (numMPs == 1)
                        {
                            hmspinner2.Visibility = ViewStates.Gone;
                            minus_major.Visibility = ViewStates.Gone;
                        }
                        else if (numMPs == 2)
                        {
                            hmspinner3.Visibility = ViewStates.Gone;
                        }
                        else if (numMPs == 3)
                        {
                            hmspinner4.Visibility = ViewStates.Gone;
                        }
                        else if (numMPs == 4)
                        {
                            hmspinner5.Visibility = ViewStates.Gone;
                        }
                    };
                    // on minus grad term click
                    minus_gt.Click += (sender, e) =>
                    {
                        numGTPs = numGTPs - 1;
                        if (numGTPs == 1)
                        {
                            hgtspinner2.Visibility = ViewStates.Gone;
                            minus_gt.Visibility = ViewStates.Gone;
                        }
                        else if (numGTPs == 2)
                        {
                            hgtspinner3.Visibility = ViewStates.Gone;
                        }
                    };

                    return view;
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
        }

        private async void PopulateName()
        {
            progressBar.Visibility = ViewStates.Visible;
            var firebase = new FirebaseClient(FirebaseURL);

            var allCFs = await firebase.Child("cfids").OnceAsync<Cfid>();

            foreach (var cf in allCFs)
            {
                if (cf.Object.cfid == myAttributes.cfid.ToString())
                {
                    cfName.Text = cf.Object.name;
                }
            }
        }

        private async void PopulatePreferences()
        {
            //progressBar.Visibility = ViewStates.Visible;
            var firebase = new FirebaseClient(FirebaseURL);
            // Get number of major and grad term preferences
            List<string> majorPreferences = new List<string>();
            List<string> gradtermPreferences = new List<string>();
            List<string> gpaPreferences = new List<string>();

            string majorFileName = "mps_" + myAttributes.cfid.ToString() + "_" + myAttributes.attribute1;
            var majorPrefs = await firebase.Child("candidatepreferences").Child(majorFileName).OnceAsync<MajorPreference>();
            numMPs = majorPrefs.Count();
            foreach (var mp in majorPrefs)
            {
                majorPreferences.Add(mp.Object.major);
            }

            string gradtermFileName = "gtps_" + myAttributes.cfid.ToString() + "_" + myAttributes.attribute1;
            var gradtermPrefs = await firebase.Child("candidatepreferences").Child(gradtermFileName).OnceAsync<GradTermPreference>();
            numGTPs = gradtermPrefs.Count();
            foreach (var gtp in gradtermPrefs)
            {
                gradtermPreferences.Add(gtp.Object.gradterm);
            }

            string gpaFileName = "gpas_" + myAttributes.cfid.ToString() + "_" + myAttributes.attribute1;
            var gpaPrefs = await firebase.Child("candidatepreferences").Child(gpaFileName).OnceAsync<GPAPreference>();
            numGPAs = gpaPrefs.Count();
            foreach (var gpap in gpaPrefs)
            {
                gpaPreferences.Add(gpap.Object.gpa);
            }

            // Populate list items in major spinner
            var majoradapter = ArrayAdapter.CreateFromResource(this.Activity, Resource.Array.major_array, Android.Resource.Layout.SimpleSpinnerItem);
            majoradapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            hmspinner1.Adapter = majoradapter;
            hmspinner2.Adapter = majoradapter;
            hmspinner3.Adapter = majoradapter;
            hmspinner4.Adapter = majoradapter;
            hmspinner5.Adapter = majoradapter;

            // Populate list items in grad term spinner
            var gradtermadapter = ArrayAdapter.CreateFromResource(this.Activity, Resource.Array.year_array, Android.Resource.Layout.SimpleSpinnerItem);
            gradtermadapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            hgtspinner1.Adapter = gradtermadapter;
            hgtspinner2.Adapter = gradtermadapter;
            hgtspinner3.Adapter = gradtermadapter;

            // Populate list items in gpa spinner
            var gpaadapter = ArrayAdapter.CreateFromResource(this.Activity, Resource.Array.gpa_array, Android.Resource.Layout.SimpleSpinnerItem);
            gpaadapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            minGPAspinner.Adapter = gpaadapter;

            // Populate gpa spinner with current preference
            if (numGPAs == 0)
            {
                isMinGPANone = true;

                minGPAspinner.Enabled = false;
                minGPAspinner.Visibility = ViewStates.Invisible;
            }
            if (numGPAs >= 1)
            {
                isMinGPANone = false;
                HGPANoneRadio.SetBackgroundResource(Resource.Drawable.radio_unchecked);
                minGPAspinner.Visibility = ViewStates.Visible;
                string defaultGPA = gpaPreferences[0];
                if (!defaultGPA.Equals(null))
                {
                    int spinnerPosition = gpaadapter.GetPosition(defaultGPA);
                    minGPAspinner.SetSelection(spinnerPosition);
                }
            }

            // Populate grad term spinners with current preferences
            if (numGTPs == 0)
            {
                isHGTAll = true;

                hgtspinner1.Enabled = false;
                hgtspinner1.Visibility = ViewStates.Invisible;
                plus_gt.Enabled = false;
                plus_gt.Visibility = ViewStates.Invisible;
            }
            if (numGTPs >= 1)
            {
                isHGTAll = false;
                HGTAllRadio.SetBackgroundResource(Resource.Drawable.radio_unchecked);
                hgtspinner1.Visibility = ViewStates.Visible;
                plus_gt.Visibility = ViewStates.Visible;
                string defaultGTP = gradtermPreferences[0];
                if (!defaultGTP.Equals(null))
                {
                    int spinnerPosition = gradtermadapter.GetPosition(defaultGTP);
                    hgtspinner1.SetSelection(spinnerPosition);
                }
            }
            if (numGTPs >= 2)
            {
                hgtspinner2.Visibility = ViewStates.Visible;
                minus_gt.Visibility = ViewStates.Visible;
                string defaultGTP = gradtermPreferences[1];
                if (!defaultGTP.Equals(null))
                {
                    int spinnerPosition = gradtermadapter.GetPosition(defaultGTP);
                    hgtspinner2.SetSelection(spinnerPosition);
                }
            }
            if (numGTPs >= 3)
            {
                hgtspinner3.Visibility = ViewStates.Visible;
                string defaultGTP = gradtermPreferences[2];
                if (!defaultGTP.Equals(null))
                {
                    int spinnerPosition = gradtermadapter.GetPosition(defaultGTP);
                    hgtspinner3.SetSelection(spinnerPosition);
                }
            }

            // Populate major spinners with current preferences
            if (numMPs == 0)
            {
                isHMAll = true;

                hmspinner1.Enabled = false;
                hmspinner1.Visibility = ViewStates.Invisible;
                plus_major.Enabled = false;
                plus_major.Visibility = ViewStates.Invisible;
            }

            if (numMPs >= 1)
            {
                isHMAll = false;
                HMAllRadio.SetBackgroundResource(Resource.Drawable.radio_unchecked);
                hmspinner1.Visibility = ViewStates.Visible;
                plus_major.Visibility = ViewStates.Visible;
                string defaultMP = majorPreferences[0];
                if (!defaultMP.Equals(null))
                {
                    int spinnerPosition = majoradapter.GetPosition(defaultMP);
                    hmspinner1.SetSelection(spinnerPosition);
                }

            }
            if (numMPs >= 2)
            {
                hmspinner2.Visibility = ViewStates.Visible;
                minus_major.Visibility = ViewStates.Visible;
                string defaultMP = majorPreferences[1];
                if (!defaultMP.Equals(null))
                {
                    int spinnerPosition = majoradapter.GetPosition(defaultMP);
                    hmspinner2.SetSelection(spinnerPosition);
                }
            }
            if (numMPs >= 3)
            {
                hmspinner3.Visibility = ViewStates.Visible;
                string defaultMP = majorPreferences[2];
                if (!defaultMP.Equals(null))
                {
                    int spinnerPosition = majoradapter.GetPosition(defaultMP);
                    hmspinner3.SetSelection(spinnerPosition);
                }
            }
            if (numMPs >= 4)
            {
                hmspinner4.Visibility = ViewStates.Visible;
                string defaultMP = majorPreferences[3];
                if (!defaultMP.Equals(null))
                {
                    int spinnerPosition = majoradapter.GetPosition(defaultMP);
                    hmspinner4.SetSelection(spinnerPosition);
                }
            }
            if (numMPs >= 5)
            {
                hmspinner5.Visibility = ViewStates.Visible;
                string defaultMP = majorPreferences[4];
                if (!defaultMP.Equals(null))
                {
                    int spinnerPosition = majoradapter.GetPosition(defaultMP);
                    hmspinner5.SetSelection(spinnerPosition);
                }
            }
            progressBar.Visibility = ViewStates.Invisible;
        }

        private async void LoadCF()
        {
            progressBar.Visibility = ViewStates.Visible;
            var firebase = new FirebaseClient(FirebaseURL);

            var cfItems = await firebase.Child("cfids").OnceAsync<Cfid>();

            foreach (var item in cfItems)
            {
                if (item.Object.cfid == myAttributes.cfid.ToString())
                {
                    myCFName = item.Object.name;
                    break;
                }
            }

            var myCFcompanies = await firebase.Child("careerfairs").Child(myAttributes.cfid.ToString()).OnceAsync<Company>();
            List<string> mWaitTimes = new List<string>();
            List<string> mNumStudents = new List<string>();

            foreach (var company in myCFcompanies)
            {
                mItems.Add(company.Object.name);
                long totalNumStudents = Convert.ToInt32(company.Object.numstudents);
                long partialWaitTime = Convert.ToInt64(company.Object.waittime);
                long totalWaitTime = partialWaitTime * totalNumStudents;
                TimeSpan ts = TimeSpan.FromTicks(totalWaitTime);
                string waittime = String.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
                mWaitTimes.Add(waittime);
                mNumStudents.Add(totalNumStudents.ToString());
            }

            string favoritesFileName = "fav_" + myAttributes.cfid.ToString() + "_" + myAttributes.typeid.ToString();
            var myFavorites = await firebase.Child("favorites").Child(favoritesFileName).OnceAsync<Favorite>();
            List<bool> favList = new List<bool>();

            foreach (var favorite in myFavorites)
            {
                favList.Add(favorite.Object.isFavorite);
            }

            cfName.Text = myCFName;
            CompaniesListViewAdapter adapter = new CompaniesListViewAdapter(mContainer.Context, mItems, favList, mWaitTimes, mNumStudents);
            mListView.Adapter = adapter;
            progressBar.Visibility = ViewStates.Invisible;
        }

        private void Plus_gt_Click(object sender, EventArgs e)
        {
            if (numGTPs == 1)
            {
                numGTPs = numGTPs + 1;
                minus_gt.Visibility = ViewStates.Visible;
                hgtspinner2.Visibility = ViewStates.Visible;
            }
            else if (numGTPs == 2)
            {
                numGTPs = numGTPs + 1;
                hgtspinner3.Visibility = ViewStates.Visible;
            }
            else if (numGTPs == 3)
            {
                Toast.MakeText(this.Activity, "Please Select a Maximum of 3 Grad Term Preferences", ToastLength.Short).Show();
            }
        }

        private void Plus_major_Click(object sender, EventArgs e)
        {
            if (numMPs == 1)
            {
                numMPs = numMPs + 1;
                minus_major.Visibility = ViewStates.Visible;
                hmspinner2.Visibility = ViewStates.Visible;
            }
            else if (numMPs == 2)
            {
                numMPs = numMPs + 1;
                hmspinner3.Visibility = ViewStates.Visible;
            }
            else if (numMPs == 3)
            {
                numMPs = numMPs + 1;
                hmspinner4.Visibility = ViewStates.Visible;
            }
            else if (numMPs == 4)
            {
                numMPs = numMPs + 1;
                hmspinner5.Visibility = ViewStates.Visible;
            }
            else if (numMPs == 5)
            {
                Toast.MakeText(this.Activity, "Please Select a Maximum of 5 Major Preferences", ToastLength.Short).Show();
            }
        }

        private async void SaveChanges_Click(object sender, EventArgs e)
        {
            progressBar.Visibility = ViewStates.Visible;

            var firebase = new FirebaseClient(FirebaseURL);
            string majorFileName = "mps_" + myAttributes.cfid.ToString() + "_" + myAttributes.attribute1;
            string gradtermFileName = "gtps_" + myAttributes.cfid.ToString() + "_" + myAttributes.attribute1;
            string gpaFileName = "gpas_" + myAttributes.cfid.ToString() + "_" + myAttributes.attribute1;

            await firebase.Child("candidatepreferences").Child(majorFileName).DeleteAsync();
            await firebase.Child("candidatepreferences").Child(gradtermFileName).DeleteAsync();
            await firebase.Child("candidatepreferences").Child(gpaFileName).DeleteAsync();

            if (isHMAll == true)
            {
                //do nothing
            }
            else
            {
                if (numMPs >= 1)
                {
                    MajorPreference majorPrefs1 = new MajorPreference();
                    majorPrefs1.id = "1";
                    majorPrefs1.major = hmspinner1.SelectedItem.ToString();

                    await firebase.Child("candidatepreferences").Child(majorFileName).PostAsync(majorPrefs1);
                }
                if (numMPs >= 2)
                {
                    MajorPreference majorPrefs2 = new MajorPreference();
                    majorPrefs2.id = "2";
                    majorPrefs2.major = hmspinner2.SelectedItem.ToString();

                    await firebase.Child("candidatepreferences").Child(majorFileName).PostAsync(majorPrefs2);
                }
                if (numMPs >= 3)
                {
                    MajorPreference majorPrefs3 = new MajorPreference();
                    majorPrefs3.id = "3";
                    majorPrefs3.major = hmspinner3.SelectedItem.ToString();

                    await firebase.Child("candidatepreferences").Child(majorFileName).PostAsync(majorPrefs3);
                }
                if (numMPs >= 4)
                {
                    MajorPreference majorPrefs4 = new MajorPreference();
                    majorPrefs4.id = "4";
                    majorPrefs4.major = hmspinner4.SelectedItem.ToString();

                    await firebase.Child("candidatepreferences").Child(majorFileName).PostAsync(majorPrefs4);
                }
                if (numMPs >= 5)
                {
                    MajorPreference majorPrefs5 = new MajorPreference();
                    majorPrefs5.id = "5";
                    majorPrefs5.major = hmspinner5.SelectedItem.ToString();

                    await firebase.Child("candidatepreferences").Child(majorFileName).PostAsync(majorPrefs5);
                }
            }
            if (isHGTAll == true)
            {
                //do nothing
            }
            else
            {
                if (numGTPs >= 1)
                {
                    GradTermPreference gradtermPrefs1 = new GradTermPreference();
                    gradtermPrefs1.id = "1";
                    gradtermPrefs1.gradterm = hgtspinner1.SelectedItem.ToString();

                    await firebase.Child("candidatepreferences").Child(gradtermFileName).PostAsync(gradtermPrefs1);
                }
                if (numGTPs >= 2)
                {
                    GradTermPreference gradtermPrefs2 = new GradTermPreference();
                    gradtermPrefs2.id = "2";
                    gradtermPrefs2.gradterm = hgtspinner2.SelectedItem.ToString();

                    await firebase.Child("candidatepreferences").Child(gradtermFileName).PostAsync(gradtermPrefs2);
                }
                if (numGTPs >= 3)
                {
                    GradTermPreference gradtermPrefs3 = new GradTermPreference();
                    gradtermPrefs3.id = "3";
                    gradtermPrefs3.gradterm = hgtspinner3.SelectedItem.ToString();

                    await firebase.Child("candidatepreferences").Child(gradtermFileName).PostAsync(gradtermPrefs3);
                }
            }
            if (isMinGPANone == true)
            {
                //do nothing
            }
            else
            {
                if (numGPAs == 1)
                {
                    GPAPreference gpaPrefs = new GPAPreference();
                    gpaPrefs.id = "1";
                    gpaPrefs.gpa = minGPAspinner.SelectedItem.ToString();

                    await firebase.Child("candidatepreferences").Child(gpaFileName).PostAsync(gpaPrefs);
                }
            }
            progressBar.Visibility = ViewStates.Invisible;
            Toast.MakeText(this.Activity, "Changes Saved", ToastLength.Short).Show();
        }

        /*private void Hmspinner1_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner hmSpinner1 = (Spinner)sender;
        }

        private void Hmspinner2_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner hmspinner2 = (Spinner)sender;
        }

        private void Hmspinner3_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner hmspinner3 = (Spinner)sender;
        }

        private void Hmspinner4_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner hmspinner4 = (Spinner)sender;
        }

        private void Hmspinner5_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner hmspinner5 = (Spinner)sender;
        }*/

        private void HGTAllRadio_Click(object sender, EventArgs e)
        {
            if (isHGTAll == true)
            {
                isHGTAll = false;
                HGTAllRadio.SetBackgroundResource(Resource.Drawable.radio_unchecked);

                plus_gt.Enabled = true;
                plus_gt.Visibility = ViewStates.Visible;

                hgtspinner1.Enabled = true;
                hgtspinner1.Visibility = ViewStates.Visible;

                numGTPs = 1;
            }
            else if (isHGTAll == false)
            {
                isHGTAll = true;
                HGTAllRadio.SetBackgroundResource(Resource.Drawable.radio_checked);

                plus_gt.Enabled = false;
                plus_gt.Visibility = ViewStates.Invisible;

                hgtspinner1.Enabled = false;
                hgtspinner1.Visibility = ViewStates.Invisible;

                minus_gt.Visibility = ViewStates.Gone;
                hgtspinner2.Visibility = ViewStates.Gone;
                hgtspinner3.Visibility = ViewStates.Gone;
            }
        }

        private void HMAllRadio_Click(object sender, EventArgs e)
        {
            if (isHMAll == true)
            {
                isHMAll = false;
                HMAllRadio.SetBackgroundResource(Resource.Drawable.radio_unchecked);

                plus_major.Enabled = true;
                plus_major.Visibility = ViewStates.Visible;

                hmspinner1.Enabled = true;
                hmspinner1.Visibility = ViewStates.Visible;

                numMPs = 1;
            }
            else if (isHMAll == false)
            {
                isHMAll = true;
                HMAllRadio.SetBackgroundResource(Resource.Drawable.radio_checked);

                plus_major.Enabled = false;
                plus_major.Visibility = ViewStates.Invisible;

                hmspinner1.Enabled = false;
                hmspinner1.Visibility = ViewStates.Invisible;

                minus_major.Visibility = ViewStates.Gone;
                hmspinner2.Visibility = ViewStates.Gone;
                hmspinner3.Visibility = ViewStates.Gone;
                hmspinner4.Visibility = ViewStates.Gone;
                hmspinner5.Visibility = ViewStates.Gone;       
            }
        }

        private void HGPANoneRadio_Click(object sender, EventArgs e)
        {
            if (isMinGPANone == true)
            {
                isMinGPANone = false;
                HGPANoneRadio.SetBackgroundResource(Resource.Drawable.radio_unchecked);

                minGPAspinner.Enabled = true;
                minGPAspinner.Visibility = ViewStates.Visible;

                numGPAs = 1;
            }
            else if (isMinGPANone == false)
            {
                isMinGPANone = true;
                HGPANoneRadio.SetBackgroundResource(Resource.Drawable.radio_checked);

                minGPAspinner.Enabled = false;
                minGPAspinner.Visibility = ViewStates.Invisible;
            }
        }
    }
}