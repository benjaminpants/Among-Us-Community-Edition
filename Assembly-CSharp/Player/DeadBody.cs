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
			GameData.PlayerInfo playerById = GameData.Instance.GetPlayerById(ParentId);
			PlayerControl.LocalPlayer.CmdReportDeadBody(playerById);
		}
	}
}
