using UnityEngine;

namespace PowerTools
{
	[DisallowMultipleComponent]
	public class SpriteAnimEventHandler : MonoBehaviour
	{
		public static class EventParser
		{
			public static readonly char MESSAGE_DELIMITER = '\t';

			public static readonly string MESSAGE_NOPARAM = "_Anim";

			public static readonly string MESSAGE_INT = "_AnimInt";

			public static readonly string MESSAGE_FLOAT = "_AnimFloat";

			public static readonly string MESSAGE_STRING = "_AnimString";

			public static readonly string MESSAGE_OBJECT_FUNCNAME = "_AnimObjectFunc";

			public static readonly string MESSAGE_OBJECT_DATA = "_AnimObjectData";

			public static int ParseInt(ref string messageString)
			{
				int num = messageString.IndexOf(MESSAGE_DELIMITER);
				int result = 0;
				int.TryParse(messageString.Substring(num + 1), out result);
				messageString = messageString.Substring(0, num);
				return result;
			}

			public static float ParseFloat(ref string messageString)
			{
				int num = messageString.IndexOf(MESSAGE_DELIMITER);
				float result = 0f;
				float.TryParse(messageString.Substring(num + 1), out result);
				messageString = messageString.Substring(0, num);
				return result;
			}

			public static string ParseString(ref string messageString)
			{
				int num = messageString.IndexOf(MESSAGE_DELIMITER);
				string result = messageString.Substring(num + 1);
				messageString = messageString.Substring(0, num);
				return result;
			}
		}

		private string m_eventWithObjectMessage;

		private object m_eventWithObjectData;

		private void _Anim(string function)
		{
			SendMessageUpwards(function, SendMessageOptions.DontRequireReceiver);
		}

		private void _AnimInt(string messageString)
		{
			int num = EventParser.ParseInt(ref messageString);
			SendMessageUpwards(messageString, num, SendMessageOptions.DontRequireReceiver);
		}

		private void _AnimFloat(string messageString)
		{
			float num = EventParser.ParseFloat(ref messageString);
			SendMessageUpwards(messageString, num, SendMessageOptions.DontRequireReceiver);
		}

		private void _AnimString(string messageString)
		{
			string value = EventParser.ParseString(ref messageString);
			SendMessageUpwards(messageString, value, SendMessageOptions.DontRequireReceiver);
		}

		private void _AnimObjectFunc(string funcName)
		{
			if (m_eventWithObjectData != null)
			{
				SendMessageUpwards(funcName, m_eventWithObjectData, SendMessageOptions.DontRequireReceiver);
				m_eventWithObjectMessage = null;
				m_eventWithObjectData = null;
				return;
			}
			if (!string.IsNullOrEmpty(m_eventWithObjectMessage))
			{
				Debug.LogError("Animation event with object parameter had no object");
			}
			m_eventWithObjectMessage = funcName;
		}

		private void _AnimObjectData(Object data)
		{
			if (!string.IsNullOrEmpty(m_eventWithObjectMessage))
			{
				SendMessageUpwards(m_eventWithObjectMessage, data, SendMessageOptions.DontRequireReceiver);
				m_eventWithObjectMessage = null;
				m_eventWithObjectData = null;
				return;
			}
			if (m_eventWithObjectData != null)
			{
				Debug.LogError("Animation event with object parameter had no object");
			}
			m_eventWithObjectData = data;
		}
	}
}
