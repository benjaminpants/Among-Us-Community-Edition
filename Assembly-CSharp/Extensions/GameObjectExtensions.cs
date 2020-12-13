using System;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtensions
{
	public static T Find<T>(this List<T> self, GameObject toFind) where T : MonoBehaviour
	{
		for (int i = 0; i < self.Count; i++)
		{
			T val = self[i];
			if (val.gameObject == toFind)
			{
				return val;
			}
		}
		return null;
	}

	public static void SetZ(this Transform self, float z)
	{
		Vector3 localPosition = self.localPosition;
		localPosition.z = z;
		self.localPosition = localPosition;
	}

	public static void LookAt2d(this Transform self, Vector3 target)
	{
		Vector3 vector = target - self.transform.position;
		vector.Normalize();
		float num = Mathf.Atan2(vector.y, vector.x);
		self.transform.rotation = Quaternion.Euler(0f, 0f, num * 57.29578f);
	}

	public static void LookAt2d(this Transform self, Transform target)
	{
		self.LookAt2d(target.transform.position);
	}

	public static void DestroyChildren(this Transform self)
	{
		for (int num = self.childCount - 1; num > -1; num--)
		{
			Transform child = self.GetChild(num);
			child.transform.SetParent(null);
			UnityEngine.Object.Destroy(child.gameObject);
		}
	}

	public static void DestroyChildren(this MonoBehaviour self)
	{
		for (int num = self.transform.childCount - 1; num > -1; num--)
		{
			UnityEngine.Object.Destroy(self.transform.GetChild(num).gameObject);
		}
	}

	public static void ForEachChild(this GameObject self, Action<GameObject> todo)
	{
		for (int num = self.transform.childCount - 1; num > -1; num--)
		{
			todo(self.transform.GetChild(num).gameObject);
		}
	}

	public static void ForEachChildBehavior<T>(this MonoBehaviour self, Action<T> todo) where T : MonoBehaviour
	{
		for (int num = self.transform.childCount - 1; num > -1; num--)
		{
			T component = self.transform.GetChild(num).GetComponent<T>();
			if ((bool)(UnityEngine.Object)component)
			{
				todo(component);
			}
		}
	}
}
