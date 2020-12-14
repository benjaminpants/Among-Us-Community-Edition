using UnityEngine;

public class UseButtonManager : MonoBehaviour
{
	private static readonly Color DisabledColor;

	private static readonly Color EnabledColor;

	public SpriteRenderer UseButton;

	public Sprite UseImage;

	public Sprite SabotageImage;

	public Sprite VentImage;

	public Sprite AdminMapImage;

	public Sprite SecurityImage;

	public Sprite OptionsImage;

	private IUsable currentTarget;

	private bool KeyPress;

	public void SetTarget(IUsable target)
	{
		currentTarget = target;
		if (target != null)
		{
			if (target is Vent)
			{
				UseButton.sprite = VentImage;
			}
			else if (target is MapConsole)
			{
				UseButton.sprite = AdminMapImage;
			}
			else if (target is OptionsConsole)
			{
				UseButton.sprite = OptionsImage;
			}
			else if (target is SystemConsole)
			{
				SystemConsole systemConsole = (SystemConsole)target;
				if (systemConsole.name.StartsWith("Surv"))
				{
					UseButton.sprite = SecurityImage;
				}
				else if (systemConsole.name.StartsWith("TaskAdd"))
				{
					UseButton.sprite = OptionsImage;
				}
				else
				{
					UseButton.sprite = UseImage;
				}
			}
			else
			{
				UseButton.sprite = UseImage;
			}
			UseButton.SetCooldownNormalizedUvs();
			UseButton.material.SetFloat("_Percent", target.PercentCool);
			UseButton.color = EnabledColor;
		}
		else if (PlayerControl.LocalPlayer.Data.IsImpostor && PlayerControl.LocalPlayer.CanMove)
		{
			UseButton.sprite = SabotageImage;
			UseButton.SetCooldownNormalizedUvs();
			UseButton.color = EnabledColor;
		}
		else
		{
			UseButton.sprite = UseImage;
			UseButton.color = DisabledColor;
		}
	}

	public void DoClick()
	{
		if ((!KeyPress && SaveManager.EnableProHUDMode) || !base.isActiveAndEnabled || !PlayerControl.LocalPlayer)
		{
			return;
		}
		GameData.PlayerInfo data = PlayerControl.LocalPlayer.Data;
		if (currentTarget != null)
		{
			PlayerControl.LocalPlayer.UseClosest();
		}
		else if (data != null && data.IsImpostor)
		{
			DestroyableSingleton<HudManager>.Instance.ShowMap(delegate(MapBehaviour m)
			{
				m.ShowInfectedMap();
			});
		}
	}

	internal void Refresh()
	{
		SetTarget(currentTarget);
	}

	static UseButtonManager()
	{
		DisabledColor = new Color(1f, 1f, 1f, 0.3f);
		EnabledColor = new Color(1f, 1f, 1f, 1f);
	}

	public void LateUpdate()
	{
		bool flag = UseButton.color == DisabledColor || UseButton.sprite == SabotageImage;
		if (SaveManager.EnableProHUDMode && flag)
		{
			UseButton.enabled = false;
		}
		else
		{
			UseButton.enabled = true;
		}
	}

	public void DoKeyClick()
	{
		KeyPress = true;
		DoClick();
		KeyPress = false;
	}
}
