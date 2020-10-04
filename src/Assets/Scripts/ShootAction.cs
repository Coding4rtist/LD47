using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootAction {
	public Vector2 direction;
	public float timeStamp;

	public ShootAction(Vector2 dir, float time) {
		direction = dir;
		timeStamp = time;
	}
}

