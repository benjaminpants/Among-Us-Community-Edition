using UnityEngine;

public class ReportButtonManager : MonoBehaviour
{
	public SpriteRenderer renderer;

	public void SetActive(bool isActive)
	{
		if (isActive)
		{
			renderer.color = Palette.EnabledColor;
			renderer.material.SetFloat("_Desat", 0f);
		}
		else
		{
			renderer.color = Palette.DisabledColor;
			renderer.material.SetFloat("_Desat", 1f);
		}
	}

	public void DoClick()
	{
		if (base.isActiveAndEnabled)
		{
			PlayerControl.LocalPlayer.ReportClosest();
		}
	}
}
