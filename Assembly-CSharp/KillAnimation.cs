using System;
using System.Collections;
using PowerTools;
using UnityEngine;

// Token: 0x0200017C RID: 380
public class KillAnimation : MonoBehaviour
{
	// Token: 0x060007D1 RID: 2001 RVA: 0x00006CF8 File Offset: 0x00004EF8
	public IEnumerator CoPerformKill(PlayerControl source, PlayerControl target)
	{
		bool isParticipant = PlayerControl.LocalPlayer == source || PlayerControl.LocalPlayer == target;
		PlayerPhysics sourcePhys = source.GetComponent<PlayerPhysics>();
		KillAnimation.SetMovement(source, false);
		KillAnimation.SetMovement(target, false);
		if (isParticipant)
		{
			Camera.main.GetComponent<FollowerCamera>().Locked = true;
		}
		bool iszomb = PlayerControl.GameOptions.Gamemode == 1;
		if (!iszomb)
		{
			target.Die(DeathReason.Kill);
		}
		SpriteAnim sourceAnim = source.GetComponent<SpriteAnim>();
		yield return new WaitForAnimationFinish(sourceAnim, this.BlurAnim);
		source.NetTransform.SnapTo(target.transform.position);
		sourceAnim.Play(sourcePhys.IdleAnim, 1f);
		KillAnimation.SetMovement(source, true);
		DeadBody deadBody = UnityEngine.Object.Instantiate<DeadBody>(this.bodyPrefab);
		Vector3 vector = target.transform.position + this.BodyOffset;
		vector.z = vector.y / 1000f;
		deadBody.transform.position = vector;
		deadBody.ParentId = target.PlayerId;
		if (!iszomb)
		{
			target.SetPlayerMaterialColors(deadBody.GetComponent<Renderer>());
		}
		KillAnimation.SetMovement(target, true);
		if (isParticipant)
		{
			Camera.main.GetComponent<FollowerCamera>().Locked = false;
		}
		yield break;
	}

	// Token: 0x060007D2 RID: 2002 RVA: 0x00006D15 File Offset: 0x00004F15
	public static void SetMovement(PlayerControl source, bool canMove)
	{
		source.moveable = canMove;
		source.NetTransform.enabled = canMove;
		source.MyPhysics.enabled = canMove;
		source.MyPhysics.ResetAnim(false);
		source.NetTransform.Halt();
	}

	// Token: 0x040007AD RID: 1965
	public AnimationClip BlurAnim;

	// Token: 0x040007AE RID: 1966
	public DeadBody bodyPrefab;

	// Token: 0x040007AF RID: 1967
	public Vector3 BodyOffset;
}
