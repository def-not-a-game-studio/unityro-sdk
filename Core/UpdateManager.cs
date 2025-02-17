using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityRO.Core
{
    public abstract class ManagedMonoBehaviour : MonoBehaviour
    {
        private void OnEnable()
        {
            UpdateManager.Add(this);
        }

        private void OnDisable()
        {
            UpdateManager.Remove(this);
        }

        public abstract void ManagedUpdate();
    }

    public static class UpdateManager
    {
        static HashSet<ManagedMonoBehaviour> _updateables = new();
        static HashSet<ManagedMonoBehaviour> _copyUpdateables;
        static UpdateManagerInnerMonoBehaviour _innerMonoBehaviour;

        static UpdateManager()
        {
            var gameObject = new GameObject();
            _innerMonoBehaviour = gameObject.AddComponent<UpdateManagerInnerMonoBehaviour>();

#if UNITY_EDITOR
            gameObject.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
            _innerMonoBehaviour.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
#endif
        }

        public static void Add(ManagedMonoBehaviour managedMonoBehaviour)
        {
            _updateables.Add(managedMonoBehaviour);
        }

        public static void Remove(ManagedMonoBehaviour managedMonoBehaviour)
        {
            _updateables.Remove(managedMonoBehaviour);
        }

        class UpdateManagerInnerMonoBehaviour : MonoBehaviour
        {
            private void Awake()
            {
                DontDestroyOnLoad(this);
            }

            private void Update()
            {
                _copyUpdateables = new HashSet<ManagedMonoBehaviour>(_updateables);

                foreach (var updateable in _copyUpdateables)
                {
                    updateable.ManagedUpdate();
                }
            }
        }
    }
}