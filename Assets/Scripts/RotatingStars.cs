using UnityEngine;
using System.Collections;

public class RotatingStars : MonoBehaviour {
	// Update is called once per frame

	public float rotation;

	void Update () {
		transform.Rotate (0, 0, rotation, 0);
	}
}
