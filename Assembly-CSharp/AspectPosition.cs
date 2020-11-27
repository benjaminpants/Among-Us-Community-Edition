using System;
using UnityEngine;

// Token: 0x020000B5 RID: 181
public class AspectPosition : MonoBehaviour
{
	// Token: 0x060003CE RID: 974 RVA: 0x0000470D File Offset: 0x0000290D
	private void OnEnable()
	{
		this.AdjustPosition();
		ResolutionManager.ResolutionChanged += this.AdjustPosition;
	}

	// Token: 0x060003CF RID: 975 RVA: 0x00004726 File Offset: 0x00002926
	private void OnDisable()
	{
		ResolutionManager.ResolutionChanged -= this.AdjustPosition;
	}

	// Token: 0x060003D0 RID: 976 RVA: 0x00018B60 File Offset: 0x00016D60
	public void AdjustPosition()
	{
		Camera camera = this.parentCam ? this.parentCam : Camera.main;
		float aspect = camera.aspect;
		this.AdjustPosition(camera.aspect);
	}

	// Token: 0x060003D1 RID: 977 RVA: 0x00018B9C File Offset: 0x00016D9C
	public void AdjustPosition(float aspect)
	{
		float orthographicSize = (this.parentCam ? this.parentCam : Camera.main).orthographicSize;
		base.transform.localPosition = AspectPosition.ComputePosition(this.Alignment, this.DistanceFromEdge, orthographicSize, aspect);
	}

	// Token: 0x060003D2 RID: 978 RVA: 0x00018BE8 File Offset: 0x00016DE8
	public static Vector3 ComputePosition(AspectPosition.EdgeAlignments alignment, Vector3 relativePos)
	{
		Camera main = Camera.main;
		float aspect = main.aspect;
		float orthographicSize = main.orthographicSize;
		return AspectPosition.ComputePosition(alignment, relativePos, orthographicSize, aspect);
	}

	// Token: 0x060003D3 RID: 979 RVA: 0x00018C10 File Offset: 0x00016E10
	public static Vector3 ComputePosition(AspectPosition.EdgeAlignments alignment, Vector3 relativePos, float cHeight, float aspect)
	{
		float num = cHeight * aspect;
		Vector3 vector = relativePos;
		if ((alignment & AspectPosition.EdgeAlignments.Left) != (AspectPosition.EdgeAlignments)0)
		{
			vector.x -= num;
		}
		else if ((alignment & AspectPosition.EdgeAlignments.Right) != (AspectPosition.EdgeAlignments)0)
		{
			vector.x = num - vector.x;
		}
		if ((alignment & AspectPosition.EdgeAlignments.Bottom) != (AspectPosition.EdgeAlignments)0)
		{
			vector.y -= cHeight;
		}
		else if ((alignment & AspectPosition.EdgeAlignments.Top) != (AspectPosition.EdgeAlignments)0)
		{
			vector.y = cHeight - vector.y;
		}
		return vector;
	}

	// Token: 0x040003B4 RID: 948
	public Camera parentCam;

	// Token: 0x040003B5 RID: 949
	private const int LeftFlag = 1;

	// Token: 0x040003B6 RID: 950
	private const int RightFlag = 2;

	// Token: 0x040003B7 RID: 951
	private const int BottomFlag = 4;

	// Token: 0x040003B8 RID: 952
	private const int TopFlag = 8;

	// Token: 0x040003B9 RID: 953
	public Vector3 DistanceFromEdge;

	// Token: 0x040003BA RID: 954
	public AspectPosition.EdgeAlignments Alignment;

	// Token: 0x020000B6 RID: 182
	public enum EdgeAlignments
	{
		// Token: 0x040003BC RID: 956
		RightBottom = 6,
		// Token: 0x040003BD RID: 957
		LeftBottom = 5,
		// Token: 0x040003BE RID: 958
		RightTop = 10,
		// Token: 0x040003BF RID: 959
		Left = 1,
		// Token: 0x040003C0 RID: 960
		Right,
		// Token: 0x040003C1 RID: 961
		Top = 8,
		// Token: 0x040003C2 RID: 962
		Bottom = 4,
		// Token: 0x040003C3 RID: 963
		LeftTop = 9
	}
}
