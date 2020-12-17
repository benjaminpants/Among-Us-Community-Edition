using System;

namespace InnerNet
{
	[Serializable]
	public class ClientData
	{
		public int Id;

		public bool InScene;

		public bool IsReady;

		public PlayerControl Character;

		public int LuaHash;

        public ClientData(int id)
        {
            Id = id;
        }
		public ClientData(int id, int hash)
		{
			Id = id;
			LuaHash = hash;
		}
	}
}
