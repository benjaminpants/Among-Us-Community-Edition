using System;
using UnityEngine;

namespace PowerTools
{
	// Token: 0x0200023D RID: 573
	public class SpriteAnimNodeSync : MonoBehaviour
	{
		// Token: 0x06000C56 RID: 3158 RVA: 0x0003C06C File Offset: 0x0003A26C
		public void LateUpdate()
		{
			if (this.Renderer && this.ParentRenderer)
			{
				this.Renderer.flipX = this.ParentRenderer.flipX;
			}
			Vector3 localPosition = base.transform.localPosition;
			Vector3 localPosition2 = this.Parent.GetLocalPosition(this.NodeId, false);
			localPosition.x = localPosition2.x;
			localPosition.y = localPosition2.y;
			base.transform.localPosition = localPosition;
			float angle = this.Parent.GetAngle(this.NodeId);
			if (!this.Renderer || !this.Renderer.flipX)
			{
				base.transform.eulerAngles = new Vector3(0f, 0f, angle);
				return;
			}
			base.transform.eulerAngles = new Vector3(0f, 0f, -angle);
		}

		// Token: 0x04000BE0 RID: 3040
		public int NodeId;

		// Token: 0x04000BE1 RID: 3041
		public SpriteAnimNodes Parent;

		// Token: 0x04000BE2 RID: 3042
		public SpriteRenderer ParentRenderer;

		// Token: 0x04000BE3 RID: 3043
		public SpriteRenderer Renderer;
	}
}
