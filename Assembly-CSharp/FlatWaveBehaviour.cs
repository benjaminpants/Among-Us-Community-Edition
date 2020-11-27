using System;
using UnityEngine;

// Token: 0x02000080 RID: 128
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class FlatWaveBehaviour : MonoBehaviour
{
	// Token: 0x060002B5 RID: 693 RVA: 0x00015148 File Offset: 0x00013348
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
			vector.y = this.Center;
			if (BoolRange.Next(this.NoiseP))
			{
				vector.y += this.Delta.Next();
			}
			this.vecs[i] = vector;
			array[i] = i;
		}
		this.mesh.vertices = this.vecs;
		this.mesh.SetIndices(array, MeshTopology.LineStrip, 0);
	}

	// Token: 0x060002B6 RID: 694 RVA: 0x00015238 File Offset: 0x00013438
	public void Update()
	{
		this.timer += Time.deltaTime;
		if (this.timer > this.TickRate)
		{
			this.timer = 0f;
			for (int i = 0; i < this.vecs.Length - this.Skip; i++)
			{
				this.vecs[i].y = this.vecs[i + this.Skip].y;
			}
			for (int j = 1; j <= this.Skip; j++)
			{
				this.vecs[this.vecs.Length - j].y = this.Center;
				if (BoolRange.Next(this.NoiseP))
				{
					Vector3[] array = this.vecs;
					int num = this.vecs.Length - j;
					array[num].y = array[num].y + this.Delta.Next();
				}
			}
			this.mesh.vertices = this.vecs;
		}
	}

	// Token: 0x040002A3 RID: 675
	public int NumPoints = 128;

	// Token: 0x040002A4 RID: 676
	public FloatRange Width;

	// Token: 0x040002A5 RID: 677
	public FloatRange Delta;

	// Token: 0x040002A6 RID: 678
	public float Center;

	// Token: 0x040002A7 RID: 679
	private Mesh mesh;

	// Token: 0x040002A8 RID: 680
	private Vector3[] vecs;

	// Token: 0x040002A9 RID: 681
	public float TickRate = 0.1f;

	// Token: 0x040002AA RID: 682
	private float timer;

	// Token: 0x040002AB RID: 683
	public int Skip = 3;

	// Token: 0x040002AC RID: 684
	[Range(0f, 1f)]
	public float NoiseP = 0.5f;
}
