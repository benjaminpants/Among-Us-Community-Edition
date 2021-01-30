using System;

namespace InnerNet
{
	[Serializable]
	public struct GameListing
	{
		public int GameId;

		public byte PlayerCount;

		public int ListingID;

		public string Icon;

		public int ImpostorCount;

		public byte MaxPlayers;

		public int Age;

		public string HostName;

		public GameListing(int id, int numImpostors, byte playerCount, byte maxPlayers, int age, string host, int listid, string iconname = "skeld.png")
		{
			GameId = id;
			ImpostorCount = numImpostors;
			PlayerCount = playerCount;
			MaxPlayers = maxPlayers;
			Age = age;
			HostName = host;
			ListingID = listid;
			Icon = iconname;
		}
	}
}
