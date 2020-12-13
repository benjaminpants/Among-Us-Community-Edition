using System;
using UnityEngine;

public class DefaultPool : IObjectPool
{
	private static DefaultPool _instance;

	private static object _lock = new object();

	public override int InUse => 0;

	public override int NotInUse => 0;

	public static bool InstanceExists => _instance;

	public static DefaultPool Instance
	{
		get
		{
			lock (_lock)
			{
				if (_instance == null)
				{
					_instance = UnityEngine.Object.FindObjectOfType<DefaultPool>();
					if (UnityEngine.Object.FindObjectsOfType<DefaultPool>().Length > 1)
					{
						Debug.LogError("[Singleton] Something went really wrong  - there should never be more than 1 singleton! Reopening the scene might fix it.");
						return _instance;
					}
					if (_instance == null)
					{
						GameObject gameObject = new GameObject();
						_instance = gameObject.AddComponent<DefaultPool>();
						gameObject.name = "(singleton) DefaultPool";
					}
				}
				return _instance;
			}
		}
	}

	public void OnDestroy()
	{
		lock (_lock)
		{
			_instance = null;
		}
	}

	public override T Get<T>()
	{
		throw new NotImplementedException();
	}

	public override void Reclaim(PoolableBehavior obj)
	{
		Debug.Log("Default Pool: Destroying this thing.");
		UnityEngine.Object.Destroy(obj.gameObject);
	}
}
