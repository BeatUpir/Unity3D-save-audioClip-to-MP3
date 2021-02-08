using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleScript : MonoBehaviour {
	[SerializeField] AudioClip clip;

	void Start() {
		byte[] mp3 = WavToMp3.ConvertWavToMp3(clip, 128);

		EncodeMP3.SaveMp3(clip, $"{Application.dataPath}/mp3File", 128);
		SavWav.SaveWav($"{Application.dataPath}/wavFile", clip);

		Debug.Log($"Save files to {$"{Application.dataPath}/*"}"); ;
	}
}
