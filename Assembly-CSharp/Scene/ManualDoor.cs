using Hazel;
using PowerTools;
using UnityEngine;

public class ManualDoor : MonoBehaviour
{
	private const float ClosedDuration = 10f;

	public const float CooldownDuration = 30f;

	public bool Open;

	public Collider2D myCollider;

	public SpriteAnim animator;

	public AnimationClip OpenDoorAnim;

	public AnimationClip CloseDoorAnim;

	public virtual void SetDoorway(bool open)
	{
		if (Open != open)
		{
			if ((bool)animator)
			{
				animator.Play(open ? OpenDoorAnim : CloseDoorAnim);
			}
			Open = open;
			myCollider.isTrigger = open;
		}
	}

	public virtual void Serialize(MessageWriter writer)
	{
		writer.Write(Open);
	}

	public virtual void Deserialize(MessageReader reader)
	{
		SetDoorway(reader.ReadBoolean());
	}
}
