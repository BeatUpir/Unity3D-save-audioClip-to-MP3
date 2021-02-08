# Unity3D-save-audioClip-to-MP3
With this package you can save an audioclip to mp3 in unity3d. Also plugin can save audioclip to wav and convert wav to mp3.

It works with *Windows, Android and IOS(tested)*. And probably on *Mac(untested, but should work)*. 

Lame unity port from https://github.com/3wz/Lame-For-Unity

Wav save script from https://gist.github.com/darktable/2317063

Usage:

```c#
byte[] mp3 = WavToMp3.ConvertWavToMp3(clip, 128);					// Convert wav clip to mp3 bytes array
EncodeMP3.SaveMp3(clip, $"{Application.dataPath}/mp3File", 128);	// Save AudioClip at path with defined bitray as mp3
SavWav.SaveWav($"{Application.dataPath}/wavFile", clip);			// Save AudioClip at path with defined bitray as wav
```
