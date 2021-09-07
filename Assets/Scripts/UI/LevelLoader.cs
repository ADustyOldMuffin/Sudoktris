using System.Collections;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class LevelLoader : MonoBehaviour
    {
        [SerializeField] private AudioSource buttonSound;
        [SerializeField] private Animator sceneTransition;
        [SerializeField] private float transitionTime = 1.0f;
        private static readonly int Start = Animator.StringToHash("Start");

        public void SwitchLevel(string switchTo)
        {
            buttonSound.Play();
            StartCoroutine(LoadLevel(switchTo));
        }

        private IEnumerator LoadLevel(string switchTo)
        {
            sceneTransition.SetTrigger(Start);

            yield return new WaitForSeconds(transitionTime);

            SceneManager.LoadScene(switchTo);
        }
    }
}
