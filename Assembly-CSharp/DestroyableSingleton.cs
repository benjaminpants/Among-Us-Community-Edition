using System;
using UnityEngine;

// Token: 0x02000021 RID: 33
public class DestroyableSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
	// Token: 0x1700001F RID: 31
	// (get) Token: 0x06000093 RID: 147 RVA: 0x00002764 File Offset: 0x00000964
	public static bool InstanceExists
	{
		get
		{
			return DestroyableSingleton<T>._instance;
		}
	}

	// Token: 0x06000094 RID: 148 RVA: 0x0000D14C File Offset: 0x0000B34C
	public virtual void Awake()
	{
		if (!DestroyableSingleton<T>._instance)
		{
			DestroyableSingleton<T>._instance = (this as T);
			if (this.DontDestroy)
			{
				UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
				return;
			}
		}
		else if (DestroyableSingleton<T>._instance != this)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x17000020 RID: 32
	// (get) Token: 0x06000095 RID: 149 RVA: 0x0000D1AC File Offset: 0x0000B3AC
	public static T Instance
	{
		get
		{
			if (!DestroyableSingleton<T>._instance)
			{
				DestroyableSingleton<T>._instance = UnityEngine.Object.FindObjectOfType<T>();
				if (!DestroyableSingleton<T>._instance)
				{
					DestroyableSingleton<T>._instance = new GameObject().AddComponent<T>();
				}
			}
			return DestroyableSingleton<T>._instance;
		}
	}

	// Token: 0x06000096 RID: 150 RVA: 0x00002775 File Offset: 0x00000975
	public virtual void OnDestroy()
	{
		if (!this.DontDestroy)
		{
			DestroyableSingleton<T>._instance = default(T);
		}
	}

	// Token: 0x040000C6 RID: 198
	private static T _instance;

	// Token: 0x040000C7 RID: 199
	public bool DontDestroy;
}
