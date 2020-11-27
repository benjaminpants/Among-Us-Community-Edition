using System;
using UnityEngine;

// Token: 0x02000015 RID: 21
public class CloudGenerator : MonoBehaviour
{
	// Token: 0x06000061 RID: 97 RVA: 0x0000C160 File Offset: 0x0000A360
	public void Start()
	{
		this.UvCache = new Vector2[this.CloudImages.Length][];
		this.ExtentCache = new Vector2[this.CloudImages.Length];
		for (int i = 0; i < this.CloudImages.Length; i++)
		{
			Sprite sprite = this.CloudImages[i];
			this.UvCache[i] = sprite.uv;
			this.ExtentCache[i] = sprite.bounds.extents;
		}
		this.stars = new CloudGenerator.Cloud[this.NumClouds];
		this.verts = new Vector3[this.NumClouds * 4];
		Vector2[] array = new Vector2[this.NumClouds * 4];
		int[] array2 = new int[this.NumClouds * 6];
		this.SetDirection(this.Direction);
		MeshFilter component = base.GetComponent<MeshFilter>();
		this.mesh = new Mesh();
		this.mesh.MarkDynamic();
		component.mesh = this.mesh;
		Vector3 vector = default(Vector3);
		for (int j = 0; j < this.stars.Length; j++)
		{
			CloudGenerator.Cloud cloud = this.stars[j];
			int num = cloud.CloudIdx = this.CloudImages.RandomIdx<Sprite>();
			Vector2 vector2 = this.ExtentCache[num];
			Vector2[] array3 = this.UvCache[num];
			float num2 = FloatRange.Next(-1f, 1f) * this.Length;
			float num3 = FloatRange.Next(-1f, 1f) * this.Width;
			float num4 = cloud.PositionX = num2 * this.NormDir.x + num3 * this.Tangent.x;
			float num5 = cloud.PositionY = num2 * this.NormDir.y + num3 * this.Tangent.y;
			cloud.Rate = this.Rates.Next();
			this.stars[j] = cloud;
			int num6 = j * 4;
			vector.x = num4 - vector2.x;
			vector.y = num5 + vector2.y;
			this.verts[num6] = vector;
			vector.x = num4 + vector2.x;
			this.verts[num6 + 1] = vector;
			vector.x = num4 - vector2.x;
			vector.y = num5 - vector2.y;
			this.verts[num6 + 2] = vector;
			vector.x = num4 + vector2.x;
			this.verts[num6 + 3] = vector;
			array[num6] = array3[0];
			array[num6 + 1] = array3[1];
			array[num6 + 2] = array3[2];
			array[num6 + 3] = array3[3];
			int num7 = j * 6;
			array2[num7] = num6;
			array2[num7 + 1] = num6 + 1;
			array2[num7 + 2] = num6 + 2;
			array2[num7 + 3] = num6 + 2;
			array2[num7 + 4] = num6 + 1;
			array2[num7 + 5] = num6 + 3;
		}
		this.mesh.vertices = this.verts;
		this.mesh.uv = array;
		this.mesh.SetIndices(array2, MeshTopology.Triangles, 0);
	}

	// Token: 0x06000062 RID: 98 RVA: 0x0000C4BC File Offset: 0x0000A6BC
	private void FixedUpdate()
	{
		float num = -0.99f * this.Length;
		Vector2 vector = this.Direction * Time.fixedDeltaTime;
		Vector3 vector2 = default(Vector3);
		for (int i = 0; i < this.stars.Length; i++)
		{
			int num2 = i * 4;
			CloudGenerator.Cloud cloud = this.stars[i];
			float num3 = cloud.PositionX;
			float num4 = cloud.PositionY;
			Vector2 vector3 = this.ExtentCache[cloud.CloudIdx];
			float rate = cloud.Rate;
			num3 += rate * vector.x;
			num4 += rate * vector.y;
			if (this.OrthoDistance(num3, num4) > this.Length)
			{
				float num5 = FloatRange.Next(-1f, 1f) * this.Width;
				num3 = num * this.NormDir.x + num5 * this.Tangent.x;
				num4 = num * this.NormDir.y + num5 * this.Tangent.y;
				cloud.Rate = this.Rates.Next();
			}
			cloud.PositionX = num3;
			cloud.PositionY = num4;
			this.stars[i] = cloud;
			vector2.x = num3 - vector3.x;
			vector2.y = num4 + vector3.y;
			this.verts[num2] = vector2;
			vector2.x = num3 + vector3.x;
			this.verts[num2 + 1] = vector2;
			vector2.x = num3 - vector3.x;
			vector2.y = num4 - vector3.y;
			this.verts[num2 + 2] = vector2;
			vector2.x = num3 + vector3.x;
			this.verts[num2 + 3] = vector2;
		}
		this.mesh.vertices = this.verts;
	}

	// Token: 0x06000063 RID: 99 RVA: 0x0000C6B4 File Offset: 0x0000A8B4
	public void SetDirection(Vector2 dir)
	{
		this.Direction = dir;
		this.NormDir = this.Direction.normalized;
		this.Tangent = new Vector2(-this.NormDir.y, this.NormDir.x);
		this.tanLen = Mathf.Sqrt(this.Tangent.y * this.Tangent.y + this.Tangent.x * this.Tangent.x);
	}

	// Token: 0x06000064 RID: 100 RVA: 0x000024E9 File Offset: 0x000006E9
	private float OrthoDistance(float pointx, float pointy)
	{
		return (this.Tangent.y * pointx - this.Tangent.x * pointy) / this.tanLen;
	}

	// Token: 0x0400006F RID: 111
	public Sprite[] CloudImages;

	// Token: 0x04000070 RID: 112
	private Vector2[][] UvCache;

	// Token: 0x04000071 RID: 113
	private Vector2[] ExtentCache;

	// Token: 0x04000072 RID: 114
	public int NumClouds = 500;

	// Token: 0x04000073 RID: 115
	public float Length = 25f;

	// Token: 0x04000074 RID: 116
	public float Width = 25f;

	// Token: 0x04000075 RID: 117
	public Vector2 Direction = new Vector2(1f, 0f);

	// Token: 0x04000076 RID: 118
	private Vector2 NormDir = new Vector2(1f, 0f);

	// Token: 0x04000077 RID: 119
	private Vector2 Tangent = new Vector2(0f, 1f);

	// Token: 0x04000078 RID: 120
	private float tanLen;

	// Token: 0x04000079 RID: 121
	public FloatRange Rates = new FloatRange(0.25f, 1f);

	// Token: 0x0400007A RID: 122
	[HideInInspector]
	private CloudGenerator.Cloud[] stars;

	// Token: 0x0400007B RID: 123
	[HideInInspector]
	private Vector3[] verts;

	// Token: 0x0400007C RID: 124
	[HideInInspector]
	private Mesh mesh;

	// Token: 0x02000016 RID: 22
	private struct Cloud
	{
		// Token: 0x0400007D RID: 125
		public int CloudIdx;

		// Token: 0x0400007E RID: 126
		public float Rate;

		// Token: 0x0400007F RID: 127
		public float PositionX;

		// Token: 0x04000080 RID: 128
		public float PositionY;
	}
}
