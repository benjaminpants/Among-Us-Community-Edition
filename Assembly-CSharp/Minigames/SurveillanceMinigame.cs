using System.Collections;
using System.Linq;
using UnityEngine;

public class SurveillanceMinigame : Minigame
{
	public Camera CameraPrefab;

	public GameObject Viewables;

	public MeshRenderer[] ViewPorts;

	private ShipRoom[] FilteredRooms;

	private RenderTexture[] textures;

	public MeshRenderer FillQuad;

	public bool StartedDead;

	public override void Begin(PlayerTask task)
	{
		base.Begin(task);
		FilteredRooms = ShipStatus.Instance.AllRooms.Where((ShipRoom i) => i.survCamera).ToArray();
		StartedDead = false;
		if (!PlayerControl.LocalPlayer.Data.IsDead)
		{
			ShipStatus.Instance.RpcRepairSystem(SystemTypes.Security, 1);
		}
		else
        {
			StartedDead = true;
        }
		textures = new RenderTexture[FilteredRooms.Length];
		for (int j = 0; j < FilteredRooms.Length; j++)
		{
			ShipRoom shipRoom = FilteredRooms[j];
			Camera camera = Object.Instantiate(CameraPrefab);
			camera.transform.SetParent(base.transform);
			camera.transform.position = shipRoom.transform.position + shipRoom.survCamera.Offset;
			camera.orthographicSize = shipRoom.survCamera.CamSize;
			RenderTexture temporary = RenderTexture.GetTemporary((int)(SaveManager.CameraRes * shipRoom.survCamera.CamAspect), SaveManager.CameraRes, 16, RenderTextureFormat.ARGB32);
			textures[j] = temporary;
			camera.targetTexture = temporary;
			ViewPorts[j].material.SetTexture("_MainTex", temporary);
		}
	}

	public void Update()
    {
		if (PlayerTask.PlayerHasHudTask(PlayerControl.LocalPlayer))
        {
			for (int j = 0; j < FilteredRooms.Length; j++)
			{
				ViewPorts[j].enabled = false;
			}
		}
		else
        {
			for (int j = 0; j < FilteredRooms.Length; j++)
			{
				ViewPorts[j].enabled = true;
			}
		}
    }

	protected override IEnumerator CoAnimateOpen()
	{
		Viewables.SetActive(value: false);
		FillQuad.material.SetFloat("_Center", -5f);
		FillQuad.material.SetColor("_Color2", Color.clear);
		for (float timer3 = 0f; timer3 < 0.25f; timer3 += Time.deltaTime)
		{
			FillQuad.material.SetColor("_Color2", Color.Lerp(Color.clear, Color.black, timer3 / 0.25f));
			yield return null;
		}
		FillQuad.material.SetColor("_Color2", Color.black);
		Viewables.SetActive(value: true);
		for (float timer3 = 0f; timer3 < 0.1f; timer3 += Time.deltaTime)
		{
			FillQuad.material.SetFloat("_Center", Mathf.Lerp(-5f, 0f, timer3 / 0.1f));
			yield return null;
		}
		for (float timer3 = 0f; timer3 < 0.15f; timer3 += Time.deltaTime)
		{
			FillQuad.material.SetFloat("_Center", Mathf.Lerp(-3f, 0.4f, timer3 / 0.15f));
			yield return null;
		}
		FillQuad.material.SetFloat("_Center", 0.4f);
	}

	private IEnumerator CoAnimateClose()
	{
		for (float timer2 = 0f; timer2 < 0.1f; timer2 += Time.deltaTime)
		{
			FillQuad.material.SetFloat("_Center", Mathf.Lerp(0.4f, -5f, timer2 / 0.1f));
			yield return null;
		}
		Viewables.SetActive(value: false);
		for (float timer2 = 0f; timer2 < 0.3f; timer2 += Time.deltaTime)
		{
			FillQuad.material.SetColor("_Color2", Color.Lerp(Color.black, Color.clear, timer2 / 0.3f));
			yield return null;
		}
		FillQuad.material.SetColor("_Color2", Color.clear);
	}

	protected override IEnumerator CoDestroySelf()
	{
		yield return CoAnimateClose();
		Object.Destroy(base.gameObject);
	}

	public void OnDestroy()
	{
		if (!StartedDead)
		{
			ShipStatus.Instance.RpcRepairSystem(SystemTypes.Security, 2);
		}
		for (int i = 0; i < textures.Length; i++)
		{
			textures[i].Release();
		}
	}
}
