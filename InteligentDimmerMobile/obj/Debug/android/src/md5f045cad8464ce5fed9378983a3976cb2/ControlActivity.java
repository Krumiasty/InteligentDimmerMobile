package md5f045cad8464ce5fed9378983a3976cb2;


public class ControlActivity
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"n_onBackPressed:()V:GetOnBackPressedHandler\n" +
			"";
		mono.android.Runtime.register ("InteligentDimmerMobile.ControlActivity, InteligentDimmerMobile, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", ControlActivity.class, __md_methods);
	}


	public ControlActivity () throws java.lang.Throwable
	{
		super ();
		if (getClass () == ControlActivity.class)
			mono.android.TypeManager.Activate ("InteligentDimmerMobile.ControlActivity, InteligentDimmerMobile, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);


	public void onBackPressed ()
	{
		n_onBackPressed ();
	}

	private native void n_onBackPressed ();

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
