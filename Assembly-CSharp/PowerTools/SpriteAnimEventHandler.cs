using System;
using UnityEngine;

namespace PowerTools
{
	// Token: 0x0200023A RID: 570
	[DisallowMultipleComponent]
	public class SpriteAnimEventHandler : MonoBehaviour
	{
		// Token: 0x06000C43 RID: 3139 RVA: 0x00009658 File Offset: 0x00007858
		private void _Anim(string function)
		{
			base.SendMessageUpwards(function, SendMessageOptions.DontRequireReceiver);
		}

		// Token: 0x06000C44 RID: 3140 RVA: 0x0003B9E0 File Offset: 0x00039BE0
		private void _AnimInt(string messageString)
		{
			int num = SpriteAnimEventHandler.EventParser.ParseInt(ref messageString);
			base.SendMessageUpwards(messageString, num, SendMessageOptions.DontRequireReceiver);
		}

		// Token: 0x06000C45 RID: 3141 RVA: 0x0003BA04 File Offset: 0x00039C04
		private void _AnimFloat(string messageString)
		{
			float num = SpriteAnimEventHandler.EventParser.ParseFloat(ref messageString);
			base.SendMessageUpwards(messageString, num, SendMessageOptions.DontRequireReceiver);
		}

		// Token: 0x06000C46 RID: 3142 RVA: 0x0003BA28 File Offset: 0x00039C28
		private void _AnimString(string messageString)
		{
			string value = SpriteAnimEventHandler.EventParser.ParseString(ref messageString);
			base.SendMessageUpwards(messageString, value, SendMessageOptions.DontRequireReceiver);
		}

		// Token: 0x06000C47 RID: 3143 RVA: 0x0003BA48 File Offset: 0x00039C48
		private void _AnimObjectFunc(string funcName)
		{
			if (this.m_eventWithObjectData != null)
			{
				base.SendMessageUpwards(funcName, this.m_eventWithObjectData, SendMessageOptions.DontRequireReceiver);
				this.m_eventWithObjectMessage = null;
				this.m_eventWithObjectData = null;
				return;
			}
			if (!string.IsNullOrEmpty(this.m_eventWithObjectMessage))
			{
				Debug.LogError("Animation event with object parameter had no object");
			}
			this.m_eventWithObjectMessage = funcName;
		}

		// Token: 0x06000C48 RID: 3144 RVA: 0x0003BA98 File Offset: 0x00039C98
		private void _AnimObjectData(UnityEngine.Object data)
		{
			if (!string.IsNullOrEmpty(this.m_eventWithObjectMessage))
			{
				base.SendMessageUpwards(this.m_eventWithObjectMessage, data, SendMessageOptions.DontRequireReceiver);
				this.m_eventWithObjectMessage = null;
				this.m_eventWithObjectData = null;
				return;
			}
			if (this.m_eventWithObjectData != null)
			{
				Debug.LogError("Animation event with object parameter had no object");
			}
			this.m_eventWithObjectData = data;
		}

		// Token: 0x04000BC1 RID: 3009
		private string m_eventWithObjectMessage;

		// Token: 0x04000BC2 RID: 3010
		private object m_eventWithObjectData;

		// Token: 0x0200023B RID: 571
		public static class EventParser
		{
			// Token: 0x06000C4A RID: 3146 RVA: 0x0003BAE8 File Offset: 0x00039CE8
			public static int ParseInt(ref string messageString)
			{
				int num = messageString.IndexOf(SpriteAnimEventHandler.EventParser.MESSAGE_DELIMITER);
				int result = 0;
				int.TryParse(messageString.Substring(num + 1), out result);
				messageString = messageString.Substring(0, num);
				return result;
			}

			// Token: 0x06000C4B RID: 3147 RVA: 0x0003BB24 File Offset: 0x00039D24
			public static float ParseFloat(ref string messageString)
			{
				int num = messageString.IndexOf(SpriteAnimEventHandler.EventParser.MESSAGE_DELIMITER);
				float result = 0f;
				float.TryParse(messageString.Substring(num + 1), out result);
				messageString = messageString.Substring(0, num);
				return result;
			}

			// Token: 0x06000C4C RID: 3148 RVA: 0x0003BB64 File Offset: 0x00039D64
			public static string ParseString(ref string messageString)
			{
				int num = messageString.IndexOf(SpriteAnimEventHandler.EventParser.MESSAGE_DELIMITER);
				string result = messageString.Substring(num + 1);
				messageString = messageString.Substring(0, num);
				return result;
			}

			// Token: 0x04000BC3 RID: 3011
			public static readonly char MESSAGE_DELIMITER = '\t';

			// Token: 0x04000BC4 RID: 3012
			public static readonly string MESSAGE_NOPARAM = "_Anim";

			// Token: 0x04000BC5 RID: 3013
			public static readonly string MESSAGE_INT = "_AnimInt";

			// Token: 0x04000BC6 RID: 3014
			public static readonly string MESSAGE_FLOAT = "_AnimFloat";

			// Token: 0x04000BC7 RID: 3015
			public static readonly string MESSAGE_STRING = "_AnimString";

			// Token: 0x04000BC8 RID: 3016
			public static readonly string MESSAGE_OBJECT_FUNCNAME = "_AnimObjectFunc";

			// Token: 0x04000BC9 RID: 3017
			public static readonly string MESSAGE_OBJECT_DATA = "_AnimObjectData";
		}
	}
}
