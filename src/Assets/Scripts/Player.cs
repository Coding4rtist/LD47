using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	private const float handX = 0.3f ;
	private const float handY = 0.65f;

	public delegate void OnShoot(Vector2 dir);
	public static event OnShoot onShoot;

	public bool CanMove = false;
	public bool CanShoot = false;

	private float dustUpInterval = 0.4f;
	private float dustUpTimer;
	public AudioClip[] stepSounds;
	private int stepSoundIndex = 0;
	public AudioClip shootSound;

	public AudioSource stepAudio;
	public AudioSource shootAudio;


	public enum PlayerState {
		IDLE = 0,
		MOVE = 1,
		SHOOT = 2,
		DIE = 3
	}

	//public Vector2 inputDirection = Vector2.zero;
	public Vector2 Speed = Vector2.zero;
	public PlayerState State = PlayerState.IDLE;
	public float maxSpeed = 2.5f;

	public float TimeBetweenShots = 0.2f; // ms
	private float shootElapsed;
	public bool isShooting = false;

	public GameObject bulletPrefab;

	private SpriteRenderer sr;
   private Animator anim;
	private InputsData inputs;
	

	private int moveX, moveY;
	private bool oldFlipX = false;

	public bool Facing {
		get { return sr.flipX; }
	}

	private void Awake() {
		sr = GetComponent<SpriteRenderer>();
		anim = GetComponentInChildren<Animator>();
	}

	private void Update() {
		if(CanMove) {
			// GET INPUTS
			inputs.kLeft = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
			inputs.kRight = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
			inputs.kUp = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
			inputs.kDown = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
		}
		else {
			inputs.kLeft = inputs.kRight = false;
			inputs.kUp = inputs.kDown = false;
		}


		if (CanShoot && Input.GetMouseButtonDown(0)) {
			Shoot();
		}

		// Time Slow
		//if (Input.GetKeyDown(KeyCode.LeftShift)) {
		//	Time.timeScale = 0.3f;
		//}
		//else if (Input.GetKeyUp(KeyCode.LeftShift)) {
		//	Time.timeScale = 1.0f;
		//}

		Move();


		switch (State) {
			case PlayerState.IDLE:
				if (Speed.magnitude >= 0.05f) {
					anim.Play("Move");
					State = PlayerState.MOVE;
				}
				// Facing
				sr.flipX = Speed.x == 0 ? sr.flipX : Speed.x < 0;
				oldFlipX = sr.flipX; // use for vfx
				break;
			case PlayerState.MOVE:
				if (Speed.magnitude < 0.05f) {
					anim.Play("Idle");
					State = PlayerState.IDLE;
				}

				// Dust Particles
				if (Speed.magnitude > 0.1f) {
					dustUpTimer += Time.deltaTime;
					if (dustUpTimer >= dustUpInterval) {
						GameObject dustGO = PoolManager.Instance.SpawnObject("walkpuff", transform.position + new Vector3(0, 0.2f, 0f), transform.rotation);
						Particle dust = dustGO.GetComponent<Particle>();
						dust.Play();
						dustUpTimer = 0f;
						stepAudio.PlayOneShot(stepSounds[stepSoundIndex]);
						stepSoundIndex = (stepSoundIndex + 1) % stepSounds.Length;
					}
				}

				// Facing
				sr.flipX = Speed.x == 0 ? sr.flipX : Speed.x < 0;
				oldFlipX = sr.flipX; // use for vfx
				break;
			case PlayerState.SHOOT:
				
				break;
			case PlayerState.DIE:
				break;
		}

	}

	private void FixedUpdate() {
		transform.Translate(Speed * Time.fixedDeltaTime);

		shootElapsed -= Time.fixedDeltaTime;
	}

	private void Move() {
		moveX = inputs.kRight ? 1 : inputs.kLeft ? -1 : 0;
		moveY = inputs.kUp ? 1 : inputs.kDown ? -1 : 0;

		float accel = maxSpeed / 0.12f; // old: 3.6f;
		float frict = maxSpeed / 0.06f; // old: 7.2f;

		if (Mathf.Abs(Speed.x) > (float)maxSpeed) {
			Speed.x = Approach(Speed.x, Mathf.Sign(Speed.x) * maxSpeed, frict);
		}
		else {
			if (moveX != 0) {
				//Debug.Log("A: " + Speed.x + " " + Time.fixedDeltaTime);
				Speed.x = Approach(Speed.x, moveX * maxSpeed, Time.fixedDeltaTime * accel);
			}

			else {
				//Debug.Log("B: " + Speed.x);
				Speed.x = Approach(Speed.x, moveX * maxSpeed, Time.fixedDeltaTime * frict);
			}
		}

		if (Mathf.Abs(Speed.y) > (float)maxSpeed) {
			Speed.y = Approach(Speed.y, Mathf.Sign(Speed.y) * maxSpeed, frict);
		}
		else {
			if (moveY != 0) {
				Speed.y = Approach(Speed.y, moveY * maxSpeed, Time.fixedDeltaTime * accel);
			}

			else {
				Speed.y = Approach(Speed.y, moveY * maxSpeed, Time.fixedDeltaTime * frict);
			}
		}
		

		// FIX DIAGONAL SPEED
		if (Speed.magnitude > maxSpeed) {
			Speed = Speed.normalized * maxSpeed;
		}
	}


	private void Shoot() {
		if(shootElapsed <= 0f) {
			isShooting = true;
			shootElapsed = TimeBetweenShots;
			anim.Play("Shoot");
			State = PlayerState.SHOOT;
			maxSpeed = 1.5f;

			shootAudio.PlayOneShot(shootSound);

			Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			if(Facing && mousePos.x >= transform.position.x) {
				sr.flipX = false;
			}
			else if(!Facing && mousePos.x <= transform.position.x) {
				sr.flipX = true;
			}
		}
	}


	public void onShootAnimationSpawn() {
		Vector2 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		float xOff = sr.flipX ? -handX : handX;
		Vector2 spawnPos = (Vector2)transform.position + new Vector2(xOff, handY);
		Vector2 dir = target - spawnPos;
		dir.Normalize();

		//GameObject bulletGO = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
		GameObject bulletGO = PoolManager.Instance.SpawnObject("bullet", spawnPos, Quaternion.identity);
		Bullet bullet = bulletGO.GetComponent<Bullet>();
		bullet.Shoot(dir);

		ScreenShake.Instance.Shake(0.1f, 0.1f);

		// Knockback
		Speed = -dir * 3.5f;

		// Shoot Event
		onShoot?.Invoke(dir);
	}
	public void onShootAnimationEnd() {
		State = PlayerState.IDLE;
		anim.Play("Idle");
		maxSpeed = 3f;
	}


	private float Approach(float start, float end, float shift) {
		if (start < end)
			return Mathf.Min(start + shift, end);
		else
			return Mathf.Max(start - shift, end);
	}
}
