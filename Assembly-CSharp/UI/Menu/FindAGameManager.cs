using System.Collections.Generic;
using InnerNet;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FindAGameManager : DestroyableSingleton<FindAGameManager>, IGameListHandler
{
	private class GameSorter : IComparer<GameListing>
	{
		public static readonly GameSorter Instance;

		public int Compare(GameListing x, GameListing y)
		{
			return -x.PlayerCount.CompareTo(y.PlayerCount);
		}

		static GameSorter()
		{
			Instance = new GameSorter();
		}
	}

	private const float RefreshTime = 5f;

	private float timer;

	public ObjectPoolBehavior buttonPool;

	public SpinAnimator RefreshSpinner;

	public Scroller TargetArea;

	public float ButtonStart = 1.75f;

	public float ButtonHeight = 0.6f;

	public const bool showPrivate = false;

	public byte Page;

	public void ResetTimer()
	{
		timer = 5f;
		RefreshSpinner.Appear();
		RefreshSpinner.StartPulse();
	}

	public void Start()
	{
		Page = 0;
		if (!AmongUsClient.Instance)
		{
			AmongUsClient.Instance = Object.FindObjectOfType<AmongUsClient>();
			if (!AmongUsClient.Instance)
			{
				SceneManager.LoadScene("MMOnline");
				return;
			}
		}
		AmongUsClient.Instance.GameListHandlers.Add(this);
		List<GameListing> GameLists = new List<GameListing>();
		for (int i=0; i < ServerManager.Instance.availableServers.Length; i++)
        {
			Debug.Log("adding server");
			ServerInfo fo = ServerManager.Instance.availableServers[i];
			GameLists.Add(new GameListing
			{
				GameId = i,
				HostName = fo.Name + "\n" + fo.Ip + ":" + fo.Port,
				ImpostorCount = 1,
				MaxPlayers = 21,
				PlayerCount = 0,
				Age = i,
				ListingID = i,
				Icon = fo.Icon
			});
		}
		HandleList(GameLists.Count, GameLists);
	}

	public void SetPage(byte page)
    {
		Page = page;
    }

	public void Update()
	{
		if (CE_Input.CE_GetKeyUp(KeyCode.Escape))
		{
			ExitGame();
		}
	}

	public void RefreshList()
	{
		RefreshSpinner.Disappear();
	}

	public override void OnDestroy()
	{
		if ((bool)AmongUsClient.Instance)
		{
			AmongUsClient.Instance.GameListHandlers.Remove(this);
		}
		base.OnDestroy();
	}

	public void HandleList(int totalGames, List<GameListing> availableGames)
	{
		Debug.Log("handling list");
		Debug.Log($"TotalGames: {totalGames}\tAvailable: {availableGames.Count}");
		RefreshSpinner.Disappear();
		timer = float.MinValue;
		availableGames.Sort(GameSorter.Instance);
		while (buttonPool.activeChildren.Count > availableGames.Count)
		{
			PoolableBehavior poolableBehavior = buttonPool.activeChildren[buttonPool.activeChildren.Count - 1];
			poolableBehavior.OwnerPool.Reclaim(poolableBehavior);
		}
		while (buttonPool.activeChildren.Count < availableGames.Count)
		{
			buttonPool.Get<PoolableBehavior>().transform.SetParent(TargetArea.Inner);
		}
		Vector3 localPosition = new Vector3(0f, ButtonStart, -1f);
		for (int i = 0; i < buttonPool.activeChildren.Count; i++)
		{
			MatchMakerGameButton obj = (MatchMakerGameButton)buttonPool.activeChildren[i];
			obj.SetGame(availableGames[i]);
			obj.transform.localPosition = localPosition;
			localPosition.y -= ButtonHeight;
		}
		TargetArea.YBounds.max = Mathf.Max(0f, 0f - localPosition.y - ButtonStart);
	}

	public void ExitGame()
	{
		AmongUsClient.Instance.ExitGame();
	}

	public void NoPublicGames()
	{
		RefreshSpinner.Disappear();
		timer = float.MinValue;
		Vector3 localPosition = new Vector3(0f, ButtonStart, -1f);
		MatchMakerGameButton obj = (MatchMakerGameButton)buttonPool.activeChildren[0];
		obj.SetNoGame();
		obj.transform.localPosition = localPosition;
		localPosition.y -= ButtonHeight;
		TargetArea.YBounds.max = Mathf.Max(0f, 0f - localPosition.y - ButtonStart);
	}
}
