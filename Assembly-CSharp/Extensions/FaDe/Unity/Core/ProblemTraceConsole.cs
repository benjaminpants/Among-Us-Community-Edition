using System;
using System.Collections.Generic;
using UnityEngine;

namespace FaDe.Unity.Core
{
	// Token: 0x020002B4 RID: 692
	public class ProblemTraceConsole : MonoBehaviour
	{
		// Token: 0x06000ECC RID: 3788 RVA: 0x0000A983 File Offset: 0x00008B83
		private void OnEnable()
		{
			if (ProblemTraceConsole.instance == null)
			{
				ProblemTraceConsole.instance = this;
			}
			else if (ProblemTraceConsole.instance != this)
			{
				UnityEngine.Object.Destroy(this);
				return;
			}
			Application.RegisterLogCallback(new Application.LogCallback(this.HandleLog));
		}

		// Token: 0x06000ECD RID: 3789 RVA: 0x00040EFC File Offset: 0x0003F0FC
		private void Update()
		{
			if ((Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)) && Input.GetKeyDown(this.toggleKey))
			{
				this.show = !this.show;
			}
			if (Input.GetKeyDown(KeyCode.C))
			{
				this.logs.Clear();
			}
		}

		// Token: 0x06000ECE RID: 3790 RVA: 0x0000A9BF File Offset: 0x00008BBF
		private void OnGUI()
		{
			if (!this.show)
			{
				return;
			}
			this.windowRect = GUILayout.Window(-1, this.windowRect, new GUI.WindowFunction(this.ConsoleWindow), "Problem Trace Console", new GUILayoutOption[0]);
		}

		// Token: 0x06000ECF RID: 3791 RVA: 0x00040F54 File Offset: 0x0003F154
		private void ConsoleWindow(int windowID)
		{
			this.scrollPosition = GUILayout.BeginScrollView(this.scrollPosition, new GUILayoutOption[0]);
			for (int i = 0; i < this.logs.Count; i++)
			{
				ProblemTraceConsole.Log log = this.logs[i];
				if (i <= 0 || !(log.message == this.logs[i - 1].message))
				{
					GUI.contentColor = ProblemTraceConsole.logTypeColors[log.type];
					GUILayout.BeginHorizontal(new GUILayoutOption[0]);
					GUILayout.Label(log.message, new GUILayoutOption[0]);
					GUILayout.Label(log.Counter.ToString(), this.counterLabel, new GUILayoutOption[0]);
					GUILayout.EndHorizontal();
					GUILayout.Label(log.stackTrace, new GUILayoutOption[0]);
				}
			}
			GUILayout.EndScrollView();
			GUI.contentColor = Color.white;
			if (GUILayout.Button("Clear", new GUILayoutOption[0]))
			{
				this.logs.Clear();
			}
			GUI.DragWindow(this.titleBarRect);
		}

		// Token: 0x06000ED0 RID: 3792 RVA: 0x00041064 File Offset: 0x0003F264
		private void HandleLog(string message, string stackTrace, LogType type)
		{
			if (this.logs.Count > 0)
			{
				int index = this.logs.Count - 1;
				if (message == this.logs[index].message && stackTrace == this.logs[index].stackTrace)
				{
					ProblemTraceConsole.Log value = this.logs[index];
					int counter = value.Counter;
					value.Counter = counter + 1;
					this.logs[index] = value;
					return;
				}
			}
			this.logs.Add(new ProblemTraceConsole.Log
			{
				counter = 1,
				message = message,
				stackTrace = stackTrace,
				type = type
			});
		}

		// Token: 0x06000ED1 RID: 3793 RVA: 0x0000A9F3 File Offset: 0x00008BF3
		public void Clear()
		{
			this.logs.Clear();
		}

		// Token: 0x06000ED2 RID: 3794 RVA: 0x0000AA00 File Offset: 0x00008C00
		public static void Init()
		{
			if (ProblemTraceConsole.instance != null)
			{
				return;
			}
			GameObject gameObject = new GameObject();
			gameObject.AddComponent<ProblemTraceConsole>();
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
		}

		// Token: 0x04000D5F RID: 3423
		public KeyCode toggleKey = KeyCode.Backspace;

		// Token: 0x04000D60 RID: 3424
		private List<ProblemTraceConsole.Log> logs = new List<ProblemTraceConsole.Log>();

		// Token: 0x04000D61 RID: 3425
		private Vector2 scrollPosition;

		// Token: 0x04000D62 RID: 3426
		public bool show;

		// Token: 0x04000D63 RID: 3427
		private static readonly Dictionary<LogType, Color> logTypeColors = new Dictionary<LogType, Color>
		{
			{
				LogType.Assert,
				Color.white
			},
			{
				LogType.Error,
				Color.red
			},
			{
				LogType.Exception,
				Color.red
			},
			{
				LogType.Log,
				Color.white
			},
			{
				LogType.Warning,
				Color.yellow
			}
		};

		// Token: 0x04000D64 RID: 3428
		private Rect windowRect = new Rect(20f, 20f, (float)(Screen.width - 40), (float)(Screen.height - 40));

		// Token: 0x04000D65 RID: 3429
		private Rect titleBarRect = new Rect(0f, 0f, 10000f, 20f);

		// Token: 0x04000D66 RID: 3430
		private static ProblemTraceConsole instance;

		// Token: 0x04000D67 RID: 3431
		private GUIStyle counterLabel = new GUIStyle
		{
			alignment = TextAnchor.UpperRight,
			fixedWidth = 40f,
			margin = new RectOffset(0, 10, 8, 0),
			normal = 
			{
				textColor = Color.white
			}
		};

		// Token: 0x020002B5 RID: 693
		private struct Log
		{
			// Token: 0x1700021B RID: 539
			// (get) Token: 0x06000ED5 RID: 3797 RVA: 0x0000AA21 File Offset: 0x00008C21
			// (set) Token: 0x06000ED6 RID: 3798 RVA: 0x0000AA29 File Offset: 0x00008C29
			public int Counter
			{
				get
				{
					return this.counter;
				}
				set
				{
					if (value > 9999)
					{
						return;
					}
					this.counter = value;
				}
			}

			// Token: 0x04000D68 RID: 3432
			public string message;

			// Token: 0x04000D69 RID: 3433
			public string stackTrace;

			// Token: 0x04000D6A RID: 3434
			public LogType type;

			// Token: 0x04000D6B RID: 3435
			public int counter;
		}
	}
}
