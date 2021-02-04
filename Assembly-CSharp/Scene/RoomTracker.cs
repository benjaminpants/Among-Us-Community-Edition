using System.Collections;
using UnityEngine;

public class RoomTracker : MonoBehaviour
{
	public TextRenderer text;

	public float SourceY = -2.5f;

	public float TargetY = -3.25f;

	private Collider2D playerCollider;

	private ContactFilter2D filter;

	private Collider2D[] buffer = new Collider2D[10];

	public ShipRoom LastRoom;

	private Coroutine slideInRoutine;

	public void Awake()
	{
		filter = default(ContactFilter2D);
		filter.layerMask = Constants.PlayersOnlyMask;
		filter.useLayerMask = true;
		filter.useTriggers = false;
	}

	public void OnDisable()
	{
		LastRoom = null;
		Vector3 localPosition = text.transform.localPosition;
		localPosition.y = TargetY;
		text.transform.localPosition = localPosition;
	}

	public void FixedUpdate()
	{
		ShipRoom[] array = null;
		if ((bool)LobbyBehaviour.Instance)
		{
			array = LobbyBehaviour.Instance.AllRooms;
		}
		if ((bool)ShipStatus.Instance)
		{
			array = ShipStatus.Instance.AllRooms;
		}
		if (array == null)
		{
			return;
		}
		ShipRoom shipRoom = null;
		if ((bool)LastRoom)
		{
			int hitCount = LastRoom.roomArea.OverlapCollider(filter, buffer);
			if (CheckHitsForPlayer(buffer, hitCount))
			{
				shipRoom = LastRoom;
			}
		}
		if (!shipRoom)
		{
			foreach (ShipRoom shipRoom2 in array)
			{
				if ((bool)shipRoom2.roomArea)
				{
					int hitCount2 = shipRoom2.roomArea.OverlapCollider(filter, buffer);
					if (CheckHitsForPlayer(buffer, hitCount2))
					{
						shipRoom = shipRoom2;
					}
				}
			}
		}
		if ((bool)shipRoom)
		{
			if (LastRoom != shipRoom)
			{
				LastRoom = shipRoom;
				if (slideInRoutine != null)
				{
					StopCoroutine(slideInRoutine);
				}
				if (shipRoom.RoomId != 0)
				{
					slideInRoutine = StartCoroutine(CoSlideIn(SystemTypeHelpers.GetName(shipRoom.RoomId)));
				}
				else
				{
					slideInRoutine = StartCoroutine(SlideOut());
				}
			}
		}
		else if ((bool)LastRoom)
		{
			LastRoom = null;
			if (slideInRoutine != null)
			{
				StopCoroutine(slideInRoutine);
			}
			slideInRoutine = StartCoroutine(SlideOut());
		}
	}

	private IEnumerator CoSlideIn(string newRoomName)
	{
		yield return SlideOut();
		Vector3 tempPos = text.transform.localPosition;
		Color tempColor = Color.white;
		text.Text = newRoomName;
		float timer = 0f;
		while (timer < 0.25f)
		{
			timer = Mathf.Min(0.25f, timer + Time.deltaTime);
			float t = timer / 0.25f;
			tempPos.y = Mathf.SmoothStep(TargetY, SourceY, t);
			tempColor.a = Mathf.Lerp(0f, 1f, t);
			text.transform.localPosition = tempPos;
			text.Color = tempColor;
			yield return null;
		}
	}

	private IEnumerator SlideOut()
	{
		Vector3 tempPos = text.transform.localPosition;
		Color tempColor = Color.white;
		float timer = FloatRange.ReverseLerp(tempPos.y, SourceY, TargetY) * 0.1f;
		while (timer < 0.1f)
		{
			timer = Mathf.Min(0.1f, timer + Time.deltaTime);
			float t = timer / 0.1f;
			tempPos.y = Mathf.SmoothStep(SourceY, TargetY, t);
			tempColor.a = Mathf.Lerp(1f, 0f, t);
			text.transform.localPosition = tempPos;
			text.Color = tempColor;
			yield return null;
		}
	}

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
}
