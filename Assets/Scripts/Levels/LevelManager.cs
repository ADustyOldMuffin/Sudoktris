using UnityEngine;

namespace Levels
{
    
    
    public abstract class LevelManager : MonoBehaviour
    {
        protected int Score { get; private set; }

        public delegate void ScoreUpdatedEventHandler(int score);
        public static event ScoreUpdatedEventHandler OnScoreUpdated;

        public delegate void GameOverEventHandler();

        public static event GameOverEventHandler OnGameOver;
        
        public abstract void SetupLevel();
        public abstract void StartLevel();

        protected void UpdateScore(int score)
        {
            Score = score;
            OnScoreUpdated?.Invoke(Score);
        }

        protected void GameOver()
        {
            OnGameOver?.Invoke();
        }
    }
}