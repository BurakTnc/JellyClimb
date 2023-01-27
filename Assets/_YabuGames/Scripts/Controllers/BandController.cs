using System;
using Dreamteck.Splines;
using UnityEngine;

namespace _YabuGames.Scripts.Controllers
{
    public class BandController : MonoBehaviour
    {
        public static BandController Instance;
        
       [SerializeField] private SplineComputer spline;
       [SerializeField] private SplineMesh mesh;
       [SerializeField] private float moveSpeed;

       private float _offset = 0;

       private void Awake()
       {
           Singleton();
       }

       private void Update()
       {
           _offset += moveSpeed * Time.deltaTime;
           mesh.uvOffset = new Vector2(0, _offset);
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