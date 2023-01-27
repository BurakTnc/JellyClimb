using System;
using _YabuGames.Scripts.Interfaces;
using _YabuGames.Scripts.Managers;
using _YabuGames.Scripts.Signals;
using DG.Tweening;
using JellyCube;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _YabuGames.Scripts.Controllers
{
    public class JellyController : MonoBehaviour,IInteractable
    {
        [SerializeField] private Transform mesh, meshParent;
        [SerializeField] private float coolDown;
        [SerializeField] private Vector3 growingSize;
        [SerializeField] private int maxLevel = 10;
        [SerializeField] private Transform splashPosition, groundSplashPosition;
        
        private bool _onMove;
        private bool _onMerge;
        private bool _ableToDrag = true;
        private Transform _transform;
        private float _timer;
        private int _level = 1;
        private Vector3 _currentScale;
        private Vector3 _oldPosition;
        private Vector3 _particlePosition;
        private RubberEffect _rubberEffect;
        private CollisionController _collisionController;
        private float _heightValue;
        
        
        private void Awake()
        {
            SetVariables();
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
            JellySignals.Instance.OnDragStart += StopMoving;
            JellySignals.Instance.OnDragEnd += BeginMoving;
        }

        private void UnSubscribe()
        {
            JellySignals.Instance.OnDragStart += StopMoving;
            JellySignals.Instance.OnDragEnd += BeginMoving;
        }
        #endregion

        private void Start()
        {
        
        }

        private void Update()
        {
            CheckInput();
        }
        
        private void SetVariables()
        {
            _transform = transform;
            // _meshParent = _transform.GetChild(0);
            // _mesh=_transform.GetChild(0).GetChild(0);
            _timer += (coolDown + Random.Range(0.1f, 1f));
            _rubberEffect = GetComponentInChildren<RubberEffect>();
            _currentScale = meshParent.localScale;
            _collisionController = GetComponent<CollisionController>();
        }
        
        private void GetVariables()
        {
            
        }

        private void StopMoving()
        {
            _onMove = true;
            _rubberEffect.m_EffectIntensity = .5f;
            _onMerge = true;
        }

        private void BeginMoving()
        {
            transform.DOMove(_oldPosition, .5f).SetEase(Ease.OutSine).OnComplete(MergeDone);
        }

        private void CheckInput()
        {
            if (_timer<=0 && !_onMerge)
            {
                if(_onMove) return;
                Climb();
            }

            _timer -= Time.deltaTime;
            _timer = Mathf.Clamp(_timer,0,2);
        }

        public void Merge(int takenLevel)
        {
            if (_level >= maxLevel)  return;
            _ableToDrag = false;
            var seq = DOTween.Sequence();
            _onMerge = true;
            _level += takenLevel;
            _heightValue += growingSize.x * takenLevel;
            var mergedScale = _currentScale + (growingSize * takenLevel);
            var effectScale = new Vector3(mergedScale.x * 1.1f, mergedScale.y*2, mergedScale.z);
            _currentScale = mergedScale;
            seq.Append(meshParent.DOScale(effectScale, .5f).SetEase(Ease.OutBack));
            seq.Append(meshParent.DOScale(_currentScale, .5f).SetEase(Ease.OutBack)).OnComplete(MergeDone);
            seq.Join(_transform.DOMoveY(_heightValue, .3f).SetEase(Ease.InSine).SetRelative(true));
            // seq.Append(_transform.DOScale(mergedScale, .3f).SetEase(Ease.OutSine).SetDelay(.2f)
            //     .OnComplete(MergeDone));

        }

        public void AllyMerge(int takenLevel)
        {
            if (_level >= maxLevel)  return;
            
            _ableToDrag = false;
            var seq = DOTween.Sequence();
            _onMerge = true;
            _level += takenLevel;
            _heightValue += growingSize.x * takenLevel;
            var mergedScale = _currentScale + (growingSize * takenLevel);
            var meshScale = meshParent.localScale;
            var effectScale = new Vector3(meshScale.x*1.1f, meshScale.y * 3f, meshScale.z);
            _currentScale = mergedScale;
            seq.Append(_transform.DOMoveY(_heightValue, .3f).SetEase(Ease.InSine).SetRelative(true));
            seq.Join(meshParent.DOScale(effectScale, .3f).SetEase(Ease.OutBack));
            seq.Append(meshParent.DOScale(_currentScale, .2f).SetEase(Ease.OutBack)).OnComplete(MergeDone);
            
            // seq.Join(_transform.DOScale(mergedScale, .3f).SetEase(Ease.OutBack)
            //     .OnComplete(MergeDone));
        }

        private void MergeDone()
        {
            _collisionController.AllowEnemyMerging();
            PoolManager.Instance.GetSplashParticle(splashPosition.position );
            JellySignals.Instance.OnAbleToMerge?.Invoke();
            
            //_transform.position += Vector3.up * _heightValue;
            _timer += .5f;
            _onMerge = false;
            _ableToDrag = true;
            _onMove = false;
            _rubberEffect.m_EffectIntensity = 1f;
        }

        private void PullParticles()
        {
            PoolManager.Instance.GetSplashParticle(splashPosition.position);
            PoolManager.Instance.GetGroundSplashParticle(groundSplashPosition.position);
        }

        private void Climb()
        {
            _onMove = true;
            _ableToDrag = false;
            _timer += coolDown;
            
            var currentScale = _transform.localScale;
            var desiredScale = new Vector3(currentScale.x * 1.1f, currentScale.y/1.1f, currentScale.z/1.1f);
            var desiredPosition = new Vector3(0, 1, 2);

            _transform.DOMove(desiredPosition, .5f).SetRelative(true)
                .SetEase(Ease.InSine);
            mesh.DOLocalRotate(new Vector3(90, 0, 0), .4f,RotateMode.WorldAxisAdd).SetRelative(true)
                .SetEase(Ease.InSine).OnComplete(OnClimbFinish);
            
            if(_onMerge) return;
             _transform.DOScale(desiredScale, .25f).SetEase(Ease.InSine);
        }

        private void OnClimbFinish()
        {
            _particlePosition = _transform.position;
            PullParticles();
            var scale = _transform.localScale;
            var desiredScale = new Vector3(scale.x / 1.1f, scale.y*1.1f, scale.z*1.1f);
            _transform.DOScale(desiredScale, .25f).SetEase(Ease.InSine).OnComplete(EnableMovement);

        }

        private void EnableMovement()
        {
            _oldPosition = _transform.position;
            _onMove = false;
            _ableToDrag = true;
        }
        
        private void Disappear()
        {
            Destroy(gameObject);
        }

        #region Public Methods
        
        public void DragEffect()
        {
            _transform.DOScale(_currentScale * 1.3f, .5f).SetEase(Ease.OutBack);
        }

        public void FinishDragEffect()
        {
            _transform.DOScale(_currentScale, .4f).SetEase(Ease.OutBack);
        }
        public void BlockDragging()
        {
            _ableToDrag = false;
        }
        public void TempMerge()
        {
            Destroy(gameObject);
            //_transform.DOScale(Vector3.zero, .4f).SetEase(Ease.OutSine).SetDelay(.2f).OnComplete(Disappear);
        }
        public int GetLevel()
        {
            return _level;
        }

        public bool CanDrag()
        {
            return _ableToDrag;
        }
        #endregion
    }
}