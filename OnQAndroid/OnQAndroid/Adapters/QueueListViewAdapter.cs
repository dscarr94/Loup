using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;

namespace OnQAndroid
{
    class QueueListViewAdapter : BaseAdapter<string>
    {
        private List<string> mItems;
        private Context mContext;

        public QueueListViewAdapter(Context context, List<string> items)
        {
            mItems = items;
            mContext = context;
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

        public override string this[int position]
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
            candidateName.Text = mItems[position];

            return row;
        }
    }
}