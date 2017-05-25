using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using SQLite;
using Android.Support.V4.App;

namespace OnQAndroid
{
    class PastQueuesListViewAdapter : BaseAdapter<int>
    {
        private List<int> mItems;
        private Context mContext;
        private string mSender;

        public PastQueuesListViewAdapter(Context context, List<int> items, string sender)
        {
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

            string dbPath_login = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "user.db3");
            SQLiteConnection db_login = new SQLiteConnection(dbPath_login);

            string dbPath_attributes = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "attributes.db3");
            SQLiteConnection db_attributes = new SQLiteConnection(dbPath_attributes);
            MyAttributes myAttributes = db_attributes.Get<MyAttributes>(1);

            string fileName_pastQs = "pastqs_" + myAttributes.attribute1 + ".db3";
            string dbPath_pastQs = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), fileName_pastQs);
            SQLiteConnection db_pastQs = new SQLiteConnection(dbPath_pastQs);

            SQLite_Tables.PastQueue thisQueue = db_pastQs.Query<SQLite_Tables.PastQueue>("SELECT * FROM PastQueue WHERE studentid = ?", mItems[position]).First();

            StudentTable thisStudent = db_login.Get<StudentTable>(mItems[position]);

            TextView candidateName = row.FindViewById<TextView>(Resource.Id.candidateName);
            ImageView favorite = row.FindViewById<ImageView>(Resource.Id.favorite);
            LinearLayout candidateRow = row.FindViewById<LinearLayout>(Resource.Id.candidateRow);

            candidateName.Text = thisStudent.name;

            if (thisQueue.rating == 1)
            {
                favorite.SetImageResource(Resource.Drawable.starfilled);
            }

            else if (thisQueue.rating == 2)
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