using System;
using _YabuGames.Scripts.Controllers;
using UnityEngine;
using UnityEngine.Events;

namespace _YabuGames.Scripts.Signals
{
    public class JellySignals : MonoBehaviour
    {
        #region Singleton
        public static JellySignals Instance;
        private void Awake()
        {
            if (Instance != null && Instance != this) 
            {
                Destroy(this);
                return;
            }
            Instance = this;
        }
        #endregion

        public UnityAction OnDragStart = delegate { };
        public UnityAction OnDragEnd = delegate { };
        public UnityAction OnAbleToMerge = delegate { };
        public UnityAction<int> OnStairUp = delegate { };
    }
}