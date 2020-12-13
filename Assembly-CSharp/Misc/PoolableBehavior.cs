using UnityEngine;

public class PoolableBehavior : MonoBehaviour
{
	[HideInInspector]
	public IObjectPool OwnerPool;

	public virtual void Reset()
	{
	}

	public void Awake()
	{
		OwnerPool = DefaultPool.Instance;
	}
}
