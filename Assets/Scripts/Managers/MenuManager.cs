using System;
using PlayerSaveData;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Managers
{
    public class MenuManager : MonoBehaviour
    {
        public const int MenuScene = 0;
        public const int GameScene = 1;
        
        [SerializeField] private Button playButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private Button deleteSaveButton;

        private bool _isLoadingGame;

        private void OnEnable()
        {
            Subscribe();
        }
        private void OnDisable()
        {
            Unsubscribe();
        }
        
        private void Subscribe()
        {
            playButton.onClick.AddListener(PlayButton);
            quitButton.onClick.AddListener(QuitButton);     
            deleteSaveButton.onClick.AddListener(DeleteSave);

            deleteSaveButton.interactable = SaveLoad.HasSaveData();
        }

        private void DeleteSave()
        {
            SaveLoad.DeleteSave();
            deleteSaveButton.interactable = false;
        }

        private void Unsubscribe()
        {
            playButton.onClick.RemoveAllListeners();
            quitButton.onClick.RemoveAllListeners();
            deleteSaveButton.onClick.RemoveAllListeners();
        }
        private void PlayButton()
        {
            if (_isLoadingGame) return;

            _isLoadingGame = true;
            SceneManager.LoadScene(GameScene);
        }
        private void QuitButton()
        {
            Application.Quit();
        }
    }
}
