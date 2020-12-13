using UnityEngine;

public class CloudGenerator : MonoBehaviour
{
	private struct Cloud
	{
		public int CloudIdx;

		public float Rate;

		public float PositionX;

		public float PositionY;
	}

	public Sprite[] CloudImages;

	private Vector2[][] UvCache;

	private Vector2[] ExtentCache;

	public int NumClouds = 500;

	public float Length = 25f;

	public float Width = 25f;

	public Vector2 Direction = new Vector2(1f, 0f);

	private Vector2 NormDir = new Vector2(1f, 0f);

	private Vector2 Tangent = new Vector2(0f, 1f);

	private float tanLen;

	public FloatRange Rates = new FloatRange(0.25f, 1f);

	[HideInInspector]
	private Cloud[] stars;

	[HideInInspector]
	private Vector3[] verts;

	[HideInInspector]
	private Mesh mesh;

	public void Start()
	{
		UvCache = new Vector2[CloudImages.Length][];
		ExtentCache = new Vector2[CloudImages.Length];
		for (int i = 0; i < CloudImages.Length; i++)
		{
			Sprite sprite = CloudImages[i];
			UvCache[i] = sprite.uv;
			ExtentCache[i] = sprite.bounds.extents;
		}
		stars = new Cloud[NumClouds];
		verts = new Vector3[NumClouds * 4];
		Vector2[] array = new Vector2[NumClouds * 4];
		int[] array2 = new int[NumClouds * 6];
		SetDirection(Direction);
		MeshFilter component = GetComponent<MeshFilter>();
		mesh = new Mesh();
		mesh.MarkDynamic();
		component.mesh = mesh;
		Vector3 vector = default(Vector3);
		for (int j = 0; j < stars.Length; j++)
		{
			Cloud cloud = stars[j];
			int num = (cloud.CloudIdx = CloudImages.RandomIdx());
			Vector2 vector2 = ExtentCache[num];
			Vector2[] array3 = UvCache[num];
			float num2 = FloatRange.Next(-1f, 1f) * Length;
			float num3 = FloatRange.Next(-1f, 1f) * Width;
			float num4 = (cloud.PositionX = num2 * NormDir.x + num3 * Tangent.x);
			float num5 = (cloud.PositionY = num2 * NormDir.y + num3 * Tangent.y);
			cloud.Rate = Rates.Next();
			stars[j] = cloud;
			int num6 = j * 4;
			vector.x = num4 - vector2.x;
			vector.y = num5 + vector2.y;
			verts[num6] = vector;
			vector.x = num4 + vector2.x;
			verts[num6 + 1] = vector;
			vector.x = num4 - vector2.x;
			vector.y = num5 - vector2.y;
			verts[num6 + 2] = vector;
			vector.x = num4 + vector2.x;
			verts[num6 + 3] = vector;
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
		mesh.vertices = verts;
		mesh.uv = array;
		mesh.SetIndices(array2, MeshTopology.Triangles, 0);
	}

	private void FixedUpdate()
	{
		float num = -0.99f * Length;
		Vector2 vector = Direction * Time.fixedDeltaTime;
		Vector3 vector2 = default(Vector3);
		for (int i = 0; i < stars.Length; i++)
		{
			int num2 = i * 4;
			Cloud cloud = stars[i];
			float positionX = cloud.PositionX;
			float positionY = cloud.PositionY;
			Vector2 vector3 = ExtentCache[cloud.CloudIdx];
			float rate = cloud.Rate;
			positionX += rate * vector.x;
			positionY += rate * vector.y;
			if (OrthoDistance(positionX, positionY) > Length)
			{
				float num3 = FloatRange.Next(-1f, 1f) * Width;
				positionX = num * NormDir.x + num3 * Tangent.x;
				positionY = num * NormDir.y + num3 * Tangent.y;
				cloud.Rate = Rates.Next();
			}
			cloud.PositionX = positionX;
			cloud.PositionY = positionY;
			stars[i] = cloud;
			vector2.x = positionX - vector3.x;
			vector2.y = positionY + vector3.y;
			verts[num2] = vector2;
			vector2.x = positionX + vector3.x;
			verts[num2 + 1] = vector2;
			vector2.x = positionX - vector3.x;
			vector2.y = positionY - vector3.y;
			verts[num2 + 2] = vector2;
			vector2.x = positionX + vector3.x;
			verts[num2 + 3] = vector2;
		}
		mesh.vertices = verts;
	}

	public void SetDirection(Vector2 dir)
	{
		Direction = dir;
		NormDir = Direction.normalized;
		Tangent = new Vector2(0f - NormDir.y, NormDir.x);
		tanLen = Mathf.Sqrt(Tangent.y * Tangent.y + Tangent.x * Tangent.x);
	}

	private float OrthoDistance(float pointx, float pointy)
	{
		return (Tangent.y * pointx - Tangent.x * pointy) / tanLen;
	}
}
