using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
   public static AudioManager Instance;

   public AudioSource backgroundMusic;
   public AudioSource exampleSFX;

	private void Awake() {
		if(Instance != null) {
			Destroy(gameObject);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(gameObject);
	}

	public void PlayExampleSFX(AudioClip clip) {
		exampleSFX.clip = clip;
		exampleSFX.Play();
	}
}
