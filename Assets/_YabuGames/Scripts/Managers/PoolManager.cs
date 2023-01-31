using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace _YabuGames.Scripts.Managers
{
    public class PoolManager : MonoBehaviour
    {
        public static PoolManager Instance;
        
    [Header("                               // Set Particles Stop Action To DISABLE //")]
    [Space(20)]
        [SerializeField] private List<GameObject> splashParticle = new List<GameObject>();
        [SerializeField] private List<GameObject> groundSplashParticle = new List<GameObject>();
        [SerializeField] private List<GameObject> incomeTextParticle = new List<GameObject>();
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
        public void GetIncomeTextParticle(Vector3 desiredPos,int income)
        {
            var temp = incomeTextParticle[0];
            incomeTextParticle.Remove(temp);
            temp.transform.position = desiredPos;
            temp.transform.localScale=Vector3.one;
            temp.GetComponent<TextMeshPro>().text = "$"+income;
            temp.SetActive(true);
            temp.transform.DOMoveY(5, .5f).SetRelative(true).SetEase(Ease.OutSine);
            temp.transform.DOScale(Vector3.zero, 1f).SetEase(Ease.InSine).OnComplete(() => DisableParticle(temp));
        }

        private void DisableParticle(GameObject particle)
        {
            particle.SetActive(false);
            incomeTextParticle.Add(particle);
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
