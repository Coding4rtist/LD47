using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRecorder : MonoBehaviour {

   public bool IsRecording { get; private set; }
   public int PositionCount { get; private set; }

   [Header("Time Recording Variables")]
   public int historySeconds = 10;
   public int framePerSecond = 10;

   

   private int maxSize;
   private Keyframe[] keyframes;
   private Queue<ShootAction> shootActions;

   private List<Keyframe> tutorialKeyframes;

   private int recordFrame;
   private int frameCounter = 0;

   private void Awake() {
      maxSize = historySeconds * framePerSecond;
      recordFrame = Mathf.RoundToInt(1f / Time.fixedDeltaTime / framePerSecond);
      Debug.Log("RECORD_FRAME: " + recordFrame + ", MAX_SIZE: " + maxSize);

      StopRecording(); // Reset all values

      // Register to PlayerShoot Event
      Player.onShoot += RecordShoot;
   }

   public void StartRecording() {
      IsRecording = true;
      Debug.Log("START RECORDING");
	}

   public void PauseRecording() {
      IsRecording = false;
      Debug.Log("PAUSE RECORDING");
   }

   public void StopRecording() {
      IsRecording = false;
      PositionCount = 0;
      keyframes = new Keyframe[maxSize];
      shootActions = new Queue<ShootAction>();
      tutorialKeyframes = new List<Keyframe>();
      Debug.Log("STOP RECORDING");
   }

   public void RecordKeyframe(Vector2 position, bool flipX, int state) {
      if (frameCounter < recordFrame) {
         frameCounter++;
      }
      else {
         // Record
         frameCounter = 0;
         if (PositionCount >= maxSize) {
            Debug.LogWarning("Cannot record position, buffer full.");
            return;
         }
         keyframes[PositionCount] = new Keyframe(position, flipX, state);
         PositionCount++;
      }
	}

   public void RecordTutorialKeyframe(Vector2 position, bool flipX, int state) {
      if (frameCounter < recordFrame) {
         frameCounter++;
      }
      else {
         // Record
         frameCounter = 0;
         if (tutorialKeyframes.Count > maxSize) {
            tutorialKeyframes.RemoveAt(0);
            PositionCount--;
         }
         tutorialKeyframes.Insert(tutorialKeyframes.Count, new Keyframe(position, flipX, state));
         PositionCount++;
      }
   }

   public void RecordShoot(Vector2 direction) {
      shootActions.Enqueue(new ShootAction(direction, GameManager.Timestamp));
	}

	public Keyframe[] GetKeyframes() { return keyframes; }

   public Keyframe[] GetTutorialKeyframes() { return tutorialKeyframes.ToArray(); }
   public Queue<ShootAction> GetShootActions() { return shootActions; }

}
