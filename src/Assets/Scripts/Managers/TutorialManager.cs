using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RedBlueGames.Tools.TextTyper;
using UnityEngine.UI;
using TMPro;

public class TutorialManager : MonoBehaviour {

	public static TutorialManager Instance;

	[SerializeField] private AudioClip printSoundEffect;

	//[Header("UI References")]
	//[SerializeField]
	//private Button printNextButton;

	//[SerializeField]
	//private Button printNoSkipButton;

	//[SerializeField]
	//private Toggle pauseGameToggle;

	private string[] dialogueLines;

	[SerializeField]
	[Tooltip("The text typer element to test typing with")]
	private TextTyper testTextTyper;


	private void Awake() {
		if (Instance != null) {
			Destroy(gameObject);
			return;
		}
		Instance = this;
	}

	private void Start() {
		testTextTyper.PrintCompleted.AddListener(HandlePrintCompleted);
		testTextTyper.CharacterPrinted.AddListener(HandleCharacterPrinted);

		//printNextButton.onClick.AddListener(HandlePrintNextClicked);
		//printNoSkipButton.onClick.AddListener(HandlePrintNoSkipClicked);

		//dialogueLines.Enqueue("You can <b>use</b> <i>uGUI</i> <size=40>text</size> <size=20>tag</size> and <color=#ff0000ff>color</color> tag <color=#00ff00ff>like this</color>.");
		//dialogueLines.Enqueue("bold <b>text</b> test <b>bold</b> text <b>test</b>");
		//dialogueLines.Enqueue("Sprites!<sprite index=0><sprite index=1><sprite index=2><sprite index=3>Isn't that neat?");
		//dialogueLines.Enqueue("You can <size=40>size 40</size> and <size=20>size 20</size>");
		//dialogueLines.Enqueue("You can <color=#ff0000ff>color</color> tag <color=#00ff00ff>like this</color>.");
		//dialogueLines.Enqueue("Sample Shake Animations: <anim=lightrot>Light Rotation</anim>, <anim=lightpos>Light Position</anim>, <anim=fullshake>Full Shake</anim>\nSample Curve Animations: <animation=slowsine>Slow Sine</animation>, <animation=bounce>Bounce Bounce</animation>, <animation=crazyflip>Crazy Flip</animation>");
		SetDefaultMessages();
		
		
		//ShowScript();
	}

	private void Update() {
		//UnityEngine.Time.timeScale = this.pauseGameToggle.isOn ? 0.0f : 1.0f;

		if (Input.GetKeyDown(KeyCode.Space) && UIManager.Instance.isTutorialOpen) {
			//Debug.Log("AAAA" + UIManager.Instance.isTutorialOpen);
			HandlePrintNextClicked();
		}
	}

	private void SetDefaultMessages() {
		dialogueLines = new string[7];
		dialogueLines[0] = "Agent, can you hear me?<delay=1.0> </delay> Get closer to the <color=#647d34>target</color> with <animation=slowsine><i>WASD</i></animation>.";
		dialogueLines[1] = "Perfect! Now you should see the <color=#9d303b>target</color>, he's wearing a <color=#9d303b>red hood</color>. Aim and shoot with the <i>mouse</i>.";
		dialogueLines[2] = "<anim=lightrot><delay=0.3>Wait</delay></anim>, what the hell happened?!?<delay=0.7> </delay>I think you killed the target in the istant the <b>timer ran out</b> and you got stuck in a time loop.";
		dialogueLines[3] = "Kill the target again, maybe this should break the loop.<delay=0.7> </delay>And watch out for yourself from the past, if he kills you,<delay=0.3> </delay>you're <anim=fullshake><delay=0.3><b>dead</b></delay></anim>!";

		dialogueLines[4] = "Time's up!<delay=0.7> </delay>Remember you only have <color=#9d303b>10 seconds</color> to complete the mission. After that, you should come back here in the present, but apparently you are still stuck!";

		dialogueLines[5] = "Mmhh... the situation has worsened, now there are <b>two copies</b> from the past.<delay=0.7> </delay>Continue to carry out your mission and try to shoot your clones too, <anim=fullshake><b>maybe</b></anim> this will work.";
		dialogueLines[6] = "<anim=lightrot>I really have no idea</anim><delay=0.7>...</delay> <i>Good luck agent</i>!";
	}

	public void SetAltMessages() {
		dialogueLines[0] = "Agent, can you hear me?<delay=1.0> </delay> What? Do you want to skip the tutorial? Go to the <color=#647d34>green area</color>!";
		dialogueLines[1] = "Good, I see you remember.<delay=1.0> </delay>Now shoot, get moving!.";
		dialogueLines[2] = "<anim=lightrot><delay=0.3>Wait</delay></anim>, what the hell happened?!?<delay=0.7> </delay>I think you killed the target in the istant the <b>timer ran out</b> and you got stuck in a time loop.";
		dialogueLines[3] = "Kill the target again, maybe this should break the loop (sploiler, it won't).<delay=0.7> </delay>And watch out for yourself from the past, if he kills you,<delay=0.3> </delay>you're <anim=fullshake><delay=0.3><b>dead</b></delay></anim>!";

		dialogueLines[4] = "Come on, haven't you learned it yet?<delay=0.7> </delay>You only have <color=#9d303b>10 seconds</color> to complete the mission. After that, you should come back here in the present, but apparently you are still stuck!";

		dialogueLines[5] = "<anim=lightrot>I'm hungry, I'm going to buy a pizza</anim><delay=0.7>...</delay> <i>Good luck agent</i>!";
	}

	public void ShowTutorialMessage(int index) {
		if (dialogueLines.Length <= 0 || index >= dialogueLines.Length) {
			return;
		}

		testTextTyper.TypeText(dialogueLines[index]);
	}


	#region TextTyper Functions
	private void HandlePrintNextClicked() {
		if (testTextTyper.IsSkippable() && testTextTyper.IsTyping) {
			testTextTyper.Skip();
		}
		else {
			//ShowScript();
			GameManager.Instance.StartTutorialRound();
		}
	}

	//private void HandlePrintNoSkipClicked() {
	//	ShowScript();
	//}

	//private void ShowScript(int index) {
	//	if (dialogueLines.Length <= 0) {
	//		return;
	//	}

	//	testTextTyper.TypeText(dialogueLines.Dequeue());
	//}

	private void HandleCharacterPrinted(string printedCharacter) {
		// Do not play a sound for whitespace
		if (printedCharacter == " " || printedCharacter == "\n") {
			return;
		}

		var audioSource = GetComponent<AudioSource>();
		if (audioSource == null) {
			audioSource = gameObject.AddComponent<AudioSource>();
		}

		audioSource.clip = printSoundEffect;
		audioSource.Play();
	}

	private void HandlePrintCompleted() {
		//Debug.Log("TypeText Complete");
	}

	#endregion
}
