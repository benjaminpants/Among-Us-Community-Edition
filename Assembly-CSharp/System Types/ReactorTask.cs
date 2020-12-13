using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ReactorTask : SabotageTask
{
	public ArrowBehaviour[] Arrows;

	private bool isComplete;

	private ReactorSystemType reactor;

	private bool even;

	public override int TaskStep => reactor.UserCount;

	public override bool IsComplete => isComplete;

	public override void Initialize()
	{
		ShipStatus instance = ShipStatus.Instance;
		reactor = (ReactorSystemType)instance.Systems[SystemTypes.Reactor];
		((ReactorShipRoom)instance.AllRooms.First((ShipRoom r) => r.RoomId == SystemTypes.Reactor)).StartMeltdown();
		List<Vector2> list = FindObjectsPos();
		for (int i = 0; i < list.Count; i++)
		{
			Arrows[i].target = list[i];
			Arrows[i].gameObject.SetActive(value: true);
		}
	}

	private void FixedUpdate()
	{
		if (!isComplete && !reactor.IsActive)
		{
			Complete();
		}
	}

	public override bool ValidConsole(Console console)
	{
		return console.TaskTypes.Contains(TaskTypes.ResetReactor);
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
		sb.Append(color.ToTextColor() + "Reactor Meltdown in " + (int)reactor.Countdown);
		sb.AppendLine(" (" + reactor.UserCount + "/" + (byte)2 + ")[]");
		for (int i = 0; i < Arrows.Length; i++)
		{
			Arrows[i].GetComponent<SpriteRenderer>().color = color;
		}
	}
}
