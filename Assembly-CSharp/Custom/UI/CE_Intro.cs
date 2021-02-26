using System.IO;
using UnityEngine;
using System.Collections;
public class CE_Intro : MonoBehaviour
{

	#region Variables (Globals)

	private Texture2D CarJemLogo;

	private Texture2D MTMLogo;

	private Texture2D FramesLogo;

	private static bool inPause = false;

	private bool SkipActive;

	private static int LogoVariant = 2;

	private static CE_Intro instance;

	private static WaitForSecondsRealtimeSkipable CurrentWait;

	private static bool _IsShown = false;
	public static bool IsShown
	{
		get
		{
			return _IsShown;

		}
		set
		{
			_IsShown = value;
			if (_IsShown == true) Startup();
		}
	}

	#endregion

	#region Methods (Globals)

	private static void Startup()
	{
		instance.StartCoroutine(instance.ExtendedIntroCo());
	}

	private void OnEnable()
	{
		if (instance == null)
		{
			instance = this;
		}
	}
	private void Update()
	{
	}
	private void OnGUI()
	{
		if (IsShown && LogoVariant == 1) Intro_Version1();
		if (IsShown && LogoVariant == 2) Intro_Version2();
	}
	private IEnumerator ExtendedIntroCo()
	{
		while (!inPause)
		{
			yield return null;
		}
		yield return CurrentWait = new WaitForSecondsRealtimeSkipable(2);
		if (LogoVariant == 1) ExtendedIntroCoExit_Version1();
		else if (LogoVariant == 2) ExtendedIntroCoExit_Version2();

	}
	public static void Init()
	{
		if (!(instance != null))
		{
			GameObject gameObject = new GameObject();
			gameObject.AddComponent<CE_Intro>();
			Object.DontDestroyOnLoad(gameObject);
		}
	}
	private static GUIStyle WindowStyle()
	{
		return new GUIStyle();
	}
	private void LoadAssets()
	{
		if (!CarJemLogo)
		{
			CarJemLogo = CE_TextureNSpriteExtensions.LoadPNG(Path.Combine(Application.dataPath, "CE_Assets", "Textures", "CJLogo.png"),false);
		}
        if (!MTMLogo)
        {
            MTMLogo = CE_TextureNSpriteExtensions.LoadPNG(Path.Combine(Application.dataPath, "CE_Assets", "Textures", "MTMLogo.png"), false);
        }
		if (!FramesLogo)
		{
			FramesLogo = CE_TextureNSpriteExtensions.LoadPNG(Path.Combine(Application.dataPath, "CE_Assets", "Textures", "framlogo.png"), false);
		}
	}
	private void LateUpdate()
	{
		IntroSkip_Version1();
		IntroSkip_Version2();

		if (SkipActive && CurrentWait != null)
        {
			CurrentWait.Skip();
		}
	}

	#endregion

	#region Intro (Version 1)
	private void Intro_Version1()
	{
		float a = (float)((double)Rev1_IntroOutProgress * 0.01);
		CE_CommonUI.GUIDrawRect(CE_CommonUI.FullWindowRect, new Color(0f, 0f, 0f, a));
		GUILayout.Window(-1, CE_CommonUI.FullWindowRect, ExtendedIntro1, "", WindowStyle());
	}
	private void ExtendedIntro1(int windowID)
	{
		LoadAssets();
		if (SkipActive)
		{
			Rev1_LogoProgress = 326;
			Rev1_LogoProgress2 = 276;
		}

		if (Rev1_LogoProgress < 325 && !inPause)
		{
			float a = (float)((double)Rev1_LogoProgress * 0.01);
			int num = Rev1_LogoProgress / 2;
			float num2 = (float)MTMLogo.width / 2f;
			float num3 = (float)MTMLogo.height / 2f;
			num2 += (float)num;
			num3 += (float)num;
			if (Rev1_LogoProgress < 175)
			{
				GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, a);
				Rev1_LogoOutProgress = 100;
			}
			else
			{
				float a2 = (float)((double)Rev1_LogoOutProgress * 0.01);
				GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, a2);
				Rev1_LogoOutProgress--;
			}
			GUI.DrawTexture(new Rect((float)(Screen.width / 2) - num2 / 2f, (float)(Screen.height / 2) - num3 / 2f, num2, num3), MTMLogo);
			Rev1_LogoProgress++;
			if (Rev1_LogoProgress == 175)
			{
				inPause = true;
				Rev1_PausePos = 1;
			}
		}
		else if (Rev1_LogoProgress < 325 && inPause)
		{
			float a = (float)((double)Rev1_LogoProgress * 0.01);
			int num = Rev1_LogoProgress / 2;
			float num2 = (float)MTMLogo.width / 2f;
			float num3 = (float)MTMLogo.height / 2f;
			num2 += (float)num;
			num3 += (float)num;
			GUI.DrawTexture(new Rect((float)(Screen.width / 2) - num2 / 2f, (float)(Screen.height / 2) - num3 / 2f, num2, num3), MTMLogo);
		}

		else if (Rev1_LogoProgress2 < 275 && !inPause)
		{
			float a3 = (float)((double)Rev1_LogoProgress2 * 0.01);
			int num4 = Rev1_LogoProgress2 / 2;
			float num5 = (float)CarJemLogo.width / 2f;
			float num6 = (float)CarJemLogo.height / 2f;
			num5 += (float)num4;
			num6 += (float)num4;
			if (Rev1_LogoProgress2 < 175)
			{
				GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, a3);
				Rev1_LogoOutProgress2 = 100;
			}
			else
			{
				float a4 = (float)((double)Rev1_LogoOutProgress2 * 0.01);
				GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, a4);
				Rev1_LogoOutProgress2--;
			}
			GUI.DrawTexture(new Rect((float)(Screen.width / 2) - num5 / 2f, (float)(Screen.height / 2) - num6 / 2f, num5, num6), CarJemLogo);
			Rev1_LogoProgress2++;
			if (Rev1_LogoProgress2 == 175)
			{
				inPause = true;
				Rev1_PausePos = 2;
			}
		}
		else if (Rev1_LogoProgress2 < 275 && inPause)
		{
			float a3 = (float)((double)Rev1_LogoProgress2 * 0.01);
			int num4 = Rev1_LogoProgress2 / 2;
			float num5 = (float)CarJemLogo.width / 2f;
			float num6 = (float)CarJemLogo.height / 2f;
			num5 += (float)num4;
			num6 += (float)num4;
			GUI.DrawTexture(new Rect((float)(Screen.width / 2) - num5 / 2f, (float)(Screen.height / 2) - num6 / 2f, num5, num6), CarJemLogo);
		}

		else if (Rev1_IntroOutProgress > 0 && !inPause)
		{
			Rev1_IntroOutProgress--;
		}
		else if (!inPause)
		{
			IsShown = false;
		}
	}
	private void ExtendedIntroCoExit_Version1()
	{
		int lastPosition = Rev1_PausePos;
		if (inPause)
		{
			if (Rev1_PausePos == 1) Rev1_LogoProgress++;
			else if (Rev1_PausePos == 2) Rev1_LogoProgress2++;
			Rev1_PausePos = 0;
		}
		if (SkipActive)
		{
			Rev1_LogoProgress = 326;
			Rev1_LogoProgress2 = 276;
		}
		inPause = false;
		if (lastPosition == 1) instance.StartCoroutine(instance.ExtendedIntroCo());
	}
	private void IntroSkip_Version1()
	{
		if (LogoVariant == 1 && IsShown && Input.anyKeyDown && (Rev1_LogoProgress > 100 || Rev1_LogoProgress2 > 100))
		{
			SkipActive = true;
		}
	}

	#endregion

	#region Variables (Version 1)

	private int Rev1_LogoProgress;

	private int Rev1_LogoOutProgress;

	private int Rev1_LogoProgress2;

	private int Rev1_LogoOutProgress2;

	private int Rev1_IntroOutProgress = 100;

	private static int Rev1_PausePos = 0;

	#endregion

	#region Variables (Version 2)

	private int Rev2_LogoProgress;

	private int Rev2_LogoOutProgress;

	private int Rev2_IntroOutProgress = 100;

	private float Rev2_LeftCome = 0;

	private float Rev2_RightCome = 0;

	#endregion

	#region Intro (Version 2)
	private void Intro_Version2()
	{
		float a = (float)((double)Rev2_IntroOutProgress * 0.01);
		CE_CommonUI.GUIDrawRect(CE_CommonUI.FullWindowRect, new Color(0f, 0f, 0f, a));
		GUILayout.Window(-1, CE_CommonUI.FullWindowRect, ExtendedIntro2, "", WindowStyle());
	}
	private void ExtendedIntro2(int windowID)
	{
		LoadAssets();

		var scale = CE_CommonUI.GetScale(Screen.width, Screen.height);

		float spacing = (Screen.height / 2);
		float offset = (Screen.height / 4);

		if (SkipActive) Rev2_LogoProgress = 326;
		if (Rev2_LogoProgress < 325)
		{
			float a = (float)((double)Rev2_LogoProgress * 0.01);
			if (Rev2_LogoProgress < 175)
			{
				GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, a);
				if (!inPause) Rev2_LogoOutProgress = 100;
			}
			else
			{
				float a2 = (float)((double)Rev2_LogoOutProgress * 0.01);
				GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, a2);
				if (!inPause) Rev2_LogoOutProgress--;
			}

			Rev2_LeftCome += 0.20f;
			Rev2_RightCome += 0.20f;

			float startposPadding = 25f;

			float num2 = (float)(MTMLogo.width / 2f) * scale;
			float num3 = (float)(MTMLogo.height / 2f) * scale;
			float startposL = 0 + startposPadding;
			GUI.DrawTexture(new Rect(startposL + Rev2_LeftCome, (float)(spacing - offset) - num3 / 2f, num2, num3), MTMLogo);

			float num5 = (float)(CarJemLogo.width / 2f) * scale;
			float num6 = (float)(CarJemLogo.height / 2f) * scale;
			float startposR = Screen.width - num5 - startposPadding;
			GUI.DrawTexture(new Rect(startposR - Rev2_RightCome, (float)(spacing + offset) - num6 / 2f, num5, num6), CarJemLogo);


			if (!inPause) Rev2_LogoProgress++;
			if (Rev2_LogoProgress == 175) inPause = true;
		}
		else if (Rev2_IntroOutProgress > 0)
		{
			if (!inPause) Rev2_IntroOutProgress--;
		}
		else
		{
			IsShown = false;
		}
	}
	private void ExtendedIntroCoExit_Version2()
	{
		if (inPause) Rev2_LogoProgress++;
		if (SkipActive) Rev2_LogoProgress = 326;
		inPause = false;
	}
	private void IntroSkip_Version2()
	{
		if (LogoVariant == 2 && IsShown && Input.anyKeyDown && (Rev2_LogoProgress > 100))
		{
			SkipActive = true;
		}
	}

	#endregion










}
