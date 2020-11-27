using System;

namespace InnerNet
{
	// Token: 0x02000258 RID: 600
	[Serializable]
	public struct GameListing
	{
		// Token: 0x06000D08 RID: 3336 RVA: 0x00009CD4 File Offset: 0x00007ED4
		public GameListing(int id, byte numImpostors, byte playerCount, byte maxPlayers, int age, string host)
		{
			this.GameId = id;
			this.ImpostorCount = numImpostors;
			this.PlayerCount = playerCount;
			this.MaxPlayers = maxPlayers;
			this.Age = age;
			this.HostName = host;
		}

		// Token: 0x04000C6F RID: 3183
		public int GameId;

		// Token: 0x04000C70 RID: 3184
		public byte PlayerCount;

		// Token: 0x04000C71 RID: 3185
		public byte ImpostorCount;

		// Token: 0x04000C72 RID: 3186
		public byte MaxPlayers;

		// Token: 0x04000C73 RID: 3187
		public int Age;

		// Token: 0x04000C74 RID: 3188
		public string HostName;
	}
}
