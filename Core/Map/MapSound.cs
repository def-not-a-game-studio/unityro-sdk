using System;
using ROIO.Models.FileTypes;
using UnityEngine;

[Serializable]
public class MapSound : MonoBehaviour {
    [SerializeField] public AudioSource source;
    [SerializeField] public float playAt;
    [SerializeField] public RSW.Sound info;

    private void Awake() {
        source = GetComponent<AudioSource>();
    }
}