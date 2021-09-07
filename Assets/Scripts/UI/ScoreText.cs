using System;
using Levels;
using TMPro;
using UnityEngine;

namespace UI
{
    public class ScoreText : MonoBehaviour
    {
        private LevelManager _levelManager;
        private TextMeshProUGUI _textMeshPro;
        [SerializeField] private TextMeshProUGUI finalScore;
        
        private void Awake()
        {
            _levelManager = FindObjectOfType<LevelManager>();
            _textMeshPro = GetComponent<TextMeshProUGUI>();
        }

        private void OnEnable()
        {
            if (ReferenceEquals(_levelManager, null))
                return;

            LevelManager.OnScoreUpdated += OnScoreUpdated;
        }

        private void OnDisable()
        {
            if (ReferenceEquals(_levelManager, null))
                return;

            LevelManager.OnScoreUpdated -= OnScoreUpdated;
        }

        private void OnScoreUpdated(int score)
        {
            _textMeshPro.text = score.ToString();

            if (!ReferenceEquals(finalScore, null))
                finalScore.text = score.ToString();
        }
    }
}