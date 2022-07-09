using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleScript : MonoBehaviour {
	[SerializeField] AudioClip clip;

	void Start() {
		ConvertToArray();
		SaveAsMP3();
		SaveAsWav();

	}

	void ConvertToArray() {
		// Convert clip to mp3 bytes array
		//128 is recommend bitray for mp3 files
		byte[] mp3 = WavToMp3.ConvertWavToMp3(clip, 128);
		Debug.Log($"Convert to array {mp3.Length}"); ;
	}

	void SaveAsMP3() {
		// Save AudioClip at assets path with defined bitray as mp3
		//128 is recommend bitray for mp3 files
		EncodeMP3.SaveMp3(clip, $"{Application.dataPath}/mp3File", 128);
		Debug.Log($"Save file to {$"{Application.dataPath}/*"}"); ;
	}

	void SaveAsWav() {
		// Save AudioClip at assets path as wav
		SavWav.SaveWav($"{Application.dataPath}/wavFile", clip);
		Debug.Log($"Save file to {$"{Application.dataPath}/*"}"); ;
	}
}
