using UnityEngine;

public class NoShadowBehaviour : MonoBehaviour
{
	public Renderer rend;

	public bool didHit;

	public Renderer shadowChild;

	public void Start()
	{
		LightSource.NoShadows.Add(base.gameObject, this);
	}

	public void OnDestroy()
	{
		LightSource.NoShadows.Remove(base.gameObject);
	}

	private void LateUpdate()
	{
		GameData.PlayerInfo data = PlayerControl.LocalPlayer.Data;
		if (data != null && !data.IsDead)
		{
			if (didHit)
			{
				didHit = false;
				ShipStatus instance = ShipStatus.Instance;
				if ((bool)instance && instance.CalculateLightRadius(data) > instance.MaxLightRadius / 3f)
				{
					SetMaskFunction(8);
					return;
				}
			}
			SetMaskFunction(1);
		}
		else
		{
			SetMaskFunction(8);
		}
	}

	private void SetMaskFunction(int func)
	{
		rend.material.SetInt("_Mask", func);
		if ((bool)shadowChild)
		{
			shadowChild.material.SetInt("_Mask", func);
		}
	}
}
