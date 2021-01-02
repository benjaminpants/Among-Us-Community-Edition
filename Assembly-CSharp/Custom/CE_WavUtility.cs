using System;
using System.IO;
using System.Text;
using UnityEngine;

public class CE_WavUtility
{
	private const int BlockSize_16Bit = 2;

	public static AudioClip ToAudioClip(string filePath)
	{
		if (!filePath.StartsWith(Application.persistentDataPath) && !filePath.StartsWith(Application.dataPath))
		{
			Debug.LogWarning(filePath);
			Debug.LogWarning("This only supports files that are stored using Unity's Application data path. \nTo load bundled resources use 'Resources.Load(\"filename\") typeof(AudioClip)' method. \nhttps://docs.unity3d.com/ScriptReference/Resources.Load.html");
			return null;
		}
		return ToAudioClip(File.ReadAllBytes(filePath));
	}

	public static AudioClip ToAudioClip(byte[] fileBytes, int offsetSamples = 0, string name = "wav")
	{
		int num = BitConverter.ToInt32(fileBytes, 16);
		FormatCode(BitConverter.ToUInt16(fileBytes, 20));
		ushort channels = BitConverter.ToUInt16(fileBytes, 22);
		int frequency = BitConverter.ToInt32(fileBytes, 24);
		ushort num2 = BitConverter.ToUInt16(fileBytes, 34);
		int num3 = 20 + num + 4;
		int dataSize = BitConverter.ToInt32(fileBytes, num3);
		float[] array = num2 switch
		{
			8 => Convert8BitByteArrayToAudioClipData(fileBytes, num3, dataSize), 
			16 => Convert16BitByteArrayToAudioClipData(fileBytes, num3, dataSize), 
			24 => Convert24BitByteArrayToAudioClipData(fileBytes, num3, dataSize), 
			32 => Convert32BitByteArrayToAudioClipData(fileBytes, num3, dataSize), 
			_ => throw new Exception(num2 + " bit depth is not supported."), 
		};
		AudioClip audioClip = AudioClip.Create(name, array.Length, channels, frequency, stream: false);
		audioClip.SetData(array, 0);
		return audioClip;
	}

	private static float[] Convert8BitByteArrayToAudioClipData(byte[] source, int headerOffset, int dataSize)
	{
		int num = BitConverter.ToInt32(source, headerOffset);
		headerOffset += 4;
		float[] array = new float[num];
		sbyte b = sbyte.MaxValue;
		for (int i = 0; i < num; i++)
		{
			array[i] = (float)(int)source[i] / (float)b;
		}
		return array;
	}

	private static float[] Convert16BitByteArrayToAudioClipData(byte[] source, int headerOffset, int dataSize)
	{
		int num = BitConverter.ToInt32(source, headerOffset);
		headerOffset += 4;
		int num2 = 2;
		int num3 = num / num2;
		float[] array = new float[num3];
		short num4 = short.MaxValue;
		for (int i = 0; i < num3; i++)
		{
			int startIndex = i * num2 + headerOffset;
			array[i] = (float)BitConverter.ToInt16(source, startIndex) / (float)num4;
		}
		return array;
	}

	private static float[] Convert24BitByteArrayToAudioClipData(byte[] source, int headerOffset, int dataSize)
	{
		int num = BitConverter.ToInt32(source, headerOffset);
		headerOffset += 4;
		int num2 = 3;
		int num3 = num / num2;
		int num4 = int.MaxValue;
		float[] array = new float[num3];
		byte[] array2 = new byte[4];
		for (int i = 0; i < num3; i++)
		{
			int srcOffset = i * num2 + headerOffset;
			Buffer.BlockCopy(source, srcOffset, array2, 1, num2);
			array[i] = (float)BitConverter.ToInt32(array2, 0) / (float)num4;
		}
		return array;
	}

	private static float[] Convert32BitByteArrayToAudioClipData(byte[] source, int headerOffset, int dataSize)
	{
		int num = BitConverter.ToInt32(source, headerOffset);
		headerOffset += 4;
		int num2 = 4;
		int num3 = num / num2;
		int num4 = int.MaxValue;
		float[] array = new float[num3];
		for (int i = 0; i < num3; i++)
		{
			int startIndex = i * num2 + headerOffset;
			array[i] = (float)BitConverter.ToInt32(source, startIndex) / (float)num4;
		}
		return array;
	}

	public static byte[] FromAudioClip(AudioClip audioClip)
	{
		string filepath;
		return FromAudioClip(audioClip, out filepath, saveAsFile: false);
	}

	public static byte[] FromAudioClip(AudioClip audioClip, out string filepath, bool saveAsFile = true, string dirname = "recordings")
	{
		MemoryStream stream = new MemoryStream();
		ushort bitDepth = 16;
		int fileSize = audioClip.samples * 2 + 44;
		WriteFileHeader(ref stream, fileSize);
		WriteFileFormat(ref stream, audioClip.channels, audioClip.frequency, bitDepth);
		WriteFileData(ref stream, audioClip, bitDepth);
		byte[] array = stream.ToArray();
		if (saveAsFile)
		{
			filepath = string.Format("{0}/{1}/{2}.{3}", Application.persistentDataPath, dirname, DateTime.UtcNow.ToString("yyMMdd-HHmmss-fff"), "wav");
			Directory.CreateDirectory(Path.GetDirectoryName(filepath));
			File.WriteAllBytes(filepath, array);
		}
		else
		{
			filepath = null;
		}
		stream.Dispose();
		return array;
	}

	private static int WriteFileHeader(ref MemoryStream stream, int fileSize)
	{
		byte[] bytes = Encoding.ASCII.GetBytes("RIFF");
		int num = 0 + WriteBytesToMemoryStream(ref stream, bytes, "ID");
		int value = fileSize - 8;
		int num2 = num + WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(value), "CHUNK_SIZE");
		byte[] bytes2 = Encoding.ASCII.GetBytes("WAVE");
		return num2 + WriteBytesToMemoryStream(ref stream, bytes2, "FORMAT");
	}

	private static int WriteFileFormat(ref MemoryStream stream, int channels, int sampleRate, ushort bitDepth)
	{
		byte[] bytes = Encoding.ASCII.GetBytes("fmt ");
		int num = 0 + WriteBytesToMemoryStream(ref stream, bytes, "FMT_ID");
		int value = 16;
		int num2 = num + WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(value), "SUBCHUNK_SIZE");
		ushort value2 = 1;
		int num3 = num2 + WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(value2), "AUDIO_FORMAT");
		ushort value3 = Convert.ToUInt16(channels);
		int num4 = num3 + WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(value3), "CHANNELS") + WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(sampleRate), "SAMPLE_RATE");
		int value4 = sampleRate * channels * BytesPerSample(bitDepth);
		int num5 = num4 + WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(value4), "BYTE_RATE");
		ushort value5 = Convert.ToUInt16(channels * BytesPerSample(bitDepth));
		return num5 + WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(value5), "BLOCK_ALIGN") + WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(bitDepth), "BITS_PER_SAMPLE");
	}

	private static int WriteFileData(ref MemoryStream stream, AudioClip audioClip, ushort bitDepth)
	{
		float[] data = new float[audioClip.samples * audioClip.channels];
		audioClip.GetData(data, 0);
		byte[] bytes = ConvertAudioClipDataToInt16ByteArray(data);
		byte[] bytes2 = Encoding.ASCII.GetBytes("data");
		int num = 0 + WriteBytesToMemoryStream(ref stream, bytes2, "DATA_ID");
		int value = Convert.ToInt32(audioClip.samples * 2);
		return num + WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(value), "SAMPLES") + WriteBytesToMemoryStream(ref stream, bytes, "DATA");
	}

	private static byte[] ConvertAudioClipDataToInt16ByteArray(float[] data)
	{
		MemoryStream memoryStream = new MemoryStream();
		int count = 2;
		short num = short.MaxValue;
		for (int i = 0; i < data.Length; i++)
		{
			memoryStream.Write(BitConverter.GetBytes(Convert.ToInt16(data[i] * (float)num)), 0, count);
		}
		byte[] result = memoryStream.ToArray();
		memoryStream.Dispose();
		return result;
	}

	private static int WriteBytesToMemoryStream(ref MemoryStream stream, byte[] bytes, string tag = "")
	{
		int num = bytes.Length;
		stream.Write(bytes, 0, num);
		return num;
	}

	public static ushort BitDepth(AudioClip audioClip)
	{
		return Convert.ToUInt16((float)(audioClip.samples * audioClip.channels) * audioClip.length / (float)audioClip.frequency);
	}

	private static int BytesPerSample(ushort bitDepth)
	{
		return (int)bitDepth / 8;
	}

	private static int BlockSize(ushort bitDepth)
	{
		return bitDepth switch
		{
			8 => 1, 
			16 => 2, 
			32 => 4, 
			_ => throw new Exception(bitDepth + " bit depth is not supported."), 
		};
	}

	private static string FormatCode(ushort code)
	{
		switch (code)
		{
		case 1:
			return "PCM";
		case 2:
			return "ADPCM";
		case 3:
			return "IEEE";
		case 7:
			return "μ-law";
		case 65534:
			return "WaveFormatExtensable";
		default:
			Debug.LogWarning("Unknown wav code format:" + code);
			return "";
		}
	}
}
