using UnityEngine;

[CreateAssetMenu]
public class SkinData : ScriptableObject, IBuyable
{
	public Sprite IdleFrame;

	public AnimationClip IdleAnim;

	public AnimationClip RunAnim;

	public AnimationClip EnterVentAnim;

	public AnimationClip ExitVentAnim;

	public AnimationClip KillTongueImpostor;

	public AnimationClip KillTongueVictim;

	public AnimationClip KillShootImpostor;

	public AnimationClip KillShootVictim;

	public AnimationClip KillStabVictim;

	public AnimationClip KillNeckVictim;

	public Sprite EjectFrame;

	public AnimationClip SpawnAnim;

	public bool Free;

	public HatBehaviour RelatedHat;

	public string StoreName;

	public int Order;

	public string ProdId => RelatedHat.ProductId;
}
