using UnityEngine;

namespace PowerTools
{
	public class SpriteAnimNodes : MonoBehaviour
	{
		public static readonly int NUM_NODES = 10;

		[SerializeField]
		[HideInInspector]
		private Vector2 m_node0 = Vector2.zero;

		[SerializeField]
		[HideInInspector]
		private Vector2 m_node1 = Vector2.zero;

		[SerializeField]
		[HideInInspector]
		private Vector2 m_node2 = Vector2.zero;

		[SerializeField]
		[HideInInspector]
		private Vector2 m_node3 = Vector2.zero;

		[SerializeField]
		[HideInInspector]
		private Vector2 m_node4 = Vector2.zero;

		[SerializeField]
		[HideInInspector]
		private Vector2 m_node5 = Vector2.zero;

		[SerializeField]
		[HideInInspector]
		private Vector2 m_node6 = Vector2.zero;

		[SerializeField]
		[HideInInspector]
		private Vector2 m_node7 = Vector2.zero;

		[SerializeField]
		[HideInInspector]
		private Vector2 m_node8 = Vector2.zero;

		[SerializeField]
		[HideInInspector]
		private Vector2 m_node9 = Vector2.zero;

		[SerializeField]
		[HideInInspector]
		private float m_ang0;

		[SerializeField]
		[HideInInspector]
		private float m_ang1;

		[SerializeField]
		[HideInInspector]
		private float m_ang2;

		[SerializeField]
		[HideInInspector]
		private float m_ang3;

		[SerializeField]
		[HideInInspector]
		private float m_ang4;

		[SerializeField]
		[HideInInspector]
		private float m_ang5;

		[SerializeField]
		[HideInInspector]
		private float m_ang6;

		[SerializeField]
		[HideInInspector]
		private float m_ang7;

		[SerializeField]
		[HideInInspector]
		private float m_ang8;

		[SerializeField]
		[HideInInspector]
		private float m_ang9;

		private SpriteRenderer m_spriteRenderer;

		public Vector3 GetPosition(int nodeId, bool ignoredPivot = false)
		{
			if (m_spriteRenderer == null)
			{
				m_spriteRenderer = GetComponent<SpriteRenderer>();
			}
			if (m_spriteRenderer == null || m_spriteRenderer.sprite == null)
			{
				return Vector2.zero;
			}
			Vector3 localPosition = GetLocalPosition(nodeId, ignoredPivot);
			localPosition = base.transform.rotation * localPosition;
			localPosition.Scale(base.transform.lossyScale);
			return localPosition + base.transform.position;
		}

		public Vector3 GetLocalPosition(int nodeId, bool ignoredPivot = false)
		{
			if (m_spriteRenderer == null)
			{
				m_spriteRenderer = GetComponent<SpriteRenderer>();
			}
			if (m_spriteRenderer == null || m_spriteRenderer.sprite == null)
			{
				return Vector2.zero;
			}
			Vector3 result = GetPositionRaw(nodeId);
			result.y = 0f - result.y;
			if (ignoredPivot)
			{
				result += (Vector3)(m_spriteRenderer.sprite.rect.size * 0.5f - m_spriteRenderer.sprite.pivot);
			}
			result *= 1f / m_spriteRenderer.sprite.pixelsPerUnit;
			if (m_spriteRenderer.flipX)
			{
				result.x = 0f - result.x;
			}
			if (m_spriteRenderer.flipY)
			{
				result.y = 0f - result.y;
			}
			return result;
		}

		public float GetAngle(int nodeId)
		{
			float angleRaw = GetAngleRaw(nodeId);
			if (m_spriteRenderer == null)
			{
				m_spriteRenderer = GetComponent<SpriteRenderer>();
			}
			if (m_spriteRenderer == null || m_spriteRenderer.sprite == null)
			{
				return 0f;
			}
			return angleRaw + base.transform.eulerAngles.z;
		}

		public Vector2 GetPositionRaw(int nodeId)
		{
			return nodeId switch
			{
				0 => m_node0, 
				1 => m_node1, 
				2 => m_node2, 
				3 => m_node3, 
				4 => m_node4, 
				5 => m_node5, 
				6 => m_node6, 
				7 => m_node7, 
				8 => m_node8, 
				9 => m_node9, 
				_ => Vector2.zero, 
			};
		}

		public float GetAngleRaw(int nodeId)
		{
			return nodeId switch
			{
				0 => m_ang0, 
				1 => m_ang1, 
				2 => m_ang2, 
				3 => m_ang3, 
				4 => m_ang4, 
				5 => m_ang5, 
				6 => m_ang6, 
				7 => m_ang7, 
				8 => m_ang8, 
				9 => m_ang9, 
				_ => 0f, 
			};
		}

		public void Reset()
		{
			m_node0 = Vector2.zero;
			m_node1 = Vector2.zero;
			m_node2 = Vector2.zero;
			m_node3 = Vector2.zero;
			m_node4 = Vector2.zero;
			m_node5 = Vector2.zero;
			m_node6 = Vector2.zero;
			m_node7 = Vector2.zero;
			m_node8 = Vector2.zero;
			m_node9 = Vector2.zero;
			m_ang0 = 0f;
			m_ang1 = 0f;
			m_ang2 = 0f;
			m_ang3 = 0f;
			m_ang4 = 0f;
			m_ang5 = 0f;
			m_ang6 = 0f;
			m_ang7 = 0f;
			m_ang8 = 0f;
			m_ang9 = 0f;
		}
	}
}
