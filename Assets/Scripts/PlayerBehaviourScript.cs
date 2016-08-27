using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

// Ads
using UnityEngine.Advertisements;

// Unity Analytics metrics
using System.Collections.Generic;
using UnityEngine.Analytics;

public class PlayerBehaviourScript : MonoBehaviour
{
    private ChangeSceneScript globalData;
    private bool tabScreen = false;
    private Touch touch;
    private static int adsCounter = 1;

    public enum GameState { Loaded, Playing, Over };

    // Links and parameters
    public Text stepsLeftBarText;
    public Text diamondsLeftText;
    public Text gameOverText;
    public Text currentScoreText;
    public Text highScoreText;
    public Text currentLevelText;

    public float speed;
    public int stepsLeft;
    public float slowdownDelta;
    public float slowdownMinimal;

    // Links
    public GameObject playmodeCanvas;
    public GameObject gameOverCanvas;
    public AudioSource audioSource;
    public Button nextLevelButton;

    // Animation
    public Animator playerAnimator;

    // Debug
    public bool resetHighScore = false;

    // Scores
    private int takenDiamonds = 0;
    private int availableDiamonds;

    // Internal states
    private GameState gameState;
    private bool updateCanvas = true;
    private bool isColliding = false;
    private bool gameWon = false;

    private int direction = 1;
    private Vector3[] arrows = new[] { Vector3.left, Vector3.forward, Vector3.right, Vector3.back };
    private int[] speedDeltas = new int[] { 0, 0, 0, 0 };

    private int arrowsLength;
    private int speedDeltasLength = 4;

    // Set prefered FPS
    //void Awake() {
    //	Application.targetFrameRate = 25;
    //}

    // Use this for initialization
    void Start()
    {
        gameState = GameState.Loaded;
        globalData = ChangeSceneScript.Instance;

        // Ads sound
        AudioListener.pause = false;

        // Count total diamonds for this level
        availableDiamonds = GameObject.FindGameObjectsWithTag("Diamond").Length;

        // Get and show current level
        currentLevelText.text = string.Format("Level: {0}", globalData.currentLevel);

        // Check is audio is mute
        audioSource.mute = !globalData.musicOn;

        // Configure individual levels:
        switch (globalData.currentLevel)
        {
            case 1:
                break;
            case 2:
                break;
            case 3:
                //arrows = new [] {Vector3.left, Vector3.back, Vector3.right, Vector3.forward};
                arrows = new[] { Vector3.left, Vector3.back, Vector3.right, Vector3.forward };
                break;
            case 4:
                speedDeltas = new int[] { 0, 0, 4, 0 };
                break;
            case 5:
                arrows = new[] { Vector3.forward, Vector3.back, Vector3.left, Vector3.right };
                break;
            case 6:
                speedDeltas = new int[] { 4, 0, 4, 0 };
                break;
            case 7:
                arrows = new[] { Vector3.forward, Vector3.back, Vector3.forward, Vector3.right };
                break;
            default:
                break;
        }

        arrowsLength = arrows.Length;
    }

    // Update is called once per frame
    void Update()
    {
        // tab screen
        for (int i = 0; i < Input.touchCount; i++)
        {
            touch = Input.GetTouch(i);

            // -- Tap: quick touch & release
            ///?? 
            if (touch.phase == TouchPhase.Ended && touch.tapCount >= 1)
            {
                tabScreen = true;
            }
        }

        // Print information  on screen
        printCanvas();

        // Move player initially
        if (gameState != GameState.Loaded)
        {
            transform.Translate(arrows[direction % arrowsLength]
                                 * (speed + speedDeltas[direction % speedDeltasLength])
                                 * Time.deltaTime);

        }

        // Game state
        switch (gameState)
        {
            case GameState.Loaded:
            case GameState.Playing:
                // Change player's direction
                if (Input.GetKeyDown("space") || tabScreen)
                {
                    gameState = GameState.Playing;
                    direction++;
                    consumeStep();
                    turnPlayer();
                    tabScreen = false;
                }

                // End game, if there are no steps left
                if (stepsLeft <= 0)
                {
                    endGame();
                }

                // End game is all diamonds are taken
                if (takenDiamonds == availableDiamonds)
                {
                    gameWon = true;
                    endGame();
                }

                // Reset Collition status
                isColliding = false;
                break;

            case GameState.Over:
                // Slowdown the game dynamics
                if (Time.timeScale > slowdownMinimal || Time.timeScale > 0)
                {
                    Time.timeScale = Mathf.Abs(Time.timeScale - slowdownDelta);
                }
                else {
                    Time.timeScale = slowdownMinimal;
                }

                break;

            default:
                break;
        }

        // Quit if back button is invoked
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    // Contact controls
    void OnTriggerEnter(Collider other)
    {
        // Exit if is already colliding
        if (isColliding) return;
        isColliding = true;

        // Detect colliders
        if (other.gameObject.CompareTag("Diamond"))
        {
            // Activate particles
            this.gameObject.GetComponent<ParticleSystem>().Play();

            //Take diamond
            if (!other.gameObject.GetComponent<DiamondScript>().WasTaken())
            {
                other.gameObject.GetComponent<DiamondScript>().TakeThis();
                takenDiamonds++;
            }

            updateCanvas = true;

        }
        else if (other.gameObject.CompareTag("Terminator"))
        {
            endGame();
        }
    }


    // Steps on screen
    StringBuilder sb;
    void printCanvas()
    {
        if (updateCanvas)
        {
            // Update on canvas
            stepsLeftBarText.text = new string('|', stepsLeft);
            diamondsLeftText.text = takenDiamonds.ToString() + " / " + availableDiamonds.ToString();

            // End update
            updateCanvas = false;
        }

    }

    // Consume a step from stepbar
    void consumeStep()
    {
        updateCanvas = true;
        if (stepsLeft > 0)
            stepsLeft--;
    }

    // End current level
    void endGame()
    {
        if (gameState == GameState.Over)
            return;

        // End game
        gameState = GameState.Over;

        // Calculate, display and save points
        int score = stepsLeft * takenDiamonds;

        currentScoreText.text = "Score: " + score.ToString();

        // Show message, won / lost
        if (gameWon)
        {
            gameWon = true;
            gameOverText.text = "You Won!";
        }
        else {
            gameOverText.text = "You Lost";
        }

        // Show and save scores
        showScores(score);

        // Send metrics
        sendMetrics(ChangeSceneScript.Instance.currentLevel, score, gameWon);


        // Enable or disable Next Level button
        nextLevelButton.interactable = LevelBehaviours.enableNextLevelButton();

        // Activate  game canvas
        playmodeCanvas.SetActive(false);
        gameOverCanvas.SetActive(true);

        // Show ads
        if (ChangeSceneScript.Instance.adsCounter % 3 == 0)
        {
            ShowAd();
            //Debug.Log(ChangeSceneScript.Instance.adsCounter);
        }

        ChangeSceneScript.Instance.adsCounter += 1;
    }

    void showScores(int score)
    {
        int highScore = PlayerPrefs.GetInt(SceneManager.GetActiveScene().name + "highScore");
        bool newHighScore = false;

        if (gameWon && score > highScore)
        {
            newHighScore = true;
            highScore = score;
            PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "highScore", score);
            PlayerPrefs.Save();
        }

        // Show scores
        currentScoreText.text = "Score: " + score.ToString();
        highScoreText.text = "High Score: " + highScore.ToString();

        if (newHighScore)
        {
            highScoreText.text += " New!";
        }

        if (resetHighScore)
        {
            PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "highScore", 0);
            PlayerPrefs.Save();
        }
    }

    void sendMetrics(int level, int score, bool win)
    {
        Analytics.CustomEvent("gameOver", new Dictionary<string, object>
        {
            { "level", level },
            { "score", score },
            { "win", win }
        });
    }

    public void ShowAd()
    {
        if (Advertisement.IsReady())
        {
            AudioListener.pause = true;
            Advertisement.Show();
        }
    }


    // Animate the player
    //private int turnLeft  = Animator.StringToHash ("TurnLeft");
    //private int turnRight = Animator.StringToHash ("TurnRight");



    void turnPlayer()
    {
        /*
		//{Vector3.left, Vector3.forward, Vector3.right, Vector3.back}
		Vector3 current = arrows [direction % arrowsLength];
		Vector3 prev    = arrows [(direction - 1) % arrowsLength];

		Debug.Log ("Prev: " + prev + ", Current: " + current);

		if ((prev == Vector3.left && current == Vector3.forward) 
			|| (prev == Vector3.forward && current == Vector3.right) 
			|| (prev == Vector3.right && current == Vector3.back) 
			|| (prev == Vector3.back && current == Vector3.left)) {

			Vector3 movement = new Vector3(1, 0.5f, 0);

			Quaternion _direction = Quaternion.LookRotation(movement);
			transform.rotation = Quaternion.Lerp(transform.rotation,
			                                     _direction, 0.01f * Time.deltaTime);
		}
		*/
    }
}


