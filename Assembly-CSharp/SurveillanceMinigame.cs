using System;
using System.Collections;
using System.Linq;
using UnityEngine;

// Token: 0x020001B2 RID: 434
public class SurveillanceMinigame : Minigame
{
	// Token: 0x06000967 RID: 2407 RVA: 0x000314DC File Offset: 0x0002F6DC
	public override void Begin(PlayerTask task)
	{
		base.Begin(task);
		this.FilteredRooms = (from i in ShipStatus.Instance.AllRooms
		where i.survCamera
		select i).ToArray<ShipRoom>();
		ShipStatus.Instance.RpcRepairSystem(SystemTypes.Security, 1);
		this.textures = new RenderTexture[this.FilteredRooms.Length];
		for (int j = 0; j < this.FilteredRooms.Length; j++)
		{
			ShipRoom shipRoom = this.FilteredRooms[j];
			Camera camera = UnityEngine.Object.Instantiate<Camera>(this.CameraPrefab);
			camera.transform.SetParent(base.transform);
			camera.transform.position = shipRoom.transform.position + shipRoom.survCamera.Offset;
			camera.orthographicSize = shipRoom.survCamera.CamSize;
			RenderTexture temporary = RenderTexture.GetTemporary((int)(256f * shipRoom.survCamera.CamAspect), 256, 16, RenderTextureFormat.ARGB32);
			this.textures[j] = temporary;
			camera.targetTexture = temporary;
			this.ViewPorts[j].material.SetTexture("_MainTex", temporary);
		}
	}

	// Token: 0x06000968 RID: 2408 RVA: 0x00007BF1 File Offset: 0x00005DF1
	protected override IEnumerator CoAnimateOpen()
	{
		this.Viewables.SetActive(false);
		this.FillQuad.material.SetFloat("_Center", -5f);
		this.FillQuad.material.SetColor("_Color2", Color.clear);
		for (float timer = 0f; timer < 0.25f; timer += Time.deltaTime)
		{
			this.FillQuad.material.SetColor("_Color2", Color.Lerp(Color.clear, Color.black, timer / 0.25f));
			yield return null;
		}
		this.FillQuad.material.SetColor("_Color2", Color.black);
		this.Viewables.SetActive(true);
		for (float timer = 0f; timer < 0.1f; timer += Time.deltaTime)
		{
			this.FillQuad.material.SetFloat("_Center", Mathf.Lerp(-5f, 0f, timer / 0.1f));
			yield return null;
		}
		for (float timer = 0f; timer < 0.15f; timer += Time.deltaTime)
		{
			this.FillQuad.material.SetFloat("_Center", Mathf.Lerp(-3f, 0.4f, timer / 0.15f));
			yield return null;
		}
		this.FillQuad.material.SetFloat("_Center", 0.4f);
		yield break;
	}

	// Token: 0x06000969 RID: 2409 RVA: 0x00007C00 File Offset: 0x00005E00
	private IEnumerator CoAnimateClose()
	{
		for (float timer = 0f; timer < 0.1f; timer += Time.deltaTime)
		{
			this.FillQuad.material.SetFloat("_Center", Mathf.Lerp(0.4f, -5f, timer / 0.1f));
			yield return null;
		}
		this.Viewables.SetActive(false);
		for (float timer = 0f; timer < 0.3f; timer += Time.deltaTime)
		{
			this.FillQuad.material.SetColor("_Color2", Color.Lerp(Color.black, Color.clear, timer / 0.3f));
			yield return null;
		}
		this.FillQuad.material.SetColor("_Color2", Color.clear);
		yield break;
	}

	// Token: 0x0600096A RID: 2410 RVA: 0x00007C0F File Offset: 0x00005E0F
	protected override IEnumerator CoDestroySelf()
	{
		yield return this.CoAnimateClose();
		UnityEngine.Object.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x0600096B RID: 2411 RVA: 0x00031608 File Offset: 0x0002F808
	public void OnDestroy()
	{
		ShipStatus.Instance.RpcRepairSystem(SystemTypes.Security, 2);
		for (int i = 0; i < this.textures.Length; i++)
		{
			this.textures[i].Release();
		}
	}

	// Token: 0x040008FD RID: 2301
	public Camera CameraPrefab;

	// Token: 0x040008FE RID: 2302
	public GameObject Viewables;

	// Token: 0x040008FF RID: 2303
	public MeshRenderer[] ViewPorts;

	// Token: 0x04000900 RID: 2304
	private ShipRoom[] FilteredRooms;

	// Token: 0x04000901 RID: 2305
	private RenderTexture[] textures;

	// Token: 0x04000902 RID: 2306
	public MeshRenderer FillQuad;
}
