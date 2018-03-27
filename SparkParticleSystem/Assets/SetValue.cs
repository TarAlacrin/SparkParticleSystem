using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SetValue : MonoBehaviour 
{
	public ValToSetEnum valToSet;

	MiscPSystemControls p;

	InputField inputty;
	 
	Text placeholder;
	public bool playerisTyping;






	// Use this for initialization
	void Start () {
		p = MiscPSystemControls.inst;
		inputty = this.gameObject.GetComponent<InputField>();
		inputty.onEndEdit.AddListener(OnEndEdit);
		inputty.onValueChanged.AddListener(OnValChange);
		placeholder = inputty.placeholder.GetComponent<Text>();
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		placeholder.text = "" + GetUpdateVal();
	}





	public void OnEndEdit(string param)
	{
		placeholder.text = inputty.text;
		inputty.text = "";
	}




	public void OnValChange(string param)
	{
		float triedToParse;
		if(float.TryParse(param, out triedToParse))
		{
			setValue(triedToParse);
		}
	}


	public float GetUpdateVal()
	{
		switch(valToSet)
		{
			case ValToSetEnum.ParticleAgeMin:
				return p.ParticleAgeMin;
			case ValToSetEnum.ParticleAgeVariation:
				return p.ParticleAgeVariation;
			case ValToSetEnum.particlesToSpawnPerFrame:
				return (float)p.particlesToSpawnPerFrame;
			case ValToSetEnum.FloorBounce:
				return p.BounceFricDrag.x;
			case ValToSetEnum.FloorFric:
				return p.BounceFricDrag.y;
			case ValToSetEnum.AirDrag:
				return p.BounceFricDrag.z;
			case ValToSetEnum.GravX:
				return p.GravityDir.x;
			case ValToSetEnum.GravY:
				return p.GravityDir.y;
			case ValToSetEnum.GravZ:
				return p.GravityDir.z;
			case ValToSetEnum.GravStr:
				return p.GravityStrength;
			case ValToSetEnum.EmitPosBaseX:
				return p.EmitPositionBase.x;
			case ValToSetEnum.EmitPosBaseY:
				return p.EmitPositionBase.y;
			case ValToSetEnum.EmitPosBaseZ:
				return p.EmitPositionBase.z ;
			case ValToSetEnum.EmitPosArcRotX:
				return p.EmitPositionRotation.x;
			case ValToSetEnum.EmitPosArcRotY:
				return p.EmitPositionRotation.y;
			case ValToSetEnum.EmitPosArcVarX:
				return p.EmitPositionApperature.x;
			case ValToSetEnum.EmitPosArcVarY:
				return p.EmitPositionApperature.y;
			case ValToSetEnum.EmitPosArcRadMin:
				return p.EmitRadiusRange.x;
			case ValToSetEnum.EmitPosArcRadMax:
				return p.EmitRadiusRange.y;
			case ValToSetEnum.EmitVelArcRotX:
				return p.EmitVelocityRotation.x;
			case ValToSetEnum.EmitVelArcRotY:
				return p.EmitVelocityRotation.y;
			case ValToSetEnum.EmitVelArcVarX:
				return p.EmitVelocityApperature.x;
			case ValToSetEnum.EmitVelArcVarY:
				return p.EmitVelocityApperature.y;
			case ValToSetEnum.EmitVelMin:
				return p.EmitVelocitySpeedVariance.x;
			case ValToSetEnum.EmitVelMax:
				return p.EmitVelocitySpeedVariance.y;
			case ValToSetEnum.SuckPosX:
				return p.SuckPos.x;
			case ValToSetEnum.SuckPosY:
				return p.SuckPos.y;
			case ValToSetEnum.SuckPosZ:
				return p.SuckPos.z;
			case ValToSetEnum.SuckInnerStr:
				return p.SuckInnerStrength;
			case ValToSetEnum.SuckOuterStr:
				return p.SuckOuterStrength;
			case ValToSetEnum.SuckInnerRad:
				return p.SuckInnerDistance;
			case ValToSetEnum.SuckTransThick:
				return p.SuckTransitionThickness;
			case ValToSetEnum.SuckCaptureDampening:
				return p.SuckCaptureDampening;
			case ValToSetEnum.SimSpeed:
				return BufferHandlerPS1.inst.simSpeed;
			case ValToSetEnum.FrameSpacing:
				return BufferHandlerPS1.inst.frameSpacing;
		}

		return float.PositiveInfinity;
	}



	public void setValue(float newVal)
	{
		switch(valToSet)
		{
			case ValToSetEnum.ParticleAgeMin:
				p.ParticleAgeMin = newVal;
				break;
			case ValToSetEnum.ParticleAgeVariation:
				p.ParticleAgeVariation = newVal;
				break;
			case ValToSetEnum.particlesToSpawnPerFrame:
				p.particlesToSpawnPerFrame = Mathf.Abs(newVal);
				break;
			case ValToSetEnum.FloorBounce:
				p.BounceFricDrag.x = newVal;
				break;
			case ValToSetEnum.FloorFric:
				p.BounceFricDrag.y = newVal;
				break;
			case ValToSetEnum.AirDrag:
				p.BounceFricDrag.z = newVal;
				break;
			case ValToSetEnum.GravX:
				p.GravityDir.x = newVal;
				break;
			case ValToSetEnum.GravY:
				p.GravityDir.y = newVal;
				break;
			case ValToSetEnum.GravZ:
				p.GravityDir.z = newVal;
				break;
			case ValToSetEnum.GravStr:
				p.GravityStrength = newVal;
				break;
			case ValToSetEnum.EmitPosBaseX:
				p.EmitPositionBase.x = newVal;
				break;
			case ValToSetEnum.EmitPosBaseY:
				p.EmitPositionBase.y = newVal;
				break;
			case ValToSetEnum.EmitPosBaseZ:
				p.EmitPositionBase.z = newVal;
				break;
			case ValToSetEnum.EmitPosArcRotX:
				p.EmitPositionRotation.x = newVal;
				break;
			case ValToSetEnum.EmitPosArcRotY:
				p.EmitPositionRotation.y = newVal;
				break;
			case ValToSetEnum.EmitPosArcVarX:
				p.EmitPositionApperature.x = newVal;
				break;
			case ValToSetEnum.EmitPosArcVarY:
				p.EmitPositionApperature.y = newVal;
				break;
			case ValToSetEnum.EmitPosArcRadMin:
				p.EmitRadiusRange.x = newVal;
				break;
			case ValToSetEnum.EmitPosArcRadMax:
				p.EmitRadiusRange.y = newVal;
				break;
			case ValToSetEnum.EmitVelArcRotX:
				p.EmitVelocityRotation.x = newVal;
				break;
			case ValToSetEnum.EmitVelArcRotY:
				p.EmitVelocityRotation.y = newVal;
				break;
			case ValToSetEnum.EmitVelArcVarX:
				p.EmitVelocityApperature.x = newVal;
				break;
			case ValToSetEnum.EmitVelArcVarY:
				p.EmitVelocityApperature.y = newVal;
				break;
			case ValToSetEnum.EmitVelMin:
				p.EmitVelocitySpeedVariance.x = newVal;
				break;
			case ValToSetEnum.EmitVelMax:
				p.EmitVelocitySpeedVariance.y = newVal;
				break;
			case ValToSetEnum.SuckPosX:
				p.SuckPos = new Vector3(newVal, p.SuckPos.y, p.SuckPos.z);
				break;
			case ValToSetEnum.SuckPosY:
				p.SuckPos = new Vector3(p.SuckPos.x, newVal, p.SuckPos.z);
				break;
			case ValToSetEnum.SuckPosZ:
				p.SuckPos = new Vector3(p.SuckPos.x, p.SuckPos.y, newVal);
				break;
			case ValToSetEnum.SuckInnerStr:
				p.SuckInnerStrength = newVal;
				break;
			case ValToSetEnum.SuckOuterStr:
				p.SuckOuterStrength = newVal;
				break;
			case ValToSetEnum.SuckInnerRad:
				p.SuckInnerDistance = newVal;
				break;
			case ValToSetEnum.SuckTransThick:
				p.SuckTransitionThickness = newVal;
				break;
			case ValToSetEnum.SuckCaptureDampening:
				p.SuckCaptureDampening = newVal;
				break;
			case ValToSetEnum.WindStr:
				p.SuckCaptureDampening = newVal;
				break;
			case ValToSetEnum.SimSpeed:
				BufferHandlerPS1.inst.simSpeed = newVal;
				break;
			case ValToSetEnum.FrameSpacing:
				BufferHandlerPS1.inst.frameSpacing = (int)newVal;
				break;


		}
	}



}


public enum ValToSetEnum
{
	ParticleAgeMin,//ParticleAgeMin
	ParticleAgeVariation,//ParticleAgeVariation

	particlesToSpawnPerFrame, //particlesToSpawnPerFrame

	FloorBounce,//BounceFricDrag
	FloorFric,
	AirDrag,

	GravX,//GravityDir
	GravY,
	GravZ,
	GravStr, //GravityStrength

	EmitPosBaseX,//EmitPositionBase
	EmitPosBaseY,
	EmitPosBaseZ,

	EmitPosArcRotX,//EmitPositionRotation
	EmitPosArcRotY,

	EmitPosArcVarX,//EmitPositionApperature
	EmitPosArcVarY, 

	EmitPosArcRadMin, //EmitRadiusRange
	EmitPosArcRadMax,


	EmitVelArcRotX,//EmitVelocityRotation
	EmitVelArcRotY,

	EmitVelArcVarX,//EmitVelocityApperature
	EmitVelArcVarY, 

	EmitVelMin, //EmitVelocitySpeedVariance
	EmitVelMax,


	SuckPosX,
	SuckPosY,
	SuckPosZ,

	SuckInnerStr, //SuckInnerStrength
	SuckOuterStr, //SuckOuterStrength
	SuckInnerRad, //SuckInnerDistance
	SuckTransThick, //SuckTransitionThickness
	SuckCaptureDampening, //SuckCaptureDampening

	WindStr,
	SimSpeed,
	FrameSpacing,
}

