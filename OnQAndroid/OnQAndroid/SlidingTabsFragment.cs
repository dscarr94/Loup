using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Support.V4.View;
using Java.Lang;

namespace OnQAndroid
{
    public class SlidingTabsFragment : Fragment
    {
        private SlidingTabScrollView mSlidingTabScrollView;
        private ViewPager mViewPager;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            return inflater.Inflate(Resource.Layout.fragment_sample, container, false);            
        }
        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            mSlidingTabScrollView = View.FindViewById<SlidingTabScrollView>(Resource.Id.sliding_tabs);
            mViewPager = view.FindViewById<ViewPager>(Resource.Id.viewpager);
            mViewPager.Adapter = new SamplePagerAdapter();
            mSlidingTabScrollView.ViewPager = mViewPager;
        }

        public class SamplePagerAdapter : PagerAdapter
        {
            List<string> items = new List<string>();

            public SamplePagerAdapter() : base()
            {
                items.Add("   Home   ");
                items.Add("   My Q's   ");
                items.Add("  Profile  ");
                items.Add(" Settings ");
            }
            public override int Count
            {
                get { return items.Count; }
            }
            public override bool IsViewFromObject(View view, Java.Lang.Object obj)
            {
                return view == obj;
            }

            public override Java.Lang.Object InstantiateItem(ViewGroup container, int position)
            {
                int pos = position + 1;
                View view;

                if (pos == 1)
                {
                    view = LayoutInflater.From(container.Context).Inflate(Resource.Layout.HomeTab, container, false);
                    container.AddView(view);
                    return view;
                }
                else if (pos == 2)
                {
                    view = LayoutInflater.From(container.Context).Inflate(Resource.Layout.MyQsTab, container, false);
                    container.AddView(view);
                    return view;
                }
                else if (pos == 3)
                {
                    view = LayoutInflater.From(container.Context).Inflate(Resource.Layout.ProfileTab, container, false);
                    container.AddView(view);
                    return view;
                }
                else if (pos == 4)
                {
                    view = LayoutInflater.From(container.Context).Inflate(Resource.Layout.SettingsTab, container, false);
                    container.AddView(view);
                    return view;
                }

                else
                {
                    throw new NotImplementedException();
                }


            }
            public string GetHeaderTitle(int position)
            {
                return items[position];
            }
            public override void DestroyItem(ViewGroup container, int position, Java.Lang.Object obj)
            {
                container.RemoveView((View)obj);
            }
        }
    }
}