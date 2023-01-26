using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace _YabuGames.Scripts.Managers
{
    public class PoolManager : MonoBehaviour
    {
        public static PoolManager Instance;
        
    [FormerlySerializedAs("firstParticle")]
    [Header("                               // Set Particles Stop Action To DISABLE //")]
    [Space(20)]
        [SerializeField] private List<GameObject> splashParticle = new List<GameObject>();
        [SerializeField] private List<GameObject> groundSplashParticle = new List<GameObject>();
        [SerializeField] private List<GameObject> thirdParticle = new List<GameObject>();
        [SerializeField] private List<GameObject> fourthParticle = new List<GameObject>();

        
        private void Awake()
        {
            #region Singleton

            if (Instance != this && Instance != null)
            {
                Destroy(this);
                return;
            }
            Instance = this;

            #endregion
            
        }

        public void GetSplashParticle(Vector3 desiredPos)
        {
            var temp = splashParticle[0];
            splashParticle.Remove(temp);
            temp.transform.position = desiredPos;
            temp.SetActive(true);
            splashParticle.Add(temp);
            
        }
        public void GetGroundSplashParticle(Vector3 desiredPos)
        {
            var temp = groundSplashParticle[0];
            groundSplashParticle.Remove(temp);
            temp.transform.position = desiredPos;
            temp.SetActive(true);
            groundSplashParticle.Add(temp);
        }
        public void GetThirdParticle(Vector3 desiredPos)
        {
            var temp = thirdParticle[0];
            thirdParticle.Remove(temp);
            temp.transform.position = desiredPos;
            temp.SetActive(true);
            thirdParticle.Add(temp);
        }
        public void GetFourthParticle(Vector3 desiredPos)
        {
            var temp = fourthParticle[0];
            fourthParticle.Remove(temp);
            temp.transform.position = desiredPos;
            temp.SetActive(true);
            fourthParticle.Add(temp);
        }
    }
}
