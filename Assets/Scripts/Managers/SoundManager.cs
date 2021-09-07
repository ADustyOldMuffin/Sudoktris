using System;
using System.Collections;
using UnityEngine;

namespace Managers
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundManager : SingletonBehavior<SoundManager>
    {
        private AudioSource _audioSource;

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void PlaySound(AudioClip clip)
        {
            _audioSource.clip = clip;
            _audioSource.loop = false;
            _audioSource.Play();
        }
    }
}