using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace OnQAndroid
{
    [Activity(Label = "uploadResume")]
    public class uploadResume : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.UploadResume);

            // Call Buttons
            Button selectButton = FindViewById<Button>(Resource.Id.SelectButton);
            Button uploadButton = FindViewById<Button>(Resource.Id.UploadButton);
            Button skipButton = FindViewById<Button>(Resource.Id.SkipButton);
            uploadButton.Enabled = false;

            // On Button Clicks
            selectButton.Click += SelectButton_Click;
            uploadButton.Click += UploadButton_Click;
            skipButton.Click += SkipButton_Click;           
        }

        private void SkipButton_Click(object sender, EventArgs e)
        {
            var advanceIntent = new Intent(this, typeof(successScreen));
            StartActivity(advanceIntent);
        }

        private void UploadButton_Click(object sender, EventArgs e)
        {
            // Code to upload selected file to server

            // Advance
            var advanceIntent = new Intent(this, typeof(successScreen));
            StartActivity(advanceIntent);
        }

        private void SelectButton_Click(object sender, EventArgs e)
        {
            // Code to select file from device

            // Enable upload button
            Button uploadButton = FindViewById<Button>(Resource.Id.UploadButton);
            uploadButton.Enabled = true;
        }
    }
}