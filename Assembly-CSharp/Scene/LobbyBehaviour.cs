using Hazel;
using InnerNet;
using UnityEngine;
using System.IO;

public class LobbyBehaviour : InnerNetObject
{
	public static LobbyBehaviour Instance;

	public AudioClip SpawnSound;

	public AnimationClip SpawnInClip;

	public Vector2[] SpawnPositions;

	public AudioClip DropShipSound;

	public ShipRoom[] AllRooms;

	private float timer;

	public void Start()
	{
		Instance = this;
		SoundManager.Instance.StopAllSound();
		//SoundManager.Instance.PlaySound(CE_WavUtility.ToAudioClip(Path.Combine(Application.dataPath, "CE_Assets", "Audio", "LobbyJazz", "A Night Alone.wav")), loop: true).pitch = 1f;
		SoundManager.Instance.PlaySound(DropShipSound, loop: true).pitch = 1.2f;
		Camera main = Camera.main;
		if ((bool)main)
		{
			FollowerCamera component = main.GetComponent<FollowerCamera>();
			if ((bool)component && SaveManager.LobbyShake)
			{
				component.shakeAmount = 0.03f;
				component.shakePeriod = 400f;
			}
		}
	}

	public void FixedUpdate()
	{
		timer += Time.deltaTime;
		if (!(timer < 0.25f))
		{
			timer = 0f;
			if (PlayerControl.GameOptions != null)
			{
                int numPlayers = (GameData.Instance ? GameData.Instance.PlayerCount : 20);
                DestroyableSingleton<HudManager>.Instance.GameSettings.Text = PlayerControl.GameOptions.ToHudString(numPlayers);
				int numlines = DestroyableSingleton<HudManager>.Instance.GameSettings.Text.Split('\n').Length;
				DestroyableSingleton<HudManager>.Instance.GameSettings.scale = 0.45f - (0.01f * (numlines - 43));
				DestroyableSingleton<HudManager>.Instance.GameSettings.gameObject.SetActive(value: true);
			}
		}
	}

	public override void OnDestroy()
	{
		Camera main = Camera.main;
		if ((bool)main)
		{
			FollowerCamera component = main.GetComponent<FollowerCamera>();
			if ((bool)component && SaveManager.LobbyShake)
			{
				component.shakeAmount = 0.02f;
				component.shakePeriod = 0.3f;
			}
		}
		base.OnDestroy();
	}

	public override void HandleRpc(byte callId, MessageReader reader)
	{
	}

	public override bool Serialize(MessageWriter writer, bool initialState)
	{
		return false;
	}

	public override void Deserialize(MessageReader reader, bool initialState)
	{
	}
}
