using System;
using System.Collections;
using System.Linq;
using Levels;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class GameSceneManager : SingletonBehavior<GameSceneManager>
    {
        [SerializeField] private float timeToChange = 0.5f;
        [SerializeField] private string initialScene = "MainMenu";

        private void Start()
        {
            if (SceneManager.GetActiveScene().name == "_Preload")
                SceneManager.LoadScene(initialScene);
        }

        public void SwitchScene(string newScene, bool shouldBeginLevel)
        {
            GameObject background = GameObject.FindWithTag("Background");

            StartCoroutine(ChangeScene(background.GetComponent<CanvasGroup>(), newScene, shouldBeginLevel));
        }

        private IEnumerator ChangeScene(CanvasGroup fromBackground, string newScene, bool shouldBeginLevel)
        {
            Scene currentScene = SceneManager.GetActiveScene();
            LeanTween.alphaCanvas(fromBackground, 1, timeToChange).setEaseInBack();

            AsyncOperation op = SceneManager.LoadSceneAsync(newScene, LoadSceneMode.Additive);
            
            while(!op.isDone)
                yield return null;

            GameObject toBackground = GameObject.FindGameObjectsWithTag("Background").First(x => x != fromBackground.gameObject);

            op = SceneManager.UnloadSceneAsync(currentScene.name);
            
            while(!op.isDone)
                yield return null;

            if (shouldBeginLevel)
            {
                var levelManager = FindObjectOfType<LevelManager>();
                levelManager.SetupLevel();
                levelManager.StartLevel();
            }

            LeanTween.alphaCanvas(toBackground.GetComponent<CanvasGroup>(), 0, timeToChange);
        }

    }
}