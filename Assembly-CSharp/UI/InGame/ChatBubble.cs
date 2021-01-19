using UnityEngine;

internal class ChatBubble : PoolableBehavior
{
	public SpriteRenderer ChatFace;

	public SpriteRenderer Xmark;

	public TextRenderer NameText;

	public TextRenderer TextArea;

	public SpriteRenderer Background;

	public void SetLeft()
	{
		base.transform.localPosition = new Vector3(-3f, 0f, 0f);
		ChatFace.flipX = false;
		ChatFace.transform.localPosition = new Vector3(0f, 0.07f, 0f);
		Xmark.transform.localPosition = new Vector3(-0.15f, -0.13f, -0.0001f);
		NameText.transform.localPosition = new Vector3(0.5f, 0.34f, 0f);
		NameText.RightAligned = false;
		TextArea.transform.localPosition = new Vector3(0.5f, 0.09f, 0f);
		TextArea.RightAligned = false;
	}

	public void SetRight()
	{
		base.transform.localPosition = new Vector3(-2.35f, 0f, 0f);
		ChatFace.flipX = true;
		ChatFace.transform.localPosition = new Vector3(4.75f, 0.07f, 0f);
		Xmark.transform.localPosition = new Vector3(0.15f, -0.13f, -0.0001f);
		NameText.transform.localPosition = new Vector3(4.35f, 0.34f, 0f);
		NameText.RightAligned = true;
		TextArea.transform.localPosition = new Vector3(4.35f, 0.09f, 0f);
		TextArea.RightAligned = true;
	}

	public void SetName(string playerName, bool isDead, bool isImpostor, bool imponly = false, byte role = 0)
	{
		NameText.Text = playerName ?? "...";
		if (PlayerControl.GameOptions.Gamemode == 1)
		{
			NameText.Color = (isImpostor ? Palette.InfectedGreen : Color.white);
		}
		else
		{
			NameText.Color = (isImpostor ? Palette.ImpostorRed : Color.white);
		}
		if (role != 0)
        {
			CE_Role roleob = CE_RoleManager.GetRoleFromID(role);
			if (roleob.CanSee(PlayerControl.LocalPlayer.Data))
            {
				NameText.Color = roleob.RoleColor;
			}
        }
        NameText.RefreshMesh();
		if (imponly)
		{
			Background.color = Palette.ImpostorOnlyRed;
		}
		if (isDead)
		{
			Xmark.enabled = true;
			Background.color = Palette.HalfWhite;
		}
	}

	public override void Reset()
	{
		Xmark.enabled = false;
		Background.color = Color.white;
	}
}
