using UnityEngine;

public class MapBehaviour : MonoBehaviour
{
	public static MapBehaviour Instance;

	public AlphaPulse ColorControl;

	public SpriteRenderer HerePoint;

	public GameObject countOverlay;

	public InfectedOverlay infectedOverlay;

	public MapTaskOverlay taskOverlay;

	public bool IsOpen => base.isActiveAndEnabled;

	public bool IsOpenStopped
	{
		get
		{
			if (IsOpen)
			{
				return countOverlay.activeSelf;
			}
			return false;
		}
	}

	private void Awake()
	{
		Instance = this;
	}

	private void GenericShow()
	{
		base.transform.localScale = Vector3.one;
		base.transform.localPosition = new Vector3(0f, 0f, -25f);
		base.gameObject.SetActive(value: true);
	}

	public void ShowInfectedMap()
	{
		if (CE_CustomMapManager.GetCurrentMap().IsCustom)
		{
			return;
		}
		if (IsOpen)
		{
			Close();
		}
		else if (PlayerControl.LocalPlayer.CanMove)
		{
			PlayerControl.LocalPlayer.SetPlayerMaterialColors(HerePoint);
			GenericShow();
			infectedOverlay.gameObject.SetActive(value: true);
			ColorControl.SetColor(PlayerControl.LocalPlayer.Data.IsImpostor ? Palette.ImpostorRed : CE_RoleManager.GetRoleFromID(PlayerControl.LocalPlayer.Data.role).RoleColor);
			taskOverlay.Hide();
			DestroyableSingleton<HudManager>.Instance.SetHudActive(isActive: false);
		}
	}

	public void ShowNormalMap()
	{
		if (CE_CustomMapManager.GetCurrentMap().IsCustom)
        {
			return;
        }
		if (IsOpen)
		{
			Close();
		}
		else if (PlayerControl.LocalPlayer.CanMove)
		{
			PlayerControl.LocalPlayer.SetPlayerMaterialColors(HerePoint);
			GenericShow();
            taskOverlay.Show();
			ColorControl.SetColor(new Color(0.05f, 0.2f, 1f, 1f));
			DestroyableSingleton<HudManager>.Instance.SetHudActive(isActive: false);
		}
	}

	public void ShowCountOverlay()
	{
		GenericShow();
		countOverlay.SetActive(value: true);
		taskOverlay.Hide();
		HerePoint.enabled = false;
		ColorControl.SetColor(Color.green);
		DestroyableSingleton<HudManager>.Instance.SetHudActive(isActive: false);
	}

	public void FixedUpdate()
	{
		if ((bool)ShipStatus.Instance)
		{
			Vector3 position = PlayerControl.LocalPlayer.transform.position;
			position /= ShipStatus.Instance.MapScale;
			position.z = -1f;
			HerePoint.transform.localPosition = position;
		}
	}

	public void Close()
	{
		base.gameObject.SetActive(value: false);
		countOverlay.SetActive(value: false);
		infectedOverlay.gameObject.SetActive(value: false);
		taskOverlay.Hide();
		HerePoint.enabled = true;
		DestroyableSingleton<HudManager>.Instance.SetHudActive(isActive: true);
	}
}
