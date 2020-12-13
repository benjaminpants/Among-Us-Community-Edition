using UnityEngine;

public class WireMinigame : Minigame
{
	private static readonly Color[] colors = new Color[4]
	{
		Color.red,
		Color.blue,
		Color.yellow,
		Color.magenta
	};

	public Wire[] LeftNodes;

	public WireNode[] RightNodes;

	public SpriteRenderer[] LeftLights;

	public SpriteRenderer[] RightLights;

	private Controller myController = new Controller();

	private sbyte[] ExpectedWires = new sbyte[4];

	private sbyte[] ActualWires = new sbyte[4];

	public AudioClip[] WireSounds;

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

	private void UpdateLights()
	{
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
}
