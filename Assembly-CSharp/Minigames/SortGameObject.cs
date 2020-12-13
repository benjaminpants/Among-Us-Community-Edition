using UnityEngine;

public class SortGameObject : MonoBehaviour
{
	public enum ObjType
	{
		Plant,
		Mineral,
		Animal
	}

	public ObjType MyType;

	public Collider2D Collider;
}
