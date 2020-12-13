using UnityEngine;

public static class PhysicsHelpers
{
	private static RaycastHit2D[] castHits = new RaycastHit2D[1];

	private static Vector2 temp = default(Vector2);

	private static ContactFilter2D filter = new ContactFilter2D
	{
		useLayerMask = true
	};

	public static bool AnythingBetween(Vector2 source, Vector2 target, int layerMask, bool useTriggers)
	{
		filter.layerMask = layerMask;
		filter.useTriggers = useTriggers;
		temp.x = target.x - source.x;
		temp.y = target.y - source.y;
		return Physics2D.Raycast(source, temp, filter, castHits, temp.magnitude) > 0;
	}
}
