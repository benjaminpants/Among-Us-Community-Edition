namespace GoogleMobileAds.Api
{
	public class AdSize
	{
		private bool isSmartBanner;

		private int width;

		private int height;

		public static readonly AdSize Banner = new AdSize(320, 50);

		public static readonly AdSize MediumRectangle = new AdSize(300, 250);

		public static readonly AdSize IABBanner = new AdSize(468, 60);

		public static readonly AdSize Leaderboard = new AdSize(728, 90);

		public static readonly AdSize SmartBanner = new AdSize(isSmartBanner: true);

		public static readonly int FullWidth = -1;

		public int Width => width;

		public int Height => height;

		public bool IsSmartBanner => isSmartBanner;

		public AdSize(int width, int height)
		{
			isSmartBanner = false;
			this.width = width;
			this.height = height;
		}

		private AdSize(bool isSmartBanner)
			: this(0, 0)
		{
			this.isSmartBanner = isSmartBanner;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			AdSize adSize = (AdSize)obj;
			if (width == adSize.width && height == adSize.height)
			{
				return isSmartBanner == adSize.isSmartBanner;
			}
			return false;
		}

		public static bool operator ==(AdSize a, AdSize b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(AdSize a, AdSize b)
		{
			return !a.Equals(b);
		}

		public override int GetHashCode()
		{
			int num = 11;
			return (((((71 * num) ^ width.GetHashCode()) * num) ^ height.GetHashCode()) * num) ^ isSmartBanner.GetHashCode();
		}
	}
}
