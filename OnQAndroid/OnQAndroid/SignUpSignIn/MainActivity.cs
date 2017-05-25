using System;
using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;
using System.Timers;
using SQLite;

namespace OnQAndroid
{
    [Activity(Label = "OnQ", MainLauncher = true, Icon = "@drawable/OnQLogo")]
    public class MainActivity : Activity
    {
        ProgressBar progressBar; // initialize progressBar variable
        Timer _timer; // initialize timer variable
        object _lock = new object();

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            // Get Advance Button controls from Main
            Button advanceButton = FindViewById<Button>(Resource.Id.advance);
            // Disable advance button until progress bar is full
            advanceButton.Enabled = false;
            // Set Background Color of disabled advance button
            advanceButton.SetBackgroundColor(Android.Graphics.Color.Rgb(239,239,239));
            // Advance button event
            advanceButton.Click += (object sender, EventArgs e) => // on advance button click
            {
                string dbPath_attributes = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "attributes.db3");
                //DeleteDatabase(dbPath_attributes); // simulate first use
                SQLiteConnection db_attributes = new SQLiteConnection(dbPath_attributes);
                db_attributes.CreateTable<MyAttributes>();

                try // required because on first use, myattributes will contain zero objects
                {
                    MyAttributes myAttributes = db_attributes.Get<MyAttributes>(1);

                    if (myAttributes.rememberme == true)
                    {
                        var advanceIntent = new Intent(this, typeof(homeScreen2)).PutExtra("UserId", myAttributes.loginid.ToString()); // remember me temporarily broken
                        StartActivity(advanceIntent);
                        Finish();
                    }

                    else if (myAttributes.rememberme == false)
                    {
                        var advanceIntent = new Intent(this, typeof(LoginSignup)); // create intent to go to login/signup screen
                        StartActivity(advanceIntent); // start the next activity
                        Finish(); // finish this activity so that you can't go back to loading screen
                    }
                }
                catch
                {
                    MyAttributes nullAttributes = new MyAttributes();
                    nullAttributes.id = 1;
                    nullAttributes.rememberme = false;
                    db_attributes.InsertOrReplace(nullAttributes);

                    var advanceIntent = new Intent(this, typeof(LoginSignup)); // create intent to go to login/signup screen
                    StartActivity(advanceIntent); // start the next activity
                    Finish(); // finish this activity so that you can't go back to loading screen
                }
            };


            // Progress Bar
            progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar1); // get progress bar from view
            progressBar.Max = 100; // maximum progress
            progressBar.Progress = 0; // initial progress

            _timer = new Timer(); // make a new timer
            _timer.Enabled = true; // enable the timer
            _timer.Interval = 20; // interval of timer is 20 ms
            _timer.Elapsed += OnTimeEvent; // whenever time elapses, execute event
        }

        private void OnTimeEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            RunOnUiThread(() =>
            {
                progressBar.IncrementProgressBy(1); // increment progress by 1 every 20 ms
                CheckProgress(progressBar.Progress); // call check progress method
            });
        }

        public void CheckProgress (int progress)
        {
            lock(_lock) // i don't know what this does
            {
                if (progress >= 100) // if progress reaches 100%
                {
                    Button advanceButton = FindViewById<Button>(Resource.Id.advance); // get controls for advance button
                    _timer.Dispose(); // dispose of the timer
                    advanceButton.Enabled = true; // enable the advance button
                    advanceButton.SetBackgroundResource(Resource.Drawable.turquoisebutton); // make background resource turquoise button
                }
                else // if progress is less than 100%
                {
                    Button advanceButton = FindViewById<Button>(Resource.Id.advance); // get controls for advance button
                    advanceButton.Enabled = false; // disable advance button
                }
            }
        }
    }
}

