using System;
using Core.Scene;
using UnityEngine;
using UnityEngine.UI;

public class LoadingSceneController : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public Slider slider;
    public float fadeSpeed;

    private float targetValue;
    
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
    }

    private void OnEnable()
    {
        GameSceneManager.SceneLoadingProgress += OnProgressUpdate;
    }

    private void OnDisable()
    {
        GameSceneManager.SceneLoadingProgress -= OnProgressUpdate;
    }
    
    private void OnProgressUpdate(float progress)
    {
        targetValue = progress;
    }

    private void Update()
    {
        if (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 1f, Time.deltaTime * fadeSpeed);
        }

        if (slider.value != targetValue)
        {
            slider.value = Mathf.Lerp(slider.value, targetValue, Time.deltaTime * fadeSpeed);
        }
    }
}