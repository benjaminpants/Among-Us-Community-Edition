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

	private void Awake()
    {
		InnerA = CE_WardrobeManager.CreateExtSpriteRender(Inner);
		InnerB = CE_WardrobeManager.CreateExtSpriteRender(Inner);
		InnerC = CE_WardrobeManager.CreateExtSpriteRender(Inner);
		InnerD = CE_WardrobeManager.CreateExtSpriteRender(Inner);
	}

	private void Update()
    {
		CE_WardrobeManager.UpdateSpriteRenderer(InnerA, Inner);
		CE_WardrobeManager.UpdateSpriteRenderer(InnerB, Inner);
        CE_WardrobeManager.UpdateSpriteRenderer(InnerC, Inner);
		CE_WardrobeManager.UpdateSpriteRenderer(InnerD, Inner);
	}
}
