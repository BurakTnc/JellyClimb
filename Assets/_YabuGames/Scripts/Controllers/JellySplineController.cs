using System;
using Dreamteck.Splines;
using Unity.VisualScripting;
using UnityEngine;

namespace _YabuGames.Scripts.Controllers
{
    
    public class JellySplineController: MonoBehaviour
    {
        public static JellySplineController Instance;
        
        private SplineFollower _splineFollower;
        private JellyController _jellyController;
        public FollowerSpeedModifier.SpeedKey key;

        private void Awake()
        {
            //_splineFollower = GetComponent<SplineFollower>();
            _jellyController = GetComponent<JellyController>();
        }
        
        
        public void SetOnBand(SplineComputer spline)
        {
            SplineSetup(spline);
        }

        private void SplineSetup(SplineComputer spline)
        {
            _splineFollower = transform.AddComponent<SplineFollower>();
            _splineFollower.enabled = false;
            _splineFollower.spline = spline;
            _splineFollower.buildOnAwake = true;
            _splineFollower.buildOnEnable = true;
            _splineFollower.autoStartPosition = true;
            _splineFollower.motion.offset = new Vector2(-.35f, .85f);
            _splineFollower.motion.retainLocalRotation = true;
            _splineFollower.onEndReached += SetOffBand;
            _splineFollower.followSpeed = 5;
            _splineFollower.enabled = true;
            _splineFollower.follow = true;
            _jellyController.SetOnBand();
            _splineFollower.speedModifier.keys.Add(key);
        }


        public void SetOffBand(double empty)
        {
            var temp = transform.GetComponent<SplineFollower>();
            _splineFollower.follow = false;
            _splineFollower.enabled = false;
            _splineFollower.spline = null;
            _jellyController.SetOffBand();
            Destroy(temp);
        }
    }
}