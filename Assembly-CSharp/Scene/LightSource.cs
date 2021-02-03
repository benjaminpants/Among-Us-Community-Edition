using System.Collections.Generic;
using UnityEngine;

public class LightSource : MonoBehaviour
{
	private class VertInfo
	{
		public float Angle;

		public Vector3 Position;

		internal void Complete(float x, float y)
		{
			Position.x = x;
			Position.y = y;
			Angle = pseudoAngle(y, x);
		}

		internal void Complete(Vector2 point)
		{
			Position.x = point.x;
			Position.y = point.y;
			Angle = pseudoAngle(point.y, point.x);
		}
	}

	private class AngleComparer : IComparer<VertInfo>
	{
		public static readonly AngleComparer Instance = new AngleComparer();

		public int Compare(VertInfo x, VertInfo y)
		{
			if (!(x.Angle > y.Angle))
			{
				if (!(x.Angle < y.Angle))
				{
					return 0;
				}
				return -1;
			}
			return 1;
		}
	}

	private class HitDepthComparer : IComparer<RaycastHit2D>
	{
		public static readonly HitDepthComparer Instance = new HitDepthComparer();

		public int Compare(RaycastHit2D x, RaycastHit2D y)
		{
			if (!(x.fraction > y.fraction))
			{
				return -1;
			}
			return 1;
		}
	}

	public static Dictionary<GameObject, NoShadowBehaviour> NoShadows = new Dictionary<GameObject, NoShadowBehaviour>();

	public static Dictionary<GameObject, OneWayShadows> OneWayShadows = new Dictionary<GameObject, OneWayShadows>();

	[HideInInspector]
	private GameObject child;

	[HideInInspector]
	private Vector2[] requiredDels;

	[HideInInspector]
	private Mesh myMesh;

	public int MinRays = 48;

	public float LightRadius = 3f;

	public Material Material;

	[HideInInspector]
	private List<VertInfo> verts = new List<VertInfo>(256);

	[HideInInspector]
	private int vertCount;

	private RaycastHit2D[] buffer = new RaycastHit2D[25];

	private Collider2D[] hits = new Collider2D[40];

	private ContactFilter2D filter;

	private Vector3[] vec;

	private Vector2[] uvs;

	private int[] triangles = new int[900];

	public float tol = 0.05f;

	private Vector2 del;

	private Vector2 tan;

	private Vector2 side;

	private List<RaycastHit2D> lightHits = new List<RaycastHit2D>();

	private void Start()
	{
		MinRays = 48;
		filter.useTriggers = true;
		filter.layerMask = Constants.ShadowMask;
		filter.useLayerMask = true;
		requiredDels = new Vector2[MinRays];
		for (int i = 0; i < requiredDels.Length; i++)
		{
			requiredDels[i] = Vector2.left.Rotate((float)i / (float)requiredDels.Length * 360f);
		}
		myMesh = new Mesh();
		myMesh.MarkDynamic();
		myMesh.name = "ShadowMesh";
		GameObject gameObject = new GameObject("LightChild");
		gameObject.layer = 10;
		gameObject.AddComponent<MeshFilter>().mesh = myMesh;
		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
		Material = new Material(Material);
		meshRenderer.sharedMaterial = Material;
		child = gameObject;
	}

	private void Update()
	{
		vertCount = 0;
		Vector3 position = base.transform.position;
		position.z -= 7f;
		child.transform.position = position;
		Vector2 vector = position;
		Material.SetFloat("_LightRadius", LightRadius);
		int num = Physics2D.OverlapCircleNonAlloc(vector, LightRadius, hits, Constants.ShadowMask);
		for (int i = 0; i < num; i++)
		{
			Collider2D collider2D = hits[i];
			if (collider2D.isTrigger)
			{
				continue;
			}
			EdgeCollider2D edgeCollider2D = collider2D as EdgeCollider2D;
			if ((bool)edgeCollider2D)
			{
				Vector2[] points = edgeCollider2D.points;
				for (int j = 0; j < points.Length; j++)
				{
					Vector2 vector2 = edgeCollider2D.transform.TransformPoint(points[j]);
					del.x = vector2.x - vector.x;
					del.y = vector2.y - vector.y;
					TestBothSides(vector);
				}
				continue;
			}
			PolygonCollider2D polygonCollider2D = collider2D as PolygonCollider2D;
			if ((bool)polygonCollider2D)
			{
				Vector2[] points2 = polygonCollider2D.points;
				for (int k = 0; k < points2.Length; k++)
				{
					Vector2 vector3 = polygonCollider2D.transform.TransformPoint(points2[k]);
					del.x = vector3.x - vector.x;
					del.y = vector3.y - vector.y;
					TestBothSides(vector);
				}
				continue;
			}
			BoxCollider2D boxCollider2D = collider2D as BoxCollider2D;
			if ((bool)boxCollider2D)
			{
				Vector2 b = boxCollider2D.size / 2f;
				Vector2 vector4 = (Vector2)boxCollider2D.transform.TransformPoint(boxCollider2D.offset - b) - vector;
				Vector2 vector5 = (Vector2)boxCollider2D.transform.TransformPoint(boxCollider2D.offset + b) - vector;
				del.x = vector4.x;
				del.y = vector4.y;
				TestBothSides(vector);
				del.x = vector5.x;
				TestBothSides(vector);
				del.y = vector5.y;
				TestBothSides(vector);
				del.x = vector4.x;
				TestBothSides(vector);
			}
		}
		float d = LightRadius * 1.05f;
		for (int l = 0; l < requiredDels.Length; l++)
		{
			Vector2 vector6 = d * requiredDels[l];
			CreateVert(vector, ref vector6);
		}
		verts.Sort(0, vertCount, AngleComparer.Instance);
		myMesh.Clear();
		if (vec == null || vec.Length < vertCount + 1)
		{
			vec = new Vector3[vertCount + 1];
			uvs = new Vector2[vec.Length];
		}
		vec[0] = Vector3.zero;
		uvs[0] = new Vector2(vec[0].x, vec[0].y);
		for (int m = 0; m < vertCount; m++)
		{
			int num2 = m + 1;
			vec[num2] = verts[m].Position;
			uvs[num2] = new Vector2(vec[num2].x, vec[num2].y);
		}
		int num3 = vertCount * 3;
		if (num3 > triangles.Length)
		{
			triangles = new int[num3];
			Debug.LogWarning("Resized triangles to: " + num3);
		}
		int num4 = 0;
		for (int n = 0; n < triangles.Length; n += 3)
		{
			if (n < num3)
			{
				triangles[n] = 0;
				triangles[n + 1] = num4 + 1;
				if (n == num3 - 3)
				{
					triangles[n + 2] = 1;
				}
				else
				{
					triangles[n + 2] = num4 + 2;
				}
				num4++;
			}
			else
			{
				triangles[n] = 0;
				triangles[n + 1] = 0;
				triangles[n + 2] = 0;
			}
		}
		myMesh.vertices = vec;
		myMesh.uv = uvs;
		myMesh.SetIndices(triangles, MeshTopology.Triangles, 0);
	}

	private void TestBothSides(Vector2 myPos)
	{
		float num = length(del.x, del.x);
		tan.x = (0f - del.y) / num * tol;
		tan.y = del.x / num * tol;
		side.x = del.x + tan.x;
		side.y = del.y + tan.y;
		CreateVert(myPos, ref side);
		side.x = del.x - tan.x;
		side.y = del.y - tan.y;
		CreateVert(myPos, ref side);
	}

	private void CreateVert(Vector2 myPos, ref Vector2 del)
	{
		float num = LightRadius * 1.5f;
		int num2 = Physics2D.Raycast(myPos, del, filter, buffer, num);
		if (num2 > 0)
		{
			lightHits.Clear();
			RaycastHit2D raycastHit2D = default(RaycastHit2D);
			Collider2D collider2D = null;
			for (int i = 0; i < num2; i++)
			{
				RaycastHit2D raycastHit2D2 = buffer[i];
				Collider2D collider = raycastHit2D2.collider;
				if (!OneWayShadows.TryGetValue(collider.gameObject, out var value) || !value.IsIgnored(this))
				{
					lightHits.Add(raycastHit2D2);
					if (!collider.isTrigger)
					{
						raycastHit2D = raycastHit2D2;
						collider2D = collider;
						break;
					}
				}
			}
			for (int j = 0; j < lightHits.Count; j++)
			{
				if (NoShadows.TryGetValue(lightHits[j].collider.gameObject, out var value2))
				{
					value2.didHit = true;
				}
			}
			if ((bool)collider2D && !collider2D.isTrigger)
			{
				Vector2 point = raycastHit2D.point;
				GetEmptyVert().Complete(point.x - myPos.x, point.y - myPos.y);
				return;
			}
		}
		Vector2 normalized = del.normalized;
		GetEmptyVert().Complete(normalized.x * num, normalized.y * num);
	}

	private VertInfo GetEmptyVert()
	{
		if (vertCount < verts.Count)
		{
			return verts[vertCount++];
		}
		VertInfo vertInfo = new VertInfo();
		verts.Add(vertInfo);
		vertCount = verts.Count;
		return vertInfo;
	}

	private static float length(float x, float y)
	{
		return Mathf.Sqrt(x * x + y * y);
	}

	public static float pseudoAngle(float dx, float dy)
	{
		if (dx < 0f)
		{
			float num = 0f - dx;
			float num2 = ((dy > 0f) ? dy : (0f - dy));
			return 2f - dy / (num + num2);
		}
		float num3 = ((dy > 0f) ? dy : (0f - dy));
		return dy / (dx + num3);
	}
}
