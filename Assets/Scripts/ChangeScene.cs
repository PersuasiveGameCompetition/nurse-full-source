using UnityEngine;
using System.Collections;

public class ChangeScene : MonoBehaviour {

	public string nextSceneName;
	public string debugText;
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonUp (0)) {
			Debug.Log (debugText);
			Application.LoadLevel(nextSceneName);
		}
	}
}
