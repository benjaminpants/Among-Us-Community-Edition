using UnityEngine;

public class ColorChip : MonoBehaviour
{
	public SpriteRenderer Inner;

	public GameObject InUseForeground;

	public PassiveButton Button;

	public SpriteRenderer InnerA;

	public SpriteRenderer InnerB;

	public SpriteRenderer InnerC;

	public SpriteRenderer InnerD;

	public uint hatId;

	private void Awake()
    {
		InnerA = CE_WardrobeManager.CreateExtSpriteRender(Inner);
		InnerB = CE_WardrobeManager.CreateExtSpriteRender(Inner);
		InnerC = CE_WardrobeManager.CreateExtSpriteRender(Inner);
		InnerD = CE_WardrobeManager.CreateExtSpriteRender(Inner);
	}

	private void Update()
    {
		CE_WardrobeManager.MatchSpriteRenderer(InnerA, Inner);
		CE_WardrobeManager.MatchSpriteRenderer(InnerB, Inner);
        CE_WardrobeManager.MatchSpriteRenderer(InnerC, Inner);
		CE_WardrobeManager.MatchSpriteRenderer(InnerD, Inner);
	}

	private void SetHatImage(uint hatId, SpriteRenderer target, int hatSlot = 0, int playerColor = 0)
	{
		if (DestroyableSingleton<HatManager>.InstanceExists)
		{
			CE_WardrobeManager.SetExtHatImage(DestroyableSingleton<HatManager>.Instance.GetHatById(hatId), target, hatSlot, playerColor, true);
		}
	}

	public void UpdateHat()
    {
		SetHatImage(hatId, Inner, 0, (int)SaveManager.BodyColor);
		SetHatImage(hatId, InnerA, 1, (int)SaveManager.BodyColor);
		SetHatImage(hatId, InnerB, 2, (int)SaveManager.BodyColor);
		SetHatImage(hatId, InnerC, 3, (int)SaveManager.BodyColor);
		SetHatImage(hatId, InnerD, 4, (int)SaveManager.BodyColor);

		CE_WardrobeManager.MatchBaseHatRender(InnerA, Inner);
		CE_WardrobeManager.MatchBaseHatRender(InnerB, Inner);
		CE_WardrobeManager.MatchBaseHatRender(InnerC, Inner);
		CE_WardrobeManager.MatchBaseHatRender(InnerD, Inner);
	}
}
