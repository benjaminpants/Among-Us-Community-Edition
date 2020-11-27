using System;
using PowerTools;
using UnityEngine;

// Token: 0x020001E0 RID: 480
public class ReactorRoomWire : MonoBehaviour
{
	// Token: 0x06000A5B RID: 2651 RVA: 0x00035548 File Offset: 0x00033748
	public void Update()
	{
		if (this.reactor == null)
		{
			ISystemType systemType;
			if (!ShipStatus.Instance || !ShipStatus.Instance.Systems.TryGetValue(SystemTypes.Reactor, out systemType))
			{
				return;
			}
			this.reactor = (ReactorSystemType)systemType;
		}
		if (this.reactor.IsActive)
		{
			if (this.reactor.GetConsoleComplete(this.myConsole.ConsoleId))
			{
				if (!this.Image.IsPlaying(this.MeltdownReady))
				{
					this.Image.Play(this.MeltdownReady, 1f);
					return;
				}
			}
			else if (!this.Image.IsPlaying(this.MeltdownNeed))
			{
				this.Image.Play(this.MeltdownNeed, 1f);
				return;
			}
		}
		else if (!this.Image.IsPlaying(this.Normal))
		{
			this.Image.Play(this.Normal, 1f);
		}
	}

	// Token: 0x040009EF RID: 2543
	public global::Console myConsole;

	// Token: 0x040009F0 RID: 2544
	public SpriteAnim Image;

	// Token: 0x040009F1 RID: 2545
	public AnimationClip Normal;

	// Token: 0x040009F2 RID: 2546
	public AnimationClip MeltdownNeed;

	// Token: 0x040009F3 RID: 2547
	public AnimationClip MeltdownReady;

	// Token: 0x040009F4 RID: 2548
	private ReactorSystemType reactor;
}
