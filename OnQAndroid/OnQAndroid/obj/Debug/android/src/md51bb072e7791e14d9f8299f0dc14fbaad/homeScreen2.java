package md51bb072e7791e14d9f8299f0dc14fbaad;


public class homeScreen2
	extends android.support.v4.app.FragmentActivity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("OnQAndroid.homeScreen2, OnQAndroid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", homeScreen2.class, __md_methods);
	}


	public homeScreen2 () throws java.lang.Throwable
	{
		super ();
		if (getClass () == homeScreen2.class)
			mono.android.TypeManager.Activate ("OnQAndroid.homeScreen2, OnQAndroid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);

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
