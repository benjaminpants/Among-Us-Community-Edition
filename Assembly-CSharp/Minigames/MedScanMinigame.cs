using System.Collections;
using System.Text;
using UnityEngine;

public class MedScanMinigame : Minigame
{
	private enum PositionState
	{
		None,
		WalkingToPad,
		WalkingToOffset
	}

	private static readonly string[] ColorNames;

	private static readonly string[] BloodTypes;

	public TextRenderer text;

	public TextRenderer charStats;

	public HorizontalGauge gauge;

	private MedScanSystem medscan;

	public float ScanDuration = 10f;

	public float ScanTimer;

	private string completeString;

	public AudioClip ScanSound;

	public AudioClip TextSound;

	private Coroutine walking;

	private PositionState state;

	public override void Begin(PlayerTask task)
	{
		base.Begin(task);
		ScanDuration *= GameOptionsData.TaskDifficultyMult[PlayerControl.GameOptions.TaskDifficulty];
		medscan = ShipStatus.Instance.Systems[SystemTypes.MedBay] as MedScanSystem;
		gauge.Value = 0f;
		base.transform.position = new Vector3(100f, 0f, 0f);
		GameData.PlayerInfo data = PlayerControl.LocalPlayer.Data;
		int playerId = data.PlayerId;
		int colorId = data.ColorId;
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("ID: ");
		stringBuilder.Append(ColorNames[colorId].Substring(0, 3).ToUpperInvariant());
		stringBuilder.Append("P" + playerId);
		stringBuilder.Append(new string(' ', 8));
		stringBuilder.Append("HT: 3' 6\"");
		stringBuilder.Append(new string(' ', 8));
		stringBuilder.Append("WT: 92lb");
		stringBuilder.AppendLine();
		stringBuilder.Append("C: ");
		stringBuilder.Append(ColorNames[colorId].PadRight(17));
		stringBuilder.Append("BT: ");
		stringBuilder.Append(BloodTypes[playerId * 3 % BloodTypes.Length]);
		completeString = stringBuilder.ToString();
		charStats.Text = string.Empty;
		ShipStatus.Instance.RpcRepairSystem(SystemTypes.MedBay, playerId | 0x80);
		walking = StartCoroutine(WalkToOffset());
	}

	private IEnumerator WalkToOffset()
	{
		state = PositionState.WalkingToOffset;
		PlayerPhysics component = PlayerControl.LocalPlayer.GetComponent<PlayerPhysics>();
		Vector2 worldPos = ShipStatus.Instance.MedScanner.transform.position;
		Vector2 a = Vector2.left.Rotate(PlayerControl.LocalPlayer.PlayerId * 36);
		worldPos += a / 2f;
		Camera.main.GetComponent<FollowerCamera>().Locked = false;
		yield return component.WalkPlayerTo(worldPos, 0.001f);
		yield return new WaitForSeconds(0.1f);
		Camera.main.GetComponent<FollowerCamera>().Locked = true;
		walking = null;
	}

	private IEnumerator WalkToPad()
	{
		state = PositionState.WalkingToPad;
		PlayerPhysics component = PlayerControl.LocalPlayer.GetComponent<PlayerPhysics>();
		Vector2 worldPos = ShipStatus.Instance.MedScanner.transform.position;
		worldPos.x += 0.14f;
		worldPos.y += 0.1f;
		Camera.main.GetComponent<FollowerCamera>().Locked = false;
		yield return component.WalkPlayerTo(worldPos, 0.001f);
		yield return new WaitForSeconds(0.1f);
		Camera.main.GetComponent<FollowerCamera>().Locked = true;
		walking = null;
	}

	private void FixedUpdate()
	{
		if (MyNormTask.IsComplete)
		{
			return;
		}
		byte playerId = PlayerControl.LocalPlayer.PlayerId;
		if (medscan.CurrentUser != playerId)
		{
			if (medscan.CurrentUser == byte.MaxValue)
			{
				text.Text = "Scan requested";
				return;
			}
			GameData.PlayerInfo playerById = GameData.Instance.GetPlayerById(medscan.CurrentUser);
			text.Text = "Waiting for " + playerById.PlayerName;
		}
		else if (state != PositionState.WalkingToPad)
		{
			if (walking != null)
			{
				StopCoroutine(walking);
			}
			walking = StartCoroutine(WalkToPad());
		}
		else
		{
			if (walking != null)
			{
				return;
			}
			if (ScanTimer == 0f)
			{
				PlayerControl.LocalPlayer.RpcSetScanner(value: true);
				SoundManager.Instance.PlaySound(ScanSound, loop: false);
			}
			ScanTimer += Time.fixedDeltaTime;
			gauge.Value = ScanTimer / ScanDuration;
			int num = (int)(Mathf.Min(1f, ScanTimer / ScanDuration * 1.25f) * (float)completeString.Length);
			if (num > charStats.Text.Length)
			{
				charStats.Text = completeString.Substring(0, num);
				if (completeString[num - 1] != ' ')
				{
					SoundManager.Instance.PlaySoundImmediate(TextSound, loop: false, 0.7f, 0.3f);
				}
			}
			if (ScanTimer >= ScanDuration)
			{
				PlayerControl.LocalPlayer.RpcSetScanner(value: false);
				text.Text = "Scan complete";
				MyNormTask.NextStep();
				ShipStatus.Instance.RpcRepairSystem(SystemTypes.MedBay, playerId | 0x40);
				StartCoroutine(CoStartClose());
			}
			else
			{
				text.Text = "Scan complete in: " + (int)(ScanDuration - ScanTimer);
			}
		}
	}

	public override void Close()
	{
		StopAllCoroutines();
		byte playerId = PlayerControl.LocalPlayer.PlayerId;
		SoundManager.Instance.StopSound(TextSound);
		SoundManager.Instance.StopSound(ScanSound);
		PlayerControl.LocalPlayer.RpcSetScanner(value: false);
		ShipStatus.Instance.RpcRepairSystem(SystemTypes.MedBay, playerId | 0x40);
		Camera.main.GetComponent<FollowerCamera>().Locked = false;
		base.Close();
	}

	static MedScanMinigame()
	{
		ColorNames = new string[]
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
			"Flesh",
			"Baby Red",
			"Blurple",
			"Kiwi",
			"Bronze",
			"Maroon",
			"Navy",
			"Light Purple",
			"Gold",
			"Impostor",
			"Shaderless",
			"Jed",
			"Tan",
			"Doodle",
			"Jlue",
			"Jreen"
		};
		BloodTypes = new string[8]
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
	}
}
