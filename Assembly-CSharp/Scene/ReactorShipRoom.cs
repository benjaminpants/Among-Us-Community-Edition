using PowerTools;
using UnityEngine;

public class ReactorShipRoom : ShipRoom
{
	public Sprite normalManifolds;

	public Sprite meltdownManifolds;

	public SpriteRenderer Manifolds;

	public AnimationClip normalReactor;

	public AnimationClip meltdownReactor;

	public SpriteAnim Reactor;

	public AnimationClip normalHighFloor;

	public AnimationClip meltdownHighFloor;

	public SpriteAnim HighFloor;

	public AnimationClip normalMidFloor;

	public AnimationClip meltdownMidFloor;

	public SpriteAnim MidFloor1;

	public SpriteAnim MidFloor2;

	public AnimationClip normalLowFloor;

	public AnimationClip meltdownLowFloor;

	public SpriteAnim LowFloor;

	public AnimationClip[] normalPipes;

	public AnimationClip[] meltdownPipes;

	public SpriteAnim[] Pipes;

	public void StartMeltdown()
	{
		DestroyableSingleton<HudManager>.Instance.StartReactorFlash();
		Manifolds.sprite = meltdownManifolds;
		Reactor.Play(meltdownReactor);
		HighFloor.Play(meltdownHighFloor);
		MidFloor1.Play(meltdownMidFloor);
		MidFloor2.Play(meltdownMidFloor);
		LowFloor.Play(meltdownLowFloor);
		for (int i = 0; i < Pipes.Length; i++)
		{
			Pipes[i].Play(meltdownPipes[i]);
		}
	}

	public void StopMeltdown()
	{
		DestroyableSingleton<HudManager>.Instance.StopReactorFlash();
		Manifolds.sprite = normalManifolds;
		Reactor.Play(normalReactor);
		HighFloor.Play(normalHighFloor);
		MidFloor1.Play(normalMidFloor);
		MidFloor2.Play(normalMidFloor);
		LowFloor.Play(normalLowFloor);
		for (int i = 0; i < Pipes.Length; i++)
		{
			Pipes[i].Play(normalPipes[i]);
		}
	}
}
