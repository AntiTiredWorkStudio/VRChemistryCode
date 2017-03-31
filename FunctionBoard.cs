using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class UISpriteController {
	public List<EventDelegate> OnWndVisibleCallBacks;
	public List<EventDelegate> OnWndHideCallBacks;
	public UITweener[] ExceptTweeners;
	public Collider[] ExceptColliders;
    public Renderer[] ExceptRenderer;
    public List<UITweener> realTweeners;
	public List<Collider> realColliders;
    public List<Renderer> realRenderer;
    public UITweener mainTweeners;
	GameObject target;
	FunctionBoard targetFunction;
	public void CreateTC(GameObject _target) {
		target = _target;
		UITweener[] tweeners = _target.GetComponentsInChildren<UITweener>();
		Collider[] Colliders = _target.GetComponentsInChildren<Collider>();
        Renderer[] Renderers = _target.GetComponentsInChildren<Renderer>();
        targetFunction = _target.GetComponent<FunctionBoard>();
		realTweeners = new List<UITweener>();
		realColliders = new List<Collider>();
        realRenderer = new List<Renderer>();
        float defendDurnations = 0.0f;
		foreach(UITweener ut in tweeners){
			realTweeners.Add(ut);
			if (ut.duration > defendDurnations) {
				defendDurnations = ut.duration;
				mainTweeners = ut;
			}
		}
		if(mainTweeners != null)
			mainTweeners.AddOnFinished(WndVisible);
		foreach (Collider cs in Colliders)
		{
			realColliders.Add(cs);
        }
        foreach (Renderer re in Renderers)
        {
            realRenderer.Add(re);
        }
        if (ExceptTweeners!=null)
			foreach (UITweener ut in ExceptTweeners)
		{
			realTweeners.Remove(ut);
		}
		if (ExceptColliders != null)
			foreach (Collider cs in ExceptColliders)
		    {
			    realColliders.Remove(cs);
            }
        if (ExceptRenderer != null)
            foreach (Renderer re in ExceptRenderer)
            {
                realRenderer.Remove(re);
            }
    }
	void WndVisible()
	{
		if (!WndStatus)
		{
			return;
		}
		Debug.LogWarning("visible");
		LaunchColliders();
        LaunchRenderer();
		foreach (EventDelegate ed in OnWndVisibleCallBacks)
		{
			ed.target.SendMessage(ed.methodName,ed.parameters);
		}
	}
	void WndHide()
	{
		Debug.LogWarning("hide");
		UnLaunchColliders();
        UnLaunchRenderer();
		foreach (EventDelegate ed in OnWndHideCallBacks)
		{
			ed.target.SendMessage(ed.methodName, ed.parameters);
		}
	}
	void LaunchTweeners() { 
		foreach(UITweener ut in realTweeners){
			ut.PlayForward();
		}
		FunctionBoard.StepList.Push(targetFunction);
	}
	void UnLaunchTweeners()
	{
		foreach (UITweener ut in realTweeners)
		{
			ut.PlayReverse();
		}
		if (FunctionBoard.StepList.Count > 0)
		{
			FunctionBoard.StepList.Pop();
		}
	}
	public bool WndStatus = false;


    void LaunchRenderer()
    {
        foreach (Renderer cr in realRenderer)
        {
            if (cr == null)
            {
                continue;
            }
            cr.enabled = true;
        }
    }
    void UnLaunchRenderer()
    {
        foreach (Renderer cr in realRenderer)
        {
            if (cr == null)
            {
                continue;
            }
            cr.enabled = false;
        }
    }


    void LaunchColliders() { 
		foreach(Collider cr in realColliders){
			if(cr == null)
			{
				continue;
			}
			cr.enabled = true;
			if (cr.gameObject.GetComponent<UIButton>()) {
				cr.gameObject.GetComponent<UIButton>().state = UIButtonColor.State.Normal;
			}
		}
	}
	void UnLaunchColliders()
	{
		foreach (Collider cr in realColliders)
		{
			if(cr == null)
			{
				continue;
			}
			cr.enabled = false;
			if (cr.gameObject.GetComponent<UIButton>())
			{
				cr.gameObject.GetComponent<UIButton>().state = UIButtonColor.State.Normal;
			}
		}
	}
	void SetTweenerInitForward() {
		foreach (UITweener ut in realTweeners) {
			ut.tweenFactor = 1.0f;
			ut.enabled = true;
			ut.PlayForward();
		}
	}
	void SetTweenerInitReverse()
	{
		foreach (UITweener ut in realTweeners)
		{
			ut.tweenFactor = 0.0f;
			ut.enabled = true;
			ut.PlayReverse();
			ut.enabled = false;
		}
	}
	public void LaunchWnd() {
		WndStatus = true;
		LaunchTweeners();
	}
	public void SwitchWnd() {
		if (WndStatus)
		{
			WndStatus = false;
			UnLaunchTweeners();
			WndHide();
		}
		else
		{
			WndStatus = true;
			LaunchTweeners();
		}
	}
	public void UnLaunchWnd()
	{
		if(WndStatus)
			WndHide();
		WndStatus = false;
		UnLaunchTweeners();
	}
	public void SetAsUnLaunchStatus() {
		UnLaunchColliders();
        UnLaunchRenderer();
        SetTweenerInitReverse();
	}
	public void SetAsLaunchStatus()
	{
		LaunchColliders();
        LaunchRenderer();
        SetTweenerInitForward();
	}
}
public class FunctionBoard : CM {
	static Stack<FunctionBoard> stepList = null;
	public static Stack<FunctionBoard> StepList {
		get
		{
			if(stepList == null)
			{
				stepList = new Stack<FunctionBoard>();
			}
			return stepList;
		}
	}
	public UISpriteController WndCotnroller;
    public int DepthFix = 0;
	// Use this for initialization
	void Start () {
		WndCotnroller.CreateTC(gameObject);
		//  WndCotnroller.SetAsLaunchStatus();
		if (!WndCotnroller.WndStatus)
		{
			WndCotnroller.SetAsUnLaunchStatus();
		}
		InitWnd();
        /*if (DepthFix != 0)
        {
            StartCoroutine(FixDepth());
        }*/
	}

   /* IEnumerator FixDepth()
    {
        yield return new WaitForSeconds(1.0f);
        foreach (UISprite sprite in GetComponentsInChildren<UISprite>())
        {
            sprite.depth += DepthFix;
            sprite.MarkAsChanged();
        }
        yield return new WaitForSeconds(1.0f);
        foreach (UILabel lab in GetComponentsInChildren<UILabel>())
        {
            lab.depth += DepthFix;
            lab.MarkAsChanged();
        }
    }*/

	/// <summary>
	/// 初始化窗口
	/// </summary>
	public virtual void InitWnd(){
		
	}
	
	/// <summary>
	/// 没帧更新窗口
	/// </summary>
	public virtual void UpdateWnd()
	{
		
	}
	
	void OnDrawGizmosSelected() {
		WndCotnroller.CreateTC(gameObject);
	}
	
	public void OpenBoard() {
		WndCotnroller.LaunchWnd();
		StepList.Push(this);
	}
	
	public void SwitchBoard()
	{
		WndCotnroller.SwitchWnd();
	}
	
	public void CloseBoard()
	{
		WndCotnroller.UnLaunchWnd();
	}
	
	void Update () {
		UpdateWnd();
	}
}