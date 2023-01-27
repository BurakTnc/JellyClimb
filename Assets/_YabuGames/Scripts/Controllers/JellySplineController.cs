using System;
using Dreamteck.Splines;
using UnityEngine;

namespace _YabuGames.Scripts.Controllers
{
    [RequireComponent(typeof(SplineFollower))]
    public class JellySplineController: MonoBehaviour
    {
        public static JellySplineController Instance;
        
        private SplineFollower _splineFollower;
        private JellyController _jellyController;

        private void Awake()
        {
            _splineFollower = GetComponent<SplineFollower>();
            _jellyController = GetComponent<JellyController>();
        }
        
        
        public void SetOnBand(SplineComputer spline)
        {
            _splineFollower.spline = spline;
            _jellyController.SetOnBand();
            _splineFollower.follow = true;
            _splineFollower.enabled = true;
        }

        public void SetOffBand()
        {
            _splineFollower.follow = false;
            _splineFollower.enabled = false;
            _splineFollower = null;
            _jellyController.SetOffBand();
        }
    }
}