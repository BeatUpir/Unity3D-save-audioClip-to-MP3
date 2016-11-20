# Unity3D-save-audioClip-to-MP3
with this package you can save an audioclip to mp3 in unity3d
It works with both Windows and Android


Usage:

```c#
EncodeMP3.convert (AudioClip clip, string path, int BitRate);
```

If any errors occured you need to change your .NET API level in unity build settings.

For example for Android:
File => Build Settings => Player Settings => (in Inspector) Other Settings => Optimization => Api Compatibility Level : .Net 2.0 
