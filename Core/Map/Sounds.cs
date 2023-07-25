using System.Collections.Generic;
using System.IO;
using ROIO;
using ROIO.Models.FileTypes;
using UnityEngine;
using UnityEngine.Audio;

public class Sounds {
    public List<MapSound> playing;
    private GameObject _parent = null;
    private AudioMixerGroup _mixer;

    public Sounds() {
        playing = new List<MapSound>();
    }

    public void Clear() {
        _parent = null;
        foreach (MapSound p in playing) {
            FileCache.Remove(p.info.file);
            //GameObject.Destroy(p.clip);
        }

        playing.Clear();
    }

    public int Count() {
        return playing.Count;
    }

    public void Add(RSW.Sound sound, GameObject parent) {
        if (_parent == null) {
            _parent = new GameObject("_sounds");
            _parent.transform.parent = GameObject.FindObjectOfType<GameMap>().transform;
            _mixer = GameObject.FindObjectOfType<GameManager>().AudioMixerGroup;
        }

        var clip = Resources.Load<AudioClip>(Path.Combine("Wav", sound.file.Replace(".wav", "")));

        var p = new GameObject(sound.file + "[" + sound.name + "]" + sound.cycle).AddComponent<MapSound>();
        p.playAt = 0;
        p.info = sound;

        p.gameObject.transform.parent = _parent.transform;
        p.source = p.gameObject.AddComponent<AudioSource>();
        p.source.transform.position = new Vector3(sound.pos[0], sound.pos[1], sound.pos[2]);

        p.source.clip = clip;
        p.source.loop = false;
        p.source.playOnAwake = false;
        p.source.volume = Mathf.Clamp(sound.vol, 0, 1);
        p.source.spatialBlend = 1;
        p.source.rolloffMode = AudioRolloffMode.Linear;
        p.source.spatialize = true;
        p.source.outputAudioMixerGroup = _mixer;
        p.source.dopplerLevel = 0;
        p.source.minDistance = p.info.width;
        p.source.maxDistance = sound.range + sound.height;

        playing.Add(p);
    }

    public void Update() {
        float now = Time.realtimeSinceStartup;

        foreach (MapSound p in playing) {
            if (p.playAt <= now && p.source != null) {
                p.source.Play();
                p.playAt = now + p.info.cycle;
            }
        }
    }
}