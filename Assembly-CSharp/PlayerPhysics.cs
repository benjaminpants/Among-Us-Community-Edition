using System;
using System.Collections;
using System.Linq;
using Assets.CoreScripts;
using Hazel;
using InnerNet;
using PowerTools;
using UnityEngine;

// Token: 0x02000196 RID: 406
public class PlayerPhysics : InnerNetObject
{
	// Token: 0x1700013E RID: 318
	// (get) Token: 0x0600088C RID: 2188 RVA: 0x000073A9 File Offset: 0x000055A9
	public float TrueSpeed
	{
		get
		{
			return this.Speed * PlayerControl.GameOptions.PlayerSpeedMod;
		}
	}

	// Token: 0x1700013F RID: 319
	// (get) Token: 0x0600088D RID: 2189 RVA: 0x000073BC File Offset: 0x000055BC
	public float TrueGhostSpeed
	{
		get
		{
			return this.GhostSpeed * PlayerControl.GameOptions.PlayerSpeedMod;
		}
	}

	// Token: 0x0600088E RID: 2190 RVA: 0x000073CF File Offset: 0x000055CF
	public void Awake()
	{
		this.body = base.GetComponent<Rigidbody2D>();
		this.Animator = base.GetComponent<SpriteAnim>();
		this.rend = base.GetComponent<SpriteRenderer>();
		this.myPlayer = base.GetComponent<PlayerControl>();
	}

	// Token: 0x0600088F RID: 2191 RVA: 0x0002ECC0 File Offset: 0x0002CEC0
	private void FixedUpdate()
	{
		this.HandleAnimation();
		if (base.AmOwner && this.myPlayer.CanMove && GameData.Instance)
		{
			DestroyableSingleton<Telemetry>.Instance.WritePosition(this.myPlayer.PlayerId, base.transform.position);
			GameData.PlayerInfo data = this.myPlayer.Data;
			if (data == null)
			{
				return;
			}
			bool isDead = data.IsDead;
			this.body.velocity = DestroyableSingleton<HudManager>.Instance.joystick.Delta * (isDead ? this.TrueGhostSpeed : this.TrueSpeed);
		}
	}

	// Token: 0x06000890 RID: 2192 RVA: 0x0002ED64 File Offset: 0x0002CF64
	private void LateUpdate()
	{
		Vector3 position = base.transform.position;
		position.z = position.y / 1000f;
		base.transform.position = position;
	}

	// Token: 0x06000891 RID: 2193 RVA: 0x00007401 File Offset: 0x00005601
	public Vector3 Vec2ToPosition(Vector2 pos)
	{
		return new Vector3(pos.x, pos.y, pos.y / 1000f);
	}

	// Token: 0x06000892 RID: 2194 RVA: 0x00007420 File Offset: 0x00005620
	public void SetSkin(uint skinId)
	{
		this.Skin.SetSkin(skinId);
		if (this.Animator.IsPlaying(this.SpawnAnim))
		{
			this.Skin.SetSpawn(this.Animator.Time);
		}
	}

	// Token: 0x06000893 RID: 2195 RVA: 0x0002ED9C File Offset: 0x0002CF9C
	public void ResetAnim(bool stopCoroutines = true)
	{
		if (stopCoroutines)
		{
			this.myPlayer.StopAllCoroutines();
			base.StopAllCoroutines();
		}
		this.myPlayer.inVent = false;
		this.myPlayer.Visible = true;
		GameData.PlayerInfo data = this.myPlayer.Data;
		if (data == null || !data.IsDead)
		{
			this.Skin.SetIdle();
			this.Animator.Play(this.IdleAnim, 1f);
			this.myPlayer.Visible = true;
			this.myPlayer.SetHatAlpha(1f);
			return;
		}
		this.Skin.SetGhost();
		this.Animator.Play(this.GhostIdleAnim, 1f);
		this.myPlayer.SetHatAlpha(0.5f);
	}

	// Token: 0x06000894 RID: 2196 RVA: 0x0002EE5C File Offset: 0x0002D05C
	private void HandleAnimation()
	{
		if (this.Animator.IsPlaying(this.SpawnAnim))
		{
			return;
		}
		if (!GameData.Instance)
		{
			return;
		}
		Vector2 velocity = this.body.velocity;
		AnimationClip currentAnimation = this.Animator.GetCurrentAnimation();
		GameData.PlayerInfo data = this.myPlayer.Data;
		if (data == null)
		{
			return;
		}
		if (!data.IsDead)
		{
			if (velocity.sqrMagnitude >= 0.05f)
			{
				if (currentAnimation != this.RunAnim)
				{
					this.Animator.Play(this.RunAnim, 1f);
					this.Skin.SetRun();
				}
				if (velocity.x < -0.01f)
				{
					this.rend.flipX = true;
				}
				else if (velocity.x > 0.01f)
				{
					this.rend.flipX = false;
				}
			}
			else if (currentAnimation == this.RunAnim || currentAnimation == this.SpawnAnim || !currentAnimation)
			{
				this.Skin.SetIdle();
				this.Animator.Play(this.IdleAnim, 1f);
				this.myPlayer.SetHatAlpha(1f);
			}
		}
		else
		{
			this.Skin.SetGhost();
			if (currentAnimation != this.GhostIdleAnim)
			{
				this.Animator.Play(this.GhostIdleAnim, 1f);
				this.myPlayer.SetHatAlpha(0.5f);
			}
			if (velocity.x < -0.01f)
			{
				this.rend.flipX = true;
			}
			else if (velocity.x > 0.01f)
			{
				this.rend.flipX = false;
			}
		}
		this.Skin.Flipped = this.rend.flipX;
	}

	// Token: 0x06000895 RID: 2197 RVA: 0x00007457 File Offset: 0x00005657
	public IEnumerator CoSpawnPlayer(LobbyBehaviour lobby)
	{
		if (!lobby)
		{
			yield break;
		}
		Vector3 spawnPos = this.Vec2ToPosition(lobby.SpawnPositions[(int)this.myPlayer.PlayerId % lobby.SpawnPositions.Length]);
		this.myPlayer.nameText.gameObject.SetActive(false);
		this.myPlayer.Collider.enabled = false;
		KillAnimation.SetMovement(this.myPlayer, false);
		bool amFlipped = this.myPlayer.PlayerId > 4;
		this.myPlayer.GetComponent<SpriteRenderer>().flipX = amFlipped;
		this.myPlayer.transform.position = spawnPos;
		SoundManager.Instance.PlaySound(lobby.SpawnSound, false, 1f).volume = 0.8f;
		this.Skin.SetSpawn(0f);
		this.Skin.Flipped = this.rend.flipX;
		yield return new WaitForAnimationFinish(this.Animator, this.SpawnAnim);
		base.enabled = true;
		base.transform.position = spawnPos + new Vector3(amFlipped ? -0.3f : 0.3f, -0.24f);
		this.ResetAnim(false);
		Vector2 b = (-spawnPos).normalized;
		yield return this.WalkPlayerTo((Vector2)spawnPos + b, 0.01f);
		this.myPlayer.Collider.enabled = true;
		KillAnimation.SetMovement(this.myPlayer, true);
		this.myPlayer.nameText.gameObject.SetActive(true);
		yield break;
	}

	// Token: 0x06000896 RID: 2198 RVA: 0x0002F01C File Offset: 0x0002D21C
	public void ExitAllVents()
	{
		this.ResetAnim(true);
		this.myPlayer.moveable = true;
		Vent[] allVents = ShipStatus.Instance.AllVents;
		for (int i = 0; i < allVents.Length; i++)
		{
			allVents[i].SetButtons(false);
		}
	}

	// Token: 0x06000897 RID: 2199 RVA: 0x0000746D File Offset: 0x0000566D
	private IEnumerator CoEnterVent(int id)
	{
		Vent vent = ShipStatus.Instance.AllVents.First((Vent v) => v.Id == id);
		this.myPlayer.moveable = false;
		yield return this.WalkPlayerTo(vent.transform.position, 0.01f);
		vent.EnterVent();
		this.Skin.SetEnterVent();
		yield return new WaitForAnimationFinish(this.Animator, this.EnterVentAnim);
		this.Skin.SetIdle();
		this.Animator.Play(this.IdleAnim, 1f);
		this.myPlayer.Visible = false;
		this.myPlayer.inVent = true;
		yield break;
	}

	// Token: 0x06000898 RID: 2200 RVA: 0x00007483 File Offset: 0x00005683
	private IEnumerator CoExitVent(int id)
	{
		Vent vent = ShipStatus.Instance.AllVents.First((Vent v) => v.Id == id);
		this.myPlayer.Visible = true;
		this.myPlayer.inVent = false;
		vent.ExitVent();
		this.Skin.SetExitVent();
		yield return new WaitForAnimationFinish(this.Animator, this.ExitVentAnim);
		this.Skin.SetIdle();
		this.Animator.Play(this.IdleAnim, 1f);
		this.myPlayer.moveable = true;
		yield break;
	}

	// Token: 0x06000899 RID: 2201 RVA: 0x00007499 File Offset: 0x00005699
	public IEnumerator WalkPlayerTo(Vector2 worldPos, float tolerance = 0.01f)
	{
		worldPos -= base.GetComponent<CircleCollider2D>().offset;
		Rigidbody2D body = this.body;
		do
		{
			Vector2 vector2;
			Vector2 vector = vector2 = worldPos - (Vector2)base.transform.position;
			if (vector2.sqrMagnitude <= tolerance)
			{
				break;
			}
			float d = Mathf.Clamp(vector.magnitude * 2f, 0.01f, 1f);
			body.velocity = vector.normalized * this.Speed * d;
			yield return null;
		}
		while (body.velocity.magnitude >= 0.0001f);
		body.velocity = Vector2.zero;
		yield break;
	}

	// Token: 0x0600089A RID: 2202 RVA: 0x00002723 File Offset: 0x00000923
	public override bool Serialize(MessageWriter writer, bool initialState)
	{
		return false;
	}

	// Token: 0x0600089B RID: 2203 RVA: 0x00002265 File Offset: 0x00000465
	public override void Deserialize(MessageReader reader, bool initialState)
	{
	}

	// Token: 0x0600089C RID: 2204 RVA: 0x000074B6 File Offset: 0x000056B6
	public void RpcEnterVent(int id)
	{
		if (AmongUsClient.Instance.AmClient)
		{
			base.StartCoroutine(this.CoEnterVent(id));
		}
		MessageWriter messageWriter = AmongUsClient.Instance.StartRpc(this.NetId, 0, SendOption.Reliable);
		messageWriter.WritePacked(id);
		messageWriter.EndMessage();
	}

	// Token: 0x0600089D RID: 2205 RVA: 0x000074F0 File Offset: 0x000056F0
	public void RpcExitVent(int id)
	{
		if (AmongUsClient.Instance.AmClient)
		{
			base.StartCoroutine(this.CoExitVent(id));
		}
		MessageWriter messageWriter = AmongUsClient.Instance.StartRpc(this.NetId, 1, SendOption.Reliable);
		messageWriter.WritePacked(id);
		messageWriter.EndMessage();
	}

	// Token: 0x0600089E RID: 2206 RVA: 0x0002F060 File Offset: 0x0002D260
	public override void HandleRpc(byte callId, MessageReader reader)
	{
		if (callId == 0)
		{
			int id = reader.ReadPackedInt32();
			base.StartCoroutine(this.CoEnterVent(id));
			return;
		}
		if (callId != 1)
		{
			return;
		}
		int id2 = reader.ReadPackedInt32();
		base.StartCoroutine(this.CoExitVent(id2));
	}

	// Token: 0x0400084A RID: 2122
	public float Speed = 4.5f;

	// Token: 0x0400084B RID: 2123
	public float GhostSpeed = 3f;

	// Token: 0x0400084C RID: 2124
	[HideInInspector]
	private Rigidbody2D body;

	// Token: 0x0400084D RID: 2125
	[HideInInspector]
	private SpriteAnim Animator;

	// Token: 0x0400084E RID: 2126
	[HideInInspector]
	private SpriteRenderer rend;

	// Token: 0x0400084F RID: 2127
	[HideInInspector]
	private PlayerControl myPlayer;

	// Token: 0x04000850 RID: 2128
	public AnimationClip RunAnim;

	// Token: 0x04000851 RID: 2129
	public AnimationClip IdleAnim;

	// Token: 0x04000852 RID: 2130
	public AnimationClip GhostIdleAnim;

	// Token: 0x04000853 RID: 2131
	public AnimationClip EnterVentAnim;

	// Token: 0x04000854 RID: 2132
	public AnimationClip ExitVentAnim;

	// Token: 0x04000855 RID: 2133
	public AnimationClip SpawnAnim;

	// Token: 0x04000856 RID: 2134
	public SkinLayer Skin;

	// Token: 0x02000197 RID: 407
	private enum RpcCalls
	{
		// Token: 0x04000858 RID: 2136
		EnterVent,
		// Token: 0x04000859 RID: 2137
		ExitVent
	}
}
