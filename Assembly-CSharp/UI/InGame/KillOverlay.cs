using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillOverlay : MonoBehaviour
{
	public SpriteRenderer background;

	public GameObject flameParent;

	public OverlayKillAnimation[] KillAnims;

	public float FadeTime = 0.6f;

	public OverlayKillAnimation EmergencyOverlay;

	public OverlayKillAnimation ReportOverlay;

	private Queue<Func<IEnumerator>> queue = new Queue<Func<IEnumerator>>();

	private Coroutine showAll;

	private Coroutine showOne;

	public bool IsOpen
	{
		get
		{
			if (showAll == null)
			{
				return queue.Count > 0;
			}
			return true;
		}
	}

	public IEnumerator WaitForFinish()
	{
		while (showAll != null || queue.Count > 0)
		{
			yield return null;
		}
	}

	public void ShowOne(PlayerControl killer, GameData.PlayerInfo victim)
	{
		queue.Enqueue(() => CoShowOne(KillAnims.Random(), killer, victim));
		if (showAll == null)
		{
			showAll = StartCoroutine(ShowAll());
		}
	}

	public void ShowOne(OverlayKillAnimation killAnimPrefab, PlayerControl killer, GameData.PlayerInfo victim)
	{
		queue.Enqueue(() => CoShowOne(killAnimPrefab, killer, victim));
		if (showAll == null)
		{
			showAll = StartCoroutine(ShowAll());
		}
	}

	private IEnumerator ShowAll()
	{
		while (queue.Count > 0 || showOne != null)
		{
			if (showOne == null)
			{
				showOne = StartCoroutine(queue.Dequeue()());
			}
			yield return null;
		}
		showAll = null;
	}

	private IEnumerator CoShowOne(OverlayKillAnimation killAnimPrefab, PlayerControl killer, GameData.PlayerInfo victim)
	{
		OverlayKillAnimation overlayKillAnimation = UnityEngine.Object.Instantiate(killAnimPrefab, base.transform);
		overlayKillAnimation.Begin(killer, victim);
		overlayKillAnimation.gameObject.SetActive(value: false);
		yield return CoShowOne(overlayKillAnimation);
	}

	private IEnumerator CoShowOne(OverlayKillAnimation anim)
	{
		if (Constants.ShouldPlaySfx())
		{
			SoundManager.Instance.PlaySound(anim.Stinger, loop: false).volume = anim.StingerVolume;
		}
		WaitForSeconds wait = new WaitForSeconds(0.0833333358f);
		background.enabled = true; 
		yield return wait;
		background.enabled = false;
		flameParent.SetActive(value: true);
		flameParent.transform.localScale = new Vector3(1f, 0.3f, 1f);
		flameParent.transform.localEulerAngles = new Vector3(0f, 0f, 25f);
		yield return wait;
		flameParent.transform.localScale = new Vector3(1f, 0.5f, 1f);
		flameParent.transform.localEulerAngles = new Vector3(0f, 0f, -15f);
		yield return wait;
		flameParent.transform.localScale = new Vector3(1f, 1f, 1f);
		flameParent.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
		anim.gameObject.SetActive(value: true);
		yield return anim.WaitForFinish();
		UnityEngine.Object.Destroy(anim.gameObject);
		yield return new WaitForLerp(355f / (678f * (float)Math.PI), delegate(float t)
		{
			flameParent.transform.localScale = new Vector3(1f, 1f - t, 1f);
		});
		flameParent.SetActive(value: false);
		showOne = null;
	}
}
