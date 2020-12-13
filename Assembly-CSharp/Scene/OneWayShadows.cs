using UnityEngine;

public class OneWayShadows : MonoBehaviour
{
	public Collider2D RoomCollider;

	public void Start()
	{
		LightSource.OneWayShadows.Add(base.gameObject, this);
	}

	public void OnDestroy()
	{
		LightSource.OneWayShadows.Remove(base.gameObject);
	}

	public bool IsIgnored(LightSource lightSource)
	{
		return RoomCollider.OverlapPoint(lightSource.transform.position);
	}
}
