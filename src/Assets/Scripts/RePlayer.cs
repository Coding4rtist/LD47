using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RePlayer : PoolObject {

   private const float handX = 0.3f;
   private const float handY = 0.65f;

   public GameObject bulletPrefab;

   public bool isReplaying = false;

   private Keyframe[] keyframes;
   private ShootAction[] shootActions;
   
   private int replayFrame = 0;
   private int replayAction = 0;
   private int frameCounter = 0;
   // Interpolation Variables
   private Vector2 currentPos;
   private Vector2 nextPos;

   private SpriteRenderer sr;
   private Animator anim;

   public float Speed = 0f;
   private float dustUpInterval = 0.4f;
   private float dustUpTimer;
   public AudioClip[] stepSounds;
   private int stepSoundIndex = 0;
   public AudioClip shootSound;

   public AudioSource stepAudio;
   public AudioSource shootAudio;

   private void Awake() {
      sr = GetComponent<SpriteRenderer>();
      anim = GetComponent<Animator>();
	}

	public void Setup(Keyframe[] keys, Queue<ShootAction> actions) {
      // Remove unused keyframes
      Keyframe[] filtered = Array.FindAll(keys, k => k != null);
      keyframes = new Keyframe[filtered.Length];
		Array.Copy(filtered, keyframes, filtered.Length);
      shootActions = actions.ToArray();

      Debug.Log(filtered.Length + "|" + shootActions.Length);
	}

   public void StartReplay() {
      transform.position = Vector2.zero;
      replayFrame = 0;
      replayAction = 0;
      frameCounter = 0;
      RestorePositionAndFacing();
      isReplaying = true;
   }

	private void Update() {

      // Dust Particles
      if (Speed > 0.01f) {
         dustUpTimer += Time.deltaTime;
         if (dustUpTimer >= dustUpInterval) {
            //GameObject dustGO = PoolManager.Instance.GetPooledObject("Walk-Puff", transform.position, transform.rotation);
            //Particle dust = dustGO.GetComponent<Particle>();
            //dust.Play("walk-puff");
            dustUpTimer = 0f;
            stepAudio.PlayOneShot(stepSounds[stepSoundIndex]);
            stepSoundIndex = (stepSoundIndex + 1) % stepSounds.Length;
         }
      }
   }

	private void FixedUpdate() {
		if(isReplaying) {
         if (frameCounter < 5) { // TODO import from recorder
            frameCounter++;
         }
			else {
				frameCounter = 0;
				
				RestorePositionAndFacing();
            replayFrame++;
         }

			if (replayFrame < keyframes.Length) {
            float interpolation = frameCounter / 5f; // TODO import from recorder
            Vector3 oldPos = transform.position;
            transform.position = Vector3.Lerp(currentPos, nextPos, interpolation);
            Speed = (transform.position - oldPos).magnitude;
         }

         if(replayAction < shootActions.Length && shootActions[replayAction].timeStamp <= GameManager.Timestamp) {
            ReplayShoot(shootActions[replayAction].direction);
            replayAction++;
			}
      }
	}

   private void RestorePositionAndFacing() {
      int firstIndex = replayFrame;
      int secondIndex = replayFrame + 1;

      if (secondIndex < keyframes.Length) {
         currentPos = keyframes[firstIndex].position;
         nextPos = keyframes[secondIndex].position;

         sr.flipX = keyframes[replayFrame].facing;
			switch (keyframes[replayFrame].state) {
				case 0: anim.Play("Idle"); break;
            case 1: anim.Play("Move"); break;
            case 2: anim.Play("Shoot"); break;
            case 3: anim.Play("Death"); break;
         }
      }
      else {
         isReplaying = false;
         anim.Play("Idle");
      }
   }


   private void ReplayShoot(Vector2 dir) {
      Debug.Log("REPLAY SHOOT");
		float xOff = sr.flipX ? -handX : handX;
		Vector2 spawnPos = (Vector2)transform.position + new Vector2(xOff, handY);

      shootAudio.PlayOneShot(shootSound);

      //GameObject bulletGO = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
      GameObject bulletGO = PoolManager.Instance.SpawnObject("bullet", spawnPos, Quaternion.identity);
		Bullet bullet = bulletGO.GetComponent<Bullet>();
		bullet.Shoot(dir, true);
	}

   public void onShootAnimationSpawn() {
     
   }
   public void onShootAnimationEnd() {
      anim.Play("Idle");
   }

}
