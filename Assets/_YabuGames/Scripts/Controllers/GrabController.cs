using System;
using _YabuGames.Scripts.Signals;
using UnityEngine;

namespace _YabuGames.Scripts.Controllers
{
    public class GrabController : MonoBehaviour
    {
        private Vector3 _difference;
        private Vector3 _destination;
        private JellyController _jellyController;
        private CollisionController _collisionController;
        private bool _isGrabbing = false;

        private void Awake()
        {
            _jellyController = GetComponent<JellyController>();
            _collisionController = GetComponent<CollisionController>();
        }

        private void OnEnable()
        {
            CoreGameSignals.Instance.OnDragging += DragSituation;
        }

        private void OnDisable()
        {
            CoreGameSignals.Instance.OnDragging -= DragSituation;
        }

        private void DragSituation(bool grabbing)
        {
            _isGrabbing = grabbing;
        }

        private void MoveDelay()
        {
            _jellyController.GoToPrevPosition();
        }

        private void OnMouseUp()
        {
            if(!_isGrabbing) return;
            CoreGameSignals.Instance.OnDragging?.Invoke(false);
            _collisionController.AllowAllyMerging();
            JellySignals.Instance.OnDragEnd?.Invoke();
            Invoke(nameof(MoveDelay), .1f);
            _jellyController.FinishDragEffect();
        }

        private void OnMouseDown()
        {
            if(_isGrabbing) return;
            CoreGameSignals.Instance.OnDragging?.Invoke(true);
            _collisionController.BlockAllyMerging();
            _collisionController.BlockEnemyMerging();
            _jellyController.DragEffect();
            JellySignals.Instance.OnDragStart?.Invoke();
            if (Camera.main != null)
                _difference = (Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position)+Vector3.back*.5f);
        }

        private void OnMouseDrag()
        {
            if (!_jellyController.CanDrag()) return;
            
            if (Camera.main != null)
            {
                var targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition - _difference);
                targetPosition = new Vector3(targetPosition.x, targetPosition.y+1, targetPosition.y*2);
                targetPosition.y = Mathf.Clamp(targetPosition.y, 0, 100);
                targetPosition.z = Mathf.Clamp(targetPosition.z, -7, 100);
                transform.position = targetPosition;
            }

            
        }
    }
}