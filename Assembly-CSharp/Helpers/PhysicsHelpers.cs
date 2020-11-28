using System;
using UnityEngine;

// Token: 0x02000044 RID: 68
public static class PhysicsHelpers
{
	// Token: 0x06000180 RID: 384 RVA: 0x0000FC90 File Offset: 0x0000DE90
	public static bool AnythingBetween(Vector2 source, Vector2 target, int layerMask, bool useTriggers)
	{
		PhysicsHelpers.filter.layerMask = layerMask;
		PhysicsHelpers.filter.useTriggers = useTriggers;
		PhysicsHelpers.temp.x = target.x - source.x;
		PhysicsHelpers.temp.y = target.y - source.y;
		return Physics2D.Raycast(source, PhysicsHelpers.temp, PhysicsHelpers.filter, PhysicsHelpers.castHits, PhysicsHelpers.temp.magnitude) > 0;
	}

	// Token: 0x04000189 RID: 393
	private static RaycastHit2D[] castHits = new RaycastHit2D[1];

	// Token: 0x0400018A RID: 394
	private static Vector2 temp = default(Vector2);

	// Token: 0x0400018B RID: 395
	private static ContactFilter2D filter = new ContactFilter2D
	{
		useLayerMask = true
	};
}
