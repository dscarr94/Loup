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

        public FavoriteCompaniesListViewAdapter(Context context, List<int> items)
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

            string dbPath_login = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "user.db3");
            var db_login = new SQLiteConnection(dbPath_login);

            string dbPath_companies = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), myCFID.ToString() + ".db3");
            var db_companies = new SQLiteConnection(dbPath_companies);

            var loginQueryResults = db_login.Query<LoginTable>("SELECT * FROM LoginTable WHERE email = ?", myAttributes.email);
            LoginTable myLogInInfo = loginQueryResults.First();

            //string favoritesFileName = "fav_" + myCFID.ToString() + "_" + myLogInInfo.id.ToString() + ".db3";
            //string dbPath_favorites = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), favoritesFileName);
            //var db_favorites = new SQLiteConnection(dbPath_favorites);

            if (row == null)
            {
                row = LayoutInflater.From(mContext).Inflate(Resource.Layout.favoritecompanieslistview_row, null, false);
            }

            TextView companyName = row.FindViewById<TextView>(Resource.Id.companyName);
            ImageView companyLogo = row.FindViewById<ImageView>(Resource.Id.companyLogo);
            LinearLayout info = row.FindViewById<LinearLayout>(Resource.Id.ll_info);

            companyName.Text = db_companies.Get<Companies>(mItems[position]).name;
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