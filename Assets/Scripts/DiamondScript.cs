using UnityEngine;
using System.Collections;

public class DiamondScript : MonoBehaviour {
	Animator anim;

	private int takeHash = Animator.StringToHash("Disappear");

	public bool wasTaken;

	void Start() {
		anim = GetComponent<Animator> ();
		wasTaken = false;
	}

	public void HideThis() {
		this.gameObject.SetActive (false);
	}

	public void TakeThis() {
		if (!wasTaken) {
			this.gameObject.GetComponent<AudioSource> ().Play ();
			anim.SetTrigger (takeHash);
			wasTaken = true;
		}
	}

	public bool WasTaken() {
		return wasTaken;
	}
}
