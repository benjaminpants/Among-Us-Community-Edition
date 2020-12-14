using System.Collections;
using System.Linq;
using Assets.CoreScripts;
using Hazel;
using InnerNet;
using PowerTools;
using UnityEngine;

public class PlayerPhysics : InnerNetObject
{
	private enum RpcCalls
	{
		EnterVent,
		ExitVent
	}

	public float Speed = 4.5f;

	public float GhostSpeed = 3f;

	[HideInInspector]
	private Rigidbody2D body;

	[HideInInspector]
	private SpriteAnim Animator;

	[HideInInspector]
	private SpriteRenderer rend;

	[HideInInspector]
	private PlayerControl myPlayer;

	public AnimationClip RunAnim;

	public AnimationClip IdleAnim;

	public AnimationClip GhostIdleAnim;

	public AnimationClip EnterVentAnim;

	public AnimationClip ExitVentAnim;

	public AnimationClip SpawnAnim;

	public SkinLayer Skin;

	public float TrueSpeed => Speed * PlayerControl.GameOptions.PlayerSpeedMod;

	public float TrueGhostSpeed => GhostSpeed * PlayerControl.GameOptions.PlayerSpeedMod;

	public void Awake()
	{
		body = GetComponent<Rigidbody2D>();
		Animator = GetComponent<SpriteAnim>();
		rend = GetComponent<SpriteRenderer>();
		myPlayer = GetComponent<PlayerControl>();
	}

	private void FixedUpdate()
	{
		HandleAnimation();
		if (base.AmOwner && myPlayer.CanMove && (bool)GameData.Instance)
		{
			DestroyableSingleton<Telemetry>.Instance.WritePosition(myPlayer.PlayerId, base.transform.position);
			GameData.PlayerInfo data = myPlayer.Data;
			if (data != null)
			{
				bool isDead = data.IsDead;
				body.velocity = DestroyableSingleton<HudManager>.Instance.joystick.Delta * (isDead ? TrueGhostSpeed : TrueSpeed);
			}
		}
	}

	private void LateUpdate()
	{
		Vector3 position = base.transform.position;
		position.z = position.y / 1000f;
		base.transform.position = position;
	}

	public Vector3 Vec2ToPosition(Vector2 pos)
	{
		return new Vector3(pos.x, pos.y, pos.y / 1000f);
	}

	public void SetSkin(uint skinId)
	{
		Skin.SetSkin(skinId);
		if (Animator.IsPlaying(SpawnAnim))
		{
			Skin.SetSpawn(Animator.Time);
		}
	}

	public void ResetAnim(bool stopCoroutines = true)
	{
		if (stopCoroutines)
		{
			myPlayer.StopAllCoroutines();
			StopAllCoroutines();
		}
		myPlayer.inVent = false;
		myPlayer.Visible = true;
		GameData.PlayerInfo data = myPlayer.Data;
		if (data == null || !data.IsDead)
		{
			Skin.SetIdle();
			Animator.Play(IdleAnim);
			myPlayer.Visible = true;
			myPlayer.SetHatAlpha(1f);
		}
		else
		{
			Skin.SetGhost();
			Animator.Play(GhostIdleAnim);
			myPlayer.SetHatAlpha(0.5f);
		}
	}

	private void HandleAnimation()
	{
		if (CE_WardrobeLoader.AnimationDebugMode)
		{
			HandleAnimationTesting();
		}
		else
		{
			if (Animator.IsPlaying(SpawnAnim) || !GameData.Instance)
			{
				return;
			}
			Vector2 velocity = body.velocity;
			AnimationClip currentAnimation = Animator.GetCurrentAnimation();
			GameData.PlayerInfo data = myPlayer.Data;
			if (data == null)
			{
				return;
			}
			if (!data.IsDead)
			{
				if (velocity.sqrMagnitude >= 0.05f)
				{
					if (currentAnimation != RunAnim)
					{
						Animator.Play(RunAnim, CE_WardrobeLoader.AnimationDebugMode ? CE_WardrobeLoader.TestPlaybackSpeed : 1f);
						Skin.SetRun();
					}
					if (velocity.x < -0.01f)
					{
						rend.flipX = true;
					}
					else if (velocity.x > 0.01f)
					{
						rend.flipX = false;
					}
				}
				else if (currentAnimation == RunAnim || currentAnimation == SpawnAnim || !currentAnimation)
				{
					Skin.SetIdle();
					Animator.Play(IdleAnim, CE_WardrobeLoader.AnimationDebugMode ? CE_WardrobeLoader.TestPlaybackSpeed : 1f);
					myPlayer.SetHatAlpha(1f);
				}
			}
			else
			{
				Skin.SetGhost();
				if (currentAnimation != GhostIdleAnim)
				{
					Animator.Play(GhostIdleAnim, CE_WardrobeLoader.AnimationDebugMode ? CE_WardrobeLoader.TestPlaybackSpeed : 1f);
					myPlayer.SetHatAlpha(0.5f);
				}
				if (velocity.x < -0.01f)
				{
					rend.flipX = true;
				}
				else if (velocity.x > 0.01f)
				{
					rend.flipX = false;
				}
			}
			Skin.Flipped = rend.flipX;
		}
	}

	public IEnumerator CoSpawnPlayer(LobbyBehaviour lobby)
	{
		if ((bool)lobby)
		{
			Vector3 spawnPos = Vec2ToPosition(lobby.SpawnPositions[(int)myPlayer.PlayerId % lobby.SpawnPositions.Length]);
			myPlayer.nameText.gameObject.SetActive(value: false);
			myPlayer.Collider.enabled = false;
			KillAnimation.SetMovement(myPlayer, canMove: false);
			bool amFlipped = myPlayer.PlayerId > 4;
			myPlayer.GetComponent<SpriteRenderer>().flipX = amFlipped;
			myPlayer.transform.position = spawnPos;
			SoundManager.Instance.PlaySound(lobby.SpawnSound, loop: false).volume = 0.8f;
			Skin.SetSpawn();
			Skin.Flipped = rend.flipX;
			yield return new WaitForAnimationFinish(Animator, SpawnAnim);
			base.enabled = true;
			base.transform.position = spawnPos + new Vector3(amFlipped ? (-0.3f) : 0.3f, -0.24f);
			ResetAnim(stopCoroutines: false);
			Vector2 b = (-spawnPos).normalized;
			Vector2 a = spawnPos;
			yield return WalkPlayerTo(a + b);
			myPlayer.Collider.enabled = true;
			KillAnimation.SetMovement(myPlayer, canMove: true);
			myPlayer.nameText.gameObject.SetActive(value: true);
		}
	}

	public void ExitAllVents()
	{
		ResetAnim();
		myPlayer.moveable = true;
		Vent[] allVents = ShipStatus.Instance.AllVents;
		for (int i = 0; i < allVents.Length; i++)
		{
			allVents[i].SetButtons(enabled: false);
		}
	}

	private IEnumerator CoEnterVent(int id)
	{
		Vent vent = ShipStatus.Instance.AllVents.First((Vent v) => v.Id == id);
		myPlayer.moveable = false;
		yield return WalkPlayerTo(vent.transform.position);
		vent.EnterVent();
		Skin.SetEnterVent();
		yield return new WaitForAnimationFinish(Animator, EnterVentAnim);
		Skin.SetIdle();
		Animator.Play(IdleAnim);
		myPlayer.Visible = false;
		myPlayer.inVent = true;
	}

	private IEnumerator CoExitVent(int id)
	{
		Vent vent = ShipStatus.Instance.AllVents.First((Vent v) => v.Id == id);
		myPlayer.Visible = true;
		myPlayer.inVent = false;
		vent.ExitVent();
		Skin.SetExitVent();
		yield return new WaitForAnimationFinish(Animator, ExitVentAnim);
		Skin.SetIdle();
		Animator.Play(IdleAnim);
		myPlayer.moveable = true;
	}

	public IEnumerator WalkPlayerTo(Vector2 worldPos, float tolerance = 0.01f)
	{
		worldPos -= GetComponent<CircleCollider2D>().offset;
		Rigidbody2D body = this.body;
		do
		{
			Vector2 b = base.transform.position;
			Vector2 vector = worldPos - b;
			if (vector.sqrMagnitude <= tolerance)
			{
				break;
			}
			float d = Mathf.Clamp(vector.magnitude * 2f, 0.01f, 1f);
			body.velocity = vector.normalized * Speed * d;
			yield return null;
		}
		while (body.velocity.magnitude >= 0.0001f);
		body.velocity = Vector2.zero;
	}

	public override bool Serialize(MessageWriter writer, bool initialState)
	{
		return false;
	}

	public override void Deserialize(MessageReader reader, bool initialState)
	{
	}

	public void RpcEnterVent(int id)
	{
		if (AmongUsClient.Instance.AmClient)
		{
			StartCoroutine(CoEnterVent(id));
		}
		MessageWriter messageWriter = AmongUsClient.Instance.StartRpc(NetId, 0);
		messageWriter.WritePacked(id);
		messageWriter.EndMessage();
	}

	public void RpcExitVent(int id)
	{
		if (AmongUsClient.Instance.AmClient)
		{
			StartCoroutine(CoExitVent(id));
		}
		MessageWriter messageWriter = AmongUsClient.Instance.StartRpc(NetId, 1);
		messageWriter.WritePacked(id);
		messageWriter.EndMessage();
	}

	public override void HandleRpc(byte callId, MessageReader reader)
	{
		switch (callId)
		{
		case 0:
		{
			int id2 = reader.ReadPackedInt32();
			StartCoroutine(CoEnterVent(id2));
			break;
		}
		case 1:
		{
			int id = reader.ReadPackedInt32();
			StartCoroutine(CoExitVent(id));
			break;
		}
		}
	}

	private void HandleAnimationTesting()
	{
		if ((Animator.GetCurrentAnimation() != RunAnim || CE_WardrobeLoader.TestPlaybackResetAnimations) && CE_WardrobeLoader.TestPlaybackMode == 0)
		{
			Animator.Play(RunAnim, CE_WardrobeLoader.AnimationDebugMode ? CE_WardrobeLoader.TestPlaybackSpeed : 1f);
			Skin.SetRun();
		}
		if ((Animator.GetCurrentAnimation() != SpawnAnim || CE_WardrobeLoader.TestPlaybackResetAnimations) && CE_WardrobeLoader.TestPlaybackMode == 1)
		{
			Animator.Play(SpawnAnim, CE_WardrobeLoader.AnimationDebugMode ? CE_WardrobeLoader.TestPlaybackSpeed : 1f);
			Skin.SetSpawn();
		}
		if ((Animator.GetCurrentAnimation() != IdleAnim || CE_WardrobeLoader.TestPlaybackResetAnimations) && CE_WardrobeLoader.TestPlaybackMode == 2)
		{
			Animator.Play(IdleAnim, CE_WardrobeLoader.AnimationDebugMode ? CE_WardrobeLoader.TestPlaybackSpeed : 1f);
			Skin.SetIdle();
		}
		if ((Animator.GetCurrentAnimation() != EnterVentAnim || CE_WardrobeLoader.TestPlaybackResetAnimations) && CE_WardrobeLoader.TestPlaybackMode == 3)
		{
			Animator.Play(EnterVentAnim, CE_WardrobeLoader.AnimationDebugMode ? CE_WardrobeLoader.TestPlaybackSpeed : 1f);
			Skin.SetEnterVent();
		}
		if ((Animator.GetCurrentAnimation() != ExitVentAnim || CE_WardrobeLoader.TestPlaybackResetAnimations) && CE_WardrobeLoader.TestPlaybackMode == 4)
		{
			Animator.Play(ExitVentAnim, CE_WardrobeLoader.AnimationDebugMode ? CE_WardrobeLoader.TestPlaybackSpeed : 1f);
			Skin.SetExitVent();
		}
		if ((Animator.GetCurrentAnimation() != GhostIdleAnim || CE_WardrobeLoader.TestPlaybackResetAnimations) && CE_WardrobeLoader.TestPlaybackMode == 5)
		{
			Animator.Play(GhostIdleAnim, CE_WardrobeLoader.AnimationDebugMode ? CE_WardrobeLoader.TestPlaybackSpeed : 1f);
			Skin.SetGhost();
		}
		if (CE_WardrobeLoader.TestPlaybackPause)
		{
			if (CE_WardrobeLoader.TestPlaybackResetAnimations)
			{
				Animator.Time = CE_WardrobeLoader.TestPlaybackPausePosition;
				Skin.animator.Time = CE_WardrobeLoader.TestPlaybackPausePositionSkin;
			}
			else
			{
				CE_WardrobeLoader.TestPlaybackPausePosition = Animator.Time;
				CE_WardrobeLoader.TestPlaybackPausePositionSkin = Skin.animator.Time;
			}
			Skin.animator.Pause();
			Animator.Pause();
		}
		else if (Animator.IsPaused())
		{
			Animator.Resume();
			Skin.animator.Resume();
			CE_WardrobeLoader.TestPlaybackPausePosition = -1f;
			CE_WardrobeLoader.TestPlaybackPausePositionSkin = -1f;
		}
		if (CE_WardrobeLoader.TestPlaybackResetAnimations)
		{
			CE_WardrobeLoader.TestPlaybackResetAnimations = false;
		}
		CE_WardrobeLoader.TestPlaybackCurrentPosition = Animator.Time;
		CE_WardrobeLoader.TestPlaybackCurrentPositionSkin = Skin.animator.Time;
	}

	public SpriteRenderer GetHatRender()
	{
		return rend;
	}
}