using System;
using _YabuGames.Scripts.Interfaces;
using _YabuGames.Scripts.Managers;
using _YabuGames.Scripts.Signals;
using DG.Tweening;
using Dreamteck.Splines;
using JellyCube;
using TMPro;
using UnityEditor;
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
        [SerializeField] private Material[] materials;
        [SerializeField] private TextMeshPro levelText;
        
        private bool _onMove;
        private bool _onMerge;
        private bool _ableToDrag = true;
        private bool _isBlocked = false;
        private bool _isStarted = false;

        private Transform _transform;
        private Transform _currentGrid;
        private Transform _currentStartGrid;
        
        private float _timer;
        private float _heightValue;
        
        private int _level = 1;
        private int _stepCount = 0;
        private int _stepLimit;

        private Vector3 _currentScale;
        private Vector3 _oldPosition;
        private Vector3 _startRotation;
        private Vector3 _startGrid;
        
        private RubberEffect _rubberEffect;
        private CollisionController _collisionController;
        private GrabController _grabController;
        private JellySplineController _jellySplineController;

        private BoxCollider _collider;
        
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
            JellySignals.Instance.OnStairUp += SetStepLimit;
        }

        private void UnSubscribe()
        {
            JellySignals.Instance.OnDragStart -= StopMoving;
            JellySignals.Instance.OnDragEnd -= BeginMoving;
            JellySignals.Instance.OnStairUp -= SetStepLimit;
        }
        #endregion

        private void Start()
        {
            SetVariables();
        }

        private void Update()
        {
            if(!_isStarted) return;
            CheckInput();
        }
        
        private void GetVariables()
        {
            _transform = transform;
            _collider = GetComponent<BoxCollider>();
            //_timer += (coolDown + Random.Range(0.1f, 1f)); //???
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
            _stepLimit = GameManager.Instance.stepLimit;
            _onMove = true;
            mesh.GetComponent<MeshRenderer>().material.color = materials[_level-1].color;
            levelText.text = _level.ToString();
        }

        private void SetMaterialAndLevel()
        {
            levelText.text = _level.ToString();
            mesh.GetComponent<MeshRenderer>().material.DOColor(materials[_level - 1].color, 1.5f).SetEase(Ease.OutBack);

        }
            
    

        private void StopMoving()
        {
            _onMove = true;
            _rubberEffect.m_EffectIntensity = .6f;
            _onMerge = true;
        }

        private void BeginMoving()
        {
            Invoke(nameof(MergeDone),.3f);
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

        public void Merge(int takenLevel,IInteractable script)
        {
            if(_isBlocked) return;
            
            if (takenLevel>_level)
            {
                _collider.enabled = false;
                _isBlocked = true;
                ResetClimb();
                ResetPosition();
            }
            else
            {
                script.TempMerge(takenLevel);
                GameManager.Instance.ClimbedStairs += 5;
                // if (_level >= maxLevel)  return;
                // _ableToDrag = false;
                // var seq = DOTween.Sequence();
                // _onMerge = true;
                // _level += takenLevel;
                // _heightValue = growingSize.x * takenLevel;
                // var mergedScale = _currentScale + (growingSize * takenLevel);
                // var effectScale = new Vector3(mergedScale.x * 1.1f, mergedScale.y*2, mergedScale.z);
                // _currentScale = mergedScale;
                // seq.Append(_transform.DOMoveY(_heightValue, .3f).SetEase(Ease.InSine).SetRelative(true));
                // seq.Join(meshParent.DOScale(effectScale, .3f).SetEase(Ease.OutBack));
                // seq.Append(meshParent.DOScale(_currentScale, .2f).SetEase(Ease.OutBack)).OnComplete(MergeDone);
            }

            
        }

        public void AllyMerge( int takenLevel,IInteractable script)
        {
            if (_level >= 7 || _level!=takenLevel)
                return;
            script.TempMerge(takenLevel);
            _ableToDrag = false;
            var seq = DOTween.Sequence();
            _onMerge = true;
            _level ++;
            HapticManager.Instance.PlayWarningHaptic();
            SetMaterialAndLevel();
            _heightValue += growingSize.x/2;
            var mergedScale = _currentScale + growingSize;
            var meshScale = meshParent.localScale;
            var effectScale = new Vector3(meshScale.x*1.1f, meshScale.y * 3.5f, meshScale.z);
            _currentScale = mergedScale;
            seq.Append(_transform.DOMoveY(_heightValue, .3f).SetEase(Ease.InSine).SetRelative(true));
            seq.Join(meshParent.DOScale(effectScale, .3f).SetEase(Ease.OutBack));
            seq.Append(meshParent.DOScale(_currentScale, .2f).SetEase(Ease.OutBack)).OnComplete(MergeDone);

        }

        private void MergeDone()
        {
            _collisionController.AllowEnemyMerging();
            var splash = Instantiate(Resources.Load<GameObject>($"Particles/splash/Splash{_level}"));
            splash.transform.position = splashPosition.position;
            JellySignals.Instance.OnAbleToMerge?.Invoke();
            _timer += .35f;
            _onMerge = false;
            _ableToDrag = true;
            _onMove = false;
            _rubberEffect.m_EffectIntensity = 1f;
        }

        private void PullParticles()
        {
            
            var splash = Instantiate(Resources.Load<GameObject>($"Particles/splash/Splash{_level}"));
            splash.transform.position = splashPosition.position;
            PoolManager.Instance.GetIncomeTextParticle(transform.position+new Vector3(.7f,0,-1),_level);
            if (_stepCount != _stepLimit + 1)
            {
                var groundSplash = Instantiate(Resources.Load<GameObject>($"Particles/ground/ground{_level}"));
                groundSplash.transform.position = groundSplashPosition.position;
            }
                
        }

        private void GetOnBand()
        {
            BandController.Instance.GetBand(_jellySplineController);
        }

        private void SetStepLimit(int limit)
        {
            _stepLimit = limit;
        }
        private void Climb()
        {
            CoreGameSignals.Instance.OnSave?.Invoke();
            if (GameManager.Instance.ClimbedStairs<100)
            {
                GameManager.Instance.ClimbedStairs++;
            }
            
            var currentScale = _transform.localScale;
            var desiredScale = new Vector3(currentScale.x * 1.1f, currentScale.y/1.1f, currentScale.z/1.1f);
            var climbPosition= new Vector3(0, 1, 2);
            var bandPosition = new Vector3(0, .2f, 2);
            
            Vector3 desiredPosition;
            if (_stepCount == _stepLimit + 1) 
            {
                return;
            }

            if (_stepCount==_stepLimit)
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
            GameManager.Money += _level;
            var scale = _transform.localScale;
            var desiredScale = new Vector3(scale.x / 1.1f, scale.y*1.1f, scale.z*1.1f);
            _transform.DOScale(desiredScale, .25f).SetEase(Ease.InSine).OnComplete(EnableMovement);

        }

        private void EnableMovement()
        {
            if (_stepCount==_stepLimit+1)
            {
                GetOnBand();
                 return;
            }
            _oldPosition = _transform.position;
            _onMove = false;
            _ableToDrag = true;
            _isStarted = true;
        }
        
        private void Disappear()
        {
            Destroy(gameObject);
        }

        private void ResetClimb()
        {
            var splash = Instantiate(Resources.Load<GameObject>($"Particles/splash/Splash{_level}"));
            splash.transform.position = splashPosition.position;
            _rubberEffect.m_EffectIntensity = 1;
            _timer = coolDown;
            _stepCount = 0;
            _onMove = false;
            _grabController.enabled = true;
            _collider.enabled = true;
        }
        private void ResetPosition()
        {
            var scaleOffset = new Vector3(0, _heightValue, 0);
            _oldPosition = _startGrid + scaleOffset;
            _transform.DORotate(_startRotation, .5f).SetEase(Ease.InSine);
            _transform.DOJump(_startGrid+scaleOffset, 3, 1, .5f).SetEase(Ease.InSine).OnComplete(ResetClimb);
            if (_isBlocked)
            {
                mesh.DORotate(new Vector3(-180,_startRotation.y,_startRotation.z), 1f);
            }
            else
            {
                _transform.DORotate(_startRotation, .5f).SetEase(Ease.InSine);
            }
            _isBlocked = false;
        }

        #region Public Methods
        
        public void SetIdleGrid(Transform grid)
        {
            if (_currentGrid)
            {
                GameManager.Instance.SetGrid(_currentGrid,false);
            }

            if (_currentStartGrid)
            {
                _currentStartGrid.GetComponent<BoxCollider>().enabled = true;
            }
            _currentGrid = grid;
            GameManager.Instance.SetGrid(_currentGrid,true);
            _stepCount = 0;
            var position = grid.position;
            position += new Vector3(0, _currentScale.x, _currentScale.x/1.5f);
            _transform.position = position;
            _startGrid = position;
            _oldPosition = position;
            _isStarted = false;
        }
        public void SetStartGrid(Transform grid)
        {
            GameManager.Instance.SetGrid(_currentGrid,false);
            if (_currentStartGrid)
            {
                _currentStartGrid.GetComponent<BoxCollider>().enabled = true;
            }
            _currentStartGrid = grid;
            _currentStartGrid.GetComponent<BoxCollider>().enabled = false;
            _timer = 0;
            _stepCount = 0;
            var position = grid.position;
            position += new Vector3(0, _currentScale.x, .5f);
            _transform.position = position;
            _startGrid = position;
            _oldPosition = position;
            EnableMovement();
        }
        public void GoToPrevPosition()
        {
            _transform.DOMove(_oldPosition, .5f).SetEase(Ease.OutSine).OnComplete(MergeDone);
        }
        public void SetOnBand()
        {
            _onMove = true;
            _grabController.enabled = false;
        }

        public void SetOffBand()
        {
            ResetPosition();
        }
        public void DragEffect()
        {
            var splash = Instantiate(Resources.Load<GameObject>($"Particles/splash/Splash{_level}"));
            splash.transform.position = splashPosition.position;
        }

        public void FinishDragEffect()
        {
            _transform.DOScale(_currentScale, .4f).SetEase(Ease.OutBack);
        }
        public void BlockDragging()
        {
            _ableToDrag = false;
        }
        public void TempMerge(int takenLevel)
        {
            
            
            if (_currentStartGrid)
            {
                _currentStartGrid.GetComponent<BoxCollider>().enabled = true;
            }
            GameManager.Instance.JellyCount--;
            GameManager.Instance.SetGrid(_currentGrid,false);
            CoreGameSignals.Instance.OnSave?.Invoke();
            transform.DOKill();
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