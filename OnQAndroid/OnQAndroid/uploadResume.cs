using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

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

        private /*async*/ void UploadButton_Click(object sender, EventArgs e)
        {
            // Code to upload selected file to server
            /*CognitoAWSCredentials credentials = new CognitoAWSCredentials("us-west-2:1c6a55f3-cae3-4590-b1f4-62f8cf269921", RegionEndpoint.USWest2);
            var s3Client = new AmazonS3Client(credentials);
            var transferUtility = new TransferUtility(s3Client);

            try
            {
                var response = await s3Client.PutObjectAsync(new PutObjectRequest()
                {
                    BucketName = "onqbucket1".ToLowerInvariant(),
                    FilePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "SYK Resume 1.5.17.pdf"),
                    Key = "Josh's Resume"
                });

                Toast.MakeText(this, "File uploaded to S3 Bucket", ToastLength.Long).Show();
            }

            catch (AmazonS3Exception s3Exception)
            {
                Toast.MakeText(this, "Upload failed, check logs for more information", ToastLength.Long).Show();
                System.Console.WriteLine(s3Exception.StackTrace);
            }*/

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