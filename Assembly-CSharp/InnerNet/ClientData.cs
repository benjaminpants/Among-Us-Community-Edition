using System;

namespace InnerNet
{
	// Token: 0x0200023F RID: 575
	[Serializable]
	public class ClientData
	{
		// Token: 0x06000C5C RID: 3164 RVA: 0x000096BD File Offset: 0x000078BD
		public ClientData(int id)
		{
			this.Id = id;
		}

		// Token: 0x04000BE7 RID: 3047
		public int Id;

		// Token: 0x04000BE8 RID: 3048
		public bool InScene;

		// Token: 0x04000BE9 RID: 3049
		public bool IsReady;

		// Token: 0x04000BEA RID: 3050
		public PlayerControl Character;
	}
}
