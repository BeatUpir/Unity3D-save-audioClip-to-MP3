# Save audioClip to MP3
With this package you can save an audioclip to mp3 in unity3d. Also plugin can save audioclip to wav and convert wav to mp3.

It works with *Windows, Android and IOS(tested)*. And probably on *Mac(untested, but should work)*. 

Lame unity port from https://github.com/3wz/Lame-For-Unity  
Wav save script from https://gist.github.com/darktable/2317063

### Convert audio clip to mp3 bytes array. For example to send it via network:
```C#
public AudioClip clip;

void Start(){
// Convert wav clip to mp3 bytes array
//128 is recommend bitray for mp3 files
byte[] mp3 = WavToMp3.ConvertWavToMp3(clip, 128);
}
```


### Save AudioClip at path with defined bitray as mp3
```C#
public AudioClip clip;

void Start(){
// Save AudioClip at assets path with defined bitray as mp3
//128 is recommend bitray for mp3 files
EncodeMP3.SaveMp3(clip, $"{Application.dataPath}/mp3File", 128);
}
```

### Save AudioClip at path with as wav
```C#
public AudioClip clip;

void Start(){
// Save AudioClip at assets path as wav
SavWav.SaveWav($"{Application.dataPath}/wavFile", clip);	
}
```
	

