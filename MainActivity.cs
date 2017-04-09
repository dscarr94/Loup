using System;
using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;
using System.Timers;

namespace OnQAndroid
{
    [Activity(Label = "OnQ", MainLauncher = true, Icon = "@drawable/OnQLogo")]
    public class MainActivity : Activity
    {
        ProgressBar progressBar;
        Timer _timer;
        object _lock = new object();

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            Button advanceButton = FindViewById<Button>(Resource.Id.advance);
            advanceButton.Enabled = false;
            advanceButton.Click += (object sender, EventArgs e) =>
            {
                var advanceIntent = new Intent(this, typeof(LoginSignup));
                StartActivity(advanceIntent);
            };

            // Progress Bar
            progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar1);
            progressBar.Max = 100;
            progressBar.Progress = 0;

            _timer = new Timer();
            _timer.Enabled = true;
            _timer.Interval = 20;
            _timer.Elapsed += OnTimeEvent;
        }

        private void OnTimeEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            RunOnUiThread(() =>
            {
                progressBar.IncrementProgressBy(1);
                CheckProgress(progressBar.Progress);
            });
        }

        public void CheckProgress (int progress)
        {
            lock(_lock)
            {
                if (progress >= 100)
                {
                    Button advanceButton = FindViewById<Button>(Resource.Id.advance);
                    _timer.Dispose();
                    advanceButton.Enabled = true;
                }
                else
                {
                    Button advanceButton = FindViewById<Button>(Resource.Id.advance);
                    advanceButton.Enabled = false;
                }

            }
        } 
    }
}

