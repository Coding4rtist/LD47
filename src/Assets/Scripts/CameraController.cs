using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

   public Transform target;

   public float smoothSpeed = 10f;
   public Vector3 offset;

   private void LateUpdate() {
      Vector3 desiredPosition = target.position + offset;
      Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
      if (System.Math.Round(smoothedPosition.y - Mathf.FloorToInt(smoothedPosition.y), 1) == 0.5f) {
         smoothedPosition.y += 0.01f;
      }
      transform.position = smoothedPosition;
   }

}
