using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffect : MonoBehaviour {

	private AudioClip[] audioClips;
	private AudioSource[] audioSources;

	public SoundEffect (AudioClip[] clips){
		audioClips = clips;

		for (int i = 0; i < clips.Length; i++) {
			audioSources [i] = gameObject.AddComponent<AudioSource>();
			audioSources [i].clip = audioClips [i];
			audioSources [i].volume = 0;
			audioSources [i].loop = false;
		}
	}

	public void PlayRand(){
		int option = Random.Range (0, audioClips.Length);
		audioSources [option].volume = 3;
		audioSources [option].Play ();
	}
}
