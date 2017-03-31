using UnityEngine;
using System.Collections;

public abstract class CM : MonoBehaviour {
	public enum DestroyFlag{
		DESTROYLOGIN,DONTDESTORYLOGIN
	}
	static Hashtable staticControllerList = null;
	/// <summary>
	/// 根据类型获取场景中的控制器
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public static T GetController<T>()
	{
		return (T)(CM.GetController(CM.GetTypeS(typeof(T))));
	}
	public static void ClearControllerLogin() {
		//staticControllerList = new Hashtable();
		//System.GC.Collect();
		Hashtable dontDestroyHash = new Hashtable();
		foreach(string key in staticControllerList.Keys)
		{
			if((staticControllerList[key] as CM).destroyFlag == DestroyFlag.DONTDESTORYLOGIN)
			{
				dontDestroyHash.Add(key, staticControllerList[key]);
			}
		}
		staticControllerList = new Hashtable();
		foreach (string key in dontDestroyHash.Keys)
		{
			staticControllerList.Add(key, dontDestroyHash[key]);
		}
		System.GC.Collect();
	}
	public static string GetTypeS(System.Type type) {
		return type.ToString();
	}
	
	[System.Obsolete]
	public static object GetController(string key) {
		if (StaticControllerHash.ContainsKey(key))
		{
			return StaticControllerHash[key];
		}
		else
		{
			
			foreach(DictionaryEntry de in staticControllerList)
			{
				Debug.Log(de.Key);
			}
			return null;
		}
	}
	public static Hashtable StaticControllerHash {
		get {
			if (staticControllerList == null) {
				staticControllerList = new Hashtable();
			}
			return staticControllerList;
		}
	}
	
	protected void UnLoginController(string id = "",bool allowDestroyFlag = true)
	{
		string key = id;
		if (key == "")
		{
			key = (this).GetType().ToString();
			while (StaticControllerHash.ContainsKey(key))
			{
				key = "Sub_" + key;
			}
		}
		if (staticControllerList.ContainsKey(key))
		{
			if ((staticControllerList[key] as CM).destroyFlag == DestroyFlag.DONTDESTORYLOGIN && allowDestroyFlag)
			{
				
			}
			else
			{
				StaticControllerHash.Remove(StaticControllerHash[key]);
				Debug.Log("Controller: " + key + " has been remove in the controller hash,hash member count:" + staticControllerList.Count);
			}
		}
	}
	DestroyFlag df = DestroyFlag.DESTROYLOGIN;
	public DestroyFlag destroyFlag {
		get
		{
			return df;
		}
		set
		{
			df = value;
		}
	}
	void OnDestroy()
	{
		UnLoginController();
	}
	
	protected void LoginController(string id = "") {
		/*     if(staticControllerList == null)
        {
            staticControllerList = new Hashtable();
        }*/
		string key = id;
		if (key == "")
		{
			key = (this).GetType().ToString();
			while (StaticControllerHash.ContainsKey(key))
			{
				key = "Sub_"+key;
			}
		}
		Debug.Log("Controller: " + key + " has been save in the controller hash,hash member count:"+staticControllerList.Count);
		
		StaticControllerHash.Add(key, this);
	}
	/// <summary>
	/// 在这个方法中注册了一切静态控制器对象，先于Start调用，若想使用自定义key注册，在此方法的重写中删除base的调用，自行调用LoginController方法即可。
	/// </summary>
	public virtual void InitController() {
		LoginController();
	}
	void Awake () {
		//InitController();
	}
}