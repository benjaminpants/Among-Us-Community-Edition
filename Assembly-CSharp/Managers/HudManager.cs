using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using InnerNet;
using UnityEngine;

// Token: 0x020000C2 RID: 194
public class HudManager : DestroyableSingleton<HudManager>
{
	// Token: 0x170000A9 RID: 169
	// (get) Token: 0x06000411 RID: 1041 RVA: 0x00004A1B File Offset: 0x00002C1B
	// (set) Token: 0x06000412 RID: 1042 RVA: 0x00004A23 File Offset: 0x00002C23
	public Coroutine ReactorFlash { get; set; }

	// Token: 0x170000AA RID: 170
	// (get) Token: 0x06000413 RID: 1043 RVA: 0x00004A2C File Offset: 0x00002C2C
	// (set) Token: 0x06000414 RID: 1044 RVA: 0x00004A34 File Offset: 0x00002C34
	public Coroutine OxyFlash { get; set; }

	// Token: 0x06000415 RID: 1045 RVA: 0x00004A3D File Offset: 0x00002C3D
	public void Start()
	{
		this.SetTouchType(SaveManager.TouchConfig);
	}

	// Token: 0x06000416 RID: 1046 RVA: 0x00004A4A File Offset: 0x00002C4A
	public void ShowTaskComplete()
	{
		base.StartCoroutine(this.CoTaskComplete());
	}

	// Token: 0x06000417 RID: 1047 RVA: 0x00004A59 File Offset: 0x00002C59
	private IEnumerator CoTaskComplete()
	{
		if (Constants.ShouldPlaySfx())
		{
			SoundManager.Instance.PlaySound(this.TaskCompleteSound, false, 1f);
		}
		this.TaskCompleteOverlay.gameObject.SetActive(true);
		yield return Effects.Slide2D(this.TaskCompleteOverlay, new Vector2(0f, -8f), Vector2.zero, 0.25f);
		for (float time = 0f; time < 0.75f; time += Time.deltaTime)
		{
			yield return null;
		}
		yield return Effects.Slide2D(this.TaskCompleteOverlay, Vector2.zero, new Vector2(0f, 8f), 0.25f);
		this.TaskCompleteOverlay.gameObject.SetActive(false);
		yield break;
	}

	// Token: 0x06000418 RID: 1048 RVA: 0x00019E3C File Offset: 0x0001803C
	public void SetJoystickSize(float size)
	{
		if (this.joystick != null && this.joystick is VirtualJoystick)
		{
			VirtualJoystick virtualJoystick = (VirtualJoystick)this.joystick;
			virtualJoystick.transform.localScale = new Vector3(size, size, 1f);
			AspectPosition component = virtualJoystick.GetComponent<AspectPosition>();
			float num = Mathf.Lerp(0.65f, 1.1f, FloatRange.ReverseLerp(size, 0.5f, 1.5f));
			component.DistanceFromEdge = new Vector3(num, num, -10f);
			component.AdjustPosition();
		}
	}

	// Token: 0x06000419 RID: 1049 RVA: 0x00019EBC File Offset: 0x000180BC
	public void SetTouchType(int type)
	{
		if (this.joystick != null && !(this.joystick is KeyboardJoystick))
		{
			UnityEngine.Object.Destroy((this.joystick as MonoBehaviour).gameObject);
		}
		MonoBehaviour monoBehaviour = UnityEngine.Object.Instantiate<MonoBehaviour>(this.Joysticks[type + 1]);
		monoBehaviour.transform.SetParent(base.transform, false);
		this.joystick = monoBehaviour.GetComponent<IVirtualJoystick>();
	}

	// Token: 0x0600041A RID: 1050 RVA: 0x00004A68 File Offset: 0x00002C68
	public void OpenMap()
	{
		this.ShowMap(delegate(MapBehaviour m)
		{
			m.ShowNormalMap();
		});
	}

	// Token: 0x0600041B RID: 1051 RVA: 0x00019F24 File Offset: 0x00018124
	public void ShowMap(Action<MapBehaviour> mapAction)
	{
		if (!MapBehaviour.Instance)
		{
			MapBehaviour.Instance = UnityEngine.Object.Instantiate<MapBehaviour>(ShipStatus.Instance.MapPrefab, base.transform);
			MapBehaviour.Instance.gameObject.SetActive(false);
		}
		mapAction(MapBehaviour.Instance);
	}

	// Token: 0x0600041C RID: 1052 RVA: 0x00019F74 File Offset: 0x00018174
	public void SetHudActive(bool isActive)
	{
		DestroyableSingleton<HudManager>.Instance.UseButton.gameObject.SetActive(isActive);
		DestroyableSingleton<HudManager>.Instance.UseButton.Refresh();
		DestroyableSingleton<HudManager>.Instance.ReportButton.gameObject.SetActive(isActive);
		GameData.PlayerInfo data = PlayerControl.LocalPlayer.Data;
		DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(isActive && (data.IsImpostor || data.role == GameData.PlayerInfo.Role.Sheriff) && !data.IsDead);
		DestroyableSingleton<HudManager>.Instance.TaskText.transform.parent.gameObject.SetActive(isActive);
	}

	// Token: 0x0600041D RID: 1053 RVA: 0x0001A018 File Offset: 0x00018218
	public void FixedUpdate()
	{
		this.taskDirtyTimer += Time.fixedDeltaTime;
		if (this.taskDirtyTimer > 0.25f)
		{
			this.taskDirtyTimer = 0f;
			if (!PlayerControl.LocalPlayer)
			{
				this.TaskText.Text = string.Empty;
				return;
			}
			GameData.PlayerInfo data = PlayerControl.LocalPlayer.Data;
			if (data == null)
			{
				return;
			}
			bool isImpostor = data.IsImpostor;
			this.tasksString.Clear();
			if (PlayerControl.LocalPlayer.myTasks.Count == 0)
			{
				this.tasksString.Append("None");
			}
			else
			{
				for (int i = 0; i < PlayerControl.LocalPlayer.myTasks.Count; i++)
				{
					PlayerTask playerTask = PlayerControl.LocalPlayer.myTasks[i];
					if (playerTask.TaskType == TaskTypes.FixComms && !isImpostor)
					{
						this.tasksString.Clear();
						playerTask.AppendTaskText(this.tasksString);
						break;
					}
					playerTask.AppendTaskText(this.tasksString);
				}
				this.tasksString.TrimEnd();
			}
			this.TaskText.Text = this.tasksString.ToString();
		}
	}

	// Token: 0x0600041E RID: 1054 RVA: 0x00004A8F File Offset: 0x00002C8F
	public IEnumerator ShowEmblem(bool shhh)
	{
		if (shhh)
		{
			this.shhhEmblem.gameObject.SetActive(true);
			yield return this.shhhEmblem.PlayAnimation();
			this.shhhEmblem.gameObject.SetActive(false);
		}
		else
		{
			this.discussEmblem.gameObject.SetActive(true);
			yield return this.discussEmblem.PlayAnimation();
			this.discussEmblem.gameObject.SetActive(false);
		}
		yield break;
	}

	// Token: 0x0600041F RID: 1055 RVA: 0x00004AA5 File Offset: 0x00002CA5
	public void StartReactorFlash()
	{
		if (this.ReactorFlash == null)
		{
			this.ReactorFlash = base.StartCoroutine(this.CoReactorFlash());
		}
	}

	// Token: 0x06000420 RID: 1056 RVA: 0x00004AC1 File Offset: 0x00002CC1
	public void StartOxyFlash()
	{
		if (this.OxyFlash == null)
		{
			this.OxyFlash = base.StartCoroutine(this.CoReactorFlash());
		}
	}

	// Token: 0x06000421 RID: 1057 RVA: 0x00004ADD File Offset: 0x00002CDD
	public void ShowPopUp(string text)
	{
		this.Dialogue.Show(text);
	}

	// Token: 0x06000422 RID: 1058 RVA: 0x00004AEB File Offset: 0x00002CEB
	public void StopReactorFlash()
	{
		if (this.ReactorFlash != null)
		{
			base.StopCoroutine(this.ReactorFlash);
			this.FullScreen.enabled = false;
			this.ReactorFlash = null;
		}
	}

	// Token: 0x06000423 RID: 1059 RVA: 0x00004B14 File Offset: 0x00002D14
	public void StopOxyFlash()
	{
		if (this.OxyFlash != null)
		{
			base.StopCoroutine(this.OxyFlash);
			this.FullScreen.enabled = false;
			this.OxyFlash = null;
		}
	}

	// Token: 0x06000424 RID: 1060 RVA: 0x00004B3D File Offset: 0x00002D3D
	public IEnumerator CoFadeFullScreen(Color source, Color target, float duration = 0.2f)
	{
		if (this.FullScreen.enabled && this.FullScreen.color == target)
		{
			yield break;
		}
		this.FullScreen.enabled = true;
		for (float t = 0f; t < duration; t += Time.deltaTime)
		{
			this.FullScreen.color = Color.Lerp(source, target, t / duration);
			yield return null;
		}
		this.FullScreen.color = target;
		if (target.a < 0.05f)
		{
			this.FullScreen.enabled = false;
		}
		yield break;
	}

	// Token: 0x06000425 RID: 1061 RVA: 0x00004B61 File Offset: 0x00002D61
	private IEnumerator CoReactorFlash()
	{
		WaitForSeconds wait = new WaitForSeconds(1f);
		this.FullScreen.color = new Color(1f, 0f, 0f, 0.372549027f);
		for (;;)
		{
			this.FullScreen.enabled = !this.FullScreen.enabled;
			SoundManager.Instance.PlaySound(this.SabotageSound, false, 1f);
			yield return wait;
		}
		yield break;
	}

	// Token: 0x06000426 RID: 1062 RVA: 0x00004B70 File Offset: 0x00002D70
	public IEnumerator CoShowIntro(List<PlayerControl> yourTeam)
	{
		DestroyableSingleton<HudManager>.Instance.FullScreen.transform.localPosition = new Vector3(0f, 0f, -250f);
		yield return DestroyableSingleton<HudManager>.Instance.ShowEmblem(true);
		IntroCutscene introCutscene = UnityEngine.Object.Instantiate<IntroCutscene>(this.IntroPrefab, base.transform);
		yield return introCutscene.CoBegin(yourTeam, PlayerControl.LocalPlayer.Data.IsImpostor);
		yield return this.CoFadeFullScreen(Color.black, Color.clear, 0.2f);
		DestroyableSingleton<HudManager>.Instance.FullScreen.transform.localPosition = new Vector3(0f, 0f, -500f);
		yield break;
	}

	// Token: 0x06000427 RID: 1063 RVA: 0x0001A134 File Offset: 0x00018334
	public void OpenMeetingRoom(PlayerControl reporter)
	{
		if (MeetingHud.Instance)
		{
			return;
		}
		Debug.Log("Opening meeting room: " + reporter);
		ShipStatus.Instance.RepairSystem(SystemTypes.Reactor, PlayerControl.LocalPlayer, 16);
		ShipStatus.Instance.RepairSystem(SystemTypes.LifeSupp, PlayerControl.LocalPlayer, 16);
		MeetingHud.Instance = UnityEngine.Object.Instantiate<MeetingHud>(this.MeetingPrefab);
		MeetingHud.Instance.ServerStart(reporter.PlayerId);
		AmongUsClient.Instance.Spawn(MeetingHud.Instance, -2, SpawnFlags.None);
	}

	// Token: 0x04000405 RID: 1029
	public MeetingHud MeetingPrefab;

	// Token: 0x04000406 RID: 1030
	public KillButtonManager KillButton;

	// Token: 0x04000407 RID: 1031
	public UseButtonManager UseButton;

	// Token: 0x04000408 RID: 1032
	public ReportButtonManager ReportButton;

	// Token: 0x04000409 RID: 1033
	public TextRenderer GameSettings;

	// Token: 0x0400040A RID: 1034
	public GameObject TaskStuff;

	// Token: 0x0400040B RID: 1035
	public ChatController Chat;

	// Token: 0x0400040C RID: 1036
	public DialogueBox Dialogue;

	// Token: 0x0400040D RID: 1037
	public TextRenderer TaskText;

	// Token: 0x0400040E RID: 1038
	public Transform TaskCompleteOverlay;

	// Token: 0x0400040F RID: 1039
	private float taskDirtyTimer;

	// Token: 0x04000410 RID: 1040
	public MeshRenderer ShadowQuad;

	// Token: 0x04000411 RID: 1041
	public SpriteRenderer FullScreen;

	// Token: 0x04000414 RID: 1044
	public SpriteRenderer MapButton;

	// Token: 0x04000415 RID: 1045
	public KillOverlay KillOverlay;

	// Token: 0x04000416 RID: 1046
	public IVirtualJoystick joystick;

	// Token: 0x04000417 RID: 1047
	public MonoBehaviour[] Joysticks;

	// Token: 0x04000418 RID: 1048
	public DiscussBehaviour discussEmblem;

	// Token: 0x04000419 RID: 1049
	public ShhhBehaviour shhhEmblem;

	// Token: 0x0400041A RID: 1050
	public IntroCutscene IntroPrefab;

	// Token: 0x0400041B RID: 1051
	public OptionsMenuBehaviour GameMenu;

	// Token: 0x0400041C RID: 1052
	public NotificationPopper Notifier;

	// Token: 0x0400041D RID: 1053
	public RoomTracker roomTracker;

	// Token: 0x0400041E RID: 1054
	public AudioClip SabotageSound;

	// Token: 0x0400041F RID: 1055
	public AudioClip TaskCompleteSound;

	// Token: 0x04000420 RID: 1056
	public AudioClip TaskUpdateSound;

	// Token: 0x04000421 RID: 1057
	private StringBuilder tasksString = new StringBuilder();
}
