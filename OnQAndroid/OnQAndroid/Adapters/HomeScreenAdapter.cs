using System;
using Android.App;
using Android.Views;
using Android.Widget;
using Android.Support.V4.App;
using com.refractored;

namespace OnQAndroid
{
    class HomeScreenAdapter : FragmentStatePagerAdapter, ICustomTabProvider
    {
        private readonly int[] _icons =
        {
            Resource.Drawable.companybuilding,
            Resource.Drawable.qblack,
            Resource.Drawable.register,
            Resource.Drawable.profile,
            Resource.Drawable.gear
        };

        public HomeScreenAdapter(Android.Support.V4.App.FragmentManager fm) : base(fm)  // empty public constructor, asks for fragment manager, inherits from base
        {
        }

        public override int Count
        {
            get
            {
                return 5; // 5 tabs
            }
        } // count of tabs to return

        public override Android.Support.V4.App.Fragment GetItem(int position)
        {
            int pos = position + 1; // position starts at 0, add 1 so tabs go from 1 to 4
            if (pos == 1)
                return CompaniesRootFragment.newInstance(); // create new instance of companiesrootfragment
            else if (pos == 2)
                return QsRootFragment.newInstance(); // create new instance of qsrootfragment
            else if (pos == 3)
                return RegisterRootFragment.newInstance(); // create new instance of registerrootfragment
            else if (pos == 4)
                return ProfileRootFragment.newInstance(); // create new instance of profilerootfragment
            else if (pos == 5)
                return SettingsRootFragment.newInstance();
            else
                throw new NotImplementedException(); // must return something
        } // fragments to return

        /*public override Java.Lang.ICharSequence GetPageTitleFormatted(int position) // names of tabs
        {
            int pos = position + 1;
            if (pos == 1)
                return new Java.Lang.String("My CF's");
            else if (pos == 2)
                return new Java.Lang.String("My Q's");
            else if (pos == 3)
                return new Java.Lang.String("Register");
            else if (pos == 4)
                return new Java.Lang.String("Profile");
            else
                throw new NotImplementedException();
        }*/

        public View GetCustomTabView(ViewGroup parent, int position) // assigns icons to tabs
        {
            var tabLayout = (LinearLayout)LayoutInflater.From(Application.Context).Inflate(Resource.Layout.tab_layout, parent, false);
            var tabImage = tabLayout.FindViewById<ImageView>(Resource.Id.tabImage);
            tabImage.SetImageResource(_icons[position]);
            return tabLayout;
        }

    }
}