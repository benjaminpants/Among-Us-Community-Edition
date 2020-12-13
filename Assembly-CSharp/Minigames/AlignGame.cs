using System.Collections;
using UnityEngine;

public class AlignGame : Minigame
{
	private Controller myController = new Controller();

	public FloatRange YRange = new FloatRange(-0.425f, 0.425f);

	public AnimationCurve curve;

	public LineRenderer centerline;

	public LineRenderer[] guidelines;

	public SpriteRenderer engine;

	public Collider2D col;

	public TextController StatusText;

	private float pulseTimer;

	public override void Begin(PlayerTask task)
	{
		base.Begin(task);
		float value = FromByte(MyNormTask.Data[base.ConsoleId]);
		bool flag = IsSuccess(MyNormTask.Data[base.ConsoleId]);
		Vector3 localPosition = col.transform.localPosition;
		localPosition.y = YRange.Clamp(value);
		float num = YRange.ReverseLerp(localPosition.y);
		localPosition.x = curve.Evaluate(num);
		col.transform.eulerAngles = new Vector3(0f, 0f, Mathf.Lerp(20f, -20f, num));
		engine.transform.eulerAngles = new Vector3(0f, 0f, Mathf.Lerp(45f, -45f, num));
		centerline.material.SetColor("_Color", flag ? Color.green : Color.red);
		engine.color = (flag ? Color.green : Color.red);
		col.transform.localPosition = localPosition;
		guidelines[0].enabled = flag;
		guidelines[1].enabled = flag;
		StatusText.gameObject.SetActive(flag);
	}

	public void Update()
	{
		centerline.material.SetTextureOffset("_MainTex", new Vector2(Time.time, 0f));
		guidelines[0].material.SetTextureOffset("_MainTex", new Vector2(Time.time, 0f));
		guidelines[1].material.SetTextureOffset("_MainTex", new Vector2(Time.time, 0f));
		if ((bool)MyTask && MyNormTask.IsComplete)
		{
			return;
		}
		Vector3 localPosition = col.transform.localPosition;
		bool flag = IsSuccess(MyNormTask.Data[base.ConsoleId]);
		bool flag2 = IsSuccess(ToByte(localPosition.y));
		myController.Update();
		switch (myController.CheckDrag(col))
		{
		case DragState.TouchStart:
			pulseTimer = 0f;
			break;
		case DragState.Dragging:
		{
			if (flag)
			{
				break;
			}
			Vector2 vector = myController.DragPosition - (Vector2)base.transform.position;
			float num = YRange.ReverseLerp(localPosition.y);
			localPosition.y = YRange.Clamp(vector.y);
			float num2 = YRange.ReverseLerp(localPosition.y);
			localPosition.x = curve.Evaluate(num2);
			col.transform.eulerAngles = new Vector3(0f, 0f, Mathf.Lerp(20f, -20f, num2));
			engine.transform.eulerAngles = new Vector3(0f, 0f, Mathf.Lerp(45f, -45f, num2));
			centerline.material.SetColor("_Color", flag2 ? Color.green : Color.red);
			if (Mathf.Abs(num2 - num) > 0.001f)
			{
				pulseTimer += Time.deltaTime * 25f;
				switch ((int)pulseTimer % 3)
				{
				case 0:
				case 1:
					engine.color = Color.red;
					break;
				case 2:
					engine.color = Color.clear;
					break;
				}
			}
			else
			{
				engine.color = Color.red;
			}
			break;
		}
		case DragState.Released:
			if (!flag && flag2)
			{
				StartCoroutine(LockEngine());
				MyNormTask.Data[base.ConsoleId] = ToByte(localPosition.y);
				MyNormTask.NextStep();
				StartCoroutine(CoStartClose());
			}
			break;
		}
		col.transform.localPosition = localPosition;
	}

	private IEnumerator LockEngine()
	{
		int i = 0;
		while (i < 3)
		{
			guidelines[0].enabled = true;
			guidelines[1].enabled = true;
			yield return new WaitForSeconds(0.1f);
			guidelines[0].enabled = false;
			guidelines[1].enabled = false;
			yield return new WaitForSeconds(0.1f);
			int num = i + 1;
			i = num;
		}
		StatusText.gameObject.SetActive(value: true);
		Color green = new Color(0f, 0.7f, 0f);
		yield return new WaitForLerp(1f, delegate(float t)
		{
			engine.color = Color.Lerp(Color.white, green, t);
		});
		guidelines[0].enabled = true;
		guidelines[1].enabled = true;
	}

	public static float FromByte(byte b)
	{
		return (float)(int)b * 0.025f - 3f;
	}

	public static byte ToByte(float y)
	{
		return (byte)((y + 3f) / 0.025f);
	}

	public static bool IsSuccess(byte b)
	{
		return Mathf.Abs(FromByte(b)) <= 0.05f;
	}
}
