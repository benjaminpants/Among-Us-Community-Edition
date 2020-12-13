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
		}
		else if (!animator.IsPlaying(skin.RunAnim))
		{
			animator.Play(skin.RunAnim);
		}
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
	}

	public void SetExitVent()
	{
		if (!skin || !animator)
		{
			SetGhost();
		}
		else
		{
			animator.Play(skin.ExitVentAnim);
		}
	}

	public void SetEnterVent()
	{
		if (!skin || !animator)
		{
			SetGhost();
		}
		else
		{
			animator.Play(skin.EnterVentAnim);
		}
	}

	public void SetIdle()
	{
		if (!skin || !animator)
		{
			SetGhost();
		}
		else if (!animator.IsPlaying(skin.IdleAnim))
		{
			animator.Play(skin.IdleAnim);
		}
	}

	public void SetGhost()
	{
		if ((bool)animator)
		{
			animator.Stop();
			layer.sprite = null;
		}
	}

	internal void SetSkin(uint skinId)
	{
		skin = DestroyableSingleton<HatManager>.Instance.GetSkinById(skinId);
		SetIdle();
	}
}
