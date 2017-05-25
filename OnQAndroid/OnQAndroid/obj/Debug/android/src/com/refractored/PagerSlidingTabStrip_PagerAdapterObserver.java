package com.refractored;


public class PagerSlidingTabStrip_PagerAdapterObserver
	extends android.database.DataSetObserver
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onChanged:()V:GetOnChangedHandler\n" +
			"";
		mono.android.Runtime.register ("com.refractored.PagerSlidingTabStrip+PagerAdapterObserver, Refractored.PagerSlidingTabStrip, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", PagerSlidingTabStrip_PagerAdapterObserver.class, __md_methods);
	}


	public PagerSlidingTabStrip_PagerAdapterObserver () throws java.lang.Throwable
	{
		super ();
		if (getClass () == PagerSlidingTabStrip_PagerAdapterObserver.class)
			mono.android.TypeManager.Activate ("com.refractored.PagerSlidingTabStrip+PagerAdapterObserver, Refractored.PagerSlidingTabStrip, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

	public PagerSlidingTabStrip_PagerAdapterObserver (com.refractored.PagerSlidingTabStrip p0) throws java.lang.Throwable
	{
		super ();
		if (getClass () == PagerSlidingTabStrip_PagerAdapterObserver.class)
			mono.android.TypeManager.Activate ("com.refractored.PagerSlidingTabStrip+PagerAdapterObserver, Refractored.PagerSlidingTabStrip, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", "com.refractored.PagerSlidingTabStrip, Refractored.PagerSlidingTabStrip, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", this, new java.lang.Object[] { p0 });
	}


	public void onChanged ()
	{
		n_onChanged ();
	}

	private native void n_onChanged ();

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
