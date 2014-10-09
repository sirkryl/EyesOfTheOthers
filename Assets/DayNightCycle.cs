using UnityEngine;
using System.Collections;

public class DayNightCycle : MonoBehaviour {

	public Color nightFog;
	public Color duskFog;
	public Color morningFog;
	public Color noonFog;

	public Color nightAmbientLight;
	public Color duskAmbientLight;
	public Color morningAmbientLight;
	public Color noonAmbientLight;

	public GUIText timeGui;

	public Color nightColor;
	public Color duskColor;
	public Color morningColor;
	public Color noonColor;

	public Color sunAtNightColor;
	public Color sunAtDayColor;

	public float cycleValue;
	public float timeOfDay;
	public int speed = 30;
	public float sunCycle = 0;
	public Light sun;
	public Light moon;
	private bool moonOut = false;
	private float helperValue;
	public Material darkSkyBoxMaterial;
	public Material brightSkyBoxMaterial;
	// Use this for initialization
	void Start () {
		cycleValue = 0.4f;
	}


	// Update is called once per frame
	void Update () {
		if (cycleValue >= 1.0)
			cycleValue = 0;

		helperValue = sunCycle*24;
		timeOfDay = cycleValue*24;

		int hours = (int)timeOfDay;
		int minutes = (int)((timeOfDay*60)%60);

		string timeString = "Time: " + hours.ToString("00") + ":" + minutes.ToString ("00");

		timeGui.text = timeString;

		sun.transform.localEulerAngles = new Vector3((cycleValue*360)-90, 0, 0);
		cycleValue = cycleValue + Time.deltaTime/speed;
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
			RenderSettings.skybox = brightSkyBoxMaterial;
			RenderSettings.skybox.SetFloat("_Blend", 0);
			RenderSettings.skybox.SetFloat ("_Blend", (helperValue/2)-3);
			brightSkyBoxMaterial.SetColor ("_Tint", Color.Lerp (duskColor, morningColor, (helperValue/2)-3));
			RenderSettings.ambientLight = Color.Lerp (duskAmbientLight, morningAmbientLight, (helperValue/2)-3);
			RenderSettings.fogColor = Color.Lerp (duskFog, morningFog, (helperValue/2)-3);
		}
		else if (helperValue > 8 && helperValue < 10)//timeOfDay >= 8 && timeOfDay < 16)
		{
			moonOut = false;
			//Debug.Log ("Noon");
			RenderSettings.ambientLight = noonAmbientLight;
			RenderSettings.skybox = brightSkyBoxMaterial;
			RenderSettings.skybox.SetFloat("_Blend", 1);
			brightSkyBoxMaterial.SetColor ("_Tint", Color.Lerp (morningColor, noonColor, (helperValue/2)-4));
			RenderSettings.ambientLight = Color.Lerp (morningAmbientLight, noonAmbientLight, (helperValue/2)-4);;
			RenderSettings.fogColor = Color.Lerp (morningFog, noonFog, (helperValue/2)-4);
		}

	}
}
