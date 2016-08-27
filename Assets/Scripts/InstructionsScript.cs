using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InstructionsScript : MonoBehaviour {
	public Text instructions;

	Touch touch;
	bool touched;

	// Use this for initialization
	void Start () {
		instructions.enabled = true;
	}
	
	// Update is called once per frame
	void Update () {
		// Hear tap screen event to hide instructions
		// Quit Message
		for (int i = 0; i < Input.touchCount; i++)
		{
			touch = Input.GetTouch(i);
			
			// -- Tap: quick touch & release
			///?? 
			if (touch.phase == TouchPhase.Ended && touch.tapCount >= 1)
			{
				touched = true;
			}
		}
		
		if (touched || Input.GetKeyDown ("space")) {
			instructions.enabled = false;
			touched = false;
		}
	}
}
