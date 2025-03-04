using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;

namespace UnityRO.Core.Effects
{
    public class FloatingText : MonoBehaviour
    {
        [SerializeField] private TextMeshPro TextField;

        private int delayMs = 0;

        public void SetText(string text, Color? color, Action<FloatingText> onDestroy)
        {
            TextField.text = text;
            TextField.color = color ?? Color.white;
            transform.localPosition = Vector3.zero;

            StartCoroutine(DestroyAfterSeconds(onDestroy));
            StartCoroutine(FadeAfterSeconds());
        }

        public async void SetText(string text, Color? color, Action<FloatingText> onDestroy, float delay = 0f)
        {
            delayMs = (int)delay;
            await Task.Delay(delayMs);
            delayMs = 0;

            TextField.text = text;
            TextField.color = color ?? Color.white;
            transform.localPosition = new Vector3(0, 3, 0);

            StartCoroutine(DestroyAfterSeconds(onDestroy));
            StartCoroutine(FadeAfterSeconds());
        }

        private IEnumerator FadeAfterSeconds()
        {
            yield return new WaitForSeconds(1f);
            var currentTime = 0f;
            var currentColor = TextField.color;

            while (currentTime <= 1f && currentColor.a > 0f)
            {
                currentTime += Time.deltaTime;
                currentColor.a = Mathf.Lerp(currentColor.a, 0f, currentTime / 1f);
                yield return null;
            }
        }

        private IEnumerator DestroyAfterSeconds(Action<FloatingText> onDestroy)
        {
            yield return new WaitForSeconds(2.5f);
            onDestroy.Invoke(this);
        }

        private void Update()
        {
            if (delayMs > 0)
                return;

            var localPosition = transform.localPosition;
            localPosition.y += Time.deltaTime * 10f;
            transform.localPosition = localPosition;
        }
    }
}