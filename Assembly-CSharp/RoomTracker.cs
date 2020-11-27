using System;
using System.Collections;
using UnityEngine;

// Token: 0x020000FC RID: 252
public class RoomTracker : MonoBehaviour
{
	// Token: 0x0600055A RID: 1370 RVA: 0x00005623 File Offset: 0x00003823
	public void Awake()
	{
		this.filter = default(ContactFilter2D);
		this.filter.layerMask = Constants.PlayersOnlyMask;
		this.filter.useLayerMask = true;
		this.filter.useTriggers = false;
	}

	// Token: 0x0600055B RID: 1371 RVA: 0x00022C24 File Offset: 0x00020E24
	public void OnDisable()
	{
		this.LastRoom = null;
		Vector3 localPosition = this.text.transform.localPosition;
		localPosition.y = this.TargetY;
		this.text.transform.localPosition = localPosition;
	}

	// Token: 0x0600055C RID: 1372 RVA: 0x00022C68 File Offset: 0x00020E68
	public void FixedUpdate()
	{
		ShipRoom[] array = null;
		if (LobbyBehaviour.Instance)
		{
			array = LobbyBehaviour.Instance.AllRooms;
		}
		if (ShipStatus.Instance)
		{
			array = ShipStatus.Instance.AllRooms;
		}
		if (array == null)
		{
			return;
		}
		ShipRoom shipRoom = null;
		if (this.LastRoom)
		{
			int hitCount = this.LastRoom.roomArea.OverlapCollider(this.filter, this.buffer);
			if (RoomTracker.CheckHitsForPlayer(this.buffer, hitCount))
			{
				shipRoom = this.LastRoom;
			}
		}
		if (!shipRoom)
		{
			foreach (ShipRoom shipRoom2 in array)
			{
				if (shipRoom2.roomArea)
				{
					int hitCount2 = shipRoom2.roomArea.OverlapCollider(this.filter, this.buffer);
					if (RoomTracker.CheckHitsForPlayer(this.buffer, hitCount2))
					{
						shipRoom = shipRoom2;
					}
				}
			}
		}
		if (shipRoom)
		{
			if (this.LastRoom != shipRoom)
			{
				this.LastRoom = shipRoom;
				if (this.slideInRoutine != null)
				{
					base.StopCoroutine(this.slideInRoutine);
				}
				if (shipRoom.RoomId != SystemTypes.Hallway)
				{
					this.slideInRoutine = base.StartCoroutine(this.CoSlideIn(SystemTypeHelpers.StringNames[(int)shipRoom.RoomId]));
					return;
				}
				this.slideInRoutine = base.StartCoroutine(this.SlideOut());
				return;
			}
		}
		else if (this.LastRoom)
		{
			this.LastRoom = null;
			if (this.slideInRoutine != null)
			{
				base.StopCoroutine(this.slideInRoutine);
			}
			this.slideInRoutine = base.StartCoroutine(this.SlideOut());
		}
	}

	// Token: 0x0600055D RID: 1373 RVA: 0x0000565E File Offset: 0x0000385E
	private IEnumerator CoSlideIn(string newRoomName)
	{
		yield return this.SlideOut();
		Vector3 tempPos = this.text.transform.localPosition;
		Color tempColor = Color.white;
		this.text.Text = newRoomName;
		float timer = 0f;
		while (timer < 0.25f)
		{
			timer = Mathf.Min(0.25f, timer + Time.deltaTime);
			float t = timer / 0.25f;
			tempPos.y = Mathf.SmoothStep(this.TargetY, this.SourceY, t);
			tempColor.a = Mathf.Lerp(0f, 1f, t);
			this.text.transform.localPosition = tempPos;
			this.text.Color = tempColor;
			yield return null;
		}
		yield break;
	}

	// Token: 0x0600055E RID: 1374 RVA: 0x00005674 File Offset: 0x00003874
	private IEnumerator SlideOut()
	{
		Vector3 tempPos = this.text.transform.localPosition;
		Color tempColor = Color.white;
		float timer = FloatRange.ReverseLerp(tempPos.y, this.SourceY, this.TargetY) * 0.1f;
		while (timer < 0.1f)
		{
			timer = Mathf.Min(0.1f, timer + Time.deltaTime);
			float t = timer / 0.1f;
			tempPos.y = Mathf.SmoothStep(this.SourceY, this.TargetY, t);
			tempColor.a = Mathf.Lerp(1f, 0f, t);
			this.text.transform.localPosition = tempPos;
			this.text.Color = tempColor;
			yield return null;
		}
		yield break;
	}

	// Token: 0x0600055F RID: 1375 RVA: 0x00022DE8 File Offset: 0x00020FE8
	private static bool CheckHitsForPlayer(Collider2D[] buffer, int hitCount)
	{
		if (!PlayerControl.LocalPlayer)
		{
			return false;
		}
		for (int i = 0; i < hitCount; i++)
		{
			if (buffer[i].gameObject == PlayerControl.LocalPlayer.gameObject)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x04000520 RID: 1312
	public TextRenderer text;

	// Token: 0x04000521 RID: 1313
	public float SourceY = -2.5f;

	// Token: 0x04000522 RID: 1314
	public float TargetY = -3.25f;

	// Token: 0x04000523 RID: 1315
	private Collider2D playerCollider;

	// Token: 0x04000524 RID: 1316
	private ContactFilter2D filter;

	// Token: 0x04000525 RID: 1317
	private Collider2D[] buffer = new Collider2D[10];

	// Token: 0x04000526 RID: 1318
	public ShipRoom LastRoom;

	// Token: 0x04000527 RID: 1319
	private Coroutine slideInRoutine;
}
