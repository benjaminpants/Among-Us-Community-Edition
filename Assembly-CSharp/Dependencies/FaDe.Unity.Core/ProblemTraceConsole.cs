using System.Collections.Generic;
using UnityEngine;

namespace FaDe.Unity.Core
{
	public class ProblemTraceConsole : MonoBehaviour
	{
		private struct Log
		{
			public string message;

			public string stackTrace;

			public LogType type;

			public int counter;

			public int Counter
			{
				get
				{
					return counter;
				}
				set
				{
					if (value <= 9999)
					{
						counter = value;
					}
				}
			}
		}

		public KeyCode toggleKey = KeyCode.Backspace;

		private List<Log> logs = new List<Log>();

		private Vector2 scrollPosition;

		public bool show;

		private static readonly Dictionary<LogType, Color> logTypeColors;

		private Rect windowRect = new Rect(20f, 20f, Screen.width - 40, Screen.height - 40);

		private Rect titleBarRect = new Rect(0f, 0f, 10000f, 20f);

		private static ProblemTraceConsole instance;

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

		private void OnEnable()
		{
			if (instance == null)
			{
				instance = this;
			}
			else if (instance != this)
			{
				Object.Destroy(this);
				return;
			}
			Application.RegisterLogCallback(HandleLog);
		}

		private void Update()
		{
			if ((Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)) && CE_Input.CE_GetKeyDown(toggleKey))
			{
				show = !show;
			}
			if (CE_Input.CE_GetKeyDown(KeyCode.C))
			{
				logs.Clear();
			}
		}

		private void OnGUI()
		{
			if (show)
			{
				windowRect = GUILayout.Window(-2, windowRect, ConsoleWindow, "Problem Trace Console");
			}
		}

		private void ConsoleWindow(int windowID)
		{
			scrollPosition = GUILayout.BeginScrollView(scrollPosition);
			for (int i = 0; i < logs.Count; i++)
			{
				Log log = logs[i];
				if (i <= 0 || !(log.message == logs[i - 1].message))
				{
					GUI.contentColor = logTypeColors[log.type];
					GUILayout.BeginHorizontal();
					GUILayout.Label(log.message);
					GUILayout.Label(log.Counter.ToString(), counterLabel);
					GUILayout.EndHorizontal();
					GUILayout.Label(log.stackTrace);
				}
			}
			GUILayout.EndScrollView();
			GUI.contentColor = Color.white;
			if (GUILayout.Button("Clear"))
			{
				logs.Clear();
			}
			GUI.DragWindow(titleBarRect);
		}

		private void HandleLog(string message, string stackTrace, LogType type)
		{
			if (logs.Count > 0)
			{
				int index = logs.Count - 1;
				if (message == logs[index].message && stackTrace == logs[index].stackTrace)
				{
					Log value = logs[index];
					value.Counter++;
					logs[index] = value;
					return;
				}
			}
			logs.Add(new Log
			{
				counter = 1,
				message = message,
				stackTrace = stackTrace,
				type = type
			});
		}

		public void Clear()
		{
			logs.Clear();
		}

		public static void Init()
		{
			if (!(instance != null))
			{
				GameObject gameObject = new GameObject();
				gameObject.AddComponent<ProblemTraceConsole>();
				Object.DontDestroyOnLoad(gameObject);
			}
		}

		static ProblemTraceConsole()
		{
			logTypeColors = new Dictionary<LogType, Color>
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
		}
	}
}
