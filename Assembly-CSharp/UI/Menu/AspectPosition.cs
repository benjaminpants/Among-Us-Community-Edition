using UnityEngine;

public class AspectPosition : MonoBehaviour
{
	public enum EdgeAlignments
	{
		RightBottom = 6,
		LeftBottom = 5,
		RightTop = 10,
		Left = 1,
		Right = 2,
		Top = 8,
		Bottom = 4,
		LeftTop = 9
	}

	public Camera parentCam;

	private const int LeftFlag = 1;

	private const int RightFlag = 2;

	private const int BottomFlag = 4;

	private const int TopFlag = 8;

	public Vector3 DistanceFromEdge;

	public EdgeAlignments Alignment;

	private void OnEnable()
	{
		AdjustPosition();
		ResolutionManager.ResolutionChanged += AdjustPosition;
	}

	private void OnDisable()
	{
		ResolutionManager.ResolutionChanged -= AdjustPosition;
	}

	public void AdjustPosition()
	{
		Camera camera = (parentCam ? parentCam : Camera.main);
		_ = camera.aspect;
		AdjustPosition(camera.aspect);
	}

	public void AdjustPosition(float aspect)
	{
		float orthographicSize = (parentCam ? parentCam : Camera.main).orthographicSize;
		base.transform.localPosition = ComputePosition(Alignment, DistanceFromEdge, orthographicSize, aspect);
	}

	public static Vector3 ComputePosition(EdgeAlignments alignment, Vector3 relativePos)
	{
		Camera main = Camera.main;
		float aspect = main.aspect;
		float orthographicSize = main.orthographicSize;
		return ComputePosition(alignment, relativePos, orthographicSize, aspect);
	}

	public static Vector3 ComputePosition(EdgeAlignments alignment, Vector3 relativePos, float cHeight, float aspect)
	{
		float num = cHeight * aspect;
		Vector3 result = relativePos;
		if ((alignment & EdgeAlignments.Left) != 0)
		{
			result.x -= num;
		}
		else if ((alignment & EdgeAlignments.Right) != 0)
		{
			result.x = num - result.x;
		}
		if ((alignment & EdgeAlignments.Bottom) != 0)
		{
			result.y -= cHeight;
		}
		else if ((alignment & EdgeAlignments.Top) != 0)
		{
			result.y = cHeight - result.y;
		}
		return result;
	}
}
