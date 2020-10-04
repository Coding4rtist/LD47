using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeEntity : MonoBehaviour {

   public bool isRewinding = false;
	[Header("Time Recording Variables")]
	public float historySeconds = 10f;
	public int framePerSecond = 10;

	private List<Vector2> posHistory;
	private int recordFrame; 
	private int frameCounter = 0;
	private int reverseCounter = 0;

	// Interpolation Variables
	private bool firstRun = true;
	private Vector2 currentPos;
	private Vector2 previousPos;


	private void Start() {
		posHistory = new List<Vector2>();

		recordFrame = Mathf.RoundToInt(1f / Time.fixedDeltaTime / framePerSecond);
		Debug.Log(recordFrame);
		Debug.Log(Mathf.Round(historySeconds * framePerSecond));
	}

	private void Update() {
		if (Input.GetKeyDown(KeyCode.R)) {
			StartRewind();
		}
		if (Input.GetKeyUp(KeyCode.R)) {
			StopRewind();
		}
	}

	private void FixedUpdate() {
		if(isRewinding) {
			if(reverseCounter > 0) {
				reverseCounter--;
			}
			else {
				reverseCounter = recordFrame;
				RestorePosition();
			}

			if (firstRun) {
				firstRun = false;
				RestorePosition();
			}

			// Rewind
			float interpolation = (float)reverseCounter / recordFrame;
			transform.position = Vector3.Lerp(previousPos, currentPos, interpolation);
		}
		else {
			if(frameCounter < recordFrame) {
				frameCounter++;
			}
			else {
				// Record
				frameCounter = 0;
				if (posHistory.Count > Mathf.Round(historySeconds * framePerSecond)) {
					posHistory.RemoveAt(posHistory.Count - 1);
				}
				posHistory.Insert(0, transform.position);
			}
		}
	}

	public void StartRewind() {
		isRewinding = true;
	}

	public void StopRewind() {
		isRewinding = false;
		firstRun = true;
	}

	private void RestorePosition() {

		if(posHistory.Count > 2) {
			currentPos = posHistory[0];
			previousPos = posHistory[1];
			posHistory.RemoveAt(0);
		}

	}

}
