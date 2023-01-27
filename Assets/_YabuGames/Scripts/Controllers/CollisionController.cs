using System;
using _YabuGames.Scripts.Interfaces;
using _YabuGames.Scripts.Signals;
using UnityEngine;

namespace _YabuGames.Scripts.Controllers
{
    public class CollisionController : MonoBehaviour
    {

        private JellyController _jellyController;
        private bool _onDrag = false;
        private bool _allyMerge = false;

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
            //JellySignals.Instance.OnDragStart += BlockEnemyMerging;
            JellySignals.Instance.OnAbleToMerge += AllowEnemyMerging;
            // JellySignals.Instance.OnAbleToMerge += BlockAllyMerging;
            // JellySignals.Instance.OnDragEnd += AllowAllyMerging;
        }

        private void UnSubscribe()
        {
            //JellySignals.Instance.OnDragStart -= BlockEnemyMerging;
             JellySignals.Instance.OnAbleToMerge -= AllowEnemyMerging;
            // JellySignals.Instance.OnAbleToMerge -= BlockAllyMerging;
            //JellySignals.Instance.OnDragEnd -= AllowAllyMerging;
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
                    
                    if (other.TryGetComponent(out IInteractable script))
                    {
                        _jellyController.BlockDragging();
                        _jellyController.Merge(script.GetLevel());
                        script.TempMerge();
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
            }
        }
    }
}
