using System;

namespace GoogleMobileAds.Api
{
	// Token: 0x0200027B RID: 635
	public class AdSize
	{
		// Token: 0x06000E2F RID: 3631 RVA: 0x0000A32C File Offset: 0x0000852C
		public AdSize(int width, int height)
		{
			this.isSmartBanner = false;
			this.width = width;
			this.height = height;
		}

		// Token: 0x06000E30 RID: 3632 RVA: 0x0000A349 File Offset: 0x00008549
		private AdSize(bool isSmartBanner) : this(0, 0)
		{
			this.isSmartBanner = isSmartBanner;
		}

		// Token: 0x17000211 RID: 529
		// (get) Token: 0x06000E31 RID: 3633 RVA: 0x0000A35A File Offset: 0x0000855A
		public int Width
		{
			get
			{
				return this.width;
			}
		}

		// Token: 0x17000212 RID: 530
		// (get) Token: 0x06000E32 RID: 3634 RVA: 0x0000A362 File Offset: 0x00008562
		public int Height
		{
			get
			{
				return this.height;
			}
		}

		// Token: 0x17000213 RID: 531
		// (get) Token: 0x06000E33 RID: 3635 RVA: 0x0000A36A File Offset: 0x0000856A
		public bool IsSmartBanner
		{
			get
			{
				return this.isSmartBanner;
			}
		}

		// Token: 0x06000E34 RID: 3636 RVA: 0x0003FD18 File Offset: 0x0003DF18
		public override bool Equals(object obj)
		{
			if (obj == null || base.GetType() != obj.GetType())
			{
				return false;
			}
			AdSize adSize = (AdSize)obj;
			return this.width == adSize.width && this.height == adSize.height && this.isSmartBanner == adSize.isSmartBanner;
		}

		// Token: 0x06000E35 RID: 3637 RVA: 0x0000A372 File Offset: 0x00008572
		public static bool operator ==(AdSize a, AdSize b)
		{
			return a.Equals(b);
		}

		// Token: 0x06000E36 RID: 3638 RVA: 0x0000A37B File Offset: 0x0000857B
		public static bool operator !=(AdSize a, AdSize b)
		{
			return !a.Equals(b);
		}

		// Token: 0x06000E37 RID: 3639 RVA: 0x0003FD70 File Offset: 0x0003DF70
		public override int GetHashCode()
		{
			int num = 71;
			int num2 = 11;
			return ((num * num2 ^ this.width.GetHashCode()) * num2 ^ this.height.GetHashCode()) * num2 ^ this.isSmartBanner.GetHashCode();
		}

		// Token: 0x04000CF0 RID: 3312
		private bool isSmartBanner;

		// Token: 0x04000CF1 RID: 3313
		private int width;

		// Token: 0x04000CF2 RID: 3314
		private int height;

		// Token: 0x04000CF3 RID: 3315
		public static readonly AdSize Banner = new AdSize(320, 50);

		// Token: 0x04000CF4 RID: 3316
		public static readonly AdSize MediumRectangle = new AdSize(300, 250);

		// Token: 0x04000CF5 RID: 3317
		public static readonly AdSize IABBanner = new AdSize(468, 60);

		// Token: 0x04000CF6 RID: 3318
		public static readonly AdSize Leaderboard = new AdSize(728, 90);

		// Token: 0x04000CF7 RID: 3319
		public static readonly AdSize SmartBanner = new AdSize(true);

		// Token: 0x04000CF8 RID: 3320
		public static readonly int FullWidth = -1;
	}
}
