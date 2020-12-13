using UnityEngine;

namespace PowerTools
{
	public class SpriteAnimNodeSync : MonoBehaviour
	{
		public int NodeId;

		public SpriteAnimNodes Parent;

		public SpriteRenderer ParentRenderer;

		public SpriteRenderer Renderer;

		public void LateUpdate()
		{
			if ((bool)Renderer && (bool)ParentRenderer)
			{
				Renderer.flipX = ParentRenderer.flipX;
			}
			Vector3 localPosition = base.transform.localPosition;
			Vector3 localPosition2 = Parent.GetLocalPosition(NodeId);
			localPosition.x = localPosition2.x;
			localPosition.y = localPosition2.y;
			base.transform.localPosition = localPosition;
			float angle = Parent.GetAngle(NodeId);
			if (!Renderer || !Renderer.flipX)
			{
				base.transform.eulerAngles = new Vector3(0f, 0f, angle);
			}
			else
			{
				base.transform.eulerAngles = new Vector3(0f, 0f, 0f - angle);
			}
		}
	}
}
