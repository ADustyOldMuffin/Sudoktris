using System;
using System.Collections;
using Levels;
using Managers;
using UnityEngine;

namespace UI
{
    public class GameOverModal : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private AudioClip buttonSound, gameOverSound;
        private static readonly int ShowGameOver = Animator.StringToHash("ShowGameOver");
        private static readonly int HideGameOver = Animator.StringToHash("HideGameOver");
        private LevelManager _levelManager;

        private void Start()
        {
            _levelManager = FindObjectOfType<LevelManager>();
        }

        private void OnEnable()
        {
            LevelManager.OnGameOver += OnGameOver;
        }

        private void OnDisable()
        {
            LevelManager.OnGameOver -= OnGameOver;
        }

        private void OnGameOver()
        {
            SoundManager.Instance.PlaySound(gameOverSound);
            animator.SetTrigger(ShowGameOver);
        }

        public void OnRetryPressed()
        {
            SoundManager.Instance.PlaySound(buttonSound);
            _levelManager.SetupLevel();
            _levelManager.StartLevel();
            animator.SetTrigger(HideGameOver);
        }

        public void OnMainMenuPressed()
        {
            SoundManager.Instance.PlaySound(buttonSound);
            GameSceneManager.Instance.SwitchScene("MainMenu", false);
        }
    }
}