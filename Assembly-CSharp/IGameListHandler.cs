using System;
using System.Collections.Generic;
using InnerNet;

// Token: 0x0200013F RID: 319
public interface IGameListHandler
{
	// Token: 0x060006B9 RID: 1721
	void HandleList(int totalGames, List<GameListing> availableGames);
}
