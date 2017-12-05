using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundEffect : MonoBehaviour {

    public bool playOnAwake = false;
    public bool loop = false;
	public AudioClip[] audioClips;

	private AudioSource audioSource;


    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = playOnAwake;
        audioSource.loop = loop;

        if (playOnAwake)
        {
            Play(0);
        }
    }

    public void Play(int index)
    {
        if (index < 0 || index > audioClips.Length - 1) return;

        audioSource.clip = audioClips[index];
        audioSource.Play();
    }

    public void PlayRand(){
        if (audioClips.Length < 1) return;

		int option = Random.Range (0, audioClips.Length);
        audioSource.clip = audioClips[option];
        audioSource.Play();
	}
}
