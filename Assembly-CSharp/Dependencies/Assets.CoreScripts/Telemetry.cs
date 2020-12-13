using System;
using System.Collections.Generic;
using InnerNet;
using UnityEngine;
using UnityEngine.Analytics;

namespace Assets.CoreScripts
{
	public class Telemetry : DestroyableSingleton<Telemetry>
	{
		private static readonly string[] ColorNames = new string[10]
		{
			"Red",
			"Blue",
			"Green",
			"Pink",
			"Orange",
			"Yellow",
			"Black",
			"White",
			"Purple",
			"Brown"
		};

		private bool amHost;

		private bool gameStarted;

		private DateTime timeStarted;

		public bool IsInitialized;

		public Guid CurrentGuid;

		public void Initialize()
		{
			Initialize(Guid.NewGuid());
		}

		public void Initialize(Guid gameGuid)
		{
			IsInitialized = true;
			CurrentGuid = gameGuid;
		}

		public void StartGame(bool sendName, bool isHost, int playerCount, int impostorCount, GameModes gameMode, uint timesImpostor, uint gamesPlayed)
		{
			if (SaveManager.SendTelemetry)
			{
				gameStarted = true;
				amHost = isHost;
				timeStarted = DateTime.UtcNow;
				Dictionary<string, object> dictionary = new Dictionary<string, object>
				{
					{
						"Platform",
						(byte)Application.platform
					},
					{
						"TimesImpostor",
						timesImpostor
					},
					{
						"GamesPlayed",
						gamesPlayed
					},
					{
						"GameMode",
						gameMode
					}
				};
				if (amHost)
				{
					dictionary.Add("PlayerCount", playerCount);
					dictionary.Add("InfectedCount", impostorCount);
				}
				Analytics.CustomEvent("StartGame", dictionary);
			}
		}

		public void WriteMeetingStarted(bool isEmergency)
		{
			if (SaveManager.SendTelemetry && amHost)
			{
				Analytics.CustomEvent("MeetingStarted", new Dictionary<string, object>
				{
					{
						"IsEmergency",
						isEmergency
					}
				});
			}
		}

		public void WriteMeetingEnded(byte[] results, float duration)
		{
			if (SaveManager.SendTelemetry && amHost)
			{
				Analytics.CustomEvent("MeetingEnded", new Dictionary<string, object>
				{
					{
						"IsEmergency",
						duration
					}
				});
			}
		}

		public void WritePosition(byte playerNum, Vector2 worldPos)
		{
		}

		public void WriteMurder(byte sourcePlayerNum, byte targetPlayerNum, Vector3 worldPos)
		{
			if (SaveManager.SendTelemetry && gameStarted)
			{
				Analytics.CustomEvent("Murder");
			}
		}

		public void WriteSabotageUsed(SystemTypes systemType)
		{
			if (SaveManager.SendTelemetry && gameStarted)
			{
				Analytics.CustomEvent("SabotageUsed", new Dictionary<string, object>
				{
					{
						"SystemType",
						systemType
					}
				});
			}
		}

		public void WriteUse(byte playerNum, TaskTypes taskType, Vector3 worldPos)
		{
			if (SaveManager.SendTelemetry && gameStarted)
			{
				Analytics.CustomEvent("ConsoleUsed", new Dictionary<string, object>
				{
					{
						"TaskType",
						taskType
					}
				});
			}
		}

		public void WriteCompleteTask(byte playerNum, TaskTypes taskType)
		{
			if (SaveManager.SendTelemetry && gameStarted)
			{
				Analytics.CustomEvent("TaskComplete", new Dictionary<string, object>
				{
					{
						"TaskType",
						taskType
					}
				});
			}
		}

		internal void WriteDisconnect(DisconnectReasons reason)
		{
			if (SaveManager.SendTelemetry && gameStarted)
			{
				Analytics.CustomEvent("Disconnect", new Dictionary<string, object>
				{
					{
						"Reason",
						reason
					}
				});
			}
		}

		public void EndGame(GameOverReason endReason)
		{
			if (SaveManager.SendTelemetry && gameStarted)
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>
				{
					{
						"Reason",
						endReason
					}
				};
				if (amHost)
				{
					dictionary.Add("DurationSec", (DateTime.UtcNow - timeStarted).TotalSeconds);
				}
				Analytics.CustomEvent("EndGame", dictionary);
			}
		}

		public void SelectInfected(int colorId, uint hatId)
		{
			if (SaveManager.SendTelemetry && gameStarted)
			{
				Analytics.CustomEvent("SelectInfected", new Dictionary<string, object>
				{
					{
						"Color",
						ColorNames[colorId]
					},
					{
						"Hat",
						DestroyableSingleton<HatManager>.Instance.GetHatById(hatId).name
					}
				});
			}
		}

		public void WonGame(int colorId, uint hatId)
		{
			if (SaveManager.SendTelemetry && gameStarted)
			{
				Analytics.CustomEvent("WonGame", new Dictionary<string, object>
				{
					{
						"Color",
						ColorNames[colorId]
					},
					{
						"Hat",
						DestroyableSingleton<HatManager>.Instance.GetHatById(hatId).name
					}
				});
			}
		}
	}
}
