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
        public static int JellyCount;
        public static int JellyLimit = 3;
        
        public List<Transform> emptyGrids = new List<Transform>();
        public List<Transform> occupiedGrids = new List<Transform>();
        public int stairsLevel = 0;
        public int stepLimit;
        public int horizontalLevel = 0;
        
        [SerializeField] private int[] maxStepCounts;
        [SerializeField] private float[] xPosReferences, yPosReferences;
        
        private float _verticalReference, _horizontalReference;

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
            SetReferencePositions();
        }

        private void SetReferencePositions()
        {
            switch (stairsLevel)
            {
                case 0:
                    _verticalReference = 4;
                    break;
                case 1:
                    _verticalReference = 5;
                    break;
                case 3:
                    _verticalReference = 6;
                    break;
                case 4:
                    _verticalReference = 7;
                    break;
                case 5:
                    _verticalReference = 8;
                    break;
            }

            switch (horizontalLevel)
            {
                case 0:
                    _horizontalReference = 2;
                    break;
                case 1:
                    _horizontalReference = 4;
                    break;
                case 2:
                    _horizontalReference = 6;
                    break;
                case 3:
                    _horizontalReference = 8;
                    break;
            }
        }

        private void SpawnJellies()
        {
            for (int i = 0; i < 1; i++)
            {
                var temp = Instantiate(Resources.Load<GameObject>("Spawnables/Jelly"));
                var script = temp.GetComponent<JellyController>();
                temp.transform.position = emptyGrids[i].position;
                script.SetIdleGrid(emptyGrids[i]);
                JellyCount++;
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
                if (!occupiedGrids.Contains(grid))
                {
                    occupiedGrids.Add(grid);
                }
                grid.GetComponent<BoxCollider>().enabled = false;
            }
            else
            {
                occupiedGrids.Remove(grid);
                if (!emptyGrids.Contains(grid))
                {
                    emptyGrids.Add(grid);
                }
                grid.GetComponent<BoxCollider>().enabled = true;
            }
        }
        public void AddJelly()
        {
            var temp = Instantiate(Resources.Load<GameObject>("Spawnables/Jelly"));
            var script = temp.GetComponent<JellyController>();
            temp.transform.position = emptyGrids[0].position;
            script.SetIdleGrid(emptyGrids[0]);
            JellyCount++;
        }

        public void ArrangeMoney(int value)
        {
            Money += value;
        }

        public int GetMoney()
        {
            return Money < 0 ? 0 : Money;
        }

        public void VerticalExpand()
        {
            stairsLevel++;
            var delay = 0f;
            Vector3 desiredPos;
            for (var i = 0; i < 3+horizontalLevel; i++)
            {
                delay += .1f;
                var temp = Instantiate(Resources.Load<GameObject>("Spawnables/Block"));
                temp.transform.localScale = Vector3.zero;
                temp.transform.position = new Vector3(xPosReferences[i], _verticalReference - 5,
                    ((_verticalReference + 1) * 2) + 1);

                temp.transform.DOMoveY(_verticalReference + 1, .5f).SetDelay(delay).SetEase(Ease.OutBack);
                temp.transform.DOScale(new Vector3(120, 60, 120), .5f).SetDelay(delay).SetEase(Ease.OutBack)
                    .OnComplete(SetReferencePositions);

            }
        }

        public void HorizontalExpand()
        {
            horizontalLevel++;
            var delay = 0f;
            Vector3 desiredPos;
            for (var i = 0; i < 5+stairsLevel; i++)
            {
                delay += .1f;
                var temp = Instantiate(Resources.Load<GameObject>("Spawnables/Block"));
                temp.transform.localScale = Vector3.zero;
                temp.transform.position = new Vector3(_horizontalReference + 2, yPosReferences[i] - 5,
                    yPosReferences[i] * 2+1);

                temp.transform.DOMoveY(yPosReferences[i], .3f).SetDelay(delay).SetEase(Ease.OutBack);
                temp.transform.DOScale(new Vector3(120, 60, 120), .3f).SetDelay(delay).SetEase(Ease.OutBack)
                    .OnComplete(SetReferencePositions);

            }
        }
    }
}