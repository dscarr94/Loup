using System;
using System.Collections.Generic;
using System.Linq;
using Android.OS;
using Android.Views;
using Android.Widget;
using SQLite;

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

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            string dbPath_attributes = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "attributes.db3");
            var db_attributes = new SQLiteConnection(dbPath_attributes);

            var myAttributes = db_attributes.Get<MyAttributes>(1);
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
                    ListView mListView = view.FindViewById<ListView>(Resource.Id.listView1);
                    LinearLayout companyInfo = mListView.FindViewById<LinearLayout>(Resource.Id.companyInfo);
                    TextView cfName = view.FindViewById<TextView>(Resource.Id.cfName);

                    // Connect to CFID database
                    string dbPath_cfids = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "CFIDs.db3");
                    var db_cfids = new SQLiteConnection(dbPath_cfids);
                    db_cfids.CreateTable<Cfids>();

                    // Query CFIDs table for myCFID
                    var cfid_queryResults = db_cfids.Query<Cfids>("SELECT * FROM Cfids WHERE cfid = ?", myCFID.ToString());
                    Cfids cfid = cfid_queryResults.First();
                    cfName.Text = cfid.name;
                    List<string> mItems = new List<string>();

                    // Connect to myCFID database
                    string fileName = myCFID.ToString() + ".db3";
                    string dbPath_companies = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), fileName);
                    var db_companies = new SQLiteConnection(dbPath_companies);

                    // Get number of rows in myCFID Companies table
                    int rows = db_companies.Table<Companies>().Count();

                    // Populate list items
                    for (int i = 1; i <= rows; i = i + 1)
                    {
                        var newRow = db_companies.Get<Companies>(i);
                        string newItem = newRow.name;
                        mItems.Add(newItem);
                    }

                    // Provide list items to list view adapter
                    CompaniesListViewAdapter adapter = new CompaniesListViewAdapter(container.Context, mItems);
                    mListView.Adapter = adapter;
                    return view;
                }
                else if (myAttributes.type == "Recruiter")
                {
                    View view = inflater.Inflate(Resource.Layout.RecruiterHomeTab, container, false);
                    
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
                    TextView cfName = view.FindViewById<TextView>(Resource.Id.cfName);
                    TextView HMAllText = view.FindViewById<TextView>(Resource.Id.HMAllText);
                    TextView HGTAllText = view.FindViewById<TextView>(Resource.Id.HGTAllText);
                    TextView MinGPANoneText = view.FindViewById<TextView>(Resource.Id.MinGPANoneText);
                    Space plus1extender = view.FindViewById<Space>(Resource.Id.plusspace1);
                    Space plus2extender = view.FindViewById<Space>(Resource.Id.plusspace2);

                    // Change name to my CF name
                    string dbPath_cfids = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "CFIDs.db3");
                    var db_cfids = new SQLiteConnection(dbPath_cfids);
                    db_cfids.CreateTable<Cfids>();

                    var cfid_queryResults = db_cfids.Query<Cfids>("SELECT * FROM Cfids WHERE cfid = ?", myCFID.ToString());
                    Cfids cfid = cfid_queryResults.First();
                    cfName.Text = cfid.name;

                    // Item Selected Methods
                    /*hmspinner1.ItemSelected += Hmspinner1_ItemSelected;
                    hmspinner2.ItemSelected += Hmspinner2_ItemSelected;
                    hmspinner3.ItemSelected += Hmspinner3_ItemSelected;
                    hmspinner4.ItemSelected += Hmspinner4_ItemSelected;
                    hmspinner5.ItemSelected += Hmspinner5_ItemSelected;*/

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

                    // Click methods
                    HMAllRadio.Click += HMAllRadio_Click;
                    HMAllText.Click += HMAllRadio_Click;
                    HGTAllRadio.Click += HGTAllRadio_Click;
                    HGTAllText.Click += HGTAllRadio_Click;
                    HGPANoneRadio.Click += HGPANoneRadio_Click;
                    MinGPANoneText.Click += HGPANoneRadio_Click;
                    saveChanges.Click += SaveChanges_Click;

                    // Connect to preferences database
                    string myCompany = myAttributes.attribute1;
                    string mPreferences_fileName = "mp_" + myCFID + "_" + myCompany + ".db3";
                    string dbPath_mPreferences = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), mPreferences_fileName);
                    var db_mPreferences = new SQLiteConnection(dbPath_mPreferences);

                    // Get number of major and grad term preferences
                    numMPs = db_mPreferences.Table<SQLite_Tables.MajorPreferences>().Count();
                    numGTPs = db_mPreferences.Table<SQLite_Tables.GradTermPreferences>().Count();
                    numGPAs = db_mPreferences.Table<SQLite_Tables.GPAPreferences>().Count();

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
                        string defaultGPA = db_mPreferences.Get<SQLite_Tables.GPAPreferences>(1).gpa;
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
                        string defaultGTP = db_mPreferences.Get<SQLite_Tables.GradTermPreferences>(1).gradterm;
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
                        string defaultGTP = db_mPreferences.Get<SQLite_Tables.GradTermPreferences>(2).gradterm;
                        if (!defaultGTP.Equals(null))
                        {
                            int spinnerPosition = gradtermadapter.GetPosition(defaultGTP);
                            hgtspinner2.SetSelection(spinnerPosition);
                        }
                    }
                    if (numGTPs >= 3)
                    {
                        hgtspinner3.Visibility = ViewStates.Visible;
                        string defaultGTP = db_mPreferences.Get<SQLite_Tables.GradTermPreferences>(3).gradterm;
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
                        string defaultMP = db_mPreferences.Get<SQLite_Tables.MajorPreferences>(1).major;
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
                        string defaultMP = db_mPreferences.Get<SQLite_Tables.MajorPreferences>(2).major;
                        if (!defaultMP.Equals(null))
                        {
                            int spinnerPosition = majoradapter.GetPosition(defaultMP);
                            hmspinner2.SetSelection(spinnerPosition);
                        }                  
                    }
                    if (numMPs >= 3)
                    {
                        hmspinner3.Visibility = ViewStates.Visible;
                        string defaultMP = db_mPreferences.Get<SQLite_Tables.MajorPreferences>(3).major;
                        if (!defaultMP.Equals(null))
                        {
                            int spinnerPosition = majoradapter.GetPosition(defaultMP);
                            hmspinner3.SetSelection(spinnerPosition);
                        }
                    }
                    if (numMPs >= 4)
                    {
                        hmspinner4.Visibility = ViewStates.Visible;
                        string defaultMP = db_mPreferences.Get<SQLite_Tables.MajorPreferences>(4).major;
                        if (!defaultMP.Equals(null))
                        {
                            int spinnerPosition = majoradapter.GetPosition(defaultMP);
                            hmspinner4.SetSelection(spinnerPosition);
                        }
                    }
                    if (numMPs >= 5)
                    {
                        hmspinner5.Visibility = ViewStates.Visible;
                        string defaultMP = db_mPreferences.Get<SQLite_Tables.MajorPreferences>(5).major;
                        if (!defaultMP.Equals(null))
                        {
                            int spinnerPosition = majoradapter.GetPosition(defaultMP);
                            hmspinner5.SetSelection(spinnerPosition);
                        }
                    }

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

        private void SaveChanges_Click(object sender, EventArgs e)
        {
            string dbPath_attributes = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "attributes.db3");
            var db_attributes = new SQLiteConnection(dbPath_attributes);
            MyAttributes myAttributes = db_attributes.Get<MyAttributes>(1);
            int myCFID = myAttributes.cfid;

            string mPreferences_fileName = "mp_" + myCFID.ToString() + "_" + myAttributes.attribute1 + ".db3";
            string dbPath_mPreferences = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), mPreferences_fileName);
            var db_mPreferences = new SQLiteConnection(dbPath_mPreferences);
            if (isHMAll == true)
            {
                db_mPreferences.DeleteAll<SQLite_Tables.MajorPreferences>();
            }
            else
            {
                db_mPreferences.DeleteAll<SQLite_Tables.MajorPreferences>();
                if (numMPs >= 1)
                {
                    SQLite_Tables.MajorPreferences majorPrefs1 = new SQLite_Tables.MajorPreferences();
                    majorPrefs1.id = 1;
                    majorPrefs1.major = hmspinner1.SelectedItem.ToString();

                    db_mPreferences.InsertOrReplace(majorPrefs1);
                }
                if (numMPs >= 2)
                {                    
                    SQLite_Tables.MajorPreferences majorPrefs2 = new SQLite_Tables.MajorPreferences();
                    majorPrefs2.id = 2;
                    majorPrefs2.major = hmspinner2.SelectedItem.ToString();

                    db_mPreferences.InsertOrReplace(majorPrefs2);                    
                }
                if (numMPs >= 3)
                {
                    SQLite_Tables.MajorPreferences majorPrefs3 = new SQLite_Tables.MajorPreferences();
                    majorPrefs3.id = 3;
                    majorPrefs3.major = hmspinner3.SelectedItem.ToString();

                    db_mPreferences.InsertOrReplace(majorPrefs3);
                }
                if (numMPs >= 4)
                {
                    SQLite_Tables.MajorPreferences majorPrefs4 = new SQLite_Tables.MajorPreferences();
                    majorPrefs4.id = 4;
                    majorPrefs4.major = hmspinner4.SelectedItem.ToString();

                    db_mPreferences.InsertOrReplace(majorPrefs4);
                }
                if (numMPs >= 5)
                {
                    SQLite_Tables.MajorPreferences majorPrefs5 = new SQLite_Tables.MajorPreferences();
                    majorPrefs5.id = 5;
                    majorPrefs5.major = hmspinner5.SelectedItem.ToString();

                    db_mPreferences.InsertOrReplace(majorPrefs5);
                }
            }
            if (isHGTAll == true)
            {
                db_mPreferences.DeleteAll<SQLite_Tables.GradTermPreferences>();
            }
            else
            {
                db_mPreferences.DeleteAll<SQLite_Tables.GradTermPreferences>();
                if (numGTPs >= 1)
                {
                    SQLite_Tables.GradTermPreferences gradtermPrefs1 = new SQLite_Tables.GradTermPreferences();
                    gradtermPrefs1.id = 1;
                    gradtermPrefs1.gradterm = hgtspinner1.SelectedItem.ToString();

                    db_mPreferences.InsertOrReplace(gradtermPrefs1);
                }
                if (numGTPs >= 2)
                {
                    SQLite_Tables.GradTermPreferences gradtermPrefs2 = new SQLite_Tables.GradTermPreferences();
                    gradtermPrefs2.id = 2;
                    gradtermPrefs2.gradterm = hgtspinner2.SelectedItem.ToString();

                    db_mPreferences.InsertOrReplace(gradtermPrefs2);
                }
                if (numGTPs >= 3)
                {
                    SQLite_Tables.GradTermPreferences gradtermPrefs3 = new SQLite_Tables.GradTermPreferences();
                    gradtermPrefs3.id = 3;
                    gradtermPrefs3.gradterm = hgtspinner3.SelectedItem.ToString();

                    db_mPreferences.InsertOrReplace(gradtermPrefs3);
                }
            }
            if (isMinGPANone == true)
            {
                db_mPreferences.DeleteAll<SQLite_Tables.GPAPreferences>();
            }
            else
            {
                db_mPreferences.DeleteAll<SQLite_Tables.GPAPreferences>();
                if (numGPAs == 1)
                {
                    SQLite_Tables.GPAPreferences gpaPrefs = new SQLite_Tables.GPAPreferences();
                    gpaPrefs.id = 1;
                    gpaPrefs.gpa = minGPAspinner.SelectedItem.ToString();

                    db_mPreferences.InsertOrReplace(gpaPrefs);
                }
            }
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