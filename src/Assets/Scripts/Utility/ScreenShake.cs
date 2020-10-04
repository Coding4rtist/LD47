using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour {

	public static ScreenShake Instance;

	private IEnumerator currentShakeCoroutine;
	private Vector3 originalPos;

	private void Awake() {
		if (Instance != null) {
			Destroy(gameObject);
			return;
		}
		Instance = this;

		originalPos = transform.localPosition;
	}

	public void Shake(float magnitude, float duration) {
		if(currentShakeCoroutine != null) {
			StopCoroutine(currentShakeCoroutine);
		}
		currentShakeCoroutine = ShakeRoutine(magnitude, duration);
		StartCoroutine(currentShakeCoroutine);
	}

	// TODO delete
	//private void Update() {
	//	if(Input.GetKeyDown(KeyCode.Space)) {
	//		Shake(0.5f, 0.1f);
	//	}
	//}

	private IEnumerator ShakeRoutine(float magnitude, float duration) {
		float elapsed = 0.0f;

		while (elapsed < duration) {
			float x = Random.Range(-1f, 1f) * magnitude;
			float y = Random.Range(-1f, 1f) * magnitude;

			transform.localPosition = originalPos + new Vector3(x, y, 0);

			elapsed += Time.deltaTime;

			yield return null;
		}

		transform.localPosition = originalPos;
	}
}
