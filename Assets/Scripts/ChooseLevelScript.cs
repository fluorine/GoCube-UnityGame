using UnityEngine.UI;
using UnityEngine;

// Chose level from MENU screen

public class ChooseLevelScript : MonoBehaviour {
    public static bool showBlockedLevelMessage = false;

    // Fields
    //public int limitLevel;
    public Text levelText; 
	public Text highScoreText; 
	public Toggle musicToggle;
	public Button playButton;

	// Internal states
	//private int currentLevel;
	private bool updateField;
	private Touch touch;

	// Instance to save Data shared among levels
	private ChangeSceneScript globalData; 

	// Access text field
	void Start () {
		globalData = ChangeSceneScript.Instance;
		updateField = true;

		// Get music configuration
		musicToggle.isOn = globalData.musicOn;


		// Add listener to change of toggle
		musicToggle.GetComponent<Toggle> ().onValueChanged.AddListener (
			delegate {
				globalData.setMusic(musicToggle.GetComponent<Toggle>().isOn);
		    });

		// Add listener to Play button
		playButton.onClick.AddListener(() => {
			globalData.ChangeScene();
		});
	}
	
	void Update() {
		if (updateField) {
			updateField = false;

			// Update level
			levelText.text = "Level: " + globalData.currentLevel.ToString();

			// Update high score
			int highScore = PlayerPrefs.GetInt ("Level"  + globalData.currentLevel.ToString() + "highScore");
			highScoreText.text = "High Score: " + highScore; 
		}

        if (showBlockedLevelMessage)
        {
            highScoreText.text = "Complete previous!";
        }
    }

	// Increase level
	public void increaseLevel() {
		if (globalData.currentLevel < globalData.limitLevel) {
			globalData.currentLevel++;
            showBlockedLevelMessage = false;
            updateField = true;
        }
	}

	public void decreaseLevel() {
		if (globalData.currentLevel > 1) {
			globalData.currentLevel--;
            showBlockedLevelMessage = false;
            updateField = true;
		}
	}

	public void setMusic() {
		globalData.setMusic (musicToggle.isOn);
	}

	
}