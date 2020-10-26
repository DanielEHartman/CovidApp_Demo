package crc64dddfdd346754f7e7;


public class MyDialogInterfaceOnClickListener
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		android.content.DialogInterface.OnClickListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onClick:(Landroid/content/DialogInterface;I)V:GetOnClick_Landroid_content_DialogInterface_IHandler:Android.Content.IDialogInterfaceOnClickListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"";
		mono.android.Runtime.register ("Toptoche.SearchableSpinnerLibrary.MyDialogInterfaceOnClickListener, ToptocheSearchableSpinner", MyDialogInterfaceOnClickListener.class, __md_methods);
	}


	public MyDialogInterfaceOnClickListener ()
	{
		super ();
		if (getClass () == MyDialogInterfaceOnClickListener.class)
			mono.android.TypeManager.Activate ("Toptoche.SearchableSpinnerLibrary.MyDialogInterfaceOnClickListener, ToptocheSearchableSpinner", "", this, new java.lang.Object[] {  });
	}


	public void onClick (android.content.DialogInterface p0, int p1)
	{
		n_onClick (p0, p1);
	}

	private native void n_onClick (android.content.DialogInterface p0, int p1);

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
