using System;
using System.IO;
using System.Collections;
using System.Threading;
using UnityEngine;
using NAudio.Lame;
using NAudio.Wave.WZT;
using NAudio.Wave;


public static class WavToMp3 {
	/// <summary>
	/// Convert Wav to MP3 to send it via network or save it to file
	/// </summary>
	/// <param name="clip">Unity AudioClip</param>
	/// <param name="bitRate">Mp3 bitrate. Recommend to set it to 128</param>
	/// <returns></returns>
	public static byte[] ConvertWavToMp3(AudioClip clip, int bitRate) {
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

		var retMs = new MemoryStream();
		var ms = new MemoryStream(SavWav.HEADER_SIZE + bytesData.Length);
		for (int i = 0; i < SavWav.HEADER_SIZE; i++)
			ms.WriteByte(new byte());
		ms.Write(bytesData, 0, bytesData.Length);
		SavWav.WriteHeader(ms, clip);

		var rdr = new WaveFileReader(ms);
		var wtr = new LameMP3FileWriter(retMs, rdr.WaveFormat, bitRate);
		rdr.CopyTo(wtr);

		return retMs.ToArray();
	}
}
