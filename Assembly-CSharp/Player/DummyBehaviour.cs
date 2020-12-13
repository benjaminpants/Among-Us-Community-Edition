using System.Collections;
using UnityEngine;

public class DummyBehaviour : MonoBehaviour
{
	private PlayerControl myPlayer;

	private FloatRange voteTime = new FloatRange(3f, 8f);

	private bool voted;

	public void Start()
	{
		myPlayer = GetComponent<PlayerControl>();
	}

	public void Update()
	{
		if (myPlayer.Data.IsDead)
		{
			return;
		}
		if ((bool)MeetingHud.Instance)
		{
			if (!voted)
			{
				voted = true;
				StartCoroutine(DoVote());
			}
		}
		else
		{
			voted = false;
		}
	}

	private IEnumerator DoVote()
	{
		yield return new WaitForSeconds(voteTime.Next());
		sbyte suspectIdx = -1;
		for (int i = 0; i < 100 && i != 99; i++)
		{
			int num = IntRange.Next(-1, GameData.Instance.PlayerCount);
			if (num >= 0)
			{
				GameData.PlayerInfo playerInfo = GameData.Instance.AllPlayers[num];
				if (!playerInfo.IsDead)
				{
					suspectIdx = (sbyte)playerInfo.PlayerId;
					break;
				}
				continue;
			}
			suspectIdx = (sbyte)num;
			break;
		}
		MeetingHud.Instance.CmdCastVote(myPlayer.PlayerId, suspectIdx);
	}
}
