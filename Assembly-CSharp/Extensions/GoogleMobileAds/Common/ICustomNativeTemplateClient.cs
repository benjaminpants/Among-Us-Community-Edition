using System;
using System.Collections.Generic;

namespace GoogleMobileAds.Common
{
	// Token: 0x02000269 RID: 617
	public interface ICustomNativeTemplateClient
	{
		// Token: 0x06000D82 RID: 3458
		string GetTemplateId();

		// Token: 0x06000D83 RID: 3459
		byte[] GetImageByteArray(string key);

		// Token: 0x06000D84 RID: 3460
		List<string> GetAvailableAssetNames();

		// Token: 0x06000D85 RID: 3461
		string GetText(string key);

		// Token: 0x06000D86 RID: 3462
		void PerformClick(string assetName);

		// Token: 0x06000D87 RID: 3463
		void RecordImpression();
	}
}
