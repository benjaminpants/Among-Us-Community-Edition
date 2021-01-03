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

	public void SetRun()
	{
		if (!skin || !animator)
		{
			SetGhost();
			return;
		}
		if (!animator.IsPlaying(skin.RunAnim))
		{
			animator.Play(skin.RunAnim, PlayerControl.GameOptions.PlayerSpeedMod);
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

	internal void SetSkin(uint skinId)
	{
		skin = DestroyableSingleton<HatManager>.Instance.GetSkinById(skinId);
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

		if ((bool)skin && skin.isCustom)
		{
			var sprite = CE_WardrobeManager.GetSkin(layer.sprite.name, skin);
			if (sprite) layer.sprite = sprite;
		}
	}


}
