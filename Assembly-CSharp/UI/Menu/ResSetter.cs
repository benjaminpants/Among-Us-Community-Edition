using System.IO;
using UnityEngine;

public class ResSetter : MonoBehaviour
{
	public int Width = 1438;

	public int Height = 810;

	private int cnt;

	public void Start()
	{
		Screen.SetResolution(Width, Height, fullscreen: false);
	}

	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.S))
		{
			Directory.CreateDirectory("C:\\AmongUsSS");
			ScreenCapture.CaptureScreenshot($"C:\\AmongUsSS\\Screenshot-{cnt++}.png");
		}
	}
}
