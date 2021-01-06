using System.IO;
using UnityEngine;
using System.Collections;
public class CE_Intro : MonoBehaviour
{
	private static CE_Intro instance;

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
	private static bool inPause = false;
	private static void Startup()
    {
		instance.StartCoroutine(instance.ExtendedIntroCo());
	}

	private Texture2D CarJemLogo;

	private int LogoProgress;

	private int LogoOutProgress;

	private int LogoProgress2;

	private int LogoOutProgress2;

	private int IntroOutProgress = 100;

	private float LeftCome = 0;

	private float RightCome = 0;

	private Texture2D MTMLogo;

	private bool InitalStyleSaved;

	private GUIStyle InitalStyle;

	private bool SkipActive;

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
		if (IsShown)
		{
			float a = (float)((double)IntroOutProgress * 0.01);
			CE_CommonUI.GUIDrawRect(CE_CommonUI.FullWindowRect, new Color(0f, 0f, 0f, a));
			GUILayout.Window(-1, CE_CommonUI.FullWindowRect, ExtendedIntro2, "");
		}
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

	private static GUIStyle WindowStyle(int w, int h)
	{
		return new GUIStyle(GUI.skin.window)
		{
			normal = 
			{
				background = CE_TextureNSpriteExtensions.MakeTex(1, 1, Color.black)
			}
		};
	}

	private void ExtendedIntro(int windowID)
	{
		LoadAssets();
		GUI.skin.window = null;
		if (SkipActive)
		{
			LogoProgress = 326;
			LogoProgress2 = 276;
		}
		if (LogoProgress < 325)
		{
			float a = (float)((double)LogoProgress * 0.01);
			int num = LogoProgress / 2;
			float num2 = (float)MTMLogo.width / 2f;
			float num3 = (float)MTMLogo.height / 2f;
			num2 += (float)num;
			num3 += (float)num;
			if (LogoProgress < 175)
			{
				GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, a);
				LogoOutProgress = 100;
			}
			else
			{
				float a2 = (float)((double)LogoOutProgress * 0.01);
				GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, a2);
				LogoOutProgress--;
			}
			GUI.DrawTexture(new Rect((float)(Screen.width / 2) - num2 / 2f, (float)(Screen.height / 2) - num3 / 2f, num2, num3), MTMLogo);
			LogoProgress++;
		}
		else if (LogoProgress2 < 275)
		{
			float a3 = (float)((double)LogoProgress2 * 0.01);
			int num4 = LogoProgress2 / 2;
			float num5 = (float)CarJemLogo.width / 2f;
			float num6 = (float)CarJemLogo.height / 2f;
			num5 += (float)num4;
			num6 += (float)num4;
			if (LogoProgress2 < 175)
			{
				GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, a3);
				LogoOutProgress2 = 100;
			}
			else
			{
				float a4 = (float)((double)LogoOutProgress2 * 0.01);
				GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, a4);
				LogoOutProgress2--;
			}
			GUI.DrawTexture(new Rect((float)(Screen.width / 2) - num5 / 2f, (float)(Screen.height / 2) - num6 / 2f, num5, num6), CarJemLogo);
			LogoProgress2++;
		}
		else if (IntroOutProgress > 0)
		{
			IntroOutProgress--;
		}
		else
		{
			GUI.skin.window = InitalStyle;
			IsShown = false;
		}
	}

	private void ExtendedIntro2(int windowID)
	{
		LoadAssets();
		GUI.skin.window = null;

		float spacing = (Screen.height / 2);
		float offset = (Screen.height / 4);

		if (SkipActive)
		{
			LogoProgress = 326;
		}
		if (LogoProgress < 325)
		{
			float a = (float)((double)LogoProgress * 0.01);
			if (LogoProgress < 175)
			{
				GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, a);
				if (!inPause) LogoOutProgress = 100;
			}
			else
			{
				float a2 = (float)((double)LogoOutProgress * 0.01);
				GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, a2);
				if (!inPause) LogoOutProgress--;
			}

			LeftCome += 0.20f;
			RightCome += 0.20f;

			float startposPadding = 25f;

			float num2 = (float)MTMLogo.width / 2f;
			float num3 = (float)MTMLogo.height / 2f;
			float startposL = 0 + startposPadding;
			GUI.DrawTexture(new Rect(startposL + LeftCome, (float)(spacing - offset) - num3 / 2f, num2, num3), MTMLogo);

			float num5 = (float)CarJemLogo.width / 2f;
			float num6 = (float)CarJemLogo.height / 2f;
			float startposR = Screen.width - num5 - startposPadding;
			GUI.DrawTexture(new Rect(startposR - RightCome, (float)(spacing + offset) - num6 / 2f, num5, num6), CarJemLogo);


			if (!inPause) LogoProgress++;
			if (LogoProgress == 175) inPause = true;
		}
		else if (IntroOutProgress > 0)
		{
			if (!inPause) IntroOutProgress--;
		}
		else
		{
			GUI.skin.window = InitalStyle;
			IsShown = false;
		}
	}

	private IEnumerator ExtendedIntroCo()
    {
		while (!inPause)
        {
			yield return null;
        }
		yield return new WaitForSecondsRealtime(2);
		if (inPause) LogoProgress++;
		inPause = false;
	}

	private void LoadAssets()
	{
		if (!CarJemLogo)
		{
			CarJemLogo = CE_TextureNSpriteExtensions.LoadPNG(Path.Combine(Application.dataPath, "CE_Assets", "Textures", "CJLogo.png"));
		}
		if (!MTMLogo)
		{
			MTMLogo = CE_TextureNSpriteExtensions.LoadPNG(Path.Combine(Application.dataPath, "CE_Assets", "Textures", "MTMLogo.png"));
		}
		if (!InitalStyleSaved)
		{
			InitalStyle = GUI.skin.window;
			InitalStyleSaved = true;
		}
	}

	private void LateUpdate()
	{
		if (IsShown && Input.anyKeyDown && (LogoProgress > 100 || LogoProgress2 > 100))
		{
			SkipActive = true;
		}
	}
}
