using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioGrab : MonoBehaviour
{

	public bool displayLevelsAsFrequency = false;
	public Color col = Color.red;
	public bool interpolate = true;
	public bool expandOut = true;
	public bool stepClampValues = true;
	public float volume = 7f;
	public float volRmsPow = 2f;
	public float volRmsInfluence = 1f;
	

	public float playbackspeed;
	public int channel = 0;

	public Material barmat;
	
	public int xSamples = 8;  // array size (number of samples chosen and displayed as Boxes)
	public int qSamples = 1024;  // array size (number of samples for quality. )
	public float refValue = 0.1f; // RMS value for 0 dB
	public float threshold= 0.02f;      // minimum amplitude to extract pitch
	public float rmsValue;   // sound level - RMS
	float dbValue;    // sound level - dB
	public float pitchValue; // sound pitch - Hz

	public Vector2 levelsPowerMult = new Vector2(1.3f, 0.8f);//determines the frequency-spread of the display cubes that display the sound 
	
	
	 float[] waveForm; 
	 GameObject[] waveObjs;
	
	 float[] levels;
	 GameObject[] levelObjs;//List of the cubes which display the levels

	public List<float> vollist;
	
	 float[] samples; // audio samples
	 float[] spectrum; // audio spectrum
	public float fSample;

	public AudioSource THEsource;
	public bool boxCollidersEnabled = false;
	bool lastCollidersEnabled = false;
	
	[Range(10,22000)]
	public float BypassRange = 2000;



	void  Start ()
	{
		if(THEsource == null)
			THEsource = this.gameObject.GetComponent<AudioSource>();

		samples = new float[qSamples];
		spectrum = new float[qSamples];
		fSample = AudioSettings.outputSampleRate;

		
		
		waveForm = new float[xSamples];
		waveObjs = new GameObject[xSamples];
		
		levels = new float[xSamples];
		levelObjs = new GameObject[xSamples];
		vollist = new List<float>(xSamples);
		vollist.Fill<float>(0.5f);
		


		float spacer;
		spacer = this.transform.localScale.x/xSamples;
		spacer /= 3f;
		GameObject Ginny = GameObject.CreatePrimitive(UnityEngine.PrimitiveType.Quad);
		Ginny.GetComponent<MeshRenderer>().material = barmat;
		Ginny.GetComponent<MeshRenderer>().material.color = col;
		DestroyImmediate(Ginny.GetComponent<Collider>());
		int i;
		for(i = 0; i < xSamples; i++)
		{
			waveObjs[i] = Instantiate(Ginny,Vector3.zero, Quaternion.identity) as GameObject; //GameObject.CreatePrimitive(UnityEngine.PrimitiveType.Cube);
			waveObjs[i].transform.position = new Vector3(	this.transform.position.x - this.transform.localScale.x *0.5f + i*3*spacer + spacer,
			                                             this.transform.position.y + 1,
			                                             this.transform.position.z + this.transform.localScale.z*0.5f + 3 - channel*5f);
			waveObjs[i].transform.localScale = new Vector3(spacer *2, waveObjs[i].transform.localScale.y, waveObjs[i].transform.localScale.z);

			waveObjs[i].transform.rotation = Quaternion.AngleAxis(180f * channel, Vector3.up);
			waveObjs[i].name = "waveCube " + i;
			waveObjs[i].transform.SetParent(this.transform);

			levelObjs[i] = Instantiate(Ginny,Vector3.zero, Quaternion.identity) as GameObject;
			levelObjs[i].transform.position = new Vector3(	this.transform.position.x - this.transform.localScale.x *0.5f + i*3*spacer + spacer,
			                                              this.transform.position.y + 1,
			                                              this.transform.position.z - this.transform.localScale.z*0.5f - 27f + channel*5f);
			levelObjs[i].transform.localScale = new Vector3(spacer *2, levelObjs[i].transform.localScale.y, levelObjs[i].transform.localScale.z);

			levelObjs[i].name ="LevelsBlox " + spectrumIndexToFreq(i) + "Hz";

			levelObjs[i].transform.SetParent(this.transform);
			//levelObjs[i].SetActive(false);
		}
		Destroy(Ginny);
	}
	bool frameOne= true;



	public float spectrumIndexToFreq(int index)
	{
	
		return (float)index * (fSample / 2) / qSamples;
	}

	public float GetRMS(float[] parSamples)
	{
		int i;
		float sum = 0;
		for (i = 0; i < qSamples; i++)
		{
			sum += samples[i] * samples[i]; // sum squared samples
		}
		return Mathf.Sqrt(sum / qSamples); // rms = square root of average

	}




	void  AnalyzeSound ()
	{
		THEsource.GetOutputData(samples, channel); // fill array with samples (this holds the wave-form's shape. Or more specifically it samples the amplitude at different "X" values)
		 //THEsource.outputAudioMixerGroup.audioMixer.SetFloat("BypassCutoffFreq", this.BypassRange);

		rmsValue = GetRMS(samples); // rms = square root of average



		dbValue = 20*Mathf.Log(rmsValue/refValue, 10); // calculate dB
		dbValue = Mathf.Clamp(dbValue, -160f, 160f);

		// get sound spectrum
		THEsource.GetSpectrumData(spectrum, channel, FFTWindow.Rectangular);
		
		int sampsPerLevel;

		sampsPerLevel = qSamples/xSamples;

		vollist.CycleAdd(dbValue);
		int i;

		int j;
		int k;
		for(i = 0; i < xSamples; i++)
		{
			levels[i] = 0;
			waveForm[i] = 0;


			int grabindex = i;
			if (expandOut)
				grabindex = Mathf.Min(qSamples - 1, i * Mathf.FloorToInt(qSamples / xSamples));



			waveForm[i] = samples[grabindex];

			if(displayLevelsAsFrequency)
				levels[i] = 0.001f*spectrumIndexToFreq(i) ;//spectrum[grabindex];
			else
				levels[i] = Mathf.Max(vollist[i],0);//spectrum[grabindex];
		}

		float highAmp = 0;
		int highAmpInd = 0;
		for (i = 0; i < qSamples; i++)
		{ // find max 
			if (spectrum[i] > highAmp && spectrum[i] > threshold)//if the amplitude (spectrum[]) of the wavelength at index (i) is greater than the threshhold of volume, and greater than the loudest freq
			{
				highAmp = spectrum[i];
				highAmpInd = i; // maxN is the index of max
			}
		}

		frameOne = false;
		//Soften the waveform a bit by averaging it with both its neighbors. 
		float freqN = highAmpInd; // pass the index to a float variable
		if (highAmpInd > 0 && highAmpInd < qSamples-1)
		{ // interpolate index using neighbours
			float dL= spectrum[highAmpInd-1]/spectrum[highAmpInd];
			float dR= spectrum[highAmpInd+1]/spectrum[highAmpInd];
			freqN += 0.5f*(dR*dR - dL*dL);
		}
		pitchValue = freqN*(fSample/2)/qSamples; // convert index to frequency
	}

	float IndexToFrequency(int INdex)
	{
		return (Mathf.Pow((float)INdex/(float)xSamples, levelsPowerMult.x)*levelsPowerMult.y*fSample*0.5f);
	}

	 
	
	void  Update (){
		AnalyzeSound();
		
		float x =  volume * Mathf.Lerp(1, Mathf.Pow(rmsValue, volRmsPow), volRmsInfluence);




		if(Input.GetKeyDown(KeyCode.O))
				this.expandOut = !this.expandOut;
		if(Input.GetKeyDown(KeyCode.I))
				this.interpolate = !this.interpolate;
				
		
		float sY;
		for(int i = 0; i < xSamples; i++)
		{


			sY =  waveForm[i] * volume;

			float uniqueLevels = 16;
			if(stepClampValues)
			{
				float signsy = Mathf.Sign(sY);

				sY = Mathf.Abs(sY * 0.1f);

				sY =  Mathf.Floor(uniqueLevels * sY) / uniqueLevels;

				sY = sY * signsy * 10f;
			}

			waveObjs[i].transform.localScale = new Vector3(waveObjs[i].transform.localScale.x, sY, waveObjs[i].transform.localScale.z);


			sY = levels[i];//Mathf.Log10(Mathf.Pow(levels[i],m)*volume*volume*volume*volume*volume + 1);
			levelObjs[i].transform.localScale = new Vector3(levelObjs[i].transform.localScale.x, sY, levelObjs[i].transform.localScale.z);


		}
	}



}