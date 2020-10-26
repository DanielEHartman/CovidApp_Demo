package com.toptoche.searchablespinnerlibrary;


public class SearchableListDialog_SearchTextChangedCallback
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.toptoche.searchablespinnerlibrary.SearchableListDialog.OnSearchTextChanged
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onSearchTextChanged:(Ljava/lang/String;)V:GetOnSearchTextChanged_Ljava_lang_String_Handler:Toptoche.SearchableSpinnerLibrary.SearchableListDialog/IOnSearchTextChangedInvoker, ToptocheSearchableSpinner\n" +
			"";
		mono.android.Runtime.register ("Toptoche.SearchableSpinnerLibrary.SearchableListDialog+SearchTextChangedCallback, ToptocheSearchableSpinner", SearchableListDialog_SearchTextChangedCallback.class, __md_methods);
	}


	public SearchableListDialog_SearchTextChangedCallback ()
	{
		super ();
		if (getClass () == SearchableListDialog_SearchTextChangedCallback.class)
			mono.android.TypeManager.Activate ("Toptoche.SearchableSpinnerLibrary.SearchableListDialog+SearchTextChangedCallback, ToptocheSearchableSpinner", "", this, new java.lang.Object[] {  });
	}


	public void onSearchTextChanged (java.lang.String p0)
	{
		n_onSearchTextChanged (p0);
	}

	private native void n_onSearchTextChanged (java.lang.String p0);

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
