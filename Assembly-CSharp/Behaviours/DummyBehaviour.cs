using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200017A RID: 378
public class DummyBehaviour : MonoBehaviour
{
	// Token: 0x060007C7 RID: 1991 RVA: 0x00006CA7 File Offset: 0x00004EA7
	public void Start()
	{
		this.myPlayer = base.GetComponent<PlayerControl>();
	}

	// Token: 0x060007C8 RID: 1992 RVA: 0x0002C230 File Offset: 0x0002A430
	public void Update()
	{
		if (this.myPlayer.Data.IsDead)
		{
			return;
		}
		if (MeetingHud.Instance)
		{
			if (!this.voted)
			{
				this.voted = true;
				base.StartCoroutine(this.DoVote());
				return;
			}
		}
		else
		{
			this.voted = false;
		}
	}

	// Token: 0x060007C9 RID: 1993 RVA: 0x00006CB5 File Offset: 0x00004EB5
	private IEnumerator DoVote()
	{
		yield return new WaitForSeconds(this.voteTime.Next());
		sbyte suspectIdx = -1;
		int num = 0;
		while (num < 100 && num != 99)
		{
			int num2 = IntRange.Next(-1, GameData.Instance.PlayerCount);
			if (num2 < 0)
			{
				suspectIdx = (sbyte)num2;
				break;
			}
			GameData.PlayerInfo playerInfo = GameData.Instance.AllPlayers[num2];
			if (!playerInfo.IsDead)
			{
				suspectIdx = (sbyte)playerInfo.PlayerId;
				break;
			}
			num++;
		}
		MeetingHud.Instance.CmdCastVote(this.myPlayer.PlayerId, suspectIdx);
		yield break;
	}

	// Token: 0x040007A7 RID: 1959
	private PlayerControl myPlayer;

	// Token: 0x040007A8 RID: 1960
	private FloatRange voteTime = new FloatRange(3f, 8f);

	// Token: 0x040007A9 RID: 1961
	private bool voted;
}
