using System;
using System.IO;
using UnityEngine;

// Token: 0x020001AD RID: 429
public class ResSetter : MonoBehaviour
{
	// Token: 0x0600091C RID: 2332 RVA: 0x00007829 File Offset: 0x00005A29
	public void Start()
	{
		Screen.SetResolution(this.Width, this.Height, false);
	}

	// Token: 0x0600091D RID: 2333 RVA: 0x00030AE0 File Offset: 0x0002ECE0
	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.S))
		{
			Directory.CreateDirectory("C:\\AmongUsSS");
			string format = "C:\\AmongUsSS\\Screenshot-{0}.png";
			int num = this.cnt;
			this.cnt = num + 1;
			ScreenCapture.CaptureScreenshot(string.Format(format, num));
		}
	}

	// Token: 0x040008D6 RID: 2262
	public int Width = 1438;

	// Token: 0x040008D7 RID: 2263
	public int Height = 810;

	// Token: 0x040008D8 RID: 2264
	private int cnt;
}
