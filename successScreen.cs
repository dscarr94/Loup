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
    [Activity(Label = "successScreen")]
    public class successScreen : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            // Create your application here
            SetContentView(Resource.Layout.SuccessScreen);

            Button advanceButton = FindViewById<Button>(Resource.Id.AdvanceButton);

            advanceButton.Click += AdvanceButton_Click;
        }

        private void AdvanceButton_Click(object sender, EventArgs e)
        {
            var advanceIntent = new Intent(this, typeof(homeScreen));
            StartActivity(advanceIntent);
        }
    }
}