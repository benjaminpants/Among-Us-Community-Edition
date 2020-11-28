using System;
using System.Collections;
using InnerNet;
using UnityEngine;

// Token: 0x0200022E RID: 558
public class TutorialManager : DestroyableSingleton<TutorialManager>
{
	// Token: 0x06000BEE RID: 3054 RVA: 0x0000928D File Offset: 0x0000748D
	public override void Awake()
	{
		base.Awake();
		StatsManager.Instance = new TutorialStatsManager();
		base.StartCoroutine(this.RunTutorial());
	}

	// Token: 0x06000BEF RID: 3055 RVA: 0x000092AC File Offset: 0x000074AC
	public override void OnDestroy()
	{
		StatsManager.Instance = new StatsManager();
		base.OnDestroy();
	}

	// Token: 0x06000BF0 RID: 3056 RVA: 0x000092BE File Offset: 0x000074BE
	private IEnumerator RunTutorial()
	{
		while (!ShipStatus.Instance)
		{
			yield return null;
		}
		ShipStatus.Instance.enabled = false;
		ShipStatus.Instance.Timer = 15f;
		while (!PlayerControl.LocalPlayer)
		{
			yield return null;
		}
		if (DestroyableSingleton<DiscordManager>.InstanceExists)
		{
			DestroyableSingleton<DiscordManager>.Instance.SetHowToPlay();
		}
		PlayerControl.GameOptions = new GameOptionsData
		{
			NumImpostors = 0,
			DiscussionTime = 0
		};
		PlayerControl.LocalPlayer.RpcSetInfected(new GameData.PlayerInfo[0]);
		for (int i = 0; i < this.DummyLocations.Length; i++)
		{
			PlayerControl playerControl = UnityEngine.Object.Instantiate<PlayerControl>(this.PlayerPrefab);
			playerControl.PlayerId = (byte)GameData.Instance.GetAvailableId();
			GameData.Instance.AddPlayer(playerControl);
			AmongUsClient.Instance.Spawn(playerControl, -2, SpawnFlags.None);
			playerControl.transform.position = this.DummyLocations[i].position;
			playerControl.GetComponent<DummyBehaviour>().enabled = true;
			playerControl.NetTransform.enabled = false;
			playerControl.SetName("Dummy " + (i + 1));
			playerControl.SetColor((byte)((i < (int)SaveManager.BodyColor) ? i : (i + 1)));
			GameData.Instance.RpcSetTasks(playerControl.PlayerId, new byte[0]);
		}
		ShipStatus.Instance.Begin();
		yield break;
	}

	// Token: 0x04000B8B RID: 2955
	public PlayerControl PlayerPrefab;

	// Token: 0x04000B8C RID: 2956
	public Transform[] DummyLocations;
}
