using System;
using UnityEngine;

// Token: 0x02000020 RID: 32
public class DefaultPool : IObjectPool
{
	// Token: 0x1700001B RID: 27
	// (get) Token: 0x0600008A RID: 138 RVA: 0x00002723 File Offset: 0x00000923
	public override int InUse
	{
		get
		{
			return 0;
		}
	}

	// Token: 0x1700001C RID: 28
	// (get) Token: 0x0600008B RID: 139 RVA: 0x00002723 File Offset: 0x00000923
	public override int NotInUse
	{
		get
		{
			return 0;
		}
	}

	// Token: 0x1700001D RID: 29
	// (get) Token: 0x0600008C RID: 140 RVA: 0x00002726 File Offset: 0x00000926
	public static bool InstanceExists
	{
		get
		{
			return DefaultPool._instance;
		}
	}

	// Token: 0x1700001E RID: 30
	// (get) Token: 0x0600008D RID: 141 RVA: 0x0000D070 File Offset: 0x0000B270
	public static DefaultPool Instance
	{
		get
		{
			object @lock = DefaultPool._lock;
			DefaultPool instance;
			lock (@lock)
			{
				if (DefaultPool._instance == null)
				{
					DefaultPool._instance = UnityEngine.Object.FindObjectOfType<DefaultPool>();
					if (UnityEngine.Object.FindObjectsOfType<DefaultPool>().Length > 1)
					{
						Debug.LogError("[Singleton] Something went really wrong  - there should never be more than 1 singleton! Reopening the scene might fix it.");
						return DefaultPool._instance;
					}
					if (DefaultPool._instance == null)
					{
						GameObject gameObject = new GameObject();
						DefaultPool._instance = gameObject.AddComponent<DefaultPool>();
						gameObject.name = "(singleton) DefaultPool";
					}
				}
				instance = DefaultPool._instance;
			}
			return instance;
		}
	}

	// Token: 0x0600008E RID: 142 RVA: 0x0000D10C File Offset: 0x0000B30C
	public void OnDestroy()
	{
		object @lock = DefaultPool._lock;
		lock (@lock)
		{
			DefaultPool._instance = null;
		}
	}

	// Token: 0x0600008F RID: 143 RVA: 0x00002732 File Offset: 0x00000932
	public override T Get<T>()
	{
		throw new NotImplementedException();
	}

	// Token: 0x06000090 RID: 144 RVA: 0x00002739 File Offset: 0x00000939
	public override void Reclaim(PoolableBehavior obj)
	{
		Debug.Log("Default Pool: Destroying this thing.");
		UnityEngine.Object.Destroy(obj.gameObject);
	}

	// Token: 0x040000C4 RID: 196
	private static DefaultPool _instance;

	// Token: 0x040000C5 RID: 197
	private static object _lock = new object();
}
