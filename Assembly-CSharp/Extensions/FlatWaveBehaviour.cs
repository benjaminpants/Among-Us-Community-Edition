using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class FlatWaveBehaviour : MonoBehaviour
{
	public int NumPoints = 128;

	public FloatRange Width;

	public FloatRange Delta;

	public float Center;

	private Mesh mesh;

	private Vector3[] vecs;

	public float TickRate = 0.1f;

	private float timer;

	public int Skip = 3;

	[Range(0f, 1f)]
	public float NoiseP = 0.5f;

	public void Start()
	{
		mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = mesh;
		mesh.MarkDynamic();
		vecs = new Vector3[NumPoints];
		int[] array = new int[NumPoints];
		for (int i = 0; i < vecs.Length; i++)
		{
			Vector3 vector = vecs[i];
			vector.x = Width.Lerp((float)i / (float)vecs.Length);
			vector.y = Center;
			if (BoolRange.Next(NoiseP))
			{
				vector.y += Delta.Next();
			}
			vecs[i] = vector;
			array[i] = i;
		}
		mesh.vertices = vecs;
		mesh.SetIndices(array, MeshTopology.LineStrip, 0);
	}

	public void Update()
	{
		timer += Time.deltaTime;
		if (!(timer > TickRate))
		{
			return;
		}
		timer = 0f;
		for (int i = 0; i < vecs.Length - Skip; i++)
		{
			vecs[i].y = vecs[i + Skip].y;
		}
		for (int j = 1; j <= Skip; j++)
		{
			vecs[vecs.Length - j].y = Center;
			if (BoolRange.Next(NoiseP))
			{
				vecs[vecs.Length - j].y += Delta.Next();
			}
		}
		mesh.vertices = vecs;
	}
}
