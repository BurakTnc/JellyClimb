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

        private void Awake()
        {
            _jellyController = GetComponent<JellyController>();
        }

        #region Subscribtions
        // private void OnEnable()
        // {
        //     Subscribe();
        // }
        //
        // private void OnDisable()
        // {
        //     UnSubscribe();
        // }

        private void Subscribe()
        {
            JellySignals.Instance.OnDragStart += BlockEnemyMerging;
            JellySignals.Instance.OnAbleToMerge += AllowEnemyMerging;
        }

        private void UnSubscribe()
        {
            JellySignals.Instance.OnDragStart -= BlockEnemyMerging;
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


        private void OnCollisionEnter(Collision collision)
        {
            switch (collision.transform.tag)
            {
                default:
                    break;
            }
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
                // case "Jelly":
                //     if(!_onDrag) return;
                //
                //     if (other.TryGetComponent(out IInteractable jellyScript))
                //     {
                //         _jellyController.TempMerge();
                //         jellyScript.Merge(_jellyController.GetLevel());
                //     }
                //     break;
            }
        }
    }
}
