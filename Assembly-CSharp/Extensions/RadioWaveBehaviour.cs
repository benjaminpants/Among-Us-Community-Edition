using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class RadioWaveBehaviour : MonoBehaviour
{
	public int NumPoints = 128;

	public FloatRange Width;

	public FloatRange Height;

	private Mesh mesh;

	private Vector3[] vecs;

	public float TickRate = 0.1f;

	private float timer;

	public int Skip = 2;

	public float Frequency = 5f;

	private int Tick;

	public bool Random;

	[Range(0f, 1f)]
	public float NoiseLevel;

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
			vector.y = Height.Next();
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
		Tick++;
		for (int i = 0; i < vecs.Length - Skip; i++)
		{
			vecs[i].y = vecs[i + Skip].y;
		}
		if (Random)
		{
			for (int j = 1; j <= Skip; j++)
			{
				vecs[vecs.Length - j].y = Height.Next();
			}
		}
		else
		{
			float num = 1f - NoiseLevel;
			for (int k = 0; k < Skip; k++)
			{
				vecs[vecs.Length - 1 - Skip + k].y = Mathf.Sin(((float)Tick + (float)k / (float)Skip) * Frequency) * Height.max * num + Height.Next() * NoiseLevel;
			}
		}
		mesh.vertices = vecs;
	}
}
