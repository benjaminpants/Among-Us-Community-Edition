using System;
using System.Collections.Generic;
using InnerNet;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000136 RID: 310
public class FindAGameManager : DestroyableSingleton<FindAGameManager>, IGameListHandler
{
	// Token: 0x0600068B RID: 1675 RVA: 0x00006153 File Offset: 0x00004353
	public void ResetTimer()
	{
		this.timer = 5f;
		this.RefreshSpinner.Appear();
		this.RefreshSpinner.StartPulse();
	}

	// Token: 0x0600068C RID: 1676 RVA: 0x00027040 File Offset: 0x00025240
	public void Start()
	{
		if (!AmongUsClient.Instance)
		{
			AmongUsClient.Instance = UnityEngine.Object.FindObjectOfType<AmongUsClient>();
			if (!AmongUsClient.Instance)
			{
				SceneManager.LoadScene("MMOnline");
				return;
			}
		}
		AmongUsClient.Instance.GameListHandlers.Add(this);
		this.HandleList(1, new List<GameListing>
		{
			new GameListing
			{
				GameId = 0,
				HostName = "\n\nPublic Lobbies are not supported.\n[FFFF00FF]The game is funner with friends anyway![]",
				ImpostorCount = 0,
				MaxPlayers = 10,
				PlayerCount = 0,
				Age = 69
			}
		});
	}

	// Token: 0x0600068D RID: 1677 RVA: 0x000270DC File Offset: 0x000252DC
	public void Update()
	{
		this.timer += Time.deltaTime;
		GameOptionsData gameSearchOptions = SaveManager.GameSearchOptions;
		if ((this.timer < 0f || this.timer > 5f) && gameSearchOptions.MapId != 0)
		{
			this.RefreshSpinner.Appear();
		}
		else
		{
			this.RefreshSpinner.Disappear();
		}
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			this.ExitGame();
		}
	}

	// Token: 0x0600068E RID: 1678 RVA: 0x00006176 File Offset: 0x00004376
	public void RefreshList()
	{
		this.RefreshSpinner.Disappear();
	}

	// Token: 0x0600068F RID: 1679 RVA: 0x00006183 File Offset: 0x00004383
	public override void OnDestroy()
	{
		if (AmongUsClient.Instance)
		{
			AmongUsClient.Instance.GameListHandlers.Remove(this);
		}
		base.OnDestroy();
	}

	// Token: 0x06000690 RID: 1680 RVA: 0x0002714C File Offset: 0x0002534C
	public void HandleList(int totalGames, List<GameListing> availableGames)
	{
		Debug.Log(string.Format("TotalGames: {0}\tAvailable: {1}", totalGames, availableGames.Count));
		this.RefreshSpinner.Disappear();
		this.timer = float.MinValue;
		availableGames.Sort(FindAGameManager.GameSorter.Instance);
		while (this.buttonPool.activeChildren.Count > availableGames.Count)
		{
			PoolableBehavior poolableBehavior = this.buttonPool.activeChildren[this.buttonPool.activeChildren.Count - 1];
			poolableBehavior.OwnerPool.Reclaim(poolableBehavior);
		}
		while (this.buttonPool.activeChildren.Count < availableGames.Count)
		{
			this.buttonPool.Get<PoolableBehavior>().transform.SetParent(this.TargetArea.Inner);
		}
		Vector3 vector = new Vector3(0f, this.ButtonStart, -1f);
		for (int i = 0; i < this.buttonPool.activeChildren.Count; i++)
		{
			MatchMakerGameButton matchMakerGameButton = (MatchMakerGameButton)this.buttonPool.activeChildren[i];
			matchMakerGameButton.SetGame(availableGames[i]);
			matchMakerGameButton.transform.localPosition = vector;
			vector.y -= this.ButtonHeight;
		}
		this.TargetArea.YBounds.max = Mathf.Max(0f, -vector.y - this.ButtonStart);
	}

	// Token: 0x06000691 RID: 1681 RVA: 0x000061A8 File Offset: 0x000043A8
	public void ExitGame()
	{
		AmongUsClient.Instance.ExitGame(DisconnectReasons.ExitGame);
	}

	// Token: 0x06000693 RID: 1683 RVA: 0x000272B8 File Offset: 0x000254B8
	public void NoPublicGames()
	{
		this.RefreshSpinner.Disappear();
		this.timer = float.MinValue;
		Vector3 vector = new Vector3(0f, this.ButtonStart, -1f);
		MatchMakerGameButton matchMakerGameButton = (MatchMakerGameButton)this.buttonPool.activeChildren[0];
		matchMakerGameButton.SetNoGame();
		matchMakerGameButton.transform.localPosition = vector;
		vector.y -= this.ButtonHeight;
		this.TargetArea.YBounds.max = Mathf.Max(0f, -vector.y - this.ButtonStart);
	}

	// Token: 0x04000656 RID: 1622
	private const float RefreshTime = 5f;

	// Token: 0x04000657 RID: 1623
	private float timer;

	// Token: 0x04000658 RID: 1624
	public ObjectPoolBehavior buttonPool;

	// Token: 0x04000659 RID: 1625
	public SpinAnimator RefreshSpinner;

	// Token: 0x0400065A RID: 1626
	public Scroller TargetArea;

	// Token: 0x0400065B RID: 1627
	public float ButtonStart = 1.75f;

	// Token: 0x0400065C RID: 1628
	public float ButtonHeight = 0.6f;

	// Token: 0x0400065D RID: 1629
	public const bool showPrivate = false;

	// Token: 0x02000137 RID: 311
	private class GameSorter : IComparer<GameListing>
	{
		// Token: 0x06000694 RID: 1684 RVA: 0x000061D3 File Offset: 0x000043D3
		public int Compare(GameListing x, GameListing y)
		{
			return -x.PlayerCount.CompareTo(y.PlayerCount);
		}

		// Token: 0x0400065E RID: 1630
		public static readonly FindAGameManager.GameSorter Instance = new FindAGameManager.GameSorter();
	}
}
