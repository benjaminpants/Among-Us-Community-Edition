using System.Collections;
using PowerTools;
using UnityEngine;

public class SteamBehaviour : MonoBehaviour
{
	public SpriteAnim anim;

	public FloatRange PlayRate = new FloatRange(0.5f, 1f);

	public void OnEnable()
	{
		StartCoroutine(Run());
	}

	private IEnumerator Run()
	{
		while (true)
		{
			float time = PlayRate.Next();
			while (time > 0f)
			{
				time -= Time.deltaTime;
				yield return null;
			}
			anim.Play();
			while (anim.IsPlaying())
			{
				yield return null;
			}
		}
	}
}
