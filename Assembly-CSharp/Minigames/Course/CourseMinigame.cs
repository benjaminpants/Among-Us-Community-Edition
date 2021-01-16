using System.Runtime.InteropServices;
using UnityEngine;

public class CourseMinigame : Minigame
{
	[StructLayout(LayoutKind.Explicit)]
	private struct UIntFloat
	{
		[FieldOffset(0)]
		public float FloatValue;

		[FieldOffset(0)]
		public int IntValue;

		public float GetFloat(byte[] bytes)
		{
			IntValue = bytes[0] | (bytes[1] << 8) | (bytes[2] << 16) | (bytes[3] << 24);
			return FloatValue;
		}

		public void GetBytes(float value, byte[] bytes)
		{
			FloatValue = value;
			bytes[0] = (byte)((uint)IntValue & 0xFFu);
			bytes[1] = (byte)((uint)(IntValue >> 8) & 0xFFu);
			bytes[2] = (byte)((uint)(IntValue >> 16) & 0xFFu);
			bytes[3] = (byte)((uint)(IntValue >> 24) & 0xFFu);
		}
	}

	public CourseStarBehaviour StarPrefab;

	public CourseStarBehaviour[] Stars;

	public SpriteRenderer DotPrefab;

	public Sprite DotLight;

	public SpriteRenderer[] Dots;

	public Collider2D Ship;

	public CourseStarBehaviour Destination;

	public Vector3[] PathPoints;

	public int NumPoints;

	public FloatRange XRange;

	public FloatRange YRange;

	public LineRenderer Path;

	public Controller myController = new Controller();

	public float lineTimer;

	private UIntFloat Converter;

	public AudioClip SetCourseSound;

	public AudioClip SetCourseLastSound;

	public override void Begin(PlayerTask task)
	{
		base.Begin(task);
		NumPoints = (int)(NumPoints * GameOptionsData.TaskDifficultyMult[PlayerControl.GameOptions.TaskDifficulty]);
		PathPoints = new Vector3[NumPoints];
		Stars = new CourseStarBehaviour[NumPoints];
		Dots = new SpriteRenderer[NumPoints];
		for (int i = 0; i < PathPoints.Length; i++)
		{
			PathPoints[i].x = XRange.Lerp((float)i / ((float)PathPoints.Length - 1f));
			do
			{
				PathPoints[i].y = YRange.Next();
			}
			while (i > 0 && Mathf.Abs(PathPoints[i - 1].y - PathPoints[i].y) < YRange.Width / 4f);
			Dots[i] = Object.Instantiate(DotPrefab, base.transform);
			Dots[i].transform.localPosition = PathPoints[i];
			switch (i)
			{
			case 0:
				Dots[i].sprite = DotLight;
				continue;
			case 1:
				Ship.transform.localPosition = PathPoints[0];
				Ship.transform.eulerAngles = new Vector3(0f, 0f, Vector2.up.AngleSigned(PathPoints[1] - PathPoints[0]));
				break;
			}
			Stars[i] = Object.Instantiate(StarPrefab, base.transform);
			Stars[i].transform.localPosition = PathPoints[i];
			if (i == PathPoints.Length - 1)
			{
				Destination.transform.localPosition = PathPoints[i];
			}
		}
		Path.positionCount = PathPoints.Length;
		Path.SetPositions(PathPoints);
	}

	public void FixedUpdate()
	{
		float num = Converter.GetFloat(MyNormTask.Data);
		int num2 = (int)num;
		Vector2 b = PathPoints[num2];
		myController.Update();
		switch (myController.CheckDrag(Ship))
		{
		case DragState.Dragging:
			if (num < (float)(PathPoints.Length - 1))
			{
				Vector2 vector2 = (Vector2)PathPoints[num2 + 1] - b;
				Vector2 a2 = new Vector2(1f, vector2.y / vector2.x);
				Vector2 vector3 = (Vector2)base.transform.InverseTransformPoint(myController.DragPosition) - b;
				if (vector3.x > 0f)
				{
					Vector2 a3 = a2 * vector3.x;
					if (Mathf.Abs(a3.y - vector3.y) < 0.5f)
					{
						num = (float)num2 + Mathf.Min(1f, vector3.x / vector2.x);
						Vector3 localPosition3 = a3 + b;
						localPosition3.z = -1f;
						Ship.transform.localPosition = localPosition3;
						Ship.transform.localPosition = localPosition3;
						Ship.transform.eulerAngles = new Vector3(0f, 0f, Vector2.up.AngleSigned(vector2));
					}
					else
					{
						myController.Reset();
					}
				}
			}
			else
			{
				Vector3 localPosition4 = PathPoints[PathPoints.Length - 1];
				localPosition4.z = -1f;
				Ship.transform.localPosition = localPosition4;
			}
			break;
		case DragState.NoTouch:
			if (num < (float)(PathPoints.Length - 1))
			{
				Vector2 vector = (Vector2)PathPoints[num2 + 1] - b;
				Vector2 a = new Vector2(1f, vector.y / vector.x);
				num = Mathf.Max(num2, Mathf.Lerp(num, num2, Time.deltaTime * 5f));
				Vector3 localPosition = a * (num - (float)num2) + b;
				localPosition.z = -1f;
				Ship.transform.localPosition = localPosition;
			}
			else
			{
				Vector3 localPosition2 = PathPoints[PathPoints.Length - 1];
				localPosition2.z = -1f;
				Ship.transform.localPosition = localPosition2;
			}
			break;
		}
		if ((int)num > num2 && (bool)Stars[num2 + 1])
		{
			Object.Destroy(Stars[num2 + 1].gameObject);
			Dots[num2 + 1].sprite = DotLight;
			if (num2 == PathPoints.Length - 2)
			{
				if (Constants.ShouldPlaySfx())
				{
					SoundManager.Instance.PlaySound(SetCourseLastSound, loop: false).volume = 0.7f;
				}
				Destination.Speed *= 5f;
				MyNormTask.NextStep();
				StartCoroutine(CoStartClose());
			}
			else if (Constants.ShouldPlaySfx())
			{
				SoundManager.Instance.PlaySound(SetCourseSound, loop: false).volume = 0.7f;
			}
		}
		Converter.GetBytes(num, MyNormTask.Data);
		SetLineDivision(num);
	}

	private void SetLineDivision(float curVec)
	{
		int num = (int)curVec;
		float num2 = 0f;
		for (int i = 0; (float)i <= curVec && i < PathPoints.Length - 1; i++)
		{
			float num3 = Vector2.Distance(PathPoints[i], PathPoints[i + 1]);
			if (i == num)
			{
				num3 *= curVec - (float)i;
			}
			num2 += num3;
		}
		lineTimer -= Time.fixedDeltaTime;
		Vector2 value = new Vector2(lineTimer, 0f);
		Path.material.SetTextureOffset("_MainTex", value);
		Path.material.SetTextureOffset("_AltTex", value);
		Path.material.SetFloat("_Perc", num2 + lineTimer / 8f);
	}
}
