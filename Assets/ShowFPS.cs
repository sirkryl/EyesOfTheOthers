using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShowFPS : MonoBehaviour {
	public  float updateInterval = 0.5F;
	public Text guiText;	
	private float accum   = 0; // FPS accumulated over the interval
	private int   frames  = 0; // Frames drawn over the interval
	private float timeleft; // Left time for current interval
	
	void Start()
	{
		timeleft = updateInterval;  
	}
	
	void Update()
	{
		timeleft -= Time.deltaTime;
		accum += Time.timeScale/Time.deltaTime;
		++frames;
		
		// Interval ended - update GUI text and start new interval
		if( timeleft <= 0.0 )
		{
			// display two fractional digits (f2 format)
			float fps = accum/frames;
			string format = System.String.Format("{0:F2} FPS",fps);
			guiText.text = format;
			timeleft = updateInterval;
			accum = 0.0F;
			frames = 0;
		}
	}
}
