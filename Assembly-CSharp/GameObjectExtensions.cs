using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200003C RID: 60
public static class GameObjectExtensions
{
	// Token: 0x0600015D RID: 349 RVA: 0x0000EF64 File Offset: 0x0000D164
	public static T Find<T>(this List<T> self, GameObject toFind) where T : MonoBehaviour
	{
		for (int i = 0; i < self.Count; i++)
		{
			T t = self[i];
			if (t.gameObject == toFind)
			{
				return t;
			}
		}
		return default(T);
	}

	// Token: 0x0600015E RID: 350 RVA: 0x0000EFA8 File Offset: 0x0000D1A8
	public static void SetZ(this Transform self, float z)
	{
		Vector3 localPosition = self.localPosition;
		localPosition.z = z;
		self.localPosition = localPosition;
	}

	// Token: 0x0600015F RID: 351 RVA: 0x0000EFCC File Offset: 0x0000D1CC
	public static void LookAt2d(this Transform self, Vector3 target)
	{
		Vector3 vector = target - self.transform.position;
		vector.Normalize();
		float num = Mathf.Atan2(vector.y, vector.x);
		self.transform.rotation = Quaternion.Euler(0f, 0f, num * 57.29578f);
	}

	// Token: 0x06000160 RID: 352 RVA: 0x00002D37 File Offset: 0x00000F37
	public static void LookAt2d(this Transform self, Transform target)
	{
		self.LookAt2d(target.transform.position);
	}

	// Token: 0x06000161 RID: 353 RVA: 0x0000F028 File Offset: 0x0000D228
	public static void DestroyChildren(this Transform self)
	{
		for (int i = self.childCount - 1; i > -1; i--)
		{
			Transform child = self.GetChild(i);
			child.transform.SetParent(null);
			UnityEngine.Object.Destroy(child.gameObject);
		}
	}

	// Token: 0x06000162 RID: 354 RVA: 0x0000F068 File Offset: 0x0000D268
	public static void DestroyChildren(this MonoBehaviour self)
	{
		for (int i = self.transform.childCount - 1; i > -1; i--)
		{
			UnityEngine.Object.Destroy(self.transform.GetChild(i).gameObject);
		}
	}

	// Token: 0x06000163 RID: 355 RVA: 0x0000F0A4 File Offset: 0x0000D2A4
	public static void ForEachChild(this GameObject self, Action<GameObject> todo)
	{
		for (int i = self.transform.childCount - 1; i > -1; i--)
		{
			todo(self.transform.GetChild(i).gameObject);
		}
	}

	// Token: 0x06000164 RID: 356 RVA: 0x0000F0E0 File Offset: 0x0000D2E0
	public static void ForEachChildBehavior<T>(this MonoBehaviour self, Action<T> todo) where T : MonoBehaviour
	{
		for (int i = self.transform.childCount - 1; i > -1; i--)
		{
			T component = self.transform.GetChild(i).GetComponent<T>();
			if (component)
			{
				todo(component);
			}
		}
	}
}
