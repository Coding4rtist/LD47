using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : PoolObject {

   public float speed;
   public float lifeTime;
   public Vector2 direction, lastDirection;
   public AnimationCurve speedCurve;
   public LayerMask collisionMask;

   private float elapsed = 0;
   private int reboundCount = 1;

   [SerializeField]private bool fromReplayer;

   public void Shoot(Vector2 dir, bool _fromReplayer = false) {
      direction = dir;
      fromReplayer = _fromReplayer;
      elapsed = 0;
      reboundCount = 1;
   }

	private void FixedUpdate() {
      float actualSpeed = speedCurve.Evaluate(elapsed / lifeTime) * speed;


      //lastDirection = direction * actualSpeed;
      float magnitude = actualSpeed * Time.fixedDeltaTime;
      CheckCollisions(magnitude);
      transform.Translate(direction * magnitude);

      elapsed += Time.fixedDeltaTime;
      if(elapsed > lifeTime) {
         Destroy();
		}
	}

   private void CheckCollisions(float moveDistance) {
      RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, moveDistance, collisionMask);
      Debug.DrawLine(transform.position, (Vector2)transform.position + direction * moveDistance, Color.red);

      if(hit) {
         // TODO: on hit object

         if (hit.collider.tag == "Wall") {
            //Debug.Log("WALL COLLISION!!!");
            direction = Vector2.Reflect(direction, hit.normal).normalized;
            reboundCount--;
            if(reboundCount < 0) {
               Destroy();
				}
         }

         if (hit.collider.tag == "Player" && hit.collider.isTrigger) {
            Debug.Log("PLAYER KILLED, GAME OVER...");
            Destroy();

            GameManager.Instance.EndGame();
         }

         if (hit.collider.tag == "Replayer" && !fromReplayer) {
            Debug.Log("REPLAYER KILLED");

            GameManager.Instance.RoundScore += 10;

            Destroy();
            hit.collider.gameObject.SetActive(false);
         }

         if (hit.collider.tag == "Enemy" && !fromReplayer) {
            Debug.Log("TARGET KILLED, RESTARTING...");
            Destroy();
            //Debug.Log("TUTORIAL STEP: " + GameManager.Instance.tutorialStep);

            GameManager.Instance.RoundScore += 30;

            if (GameManager.Instance.tutorialStep < 5)
               GameManager.Instance.CompleteTutorialRound();
            else
               GameManager.Instance.StartRound(true);
			}

      }
	}

}
