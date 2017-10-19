/*                    GNU GENERAL PUBLIC LICENSE
                       Version 3, 29 June 2007

 Copyright (C) 2007 Free Software Foundation, Inc. <http://fsf.org/>
 Everyone is permitted to copy and distribute verbatim copies
 of this license document, but changing it is not allowed.

*/

/*---------------------- BeatUp (C) 2016-------------------- */


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using NAudio.Wave;
using NAudio.Lame;

public static class EncodeMP3
{


	public static void convert (AudioClip clip, string path, int bitRate)
	{
		var samples = new float[clip.samples * clip.channels];
		clip.GetData (samples, 0);
		Convert (samples, path, clip.frequency, clip.channels, bitRate);
	}

	public static void convert (float[] samples, string path, int sampleRate, int channels, int bitRate)
	{

		if (!path.EndsWith (".mp3"))
			path = path + ".mp3";

		ConvertAndWrite (samples, path, sampleRate, channels, bitRate);
	}

	static void ConvertAndWrite (float[] samples, string path, int sampleRate, int channels, int bitRate)
	{
		var intData = new Int16[samples.Length];
		var bytesData = new Byte[samples.Length * sizeof(Int16)];

		const float rescaleFactor = 32767;

		for (int i = 0; i < samples.Length; i++)
		{
			intData[i] = (short)(samples[i] * rescaleFactor);
		}
		Buffer.BlockCopy(intData, 0, bytesData, 0, bytesData.Length);

		File.WriteAllBytes (path, ConvertWavToMp3 (bytesData, sampleRate, channels, bitRate));
	}

	static byte[] ConvertWavToMp3 (byte[] wavFile, int sampleRate, int channels, int bitRate)
	{

		var retMs = new MemoryStream ();
		var ms = new MemoryStream (wavFile);
		var rdr = new RawSourceWaveStream (ms, new WaveFormat (sampleRate, channels));
		var wtr = new LameMP3FileWriter (retMs, rdr.WaveFormat, bitRate);

		rdr.CopyTo (wtr);
		return retMs.ToArray ();
	}
}
