using UnityEngine;

public class MapConsole : MonoBehaviour, IUsable
{
	public float usableDistance = 1f;

	public SpriteRenderer Image;

	public float UsableDistance => usableDistance;

	public float PercentCool => 0f;

	public void SetOutline(bool on, bool mainTarget)
	{
		if ((bool)Image)
		{
			Image.material.SetFloat("_Outline", on ? 1 : 0);
			Image.material.SetColor("_OutlineColor", Color.white);
			Image.material.SetColor("_AddColor", mainTarget ? Color.white : Color.clear);
		}
	}

	public float CanUse(GameData.PlayerInfo pc, out bool canUse, out bool couldUse)
	{
		float num = float.MaxValue;
		PlayerControl @object = pc.Object;
		couldUse = pc.Object.CanMove;
		canUse = couldUse;
		if (canUse)
		{
			num = Vector2.Distance(@object.GetTruePosition(), base.transform.position);
			canUse &= num <= UsableDistance;
		}
		return num;
	}

	public void Use()
	{
		CanUse(PlayerControl.LocalPlayer.Data, out var canUse, out var _);
		if (canUse)
		{
			PlayerControl.LocalPlayer.NetTransform.Halt();
			DestroyableSingleton<HudManager>.Instance.ShowMap(delegate(MapBehaviour m)
			{
				m.ShowCountOverlay();
			});
		}
	}
}
