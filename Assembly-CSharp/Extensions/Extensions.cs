using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class Extensions
{
	private static string[] ByteHex = (from x in Enumerable.Range(0, 256)
		select x.ToString("X2")).ToArray();

	public static void TrimEnd(this StringBuilder self)
	{
		int num = self.Length - 1;
		while (num >= 0)
		{
			char c = self[num];
			if (c == ' ' || c == '\t' || c == '\n' || c == '\r')
			{
				self.Length--;
				num--;
				continue;
			}
			break;
		}
	}

	public static void AddUnique<T>(this IList<T> self, T item)
	{
		if (!self.Contains(item))
		{
			self.Add(item);
		}
	}

	public static SystemTypes[] RoomsToSab = new SystemTypes[]
	{
		SystemTypes.Storage,
		SystemTypes.Security,
		SystemTypes.Electrical,
		SystemTypes.UpperEngine,
		SystemTypes.LowerEngine,
		SystemTypes.Cafeteria,
		SystemTypes.MedBay
	};


	public static string ToTextColor(this Color c)
	{
		return "[" + ByteHex[(byte)(c.r * 255f)] + ByteHex[(byte)(c.g * 255f)] + ByteHex[(byte)(c.b * 255f)] + ByteHex[(byte)(c.a * 255f)] + "]";
	}

	public static int ToInteger(this Color c, bool alpha)
	{
		if (alpha)
		{
			return ((byte)(c.r * 256f) << 24) | ((byte)(c.g * 256f) << 16) | ((byte)(c.b * 256f) << 8) | (byte)(c.a * 256f);
		}
		return ((byte)(c.r * 256f) << 16) | ((byte)(c.g * 256f) << 8) | (byte)(c.b * 256f);
	}

	public static bool HasAnyBit(this int self, int bit)
	{
		return (self & bit) != 0;
	}

	public static bool HasAnyBit(this byte self, byte bit)
	{
		return (self & bit) != 0;
	}

	public static bool HasBit(this byte self, byte bit)
	{
		return (self & bit) == bit;
	}

	public static int BitCount(this byte self)
	{
		int num = 0;
		for (int i = 0; i < 8; i++)
		{
			if (((1 << i) & self) != 0)
			{
				num++;
			}
		}
		return num;
	}

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

	public static void SetAll<T>(this IList<T> self, T value)
	{
		for (int i = 0; i < self.Count; i++)
		{
			self[i] = value;
		}
	}

	public static void AddAll<T>(this List<T> self, IList<T> other)
	{
		self.Capacity = Math.Max(self.Capacity, self.Count + other.Count);
		for (int i = 0; i < other.Count; i++)
		{
			self.Add(other[i]);
		}
	}

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

	public static T[] RandomSet<T>(this IList<T> self, int length)
	{
		T[] array = new T[length];
		self.RandomFill(array);
		return array;
	}

	public static void RandomFill<T>(this IList<T> self, T[] target)
	{
		HashSet<int> hashSet = new HashSet<int>();
		for (int i = 0; i < target.Length; i++)
		{
			int num;
			do
			{
				num = self.RandomIdx();
			}
			while (hashSet.Contains(num));
			target[i] = self[num];
			hashSet.Add(num);
			if (hashSet.Count == self.Count)
			{
				break;
			}
		}
	}

	public static int RandomIdx<T>(this IList<T> self)
	{
		return UnityEngine.Random.Range(0, self.Count);
	}

	public static int RandomIdx<T>(this IEnumerable<T> self)
	{
		return UnityEngine.Random.Range(0, self.Count());
	}

	public static T Random<T>(this IEnumerable<T> self)
	{
		return self.ToArray().Random();
	}

	public static T Random<T>(this IList<T> self)
	{
		if (self.Count > 0)
		{
			return self[UnityEngine.Random.Range(0, self.Count)];
		}
		return default(T);
	}

	public static Vector2 Div(this Vector2 a, Vector2 b)
	{
		return new Vector2(a.x / b.x, a.y / b.y);
	}

	public static Vector2 Mul(this Vector2 a, Vector2 b)
	{
		return new Vector2(a.x * b.x, a.y * b.y);
	}

	public static Vector3 Mul(this Vector3 a, Vector3 b)
	{
		return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
	}

	public static Vector3 Inv(this Vector3 a)
	{
		return new Vector3(1f / a.x, 1f / a.y, 1f / a.z);
	}

	public static Rect Lerp(this Rect source, Rect target, float t)
	{
		Rect result = default(Rect);
		result.position = Vector2.Lerp(source.position, target.position, t);
		result.size = Vector2.Lerp(source.size, target.size, t);
		return result;
	}

	public static void ForEach<T>(this IList<T> self, Action<T> todo)
	{
		for (int i = 0; i < self.Count; i++)
		{
			todo(self[i]);
		}
	}

	public static T Max<T>(this IList<T> self, Func<T, float> comparer)
	{
		T val = self.First();
		float num = comparer(val);
		for (int i = 0; i < self.Count; i++)
		{
			T val2 = self[i];
			float num2 = comparer(val2);
			if (num < num2 || (num == num2 && UnityEngine.Random.value > 0.5f))
			{
				num = num2;
				val = val2;
			}
		}
		return val;
	}

	public static T Max<T>(this IList<T> self, Func<T, decimal> comparer)
	{
		T val = self.First();
		decimal d = comparer(val);
		for (int i = 0; i < self.Count; i++)
		{
			T val2 = self[i];
			decimal num = comparer(val2);
			if (d < num || (d == num && UnityEngine.Random.value > 0.5f))
			{
				d = num;
				val = val2;
			}
		}
		return val;
	}

	public static int Wrap(this int self, int max)
	{
		if (self >= 0)
		{
			return self % max;
		}
		return (self + -(self / max) * max + max) % max;
	}

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

	public static Vector2 MapToRectangle(this Vector2 del, Vector2 widthAndHeight)
	{
		del = del.normalized;
		if (Mathf.Abs(del.x) > Mathf.Abs(del.y))
		{
			return new Vector2(Mathf.Sign(del.x) * widthAndHeight.x, del.y * widthAndHeight.y / 0.707106769f);
		}
		return new Vector2(del.x * widthAndHeight.x / 0.707106769f, Mathf.Sign(del.y) * widthAndHeight.y);
	}

	public static float AngleSignedRad(this Vector2 vector1, Vector2 vector2)
	{
		return Mathf.Atan2(vector2.y, vector2.x) - Mathf.Atan2(vector1.y, vector1.x);
	}

	public static float AngleSigned(this Vector2 vector1, Vector2 vector2)
	{
		return vector1.AngleSignedRad(vector2) * 57.29578f;
	}

	public static Vector2 Rotate(this Vector2 self, float degrees)
	{
		float f = (float)Math.PI / 180f * degrees;
		float num = Mathf.Cos(f);
		float num2 = Mathf.Sin(f);
		return new Vector2(self.x * num - num2 * self.y, self.x * num2 + num * self.y);
	}

	public static Vector3 RotateY(this Vector3 self, float degrees)
	{
		float f = (float)Math.PI / 180f * degrees;
		float num = Mathf.Cos(f);
		float num2 = Mathf.Sin(f);
		return new Vector3(self.x * num - num2 * self.z, self.y, self.x * num2 + num * self.z);
	}

	public static bool TryToEnum<TEnum>(this string strEnumValue, out TEnum enumValue)
	{
		enumValue = default(TEnum);
		if (!Enum.IsDefined(typeof(TEnum), strEnumValue))
		{
			return false;
		}
		enumValue = (TEnum)Enum.Parse(typeof(TEnum), strEnumValue);
		return true;
	}

	public static TEnum ToEnum<TEnum>(this string strEnumValue)
	{
		if (!Enum.IsDefined(typeof(TEnum), strEnumValue))
		{
			return default(TEnum);
		}
		return (TEnum)Enum.Parse(typeof(TEnum), strEnumValue);
	}

	public static TEnum ToEnum<TEnum>(this string strEnumValue, TEnum defaultValue)
	{
		if (!Enum.IsDefined(typeof(TEnum), strEnumValue))
		{
			return defaultValue;
		}
		return (TEnum)Enum.Parse(typeof(TEnum), strEnumValue);
	}

	public static bool IsNullOrWhiteSpace(this string s)
	{
		if (s == null)
		{
			return true;
		}
		if (s.Any((char c) => !char.IsWhiteSpace(c)))
		{
			return false;
		}
		return true;
	}
}
