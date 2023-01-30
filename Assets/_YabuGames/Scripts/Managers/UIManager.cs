using System;
using _YabuGames.Scripts.Signals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _YabuGames.Scripts.Managers
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;
        
        [SerializeField] private GameObject mainPanel, gamePanel, winPanel, losePanel, storePanel;
        [SerializeField] private TextMeshProUGUI[] moneyText;
        [SerializeField] private Button addJellyButton, incomeButton, increaseStairsButton, expandButton;

        private int _jellyPrice, _incomePrice, _increasePrice, _expandPrice;
        private int _jellyLevel, _incomeLevel, _increaseLevel, _expandLevel;


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
            _jellyPrice = PlayerPrefs.GetInt("jellyPrice", 10);
            _incomePrice = PlayerPrefs.GetInt("incomePrice", 10);
            _increasePrice = PlayerPrefs.GetInt("increasePrice", 1000);
            _expandPrice = PlayerPrefs.GetInt("expandPrice", 500);
            //---------//
            _jellyLevel = PlayerPrefs.GetInt("jellyLevel", 1);
            _incomeLevel = PlayerPrefs.GetInt("incomeLevel", 1);
            _increaseLevel = PlayerPrefs.GetInt("increaseLevel", 1);
            _expandLevel = PlayerPrefs.GetInt("expandLevel", 1);
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
        }

        private void Start()
        {
            SetMoneyTexts();
        }

        #region Subscribtions
        private void Subscribe()
                {
                    CoreGameSignals.Instance.OnLevelWin += LevelWin;
                    CoreGameSignals.Instance.OnLevelFail += LevelLose;
                    CoreGameSignals.Instance.OnGameStart += OnGameStart;
                }
        
                private void UnSubscribe()
                {
                    CoreGameSignals.Instance.OnLevelWin -= LevelWin;
                    CoreGameSignals.Instance.OnLevelFail -= LevelLose;
                    CoreGameSignals.Instance.OnGameStart -= OnGameStart;
                }

        #endregion

        private void CheckButtonStats()
        {
            
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
                    t.text = "$" + GameManager.Money;
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

        public void JellyButton()
        {
            GameManager.Instance.AddJelly();
        }

        public void MenuButton()
        {
            mainPanel.SetActive(true);
            HapticManager.Instance.PlayLightHaptic();
        }
        
    }
}
