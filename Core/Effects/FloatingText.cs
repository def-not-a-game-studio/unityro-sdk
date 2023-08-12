using System;
using System.Collections;
using UnityEngine;
using TMPro;

namespace UnityRO.Core.Effects {
    public class FloatingText : MonoBehaviour {

        [SerializeField]
        private TextMeshPro TextField;

        public void SetText(EntityActionRequest actionRequest, Action<FloatingText> onDestroy) {
            switch (actionRequest.action) {
                    case ActionRequestType.ATTACK_MULTIPLE_NOMOTION:
                    case ActionRequestType.ATTACK:
                        SetText($"{actionRequest.damage}", Color.white, onDestroy);
                        //target.Damage(pkt.damage, GameManager.Tick + pkt.sourceSpeed);
                        break;

                    // double attack
                    case ActionRequestType.ATTACK_MULTIPLE:
                        SetText($"{actionRequest.damage}", Color.white, onDestroy);
                        // Display combo only if entity is mob and the attack don't miss
                        // if (dstEntity.Type == EntityType.MOB && pkt.damage > 0) {
                        //     dstEntity.Damage(pkt.damage / 2, GameManager.Tick + pkt.sourceSpeed * 1, DamageType.COMBO);
                        //     dstEntity.Damage(pkt.damage, GameManager.Tick + pkt.sourceSpeed * 2, DamageType.COMBO | DamageType.COMBO_FINAL);
                        // }

                        // target.Damage(pkt.damage / 2, GameManager.Tick + pkt.sourceSpeed * 1);
                        // target.Damage(pkt.damage / 2, GameManager.Tick + pkt.sourceSpeed * 2);
                        break;

                    // TODO: critical damage
                    case ActionRequestType.ATTACK_CRITICAL:
                        SetText($"{actionRequest.damage}", Color.white, onDestroy);
                        // target.Damage(pkt.damage, GameManager.Tick + pkt.sourceSpeed);
                        break;

                    // TODO: lucky miss
                    case ActionRequestType.ATTACK_LUCKY:
                        SetText($"{actionRequest.damage}", Color.white, onDestroy);
                        // target.Damage(0, GameManager.Tick + pkt.sourceSpeed);
                        break;
                }
        }

        public void SetText(string text, Color? color, Action<FloatingText> onDestroy) {
            TextField.text = text;
            TextField.color = color ?? Color.white;
            transform.localPosition = Vector3.zero;
            
            StartCoroutine(DestroyAfterSeconds(onDestroy));
            StartCoroutine(FadeAfterSeconds());
        }
        private IEnumerator FadeAfterSeconds() {
            yield return new WaitForSeconds(1f);
            var currentTime = 0f;
            var currentColor = TextField.color;

            while (currentTime <= 1f && currentColor.a > 0f) {
                currentTime += Time.deltaTime;
                currentColor.a = Mathf.Lerp(currentColor.a, 0f, currentTime / 1f);
                yield return null;
            }
        }
        private IEnumerator DestroyAfterSeconds(Action<FloatingText> onDestroy) {
            yield return new WaitForSeconds(2.5f);
            onDestroy.Invoke(this);
        }

        private void Update() {
            var localPosition = transform.localPosition;
            localPosition.y += Time.deltaTime * 10f;
            transform.localPosition = localPosition;
        }
    }
}