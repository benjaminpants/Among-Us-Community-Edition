using System;
using System.Collections.Generic;
using GoogleMobileAds.Common;
using UnityEngine;

namespace GoogleMobileAds.Api
{
	// Token: 0x0200027E RID: 638
	public class CustomNativeTemplateAd
	{
		// Token: 0x06000E57 RID: 3671 RVA: 0x0000A484 File Offset: 0x00008684
		internal CustomNativeTemplateAd(ICustomNativeTemplateClient client)
		{
			this.client = client;
		}

		// Token: 0x06000E58 RID: 3672 RVA: 0x0000A493 File Offset: 0x00008693
		public List<string> GetAvailableAssetNames()
		{
			return this.client.GetAvailableAssetNames();
		}

		// Token: 0x06000E59 RID: 3673 RVA: 0x0000A4A0 File Offset: 0x000086A0
		public string GetCustomTemplateId()
		{
			return this.client.GetTemplateId();
		}

		// Token: 0x06000E5A RID: 3674 RVA: 0x0004016C File Offset: 0x0003E36C
		public Texture2D GetTexture2D(string key)
		{
			byte[] imageByteArray = this.client.GetImageByteArray(key);
			if (imageByteArray == null)
			{
				return null;
			}
			return Utils.GetTexture2DFromByteArray(imageByteArray);
		}

		// Token: 0x06000E5B RID: 3675 RVA: 0x0000A4AD File Offset: 0x000086AD
		public string GetText(string key)
		{
			return this.client.GetText(key);
		}

		// Token: 0x06000E5C RID: 3676 RVA: 0x0000A4BB File Offset: 0x000086BB
		public void PerformClick(string assetName)
		{
			this.client.PerformClick(assetName);
		}

		// Token: 0x06000E5D RID: 3677 RVA: 0x0000A4C9 File Offset: 0x000086C9
		public void RecordImpression()
		{
			this.client.RecordImpression();
		}

		// Token: 0x04000D00 RID: 3328
		private ICustomNativeTemplateClient client;
	}
}
