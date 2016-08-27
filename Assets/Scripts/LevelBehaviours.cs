using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelBehaviours : MonoBehaviour {
	static ChangeSceneScript globalData;

	// Show or supress Next Level Button
	public Button nextLevelButton;

	// Save the level's score record

	// Restart the current level
	public void restartLevel() {
		Time.timeScale = 1f;
		SceneManager.LoadScene (SceneManager.GetActiveScene().name, LoadSceneMode.Single);
		Destroy (this);
	}

	// Go to the next level, if possible
	public void nextLevel() {
		Time.timeScale = 1f;

		int nextLevel = globalData.currentLevel + 1;

		if (nextLevel <= globalData.limitLevel) { 
			globalData.currentLevel = nextLevel;
            //Debug.Log("Go to next level: Level" + nextLevel);
            SceneManager.LoadScene("Level" + nextLevel);
			Destroy (this);
		}
	}

	// Go to main menu
	public void loadMainMenu() {
		Time.timeScale = 1f;
        SceneManager.LoadScene ("MainMenu");
		Destroy (this);
	}

	public void Start() {
		globalData = ChangeSceneScript.Instance;

		nextLevelButton.interactable = enableNextLevelButton ();
	}

	// Enable or disable Next Level button
	public static bool enableNextLevelButton() {
		int highScore = PlayerPrefs.GetInt ("Level"  + globalData.currentLevel.ToString() + "highScore");
		return !(globalData.currentLevel == globalData.limitLevel || highScore <= 0);
	}
}
