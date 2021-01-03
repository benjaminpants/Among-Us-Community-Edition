using System.Collections;
using UnityEngine;

public class TutorialManager : DestroyableSingleton<TutorialManager>
{
	public PlayerControl PlayerPrefab;

	public Transform[] DummyLocations;

	public override void Awake()
	{
		base.Awake();
		StatsManager.Instance = new TutorialStatsManager();
		StartCoroutine(RunTutorial());
	}

	public override void OnDestroy()
	{
		StatsManager.Instance = new StatsManager();
		base.OnDestroy();
	}

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
		int CurrentPlayer = 1;
		while (CurrentPlayer < 20)
        {
			for (int i = 0; i < DummyLocations.Length; i++)
			{
				if (CurrentPlayer < 20)
                {
					PlayerControl playerControl = Object.Instantiate(PlayerPrefab);
					playerControl.PlayerId = (byte)GameData.Instance.GetAvailableId();
					GameData.Instance.AddPlayer(playerControl);
					AmongUsClient.Instance.Spawn(playerControl);
					playerControl.transform.position = DummyLocations[i].position;
					playerControl.transform.position += new Vector3(Random.Range(-0.2f,0.2f), Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f));
					playerControl.GetComponent<DummyBehaviour>().enabled = true;
					playerControl.NetTransform.enabled = false;
					playerControl.SetName("Dummy " + CurrentPlayer);
					playerControl.SetColor((byte)((CurrentPlayer < SaveManager.BodyColor) ? CurrentPlayer : (CurrentPlayer + 1)));
                    playerControl.SetHat((uint)Random.Range(0, HatManager.Instance.AllHats.Count));
					playerControl.SetSkin((uint)Random.Range(0, HatManager.Instance.AllSkins.Count));
					GameData.Instance.RpcSetTasks(playerControl.PlayerId, new byte[0]);
					CurrentPlayer++;
				}
			}
		}
		ShipStatus.Instance.Begin();
	}
}
