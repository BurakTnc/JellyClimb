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

        private void Awake()
        {
            _jellyController = GetComponent<JellyController>();
            _collisionController = GetComponent<CollisionController>();
        }

        private void EndDrag()
        {
            
        }

        private void OnMouseUp()
        {
            JellySignals.Instance.OnDragEnd?.Invoke();
            _jellyController.FinishDragEffect();
        }

        private void OnMouseDown()
        {
            _collisionController.BlockEnemyMerging();
            _jellyController.DragEffect();
            JellySignals.Instance.OnDragStart?.Invoke();
            if (Camera.main != null)
                _difference = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
        }

        private void OnMouseDrag()
        {
            if (!_jellyController.CanDrag()) return;
            
            if (Camera.main != null)
            {
                var targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition - _difference);
                targetPosition = new Vector3(targetPosition.x, targetPosition.y+1, targetPosition.y*2);
                targetPosition.y = Mathf.Clamp(targetPosition.y, 1, 100);
                targetPosition.z = Mathf.Clamp(targetPosition.z, -7, 100);
                transform.position = targetPosition;
            }

            
        }
    }
}