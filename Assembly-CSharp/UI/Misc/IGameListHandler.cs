using System.Collections.Generic;
using InnerNet;

public interface IGameListHandler
{
	void HandleList(int totalGames, List<GameListing> availableGames);
}
