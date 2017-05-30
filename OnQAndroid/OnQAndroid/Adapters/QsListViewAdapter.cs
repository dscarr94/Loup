using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using SQLite;
using Android.Support.V4.App;
using Firebase.Xamarin.Database;
using OnQAndroid.FirebaseObjects;
using Firebase.Xamarin.Database.Query;

namespace OnQAndroid
{
    class QsListViewAdapter : BaseAdapter<string>
    {
        private List<string> mItems;
        private List<bool> mFavs;
        private Context mContext;
        private string mSender;
        private const string FirebaseURL = "https://onqfirebase.firebaseio.com/";
        bool isFavorite;
        public string favoritesFileName;
        private List<int> mCompanyIds;
        private List<string> mTimes;
        private List<string> mPositions;

        public QsListViewAdapter(Context context, List<string> items, string sender, List<bool> favs, List<int> companyIds, List<string> times, List<string> positions)
        {
            mCompanyIds = companyIds;
            mItems = items;
            mContext = context;
            mSender = sender;
            mFavs = favs;
            mPositions = positions;
            mTimes = times;
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

            int companyId = mCompanyIds[position];

            favoritesFileName = "fav_" + myCFID.ToString() + "_" + myAttributes.typeid.ToString();

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
            TextView timeText = row.FindViewById<TextView>(Resource.Id.timeText);
            TextView positionText = row.FindViewById<TextView>(Resource.Id.positionText);

            q_ll.Enabled = false;

            if (mSender == "CurrentQs")
            {
                timeText.Text = mTimes[position];
                positionText.Text = mPositions[position];
            }
            else if (mSender == "PastQs")
            {
                timeText.Visibility = ViewStates.Invisible;
                positionText.Visibility = ViewStates.Invisible;
            }

            companyName.Text = mItems[position];
            string fileName = companyName.Text.ToLower().Replace(" ", "");
            int resourceId = (int)typeof(Resource.Drawable).GetField(fileName).GetValue(null);
            companyLogo.SetImageResource(resourceId);

            isFavorite = mFavs[position];

            if (isFavorite == true)
            {
                star.SetImageResource(Resource.Drawable.starfilled);
            }
            else if (isFavorite == false)
            {
                star.SetImageResource(Resource.Drawable.starunfilled);
            }

            favorite.Click += (sender, e) =>
            {
                bool thisFavorite = mFavs[position];
                if (thisFavorite == true)
                {
                    star.SetImageResource(Resource.Drawable.starunfilled);
                    bool newFavorite = false;
                    mFavs[position] = false;
                    UpdateIsFavorite(newFavorite, companyId);
                }
                else if (thisFavorite == false)
                {
                    star.SetImageResource(Resource.Drawable.starfilled);
                    bool newFavorite = true;
                    mFavs[position] = true;
                    UpdateIsFavorite(newFavorite, companyId);
                }
            };

            companyInfo.Click += (sender, e) =>
            {
                Android.Support.V4.App.FragmentTransaction trans = ((FragmentActivity)mContext).SupportFragmentManager.BeginTransaction();

                CompanyInfoFragment fragment = new CompanyInfoFragment();

                Bundle arguments = new Bundle();

                arguments.PutInt("CompanyInt", companyId);

                if (mSender == "CurrentQs")
                {
                    arguments.PutString("Sender", "CurrentQs");
                }
                else if (mSender == "PastQs")
                {
                    arguments.PutString("Sender", "PastQs");
                }
                fragment.Arguments = arguments;
                trans.Replace(Resource.Id.qs_root_frame, fragment);
                trans.Commit();
            };

            return row;
        }
        private async void UpdateIsFavorite(bool newIsFavorite, int companyid)
        {
            var firebase = new FirebaseClient(FirebaseURL);
            var allFavorites = await firebase.Child("favorites").Child(favoritesFileName).OnceAsync<Favorite>();
            string key = "";
            string name = "";

            foreach (var favorite in allFavorites)
            {
                if (favorite.Object.companyid == companyid.ToString())
                {
                    key = favorite.Key;
                    name = favorite.Object.name;
                }
            }

            Favorite updateFavorite = new Favorite();
            updateFavorite.companyid = companyid.ToString();
            updateFavorite.isFavorite = newIsFavorite;
            updateFavorite.name = name;

            await firebase.Child("favorites").Child(favoritesFileName).Child(key).PutAsync(updateFavorite);
        }
    }
}