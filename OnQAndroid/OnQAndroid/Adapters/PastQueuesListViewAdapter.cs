using System.Collections.Generic;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V4.App;

namespace OnQAndroid
{
    class PastQueuesListViewAdapter : BaseAdapter<int>
    {
        private List<int> mItems;
        private Context mContext;
        private string mSender;
        private List<string> mNames;
        private List<string> mRatings;

        public PastQueuesListViewAdapter(Context context, List<int> items, string sender, List<string> names, List<string> ratings)
        {
            mRatings = ratings;
            mNames = names;
            mItems = items;
            mContext = context;
            mSender = sender;
        }

        public override int Count
        {
            get
            {
                return mItems.Count;
            }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override int this[int position]
        {
            get
            {
                return mItems[position];
            }
        }

        View row;

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            row = convertView;

            if (row == null)
            {
                row = LayoutInflater.From(mContext).Inflate(Resource.Layout.Qrow, null, false);
            }

            TextView candidateName = row.FindViewById<TextView>(Resource.Id.candidateName);
            ImageView favorite = row.FindViewById<ImageView>(Resource.Id.favorite);
            LinearLayout candidateRow = row.FindViewById<LinearLayout>(Resource.Id.candidateRow);

            candidateName.Text = mNames[position];

            if (mRatings[position] == "1")
            {
                favorite.SetImageResource(Resource.Drawable.starfilled);
            }

            else if (mRatings[position] == "2")
            {
                favorite.SetImageResource(Resource.Drawable.heartfilled);
            }

            candidateRow.Click += (sender, e) =>
            {
                Android.Support.V4.App.FragmentTransaction trans = ((FragmentActivity)mContext).SupportFragmentManager.BeginTransaction();
                Bundle arguments = new Bundle();
                arguments.PutInt("StudentId", mItems[position]);

                arguments.PutString("Sender", mSender);

                Fragments.PastQStudentProfileView fragment = new Fragments.PastQStudentProfileView();
                fragment.Arguments = arguments;

                if (mSender == "Profile")
                {
                    trans.Replace(Resource.Id.profile_root_frame, fragment);
                }
                else if (mSender == "PastQs")
                {
                    trans.Replace(Resource.Id.qs_root_frame, fragment);
                }
                trans.Commit();
            };
            return row;
        }
    }
}