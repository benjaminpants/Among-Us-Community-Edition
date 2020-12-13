using System.Linq;
using System.Text;
using UnityEngine;

public class NoOxyTask : SabotageTask
{
	public ArrowBehaviour[] Arrows;

	private bool isComplete;

	private LifeSuppSystemType reactor;

	private bool even;

	public int targetNumber;

	public override int TaskStep => reactor.UserCount;

	public override bool IsComplete => isComplete;

	public override void Initialize()
	{
		targetNumber = IntRange.Next(0, 99999);
		ShipStatus instance = ShipStatus.Instance;
		reactor = (LifeSuppSystemType)instance.Systems[SystemTypes.LifeSupp];
		Console[] array = (from c in FindConsoles()
			orderby c.ConsoleId
			select c).ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			Arrows[i].target = array[i].transform.position;
			Arrows[i].gameObject.SetActive(value: true);
		}
		DestroyableSingleton<HudManager>.Instance.StartOxyFlash();
	}

	private void FixedUpdate()
	{
		if (isComplete)
		{
			return;
		}
		if (!reactor.IsActive)
		{
			Complete();
			return;
		}
		for (int i = 0; i < Arrows.Length; i++)
		{
			Arrows[i].gameObject.SetActive(!reactor.GetConsoleComplete(i));
		}
	}

	public override bool ValidConsole(Console console)
	{
		if (!reactor.GetConsoleComplete(console.ConsoleId))
		{
			return console.TaskTypes.Contains(TaskTypes.RestoreOxy);
		}
		return false;
	}

	public override void OnRemove()
	{
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
		sb.Append(color.ToTextColor() + "Oxygen depleted in " + (int)reactor.Countdown);
		sb.AppendLine(" (" + reactor.UserCount + "/" + (byte)2 + ")[]");
		for (int i = 0; i < Arrows.Length; i++)
		{
			Arrows[i].GetComponent<SpriteRenderer>().color = color;
		}
	}
}
