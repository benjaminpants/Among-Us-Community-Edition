using System;
using UnityEngine;

// Token: 0x02000018 RID: 24
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class RadioWaveBehaviour : MonoBehaviour
{
	// Token: 0x06000069 RID: 105 RVA: 0x0000C890 File Offset: 0x0000AA90
	public void Start()
	{
		this.mesh = new Mesh();
		base.GetComponent<MeshFilter>().mesh = this.mesh;
		this.mesh.MarkDynamic();
		this.vecs = new Vector3[this.NumPoints];
		int[] array = new int[this.NumPoints];
		for (int i = 0; i < this.vecs.Length; i++)
		{
			Vector3 vector = this.vecs[i];
			vector.x = this.Width.Lerp((float)i / (float)this.vecs.Length);
			vector.y = this.Height.Next();
			this.vecs[i] = vector;
			array[i] = i;
		}
		this.mesh.vertices = this.vecs;
		this.mesh.SetIndices(array, MeshTopology.LineStrip, 0);
	}

	// Token: 0x0600006A RID: 106 RVA: 0x0000C964 File Offset: 0x0000AB64
	public void Update()
	{
		this.timer += Time.deltaTime;
		if (this.timer > this.TickRate)
		{
			this.timer = 0f;
			this.Tick++;
			for (int i = 0; i < this.vecs.Length - this.Skip; i++)
			{
				this.vecs[i].y = this.vecs[i + this.Skip].y;
			}
			if (this.Random)
			{
				for (int j = 1; j <= this.Skip; j++)
				{
					this.vecs[this.vecs.Length - j].y = this.Height.Next();
				}
			}
			else
			{
				float num = 1f - this.NoiseLevel;
				for (int k = 0; k < this.Skip; k++)
				{
					this.vecs[this.vecs.Length - 1 - this.Skip + k].y = Mathf.Sin(((float)this.Tick + (float)k / (float)this.Skip) * this.Frequency) * this.Height.max * num + this.Height.Next() * this.NoiseLevel;
				}
			}
			this.mesh.vertices = this.vecs;
		}
	}

	// Token: 0x04000088 RID: 136
	public int NumPoints = 128;

	// Token: 0x04000089 RID: 137
	public FloatRange Width;

	// Token: 0x0400008A RID: 138
	public FloatRange Height;

	// Token: 0x0400008B RID: 139
	private Mesh mesh;

	// Token: 0x0400008C RID: 140
	private Vector3[] vecs;

	// Token: 0x0400008D RID: 141
	public float TickRate = 0.1f;

	// Token: 0x0400008E RID: 142
	private float timer;

	// Token: 0x0400008F RID: 143
	public int Skip = 2;

	// Token: 0x04000090 RID: 144
	public float Frequency = 5f;

	// Token: 0x04000091 RID: 145
	private int Tick;

	// Token: 0x04000092 RID: 146
	public bool Random;

	// Token: 0x04000093 RID: 147
	[Range(0f, 1f)]
	public float NoiseLevel;
}
