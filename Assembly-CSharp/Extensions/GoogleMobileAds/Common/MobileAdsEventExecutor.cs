using System;
using System.Collections.Generic;
using UnityEngine;

namespace GoogleMobileAds.Common
{
	// Token: 0x0200026E RID: 622
	public class MobileAdsEventExecutor : MonoBehaviour
	{
		// Token: 0x06000DC3 RID: 3523 RVA: 0x00009F79 File Offset: 0x00008179
		public static void Initialize()
		{
			if (MobileAdsEventExecutor.IsActive())
			{
				return;
			}
			GameObject gameObject = new GameObject("MobileAdsMainThreadExecuter");
			gameObject.hideFlags = HideFlags.HideAndDontSave;
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			MobileAdsEventExecutor.instance = gameObject.AddComponent<MobileAdsEventExecutor>();
		}

		// Token: 0x06000DC4 RID: 3524 RVA: 0x00009FA5 File Offset: 0x000081A5
		public static bool IsActive()
		{
			return MobileAdsEventExecutor.instance != null;
		}

		// Token: 0x06000DC5 RID: 3525 RVA: 0x00009FB2 File Offset: 0x000081B2
		public void Awake()
		{
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}

		// Token: 0x06000DC6 RID: 3526 RVA: 0x0003F708 File Offset: 0x0003D908
		public static void ExecuteInUpdate(Action action)
		{
			List<Action> obj = MobileAdsEventExecutor.adEventsQueue;
			lock (obj)
			{
				MobileAdsEventExecutor.adEventsQueue.Add(action);
				MobileAdsEventExecutor.adEventsQueueEmpty = false;
			}
		}

		// Token: 0x06000DC7 RID: 3527 RVA: 0x0003F754 File Offset: 0x0003D954
		public void Update()
		{
			if (MobileAdsEventExecutor.adEventsQueueEmpty)
			{
				return;
			}
			List<Action> list = new List<Action>();
			List<Action> obj = MobileAdsEventExecutor.adEventsQueue;
			lock (obj)
			{
				list.AddRange(MobileAdsEventExecutor.adEventsQueue);
				MobileAdsEventExecutor.adEventsQueue.Clear();
				MobileAdsEventExecutor.adEventsQueueEmpty = true;
			}
			foreach (Action action in list)
			{
				action();
			}
		}

		// Token: 0x06000DC8 RID: 3528 RVA: 0x00009FBF File Offset: 0x000081BF
		public void OnDisable()
		{
			MobileAdsEventExecutor.instance = null;
		}

		// Token: 0x04000CBA RID: 3258
		public static MobileAdsEventExecutor instance = null;

		// Token: 0x04000CBB RID: 3259
		private static List<Action> adEventsQueue = new List<Action>();

		// Token: 0x04000CBC RID: 3260
		private static volatile bool adEventsQueueEmpty = true;
	}
}
