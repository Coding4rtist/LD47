using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour {

   public static UIManager Instance;

	private Screens currentScreen;

	public Screens CurrentScreen {
		get { return currentScreen; }
	}

	[Header("Screens")]
	[SerializeField] private GameObject mainScreen;
	[SerializeField] private GameObject gameScreen;
	[SerializeField] private GameObject gameoverScreen;

	[Space]
	[Header("Game")]
	[SerializeField] private TextMeshProUGUI scoreText;

	[Space]
	[Header("Tutorial")]
	[SerializeField] private GameObject tutorialUI;

	[Space]
	[Header("GameOver")]
	[SerializeField] private TextMeshProUGUI gameOverScoreText;

	public Image digitalClockImage;
	public List<Sprite> digitalClockNumbers;

	public bool isTutorialOpen = false;

	private AudioSource audioS;
	public AudioClip beepSound;
	public AudioClip doubleBeepSound;
	private int lastTime = -1;

	private void Awake() {
		if (Instance != null) {
			Destroy(gameObject);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(gameObject);

		audioS = GetComponent<AudioSource>();

		GetHighscores();
	}

	public void ChangeScreen(int screenIndex) { ChangeScreen((Screens)screenIndex); }

	public void ChangeScreen(Screens screen) {
		currentScreen = screen;
		mainScreen.SetActive(screen == Screens.MainMenu);
		gameScreen.SetActive(screen == Screens.Game);
		gameoverScreen.SetActive(screen == Screens.GameOver);

		switch (screen) {
			case Screens.MainMenu:	
				break;
			case Screens.Game:
				lastTime = -1;
				break;
			case Screens.GameOver:
				submitButton.interactable = true;
				helpMessage.text = "";
				GetHighscores();

				break;
		}
	}

	public void OpenPause() {

	}

	public void ClosePause() {

	}

	#region Tutorial

	public void ShowTutorial(int index) {
		if(currentScreen != Screens.Game) {
			Debug.LogWarning("Tutorial UI can be opened only in Game Screen.");
			return;
		}
		tutorialUI.SetActive(true);
		TutorialManager.Instance.ShowTutorialMessage(index);
		isTutorialOpen = true;
	}

	public void CloseTutorial() {
		if (currentScreen != Screens.Game) {
			Debug.LogWarning("Tutorial UI can be closed only in Game Screen.");
			return;
		}
		tutorialUI.SetActive(false);
		isTutorialOpen = false;
	}
	#endregion

	public void SetScoreText(int score) {
		scoreText.text = score.ToString();
	}

	public void SetGameOverScoreText(int score) {
		gameOverScoreText.text = "Score: " + score;
	}

	public void SetVisibleClockVisible(bool show) {
		digitalClockImage.transform.parent.gameObject.SetActive(show);
	}

	public void SetDigitalClock(int currentTime) {
		//Debug.Log(Mathf.CeilToInt(10- currentTime));
		int index = Mathf.CeilToInt(10 - currentTime);
		index = Mathf.Clamp(index, 0, 10);

		if(index != lastTime) {
			audioS.PlayOneShot(index == 0 ? doubleBeepSound : beepSound);
		}
		lastTime = index;

		digitalClockImage.sprite = digitalClockNumbers[index];
	}

	#region Highscores

	public string nickname = "";
	public Button submitButton;
	public List<TextMeshProUGUI> names;
	public List<TextMeshProUGUI> scores;
	public TextMeshProUGUI helpMessage;

	public void SetNickname(string value) {
		nickname = value;
	}

	public void SubmitHighscore() {
		if(nickname == "") {
			helpMessage.text = "You can't submit your score without a nikcname.";
			return;
		}

		Highscores.Instance.AddNewHighscore(nickname, GameManager.Instance.Score, result => { 
			if(result) {
				submitButton.interactable = false;
				helpMessage.text = "Highscore submitted successfully!";
				GetHighscores();
			}
			else {
				helpMessage.text = "An error occured while submitting the highscore, please retry.";
			}
		});
	}

	private void GetHighscores() {
		Highscores.Instance.DownloadHighscores(highscores => {
			if(highscores != null) {
				for(int i=0; i<highscores.Length;i++) {
					names[i].text = (i + 1) + ". " + highscores[i].username;
					scores[i].text = highscores[i].score.ToString();
				}
			}
		});
	}

	#endregion

}


