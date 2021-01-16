using System.Collections;
using UnityEngine;

public class AcceptDivertPowerGame : Minigame
{
	private LineRenderer[] LeftWires;

	private LineRenderer[] RightWires;

	public GameObject RightWireParent;

	public GameObject LeftWireParent;

	public SpriteRenderer Switch;

	public AudioClip SwitchSound;

	private bool DoNormalMinigame;

	private int CurrentColor;

	private int SelectedColor;

	private int ColorsLeft;

	private bool done;

	private static Color[] colors = new Color[4]
	{
			Color.red,
			Color.blue,
			Color.yellow,
			Color.magenta
	};

	public void Start()
	{
		LeftWires = LeftWireParent.GetComponentsInChildren<LineRenderer>();
		RightWires = RightWireParent.GetComponentsInChildren<LineRenderer>();
		CurrentColor = Random.Range(0,3);
		SelectedColor = Random.Range(0,3);
        if (PlayerControl.GameOptions.TaskDifficulty == 2)
        {
            ColorsLeft = 2;
        }
		else if (PlayerControl.GameOptions.TaskDifficulty == 3)
		{
			ColorsLeft = 5;
		}
		else
        {
			DoNormalMinigame = true;
        }

		if (DoNormalMinigame)
		{
			for (int i = 0; i < LeftWires.Length; i++)
			{
				LeftWires[i].material.SetColor("_Color", Color.yellow);
			}
		}
		else
        {
            for (int i = 0; i < LeftWires.Length; i++)
            {
                LeftWires[i].material.SetColor("_Color", colors[CurrentColor]);
            }
			for (int i = 0; i < RightWires.Length; i++)
			{
				RightWires[i].enabled = true;
				RightWires[i].material.SetColor("_Color", colors[SelectedColor]);
			}
		}
	}

	public void DoSwitch()
	{
		if (!done)
		{
			done = true;
			if (Constants.ShouldPlaySfx())
			{
				SoundManager.Instance.PlaySound(SwitchSound, loop: false);
			}
			StartCoroutine(CoDoSwitch());
		}
	}

	private IEnumerator CoDoSwitch()
	{
		if (!DoNormalMinigame)
		{
			int newcolor = CurrentColor + 1;
			if (newcolor == 4)
            {
				newcolor = 0;
            }
			for (int i = 0; i < RightWires.Length; i++)
			{
				RightWires[i].enabled = true;
				RightWires[i].material.SetColor("_Color", colors[newcolor]);
			}
			if (CurrentColor == SelectedColor)
			{
				ColorsLeft--;
				SelectedColor = Random.Range(0,3);
				for (int j = 0; j < LeftWires.Length; j++)
				{
					LeftWires[j].material.SetColor("_Color", colors[SelectedColor]);
				}
			}
            if (ColorsLeft == 0)
            {
				yield return new WaitForLerp(0.25f, delegate (float t)
				{
					Switch.transform.localEulerAngles = new Vector3(0f, 0f, Mathf.Lerp(0f, 90f, t));
				});
				LeftWires[0].SetPosition(1, new Vector3(1.265f, 0f, 0f));
                yield return new WaitForLerp(0.25f, delegate (float t)
                {
                    Switch.transform.localEulerAngles = new Vector3(0f, 0f, Mathf.Lerp(0f, 90f, t));
                });
                if ((bool)MyNormTask)
                {
                    MyNormTask.NextStep();
                }
                StartCoroutine(CoStartClose());
            }
			yield return new WaitForSeconds(0.5f);
			done = false;
			CurrentColor = newcolor;
		}
		else
        {
			yield return new WaitForLerp(0.25f, delegate (float t)
			{
				Switch.transform.localEulerAngles = new Vector3(0f, 0f, Mathf.Lerp(0f, 90f, t));
			});
			LeftWires[0].SetPosition(1, new Vector3(1.265f, 0f, 0f));
			for (int i = 0; i < RightWires.Length; i++)
			{
				RightWires[i].enabled = true;
				RightWires[i].material.SetColor("_Color", Color.yellow);
			}
			for (int j = 0; j < LeftWires.Length; j++)
			{
				LeftWires[j].material.SetColor("_Color", Color.yellow);
			}
		}
		if (DoNormalMinigame)
		{
			if ((bool)MyNormTask)
			{
				MyNormTask.NextStep();
			}
			StartCoroutine(CoStartClose());
		}
	}

	public void Update()
	{
		for (int i = 0; i < LeftWires.Length; i++)
		{
			Vector2 textureOffset = LeftWires[i].material.GetTextureOffset("_MainTex");
			textureOffset.x -= Time.fixedDeltaTime * 3f;
			LeftWires[i].material.SetTextureOffset("_MainTex", textureOffset);
		}
		for (int j = 0; j < RightWires.Length; j++)
		{
			Vector2 textureOffset2 = RightWires[j].material.GetTextureOffset("_MainTex");
			textureOffset2.x += Time.fixedDeltaTime * 3f;
			RightWires[j].material.SetTextureOffset("_MainTex", textureOffset2);
		}
	}
}
