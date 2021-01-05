using UnityEngine;

public class WireMinigame : Minigame
{
	private static readonly Color[] colors;

	public Wire[] LeftNodes;

	public WireNode[] RightNodes;

	public SpriteRenderer[] LeftLights;

	public SpriteRenderer[] RightLights;

	private Controller myController = new Controller();

	private sbyte[] ExpectedWires = new sbyte[4];

	private sbyte[] ActualWires = new sbyte[4];

	public AudioClip[] WireSounds;

	private Sprite[] IconSpritesR;

	private Sprite[] IconSpritesL;

	private SpriteRenderer[] Icons;

	private bool TaskIsForThisPanel()
	{
		if (MyNormTask.taskStep < MyNormTask.Data.Length && !MyNormTask.IsComplete)
		{
			return MyNormTask.Data[MyNormTask.taskStep] == base.ConsoleId;
		}
		return false;
	}

	public override void Begin(PlayerTask task)
	{
		base.Begin(task);
		CreateSprites();
		IntRange.FillRandomRange(ExpectedWires);
		for (int i = 0; i < LeftNodes.Length; i++)
		{
			ActualWires[i] = -1;
			int num = ExpectedWires[i];
			Wire wire = LeftNodes[i];
			wire.SetColor(colors[num]);
			wire.WireId = (sbyte)i;
			RightNodes[i].SetColor(colors[i]);
			RightNodes[i].WireId = (sbyte)i;
			int num2 = ActualWires[i];
			if (num2 > -1)
			{
				wire.ConnectRight(RightNodes[num2]);
			}
			else
			{
				wire.ResetLine(Vector3.zero, reset: true);
			}
		}
		UpdateLights();
	}

	public void Update()
	{
		if (!TaskIsForThisPanel())
		{
			return;
		}
		myController.Update();
		_ = (Vector2)base.transform.position;
		for (int i = 0; i < LeftNodes.Length; i++)
		{
			Wire wire = LeftNodes[i];
			switch (myController.CheckDrag(wire.hitbox))
			{
			case DragState.Dragging:
			{
				Vector2 vector = myController.DragPosition;
				WireNode wireNode = CheckRightSide(vector);
				if ((bool)wireNode)
				{
					vector = wireNode.transform.position;
					ActualWires[wire.WireId] = wireNode.WireId;
				}
				else
				{
					vector -= wire.BaseWorldPos.normalized * 0.05f;
					ActualWires[wire.WireId] = -1;
				}
				wire.ResetLine(vector);
				break;
			}
			case DragState.Released:
				if (ActualWires[wire.WireId] == -1)
				{
					wire.ResetLine(wire.BaseWorldPos, reset: true);
				}
				else if (Constants.ShouldPlaySfx())
				{
					SoundManager.Instance.PlaySound(WireSounds.Random(), loop: false);
				}
				CheckTask();
				break;
			}
		}
		UpdateLights();
	}

	private void UpdateIcons()
    {
		float y_pos = -0.16f;
		float y_pos2 = -0.17f;
		float z_pos = 1f;
		int index = 0;
		for (int i = 0; i < 4; i++)
		{
			var position = LeftLights[i].transform.position;
			position.y += y_pos;
			position.z += z_pos;

			Icons[index].transform.localPosition = LeftLights[i].transform.localPosition;
			Icons[index].transform.position = position;
			Icons[index].enabled = false; //TODO: Validate Icons
			switch (LeftNodes[i].WireId)
			{
				case 0:
					Icons[index].sprite = IconSpritesL[0];
					break;
				case 1:
					Icons[index].sprite = IconSpritesL[1];
					break;
				case 2:
					Icons[index].sprite = IconSpritesL[2];
					break;
				case 3:
					Icons[index].sprite = IconSpritesL[3];
					break;
			}

			index++;
		}
		for (int i = 0; i < 4; i++)
		{
			var position = RightLights[i].transform.position;
			position.y += y_pos2;
			position.z -= z_pos;

			Icons[index].transform.localPosition = RightLights[i].transform.localPosition;
			Icons[index].transform.position = position;
			Icons[index].enabled = false; //TODO: Validate Icons
			switch (ExpectedWires[i])
            {
				case 0:
					Icons[index].sprite = IconSpritesR[0];
					break;
				case 1:
					Icons[index].sprite = IconSpritesR[1];
					break;
				case 2:
					Icons[index].sprite = IconSpritesR[2];
					break;
				case 3:
					Icons[index].sprite = IconSpritesR[3];
					break;
			}

			index++;
		}
	}

	private void UpdateLights()
	{
		UpdateIcons();
		for (int i = 0; i < ActualWires.Length; i++)
		{
			Color yellow = Color.yellow;
			yellow *= 1f - Mathf.PerlinNoise(i, Time.time * 35f) * 0.3f;
			yellow.a = 1f;
			if (ActualWires[i] != ExpectedWires[i])
			{
				RightLights[ExpectedWires[i]].color = new Color(0.2f, 0.2f, 0.2f);
			}
			else
			{
				RightLights[ExpectedWires[i]].color = yellow;
			}
			LeftLights[i].color = yellow;
		}
	}

	private WireNode CheckRightSide(Vector2 pos)
	{
		for (int i = 0; i < RightNodes.Length; i++)
		{
			WireNode wireNode = RightNodes[i];
			if (wireNode.hitbox.OverlapPoint(pos))
			{
				return wireNode;
			}
		}
		return null;
	}

	private void CheckTask()
	{
		bool flag = true;
		for (int i = 0; i < ActualWires.Length; i++)
		{
			if (ActualWires[i] != ExpectedWires[i])
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			MyNormTask.NextStep();
			Close();
		}
	}



	private bool CreateSprites()
    {
		//TODO: Actually Use
		if (IconSpritesL == null || IconSpritesR == null)
		{
			System.Collections.Generic.List<string> IconFilePaths = new System.Collections.Generic.List<string>()
			{
				"WireIcon1.png",
				"WireIcon2.png",
				"WireIcon3.png",
				"WireIcon4.png"
			};

            IconSpritesL = new Sprite[4];
			IconSpritesR = new Sprite[4];
			Icons = new SpriteRenderer[8];

			for (int i = 0; i < IconSpritesR.Length; i++)
			{
				string path = System.IO.Path.Combine(CE_Extensions.GetTexturesDirectory("Minigames"), IconFilePaths[i]);
				var texture = CE_TextureNSpriteExtensions.LoadPNG(path);
				texture.filterMode = FilterMode.Point;
				IconSpritesR[i] = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));			
			}

			for (int i = 0; i < IconSpritesL.Length; i++)
			{
				string path = System.IO.Path.Combine(CE_Extensions.GetTexturesDirectory("Minigames"), IconFilePaths[i]);
				var texture = CE_TextureNSpriteExtensions.LoadPNG(path);
				texture.filterMode = FilterMode.Point;
				IconSpritesL[i] = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
			}

			for (int i = 0; i < 8; i++)
            {
				Icons[i] = CreateColorBlindIcon(this.gameObject.layer);
			}
		}
		return true;
	}

	private SpriteRenderer CreateColorBlindIcon(int layer)
    {
		GameObject gameObject = new GameObject("ColorBlindIcon");
		gameObject.layer = layer;
		SpriteRenderer HatRendererExt = gameObject.AddComponent<SpriteRenderer>();
		HatRendererExt.transform.SetParent(this.transform);
		HatRendererExt.transform.position = this.transform.position;
		return HatRendererExt;
	}

	static WireMinigame()
	{
		colors = new Color[4]
		{
			Color.red,
			Color.blue,
			Color.yellow,
			Color.magenta
		};
	}
}
