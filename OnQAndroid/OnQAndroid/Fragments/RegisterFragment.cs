using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Views.InputMethods;
using SQLite;
using Android.Support.V4.View;
using Firebase.Xamarin.Database;

namespace OnQAndroid
{
    public class RegisterFragment : Android.Support.V4.App.Fragment
    {
        public RegisterFragment()
        {
            // Required empty public constructor
        }

        public static RegisterFragment NewInstance()
        {
            RegisterFragment fragment = new OnQAndroid.RegisterFragment();
            return fragment;
        }

        InputMethodManager imm;
        EditText cfID;
        ViewGroup mContainer;
        ProgressBar progressBar;
        private const string FirebaseURL = "https://onqfirebase.firebaseio.com/";

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            mContainer = container;
            View view = inflater.Inflate(Resource.Layout.RegisterTab, container, false);
            progressBar = view.FindViewById<ProgressBar>(Resource.Id.circularProgress);
            cfID = view.FindViewById<EditText>(Resource.Id.cfID);
            Button registerButton = view.FindViewById<Button>(Resource.Id.registerButton);
            ImageView infoButton = view.FindViewById<ImageView>(Resource.Id.questionMark);
            ViewPager viewPager = this.Activity.FindViewById<ViewPager>(Resource.Id.viewpager);

            registerButton.Enabled = false;

            registerButton.SetBackgroundColor(Android.Graphics.Color.Rgb(239, 239, 239));
            cfID.TextChanged += (sender, e) =>
            {
                string CFID = cfID.Text;
                if (string.IsNullOrWhiteSpace(CFID))
                {
                    registerButton.Enabled = false;
                    registerButton.SetBackgroundColor(Android.Graphics.Color.Rgb(239, 239, 239));
                }
                else
                {
                    registerButton.Enabled = true;
                    registerButton.SetBackgroundResource(Resource.Drawable.turquoisebutton);
                }
            };

            imm = (InputMethodManager)this.Activity.GetSystemService(Context.InputMethodService);

            viewPager.PageSelected += (sender, e) =>
            {
                imm.HideSoftInputFromInputMethod(cfID.WindowToken, 0);
            };
            /*viewPager.LayoutChange += (sender, e) =>
            {
                imm.HideSoftInputFromInputMethod(cfID.WindowToken, 0);
            };*/

            infoButton.Click += InfoButton_Click;
            registerButton.Click += RegisterButton_Click;
            registerButton.Click += (sender, e) =>
            {
                
            };
            return view;
        }

        private async void RegisterButton_Click(object sender, EventArgs e)
        {
            progressBar.Visibility = ViewStates.Visible;
            imm.HideSoftInputFromWindow(cfID.WindowToken, 0);

            string dbPath_attributes = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "attributes.db3");
            var db_attributes = new SQLiteConnection(dbPath_attributes);

            bool cfExists = false;

            var firebase = new FirebaseClient(FirebaseURL);
            var cfItems = await firebase.Child("cfids").OnceAsync<Cfids>();
            string cfName = "";

            foreach (var item in cfItems)
            {
                string cfid = item.Object.cfid;
                if (cfID.Text == cfid)
                {
                    cfExists = true;
                    cfName = item.Object.name;
                    break;
                }
            }

            int myCFID = db_attributes.Get<MyAttributes>(1).cfid;

            if (myCFID.ToString() == cfID.Text)
            {
                Toast.MakeText(this.Activity, "You Are Already Registered for This Career Fair", ToastLength.Short).Show();
                progressBar.Visibility = ViewStates.Invisible;
            }

            else
            {
                if (cfExists == true)
                {
                    progressBar.Visibility = ViewStates.Invisible;
                    Toast.MakeText(Activity, "Career Fair Found!", ToastLength.Short).Show();                    

                    confirmCF fragment = new confirmCF();
                    Bundle arguments = new Bundle();
                    arguments.PutString("CFID", cfID.Text);
                    arguments.PutString("cfName", cfName);
                    fragment.Arguments = arguments;

                    Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();
                    trans.Replace(Resource.Id.register_root_frame, fragment);
                    trans.Commit();
                }

                else
                {
                    progressBar.Visibility = ViewStates.Invisible;
                    Toast.MakeText(Activity, "No Career Fair Found :(", ToastLength.Short).Show();
                }
            }
        }

        private void InfoButton_Click(object sender, EventArgs e)
        {
            Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();
            trans.Replace(Resource.Id.register_root_frame, new CFIDinfo());
            //trans.AddToBackStack(null);
            trans.Commit();
        }
    }
}