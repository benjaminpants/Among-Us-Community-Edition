using System;
using System.Collections;
using System.Text;
using UnityEngine;

// Token: 0x02000160 RID: 352
public class MedScanMinigame : Minigame
{
	// Token: 0x06000745 RID: 1861 RVA: 0x00029D44 File Offset: 0x00027F44
	public override void Begin(PlayerTask task)
	{
		base.Begin(task);
		this.medscan = (ShipStatus.Instance.Systems[SystemTypes.MedBay] as MedScanSystem);
		this.gauge.Value = 0f;
		base.transform.position = new Vector3(100f, 0f, 0f);
		GameData.PlayerInfo data = PlayerControl.LocalPlayer.Data;
		int playerId = (int)data.PlayerId;
		int colorId = (int)data.ColorId;
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("ID: ");
		stringBuilder.Append(MedScanMinigame.ColorNames[colorId].Substring(0, 3).ToUpperInvariant());
		stringBuilder.Append("P" + playerId);
		stringBuilder.Append(new string(' ', 8));
		stringBuilder.Append("HT: 3' 6\"");
		stringBuilder.Append(new string(' ', 8));
		stringBuilder.Append("WT: 92lb");
		stringBuilder.AppendLine();
		stringBuilder.Append("C: ");
		stringBuilder.Append(MedScanMinigame.ColorNames[colorId].PadRight(17));
		stringBuilder.Append("BT: ");
		stringBuilder.Append(MedScanMinigame.BloodTypes[playerId * 3 % MedScanMinigame.BloodTypes.Length]);
		this.completeString = stringBuilder.ToString();
		this.charStats.Text = string.Empty;
		ShipStatus.Instance.RpcRepairSystem(SystemTypes.MedBay, playerId | 128);
		this.walking = base.StartCoroutine(this.WalkToOffset());
	}

	// Token: 0x06000746 RID: 1862 RVA: 0x000068A8 File Offset: 0x00004AA8
	private IEnumerator WalkToOffset()
	{
		this.state = MedScanMinigame.PositionState.WalkingToOffset;
		PlayerPhysics component = PlayerControl.LocalPlayer.GetComponent<PlayerPhysics>();
		Vector2 vector = ShipStatus.Instance.MedScanner.transform.position;
		Vector2 a = Vector2.left.Rotate((float)(PlayerControl.LocalPlayer.PlayerId * 36));
		vector += a / 2f;
		Camera.main.GetComponent<FollowerCamera>().Locked = false;
		yield return component.WalkPlayerTo(vector, 0.001f);
		yield return new WaitForSeconds(0.1f);
		Camera.main.GetComponent<FollowerCamera>().Locked = true;
		this.walking = null;
		yield break;
	}

	// Token: 0x06000747 RID: 1863 RVA: 0x000068B7 File Offset: 0x00004AB7
	private IEnumerator WalkToPad()
	{
		this.state = MedScanMinigame.PositionState.WalkingToPad;
		PlayerPhysics component = PlayerControl.LocalPlayer.GetComponent<PlayerPhysics>();
		Vector2 worldPos = ShipStatus.Instance.MedScanner.transform.position;
		worldPos.x += 0.14f;
		worldPos.y += 0.1f;
		Camera.main.GetComponent<FollowerCamera>().Locked = false;
		yield return component.WalkPlayerTo(worldPos, 0.001f);
		yield return new WaitForSeconds(0.1f);
		Camera.main.GetComponent<FollowerCamera>().Locked = true;
		this.walking = null;
		yield break;
	}

	// Token: 0x06000748 RID: 1864 RVA: 0x00029EC0 File Offset: 0x000280C0
	private void FixedUpdate()
	{
		if (this.MyNormTask.IsComplete)
		{
			return;
		}
		byte playerId = PlayerControl.LocalPlayer.PlayerId;
		if (this.medscan.CurrentUser != playerId)
		{
			if (this.medscan.CurrentUser == 255)
			{
				this.text.Text = "Scan requested";
				return;
			}
			GameData.PlayerInfo playerById = GameData.Instance.GetPlayerById(this.medscan.CurrentUser);
			this.text.Text = "Waiting for " + playerById.PlayerName;
			return;
		}
		else
		{
			if (this.state != MedScanMinigame.PositionState.WalkingToPad)
			{
				if (this.walking != null)
				{
					base.StopCoroutine(this.walking);
				}
				this.walking = base.StartCoroutine(this.WalkToPad());
				return;
			}
			if (this.walking != null)
			{
				return;
			}
			if (this.ScanTimer == 0f)
			{
				PlayerControl.LocalPlayer.RpcSetScanner(true);
				SoundManager.Instance.PlaySound(this.ScanSound, false, 1f);
			}
			this.ScanTimer += Time.fixedDeltaTime;
			this.gauge.Value = this.ScanTimer / this.ScanDuration;
			int num = (int)(Mathf.Min(1f, this.ScanTimer / this.ScanDuration * 1.25f) * (float)this.completeString.Length);
			if (num > this.charStats.Text.Length)
			{
				this.charStats.Text = this.completeString.Substring(0, num);
				if (this.completeString[num - 1] != ' ')
				{
					SoundManager.Instance.PlaySoundImmediate(this.TextSound, false, 0.7f, 0.3f);
				}
			}
			if (this.ScanTimer >= this.ScanDuration)
			{
				PlayerControl.LocalPlayer.RpcSetScanner(false);
				this.text.Text = "Scan complete";
				this.MyNormTask.NextStep();
				ShipStatus.Instance.RpcRepairSystem(SystemTypes.MedBay, (int)(playerId | 64));
				base.StartCoroutine(base.CoStartClose(0.75f));
				return;
			}
			this.text.Text = "Scan complete in: " + (int)(this.ScanDuration - this.ScanTimer);
			return;
		}
	}

	// Token: 0x06000749 RID: 1865 RVA: 0x0002A0E0 File Offset: 0x000282E0
	public override void Close()
	{
		base.StopAllCoroutines();
		byte playerId = PlayerControl.LocalPlayer.PlayerId;
		SoundManager.Instance.StopSound(this.TextSound);
		SoundManager.Instance.StopSound(this.ScanSound);
		PlayerControl.LocalPlayer.RpcSetScanner(false);
		ShipStatus.Instance.RpcRepairSystem(SystemTypes.MedBay, (int)(playerId | 64));
		Camera.main.GetComponent<FollowerCamera>().Locked = false;
		base.Close();
	}

	// Token: 0x0400071B RID: 1819
	private static readonly string[] ColorNames = new string[]
	{
		"Red",
		"Blue",
		"Green",
		"Pink",
		"Orange",
		"Yellow",
		"Black",
		"White",
		"Purple",
		"Brown",
		"Cyan",
		"Lime",
		"Tan",
		"Baby Red",
		"Blurple",
		"Kiwi",
		"Bronze",
		"Maroon",
		"Navy",
		"Light Purple",
		"Gold",
		"Impostor",
		"Shaderless"
	};

	// Token: 0x0400071C RID: 1820
	private static readonly string[] BloodTypes = new string[]
	{
		"O-",
		"A-",
		"B-",
		"AB-",
		"O+",
		"A+",
		"B+",
		"AB+"
	};

	// Token: 0x0400071D RID: 1821
	public TextRenderer text;

	// Token: 0x0400071E RID: 1822
	public TextRenderer charStats;

	// Token: 0x0400071F RID: 1823
	public HorizontalGauge gauge;

	// Token: 0x04000720 RID: 1824
	private MedScanSystem medscan;

	// Token: 0x04000721 RID: 1825
	public float ScanDuration = 10f;

	// Token: 0x04000722 RID: 1826
	public float ScanTimer;

	// Token: 0x04000723 RID: 1827
	private string completeString;

	// Token: 0x04000724 RID: 1828
	public AudioClip ScanSound;

	// Token: 0x04000725 RID: 1829
	public AudioClip TextSound;

	// Token: 0x04000726 RID: 1830
	private Coroutine walking;

	// Token: 0x04000727 RID: 1831
	private MedScanMinigame.PositionState state;

	// Token: 0x02000161 RID: 353
	private enum PositionState
	{
		// Token: 0x04000729 RID: 1833
		None,
		// Token: 0x0400072A RID: 1834
		WalkingToPad,
		// Token: 0x0400072B RID: 1835
		WalkingToOffset
	}
}
