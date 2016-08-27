using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;
using UnityEngine.SceneManagement;

// Change screens at MENU screen

public class ChangeSceneScript : MonoBehaviour {
    public int adsCounter;

	public int currentLevel;
	public int defaultLimitLevel = 7;
	public int limitLevel;
	//public Text scene;
	public bool musicOn;

	//Global data
	private static ChangeSceneScript GlobalData;

	public static ChangeSceneScript Instance
	{
		get
		{ 
			return GlobalData; 
		}
	}
	
	void Awake ()   
	{
		// if the singleton hasn't been initialized yet
		if (GlobalData != null && GlobalData != this) 
		{
			Destroy(this.gameObject);
		}
		
		GlobalData = this;
		DontDestroyOnLoad( this.gameObject );

        /// LOAD DATA ///
        adsCounter = 1;

		// Get if music is on
		musicOn = PlayerPrefs.GetInt ("GameMusic", 1) == 1;
		
		// Set chosen level
		limitLevel = PlayerPrefs.GetInt ("LimitLevel", defaultLimitLevel);
		
		// Get last available Level
		currentLevel = PlayerPrefs.GetInt ("CurrentGameLevel", 1);
	}

	// Save configuraton to Enable or Disable music
	public void setMusic(bool value) {
		Debug.Log ("setMusic(" + value + "), Saved");
		PlayerPrefs.SetInt ("GameMusic", value ? 1 : 0);
		PlayerPrefs.Save ();

		musicOn = value;
	}

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

	// Open the chosen level from Main menu
	public void ChangeScene () {
		// Check if previous level was completed
		if (currentLevel > 1) {
			int highScore = PlayerPrefs.GetInt ("Level" + (currentLevel - 1).ToString () + "highScore");

			if (highScore <= 0) {
				// Show message on screen and don't open the message box
				//Message.Str = "Deleted"; 
				ChooseLevelScript.showBlockedLevelMessage = true;

				return;
			}
		} 

		//Open level
		ChangeScene ("Level" + currentLevel.ToString());
	}

	// Change level from anywhere else
	public void ChangeScene(int level) {
		ChangeScene ("Level" + level.ToString ());
	}
	 
	
	// Move to level
	public void ChangeScene(string scene) {
		//Debug.Log ("CurrentGameLevel saved: " + currentLevel);

		PlayerPrefs.SetInt ("CurrentGameLevel", currentLevel);
		PlayerPrefs.Save ();

		//Application.LoadLevel (scene);
        SceneManager.LoadScene(scene);
	}

	// Go to official webpage
	public void gotoOfficialSite() {
		Application.OpenURL ("http://fluorinestudios.com/");
	}

}
