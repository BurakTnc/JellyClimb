using System;
using System.Collections;
using System.Collections.Generic;
using _YabuGames.Scripts.Interfaces;
using DG.Tweening;
using UnityEngine;

namespace _YabuGames.Scripts.Controllers
{
    public class EnemyJellyController : MonoBehaviour,IInteractable
    {
        [SerializeField] private int level;

        private Transform _transform;
        private BoxCollider _collider;

        private void Awake()
        {
            _transform = transform;
            _collider = GetComponent<BoxCollider>();
        }

        private void DisAppear()
        {
            Destroy(gameObject);
        }

        // private IEnumerator Latency()
        // {
        //     _collider.enabled = false;
        //     yield return new WaitForSeconds(1);
        //     _collider.enabled = true;
        //
        // }

        #region Public Voids

        public void Merge(int empty,IInteractable empty1)
        {
            
        }

        public void AllyMerge(int empty)
        {
            
        }

        public void TempMerge()
        {
            _transform.DOScale(Vector3.zero, .4f).SetEase(Ease.OutSine).SetDelay(.2f).OnComplete(DisAppear);
        }

        public void DisableCollider()
        {
           // StartCoroutine(Latency());
        }

        public int GetLevel()
        {
            return level;
        }

        #endregion
    }
}