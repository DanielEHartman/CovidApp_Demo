package com.toptoche.searchablespinnerlibrary;


public class SearchableListDialog_SearchableItemCallback_1
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.toptoche.searchablespinnerlibrary.SearchableListDialog.SearchableItem,
		java.io.Serializable
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onSearchableItemClicked:(Ljava/lang/Object;I)V:GetOnSearchableItemClicked_Ljava_lang_Object_IHandler:Toptoche.SearchableSpinnerLibrary.SearchableListDialog/ISearchableItemInvoker, ToptocheSearchableSpinner\n" +
			"";
		mono.android.Runtime.register ("Toptoche.SearchableSpinnerLibrary.SearchableListDialog+SearchableItemCallback`1, ToptocheSearchableSpinner", SearchableListDialog_SearchableItemCallback_1.class, __md_methods);
	}


	public SearchableListDialog_SearchableItemCallback_1 ()
	{
		super ();
		if (getClass () == SearchableListDialog_SearchableItemCallback_1.class)
			mono.android.TypeManager.Activate ("Toptoche.SearchableSpinnerLibrary.SearchableListDialog+SearchableItemCallback`1, ToptocheSearchableSpinner", "", this, new java.lang.Object[] {  });
	}


	public void onSearchableItemClicked (java.lang.Object p0, int p1)
	{
		n_onSearchableItemClicked (p0, p1);
	}

	private native void n_onSearchableItemClicked (java.lang.Object p0, int p1);

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
