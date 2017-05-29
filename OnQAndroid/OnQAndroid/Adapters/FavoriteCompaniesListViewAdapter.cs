using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V4.App;
using SQLite;

namespace OnQAndroid
{
    class FavoriteCompaniesListViewAdapter : BaseAdapter<int>
    {
        private List<int> mItems;
        private Context mContext;
        private List<string> mCompanies;

        public FavoriteCompaniesListViewAdapter(Context context, List<int> items, List<string> companies)
        {
            mItems = items;
            mContext = context;
            mCompanies = companies;
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

            string dbPath_attributes = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "attributes.db3");
            var db_attributes = new SQLiteConnection(dbPath_attributes);

            var myAttributes = db_attributes.Get<MyAttributes>(1);
            int myCFID = myAttributes.cfid;

            if (row == null)
            {
                row = LayoutInflater.From(mContext).Inflate(Resource.Layout.favoritecompanieslistview_row, null, false);
            }

            TextView companyName = row.FindViewById<TextView>(Resource.Id.companyName);
            ImageView companyLogo = row.FindViewById<ImageView>(Resource.Id.companyLogo);
            LinearLayout info = row.FindViewById<LinearLayout>(Resource.Id.ll_info);

            companyName.Text = mCompanies[position];
            string fileName = companyName.Text.ToLower().Replace(" ", "");
            int resourceId = (int)typeof(Resource.Drawable).GetField(fileName).GetValue(null);
            companyLogo.SetImageResource(resourceId);            

            info.Click += (sender, e) =>
            {
                Android.Support.V4.App.FragmentTransaction trans = ((FragmentActivity)mContext).SupportFragmentManager.BeginTransaction();

                CompanyInfoFragment fragment = new CompanyInfoFragment();

                Bundle arguments = new Bundle();

                arguments.PutInt("CompanyInt", mItems[position]);

                arguments.PutString("Sender", "Profile");

                fragment.Arguments = arguments;

                trans.Replace(Resource.Id.profile_root_frame, fragment);

                //trans.AddToBackStack(null);

                trans.Commit();
            };

            return row;
        }
    }
}