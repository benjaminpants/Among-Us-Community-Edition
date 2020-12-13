using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CounterArea : MonoBehaviour
{
	public SystemTypes RoomType;

	public ObjectPoolBehavior pool;

	private Collider2D[] buffer = new Collider2D[10];

	private ContactFilter2D filter;

	private List<PoolableBehavior> myIcons = new List<PoolableBehavior>();

	public float XOffset;

	public float YOffset;

	public int MaxWidth = 5;

	public void OnEnable()
	{
		StartCoroutine(Run());
	}

	private IEnumerator Run()
	{
		ShipRoom shipRoom = ShipStatus.Instance.AllRooms.First((ShipRoom r) => r.RoomId == RoomType);
		filter.useLayerMask = true;
		filter.layerMask = Constants.PlayersOnlyMask;
		filter.useTriggers = true;
		WaitForSeconds wait = new WaitForSeconds(0.1f);
		Collider2D myCollider = shipRoom.roomArea;
		while (true)
		{
			int num = myCollider.OverlapCollider(filter, buffer);
			int num2 = num;
			for (int i = 0; i < num; i++)
			{
				Collider2D collider2D = buffer[i];
				if (!(collider2D.tag == "DeadBody"))
				{
					PlayerControl component = collider2D.GetComponent<PlayerControl>();
					if (!component || component.Data == null || component.Data.Disconnected || component.Data.IsDead)
					{
						num2--;
					}
				}
			}
			bool flag = myIcons.Count != num2;
			while (myIcons.Count < num2)
			{
				PoolableBehavior item = pool.Get<PoolableBehavior>();
				myIcons.Add(item);
			}
			while (myIcons.Count > num2)
			{
				PoolableBehavior poolableBehavior = myIcons[myIcons.Count - 1];
				myIcons.RemoveAt(myIcons.Count - 1);
				poolableBehavior.OwnerPool.Reclaim(poolableBehavior);
			}
			if (flag)
			{
				for (int j = 0; j < myIcons.Count; j++)
				{
					int num3 = j % 5;
					int num4 = j / 5;
					float num5 = (float)(Mathf.Min(num2 - num4 * 5, 5) - 1) * XOffset / -2f;
					myIcons[j].transform.position = base.transform.position + new Vector3(num5 + (float)num3 * XOffset, (float)num4 * YOffset, -1f);
				}
			}
			yield return wait;
		}
	}
}
