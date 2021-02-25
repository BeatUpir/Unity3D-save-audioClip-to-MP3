/*                    GNU GENERAL PUBLIC LICENSE
                       Version 3, 29 June 2007

 Copyright (C) 2007 Free Software Foundation, Inc. <http://fsf.org/>
 Everyone is permitted to copy and distribute verbatim copies
 of this license document, but changing it is not allowed.

*/

/*---------------------- BeatUp (C) 2016-------------------- */


using System;
using System.IO;
using System.Collections;
using System.Threading;
using UnityEngine;
using NAudio.Lame;
using NAudio.Wave.WZT;
using NAudio.Wave;

public static class EncodeMP3 {
	/// <summary>
	/// Save clip as mp3
	/// </summary>
	/// <param name="clip">Unity AudioClip</param>
	/// <param name="path">Path to save it</param>
	/// <param name="bitRate">Mp3 bitrate. Recommend to set it to 128</param>
	public static void SaveMp3(AudioClip clip, string path, int bitRate) {
		if (!path.EndsWith(".mp3"))
			path = path + ".mp3";

		ConvertAndWrite(clip, path, bitRate);
	}

	/// <summary>
	/// Save clip as wav
	/// </summary>
	/// <param name="clip">Unity AudioClip</param>
	/// <param name="path">Path to save it</param>
	/// <param name="bitRate">Mp3 bitrate. Recommend to set it to 128</param>
	private static void ConvertAndWrite(AudioClip clip, string path, int bitRate) {
		var samples = new float[clip.samples * clip.channels];

		clip.GetData(samples, 0);

		Int16[] intData = new Int16[samples.Length];
		//converting in 2 float[] steps to Int16[], //then Int16[] to Byte[]

		Byte[] bytesData = new Byte[samples.Length * 2];
		//bytesData array is twice the size of
		//dataSource array because a float converted in Int16 is 2 bytes.

		float rescaleFactor = 32767; //to convert float to Int16

		for (int i = 0; i < samples.Length; i++) {
			intData[i] = (short)(samples[i] * rescaleFactor);
			Byte[] byteArr = new Byte[2];
			byteArr = BitConverter.GetBytes(intData[i]);
			byteArr.CopyTo(bytesData, i * 2);
		}

		File.WriteAllBytes(path, ConvertWavToMp3(bytesData, bitRate));
	}

	/// <summary>
	/// Convert wav bytes to mp3 bytes
	/// derived from Gregorio Zanon's script
	/// </summary>
	/// <param name="wavFile">Wav file readed as array</param>
	/// <param name="bitRate">Mp3 bitrate. Recommend to set it to 128</param>
	/// <returns></returns>
	private static byte[] ConvertWavToMp3(byte[] wavFile, int bitRate) {

		var retMs = new MemoryStream();
		var ms = new MemoryStream(wavFile);
		var rdr = new WaveFileReader(ms);
		var wtr = new LameMP3FileWriter(retMs, rdr.WaveFormat, bitRate);

		rdr.CopyTo(wtr);
		return retMs.ToArray();
	}
}