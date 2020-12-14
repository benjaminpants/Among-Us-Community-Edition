using UnityEngine;

public class DeadBody : MonoBehaviour
{
	public bool Reported;

	public short KillIdx;

	public byte ParentId;

	public Collider2D myCollider;

	public Vector2 TruePosition => base.transform.position + (Vector3)myCollider.offset;

	public void OnClick()
	{
		if (!Reported && !PhysicsHelpers.AnythingBetween(PlayerControl.LocalPlayer.GetTruePosition(), TruePosition, Constants.ShipAndObjectsMask, useTriggers: false))
		{
			Reported = true;
			GameData.PlayerInfo target = PlayerControl.LocalPlayer.Data;
			if (PlayerControl.GameOptions.Gamemode != 1)
			{
				target = GameData.Instance.GetPlayerById(ParentId);
			}
			PlayerControl.LocalPlayer.CmdReportDeadBody(target);
		}
	}
}
