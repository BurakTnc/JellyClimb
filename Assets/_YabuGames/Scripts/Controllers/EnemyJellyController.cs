using System;
using _YabuGames.Scripts.Interfaces;
using DG.Tweening;
using UnityEngine;

namespace _YabuGames.Scripts.Controllers
{
    public class EnemyJellyController : MonoBehaviour,IInteractable
    {
        [SerializeField] private int level;

        private Transform _transform;

        private void Awake()
        {
            _transform = transform;
        }

        private void Start()
        {
            IdleShake();
        }

        private void IdleShake()
        {
            _transform.DOShakeScale(2f, Vector3.one * .1f, 1, 100, false)
                .SetLoops(-1, LoopType.Yoyo);
        }
        private void DisAppear()
        {
            Destroy(gameObject);
        }

        #region Public Voids

        public void Merge(int empty)
        {
            
        }

        public void TempMerge()
        {
            _transform.DOScale(Vector3.zero, .4f).SetEase(Ease.OutSine).SetDelay(.2f).OnComplete(DisAppear);
        }

        public int GetLevel()
        {
            return level;
        }

        #endregion
    }
}