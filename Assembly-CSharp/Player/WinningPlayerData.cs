public class WinningPlayerData
{
	public string Name;

	public bool IsDead;

	public uint ColorId;

	public uint SkinId;

	public uint HatId;

	public bool IsYou;

	public WinningPlayerData()
	{
	}

	public WinningPlayerData(GameData.PlayerInfo player)
	{
		IsYou = player.Object == PlayerControl.LocalPlayer;
		Name = player.PlayerName;
		IsDead = player.IsDead || player.Disconnected;
		ColorId = player.ColorId;
		SkinId = player.SkinId;
		HatId = player.HatId;
	}
}
