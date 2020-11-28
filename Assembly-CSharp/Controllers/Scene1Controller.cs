using System;
using System.Collections;
using UnityEngine;

// Token: 0x020000B0 RID: 176
public class Scene1Controller : SceneController
{
	// Token: 0x060003B4 RID: 948 RVA: 0x00018798 File Offset: 0x00016998
	public void OnDrawGizmos()
	{
		for (int i = 0; i < this.WayPoints.Length; i++)
		{
			Vector2 v = this.WayPoints[i];
			Vector2 v2 = this.WayPoints[(i + 1) % this.WayPoints.Length];
			Gizmos.DrawLine(v, v2);
		}
	}

	// Token: 0x060003B5 RID: 949 RVA: 0x0000462E File Offset: 0x0000282E
	public void OnEnable()
	{
		this.backupCam.cullingMask = 0;
		base.StartCoroutine(this.RunPlayer(0));
		base.StartCoroutine(this.RunPlayer(1));
	}

	// Token: 0x060003B6 RID: 950 RVA: 0x00004658 File Offset: 0x00002858
	public void OnDisable()
	{
		this.backupCam.cullingMask = (int.MaxValue ^ LayerMask.GetMask(new string[]
		{
			"UI"
		}));
	}

	// Token: 0x060003B7 RID: 951 RVA: 0x0000467E File Offset: 0x0000287E
	private IEnumerator RunPlayer(int idx)
	{
		PlayerAnimator myPlayer = this.players[idx];
		for (;;)
		{
			int num;
			for (int i = 0; i < this.WayPoints.Length; i = num)
			{
				bool willInterrupt = i == 2 || i == 5;
				yield return myPlayer.WalkPlayerTo(this.WayPoints[i], willInterrupt, 0.1f);
				if (willInterrupt)
				{
					yield return this.DoUse(idx, (i == 2) ? 0 : 1);
				}
				num = i + 1;
			}
		}
		yield break;
	}

	// Token: 0x060003B8 RID: 952 RVA: 0x00004694 File Offset: 0x00002894
	private IEnumerator DoUse(int idx, int consoleid)
	{
		PlayerAnimator myPlayer = this.players[idx];
		yield return Scene1Controller.WaitForSeconds(0.2f);
		if (idx == 0)
		{
			yield return myPlayer.finger.MoveTo(myPlayer.UseButton.transform.position, 0.75f);
		}
		else
		{
			yield return myPlayer.finger.MoveTo(this.Consoles[consoleid].transform.position, 0.75f);
		}
		yield return Scene1Controller.WaitForSeconds(0.2f);
		yield return myPlayer.finger.DoClick(0.4f);
		yield return Scene1Controller.WaitForSeconds(0.2f);
		if (!(myPlayer.joystick is DemoKeyboardStick))
		{
			yield return myPlayer.finger.MoveTo(myPlayer.joystick.transform.position, 0.75f);
		}
		else
		{
			yield return Scene1Controller.WaitForSeconds(0.75f);
		}
		yield break;
	}

	// Token: 0x060003B9 RID: 953 RVA: 0x000046B1 File Offset: 0x000028B1
	public static IEnumerator WaitForSeconds(float duration)
	{
		for (float time = 0f; time < duration; time += Time.deltaTime)
		{
			yield return null;
		}
		yield break;
	}

	// Token: 0x0400039F RID: 927
	public PlayerAnimator[] players;

	// Token: 0x040003A0 RID: 928
	public DummyConsole[] Consoles;

	// Token: 0x040003A1 RID: 929
	public Vector2[] WayPoints;

	// Token: 0x040003A2 RID: 930
	public Camera backupCam;
}
