using UnityEngine;
using System.Collections;

public class MiscPSystemControls : MonoBehaviour 
{
	BufferHandlerPS1 partHandler;
	Material[] elidgableMats;
	public static MiscPSystemControls inst;

	public float ParticleAgeMin;
	public float ParticleAgeVariation;

	public Vector4 ageMinVar{
		get{
			return new Vector4(ParticleAgeMin, ParticleAgeVariation,0,0);
		}
	}

    public float particlesToSpawnPerFrame = 6;

	public Vector4 BounceFricDrag =  new Vector4(0.7f, 0.95f, 0.01f);
		   
	public Vector3 GravityDir = Vector3.down;
	public float GravityStrength = 10f;


	public Vector3 EmitPositionBase;
	public Vector2 EmitRadiusRange;

	public Vector2 EmitPositionRotation;
	public Vector2 EmitPositionApperature;

	public Vector4 emitPosBase{
		get{
			return new Vector4(EmitPositionBase.x,EmitPositionBase.y,EmitPositionBase.z,1);
		}
	}


	public Vector4 emitPosRotAperature{
		get{
			return new Vector4(EmitPositionRotation.x,EmitPositionRotation.y,EmitPositionApperature.x,EmitPositionApperature.y);
		}
	}



	public Vector2 EmitVelocitySpeedVariance;

	public Vector2 EmitVelocityRotation;
	public Vector2 EmitVelocityApperature;
	public Vector4 emitVelDirData{
		get{
			return new Vector4(EmitVelocityRotation.x, EmitVelocityRotation.y, EmitVelocityApperature.x, EmitVelocityApperature.y);
		}
	}

	public Vector4 emitVelspeedrangePosoffrange{
		get{
			return new Vector4(EmitVelocitySpeedVariance.x, EmitVelocitySpeedVariance.y, EmitRadiusRange.x, EmitRadiusRange.y);
		}
	}



	public float SuckInnerStrength;
	public float SuckOuterStrength;
	public float SuckInnerDistance;
	public float SuckTransitionThickness;
	public float SuckCaptureDampening;

	public Vector4 suckInnerOuter{
		get{
			return new Vector4(SuckInnerStrength, SuckOuterStrength, SuckInnerDistance, 1/(SuckTransitionThickness));
		} 
	}

	public Vector3 SuckPos{
		get{
			return BufferHandlerPS1.inst.gravityObject.position;
		}
		set{
			BufferHandlerPS1.inst.gravityObject.position = value;
		}
	}


	public float WindStr;// max strength
	public float WindVariability;//how much it varies
	public float WindVolitility; //how quickly it varies
	public float WindSize;//how big an impact position has

	public Vector4 windData{
		get{
			return new Vector4(WindStr, WindVariability, WindVolitility, WindSize);
		}
	}

	// Use this for initialization
	void Start () {
		MiscPSystemControls.inst = this;

	}
	
	// Update is called once per frame
	void Update () {
			MiscPSystemControls.inst = this;
	}
}

