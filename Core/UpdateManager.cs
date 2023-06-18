using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityRO.Core {
    public abstract class ManagedMonoBehaviour : MonoBehaviour {
        private void OnEnable() {
            UpdateManager.Add(this);
        }

        private void OnDisable() {
            UpdateManager.Remove(this);
        }

        public abstract void ManagedUpdate();
    }

    public static class UpdateManager {
        static HashSet<ManagedMonoBehaviour> _updateables = new();
        static ManagedMonoBehaviour[] _copyUpdateables;
        static UpdateManagerInnerMonoBehaviour _innerMonoBehaviour;

        static UpdateManager() {
            var gameObject = new GameObject();
            _innerMonoBehaviour = gameObject.AddComponent<UpdateManagerInnerMonoBehaviour>();

#if UNITY_EDITOR
            gameObject.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
            _innerMonoBehaviour.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
#endif
        }

        public static void Add(ManagedMonoBehaviour managedMonoBehaviour) {
            lock (_updateables) {
                _updateables.Add(managedMonoBehaviour);
                Array.Resize(ref _copyUpdateables, _updateables.Count);
            }
        }

        public static void Remove(ManagedMonoBehaviour managedMonoBehaviour) {
            lock (_updateables) {
                _updateables.Remove(managedMonoBehaviour);
                Array.Resize(ref _copyUpdateables, _updateables.Count);
            }
        }

        class UpdateManagerInnerMonoBehaviour : MonoBehaviour {
            private void Awake() {
                DontDestroyOnLoad(this);
            }

            private void Update() {
                lock (_updateables) {
                    _updateables.CopyTo(_copyUpdateables);
                }

                for (int i = 0; i < _copyUpdateables.Length; i++) {
                    _copyUpdateables[i]?.ManagedUpdate();
                }
            }
        }
    }
}