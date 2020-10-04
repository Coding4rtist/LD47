using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public static GameManager Instance;
	public static float Timestamp { get; private set; }

	

	public Player player;
	public GameObject replayerPrefab;
	public GameObject tutorialArea;

	public GameObject enemy;
	public Transform playerSpawnPoint;
	public Transform enemyTutorialSpawnPoint;
	public List<Transform> enemySpawnPoints;

	public int tutorialStep = 0;
	public bool roundStarted = false;

	public int RoundScore {
		get { 
			return roundScore;
		}
		set { 
			roundScore = value;
			//UIManager.Instance.SetScoreText(value);
		}
	}

	public int Score {
		get {
			return score;
		}
		set {
			score = value;
			UIManager.Instance.SetScoreText(value);
		}
	}

	private int score = 0;
	private int roundScore = 0;

	private PlayerRecorder recorder;
	//private int maxCopies = 10;
	private List<RePlayer> replayers;


	private void Awake() {
		if (Instance != null) {
			Destroy(gameObject);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(gameObject);

		recorder = GetComponent<PlayerRecorder>();
	}

	private void Start() {
		Application.targetFrameRate = 60;

		replayers = new List<RePlayer>();
		//recorder.StartRecording();
	}

	private void FixedUpdate() {
		if(!roundStarted) {
			return;
		}

		Timestamp += Time.fixedDeltaTime;
		

		if (tutorialStep < 2) {
			recorder.RecordTutorialKeyframe(player.transform.position, player.Facing, (int)player.State);
			return;
		}

		UIManager.Instance.SetDigitalClock((int)Timestamp);

		if (recorder.IsRecording)
			recorder.RecordKeyframe(player.transform.position, player.Facing, (int)player.State);

		if(Timestamp > 10.5f) { // TODO: import from recorder
			if(tutorialStep == 3) {
				CompleteTutorialRound(false);
				return;
			}

			StartRound(false);
		}
	}



	private void CreateRePlayer(bool isTutorial = false) {
		GameObject replayerGO = Instantiate(replayerPrefab, playerSpawnPoint.position, Quaternion.identity);
		//GameObject replayerGO = PoolManager.Instance.SpawnObject("replayer", playerSpawnPoint.position, Quaternion.identity);
		RePlayer replayer = replayerGO.GetComponent<RePlayer>();

		if (isTutorial)
			replayer.Setup(recorder.GetTutorialKeyframes(), recorder.GetShootActions());
		else
			replayer.Setup(recorder.GetKeyframes(), recorder.GetShootActions());

		replayers.Add(replayer);
	}

	#region Game

	private void StartNewGame() {
		tutorialStep = 0;
		Score = 0;
		RoundScore = 0;
		SetupNextTutorialRound();
	}

	public void EndGame() {
		foreach (RePlayer replayer in replayers) {
			Destroy(replayer.gameObject);
		}
		replayers.Clear();
		player.transform.position = playerSpawnPoint.position;
		player.CanMove = false;
		player.CanShoot = false;
		roundStarted = false;
		UIManager.Instance.ChangeScreen(Screens.GameOver);
		UIManager.Instance.SetGameOverScoreText(Score);
		TutorialManager.Instance.SetAltMessages();
	}

	

	public void StartRound(bool win) {
		Timestamp = 0;
		roundStarted = true;

		//if (maxCopies > 0) {
		//	CreateRePlayer();
		//	maxCopies--;
		//}

		if(win) {
			Score += RoundScore;
		}
		RoundScore = 0;
			

		if(tutorialStep > 4) {
			CreateRePlayer();
			enemy.transform.position = enemySpawnPoints[Random.Range(0, enemySpawnPoints.Count)].position;
		}


		recorder.StopRecording();

		player.transform.position = playerSpawnPoint.position;

		recorder.StartRecording();
		Debug.Log("REPLAYERS: " + replayers.Count);

		foreach (RePlayer replayer in replayers) {
			replayer.gameObject.SetActive(true);
			replayer.StartReplay();
		}
	}



	#endregion

	#region Tutorial

	public void SetupNextTutorialRound() {
		switch (tutorialStep) {
			case 0:
				// Green area text
				tutorialArea.SetActive(true);
				UIManager.Instance.ShowTutorial(0);
				UIManager.Instance.SetVisibleClockVisible(false);
				player.CanMove = false;
				player.CanShoot = false;
				enemy.transform.position = enemyTutorialSpawnPoint.position;
				break;
			case 1:
				// Shoot text
				tutorialArea.SetActive(false);
				UIManager.Instance.ShowTutorial(1);
				player.CanMove = false;
				player.CanShoot = false;
				roundStarted = false;
				recorder.PauseRecording();
				break;
			case 2:
				// Loop text 1
				UIManager.Instance.ShowTutorial(2);
				UIManager.Instance.SetDigitalClock(10);
				UIManager.Instance.SetVisibleClockVisible(true);
				player.transform.position = playerSpawnPoint.position;
				player.CanMove = false;
				player.CanShoot = false;
				roundStarted = false;
				CreateRePlayer(true);
				recorder.StopRecording();
				break;
			case 3:
				// Loop text 2
				UIManager.Instance.ShowTutorial(3);
				player.CanMove = false;
				player.CanShoot = false;
				break;
			case 4:
				// Timeout
				UIManager.Instance.ShowTutorial(4);
				player.CanMove = false;
				player.CanShoot = false;
				roundStarted = false;
				break;
			case 5:
				// Last msg 1
				roundStarted = false;
				UIManager.Instance.ShowTutorial(5);
				player.CanMove = false;
				player.CanShoot = false;
				break;
			case 6:
				// Last msg 2
				UIManager.Instance.ShowTutorial(6);
				player.CanMove = false;
				player.CanShoot = false;
				break;
		}
	}

	public void StartTutorialRound() {
		switch (tutorialStep) {
			case 0:
				// Go to green area
				UIManager.Instance.CloseTutorial();
				player.CanMove = true;
				player.CanShoot = false;
				roundStarted = true;
				recorder.StartRecording();
				break;
			case 1:
				// Shoot
				UIManager.Instance.CloseTutorial();
				player.CanMove = false;
				player.CanShoot = true;
				roundStarted = true;
				recorder.StartRecording();
				break;
			case 2:
				CompleteTutorialRound();
				break;
			case 3:
				// Start first loop round
				UIManager.Instance.CloseTutorial();
				player.CanMove = true;
				player.CanShoot = true;
				StartRound(true);
				break;
			case 4:
				// Timeout
				UIManager.Instance.CloseTutorial();
				player.CanMove = true;
				player.CanShoot = true;
				StartRound(false);
				break;
			case 5:
				// The game starts
				CompleteTutorialRound();
				break;
			case 6:
				// The game starts
				UIManager.Instance.CloseTutorial();
				player.CanMove = true;
				player.CanShoot = true;
				StartRound(true);
				break;

		}
	}

	public void CompleteTutorialRound(bool success = true) {
		if (tutorialStep == 3 && success) {
			// Skip tutorial step 4
			tutorialStep++;
		}
		tutorialStep++;
		//Debug.Log("tutorial complete");

		SetupNextTutorialRound();
		//switch (tutorialStep) {
		//	case 0:
		//		// Go to green area
		//		tutorialArea
		//		break;
		//	case 1:
		//		break;
		//}
	}

	#endregion

	#region UI Callbacks

	public void OnStartGameButtonClicked() {
		UIManager.Instance.ChangeScreen(Screens.Game);
		StartNewGame();
	}

	#endregion

}
