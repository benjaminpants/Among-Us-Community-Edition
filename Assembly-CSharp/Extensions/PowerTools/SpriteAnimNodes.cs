using System;
using UnityEngine;

namespace PowerTools
{
	// Token: 0x0200023C RID: 572
	public class SpriteAnimNodes : MonoBehaviour
	{
		// Token: 0x06000C4E RID: 3150 RVA: 0x0003BBE4 File Offset: 0x00039DE4
		public Vector3 GetPosition(int nodeId, bool ignoredPivot = false)
		{
			if (this.m_spriteRenderer == null)
			{
				this.m_spriteRenderer = base.GetComponent<SpriteRenderer>();
			}
			if (this.m_spriteRenderer == null || this.m_spriteRenderer.sprite == null)
			{
				return Vector2.zero;
			}
			Vector3 vector = this.GetLocalPosition(nodeId, ignoredPivot);
			vector = base.transform.rotation * vector;
			vector.Scale(base.transform.lossyScale);
			return vector + base.transform.position;
		}

		// Token: 0x06000C4F RID: 3151 RVA: 0x0003BC78 File Offset: 0x00039E78
		public Vector3 GetLocalPosition(int nodeId, bool ignoredPivot = false)
		{
			if (this.m_spriteRenderer == null)
			{
				this.m_spriteRenderer = base.GetComponent<SpriteRenderer>();
			}
			if (this.m_spriteRenderer == null || this.m_spriteRenderer.sprite == null)
			{
				return Vector2.zero;
			}
			Vector3 vector = this.GetPositionRaw(nodeId);
			vector.y = -vector.y;
			if (ignoredPivot)
			{
				vector += (Vector3)(this.m_spriteRenderer.sprite.rect.size * 0.5f - this.m_spriteRenderer.sprite.pivot);
			}
			vector *= 1f / this.m_spriteRenderer.sprite.pixelsPerUnit;
			if (this.m_spriteRenderer.flipX)
			{
				vector.x = -vector.x;
			}
			if (this.m_spriteRenderer.flipY)
			{
				vector.y = -vector.y;
			}
			return vector;
		}

		// Token: 0x06000C50 RID: 3152 RVA: 0x0003BD80 File Offset: 0x00039F80
		public float GetAngle(int nodeId)
		{
			float angleRaw = this.GetAngleRaw(nodeId);
			if (this.m_spriteRenderer == null)
			{
				this.m_spriteRenderer = base.GetComponent<SpriteRenderer>();
			}
			if (this.m_spriteRenderer == null || this.m_spriteRenderer.sprite == null)
			{
				return 0f;
			}
			return angleRaw + base.transform.eulerAngles.z;
		}

		// Token: 0x06000C51 RID: 3153 RVA: 0x0003BDEC File Offset: 0x00039FEC
		public Vector2 GetPositionRaw(int nodeId)
		{
			switch (nodeId)
			{
			case 0:
				return this.m_node0;
			case 1:
				return this.m_node1;
			case 2:
				return this.m_node2;
			case 3:
				return this.m_node3;
			case 4:
				return this.m_node4;
			case 5:
				return this.m_node5;
			case 6:
				return this.m_node6;
			case 7:
				return this.m_node7;
			case 8:
				return this.m_node8;
			case 9:
				return this.m_node9;
			default:
				return Vector2.zero;
			}
		}

		// Token: 0x06000C52 RID: 3154 RVA: 0x0003BE74 File Offset: 0x0003A074
		public float GetAngleRaw(int nodeId)
		{
			switch (nodeId)
			{
			case 0:
				return this.m_ang0;
			case 1:
				return this.m_ang1;
			case 2:
				return this.m_ang2;
			case 3:
				return this.m_ang3;
			case 4:
				return this.m_ang4;
			case 5:
				return this.m_ang5;
			case 6:
				return this.m_ang6;
			case 7:
				return this.m_ang7;
			case 8:
				return this.m_ang8;
			case 9:
				return this.m_ang9;
			default:
				return 0f;
			}
		}

		// Token: 0x06000C53 RID: 3155 RVA: 0x0003BEFC File Offset: 0x0003A0FC
		public void Reset()
		{
			this.m_node0 = Vector2.zero;
			this.m_node1 = Vector2.zero;
			this.m_node2 = Vector2.zero;
			this.m_node3 = Vector2.zero;
			this.m_node4 = Vector2.zero;
			this.m_node5 = Vector2.zero;
			this.m_node6 = Vector2.zero;
			this.m_node7 = Vector2.zero;
			this.m_node8 = Vector2.zero;
			this.m_node9 = Vector2.zero;
			this.m_ang0 = 0f;
			this.m_ang1 = 0f;
			this.m_ang2 = 0f;
			this.m_ang3 = 0f;
			this.m_ang4 = 0f;
			this.m_ang5 = 0f;
			this.m_ang6 = 0f;
			this.m_ang7 = 0f;
			this.m_ang8 = 0f;
			this.m_ang9 = 0f;
		}

		// Token: 0x04000BCA RID: 3018
		public static readonly int NUM_NODES = 10;

		// Token: 0x04000BCB RID: 3019
		[SerializeField]
		[HideInInspector]
		private Vector2 m_node0 = Vector2.zero;

		// Token: 0x04000BCC RID: 3020
		[SerializeField]
		[HideInInspector]
		private Vector2 m_node1 = Vector2.zero;

		// Token: 0x04000BCD RID: 3021
		[SerializeField]
		[HideInInspector]
		private Vector2 m_node2 = Vector2.zero;

		// Token: 0x04000BCE RID: 3022
		[SerializeField]
		[HideInInspector]
		private Vector2 m_node3 = Vector2.zero;

		// Token: 0x04000BCF RID: 3023
		[SerializeField]
		[HideInInspector]
		private Vector2 m_node4 = Vector2.zero;

		// Token: 0x04000BD0 RID: 3024
		[SerializeField]
		[HideInInspector]
		private Vector2 m_node5 = Vector2.zero;

		// Token: 0x04000BD1 RID: 3025
		[SerializeField]
		[HideInInspector]
		private Vector2 m_node6 = Vector2.zero;

		// Token: 0x04000BD2 RID: 3026
		[SerializeField]
		[HideInInspector]
		private Vector2 m_node7 = Vector2.zero;

		// Token: 0x04000BD3 RID: 3027
		[SerializeField]
		[HideInInspector]
		private Vector2 m_node8 = Vector2.zero;

		// Token: 0x04000BD4 RID: 3028
		[SerializeField]
		[HideInInspector]
		private Vector2 m_node9 = Vector2.zero;

		// Token: 0x04000BD5 RID: 3029
		[SerializeField]
		[HideInInspector]
		private float m_ang0;

		// Token: 0x04000BD6 RID: 3030
		[SerializeField]
		[HideInInspector]
		private float m_ang1;

		// Token: 0x04000BD7 RID: 3031
		[SerializeField]
		[HideInInspector]
		private float m_ang2;

		// Token: 0x04000BD8 RID: 3032
		[SerializeField]
		[HideInInspector]
		private float m_ang3;

		// Token: 0x04000BD9 RID: 3033
		[SerializeField]
		[HideInInspector]
		private float m_ang4;

		// Token: 0x04000BDA RID: 3034
		[SerializeField]
		[HideInInspector]
		private float m_ang5;

		// Token: 0x04000BDB RID: 3035
		[SerializeField]
		[HideInInspector]
		private float m_ang6;

		// Token: 0x04000BDC RID: 3036
		[SerializeField]
		[HideInInspector]
		private float m_ang7;

		// Token: 0x04000BDD RID: 3037
		[SerializeField]
		[HideInInspector]
		private float m_ang8;

		// Token: 0x04000BDE RID: 3038
		[SerializeField]
		[HideInInspector]
		private float m_ang9;

		// Token: 0x04000BDF RID: 3039
		private SpriteRenderer m_spriteRenderer;
	}
}
