using System;
using System.Collections.Generic;
using InnerNet;
using UnityEngine;
using UnityEngine.Analytics;

namespace Assets.CoreScripts
{
	// Token: 0x02000286 RID: 646
	public class Telemetry : DestroyableSingleton<Telemetry>
	{
		// Token: 0x06000EBB RID: 3771 RVA: 0x0000A808 File Offset: 0x00008A08
		public void Initialize()
		{
			this.Initialize(Guid.NewGuid());
		}

		// Token: 0x06000EBC RID: 3772 RVA: 0x0000A815 File Offset: 0x00008A15
		public void Initialize(Guid gameGuid)
		{
			this.IsInitialized = true;
			this.CurrentGuid = gameGuid;
		}

		// Token: 0x06000EBD RID: 3773 RVA: 0x00040C74 File Offset: 0x0003EE74
		public void StartGame(bool sendName, bool isHost, int playerCount, int impostorCount, GameModes gameMode, uint timesImpostor, uint gamesPlayed)
		{
			if (!SaveManager.SendTelemetry)
			{
				return;
			}
			this.gameStarted = true;
			this.amHost = isHost;
			this.timeStarted = DateTime.UtcNow;
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
			if (this.amHost)
			{
				dictionary.Add("PlayerCount", playerCount);
				dictionary.Add("InfectedCount", impostorCount);
			}
			Analytics.CustomEvent("StartGame", dictionary);
		}

		// Token: 0x06000EBE RID: 3774 RVA: 0x0000A825 File Offset: 0x00008A25
		public void WriteMeetingStarted(bool isEmergency)
		{
			if (!SaveManager.SendTelemetry)
			{
				return;
			}
			if (!this.amHost)
			{
				return;
			}
			Analytics.CustomEvent("MeetingStarted", new Dictionary<string, object>
			{
				{
					"IsEmergency",
					isEmergency
				}
			});
		}

		// Token: 0x06000EBF RID: 3775 RVA: 0x0000A859 File Offset: 0x00008A59
		public void WriteMeetingEnded(byte[] results, float duration)
		{
			if (!SaveManager.SendTelemetry)
			{
				return;
			}
			if (!this.amHost)
			{
				return;
			}
			Analytics.CustomEvent("MeetingEnded", new Dictionary<string, object>
			{
				{
					"IsEmergency",
					duration
				}
			});
		}

		// Token: 0x06000EC0 RID: 3776 RVA: 0x00002265 File Offset: 0x00000465
		public void WritePosition(byte playerNum, Vector2 worldPos)
		{
		}

		// Token: 0x06000EC1 RID: 3777 RVA: 0x0000A88D File Offset: 0x00008A8D
		public void WriteMurder(byte sourcePlayerNum, byte targetPlayerNum, Vector3 worldPos)
		{
			if (!SaveManager.SendTelemetry)
			{
				return;
			}
			if (!this.gameStarted)
			{
				return;
			}
			Analytics.CustomEvent("Murder");
		}

		// Token: 0x06000EC2 RID: 3778 RVA: 0x0000A8AB File Offset: 0x00008AAB
		public void WriteSabotageUsed(SystemTypes systemType)
		{
			if (!SaveManager.SendTelemetry)
			{
				return;
			}
			if (!this.gameStarted)
			{
				return;
			}
			Analytics.CustomEvent("SabotageUsed", new Dictionary<string, object>
			{
				{
					"SystemType",
					systemType
				}
			});
		}

		// Token: 0x06000EC3 RID: 3779 RVA: 0x0000A8DF File Offset: 0x00008ADF
		public void WriteUse(byte playerNum, TaskTypes taskType, Vector3 worldPos)
		{
			if (!SaveManager.SendTelemetry)
			{
				return;
			}
			if (!this.gameStarted)
			{
				return;
			}
			Analytics.CustomEvent("ConsoleUsed", new Dictionary<string, object>
			{
				{
					"TaskType",
					taskType
				}
			});
		}

		// Token: 0x06000EC4 RID: 3780 RVA: 0x0000A913 File Offset: 0x00008B13
		public void WriteCompleteTask(byte playerNum, TaskTypes taskType)
		{
			if (!SaveManager.SendTelemetry)
			{
				return;
			}
			if (!this.gameStarted)
			{
				return;
			}
			Analytics.CustomEvent("TaskComplete", new Dictionary<string, object>
			{
				{
					"TaskType",
					taskType
				}
			});
		}

		// Token: 0x06000EC5 RID: 3781 RVA: 0x0000A947 File Offset: 0x00008B47
		internal void WriteDisconnect(DisconnectReasons reason)
		{
			if (!SaveManager.SendTelemetry)
			{
				return;
			}
			if (!this.gameStarted)
			{
				return;
			}
			Analytics.CustomEvent("Disconnect", new Dictionary<string, object>
			{
				{
					"Reason",
					reason
				}
			});
		}

		// Token: 0x06000EC6 RID: 3782 RVA: 0x00040D2C File Offset: 0x0003EF2C
		public void EndGame(GameOverReason endReason)
		{
			if (!SaveManager.SendTelemetry)
			{
				return;
			}
			if (!this.gameStarted)
			{
				return;
			}
			Dictionary<string, object> dictionary = new Dictionary<string, object>
			{
				{
					"Reason",
					endReason
				}
			};
			if (this.amHost)
			{
				dictionary.Add("DurationSec", (DateTime.UtcNow - this.timeStarted).TotalSeconds);
			}
			Analytics.CustomEvent("EndGame", dictionary);
		}

		// Token: 0x06000EC7 RID: 3783 RVA: 0x00040DA0 File Offset: 0x0003EFA0
		public void SelectInfected(int colorId, uint hatId)
		{
			if (!SaveManager.SendTelemetry)
			{
				return;
			}
			if (!this.gameStarted)
			{
				return;
			}
			Analytics.CustomEvent("SelectInfected", new Dictionary<string, object>
			{
				{
					"Color",
					Telemetry.ColorNames[colorId]
				},
				{
					"Hat",
					DestroyableSingleton<HatManager>.Instance.GetHatById(hatId).name
				}
			});
		}

		// Token: 0x06000EC8 RID: 3784 RVA: 0x00040DFC File Offset: 0x0003EFFC
		public void WonGame(int colorId, uint hatId)
		{
			if (!SaveManager.SendTelemetry)
			{
				return;
			}
			if (!this.gameStarted)
			{
				return;
			}
			Analytics.CustomEvent("WonGame", new Dictionary<string, object>
			{
				{
					"Color",
					Telemetry.ColorNames[colorId]
				},
				{
					"Hat",
					DestroyableSingleton<HatManager>.Instance.GetHatById(hatId).name
				}
			});
		}

		// Token: 0x04000D20 RID: 3360
		private static readonly string[] ColorNames = new string[]
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

		// Token: 0x04000D21 RID: 3361
		private bool amHost;

		// Token: 0x04000D22 RID: 3362
		private bool gameStarted;

		// Token: 0x04000D23 RID: 3363
		private DateTime timeStarted;

		// Token: 0x04000D24 RID: 3364
		public bool IsInitialized;

		// Token: 0x04000D25 RID: 3365
		public Guid CurrentGuid;
	}
}
