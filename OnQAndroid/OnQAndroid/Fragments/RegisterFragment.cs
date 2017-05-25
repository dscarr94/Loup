using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Views.InputMethods;
using SQLite;
using Android.Support.V4.View;

namespace OnQAndroid
{
    public class RegisterFragment : Android.Support.V4.App.Fragment
    {
        public RegisterFragment()
        {
            // Required empty public constructor
        }

        public static RegisterFragment newInstance()
        {
            RegisterFragment fragment = new OnQAndroid.RegisterFragment();
            return fragment;
        }
        InputMethodManager imm;
        EditText cfID;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.RegisterTab, container, false);
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


            infoButton.Click += InfoButton_Click;
            registerButton.Click += (sender, e) =>
            {
                imm.HideSoftInputFromWindow(cfID.WindowToken, 0);

                string dbPath_cfids = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "CFIDs.db3"); // Call Database
                var db_cfids = new SQLiteConnection(dbPath_cfids);

                string dbPath_user = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "user.db3");
                var db_user = new SQLiteConnection(dbPath_user);

                string dbPath_attributes = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "attributes.db3");
                var db_attributes = new SQLiteConnection(dbPath_attributes);

                var queryResults = db_cfids.Query<Cfids>("SELECT * FROM Cfids WHERE cfid = ?", cfID.Text);
                int myCFID = db_attributes.Get<MyAttributes>(1).cfid;

                if (myCFID.ToString() == cfID.Text)
                {
                    Toast.MakeText(this.Activity, "You Are Already Registered for This Career Fair", ToastLength.Short).Show();
                }

                else
                {
                    if (queryResults.Count != 0)
                    {
                        Toast.MakeText(container.Context, "Career Fair Found!", ToastLength.Short).Show();

                        confirmCF fragment = new confirmCF();
                        Bundle arguments = new Bundle();
                        arguments.PutString("CFID", cfID.Text);
                        fragment.Arguments = arguments;

                        Android.Support.V4.App.FragmentTransaction trans = FragmentManager.BeginTransaction();
                        trans.Replace(Resource.Id.register_root_frame, fragment);
                        trans.Commit();
                    }
                    
                    else
                    {
                        Toast.MakeText(container.Context, "No Career Fair Found :(", ToastLength.Short).Show();
                    }
                }
            };
            return view;
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