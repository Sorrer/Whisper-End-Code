using System.Collections.Generic;
using UnityEngine;

namespace Game.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioRandomClip : MonoBehaviour
    {

        public List<AudioClip> clips = new List<AudioClip>();
        [HideInInspector]
        public AudioSource source;

        public float PitchMin = 0.5f, PitchMax = 1.0f;
        
        public void Start() {
            source = GetComponent<AudioSource>();
        }

        public void Play() {
            int index = Random.Range(0, clips.Count);

            source.pitch = Random.Range(PitchMin, PitchMax);
            source.PlayOneShot(clips[index]);
        }
    }
}
