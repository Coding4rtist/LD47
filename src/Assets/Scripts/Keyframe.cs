using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keyframe {
	public Vector2 position;
	public bool facing;
	public int state;

	public Keyframe(Vector2 _pos, bool _facing, int _state) {
		position = _pos;
		facing = _facing;
		state = _state;	}
}
