using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class HudOverrideTask : SabotageTask
{
	public ArrowBehaviour[] Arrows;

	private bool isComplete;

	private HudOverrideSystemType system;

	private bool even;

	public override int TaskStep
	{
		get
		{
			if (!isComplete)
			{
				return 0;
			}
			return 1;
		}
	}

	public override bool IsComplete => isComplete;

	public override void Initialize()
	{
		ShipStatus instance = ShipStatus.Instance;
		system = instance.Systems[SystemTypes.Comms] as HudOverrideSystemType;
		List<Vector2> list = FindObjectsPos();
		for (int i = 0; i < list.Count; i++)
		{
			Arrows[i].target = list[i];
			Arrows[i].gameObject.SetActive(value: true);
		}
	}

	private void FixedUpdate()
	{
		if (!isComplete && !system.IsActive)
		{
			Complete();
		}
	}

	public override bool ValidConsole(Console console)
	{
		return console.TaskTypes.Contains(TaskTypes.FixComms);
	}

	public override void Complete()
	{
		isComplete = true;
		PlayerControl.LocalPlayer.RemoveTask(this);
		if (didContribute)
		{
			StatsManager.Instance.SabsFixed++;
		}
	}

	public override void AppendTaskText(StringBuilder sb)
	{
		even = !even;
		Color color = (even ? Color.yellow : Color.red);
		sb.Append(color.ToTextColor() + "Comms Sabotaged[]");
		for (int i = 0; i < Arrows.Length; i++)
		{
			Arrows[i].GetComponent<SpriteRenderer>().color = color;
		}
	}
}
