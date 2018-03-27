using UnityEngine;
using System.Collections;

public class AudioSampler : MonoBehaviour {
	
	public bool  interpolate = true;
	public bool expandOut = true;
	public float volume = 7f;

	public int channel = 0;
	public float waveFormPower = 1.5f;
	
	public int xSamples = 8;  // array size (number of samples chosen and displayed as Boxes)
	public int qSamples = 1024;  // array size (number of samples for quality. )
	public float refValue = 0.1f; // RMS value for 0 dB
	public float threshold= 0.02f;      // minimum amplitude to extract pitch
	public float rmsValue;   // sound level - RMS
	 float dbValue;    // sound level - dB
	 float pitchValue; // sound pitch - Hz

	public Vector2 levelsPowerMult = new Vector2(1.3f, 0.8f);//determines the frequency-spread of the display cubes that display the sound 
	
	
	 float[] waveForm; 
	 GameObject[] waveObjs;
	
	 float[] levels;
	 GameObject[] levelObjs;//List of the cubes which display the levels 
	
	 float[] samples; // audio samples
	 float[] spectrum; // audio spectrum
	public float fSample;

	AudioSource THEsource;
	public bool boxCollidersEnabled = false;
	bool lastCollidersEnabled = false;
	
	[Range(10,22000)]
	public float BypassRange = 2000;

	void  Start ()
	{

		THEsource = this.gameObject.GetComponent<AudioSource>();
		samples = new float[qSamples];
		spectrum = new float[qSamples];
		fSample = AudioSettings.outputSampleRate;

		
		
		waveForm = new float[xSamples];
		waveObjs = new GameObject[xSamples];
		
		levels = new float[xSamples];
		levelObjs = new GameObject[xSamples];
		
		
		float spacer;
		spacer = this.transform.localScale.x/xSamples;
		spacer /= 3f;
		GameObject Ginny = GameObject.CreatePrimitive(UnityEngine.PrimitiveType.Cube);
		Ginny.GetComponent<MeshRenderer>().material.color = Color.red;
		DestroyImmediate(Ginny.GetComponent<BoxCollider>());
		int i;
		for(i = 0; i < xSamples; i++)
		{
			waveObjs[i] = Instantiate(Ginny,Vector3.zero, Quaternion.identity) as GameObject; //GameObject.CreatePrimitive(UnityEngine.PrimitiveType.Cube);
			waveObjs[i].transform.position = new Vector3(	this.transform.position.x - this.transform.localScale.x *0.5f + i*3*spacer + spacer,
			                                             this.transform.position.y + 1,
			                                             this.transform.position.z + this.transform.localScale.z*0.5f + 3);
			waveObjs[i].transform.localScale = new Vector3(spacer *2, waveObjs[i].transform.localScale.y, waveObjs[i].transform.localScale.z);
			waveObjs[i].name = "waveCube " + i;
			waveObjs[i].transform.SetParent(this.transform);

			levelObjs[i] = Instantiate(Ginny,Vector3.zero, Quaternion.identity) as GameObject;
			levelObjs[i].transform.position = new Vector3(	this.transform.position.x - this.transform.localScale.x *0.5f + i*3*spacer + spacer,
			                                              this.transform.position.y + 1,
			                                              this.transform.position.z - this.transform.localScale.z*0.5f - 3);
			levelObjs[i].transform.localScale = new Vector3(spacer *2, levelObjs[i].transform.localScale.y, levelObjs[i].transform.localScale.z);

			levelObjs[i].name ="LevelsBlox " + 	IndexToFrequency(i) + "Hz";

			levelObjs[i].transform.SetParent(this.transform);
		}
		Destroy(Ginny);
	}
	bool frameOne= true;
	void  AnalyzeSound ()
	{
		THEsource.GetOutputData(samples, channel); // fill array with samples (this holds the wave-form's shape. Or more specifically it samples the amplitude at different "X" values)
		THEsource.outputAudioMixerGroup.audioMixer.SetFloat("BypassCutoffFreq", this.BypassRange);
		
		int i;
		float sum = 0;
		for (i=0; i < qSamples; i++)
		{
			sum += samples[i]*samples[i]; // sum squared samples
		}
		rmsValue = Mathf.Sqrt(sum/qSamples); // rms = square root of average
		dbValue = 10*Mathf.Log(rmsValue/refValue, 4); // calculate dB
		if (dbValue < -160) dbValue = -160; // clamp it to -160dB min
		// get sound spectrum
		THEsource.GetSpectrumData(spectrum, channel, FFTWindow.BlackmanHarris);
		float maxV = 0;
		int maxN = 0;
		for (i=0; i < qSamples; i++)
		{ // find max 
			if (spectrum[i] > maxV && spectrum[i] > threshold)
			{
				maxV = spectrum[i];
				maxN = i; // maxN is the index of max
			}
		}
		
		int sampsPerLevel;
		sampsPerLevel = qSamples/xSamples;
		if(!expandOut)
		{
			if(sampsPerLevel >2)
			{
				sampsPerLevel = 2;
			}
		}

		int j;
		int k;
		for(i = 0; i < xSamples; i++)
		{
			levels[i] = 0;
			waveForm[i] = 0;

			if(sampsPerLevel == 1)
			{
				waveForm[i] = samples[i];
				levels[i] = spectrum[i];
			}
			else
			{
				for (j = 1; j < sampsPerLevel; j++)
				{
					//expandout determines whether the wave form will be as zoomed in as possible or zoomed out to encompass the whole thing
					k = i * sampsPerLevel + j;


					if (k < qSamples - 1)
					{
						float aL;
						if (interpolate)
						{
							aL = samples[k - 1] + samples[k + 1] + 2 * samples[k];
						}
						else
						{
							aL = 4 * samples[k];
						}

						waveForm[i] += Mathf.Pow((Mathf.Abs(0.25f * aL)), waveFormPower);/**(0.25f*aL)*/;


						int k1 = (int)(Mathf.Pow((float)i / (float)xSamples, levelsPowerMult.x) * xSamples * sampsPerLevel * levelsPowerMult.y) + j;
						/*					float bin = 1f;

											if(IndexToFrequency(i) > this.BypassRange) bin = 0;
						*/

						levels[i] += spectrum[k1];// k*(fSample/2)/qSamples;
					}


				}

				waveForm[i] /= sampsPerLevel - 1;
			}
		}
		frameOne = false;
		//Soften the waveform a bit by averaging it with both its neighbors. 
		float freqN = maxN; // pass the index to a float variable
		if (maxN > 0 && maxN < qSamples-1)
		{ // interpolate index using neighbours
			float dL= spectrum[maxN-1]/spectrum[maxN];
			float dR= spectrum[maxN+1]/spectrum[maxN];
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
		
		float x =  volume * rmsValue*rmsValue*0.9f;
		if(interpolate)
		{
			transform.localScale = Vector3.Scale(transform.localScale, new Vector3(1,0,1)) + Vector3.up*(transform.localScale.y + x*3)/4;
		}
		else
		{
			transform.localScale = Vector3.Scale(transform.localScale, new Vector3(1,0,1)) + Vector3.up*x;
		}


				if(Input.GetKeyDown(KeyCode.O))
						this.expandOut = !this.expandOut;
				if(Input.GetKeyDown(KeyCode.I))
						this.interpolate = !this.interpolate;
				
		
		float sY;
		for(int i = 0; i < xSamples; i++)
		{
			sY =  waveForm[i] * volume;
			waveObjs[i].transform.localScale = new Vector3(waveObjs[i].transform.localScale.x, sY, waveObjs[i].transform.localScale.z);

			float m = 1f;
			if(!expandOut)
					m = 1.4f;
						
			sY =Mathf.Log10(Mathf.Pow(levels[i],m)*volume*volume*volume*volume*volume + 1);
			levelObjs[i].transform.localScale = new Vector3(levelObjs[i].transform.localScale.x, sY, levelObjs[i].transform.localScale.z);
		
		}
	}
}