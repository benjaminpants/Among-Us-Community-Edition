using UnityEngine;

public class GarbageBehaviour : MonoBehaviour
{
	public void FixedUpdate()
	{
		if (base.transform.localPosition.y < -3.49f)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
