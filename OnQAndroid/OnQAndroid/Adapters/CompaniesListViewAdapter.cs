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
    class CompaniesListViewAdapter : BaseAdapter<string>
    {
        private List<string> mItems;
        private Context mContext;

        public CompaniesListViewAdapter(Context context, List<string> items)
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

            string dbPath_attributes = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "attributes.db3");
            var db_attributes = new SQLiteConnection(dbPath_attributes);

            var myAttributes = db_attributes.Get<MyAttributes>(1);
            int myCFID = myAttributes.cfid;

            string dbPath_login = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "user.db3");
            var db_login = new SQLiteConnection(dbPath_login);
            
            var loginQueryResults = db_login.Query<LoginTable>("SELECT * FROM LoginTable WHERE email = ?", myAttributes.email);
            LoginTable myLogInInfo = loginQueryResults.First();

            string favoritesFileName = "fav_" + myCFID.ToString() + "_" + myLogInInfo.id.ToString() + ".db3";
            string dbPath_favorites = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), favoritesFileName);
            var db_favorites = new SQLiteConnection(dbPath_favorites);

            //string fileName_companies = myCFID.ToString() + ".db3";
            //string dbPath_companies = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), fileName_companies);
            //var db_companies = new SQLiteConnection(dbPath_companies);

            //int companyId = db_companies.Query<Companies>("SELECT * FROM Companies WHERE id = ?", position + 1).First().id;

            if (row == null)
            {
                row = LayoutInflater.From(mContext).Inflate(Resource.Layout.companieslistview_row, null, false);
            }

            TextView companyName = row.FindViewById<TextView>(Resource.Id.companyName);
            ImageView companyLogo = row.FindViewById<ImageView>(Resource.Id.companyLogo);
            LinearLayout companyInfo = row.FindViewById<LinearLayout>(Resource.Id.companyInfo);
            LinearLayout favorite = row.FindViewById<LinearLayout>(Resource.Id.favorite);
            LinearLayout q_ll = row.FindViewById<LinearLayout>(Resource.Id.q_ll);
            ImageView star = row.FindViewById<ImageView>(Resource.Id.star);

            companyName.Text = mItems[position];
            string fileName = companyName.Text.ToLower().Replace(" ", "");
            int resourceId = (int)typeof(Resource.Drawable).GetField(fileName).GetValue(null);
            companyLogo.SetImageResource(resourceId);

            //int numRows = db_favorites.Table<SQLite_Tables.MyFavorites>().Count();
            bool isFavorite = db_favorites.Get<SQLite_Tables.MyFavorites>(position + 1).isFavorite;
            if (isFavorite == true)
            {
                star.SetImageResource(Resource.Drawable.starfilled);
            }
            else if (isFavorite == false)
            {
                star.SetImageResource(Resource.Drawable.starunfilled);
            }

            q_ll.Click += (sender, e) =>
            {
                Android.Support.V4.App.FragmentTransaction trans = ((FragmentActivity)mContext).SupportFragmentManager.BeginTransaction();
                Fragments.confirmQ fragment = new Fragments.confirmQ();
                Bundle arguments = new Bundle();
                arguments.PutInt("CompanyInt", position + 1);
                fragment.Arguments = arguments;
                trans.Replace(Resource.Id.companies_root_frame, fragment);
                trans.Commit();
            };

            favorite.Click += (sender, e) =>
            {
                //bool isFavorite = db_favorites.Get<SQLite_Tables.MyFavorites>(position + 1).isFavorite;
                if (isFavorite == true)
                {
                    SQLite_Tables.MyFavorites newIsFavorite = new SQLite_Tables.MyFavorites();
                    newIsFavorite.id = position+1;
                    newIsFavorite.isFavorite = false;
                    star.SetImageResource(Resource.Drawable.starunfilled);
                    isFavorite = false;
                    db_favorites.Update(newIsFavorite);
                }
                else if (isFavorite == false)
                {
                    SQLite_Tables.MyFavorites newIsFavorite = new SQLite_Tables.MyFavorites();
                    newIsFavorite.id = position+1;
                    newIsFavorite.isFavorite = true;
                    star.SetImageResource(Resource.Drawable.starfilled);
                    isFavorite = true;
                    db_favorites.Update(newIsFavorite);
                }
            };

            companyInfo.Click += (sender, e) =>
            {
                Android.Support.V4.App.FragmentTransaction trans = ((FragmentActivity)mContext).SupportFragmentManager.BeginTransaction();

                CompanyInfoFragment fragment = new CompanyInfoFragment();

                Bundle arguments = new Bundle();

                arguments.PutInt("CompanyInt", position + 1);

                arguments.PutString("Sender", "Companies");
                fragment.Arguments = arguments;
                trans.Replace(Resource.Id.companies_root_frame, fragment);

                //trans.AddToBackStack(null);

                trans.Commit();
            };

            return row;
        }
    }
}