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
			animator.Play(skin.RunAnim, CE_WardrobeLoader.AnimationDebugMode ? CE_WardrobeLoader.TestPlaybackSpeed : 1f);
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
			animator.Play(skin.IdleAnim, CE_WardrobeLoader.AnimationDebugMode ? CE_WardrobeLoader.TestPlaybackSpeed : 1f);
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
		if ((bool)skin && skin.isCustom)
		{
			CustomDrawSkin();
		}
	}

	private void CustomDrawSkin()
	{
		string name = layer.sprite.name;
		string key = name.Substring(name.IndexOf("_") + 1);
		if (skin.FrameList.ContainsKey(key))
		{
			CE_CustomSkinDefinition.CustomSkinFrame customSkinFrame = skin.FrameList[key];
			float x = customSkinFrame.Position.x;
			float y = customSkinFrame.Position.y;
			float x2 = customSkinFrame.Size.x;
			float y2 = customSkinFrame.Size.y;
			float x3 = customSkinFrame.Offset.x;
			float y3 = customSkinFrame.Offset.y;
			Texture2D texture = customSkinFrame.Texture;
			layer.sprite = Sprite.Create(texture, new Rect(x, y, x2, y2), new Vector2(x3, y3));
		}
	}
}
