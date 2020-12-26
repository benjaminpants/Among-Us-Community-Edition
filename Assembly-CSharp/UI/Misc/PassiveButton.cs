using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PassiveButton : MonoBehaviour
{
	public bool OnUp = true;

	public bool OnDown;

	public Button.ButtonClickedEvent OnClick = new Button.ButtonClickedEvent();

	public AudioClip ClickSound;

	public UnityEvent OnMouseOver;

	public UnityEvent OnMouseOut;

	public Collider2D[] Colliders;

	public void Start()
	{
		DestroyableSingleton<PassiveButtonManager>.Instance.RegisterOne(this);
		if (Colliders == null || Colliders.Length == 0)
		{
			Colliders = GetComponents<Collider2D>();
		}
	}

	public void Recreate()
    {
		DestroyableSingleton<PassiveButtonManager>.Instance.RegisterOne(this);
		if (Colliders == null || Colliders.Length == 0)
		{
			Colliders = GetComponents<Collider2D>();
		}
	}

	public void DoClick()
	{
		if ((bool)ClickSound)
		{
			SoundManager.Instance.PlaySound(ClickSound, loop: false);
		}
		OnClick.Invoke();
	}

	public void OnDestroy()
	{
		if (DestroyableSingleton<PassiveButtonManager>.InstanceExists)
		{
			DestroyableSingleton<PassiveButtonManager>.Instance.RemoveOne(this);
		}
	}
}
