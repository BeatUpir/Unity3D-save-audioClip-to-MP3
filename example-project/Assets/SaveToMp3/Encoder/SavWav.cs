//	Copyright (c) 2012 Calvin Rien
//        http://the.darktable.com
//
//	This software is provided 'as-is', without any express or implied warranty. In
//	no event will the authors be held liable for any damages arising from the use
//	of this software.
//
//	Permission is granted to anyone to use this software for any purpose,
//	including commercial applications, and to alter it and redistribute it freely,
//	subject to the following restrictions:
//
//	1. The origin of this software must not be misrepresented; you must not claim
//	that you wrote the original software. If you use this software in a product,
//	an acknowledgment in the product documentation would be appreciated but is not
//	required.
//
//	2. Altered source versions must be plainly marked as such, and must not be
//	misrepresented as being the original software.
//
//	3. This notice may not be removed or altered from any source distribution.
//
//  =============================================================================
//
//  derived from Gregorio Zanon's script
//  http://forum.unity3d.com/threads/119295-Writing-AudioListener.GetOutputData-to-wav-problem?p=806734&viewfull=1#post806734

using System;
using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public static class SavWav {
	public const int HEADER_SIZE = 44;

	/// <summary>
	/// Save AudioClip as wav file
	/// </summary>
	/// <param name="filename">Path to save that</param>
	/// <param name="clip">Unity AudioClip</param>
	/// <returns></returns>
	public static bool SaveWav(string filename, AudioClip clip) {
		if (!filename.ToLower().EndsWith(".wav")) {
			filename += ".wav";
		}

		var filepath = Path.Combine(Application.persistentDataPath, filename);

		Debug.Log(filepath);

		// Make sure directory exists if user is saving to sub dir.
		Directory.CreateDirectory(Path.GetDirectoryName(filepath));

		using (var fileStream = CreateEmpty(filepath)) {
			ConvertAndWrite(fileStream, clip);
			WriteHeader(fileStream, clip);
		}

		return true; // TODO: return false if there's a failure saving the file
	}

	/// <summary>
	/// Trim silence for audio
	/// </summary>
	/// <param name="clip">Unity AudioClip</param>
	/// <param name="threshold">Silence threshold</param>
	/// <param name="callback">On end trim event</param>
	/// <returns></returns>
	public static IEnumerator TrimSilence(AudioClip clip, float threshold, Action<AudioClip> callback) {
		var samples = new float[clip.samples * clip.channels];

		clip.GetData(samples, 0);

		yield return TrimSilence(new List<float>(samples), threshold, clip.channels, clip.frequency, false, false, callback);
	}

	/// <summary>
	/// Trim silence for audio
	/// </summary>
	/// <param name="samples">List of audio samples</param>
	/// <param name="threshold">Silence threshold</param>
	/// <param name="channels">Channels. 1 - mono, 2 - stereo</param>
	/// <param name="hz">HZ. recommend is 44000</param>
	/// <param name="_3D">is 3d sound?</param>
	/// <param name="stream">Is streaming sound?</param>
	/// <param name="callback">On end trim event></param>
	/// <returns></returns>
	public static IEnumerator TrimSilence(List<float> samples, float threshold, int channels, int hz, bool _3D, bool stream, Action<AudioClip> callback) {
		int i;

		for (i = 0; i < samples.Count; i++) {
			if (i % 22050 == 0)
				yield return null;
			if (Mathf.Abs(samples[i]) > threshold)
				break;
		}
		yield return null;

		samples.RemoveRange(0, i);
		yield return null;

		for (i = samples.Count - 1; i > 0; i--) {
			if (i % 22050 == 0)
				yield return null;
			if (Mathf.Abs(samples[i]) > threshold)
				break;
		}
		yield return null;

		if (samples.Count != 0)
			samples.RemoveRange(i, samples.Count - i);
		yield return null;

		if (samples.Count == 0)
			samples.Add(0);

		AudioClip clip = AudioClip.Create("TempClip", samples.Count, channels, hz, _3D, stream);
		clip.SetData(samples.ToArray(), 0);
		yield return null;
		callback(clip);
	}

	/// <summary>
	/// Trim silence for audio
	/// </summary>
	/// <param name="clip">Unity AudioClip</param>
	/// <param name="threshold">Silence threshold</param>
	/// <param name="callback">On end trim event></param>
	/// <returns></returns>
	public static IEnumerator TrimSilenceAll(AudioClip clip, float threshold, Action<AudioClip> callback) {
		var samples = new float[clip.samples * clip.channels];

		clip.GetData(samples, 0);
		yield return TrimSilenceAll(new List<float>(samples), threshold, clip.channels, clip.frequency, false, false, callback);
	}

	/// <summary>
	/// Trim silence for audio
	/// </summary>
	/// <param name="samples">List of audio samples</param>
	/// <param name="threshold">Silence threshold</param>
	/// <param name="channels">Channels. 1 - mono, 2 - stereo</param>
	/// <param name="hz">HZ. recommend is 44000</param>
	/// <param name="_3D">is 3d sound?</param>
	/// <param name="stream">Is streaming sound?</param>
	/// <param name="callback">On end trim event></param>
	/// <returns></returns>
	public static IEnumerator TrimSilenceAll(List<float> samples, float threshold, int channels, int hz, bool _3D, bool stream, Action<AudioClip> callback) {
		List<float> newSamples = new List<float>(samples.Count);
		for (int i = 0; i < samples.Count; ++i) {
			if (i % 22050 == 0)
				yield return null;
			if (Mathf.Abs(samples[i]) > threshold)
				newSamples.Add(samples[i]);
		}

		yield return null;
		var clip = AudioClip.Create("TempClip", newSamples.Count, channels, hz, _3D, stream);

		clip.SetData(newSamples.ToArray(), 0);

		yield return null;
		callback(clip);
	}

	/// <summary>
	/// Write riff header
	/// </summary>
	public static void WriteHeader(MemoryStream stream, AudioClip clip) {
		var hz = clip.frequency;
		var channels = clip.channels;
		var samples = clip.samples;

		stream.Seek(0, SeekOrigin.Begin);

		Byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
		stream.Write(riff, 0, 4);

		Byte[] chunkSize = BitConverter.GetBytes(stream.Length - 8);
		stream.Write(chunkSize, 0, 4);

		Byte[] wave = System.Text.Encoding.ASCII.GetBytes("WAVE");
		stream.Write(wave, 0, 4);

		Byte[] fmt = System.Text.Encoding.ASCII.GetBytes("fmt ");
		stream.Write(fmt, 0, 4);

		Byte[] subChunk1 = BitConverter.GetBytes(16);
		stream.Write(subChunk1, 0, 4);

		UInt16 two = 2;
		UInt16 one = 1;

		Byte[] audioFormat = BitConverter.GetBytes(one);
		stream.Write(audioFormat, 0, 2);

		Byte[] numChannels = BitConverter.GetBytes(channels);
		stream.Write(numChannels, 0, 2);

		Byte[] sampleRate = BitConverter.GetBytes(hz);
		stream.Write(sampleRate, 0, 4);

		Byte[] byteRate = BitConverter.GetBytes(hz * channels * 2); // sampleRate * bytesPerSample*number of channels, here 44100*2*2
		stream.Write(byteRate, 0, 4);

		UInt16 blockAlign = (ushort)(channels * 2);
		stream.Write(BitConverter.GetBytes(blockAlign), 0, 2);

		UInt16 bps = 16;
		Byte[] bitsPerSample = BitConverter.GetBytes(bps);
		stream.Write(bitsPerSample, 0, 2);

		Byte[] datastring = System.Text.Encoding.UTF8.GetBytes("data");
		stream.Write(datastring, 0, 4);

		Byte[] subChunk2 = BitConverter.GetBytes(samples * channels * 2);
		stream.Write(subChunk2, 0, 4);

		stream.Seek(0, SeekOrigin.Begin);
		var bytes = new byte[] { (byte)stream.ReadByte(), (byte)stream.ReadByte(), (byte)stream.ReadByte(), (byte)stream.ReadByte() };
		string s = Encoding.ASCII.GetString(bytes);
		Debug.Log(s);
		stream.Seek(0, SeekOrigin.Begin);
	}

	/// <summary>
	/// Create empty file
	/// </summary>
	/// <param name="filepath">Path</param>
	/// <returns></returns>
	static FileStream CreateEmpty(string filepath) {
		var fileStream = new FileStream(filepath, FileMode.Create);
		byte emptyByte = new byte();

		for (int i = 0; i < HEADER_SIZE; i++) //preparing the header
		{
			fileStream.WriteByte(emptyByte);
		}

		return fileStream;
	}

	static void ConvertAndWrite(FileStream fileStream, AudioClip clip) {

		var samples = new float[clip.samples];

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

		fileStream.Write(bytesData, 0, bytesData.Length);
	}

	static void WriteHeader(FileStream stream, AudioClip clip) {
		var hz = clip.frequency;
		var channels = clip.channels;
		var samples = clip.samples;

		stream.Seek(0, SeekOrigin.Begin);

		Byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
		stream.Write(riff, 0, 4);

		Byte[] chunkSize = BitConverter.GetBytes(stream.Length - 8);
		stream.Write(chunkSize, 0, 4);

		Byte[] wave = System.Text.Encoding.ASCII.GetBytes("WAVE");
		stream.Write(wave, 0, 4);

		Byte[] fmt = System.Text.Encoding.ASCII.GetBytes("fmt ");
		stream.Write(fmt, 0, 4);

		Byte[] subChunk1 = BitConverter.GetBytes(16);
		stream.Write(subChunk1, 0, 4);

		UInt16 two = 2;
		UInt16 one = 1;

		Byte[] audioFormat = BitConverter.GetBytes(one);
		stream.Write(audioFormat, 0, 2);

		Byte[] numChannels = BitConverter.GetBytes(channels);
		stream.Write(numChannels, 0, 2);

		Byte[] sampleRate = BitConverter.GetBytes(hz);
		stream.Write(sampleRate, 0, 4);

		Byte[] byteRate = BitConverter.GetBytes(hz * channels * 2); // sampleRate * bytesPerSample*number of channels, here 44100*2*2
		stream.Write(byteRate, 0, 4);

		UInt16 blockAlign = (ushort)(channels * 2);
		stream.Write(BitConverter.GetBytes(blockAlign), 0, 2);

		UInt16 bps = 16;
		Byte[] bitsPerSample = BitConverter.GetBytes(bps);
		stream.Write(bitsPerSample, 0, 2);

		Byte[] datastring = System.Text.Encoding.UTF8.GetBytes("data");
		stream.Write(datastring, 0, 4);

		Byte[] subChunk2 = BitConverter.GetBytes(samples * channels * 2);
		stream.Write(subChunk2, 0, 4);

		stream.Seek(0, SeekOrigin.Begin);
		var bytes = new byte[] { (byte)stream.ReadByte(), (byte)stream.ReadByte(), (byte)stream.ReadByte(), (byte)stream.ReadByte() };
		string s = Encoding.ASCII.GetString(bytes);
		Debug.Log(s);
		stream.Seek(0, SeekOrigin.Begin);
	}
}