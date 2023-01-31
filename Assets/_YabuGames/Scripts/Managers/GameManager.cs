using System;
using System.Collections.Generic;
using _YabuGames.Scripts.Controllers;
using _YabuGames.Scripts.Signals;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _YabuGames.Scripts.Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        public static int Money;

        public List<Transform> emptyGrids = new List<Transform>();
        public List<Transform> occupiedGrids = new List<Transform>();
        public int stairsLevel = 1;
        public int stepLimit;
        
        [SerializeField] private int[] maxStepCounts;
        
        

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
            GetValues();
            SetValues();
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
            CoreGameSignals.Instance.OnSave += Save;
        }

        private void UnSubscribe()
        {
            CoreGameSignals.Instance.OnSave -= Save;
        }

        #endregion

        private void Start()
        {
            SpawnJellies();
        }

        private void GetValues()
        {
            Money = PlayerPrefs.GetInt("money", 0);
        }

        private void SetValues()
        {
            stepLimit = maxStepCounts[stairsLevel];

        }

        private void SpawnJellies()
        {
            for (int i = 0; i < emptyGrids.Count; i++)
            {
                var temp = Instantiate(Resources.Load<GameObject>("Spawnables/Jelly"));
                var script = temp.GetComponent<JellyController>();
                temp.transform.position = emptyGrids[i].position;
                script.SetIdleGrid();
                Debug.Log("check");
            }
        }

        private void Save()
        {
            PlayerPrefs.SetInt("money",Money);
        }

        public void SetGrid(Transform grid, bool isOccupied)
        {
            if (isOccupied)
            {
                emptyGrids.Remove(grid);
                occupiedGrids.Add(grid);
            }
            else
            {
                occupiedGrids.Remove(grid);
                emptyGrids.Add(grid);
            }
        }
        public void AddJelly()
        {
            var temp = Instantiate(Resources.Load<GameObject>("Spawnables/Jelly"));
            temp.transform.localScale=Vector3.zero;
            temp.transform.position = emptyGrids[Random.Range(0, emptyGrids.Count)].position;
            temp.transform.DOScale(Vector3.one, .5f).SetEase(Ease.OutBack);
        }

        public void ArrangeMoney(int value)
        {
            Money += value;
        }

        public int GetMoney()
        {
            return Money < 0 ? 0 : Money;
        }
    }
}