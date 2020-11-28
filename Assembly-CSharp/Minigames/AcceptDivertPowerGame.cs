using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200007A RID: 122
public class AcceptDivertPowerGame : Minigame
{
	// Token: 0x0600029C RID: 668 RVA: 0x0001486C File Offset: 0x00012A6C
	public void Start()
	{
		this.LeftWires = this.LeftWireParent.GetComponentsInChildren<LineRenderer>();
		this.RightWires = this.RightWireParent.GetComponentsInChildren<LineRenderer>();
		for (int i = 0; i < this.LeftWires.Length; i++)
		{
			this.LeftWires[i].material.SetColor("_Color", Color.yellow);
		}
	}

	// Token: 0x0600029D RID: 669 RVA: 0x00003ACB File Offset: 0x00001CCB
	public void DoSwitch()
	{
		if (this.done)
		{
			return;
		}
		this.done = true;
		if (Constants.ShouldPlaySfx())
		{
			SoundManager.Instance.PlaySound(this.SwitchSound, false, 1f);
		}
		base.StartCoroutine(this.CoDoSwitch());
	}

	// Token: 0x0600029E RID: 670 RVA: 0x00003B08 File Offset: 0x00001D08
	private IEnumerator CoDoSwitch()
	{
		yield return new WaitForLerp(0.25f, delegate(float t)
		{
			this.Switch.transform.localEulerAngles = new Vector3(0f, 0f, Mathf.Lerp(0f, 90f, t));
		});
		this.LeftWires[0].SetPosition(1, new Vector3(1.265f, 0f, 0f));
		for (int i = 0; i < this.RightWires.Length; i++)
		{
			this.RightWires[i].enabled = true;
			this.RightWires[i].material.SetColor("_Color", Color.yellow);
		}
		for (int j = 0; j < this.LeftWires.Length; j++)
		{
			this.LeftWires[j].material.SetColor("_Color", Color.yellow);
		}
		if (this.MyNormTask)
		{
			this.MyNormTask.NextStep();
		}
		base.StartCoroutine(base.CoStartClose(0.75f));
		yield break;
	}

	// Token: 0x0600029F RID: 671 RVA: 0x000148CC File Offset: 0x00012ACC
	public void Update()
	{
		for (int i = 0; i < this.LeftWires.Length; i++)
		{
			Vector2 textureOffset = this.LeftWires[i].material.GetTextureOffset("_MainTex");
			textureOffset.x -= Time.fixedDeltaTime * 3f;
			this.LeftWires[i].material.SetTextureOffset("_MainTex", textureOffset);
		}
		for (int j = 0; j < this.RightWires.Length; j++)
		{
			Vector2 textureOffset2 = this.RightWires[j].material.GetTextureOffset("_MainTex");
			textureOffset2.x += Time.fixedDeltaTime * 3f;
			this.RightWires[j].material.SetTextureOffset("_MainTex", textureOffset2);
		}
	}

	// Token: 0x04000284 RID: 644
	private LineRenderer[] LeftWires;

	// Token: 0x04000285 RID: 645
	private LineRenderer[] RightWires;

	// Token: 0x04000286 RID: 646
	public GameObject RightWireParent;

	// Token: 0x04000287 RID: 647
	public GameObject LeftWireParent;

	// Token: 0x04000288 RID: 648
	public SpriteRenderer Switch;

	// Token: 0x04000289 RID: 649
	public AudioClip SwitchSound;

	// Token: 0x0400028A RID: 650
	private bool done;
}
