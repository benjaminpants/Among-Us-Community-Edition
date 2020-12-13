using UnityEngine;

public class ParallaxController : MonoBehaviour
{
	public float Rate;

	private Camera cam;

	public void Start()
	{
		cam = Camera.main;
	}

	private void Update()
	{
		Vector3 localPosition = base.transform.parent.position - cam.transform.position;
		localPosition *= Rate;
		localPosition.z = 0f - Rate;
		base.transform.localPosition = localPosition;
	}
}
