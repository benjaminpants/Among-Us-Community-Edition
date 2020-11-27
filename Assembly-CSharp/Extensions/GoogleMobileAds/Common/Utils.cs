using System;
using UnityEngine;

namespace GoogleMobileAds.Common
{
	// Token: 0x02000270 RID: 624
	internal class Utils
	{
		// Token: 0x06000DDD RID: 3549 RVA: 0x00009FE1 File Offset: 0x000081E1
		public static void CheckInitialization()
		{
			if (!MobileAdsEventExecutor.IsActive())
			{
				Debug.Log("You intitialized an ad object but have not yet called MobileAds.Initialize(). We highly recommend you call MobileAds.Initialize() before interacting with the Google Mobile Ads SDK.");
			}
			MobileAdsEventExecutor.Initialize();
		}

		// Token: 0x06000DDE RID: 3550 RVA: 0x00009FF9 File Offset: 0x000081F9
		public static Texture2D GetTexture2DFromByteArray(byte[] img)
		{
			Texture2D texture2D = new Texture2D(1, 1);
			if (!texture2D.LoadImage(img))
			{
				throw new InvalidOperationException("Could not load custom native template\n                        image asset as texture");
			}
			return texture2D;
		}
	}
}
