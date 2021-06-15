public class WinningPlayerData
{
	public string Name;

	public bool IsDead;

	public int ColorId;

	public uint SkinId;

	public uint HatId;

	public byte RoleID;

	public bool IsYou;

	public bool IsImp;

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
		RoleID = player.role;
		IsImp = player.IsImpostor;
	}
}
