Example usege:
byte[] mp3 = WavToMp3.ConvertWavToMp3(clip, 128);					// Convert wav clip to mp3 bytes array
EncodeMP3.SaveMp3(clip, $"{Application.dataPath}/mp3File", 128);	// Save AudioClip at path with defined bitray as mp3
SavWav.SaveWav($"{Application.dataPath}/wavFile", clip);			// Save AudioClip at path with defined bitray as wav
