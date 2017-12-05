using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundEffect : MonoBehaviour {

	public AudioClip[] audioClips;
	private AudioSource audioSource;


    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayRand(){
        if (audioClips.Length < 1) return;

		int option = Random.Range (0, audioClips.Length);
        audioSource.clip = audioClips[option];
        audioSource.Play();
	}
}
