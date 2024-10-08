using System;
using UnityEngine;

public class Billboard : MonoBehaviour {

    private Camera _camera;
    
    private void Awake() {
        _camera = Camera.main;
    }

    public void Update() {
        if (_camera == null) {
            _camera = Camera.main;
        }

        var rotation = _camera.transform.eulerAngles;
        rotation.x = 0;
        rotation.z = 0;
        transform.localRotation = Quaternion.Euler(rotation);
    }
}