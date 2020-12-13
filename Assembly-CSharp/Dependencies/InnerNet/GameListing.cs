using System;

namespace InnerNet
{
	[Serializable]
	public struct GameListing
	{
		public int GameId;

		public byte PlayerCount;

		public byte ImpostorCount;

		public byte MaxPlayers;

		public int Age;

		public string HostName;

		public GameListing(int id, byte numImpostors, byte playerCount, byte maxPlayers, int age, string host)
		{
			GameId = id;
			ImpostorCount = numImpostors;
			PlayerCount = playerCount;
			MaxPlayers = maxPlayers;
			Age = age;
			HostName = host;
		}
	}
}
