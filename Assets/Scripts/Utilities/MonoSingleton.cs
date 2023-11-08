using System;
using UnityEngine;

namespace Utilities
{
    public class MonoSingleton<T> : MonoBehaviour where T: MonoSingleton<T>
    {
        private static T _instance;
        private static bool _isInitialized;

        public static T Instance
        {
            get
            {
                if (_instance != null) return _instance;

                _instance = FindObjectOfType<T>();
                _instance.Init();
                return _instance;
            }
        }

        public virtual void Init()
        {
            if (_isInitialized) return;
            _isInitialized = true;
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
            } else if (_instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            Init();
        }
    }
}