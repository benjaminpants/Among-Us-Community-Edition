using System;
using System.Runtime.InteropServices;
using UnityEngine;

// Token: 0x02000170 RID: 368
public class CourseMinigame : Minigame
{
	// Token: 0x060007A4 RID: 1956 RVA: 0x0002B49C File Offset: 0x0002969C
	public override void Begin(PlayerTask task)
	{
		base.Begin(task);
		this.PathPoints = new Vector3[this.NumPoints];
		this.Stars = new CourseStarBehaviour[this.NumPoints];
		this.Dots = new SpriteRenderer[this.NumPoints];
		for (int i = 0; i < this.PathPoints.Length; i++)
		{
			this.PathPoints[i].x = this.XRange.Lerp((float)i / ((float)this.PathPoints.Length - 1f));
			do
			{
				this.PathPoints[i].y = this.YRange.Next();
			}
			while (i > 0 && Mathf.Abs(this.PathPoints[i - 1].y - this.PathPoints[i].y) < this.YRange.Width / 4f);
			this.Dots[i] = UnityEngine.Object.Instantiate<SpriteRenderer>(this.DotPrefab, base.transform);
			this.Dots[i].transform.localPosition = this.PathPoints[i];
			if (i == 0)
			{
				this.Dots[i].sprite = this.DotLight;
			}
			else
			{
				if (i == 1)
				{
					this.Ship.transform.localPosition = this.PathPoints[0];
					this.Ship.transform.eulerAngles = new Vector3(0f, 0f, Vector2.up.AngleSigned(this.PathPoints[1] - this.PathPoints[0]));
				}
				this.Stars[i] = UnityEngine.Object.Instantiate<CourseStarBehaviour>(this.StarPrefab, base.transform);
				this.Stars[i].transform.localPosition = this.PathPoints[i];
				if (i == this.PathPoints.Length - 1)
				{
					this.Destination.transform.localPosition = this.PathPoints[i];
				}
			}
		}
		this.Path.positionCount = this.PathPoints.Length;
		this.Path.SetPositions(this.PathPoints);
	}

	// Token: 0x060007A5 RID: 1957 RVA: 0x0002B6C8 File Offset: 0x000298C8
	public void FixedUpdate()
	{
		float num = this.Converter.GetFloat(this.MyNormTask.Data);
		int num2 = (int)num;
		Vector2 b = this.PathPoints[num2];
		this.myController.Update();
		DragState dragState = this.myController.CheckDrag(this.Ship, false);
		if (dragState != DragState.NoTouch)
		{
			if (dragState == DragState.Dragging)
			{
				if (num < (float)(this.PathPoints.Length - 1))
				{
					Vector2 vector = (Vector2)this.PathPoints[num2 + 1] - b;
					Vector2 a = new Vector2(1f, vector.y / vector.x);
					Vector2 vector2 = (Vector2)base.transform.InverseTransformPoint(this.myController.DragPosition) - b;
					if (vector2.x > 0f)
					{
						Vector2 vector3 = a * vector2.x;
						if (Mathf.Abs(vector3.y - vector2.y) < 0.5f)
						{
							num = (float)num2 + Mathf.Min(1f, vector2.x / vector.x);
							Vector3 localPosition = vector3 + b;
							localPosition.z = -1f;
							this.Ship.transform.localPosition = localPosition;
							this.Ship.transform.localPosition = localPosition;
							this.Ship.transform.eulerAngles = new Vector3(0f, 0f, Vector2.up.AngleSigned(vector));
						}
						else
						{
							this.myController.Reset();
						}
					}
				}
				else
				{
					Vector3 localPosition2 = this.PathPoints[this.PathPoints.Length - 1];
					localPosition2.z = -1f;
					this.Ship.transform.localPosition = localPosition2;
				}
			}
		}
		else if (num < (float)(this.PathPoints.Length - 1))
		{
			Vector2 vector4 = (Vector2)this.PathPoints[num2 + 1] - b;
			Vector2 a2 = new Vector2(1f, vector4.y / vector4.x);
			num = Mathf.Max((float)num2, Mathf.Lerp(num, (float)num2, Time.deltaTime * 5f));
			Vector3 localPosition3 = a2 * (num - (float)num2) + b;
			localPosition3.z = -1f;
			this.Ship.transform.localPosition = localPosition3;
		}
		else
		{
			Vector3 localPosition4 = this.PathPoints[this.PathPoints.Length - 1];
			localPosition4.z = -1f;
			this.Ship.transform.localPosition = localPosition4;
		}
		if ((int)num > num2 && this.Stars[num2 + 1])
		{
			UnityEngine.Object.Destroy(this.Stars[num2 + 1].gameObject);
			this.Dots[num2 + 1].sprite = this.DotLight;
			if (num2 == this.PathPoints.Length - 2)
			{
				if (Constants.ShouldPlaySfx())
				{
					SoundManager.Instance.PlaySound(this.SetCourseLastSound, false, 1f).volume = 0.7f;
				}
				this.Destination.Speed *= 5f;
				this.MyNormTask.NextStep();
				base.StartCoroutine(base.CoStartClose(0.75f));
			}
			else if (Constants.ShouldPlaySfx())
			{
				SoundManager.Instance.PlaySound(this.SetCourseSound, false, 1f).volume = 0.7f;
			}
		}
		this.Converter.GetBytes(num, this.MyNormTask.Data);
		this.SetLineDivision(num);
	}

	// Token: 0x060007A6 RID: 1958 RVA: 0x0002BA70 File Offset: 0x00029C70
	private void SetLineDivision(float curVec)
	{
		int num = (int)curVec;
		float num2 = 0f;
		int num3 = 0;
		while ((float)num3 <= curVec && num3 < this.PathPoints.Length - 1)
		{
			float num4 = Vector2.Distance(this.PathPoints[num3], this.PathPoints[num3 + 1]);
			if (num3 == num)
			{
				num4 *= curVec - (float)num3;
			}
			num2 += num4;
			num3++;
		}
		this.lineTimer -= Time.fixedDeltaTime;
		Vector2 value = new Vector2(this.lineTimer, 0f);
		this.Path.material.SetTextureOffset("_MainTex", value);
		this.Path.material.SetTextureOffset("_AltTex", value);
		this.Path.material.SetFloat("_Perc", num2 + this.lineTimer / 8f);
	}

	// Token: 0x0400077A RID: 1914
	public CourseStarBehaviour StarPrefab;

	// Token: 0x0400077B RID: 1915
	public CourseStarBehaviour[] Stars;

	// Token: 0x0400077C RID: 1916
	public SpriteRenderer DotPrefab;

	// Token: 0x0400077D RID: 1917
	public Sprite DotLight;

	// Token: 0x0400077E RID: 1918
	public SpriteRenderer[] Dots;

	// Token: 0x0400077F RID: 1919
	public Collider2D Ship;

	// Token: 0x04000780 RID: 1920
	public CourseStarBehaviour Destination;

	// Token: 0x04000781 RID: 1921
	public Vector3[] PathPoints;

	// Token: 0x04000782 RID: 1922
	public int NumPoints;

	// Token: 0x04000783 RID: 1923
	public FloatRange XRange;

	// Token: 0x04000784 RID: 1924
	public FloatRange YRange;

	// Token: 0x04000785 RID: 1925
	public LineRenderer Path;

	// Token: 0x04000786 RID: 1926
	public Controller myController = new Controller();

	// Token: 0x04000787 RID: 1927
	public float lineTimer;

	// Token: 0x04000788 RID: 1928
	private CourseMinigame.UIntFloat Converter;

	// Token: 0x04000789 RID: 1929
	public AudioClip SetCourseSound;

	// Token: 0x0400078A RID: 1930
	public AudioClip SetCourseLastSound;

	// Token: 0x02000171 RID: 369
	[StructLayout(LayoutKind.Explicit)]
	private struct UIntFloat
	{
		// Token: 0x060007A8 RID: 1960 RVA: 0x00006B49 File Offset: 0x00004D49
		public float GetFloat(byte[] bytes)
		{
			this.IntValue = ((int)bytes[0] | (int)bytes[1] << 8 | (int)bytes[2] << 16 | (int)bytes[3] << 24);
			return this.FloatValue;
		}

		// Token: 0x060007A9 RID: 1961 RVA: 0x0002BB50 File Offset: 0x00029D50
		public void GetBytes(float value, byte[] bytes)
		{
			this.FloatValue = value;
			bytes[0] = (byte)(this.IntValue & 255);
			bytes[1] = (byte)(this.IntValue >> 8 & 255);
			bytes[2] = (byte)(this.IntValue >> 16 & 255);
			bytes[3] = (byte)(this.IntValue >> 24 & 255);
		}

		// Token: 0x0400078B RID: 1931
		[FieldOffset(0)]
		public float FloatValue;

		// Token: 0x0400078C RID: 1932
		[FieldOffset(0)]
		public int IntValue;
	}
}
