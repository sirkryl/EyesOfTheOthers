﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public enum DayPhase { Night, Day }
public class DayNightCycleManager : MonoBehaviour {

	public static DayNightCycleManager SharedInstance { get; private set; }
	public event OnStateChangeHandler OnDayTimeChange;

	public DayPhase dayPhase { get; private set; }

	public Material darkSkyBoxMaterial;
	public Material brightSkyBoxMaterial;
	public Color nightColor;
	public Color duskColor;
	public Color morningColor;
	public Color noonColor;

	public Color nightAmbientLight;
	public Color duskAmbientLight;
	public Color morningAmbientLight;
	public Color noonAmbientLight;

	public Color nightFog;
	public Color duskFog;
	public Color morningFog;
	public Color noonFog;

	public Flare sunFlare;
	public Flare sunAtNoonFlare;
	public GameObject sunFlareObject;
	public Color sunAtNightColor;
	public Color sunAtDayColor;

	public Transform rotationCenter;
	public Light sun;
	public Light moon;
	public Text timeGui;
	public Slider timeSlider;
	public bool enabled = false;

	public float cycleValue;
	public float timeOfDay;
	public int speed = 30;
	public float sunCycle = 0;

	private bool moonOut = false;
	private float helperValue;

	void Awake () {
		if(SharedInstance != null && SharedInstance != this)
			Destroy(gameObject);
		
		SharedInstance = this;
		DontDestroyOnLoad(gameObject);
	}

	// Use this for initialization
	void Start () {

		cycleValue = 0.4f;
		GlobalVariableManager.SharedInstance.SetGlobalVariable("timeOfDay", (int)Mathf.Round (cycleValue*24));
	}


	// Update is called once per frame
	void Update () {
		if (enabled)
		{
			if (cycleValue >= 1.0)
				cycleValue = 0;

			helperValue = sunCycle*24;
			timeOfDay = cycleValue*24;
			GlobalVariableManager.SharedInstance.SetGlobalVariable("timeOfDay", (int)Mathf.Round (timeOfDay));
			int hours = (int)timeOfDay;
			int minutes = (int)((timeOfDay*60)%60);

			string timeString = "Time: " + hours.ToString("00") + ":" + minutes.ToString ("00");

			timeGui.text = timeString;

			rotationCenter.transform.localEulerAngles = new Vector3((cycleValue*360)-90, 0, 0);
			cycleValue = cycleValue + Time.deltaTime/speed;
			timeSlider.value = cycleValue;
			sun.color = Color.Lerp (sunAtNightColor, sunAtDayColor, cycleValue*2.0f);

			if (cycleValue < 0.5)
				sunCycle = cycleValue;
			else
				sunCycle = (1-cycleValue);

			sun.intensity = (sunCycle - 0.2f) * 1.7f;
			if (moonOut)
				moon.intensity = (1-sunCycle*2)*0.1f;

			if(helperValue < 4) //timeOfDay >= 20 && timeOfDay < 4) //helperValue < 4)
			{
				moonOut = true;
				//Debug.Log ("Night");
				if (dayPhase == DayPhase.Day)
					SetDayPhase(DayPhase.Night);
				if(sunFlareObject.GetComponent<LensFlare>().flare != sunFlare)
				sunFlareObject.GetComponent<LensFlare>().flare = sunFlare;
				RenderSettings.skybox = darkSkyBoxMaterial;
				RenderSettings.skybox.SetFloat("_Blend", 0);
				darkSkyBoxMaterial.SetColor ("_Tint", nightColor);
				RenderSettings.ambientLight = nightAmbientLight;
				RenderSettings.fogColor = nightFog;
			}
			else if (helperValue > 4 && helperValue < 6)//(timeOfDay >= 18 && timeOfDay < 20) || (timeOfDay >= 4 && timeOfDay < 6)) 
			{
				moonOut = true;
				//Debug.Log ("Dusk");
				if(sunFlareObject.GetComponent<LensFlare>().flare != sunFlare)
					sunFlareObject.GetComponent<LensFlare>().flare = sunFlare;
				RenderSettings.skybox = darkSkyBoxMaterial;
				RenderSettings.skybox.SetFloat("_Blend", 0);
				RenderSettings.skybox.SetFloat ("_Blend", (helperValue/2)-2);
				darkSkyBoxMaterial.SetColor ("_Tint", Color.Lerp (nightColor, duskColor, (helperValue/2)-2));
				RenderSettings.ambientLight = Color.Lerp (nightAmbientLight, duskAmbientLight, (helperValue/2)-2);
				RenderSettings.fogColor = Color.Lerp (nightFog, duskFog, (helperValue/2)-2);
			}
			else if (helperValue > 6 && helperValue < 8) //timeOfDay >= 4 && timeOfDay < 8)
			{
				moonOut = true;
				//Debug.Log ("Morning");
				if(sunFlareObject.GetComponent<LensFlare>().flare != sunFlare)
					sunFlareObject.GetComponent<LensFlare>().flare = sunFlare;
				RenderSettings.skybox = brightSkyBoxMaterial;
				RenderSettings.skybox.SetFloat("_Blend", 0);
				RenderSettings.skybox.SetFloat ("_Blend", (helperValue/2)-3);
				brightSkyBoxMaterial.SetColor ("_Tint", Color.Lerp (duskColor, morningColor, (helperValue/2)-3));
				RenderSettings.ambientLight = Color.Lerp (duskAmbientLight, morningAmbientLight, (helperValue/2)-3);
				RenderSettings.fogColor = Color.Lerp (duskFog, morningFog, (helperValue/2)-3);
			}
			else if (helperValue > 8 && helperValue < 10)//timeOfDay >= 8 && timeOfDay < 16)
			{
				if (dayPhase == DayPhase.Night)
					SetDayPhase(DayPhase.Day);
				moonOut = false;
				if(sunFlareObject.GetComponent<LensFlare>().flare != sunAtNoonFlare)
					sunFlareObject.GetComponent<LensFlare>().flare = sunAtNoonFlare;
				//Debug.Log ("Noon");
				RenderSettings.ambientLight = noonAmbientLight;
				RenderSettings.skybox = brightSkyBoxMaterial;
				//sun.GetComponent<Flare>().
				RenderSettings.skybox.SetFloat("_Blend", 1);
				brightSkyBoxMaterial.SetColor ("_Tint", Color.Lerp (morningColor, noonColor, (helperValue/2)-4));
				RenderSettings.ambientLight = Color.Lerp (morningAmbientLight, noonAmbientLight, (helperValue/2)-4);;
				RenderSettings.fogColor = Color.Lerp (morningFog, noonFog, (helperValue/2)-4);
			}
		}
	}

	public void SetDayPhase(DayPhase dayPhase) {
		this.dayPhase = dayPhase;
		if(OnDayTimeChange!=null) {
			OnDayTimeChange();
		}
	}

	public void UpdateTimeOfDay(float value)
	{
		cycleValue = value;
	}

	public void ToggleCycle(bool flag)
	{
		enabled = flag;
	}
}
