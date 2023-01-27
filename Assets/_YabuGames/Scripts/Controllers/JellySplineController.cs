using System;
using Dreamteck.Splines;
using UnityEngine;

namespace _YabuGames.Scripts.Controllers
{
    public class JellySplineController: MonoBehaviour
    {
        public static JellySplineController Instance;
        
        private SplineComputer _splineComputer;
        private JellyController _jellyController;

        private void Awake()
        {
            _splineComputer = GetComponent<SplineComputer>();
            _jellyController = GetComponent<JellyController>();
        }

        private void Singleton()
        {
            if (Instance != this && Instance != null) ;
            {
                
            }
        }
        
        public void SetOnBand()
        {
            _jellyController.SetOnBand(_splineComputer);
        }

        public void SetOffBand()
        {
            _splineComputer = null;
            _jellyController.SetOffBand();
        }
    }
}