# Unity3D-save-audioClip-to-MP3
With this package you can save an audioclip to mp3 in unity3d. Also plugin can save audioclip to wav and convert wav to mp3 and mp3 to wav.

It works with Windows, Android and IOS(tested). And probably on Mac(untested, but should work). 
Lame unity port from here https://github.com/3wz/Lame-For-Unity

Usage:

```c#
//Save wav
EncodeMP3.convert (AudioClip clip, string path, int BitRate);

//Save mp3
EncodeMP3.convert (AudioClip clip, string path, int BitRate);

//Save wav to mp3
EncodeMP3.convert (AudioClip clip, string path, int BitRate);

//Save mp3 to wav
EncodeMP3.convert (AudioClip clip, string path, int BitRate);
```
