using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001C4 RID: 452
public class LightSource : MonoBehaviour
{
	// Token: 0x060009D4 RID: 2516 RVA: 0x000336F0 File Offset: 0x000318F0
	private void Start()
	{
		this.filter.useTriggers = true;
		this.filter.layerMask = Constants.ShadowMask;
		this.filter.useLayerMask = true;
		this.requiredDels = new Vector2[this.MinRays];
		for (int i = 0; i < this.requiredDels.Length; i++)
		{
			this.requiredDels[i] = Vector2.left.Rotate((float)i / (float)this.requiredDels.Length * 360f);
		}
		this.myMesh = new Mesh();
		this.myMesh.MarkDynamic();
		this.myMesh.name = "ShadowMesh";
		GameObject gameObject = new GameObject("LightChild");
		gameObject.layer = 10;
		gameObject.AddComponent<MeshFilter>().mesh = this.myMesh;
		Renderer renderer = gameObject.AddComponent<MeshRenderer>();
		this.Material = new Material(this.Material);
		renderer.sharedMaterial = this.Material;
		this.child = gameObject;
	}

	// Token: 0x060009D5 RID: 2517 RVA: 0x000337EC File Offset: 0x000319EC
	private void Update()
	{
		this.vertCount = 0;
		Vector3 position = base.transform.position;
		position.z -= 7f;
		this.child.transform.position = position;
		Vector2 vector = position;
		this.Material.SetFloat("_LightRadius", this.LightRadius);
		int num = Physics2D.OverlapCircleNonAlloc(vector, this.LightRadius, this.hits, Constants.ShadowMask);
		for (int i = 0; i < num; i++)
		{
			Collider2D collider2D = this.hits[i];
			if (!collider2D.isTrigger)
			{
				EdgeCollider2D edgeCollider2D = collider2D as EdgeCollider2D;
				if (edgeCollider2D)
				{
					Vector2[] points = edgeCollider2D.points;
					for (int j = 0; j < points.Length; j++)
					{
						Vector2 vector2 = edgeCollider2D.transform.TransformPoint(points[j]);
						this.del.x = vector2.x - vector.x;
						this.del.y = vector2.y - vector.y;
						this.TestBothSides(vector);
					}
				}
				else
				{
					PolygonCollider2D polygonCollider2D = collider2D as PolygonCollider2D;
					if (polygonCollider2D)
					{
						Vector2[] points2 = polygonCollider2D.points;
						for (int k = 0; k < points2.Length; k++)
						{
							Vector2 vector3 = polygonCollider2D.transform.TransformPoint(points2[k]);
							this.del.x = vector3.x - vector.x;
							this.del.y = vector3.y - vector.y;
							this.TestBothSides(vector);
						}
					}
					else
					{
						BoxCollider2D boxCollider2D = collider2D as BoxCollider2D;
						if (boxCollider2D)
						{
							Vector2 b = boxCollider2D.size / 2f;
							Vector2 vector4 = (Vector2)boxCollider2D.transform.TransformPoint(boxCollider2D.offset - b) - vector;
							Vector2 vector5 = (Vector2)boxCollider2D.transform.TransformPoint(boxCollider2D.offset + b) - vector;
							this.del.x = vector4.x;
							this.del.y = vector4.y;
							this.TestBothSides(vector);
							this.del.x = vector5.x;
							this.TestBothSides(vector);
							this.del.y = vector5.y;
							this.TestBothSides(vector);
							this.del.x = vector4.x;
							this.TestBothSides(vector);
						}
					}
				}
			}
		}
		float d = this.LightRadius * 1.05f;
		for (int l = 0; l < this.requiredDels.Length; l++)
		{
			Vector2 vector6 = d * this.requiredDels[l];
			this.CreateVert(vector, ref vector6);
		}
		this.verts.Sort(0, this.vertCount, LightSource.AngleComparer.Instance);
		this.myMesh.Clear();
		if (this.vec == null || this.vec.Length < this.vertCount + 1)
		{
			this.vec = new Vector3[this.vertCount + 1];
			this.uvs = new Vector2[this.vec.Length];
		}
		this.vec[0] = Vector3.zero;
		this.uvs[0] = new Vector2(this.vec[0].x, this.vec[0].y);
		for (int m = 0; m < this.vertCount; m++)
		{
			int num2 = m + 1;
			this.vec[num2] = this.verts[m].Position;
			this.uvs[num2] = new Vector2(this.vec[num2].x, this.vec[num2].y);
		}
		int num3 = this.vertCount * 3;
		if (num3 > this.triangles.Length)
		{
			this.triangles = new int[num3];
			Debug.LogWarning("Resized triangles to: " + num3);
		}
		int num4 = 0;
		for (int n = 0; n < this.triangles.Length; n += 3)
		{
			if (n < num3)
			{
				this.triangles[n] = 0;
				this.triangles[n + 1] = num4 + 1;
				if (n == num3 - 3)
				{
					this.triangles[n + 2] = 1;
				}
				else
				{
					this.triangles[n + 2] = num4 + 2;
				}
				num4++;
			}
			else
			{
				this.triangles[n] = 0;
				this.triangles[n + 1] = 0;
				this.triangles[n + 2] = 0;
			}
		}
		this.myMesh.vertices = this.vec;
		this.myMesh.uv = this.uvs;
		this.myMesh.SetIndices(this.triangles, MeshTopology.Triangles, 0);
	}

	// Token: 0x060009D6 RID: 2518 RVA: 0x00033CF0 File Offset: 0x00031EF0
	private void TestBothSides(Vector2 myPos)
	{
		float num = LightSource.length(this.del.x, this.del.x);
		this.tan.x = -this.del.y / num * this.tol;
		this.tan.y = this.del.x / num * this.tol;
		this.side.x = this.del.x + this.tan.x;
		this.side.y = this.del.y + this.tan.y;
		this.CreateVert(myPos, ref this.side);
		this.side.x = this.del.x - this.tan.x;
		this.side.y = this.del.y - this.tan.y;
		this.CreateVert(myPos, ref this.side);
	}

	// Token: 0x060009D7 RID: 2519 RVA: 0x00033DFC File Offset: 0x00031FFC
	private void CreateVert(Vector2 myPos, ref Vector2 del)
	{
		float num = this.LightRadius * 1.5f;
		int num2 = Physics2D.Raycast(myPos, del, this.filter, this.buffer, num);
		if (num2 > 0)
		{
			this.lightHits.Clear();
			RaycastHit2D raycastHit2D = default(RaycastHit2D);
			Collider2D collider2D = null;
			for (int i = 0; i < num2; i++)
			{
				RaycastHit2D raycastHit2D2 = this.buffer[i];
				Collider2D collider = raycastHit2D2.collider;
				OneWayShadows oneWayShadows;
				if (!LightSource.OneWayShadows.TryGetValue(collider.gameObject, out oneWayShadows) || !oneWayShadows.IsIgnored(this))
				{
					this.lightHits.Add(raycastHit2D2);
					if (!collider.isTrigger)
					{
						raycastHit2D = raycastHit2D2;
						collider2D = collider;
						break;
					}
				}
			}
			for (int j = 0; j < this.lightHits.Count; j++)
			{
				NoShadowBehaviour noShadowBehaviour;
				if (LightSource.NoShadows.TryGetValue(this.lightHits[j].collider.gameObject, out noShadowBehaviour))
				{
					noShadowBehaviour.didHit = true;
				}
			}
			if (collider2D && !collider2D.isTrigger)
			{
				Vector2 point = raycastHit2D.point;
				this.GetEmptyVert().Complete(point.x - myPos.x, point.y - myPos.y);
				return;
			}
		}
		Vector2 normalized = del.normalized;
		this.GetEmptyVert().Complete(normalized.x * num, normalized.y * num);
	}

	// Token: 0x060009D8 RID: 2520 RVA: 0x00033F64 File Offset: 0x00032164
	private LightSource.VertInfo GetEmptyVert()
	{
		if (this.vertCount < this.verts.Count)
		{
			List<LightSource.VertInfo> list = this.verts;
			int num = this.vertCount;
			this.vertCount = num + 1;
			return list[num];
		}
		LightSource.VertInfo vertInfo = new LightSource.VertInfo();
		this.verts.Add(vertInfo);
		this.vertCount = this.verts.Count;
		return vertInfo;
	}

	// Token: 0x060009D9 RID: 2521 RVA: 0x00007FD8 File Offset: 0x000061D8
	private static float length(float x, float y)
	{
		return Mathf.Sqrt(x * x + y * y);
	}

	// Token: 0x060009DA RID: 2522 RVA: 0x00033FC8 File Offset: 0x000321C8
	public static float pseudoAngle(float dx, float dy)
	{
		if (dx < 0f)
		{
			float num = -dx;
			float num2 = (dy > 0f) ? dy : (-dy);
			return 2f - dy / (num + num2);
		}
		float num3 = (dy > 0f) ? dy : (-dy);
		return dy / (dx + num3);
	}

	// Token: 0x04000970 RID: 2416
	public static Dictionary<GameObject, NoShadowBehaviour> NoShadows = new Dictionary<GameObject, NoShadowBehaviour>();

	// Token: 0x04000971 RID: 2417
	public static Dictionary<GameObject, OneWayShadows> OneWayShadows = new Dictionary<GameObject, OneWayShadows>();

	// Token: 0x04000972 RID: 2418
	[HideInInspector]
	private GameObject child;

	// Token: 0x04000973 RID: 2419
	[HideInInspector]
	private Vector2[] requiredDels;

	// Token: 0x04000974 RID: 2420
	[HideInInspector]
	private Mesh myMesh;

	// Token: 0x04000975 RID: 2421
	public int MinRays = 24;

	// Token: 0x04000976 RID: 2422
	public float LightRadius = 3f;

	// Token: 0x04000977 RID: 2423
	public Material Material;

	// Token: 0x04000978 RID: 2424
	[HideInInspector]
	private List<LightSource.VertInfo> verts = new List<LightSource.VertInfo>(256);

	// Token: 0x04000979 RID: 2425
	[HideInInspector]
	private int vertCount;

	// Token: 0x0400097A RID: 2426
	private RaycastHit2D[] buffer = new RaycastHit2D[25];

	// Token: 0x0400097B RID: 2427
	private Collider2D[] hits = new Collider2D[40];

	// Token: 0x0400097C RID: 2428
	private ContactFilter2D filter;

	// Token: 0x0400097D RID: 2429
	private Vector3[] vec;

	// Token: 0x0400097E RID: 2430
	private Vector2[] uvs;

	// Token: 0x0400097F RID: 2431
	private int[] triangles = new int[900];

	// Token: 0x04000980 RID: 2432
	public float tol = 0.05f;

	// Token: 0x04000981 RID: 2433
	private Vector2 del;

	// Token: 0x04000982 RID: 2434
	private Vector2 tan;

	// Token: 0x04000983 RID: 2435
	private Vector2 side;

	// Token: 0x04000984 RID: 2436
	private List<RaycastHit2D> lightHits = new List<RaycastHit2D>();

	// Token: 0x020001C5 RID: 453
	private class VertInfo
	{
		// Token: 0x060009DD RID: 2525 RVA: 0x00007FFC File Offset: 0x000061FC
		internal void Complete(float x, float y)
		{
			this.Position.x = x;
			this.Position.y = y;
			this.Angle = LightSource.pseudoAngle(y, x);
		}

		// Token: 0x060009DE RID: 2526 RVA: 0x00008023 File Offset: 0x00006223
		internal void Complete(Vector2 point)
		{
			this.Position.x = point.x;
			this.Position.y = point.y;
			this.Angle = LightSource.pseudoAngle(point.y, point.x);
		}

		// Token: 0x04000985 RID: 2437
		public float Angle;

		// Token: 0x04000986 RID: 2438
		public Vector3 Position;
	}

	// Token: 0x020001C6 RID: 454
	private class AngleComparer : IComparer<LightSource.VertInfo>
	{
		// Token: 0x060009E0 RID: 2528 RVA: 0x0000805E File Offset: 0x0000625E
		public int Compare(LightSource.VertInfo x, LightSource.VertInfo y)
		{
			if (x.Angle > y.Angle)
			{
				return 1;
			}
			if (x.Angle >= y.Angle)
			{
				return 0;
			}
			return -1;
		}

		// Token: 0x04000987 RID: 2439
		public static readonly LightSource.AngleComparer Instance = new LightSource.AngleComparer();
	}

	// Token: 0x020001C7 RID: 455
	private class HitDepthComparer : IComparer<RaycastHit2D>
	{
		// Token: 0x060009E3 RID: 2531 RVA: 0x0000808D File Offset: 0x0000628D
		public int Compare(RaycastHit2D x, RaycastHit2D y)
		{
			if (x.fraction <= y.fraction)
			{
				return -1;
			}
			return 1;
		}

		// Token: 0x04000988 RID: 2440
		public static readonly LightSource.HitDepthComparer Instance = new LightSource.HitDepthComparer();
	}
}
