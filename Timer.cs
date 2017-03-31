using UnityEngine;
using System.Collections;

public class Timer : MonoBehaviour {
    public static Timer lastCircleTimer = null;
    public static Timer lastFinishedTimer = null;
	public static Timer addTimer(GameObject target,float _dur,func callback,int _repeatTimes = 1,funcWithID eachCallBack = null,float minDur = 0.08f ,float maxDur = 10000.0f){
        Timer newTimer = target.AddComponent<Timer>();
		newTimer.durnation = Mathf.Clamp(_dur,minDur,maxDur);
		newTimer._callback = callback;
		newTimer._times = _repeatTimes;
		newTimer._eachCallback = eachCallBack;
		newTimer.run = false;
		return newTimer;
	}
    string timerID = "";
    public string TimerID{
        get {
            return timerID;
        }
        set {
            timerID = value;
        }
    }
	public delegate void func();

	public delegate void funcWithID(int id);

	public float durnation = 10.0f;

	public int _times = 1;

	private bool run = false;

	public float timer = 0.0f;
    /// <summary>
    ///记录该计时器工作的总时长
    /// </summary>
    public float totalTimeCount = 0.0f;

	private int timeCount = 0;

	private func _callback;

	private funcWithID _eachCallback;

	private bool isPause = false;

	public void startTimer(){
		run = true;
	}
	// Update is called once per frame
	void Update () {
		if(!run){
			return;
		}
      //  Debug.Log("Timer Running");
		TimerLoop();
	}
	void TimerLoop(){
		if(timer<durnation){
			if(!isPause){
				timer+=Time.unscaledDeltaTime;
                totalTimeCount = Mathf.Clamp(totalTimeCount + Time.unscaledDeltaTime, 0.0f,18275.0f) ;
			}
		}else{
			//计时器最关键位置的修改！！！！
			if(timeCount<_times || _times==0){
                if (_eachCallback != null)
                {
                    Timer.lastCircleTimer = this;
					_eachCallback(timeCount);
				}
				timer = 0.0f;
				timeCount++;
				//*******新加的行********
				if(_times!=0 && timeCount>=_times){
					endTimer();
                    lastFinishedTimer = this;
				}
				//*********************
			}//else{
			//	endTimer();
			//}
		}
	}
	public void pauseTimer(){
		if(!isPause){
			isPause = true;
		}
	}
	public void continueTimer(){
		if(isPause){
			isPause = false;
		}
	}
	public void endTimer(){
		run = false;
        if(_callback!=null)
    		_callback();
		Destroy(this);
	}
	public float getCurrentTime(){
		return timer;
	}
	public int getCurrentRunTime(){
		return timeCount;
	}

    void OnEnable()
    {
        if (!isPause)
        {
            startTimer();
        }
    }

    void OnDisable()
    {
        pauseTimer();
    }
}
