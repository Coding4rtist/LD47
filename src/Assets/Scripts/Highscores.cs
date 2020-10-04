using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Highscores : MonoBehaviour {

	// http://dreamlo.com/lb/aQnUF_c_lEyW157WFrr47A7CYP1dP9R0qQ_WryBWj8vg

	private const string privateCode = "aQnUF_c_lEyW157WFrr47A7CYP1dP9R0qQ_WryBWj8vg";
   private const string publicCode = "5f7a0754eb371809c464e99f";
   private const string webURL = "http://dreamlo.com/lb/";

   //public Highscore[] highscoresList;

	public static Highscores Instance;

	private void Awake() {
		if (Instance != null) {
			Destroy(gameObject);
			return;
		}
		Instance = this;
	}

	public void AddNewHighscore(string username, int score, Action<bool> callback) {
		StartCoroutine(UploadNewHighscore(username, score, callback));
	}

	public void DownloadHighscores(Action<Highscore[]> callback) {
		StartCoroutine(DownloadHighscoresFromDatabase(callback));
	}

	private IEnumerator UploadNewHighscore(string username, int score, Action<bool> callback) {
		WWW www = new WWW(webURL + privateCode + "/add/" + WWW.EscapeURL(username) + "/" + score);
		yield return www;

		if (string.IsNullOrEmpty(www.error)) {
			print("Upload Successful");
			callback(true);
		}
		else {
			print("Error uploading: " + www.error);
			callback(false);
		}
	}

	IEnumerator DownloadHighscoresFromDatabase(Action<Highscore[]> callback) {
		WWW www = new WWW(webURL + publicCode + "/pipe/");
		yield return www;

		if (string.IsNullOrEmpty(www.error))
			callback(HighscoresFromString(www.text));
		else {
			print("Error Downloading: " + www.error);
			callback(null);
		}
	}

	private Highscore[] HighscoresFromString(string textStream) {
		string[] entries = textStream.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
		Highscore[] highscoresList = new Highscore[entries.Length];

		for (int i = 0; i < entries.Length; i++) {
			string[] entryInfo = entries[i].Split(new char[] { '|' });
			string username = entryInfo[0];
			int score = int.Parse(entryInfo[1]);
			highscoresList[i] = new Highscore(username, score);
			print(highscoresList[i].username + ": " + highscoresList[i].score);
		}

		return highscoresList;
	}
}

public struct Highscore {
	public string username;
	public int score;

	public Highscore(string _username, int _score) {
		username = _username;
		score = _score;
	}

}
