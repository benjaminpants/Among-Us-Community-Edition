using System;
using Hazel;
using InnerNet;
using UnityEngine;

// Token: 0x02000148 RID: 328
public class LobbyBehaviour : InnerNetObject
{
	// Token: 0x060006D9 RID: 1753 RVA: 0x000284D8 File Offset: 0x000266D8
	public void Start()
	{
		LobbyBehaviour.Instance = this;
		SoundManager.Instance.StopAllSound();
		SoundManager.Instance.PlaySound(this.DropShipSound, true, 1f).pitch = 1.2f;
		Camera main = Camera.main;
		if (main)
		{
			FollowerCamera component = main.GetComponent<FollowerCamera>();
			if (component)
			{
				component.shakeAmount = 0.03f;
				component.shakePeriod = 400f;
			}
		}
	}

	// Token: 0x060006DA RID: 1754 RVA: 0x00028548 File Offset: 0x00026748
	public void FixedUpdate()
	{
		this.timer += Time.deltaTime;
		if (this.timer < 0.25f)
		{
			return;
		}
		this.timer = 0f;
		if (PlayerControl.GameOptions != null)
		{
			int numPlayers = GameData.Instance != null ? GameData.Instance.PlayerCount : 20;
			DestroyableSingleton<HudManager>.Instance.GameSettings.Text = PlayerControl.GameOptions.ToHudString(numPlayers);
			DestroyableSingleton<HudManager>.Instance.GameSettings.gameObject.SetActive(true);
		}
	}

	// Token: 0x060006DB RID: 1755 RVA: 0x000285D4 File Offset: 0x000267D4
	public override void OnDestroy()
	{
		Camera main = Camera.main;
		if (main)
		{
			FollowerCamera component = main.GetComponent<FollowerCamera>();
			if (component)
			{
				component.shakeAmount = 0.02f;
				component.shakePeriod = 0.3f;
			}
		}
		base.OnDestroy();
	}

	// Token: 0x060006DC RID: 1756 RVA: 0x00002265 File Offset: 0x00000465
	public override void HandleRpc(byte callId, MessageReader reader)
	{
	}

	// Token: 0x060006DD RID: 1757 RVA: 0x00002723 File Offset: 0x00000923
	public override bool Serialize(MessageWriter writer, bool initialState)
	{
		return false;
	}

	// Token: 0x060006DE RID: 1758 RVA: 0x00002265 File Offset: 0x00000465
	public override void Deserialize(MessageReader reader, bool initialState)
	{
	}

	// Token: 0x0400069F RID: 1695
	public static LobbyBehaviour Instance;

	// Token: 0x040006A0 RID: 1696
	public AudioClip SpawnSound;

	// Token: 0x040006A1 RID: 1697
	public AnimationClip SpawnInClip;

	// Token: 0x040006A2 RID: 1698
	public Vector2[] SpawnPositions;

	// Token: 0x040006A3 RID: 1699
	public AudioClip DropShipSound;

	// Token: 0x040006A4 RID: 1700
	public ShipRoom[] AllRooms;

	// Token: 0x040006A5 RID: 1701
	private float timer;
}
