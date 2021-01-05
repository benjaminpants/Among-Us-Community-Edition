using System.Collections.Generic;
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

	public bool isCustom;

	public string RelatedHatName;

	public Dictionary<string, CE_SpriteFrame> FrameList = new Dictionary<string, CE_SpriteFrame>();

	public string ID;

	public bool IsHidden;

	public string ProdId => (RelatedHat != null ? RelatedHat.ProductId : ID);
}
