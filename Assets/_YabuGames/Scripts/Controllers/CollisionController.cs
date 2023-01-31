using System;
using _YabuGames.Scripts.Interfaces;
using _YabuGames.Scripts.Managers;
using _YabuGames.Scripts.Signals;
using UnityEngine;

namespace _YabuGames.Scripts.Controllers
{
    public class CollisionController : MonoBehaviour
    {

        private JellyController _jellyController;
        
        private bool _onDrag = false;
        private bool _allyMerge = false;
        private bool _onFirstMove = true;

        private void Awake()
        {
            _jellyController = GetComponent<JellyController>();
        }

        #region Subscribtions
        private void OnEnable()
        {
            Subscribe();
        }
        
        private void OnDisable()
        {
            UnSubscribe();
        }

        private void Subscribe()
        {
            JellySignals.Instance.OnAbleToMerge += AllowEnemyMerging;
        }

        private void UnSubscribe()
        {
            JellySignals.Instance.OnAbleToMerge -= AllowEnemyMerging;
        }
        #endregion
        

        public void BlockEnemyMerging()
        {
            _onDrag = true;
        }

        public void AllowEnemyMerging()
        {
            _onDrag = false;
        }

        public void AllowAllyMerging()
        {
            _allyMerge = true;
        }

        public void BlockAllyMerging()
        {
            _allyMerge = false;
        }
        

        private void OnTriggerEnter(Collider other)
        {
            switch (other.tag)
            {
                case "EnemyJelly":
                    if(_onDrag) return;
                    
                    if (other.TryGetComponent(out EnemyJellyController script))
                    {
                        script.DisableCollider();
                        _jellyController.BlockDragging();
                        _jellyController.Merge(script.GetLevel(),script);
                    }
                    break;
                
            }
        }

        private void OnTriggerExit(Collider other)
        {
            switch (other.tag)
            {
                case "Grid":
                    if (_onDrag)
                    {
                        //GameManager.Instance.SetGrid(other.transform,false);
                    }
                    break;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            switch (other.tag)
            {
                case "Jelly":
                    if(!_onDrag) return;
                
                    if (other.TryGetComponent(out IInteractable jellyScript))
                    {
                        if(!_allyMerge) return;

                        _allyMerge = false;
                        jellyScript.AllyMerge(_jellyController.GetLevel());
                        _jellyController.TempMerge();
                        
                    }
                    break;
                case "Grid":
                    if (_onDrag || _onFirstMove)
                    {
                        if(!_allyMerge) return;

                        _allyMerge = false;
                        _onFirstMove = false;
                        //GameManager.Instance.SetGrid(other.transform,true);
                        _jellyController.SetIdleGrid(other.transform);
                    }
                    break;
                case "StartGrid":
                    if (_onDrag)
                    {
                        if(!_allyMerge) return;
                        
                        _allyMerge = false;
                        _jellyController.SetStartGrid(other.transform);
                    }
                    break;
            }
        }
    }
}
