using PowerTools;
using UnityEngine;

public class ReactorRoomWire : MonoBehaviour
{
	public Console myConsole;

	public SpriteAnim Image;

	public AnimationClip Normal;

	public AnimationClip MeltdownNeed;

	public AnimationClip MeltdownReady;

	private ReactorSystemType reactor;

	public void Update()
	{
		if (reactor == null)
		{
			if (!ShipStatus.Instance || !ShipStatus.Instance.Systems.TryGetValue(SystemTypes.Reactor, out var value))
			{
				return;
			}
			reactor = (ReactorSystemType)value;
		}
		if (reactor.IsActive)
		{
			if (reactor.GetConsoleComplete(myConsole.ConsoleId))
			{
				if (!Image.IsPlaying(MeltdownReady))
				{
					Image.Play(MeltdownReady);
				}
			}
			else if (!Image.IsPlaying(MeltdownNeed))
			{
				Image.Play(MeltdownNeed);
			}
		}
		else if (!Image.IsPlaying(Normal))
		{
			Image.Play(Normal);
		}
	}
}
