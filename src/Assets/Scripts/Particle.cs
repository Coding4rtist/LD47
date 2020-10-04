using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : PoolObject {
   private Animator anim;
   private SpriteRenderer sr;

   private void Awake() {
      anim = GetComponent<Animator>();
      sr = GetComponent<SpriteRenderer>();
   }

   //public void Play(string animation) {
   //   StartCoroutine(OnAnimationEnd(animation));
   //}

   public void Play() {
      //sr.flipX = direction.x < 0;
      StartCoroutine(OnAnimationEnd());
      anim.Play("walkpuff");
   }

   private IEnumerator OnAnimationEnd() {
      yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
      Destroy();
   }
}
