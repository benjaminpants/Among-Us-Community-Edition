using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

// DONT ADD ANY NEW VARIABLES TO THIS CLASS, IT RESULTS IN AN INSTANT CRASH.
public class HudManager : DestroyableSingleton<HudManager>
{
	public MeetingHud MeetingPrefab;

	public KillButtonManager KillButton;

	public UseButtonManager UseButton;

	public ReportButtonManager ReportButton;

	public TextRenderer GameSettings;

	public GameObject TaskStuff;

	public ChatController Chat;

	public DialogueBox Dialogue;

	public TextRenderer TaskText;

	public Transform TaskCompleteOverlay;

	private float taskDirtyTimer;

	public MeshRenderer ShadowQuad;

	public SpriteRenderer FullScreen;

	public SpriteRenderer MapButton;

	public KillOverlay KillOverlay;

	public IVirtualJoystick joystick;

	public MonoBehaviour[] Joysticks;

	public DiscussBehaviour discussEmblem;

	public ShhhBehaviour shhhEmblem;

	public IntroCutscene IntroPrefab;

	public OptionsMenuBehaviour GameMenu;

	public NotificationPopper Notifier;

	public RoomTracker roomTracker;

	public AudioClip SabotageSound;

	public AudioClip TaskCompleteSound;

	public AudioClip TaskUpdateSound;

	private StringBuilder tasksString = new StringBuilder();

	private bool custombuttontestingon = false;

	private CustomButtonManager[] CustomButtons = new CustomButtonManager[3];

	public Coroutine ReactorFlash
	{
		get;
		set;
	}

	public Coroutine OxyFlash
	{
		get;
		set;
	}




	public void Start()
	{
		if (custombuttontestingon)
		{
			for (int i = 0; i < 3; i++)
			{
				GameObject kill = GameObject.Instantiate<GameObject>(KillButton.gameObject);
				kill.transform.parent = KillButton.transform.parent;
				KillButtonManager kbm = kill.GetComponent<KillButtonManager>();
				Destroy(kbm);
				CustomButtons[i] = kill.AddComponent<CustomButtonManager>();
				CustomButtons[i].renderer = CustomButtons[i].GetComponent<SpriteRenderer>();
				CustomButtons[i].TimerText = CustomButtons[i].GetComponentInChildren<TextRenderer>();
				kill.SetActive(true);
				kill.transform.position = new Vector3(-4.7f + (((i - (i == 0 || i == 1 ? 1 : 0) + 1) % 3) * 1.1f), ((i + 1) % 3 == 0 ? 1.1f : 0) + -2.3f, -29.0f);
			}
		}
        
		SetTouchType(SaveManager.TouchConfig);
	}

	public void ShowTaskComplete()
	{
		StartCoroutine(CoTaskComplete());
	}

	private IEnumerator CoTaskComplete()
	{
		if (Constants.ShouldPlaySfx())
		{
			SoundManager.Instance.PlaySound(TaskCompleteSound, loop: false);
		}
		TaskCompleteOverlay.gameObject.SetActive(value: true);
		yield return Effects.Slide2D(TaskCompleteOverlay, new Vector2(0f, -8f), Vector2.zero, 0.25f);
		for (float time = 0f; time < 0.75f; time += Time.deltaTime)
		{
			yield return null;
		}
		yield return Effects.Slide2D(TaskCompleteOverlay, Vector2.zero, new Vector2(0f, 8f), 0.25f);
		TaskCompleteOverlay.gameObject.SetActive(value: false);
	}

	public void SetJoystickSize(float size)
	{
		if (joystick != null && joystick is VirtualJoystick)
		{
			VirtualJoystick obj = (VirtualJoystick)joystick;
			obj.transform.localScale = new Vector3(size, size, 1f);
			AspectPosition component = obj.GetComponent<AspectPosition>();
			float num = Mathf.Lerp(0.65f, 1.1f, FloatRange.ReverseLerp(size, 0.5f, 1.5f));
			component.DistanceFromEdge = new Vector3(num, num, -10f);
			component.AdjustPosition();
		}
	}

	public void SetTouchType(int type)
	{
		if (joystick != null && !(joystick is KeyboardJoystick))
		{
			UnityEngine.Object.Destroy((joystick as MonoBehaviour).gameObject);
		}
		MonoBehaviour monoBehaviour = UnityEngine.Object.Instantiate(Joysticks[type + 1]);
		monoBehaviour.transform.SetParent(base.transform, worldPositionStays: false);
		joystick = monoBehaviour.GetComponent<IVirtualJoystick>();
	}

	public void OpenMap()
	{
		ShowMap(delegate(MapBehaviour m)
		{
			m.ShowNormalMap();
		});
	}

	public void ShowMap(Action<MapBehaviour> mapAction)
	{
		if (!MapBehaviour.Instance)
		{
			MapBehaviour.Instance = UnityEngine.Object.Instantiate(ShipStatus.Instance.MapPrefab, base.transform);
			MapBehaviour.Instance.gameObject.SetActive(value: false);
		}
		mapAction(MapBehaviour.Instance);
	}

	public void SetHudActive(bool isActive)
	{
		DestroyableSingleton<HudManager>.Instance.UseButton.gameObject.SetActive(isActive);
		DestroyableSingleton<HudManager>.Instance.UseButton.Refresh();
		DestroyableSingleton<HudManager>.Instance.ReportButton.gameObject.SetActive(isActive);
		GameData.PlayerInfo data = PlayerControl.LocalPlayer.Data;
		DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(isActive && (data.IsImpostor || CE_RoleManager.GetRoleFromID(data.role).CanDo(CE_Specials.Kill)) && !data.IsDead);
		DestroyableSingleton<HudManager>.Instance.TaskText.transform.parent.gameObject.SetActive(isActive);
	}

	public void FixedUpdate()
	{
		if (custombuttontestingon)
		{
			for (int i = 0; i < 3; i++)
			{
				CustomButtons[i].SetCoolDown(i + 1, i + 1);
				CustomButtons[i].SetActivateState(true);
			}
		}
		taskDirtyTimer += Time.fixedDeltaTime;
		if (!(taskDirtyTimer > 0.25f))
		{
			return;
		}
		taskDirtyTimer = 0f;
		if (!PlayerControl.LocalPlayer)
		{
			TaskText.Text = string.Empty;
			return;
		}
		GameData.PlayerInfo data = PlayerControl.LocalPlayer.Data;
		if (data == null)
		{
			return;
		}
		bool isImpostor = data.IsImpostor;
		tasksString.Clear();
		if (PlayerControl.LocalPlayer.myTasks.Count == 0)
		{
			tasksString.Append("None");
		}
		else
		{
			for (int i = 0; i < PlayerControl.LocalPlayer.myTasks.Count; i++)
			{
				PlayerTask playerTask = PlayerControl.LocalPlayer.myTasks[i];
				if (playerTask.TaskType == TaskTypes.FixComms && !isImpostor)
				{
					tasksString.Clear();
					playerTask.AppendTaskText(tasksString);
					break;
				}
				playerTask.AppendTaskText(tasksString);
			}
			tasksString.TrimEnd();
		}
		TaskText.Text = tasksString.ToString();
	}

	public IEnumerator ShowEmblem(bool shhh)
	{
		if (shhh)
		{
			shhhEmblem.gameObject.SetActive(value: true);
			yield return shhhEmblem.PlayAnimation();
			shhhEmblem.gameObject.SetActive(value: false);
		}
		else
		{
			discussEmblem.gameObject.SetActive(value: true);
			yield return discussEmblem.PlayAnimation();
			discussEmblem.gameObject.SetActive(value: false);
		}
	}

	public void StartReactorFlash()
	{
		if (ReactorFlash == null)
		{
			ReactorFlash = StartCoroutine(CoReactorFlash());
		}
	}

	public void StartOxyFlash()
	{
		if (OxyFlash == null)
		{
			OxyFlash = StartCoroutine(CoReactorFlash());
		}
	}

	public void ShowPopUp(string text)
	{
		Dialogue.Show(text);
	}

	public void StopReactorFlash()
	{
		if (ReactorFlash != null)
		{
			StopCoroutine(ReactorFlash);
			FullScreen.enabled = false;
			ReactorFlash = null;
		}
	}

	public void StopOxyFlash()
	{
		if (OxyFlash != null)
		{
			StopCoroutine(OxyFlash);
			FullScreen.enabled = false;
			OxyFlash = null;
		}
	}

	public IEnumerator CoFadeFullScreen(Color source, Color target, float duration = 0.2f)
	{
		if (!FullScreen.enabled || !(FullScreen.color == target))
		{
			FullScreen.enabled = true;
			for (float t = 0f; t < duration; t += Time.deltaTime)
			{
				FullScreen.color = Color.Lerp(source, target, t / duration);
				yield return null;
			}
			FullScreen.color = target;
			if (target.a < 0.05f)
			{
				FullScreen.enabled = false;
			}
		}
	}

	private IEnumerator CoReactorFlash()
	{
		WaitForSeconds wait = new WaitForSeconds(1f);
		FullScreen.color = new Color(1f, 0f, 0f, 19f / 51f);
		while (true)
		{
			FullScreen.enabled = !FullScreen.enabled;
			SoundManager.Instance.PlaySound(SabotageSound, loop: false);
			yield return wait;
		}
	}

	public IEnumerator ForceShowIntro(List<PlayerControl> yourTeam, bool IsImpostor)
	{
		DestroyableSingleton<HudManager>.Instance.FullScreen.transform.localPosition = new Vector3(0f, 0f, -250f);
		yield return DestroyableSingleton<HudManager>.Instance.ShowEmblem(shhh: true);
		IntroCutscene introCutscene = UnityEngine.Object.Instantiate(IntroPrefab, base.transform);
		yield return introCutscene.CoBegin(yourTeam, IsImpostor);
		yield return CoFadeFullScreen(Color.black, Color.clear);
		DestroyableSingleton<HudManager>.Instance.FullScreen.transform.localPosition = new Vector3(0f, 0f, -500f);
	}

	public IEnumerator CoShowIntro(List<PlayerControl> yourTeam)
	{
		DestroyableSingleton<HudManager>.Instance.FullScreen.transform.localPosition = new Vector3(0f, 0f, -250f);
		yield return DestroyableSingleton<HudManager>.Instance.ShowEmblem(shhh: true);
		IntroCutscene introCutscene = UnityEngine.Object.Instantiate(IntroPrefab, base.transform);
		yield return introCutscene.CoBegin(yourTeam, PlayerControl.LocalPlayer.Data.IsImpostor);
		yield return CoFadeFullScreen(Color.black, Color.clear);
		DestroyableSingleton<HudManager>.Instance.FullScreen.transform.localPosition = new Vector3(0f, 0f, -500f);
	}

	public void OpenMeetingRoom(PlayerControl reporter)
	{
		if (!MeetingHud.Instance)
		{
			Debug.Log("Opening meeting room: " + reporter);
			ShipStatus.Instance.RepairSystem(SystemTypes.Reactor, PlayerControl.LocalPlayer, 16);
			ShipStatus.Instance.RepairSystem(SystemTypes.LifeSupp, PlayerControl.LocalPlayer, 16);
			MeetingHud.Instance = UnityEngine.Object.Instantiate(MeetingPrefab);
			MeetingHud.Instance.ServerStart(reporter.PlayerId);
			AmongUsClient.Instance.Spawn(MeetingHud.Instance);
		}
	}

	public void OpenInfectedMap()
	{
		DestroyableSingleton<HudManager>.Instance.ShowMap(delegate(MapBehaviour m)
		{
			m.ShowInfectedMap();
		});
	}


	public void Update()
	{
		try
		{
			if (SaveManager.EnableProHUDMode)
			{
				float num = 30f;
				Vector3 position = Camera.current.ScreenToWorldPoint(new Vector3((float)Camera.current.pixelWidth - 40f, 40f, 0f));
				DestroyableSingleton<HudManager>.Instance.UseButton.gameObject.transform.position = position;
				DestroyableSingleton<HudManager>.Instance.UseButton.gameObject.transform.localScale = new Vector3(0.45f, 0.45f, 0f);
				Vector3 position2 = Camera.current.ScreenToWorldPoint(new Vector3((float)Camera.current.pixelWidth - 40f, 80f + num, 0f));
				DestroyableSingleton<HudManager>.Instance.ReportButton.gameObject.transform.position = position2;
				DestroyableSingleton<HudManager>.Instance.ReportButton.gameObject.transform.localScale = new Vector3(0.45f, 0.45f, 0f);
				Vector3 position3 = Camera.current.ScreenToWorldPoint(new Vector3((float)Camera.current.pixelWidth - 40f, 120f + num * 2f, 0f));
				DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.transform.position = position3;
				DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.transform.localScale = new Vector3(0.45f, 0.45f, 0f);
			}
		}
		catch (Exception)
		{
		}
	}
}
