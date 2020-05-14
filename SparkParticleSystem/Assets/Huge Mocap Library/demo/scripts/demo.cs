using UnityEngine;
using System.Collections;

public class demo : MonoBehaviour {

	public Animator animator;
	
	void OnGUI() {

		GUILayout.BeginVertical("box");
		if (GUILayout.Button("movements")) {
			animator.SetTrigger("movements");
		}
		if (GUILayout.Button("sports")) {
			animator.SetTrigger("sports");
		}
		if (GUILayout.Button("martial arts")) {
			animator.SetTrigger("martialarts");
		}
		GUILayout.FlexibleSpace();
		GUILayout.Box("This is just a tiny sample of the 2534 animations inside of this library.");
		GUILayout.FlexibleSpace();
		GUILayout.EndVertical();
	}
}
