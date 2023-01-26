using DG.Tweening;
using UnityEngine;

namespace _YabuGames.Scripts.Controllers
{
    public class JellyController : MonoBehaviour
    {
     [SerializeField] private float coolDown;
        
        private bool _onMove = false;
        private bool _onMerge = false;
        private Transform _transform;
        private float _timer = 0;
        private Transform _mesh;
        private void Awake()
        {
            _transform = transform;
            _mesh = _transform.GetChild(0);
        }
        private void Start()
        {
        
        }

        private void Update()
        {
            CheckInput();
        }

        public Vector3 GetScale()
        {
            return _transform.localScale;
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

        private void MergeProcess(Vector3 scale)
        {
            Sequence seq = DOTween.Sequence();
            _onMerge = true;
            var mergedScale = _transform.localScale + scale;
            var effectScale = new Vector3(mergedScale.x, mergedScale.y * 1.5f, mergedScale.z * 1.5f);
            seq.Append(_transform.DOScale(effectScale, .5f).SetEase(Ease.OutBack));
            seq.Append(_transform.DOScale(mergedScale, .3f).SetEase(Ease.OutSine).SetDelay(.2f)
                .OnComplete(MergeDone));
            
            
            
            
           // _transform.DOScale(effectScale, .2f).SetEase(Ease.OutBack).OnComplete(() => MergeDone());


        }

        private void MergeEffect(Vector3 mergedScale)
        {
            Debug.Log(mergedScale);
            _transform.DOScale(mergedScale, .2f).SetEase(Ease.InBack)
                .OnComplete(MergeDone).SetDelay(.2f);
        }

        private void MergeDone()
        {
            _timer += .5f;
            _onMerge = false;
        }

        private void Climb()
        {
            _onMove = true;
            _timer += coolDown;
            var currentScale = _transform.localScale;
            var desiredScale = new Vector3(currentScale.x * 1.1f, currentScale.y/1.1f, currentScale.z/1.1f);
            var desiredPosition = new Vector3(0, 1,2);
            
            
            _transform.DOMove(desiredPosition, .5f, false).SetRelative(true)
                .SetEase(Ease.InSine);
            _mesh.DOLocalRotate(new Vector3(90, 0, 0), .4f,RotateMode.WorldAxisAdd).SetRelative(true)
                .SetEase(Ease.InSine).OnComplete(OnClimbFinish);
            
            if(_onMerge) return;
             _transform.DOScale(desiredScale, .25f).SetEase(Ease.InSine);
        }

        private void OnClimbFinish()
        {
            var currentScale = _transform.localScale;
            var desiredScale = new Vector3(currentScale.x / 1.1f, currentScale.y*1.1f, currentScale.z*1.1f);
            _transform.DOScale(desiredScale, .25f).SetEase(Ease.InSine).OnComplete(EnableMovement);

        }

        private void EnableMovement()
        {
            _onMove = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Finish"))
            {
                var script = other.GetComponent<EnemyJellyController>();
                MergeProcess(script.GetScale());
                script.Merge();
                
            }
        }
    }
}