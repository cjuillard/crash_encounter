using System.Collections.Generic;
using UnityEngine;

namespace Runamuck
{
    public class GameAudio : MonoBehaviour
    {
        private static GameAudio instance;
        public static GameAudio Instance => instance;

        private Dictionary<AudioClip, float> lastPlayedTime = new Dictionary<AudioClip, float>();
        private AudioListener audioListener;

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }

        public void PlayClip(AudioClip clip, Vector3 pos, float minDuplicateDelay = .1f)
        {
            if(audioListener == null)
                audioListener = FindObjectOfType<AudioListener>();

            float lastPlayTime = 0;
            lastPlayedTime.TryGetValue(clip, out lastPlayTime);

            if (Time.realtimeSinceStartup - lastPlayTime < minDuplicateDelay)
                return;

            // Keep volume basically at max but maintain the spatial part of things by using the direction
            Vector3 dir = (pos - audioListener.transform.position).normalized;
            Vector3 adjustedPos = audioListener.transform.position + dir;
            AudioSource.PlayClipAtPoint(clip, adjustedPos);

            lastPlayedTime[clip] = Time.realtimeSinceStartup;
        }
    }
}
