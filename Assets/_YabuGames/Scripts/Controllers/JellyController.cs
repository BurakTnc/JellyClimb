using System;
using _YabuGames.Scripts.Interfaces;
using _YabuGames.Scripts.Managers;
using _YabuGames.Scripts.Signals;
using DG.Tweening;
using Dreamteck.Splines;
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
        private Vector3 _startRotation;
        private Vector3 _startGrid;
        private RubberEffect _rubberEffect;
        private CollisionController _collisionController;
        private GrabController _grabController;
        private JellySplineController _jellySplineController;
        private float _heightValue;
        private int _stepCount = 0;


        private void Awake()
        {
            GetVariables();
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
        
        private void GetVariables()
        {
            _transform = transform;
            _timer += (coolDown + Random.Range(0.1f, 1f));
            _rubberEffect = GetComponentInChildren<RubberEffect>();
            _currentScale = meshParent.localScale;
            _collisionController = GetComponent<CollisionController>();
            _grabController = GetComponent<GrabController>();
            _jellySplineController = GetComponent<JellySplineController>();
            var position = _transform.position;
            _oldPosition = position;
            _startGrid = position;
            _startRotation = _transform.rotation.eulerAngles;
        }
        
        private void SetVariables()
        {
            
        }

        private void StopMoving()
        {
            _onMove = true;
            _rubberEffect.m_EffectIntensity = .6f;
            _onMerge = true;
        }

        private void BeginMoving()
        {
            _transform.DOMove(_oldPosition, .5f).SetEase(Ease.OutSine).OnComplete(MergeDone);
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
            seq.Append(_transform.DOMoveY(_heightValue, .3f).SetEase(Ease.InSine).SetRelative(true));
            seq.Join(meshParent.DOScale(effectScale, .3f).SetEase(Ease.OutBack));
            seq.Append(meshParent.DOScale(_currentScale, .2f).SetEase(Ease.OutBack)).OnComplete(MergeDone);

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
            
        }

        private void MergeDone()
        {
            _collisionController.AllowEnemyMerging();
            PoolManager.Instance.GetSplashParticle(splashPosition.position );
            JellySignals.Instance.OnAbleToMerge?.Invoke();
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

        private void GetOnBand()
        {
            BandController.Instance.GetBand(_jellySplineController);
        }

        private void Climb()
        {
            var currentScale = _transform.localScale;
            var desiredScale = new Vector3(currentScale.x * 1.1f, currentScale.y/1.1f, currentScale.z/1.1f);
            var climbPosition= new Vector3(0, 1, 2);
            Vector3 desiredPosition;
            var bandPosition = new Vector3(0, .2f, 1);
            
            if (_stepCount==6)
            {
                
                return;
            }

            if (_stepCount==5)
            {
                desiredPosition = bandPosition;
            }
            else
            {
                desiredPosition = climbPosition;
            }
            
            _onMove = true;
            _ableToDrag = false;
            _timer += coolDown;
            _stepCount++;

            _transform.DOMove(desiredPosition, .5f).SetRelative(true)
                .SetEase(Ease.InSine);
            mesh.DOLocalRotate(new Vector3(90, 0, 0), .4f,RotateMode.WorldAxisAdd).SetRelative(true)
                .SetEase(Ease.InSine).OnComplete(OnClimbFinish);
            
            if(_onMerge && _stepCount==5) return;
             _transform.DOScale(desiredScale, .25f).SetEase(Ease.InSine);
        }

        private void OnClimbFinish()
        {
            PullParticles();
            var scale = _transform.localScale;
            var desiredScale = new Vector3(scale.x / 1.1f, scale.y*1.1f, scale.z*1.1f);
            _transform.DOScale(desiredScale, .25f).SetEase(Ease.InSine).OnComplete(EnableMovement);

        }

        private void EnableMovement()
        {
            if (_stepCount==6)
            {
                GetOnBand();
                 return;
            }
            _oldPosition = _transform.position;
            _onMove = false;
            _ableToDrag = true;
        }
        
        private void Disappear()
        {
            Destroy(gameObject);
        }

        private void ResetClimb()
        {
            _timer = coolDown;
            _stepCount = 0;
            _onMove = false;
            _grabController.enabled = true;
        }
        private void ResetPosition()
        {
            var scaleOffset = new Vector3(0, _heightValue, -_heightValue*2);
            _transform.DORotate(_startRotation, .5f).SetEase(Ease.InSine);
            _transform.DOJump(_startGrid+scaleOffset, 3, 1, .5f).SetEase(Ease.InSine).OnComplete(ResetClimb);
        }

        #region Public Methods
        
        public void SetOnBand()
        {
            _onMove = true;
            _grabController.enabled = false;
        }

        public void SetOffBand()
        {
            Invoke(nameof(ResetPosition),.1f);
        }
        public void DragEffect()
        {
            PoolManager.Instance.GetSplashParticle(splashPosition.position );
           // _transform.DOScale(_currentScale * 1.3f, .5f).SetEase(Ease.OutBack);
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