using UnityEngine;

public class DestroyableSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T _instance;

	public bool DontDestroy;

	public static bool InstanceExists => (Object)_instance;

	public static T Instance
	{
		get
		{
			if (!(Object)_instance)
			{
				_instance = Object.FindObjectOfType<T>();
				if (!(Object)_instance)
				{
					_instance = new GameObject().AddComponent<T>();
				}
			}
			return _instance;
		}
	}

	public virtual void Awake()
	{
		if (!(Object)_instance)
		{
			_instance = this as T;
			if (DontDestroy)
			{
				Object.DontDestroyOnLoad(base.gameObject);
			}
		}
		else if (_instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	public virtual void OnDestroy()
	{
		if (!DontDestroy)
		{
			_instance = null;
		}
	}
}
