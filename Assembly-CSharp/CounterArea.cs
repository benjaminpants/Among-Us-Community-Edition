using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x020001CC RID: 460
public class CounterArea : MonoBehaviour
{
	// Token: 0x060009F9 RID: 2553 RVA: 0x0000815E File Offset: 0x0000635E
	public void OnEnable()
	{
		base.StartCoroutine(this.Run());
	}

	// Token: 0x060009FA RID: 2554 RVA: 0x0000816D File Offset: 0x0000636D
	private IEnumerator Run()
	{
		ShipRoom shipRoom = ShipStatus.Instance.AllRooms.First((ShipRoom r) => r.RoomId == this.RoomType);
		this.filter.useLayerMask = true;
		this.filter.layerMask = Constants.PlayersOnlyMask;
		this.filter.useTriggers = true;
		WaitForSeconds wait = new WaitForSeconds(0.1f);
		Collider2D myCollider = shipRoom.roomArea;
		for (;;)
		{
			int num = myCollider.OverlapCollider(this.filter, this.buffer);
			int num2 = num;
			for (int i = 0; i < num; i++)
			{
				Collider2D collider2D = this.buffer[i];
				if (!(collider2D.tag == "DeadBody"))
				{
					PlayerControl component = collider2D.GetComponent<PlayerControl>();
					if (!component || component.Data == null || component.Data.Disconnected || component.Data.IsDead)
					{
						num2--;
					}
				}
			}
			bool flag = this.myIcons.Count != num2;
			while (this.myIcons.Count < num2)
			{
				PoolableBehavior item = this.pool.Get<PoolableBehavior>();
				this.myIcons.Add(item);
			}
			while (this.myIcons.Count > num2)
			{
				PoolableBehavior poolableBehavior = this.myIcons[this.myIcons.Count - 1];
				this.myIcons.RemoveAt(this.myIcons.Count - 1);
				poolableBehavior.OwnerPool.Reclaim(poolableBehavior);
			}
			if (flag)
			{
				for (int j = 0; j < this.myIcons.Count; j++)
				{
					int num3 = j % 5;
					int num4 = j / 5;
					float num5 = (float)(Mathf.Min(num2 - num4 * 5, 5) - 1) * this.XOffset / -2f;
					this.myIcons[j].transform.position = base.transform.position + new Vector3(num5 + (float)num3 * this.XOffset, (float)num4 * this.YOffset, -1f);
				}
			}
			yield return wait;
		}
		yield break;
	}

	// Token: 0x0400099D RID: 2461
	public SystemTypes RoomType;

	// Token: 0x0400099E RID: 2462
	public ObjectPoolBehavior pool;

	// Token: 0x0400099F RID: 2463
	private Collider2D[] buffer = new Collider2D[10];

	// Token: 0x040009A0 RID: 2464
	private ContactFilter2D filter;

	// Token: 0x040009A1 RID: 2465
	private List<PoolableBehavior> myIcons = new List<PoolableBehavior>();

	// Token: 0x040009A2 RID: 2466
	public float XOffset;

	// Token: 0x040009A3 RID: 2467
	public float YOffset;

	// Token: 0x040009A4 RID: 2468
	public int MaxWidth = 5;
}
