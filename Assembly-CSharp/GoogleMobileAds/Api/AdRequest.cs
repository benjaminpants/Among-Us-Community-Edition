using System;
using System.Collections.Generic;
using GoogleMobileAds.Api.Mediation;

namespace GoogleMobileAds.Api
{
	// Token: 0x02000279 RID: 633
	public class AdRequest
	{
		// Token: 0x06000E09 RID: 3593 RVA: 0x0003FC34 File Offset: 0x0003DE34
		private AdRequest(AdRequest.Builder builder)
		{
			this.TestDevices = new List<string>(builder.TestDevices);
			this.Keywords = new HashSet<string>(builder.Keywords);
			this.Birthday = builder.Birthday;
			this.Gender = builder.Gender;
			this.TagForChildDirectedTreatment = builder.ChildDirectedTreatmentTag;
			this.Extras = new Dictionary<string, string>(builder.Extras);
			this.MediationExtras = builder.MediationExtras;
		}

		// Token: 0x17000203 RID: 515
		// (get) Token: 0x06000E0A RID: 3594 RVA: 0x0000A1CB File Offset: 0x000083CB
		// (set) Token: 0x06000E0B RID: 3595 RVA: 0x0000A1D3 File Offset: 0x000083D3
		public List<string> TestDevices { get; private set; }

		// Token: 0x17000204 RID: 516
		// (get) Token: 0x06000E0C RID: 3596 RVA: 0x0000A1DC File Offset: 0x000083DC
		// (set) Token: 0x06000E0D RID: 3597 RVA: 0x0000A1E4 File Offset: 0x000083E4
		public HashSet<string> Keywords { get; private set; }

		// Token: 0x17000205 RID: 517
		// (get) Token: 0x06000E0E RID: 3598 RVA: 0x0000A1ED File Offset: 0x000083ED
		// (set) Token: 0x06000E0F RID: 3599 RVA: 0x0000A1F5 File Offset: 0x000083F5
		public DateTime? Birthday { get; private set; }

		// Token: 0x17000206 RID: 518
		// (get) Token: 0x06000E10 RID: 3600 RVA: 0x0000A1FE File Offset: 0x000083FE
		// (set) Token: 0x06000E11 RID: 3601 RVA: 0x0000A206 File Offset: 0x00008406
		public Gender? Gender { get; private set; }

		// Token: 0x17000207 RID: 519
		// (get) Token: 0x06000E12 RID: 3602 RVA: 0x0000A20F File Offset: 0x0000840F
		// (set) Token: 0x06000E13 RID: 3603 RVA: 0x0000A217 File Offset: 0x00008417
		public bool? TagForChildDirectedTreatment { get; private set; }

		// Token: 0x17000208 RID: 520
		// (get) Token: 0x06000E14 RID: 3604 RVA: 0x0000A220 File Offset: 0x00008420
		// (set) Token: 0x06000E15 RID: 3605 RVA: 0x0000A228 File Offset: 0x00008428
		public Dictionary<string, string> Extras { get; private set; }

		// Token: 0x17000209 RID: 521
		// (get) Token: 0x06000E16 RID: 3606 RVA: 0x0000A231 File Offset: 0x00008431
		// (set) Token: 0x06000E17 RID: 3607 RVA: 0x0000A239 File Offset: 0x00008439
		public List<MediationExtras> MediationExtras { get; private set; }

		// Token: 0x04000CE0 RID: 3296
		public const string Version = "3.16.0";

		// Token: 0x04000CE1 RID: 3297
		public const string TestDeviceSimulator = "SIMULATOR";

		// Token: 0x0200027A RID: 634
		public class Builder
		{
			// Token: 0x06000E18 RID: 3608 RVA: 0x0003FCAC File Offset: 0x0003DEAC
			public Builder()
			{
				this.TestDevices = new List<string>();
				this.Keywords = new HashSet<string>();
				this.Birthday = null;
				this.Gender = null;
				this.ChildDirectedTreatmentTag = null;
				this.Extras = new Dictionary<string, string>();
				this.MediationExtras = new List<MediationExtras>();
			}

			// Token: 0x1700020A RID: 522
			// (get) Token: 0x06000E19 RID: 3609 RVA: 0x0000A242 File Offset: 0x00008442
			// (set) Token: 0x06000E1A RID: 3610 RVA: 0x0000A24A File Offset: 0x0000844A
			internal List<string> TestDevices { get; private set; }

			// Token: 0x1700020B RID: 523
			// (get) Token: 0x06000E1B RID: 3611 RVA: 0x0000A253 File Offset: 0x00008453
			// (set) Token: 0x06000E1C RID: 3612 RVA: 0x0000A25B File Offset: 0x0000845B
			internal HashSet<string> Keywords { get; private set; }

			// Token: 0x1700020C RID: 524
			// (get) Token: 0x06000E1D RID: 3613 RVA: 0x0000A264 File Offset: 0x00008464
			// (set) Token: 0x06000E1E RID: 3614 RVA: 0x0000A26C File Offset: 0x0000846C
			internal DateTime? Birthday { get; private set; }

			// Token: 0x1700020D RID: 525
			// (get) Token: 0x06000E1F RID: 3615 RVA: 0x0000A275 File Offset: 0x00008475
			// (set) Token: 0x06000E20 RID: 3616 RVA: 0x0000A27D File Offset: 0x0000847D
			internal Gender? Gender { get; private set; }

			// Token: 0x1700020E RID: 526
			// (get) Token: 0x06000E21 RID: 3617 RVA: 0x0000A286 File Offset: 0x00008486
			// (set) Token: 0x06000E22 RID: 3618 RVA: 0x0000A28E File Offset: 0x0000848E
			internal bool? ChildDirectedTreatmentTag { get; private set; }

			// Token: 0x1700020F RID: 527
			// (get) Token: 0x06000E23 RID: 3619 RVA: 0x0000A297 File Offset: 0x00008497
			// (set) Token: 0x06000E24 RID: 3620 RVA: 0x0000A29F File Offset: 0x0000849F
			internal Dictionary<string, string> Extras { get; private set; }

			// Token: 0x17000210 RID: 528
			// (get) Token: 0x06000E25 RID: 3621 RVA: 0x0000A2A8 File Offset: 0x000084A8
			// (set) Token: 0x06000E26 RID: 3622 RVA: 0x0000A2B0 File Offset: 0x000084B0
			internal List<MediationExtras> MediationExtras { get; private set; }

			// Token: 0x06000E27 RID: 3623 RVA: 0x0000A2B9 File Offset: 0x000084B9
			public AdRequest.Builder AddKeyword(string keyword)
			{
				this.Keywords.Add(keyword);
				return this;
			}

			// Token: 0x06000E28 RID: 3624 RVA: 0x0000A2C9 File Offset: 0x000084C9
			public AdRequest.Builder AddTestDevice(string deviceId)
			{
				this.TestDevices.Add(deviceId);
				return this;
			}

			// Token: 0x06000E29 RID: 3625 RVA: 0x0000A2D8 File Offset: 0x000084D8
			public AdRequest Build()
			{
				return new AdRequest(this);
			}

			// Token: 0x06000E2A RID: 3626 RVA: 0x0000A2E0 File Offset: 0x000084E0
			public AdRequest.Builder SetBirthday(DateTime birthday)
			{
				this.Birthday = new DateTime?(birthday);
				return this;
			}

			// Token: 0x06000E2B RID: 3627 RVA: 0x0000A2EF File Offset: 0x000084EF
			public AdRequest.Builder SetGender(Gender gender)
			{
				this.Gender = new Gender?(gender);
				return this;
			}

			// Token: 0x06000E2C RID: 3628 RVA: 0x0000A2FE File Offset: 0x000084FE
			public AdRequest.Builder AddMediationExtras(MediationExtras extras)
			{
				this.MediationExtras.Add(extras);
				return this;
			}

			// Token: 0x06000E2D RID: 3629 RVA: 0x0000A30D File Offset: 0x0000850D
			public AdRequest.Builder TagForChildDirectedTreatment(bool tagForChildDirectedTreatment)
			{
				this.ChildDirectedTreatmentTag = new bool?(tagForChildDirectedTreatment);
				return this;
			}

			// Token: 0x06000E2E RID: 3630 RVA: 0x0000A31C File Offset: 0x0000851C
			public AdRequest.Builder AddExtra(string key, string value)
			{
				this.Extras.Add(key, value);
				return this;
			}
		}
	}
}
