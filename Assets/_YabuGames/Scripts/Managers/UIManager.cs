using System;
using System.Collections;
using System.Collections.Generic;
using _YabuGames.Scripts.Signals;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _YabuGames.Scripts.Managers
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;
        
        [SerializeField] private GameObject mainPanel, gamePanel, winPanel, losePanel, storePanel;
        [SerializeField] private TextMeshProUGUI[] moneyText;
        [SerializeField] private TextMeshProUGUI jellyButtonText, progressText, levelText;
        [SerializeField] private Image progressBar;
        [SerializeField] private GameObject startGrid1, startGrid2;
        [SerializeField] private Transform grid1, grid2;
        [SerializeField] private Button addJellyButton,
            incomeButton,
            increaseStairsButton,
            expandButton1,
            expandButton2,
            gridButton1,
            gridButton2;
            

        private int _jellyPrice, _incomePrice, _increasePrice, _expandPrice;
        private int _jellyLevel, _incomeLevel, _increaseLevel, _expandLevel;
        private float _fillAmount;
        private int _level = 1;
        private int _grid1Taken, _grid2Taken, _expand1Taken, _expand2Taken, _increaseStairsTaken;

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
        }

        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            UnSubscribe();
        }
        

        private void GetValues()
        {
            _jellyPrice = PlayerPrefs.GetInt("jellyPrice", 5);
            _incomePrice = PlayerPrefs.GetInt("incomePrice", 10);
            _increasePrice = PlayerPrefs.GetInt("increasePrice", 1000);
            _expandPrice = PlayerPrefs.GetInt("expandPrice", 500);
            //---------//
            _jellyLevel = PlayerPrefs.GetInt("jellyLevel", 1);
            _incomeLevel = PlayerPrefs.GetInt("incomeLevel", 1);
            _increaseLevel = PlayerPrefs.GetInt("increaseLevel", 1);
            _expandLevel = PlayerPrefs.GetInt("expandLevel", 1);
            //---------//
            _grid1Taken = PlayerPrefs.GetInt("grid1Taken", 0);
            _grid2Taken=PlayerPrefs.GetInt("grid2Taken", 0);
            _expand1Taken = PlayerPrefs.GetInt("expand1Taken", 0);
            _expand2Taken = PlayerPrefs.GetInt("expand2Taken", 0);
            _increaseStairsTaken = PlayerPrefs.GetInt("increaseStairsTaken", 0);
        }

        private void SaveValues()
        {
            PlayerPrefs.SetInt("jellyPrice",_jellyPrice);
            PlayerPrefs.SetInt("incomePrice",_incomePrice);
            PlayerPrefs.SetInt("increasePrice",_increasePrice);
            PlayerPrefs.SetInt("expandPrice",_expandPrice);
            //---------//
            PlayerPrefs.SetInt("jellyLevel",_jellyLevel);
            PlayerPrefs.SetInt("incomeLevel",_incomeLevel);
            PlayerPrefs.SetInt("increaseLevel",_increaseLevel);
            PlayerPrefs.SetInt("expandPrice",_expandLevel);
            //---------//
            PlayerPrefs.SetInt("grid1Taken",_grid1Taken);
            PlayerPrefs.SetInt("grid2Taken",_grid2Taken);
            PlayerPrefs.SetInt("expand1Taken",_expand1Taken);
            PlayerPrefs.SetInt("expand2Taken",_expand2Taken);
            PlayerPrefs.SetInt("increaseStairsTaken", _increaseStairsTaken);
        }

        private void Start()
        {
            StartCoroutine(CheckLevelStats());
            SetMoneyTexts();
            CheckButtonStats();
        }

        #region Subscribtions
        private void Subscribe()
        {
            CoreGameSignals.Instance.OnLevelWin += LevelWin;
            CoreGameSignals.Instance.OnLevelFail += LevelLose;
            CoreGameSignals.Instance.OnGameStart += OnGameStart;
            CoreGameSignals.Instance.OnSave += SetMoneyTexts;
            CoreGameSignals.Instance.OnSave += CheckButtonStats;
            CoreGameSignals.Instance.OnSave += SaveValues;
        }
        
        private void UnSubscribe()
        {
            CoreGameSignals.Instance.OnLevelWin -= LevelWin;
            CoreGameSignals.Instance.OnLevelFail -= LevelLose;
            CoreGameSignals.Instance.OnGameStart -= OnGameStart;
            CoreGameSignals.Instance.OnSave -= SetMoneyTexts;
            CoreGameSignals.Instance.OnSave -= CheckButtonStats;
            CoreGameSignals.Instance.OnSave -= SaveValues;
        }

        #endregion

        private IEnumerator CheckLevelStats()
        {
            if (_grid1Taken == 1) 
            {
                OpenGrids(grid1,gridButton1);
            }
            if (_grid2Taken == 1) 
            {
                OpenGrids(grid2,gridButton2);
            }
            
            if (_expand1Taken == 1)  
            {
                SetHorizontalExpand();
                expandButton1.gameObject.SetActive(false);
            }

            yield return new WaitForSeconds(.4f);
            if (_expand2Taken == 1) 
            {
                SetHorizontalExpand();
                expandButton2.gameObject.SetActive(false);
            }

            yield return new WaitForSeconds(.4f);
            
            if (_increaseStairsTaken == 1) 
            {
                GameManager.Instance.VerticalExpand();
                increaseStairsButton.gameObject.SetActive(false);
                CheckButtonStats();
            }
            
        }
        
        private void SetHorizontalExpand()
        {
            GameManager.Instance.HorizontalExpand();
            if (GameManager.Instance.horizontalLevel==1)
            {
                expandButton2.gameObject.SetActive(true);
                expandButton2.transform.DOScale(Vector3.one, .5f).SetEase(Ease.OutBack).SetDelay(.5f);
                startGrid1.SetActive(true);
            }
            else
            {
                startGrid2.SetActive(true);
            }
            CheckButtonStats();
        }
        private void OpenGrids(Transform grid,Button btn)
        {
            GameManager.Instance.SetGrid(grid,false);
            GameManager.Instance.JellyLimit++;
            GameManager.Instance.BoughtGrid++;
            btn.gameObject.SetActive(false);

            grid.gameObject.SetActive(true);
            CheckButtonStats();
        }
        private void CheckButtonStats()
        {
            JellyButtonCheck();
            GridButton1Check();
            GridButton2Check();
            ExpandButton1Check();
            ExpandButton2Check();
            IncreaseButtonCheck();
            ProgressCheck();
            SetMoneyTexts();
           // CoreGameSignals.Instance.OnSave?.Invoke();
        }

        private void ProgressCheck()
        {
            if (GameManager.Instance.ClimbedStairs==300)
            {
                LevelUp(); 
                return;
            }
            _fillAmount = (float)GameManager.Instance.ClimbedStairs / 300;
            progressBar.fillAmount = _fillAmount;
            progressText.text = "%" + (int)(_fillAmount * 100);
            levelText.text = "LEVEL " + _level;
        }

        private void LevelUp()
        {
            _level++;
            GameManager.Instance.ClimbedStairs = 0;
        }

        private void IncreaseButtonCheck()
        {
            if (GameManager.Money < 100000)
            {
                increaseStairsButton.interactable = false;
            }
            else
            {
                increaseStairsButton.interactable = true;
            }
        }

        private void ExpandButton2Check()
        {
            if (GameManager.Money < 50000)
            {
                expandButton2.interactable = false;
            }
            else
            {
                expandButton2.interactable = true;
            }
        }

        private void ExpandButton1Check()
        {
            if (GameManager.Money < 10000)
            {
                expandButton1.interactable = false;
            }
            else
            {
                expandButton1.interactable = true;
            }
        }

        private void GridButton2Check()
        {
            if (GameManager.Money < 5000)
            {
                gridButton1.interactable = false;
            }
            else
            {
                gridButton1.interactable = true;
            }
        }

        private void GridButton1Check()
        {
            if (GameManager.Money < 2000)
            {
                gridButton1.interactable = false;
            }
            else
            {
                gridButton1.interactable = true;
            }
        }

        private void JellyButtonCheck()
        {
            if (GameManager.Instance.JellyLimit > 0 && _jellyPrice <= GameManager.Money) 
            {
                addJellyButton.interactable = true;
            }
            else
            {
                addJellyButton.interactable = false;
            }
            
            if (_jellyPrice >= 1000) 
            {
                var thousand = Mathf.FloorToInt(_jellyPrice / 1000);
                var hundred = Mathf.FloorToInt(_jellyPrice % 1000);
                        
                hundred = Mathf.FloorToInt(hundred / 100);

                jellyButtonText.text = "$ " + thousand + "." + hundred + "k";
            }
                    
            else
            {
                jellyButtonText.text = "$ " + (int)_jellyPrice;
            }
        }

        private void OnGameStart()
        {
            mainPanel.SetActive(false);
            gamePanel.SetActive(true);
        }
        
        private void SetMoneyTexts()
        {
            if (moneyText.Length <= 0) return;

            foreach (var t in moneyText)
            {
                if (t)
                {
                    if (GameManager.Money>=1000000)
                    {
                        var million = Mathf.FloorToInt(GameManager.Money / 1000000);
                        var thousand = Mathf.FloorToInt(GameManager.Money % 1000000);
                        
                        thousand = Mathf.FloorToInt(thousand / 1000);

                        t.text = "$ " + million + "." + thousand + "m";
                        return;
                    }
                    if (GameManager.Money >= 1000 && GameManager.Money<1000000) 
                    {
                        var thousand = Mathf.FloorToInt(GameManager.Money / 1000);
                        var hundred = Mathf.FloorToInt(GameManager.Money % 1000);
                        
                        hundred = Mathf.FloorToInt(hundred / 100);

                        t.text = "$ " + thousand + "." + hundred + "k";
                    }
                    
                    else
                    {
                        t.text = "$ " + (int)GameManager.Money;
                    }
                    
                }
            }
        }
        private void LevelWin()
        {
            gamePanel.SetActive(false);
            winPanel.SetActive(true);
            HapticManager.Instance.PlaySuccessHaptic();
        }

        private void LevelLose()
        {
            gamePanel.SetActive(false);
            gamePanel.SetActive(true);
            HapticManager.Instance.PlayFailureHaptic();
        }

        private void DisableButton(Transform button)
        {
            Destroy(button.gameObject);
        }

        public void CheatButton()
        {
            GameManager.Money += 1000000;
            CheckButtonStats();
            HapticManager.Instance.PlayLightHaptic();
        }

        public void ResetButton()
        {
            PlayerPrefs.DeleteAll();
            SceneManager.LoadScene(0);
        }
        public void JellyButton()
        {
            GameManager.Money -= _jellyPrice;
            GameManager.Instance.AddJelly();
            _jellyPrice = (int)(_jellyPrice * 1.5f);
            CheckButtonStats();
            HapticManager.Instance.PlayLightHaptic();
        }

        public void OpenGridButton(Transform grid)
        {
            GameManager.Instance.SetGrid(grid,false);
            GameManager.Instance.JellyLimit++;
            GameManager.Instance.BoughtGrid++;
            if (GameManager.Instance.BoughtGrid==1)
            {
                _grid1Taken = 1;
                GameManager.Money -= 2000;
                gridButton1.interactable = false;
                gridButton2.gameObject.SetActive(true);
            }
            else
            {
                _grid2Taken = 1;
                GameManager.Money -= 5000;
            }

            grid.gameObject.SetActive(true);
            CheckButtonStats();
            HapticManager.Instance.PlayLightHaptic();
        }

        public void DisableGridButton(Transform gridButton)
        {
            gridButton.DOScale(Vector3.zero, .5f).SetEase(Ease.InBack).OnComplete(() => DisableButton(gridButton));
        }

        public void HorizontalExpand()
        {
            GameManager.Instance.HorizontalExpand();
            if (GameManager.Instance.horizontalLevel==1)
            {
                _expand1Taken = 1;
                GameManager.Money -= 10000;
                expandButton2.gameObject.SetActive(true);
                expandButton2.transform.DOScale(Vector3.one, .5f).SetEase(Ease.OutBack).SetDelay(.5f);
                startGrid1.SetActive(true);
            }
            else
            {
                _expand2Taken = 1;
                GameManager.Money -= 50000;
                startGrid2.SetActive(true);
            }
            HapticManager.Instance.PlayLightHaptic();
            CheckButtonStats();
        }

        public void VerticalExpand()
        {
            _increaseStairsTaken = 1;
            GameManager.Instance.VerticalExpand();
            GameManager.Money -= 100000;
            HapticManager.Instance.PlayLightHaptic();
            CheckButtonStats();
        }
        

        public void MenuButton()
        {
            mainPanel.SetActive(true);
            HapticManager.Instance.PlayLightHaptic();
        }
        
    }
}
