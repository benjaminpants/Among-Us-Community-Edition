using UnityEngine;

public class DivertPowerMinigame : Minigame
{
	public SystemTypes[] SliderOrder = new SystemTypes[8]
	{
		SystemTypes.LowerEngine,
		SystemTypes.UpperEngine,
		SystemTypes.Weapons,
		SystemTypes.Shields,
		SystemTypes.Nav,
		SystemTypes.Comms,
		SystemTypes.LifeSupp,
		SystemTypes.Security
	};

	public Collider2D[] Sliders;

	public LineRenderer[] Wires;

	public VerticalGauge[] Gauges;

	private int sliderId;

	public FloatRange SliderY = new FloatRange(-1f, 1f);

	private Controller myController = new Controller();

	public override void Begin(PlayerTask task)
	{
		base.Begin(task);
		DivertPowerTask powerTask = (DivertPowerTask)task;
		sliderId = SliderOrder.IndexOf((SystemTypes t) => t == powerTask.TargetSystem);
		for (int i = 0; i < Sliders.Length; i++)
		{
			if (i != sliderId)
			{
				Sliders[i].GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);
			}
		}
	}

	public void FixedUpdate()
	{
		myController.Update();
		float num = 0f;
		for (int i = 0; i < Sliders.Length; i++)
		{
			num += SliderY.ReverseLerp(Sliders[i].transform.localPosition.y) / (float)Sliders.Length;
		}
		for (int j = 0; j < Sliders.Length; j++)
		{
			float num2 = SliderY.ReverseLerp(Sliders[j].transform.localPosition.y);
			float num3 = num2 / num / 1.6f;
			Gauges[j].value = num3 + (Mathf.PerlinNoise(j, Time.time * 51f) - 0.5f) * 0.04f;
			Color value = Color.Lerp(Color.gray, Color.yellow, num2 * num2);
			value.a = ((!(num3 < 0.1f)) ? 1 : 0);
			Vector2 textureOffset = Wires[j].material.GetTextureOffset("_MainTex");
			textureOffset.x -= Time.fixedDeltaTime * 3f * Mathf.Lerp(0.1f, 2f, num3);
			Wires[j].material.SetTextureOffset("_MainTex", textureOffset);
			Wires[j].material.SetColor("_Color", value);
		}
		if (sliderId < 0)
		{
			return;
		}
		Collider2D collider2D = Sliders[sliderId];
		Vector2 v = collider2D.transform.localPosition;
		switch (myController.CheckDrag(collider2D))
		{
		case DragState.Dragging:
		{
			Vector2 vector = myController.DragPosition - (Vector2)collider2D.transform.parent.position;
			vector.y = SliderY.Clamp(vector.y);
			v.y = vector.y;
			collider2D.transform.localPosition = v;
			break;
		}
		case DragState.Released:
			if (SliderY.max - v.y < 0.05f)
			{
				MyNormTask.NextStep();
				StartCoroutine(CoStartClose());
				sliderId = -1;
				collider2D.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);
			}
			break;
		}
	}
}
