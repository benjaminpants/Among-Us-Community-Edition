using PowerTools;
using UnityEngine;

public class SkinLayer : MonoBehaviour
{
	public SpriteRenderer layer;

	public SpriteAnim animator;

	public SkinData skin;

	public bool Flipped
	{
		set
		{
			layer.flipX = value;
		}
	}

	public bool Visible
	{
		set
		{
			layer.enabled = value;
		}
	}

	public void SetRun(float time = -1f,float speed = -1f)
	{
		if (!skin || !animator)
		{
			SetGhost();
			return;
		}
        if (!animator.IsPlaying(skin.RunAnim))
        {
            animator.Play(skin.RunAnim, (PlayerControl.GameOptions.PlayerSpeedMod));
        }
		if (time != -1f)
		{
			animator.Time = time;
		}
		Update();
	}

	public void SetSpawn(float time = 0f)
	{
		if (!skin || !animator)
		{
			SetGhost();
			return;
		}
		animator.Play(skin.SpawnAnim);
		animator.Time = time;
		Update();
	}

	public void SetExitVent()
	{
		if (!skin || !animator)
		{
			SetGhost();
			return;
		}
		animator.Play(skin.ExitVentAnim);
		Update();
	}

	public void SetEnterVent()
	{
		if (!skin || !animator)
		{
			SetGhost();
			return;
		}
		animator.Play(skin.EnterVentAnim);
		Update();
	}

	public void SetIdle()
	{
		if (!skin || !animator)
		{
			SetGhost();
			return;
		}
		if (!animator.IsPlaying(skin.IdleAnim))
		{
			animator.Play(skin.IdleAnim, 1f);
		}
		Update();
	}

	public void SetGhost()
	{
		if ((bool)animator)
		{
			animator.Stop();
			layer.sprite = null;
			Update();
		}
	}

	internal void SetSkin(uint skinId,int colorid)
	{
		skin = DestroyableSingleton<HatManager>.Instance.GetSkinById(skinId);
		CE_WardrobeManager.SetHatRenderColors(layer,colorid,skin.IsPlayerOverride);
		SetIdle();
		Update();
	}

	public void Update()
	{
	}

	public void LateUpdate()
	{
		if (CE_WardrobeManager.AnimationEditor_Active)
		{
			animator.Speed = CE_WardrobeManager.AnimationEditor_CurrentSpeed;
			animator.Paused = CE_WardrobeManager.AnimationEditor_Paused;
		}

		if (skin && layer)
		{
			if (skin.isCustom && layer.sprite)
            {
				var sprite = CE_WardrobeManager.GetSkin(layer.sprite.name, skin);
				if (sprite) layer.sprite = sprite;
				
			}
		}
	}


}
