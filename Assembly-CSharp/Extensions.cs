using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

// Token: 0x0200003A RID: 58
public static class Extensions
{
	// Token: 0x06000130 RID: 304 RVA: 0x0000E8B8 File Offset: 0x0000CAB8
	public static void TrimEnd(this StringBuilder self)
	{
		for (int i = self.Length - 1; i >= 0; i--)
		{
			char c = self[i];
			if (c != ' ' && c != '\t' && c != '\n' && c != '\r')
			{
				break;
			}
			int length = self.Length;
			self.Length = length - 1;
		}
	}

	// Token: 0x06000131 RID: 305 RVA: 0x00002B21 File Offset: 0x00000D21
	public static void AddUnique<T>(this IList<T> self, T item)
	{
		if (!self.Contains(item))
		{
			self.Add(item);
		}
	}

	// Token: 0x06000132 RID: 306 RVA: 0x0000E904 File Offset: 0x0000CB04
	public static string ToTextColor(this Color c)
	{
		return string.Concat(new string[]
		{
			"[",
			Extensions.ByteHex[(int)((byte)(c.r * 255f))],
			Extensions.ByteHex[(int)((byte)(c.g * 255f))],
			Extensions.ByteHex[(int)((byte)(c.b * 255f))],
			Extensions.ByteHex[(int)((byte)(c.a * 255f))],
			"]"
		});
	}

	// Token: 0x06000133 RID: 307 RVA: 0x0000E984 File Offset: 0x0000CB84
	public static int ToInteger(this Color c, bool alpha)
	{
		if (alpha)
		{
			return (int)((byte)(c.r * 256f)) << 24 | (int)((byte)(c.g * 256f)) << 16 | (int)((byte)(c.b * 256f)) << 8 | (int)((byte)(c.a * 256f));
		}
		return (int)((byte)(c.r * 256f)) << 16 | (int)((byte)(c.g * 256f)) << 8 | (int)((byte)(c.b * 256f));
	}

	// Token: 0x06000134 RID: 308 RVA: 0x00002B33 File Offset: 0x00000D33
	public static bool HasAnyBit(this int self, int bit)
	{
		return (self & bit) != 0;
	}

	// Token: 0x06000135 RID: 309 RVA: 0x00002B33 File Offset: 0x00000D33
	public static bool HasAnyBit(this byte self, byte bit)
	{
		return (self & bit) > 0;
	}

	// Token: 0x06000136 RID: 310 RVA: 0x00002B3B File Offset: 0x00000D3B
	public static bool HasBit(this byte self, byte bit)
	{
		return (self & bit) == bit;
	}

	// Token: 0x06000137 RID: 311 RVA: 0x0000EA04 File Offset: 0x0000CC04
	public static int BitCount(this byte self)
	{
		int num = 0;
		for (int i = 0; i < 8; i++)
		{
			if ((1 << i & (int)self) != 0)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x06000138 RID: 312 RVA: 0x0000EA30 File Offset: 0x0000CC30
	public static int IndexOf<T>(this T[] self, T item) where T : class
	{
		for (int i = 0; i < self.Length; i++)
		{
			if (self[i] == item)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06000139 RID: 313 RVA: 0x0000EA64 File Offset: 0x0000CC64
	public static int IndexOfMin<T>(this T[] self, Func<T, float> comparer)
	{
		float num = float.MaxValue;
		int result = -1;
		for (int i = 0; i < self.Length; i++)
		{
			float num2 = comparer(self[i]);
			if (num2 <= num)
			{
				result = i;
				num = num2;
			}
		}
		return result;
	}

	// Token: 0x0600013A RID: 314 RVA: 0x0000EAA0 File Offset: 0x0000CCA0
	public static int IndexOfMax<T>(this T[] self, Func<T, int> comparer, out bool tie)
	{
		tie = false;
		int num = int.MinValue;
		int result = -1;
		for (int i = 0; i < self.Length; i++)
		{
			int num2 = comparer(self[i]);
			if (num2 > num)
			{
				result = i;
				num = num2;
				tie = false;
			}
			else if (num2 == num)
			{
				tie = true;
				result = -1;
			}
		}
		return result;
	}

	// Token: 0x0600013B RID: 315 RVA: 0x0000EAEC File Offset: 0x0000CCEC
	public static void SetAll<T>(this IList<T> self, T value)
	{
		for (int i = 0; i < self.Count; i++)
		{
			self[i] = value;
		}
	}

	// Token: 0x0600013C RID: 316 RVA: 0x0000EB14 File Offset: 0x0000CD14
	public static void AddAll<T>(this List<T> self, IList<T> other)
	{
		self.Capacity = Math.Max(self.Capacity, self.Count + other.Count);
		for (int i = 0; i < other.Count; i++)
		{
			self.Add(other[i]);
		}
	}

	// Token: 0x0600013D RID: 317 RVA: 0x0000EB60 File Offset: 0x0000CD60
	public static void Shuffle<T>(this IList<T> self)
	{
		for (int i = 0; i < self.Count - 1; i++)
		{
			T value = self[i];
			int index = UnityEngine.Random.Range(i, self.Count);
			self[i] = self[index];
			self[index] = value;
		}
	}

	// Token: 0x0600013E RID: 318 RVA: 0x0000EBAC File Offset: 0x0000CDAC
	public static void Shuffle<T>(this System.Random r, IList<T> self)
	{
		for (int i = 0; i < self.Count; i++)
		{
			T value = self[i];
			int index = r.Next(self.Count);
			self[i] = self[index];
			self[index] = value;
		}
	}

	// Token: 0x0600013F RID: 319 RVA: 0x0000EBF8 File Offset: 0x0000CDF8
	public static T[] RandomSet<T>(this IList<T> self, int length)
	{
		T[] array = new T[length];
		self.RandomFill(array);
		return array;
	}

	// Token: 0x06000140 RID: 320 RVA: 0x0000EC14 File Offset: 0x0000CE14
	public static void RandomFill<T>(this IList<T> self, T[] target)
	{
		HashSet<int> hashSet = new HashSet<int>();
		for (int i = 0; i < target.Length; i++)
		{
			int num;
			do
			{
				num = self.RandomIdx<T>();
			}
			while (hashSet.Contains(num));
			target[i] = self[num];
			hashSet.Add(num);
			if (hashSet.Count == self.Count)
			{
				return;
			}
		}
	}

	// Token: 0x06000141 RID: 321 RVA: 0x00002B43 File Offset: 0x00000D43
	public static int RandomIdx<T>(this IList<T> self)
	{
		return UnityEngine.Random.Range(0, self.Count);
	}

	// Token: 0x06000142 RID: 322 RVA: 0x00002B51 File Offset: 0x00000D51
	public static int RandomIdx<T>(this IEnumerable<T> self)
	{
		return UnityEngine.Random.Range(0, self.Count<T>());
	}

	// Token: 0x06000143 RID: 323 RVA: 0x00002B5F File Offset: 0x00000D5F
	public static T Random<T>(this IEnumerable<T> self)
	{
		return self.ToArray<T>().Random<T>();
	}

	// Token: 0x06000144 RID: 324 RVA: 0x0000EC6C File Offset: 0x0000CE6C
	public static T Random<T>(this IList<T> self)
	{
		if (self.Count > 0)
		{
			return self[UnityEngine.Random.Range(0, self.Count)];
		}
		return default(T);
	}

	// Token: 0x06000145 RID: 325 RVA: 0x00002B6C File Offset: 0x00000D6C
	public static Vector2 Div(this Vector2 a, Vector2 b)
	{
		return new Vector2(a.x / b.x, a.y / b.y);
	}

	// Token: 0x06000146 RID: 326 RVA: 0x00002B8D File Offset: 0x00000D8D
	public static Vector2 Mul(this Vector2 a, Vector2 b)
	{
		return new Vector2(a.x * b.x, a.y * b.y);
	}

	// Token: 0x06000147 RID: 327 RVA: 0x00002BAE File Offset: 0x00000DAE
	public static Vector3 Mul(this Vector3 a, Vector3 b)
	{
		return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
	}

	// Token: 0x06000148 RID: 328 RVA: 0x00002BDC File Offset: 0x00000DDC
	public static Vector3 Inv(this Vector3 a)
	{
		return new Vector3(1f / a.x, 1f / a.y, 1f / a.z);
	}

	// Token: 0x06000149 RID: 329 RVA: 0x0000ECA0 File Offset: 0x0000CEA0
	public static Rect Lerp(this Rect source, Rect target, float t)
	{
		return new Rect
		{
			position = Vector2.Lerp(source.position, target.position, t),
			size = Vector2.Lerp(source.size, target.size, t)
		};
	}

	// Token: 0x0600014A RID: 330 RVA: 0x0000ECEC File Offset: 0x0000CEEC
	public static void ForEach<T>(this IList<T> self, Action<T> todo)
	{
		for (int i = 0; i < self.Count; i++)
		{
			todo(self[i]);
		}
	}

	// Token: 0x0600014B RID: 331 RVA: 0x0000ED18 File Offset: 0x0000CF18
	public static T Max<T>(this IList<T> self, Func<T, float> comparer)
	{
		T t = self.First<T>();
		float num = comparer(t);
		for (int i = 0; i < self.Count; i++)
		{
			T t2 = self[i];
			float num2 = comparer(t2);
			if (num < num2 || (num == num2 && UnityEngine.Random.value > 0.5f))
			{
				num = num2;
				t = t2;
			}
		}
		return t;
	}

	// Token: 0x0600014C RID: 332 RVA: 0x0000ED74 File Offset: 0x0000CF74
	public static T Max<T>(this IList<T> self, Func<T, decimal> comparer)
	{
		T t = self.First<T>();
		decimal d = comparer(t);
		for (int i = 0; i < self.Count; i++)
		{
			T t2 = self[i];
			decimal num = comparer(t2);
			if (d < num || (d == num && UnityEngine.Random.value > 0.5f))
			{
				d = num;
				t = t2;
			}
		}
		return t;
	}

	// Token: 0x0600014D RID: 333 RVA: 0x00002C07 File Offset: 0x00000E07
	public static int Wrap(this int self, int max)
	{
		if (self >= 0)
		{
			return self % max;
		}
		return (self + -(self / max) * max + max) % max;
	}

	// Token: 0x0600014E RID: 334 RVA: 0x0000EDD8 File Offset: 0x0000CFD8
	public static int IndexOf<T>(this T[] self, Predicate<T> pred)
	{
		for (int i = 0; i < self.Length; i++)
		{
			if (pred(self[i]))
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x0600014F RID: 335 RVA: 0x0000EE08 File Offset: 0x0000D008
	public static Vector2 MapToRectangle(this Vector2 del, Vector2 widthAndHeight)
	{
		del = del.normalized;
		if (Mathf.Abs(del.x) > Mathf.Abs(del.y))
		{
			return new Vector2(Mathf.Sign(del.x) * widthAndHeight.x, del.y * widthAndHeight.y / 0.707106769f);
		}
		return new Vector2(del.x * widthAndHeight.x / 0.707106769f, Mathf.Sign(del.y) * widthAndHeight.y);
	}

	// Token: 0x06000150 RID: 336 RVA: 0x00002C1D File Offset: 0x00000E1D
	public static float AngleSignedRad(this Vector2 vector1, Vector2 vector2)
	{
		return Mathf.Atan2(vector2.y, vector2.x) - Mathf.Atan2(vector1.y, vector1.x);
	}

	// Token: 0x06000151 RID: 337 RVA: 0x00002C42 File Offset: 0x00000E42
	public static float AngleSigned(this Vector2 vector1, Vector2 vector2)
	{
		return vector1.AngleSignedRad(vector2) * 57.29578f;
	}

	// Token: 0x06000152 RID: 338 RVA: 0x0000EE8C File Offset: 0x0000D08C
	public static Vector2 Rotate(this Vector2 self, float degrees)
	{
		float f = 0.0174532924f * degrees;
		float num = Mathf.Cos(f);
		float num2 = Mathf.Sin(f);
		return new Vector2(self.x * num - num2 * self.y, self.x * num2 + num * self.y);
	}

	// Token: 0x06000153 RID: 339 RVA: 0x0000EED4 File Offset: 0x0000D0D4
	public static Vector3 RotateY(this Vector3 self, float degrees)
	{
		float f = 0.0174532924f * degrees;
		float num = Mathf.Cos(f);
		float num2 = Mathf.Sin(f);
		return new Vector3(self.x * num - num2 * self.z, self.y, self.x * num2 + num * self.z);
	}

	// Token: 0x06000154 RID: 340 RVA: 0x00002C51 File Offset: 0x00000E51
	public static bool TryToEnum<TEnum>(this string strEnumValue, out TEnum enumValue)
	{
		enumValue = default(TEnum);
		if (!Enum.IsDefined(typeof(TEnum), strEnumValue))
		{
			return false;
		}
		enumValue = (TEnum)((object)Enum.Parse(typeof(TEnum), strEnumValue));
		return true;
	}

	// Token: 0x06000155 RID: 341 RVA: 0x0000EF24 File Offset: 0x0000D124
	public static TEnum ToEnum<TEnum>(this string strEnumValue)
	{
		if (!Enum.IsDefined(typeof(TEnum), strEnumValue))
		{
			return default(TEnum);
		}
		return (TEnum)((object)Enum.Parse(typeof(TEnum), strEnumValue));
	}

	// Token: 0x06000156 RID: 342 RVA: 0x00002C8A File Offset: 0x00000E8A
	public static TEnum ToEnum<TEnum>(this string strEnumValue, TEnum defaultValue)
	{
		if (!Enum.IsDefined(typeof(TEnum), strEnumValue))
		{
			return defaultValue;
		}
		return (TEnum)((object)Enum.Parse(typeof(TEnum), strEnumValue));
	}

	// Token: 0x06000157 RID: 343 RVA: 0x00002CB5 File Offset: 0x00000EB5
	public static bool IsNullOrWhiteSpace(this string s)
	{
		if (s == null)
		{
			return true;
		}
		return !s.Any((char c) => !char.IsWhiteSpace(c));
	}

	// Token: 0x04000166 RID: 358
	private static string[] ByteHex = (from x in Enumerable.Range(0, 256)
	select x.ToString("X2")).ToArray<string>();
}
