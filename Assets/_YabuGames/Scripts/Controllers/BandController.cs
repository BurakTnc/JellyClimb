using System;
using Dreamteck.Splines;
using UnityEngine;

namespace _YabuGames.Scripts.Controllers
{
    public class BandController : MonoBehaviour
    {
        public static BandController Instance;
        
       [SerializeField] private SplineComputer spline;

       private void Awake()
       {
           Singleton();
       }

       private void Singleton()
       {
           if (Instance != this && Instance != null)
           {
               Destroy(this);
           }

           Instance = this;
       }

       public void GetBand(JellySplineController jellySplineController)
       {
           jellySplineController.SetOnBand(spline);
       }
       
    }
}