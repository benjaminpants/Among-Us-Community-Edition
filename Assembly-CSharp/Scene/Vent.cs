using PowerTools;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class Vent : MonoBehaviour, IUsable
{
	public int Id;

	public Vent Left;

	public Vent Right;

	public ButtonBehavior[] Buttons;

	public AnimationClip EnterVentAnim;

	public AnimationClip ExitVentAnim;

	private static readonly Vector3 CollOffset;

	private SpriteRenderer myRend;

	public float UsableDistance => 0.75f;

	public float PercentCool => 0f;

	public Vent Up;

	public Vent Down;

	private void Start()
	{
        SetButtons(enabled: false);
		InitButtons();
		myRend = GetComponent<SpriteRenderer>();

		int Venting = PlayerControl.GameOptions.Venting;
		byte VentMode = PlayerControl.GameOptions.VentMode;
		
		if (Venting == 3)
		{
			myRend.enabled = false;
		}
		if (Venting == 2)
		{
			//TODO: Failsafe Checking Need... Can Cause NullException
			bool CanNotVent = (!GameData.Instance.GetPlayerById(PlayerControl.LocalPlayer.PlayerId).IsImpostor || CE_RoleManager.GetRoleFromID(GameData.Instance.GetPlayerById(PlayerControl.LocalPlayer.PlayerId).role).CanDo(CE_Specials.Vent));
			if (CanNotVent)
            {
				Left = null;
				Right = null;
				Up = null;
				Down = null;
				return;
			}
		}
		switch (VentMode)
		{
			case 0:
				Init_Default();
				break;
			case 1: 
				Init_Linked(); 
				break;
			case 2: 
				Init_Pairs(); 
				break;
			case 3:
				Init_Locked(); 
				break;
			case 4:
				Init_Randomized();
				break;
			case 5:
				Init_OneWayRandomized();
				break;
		}



		void Init_Default()
		{

		}
		void Init_LinkedProto()
		{			
			//Prototyping Only
			Vent[] array = Object.FindObjectsOfType<Vent>();
			int num = array.IndexOf(this);
			int num2 = num + 1;
			int num3 = num - 1;
			Up = null;
			Down = null;
			if (num2 > array.Length) Up = null;
			else Up = array[num2];
			if (num3 == -1) Down = null;
			else Down = array[num3];	
		}
		void Init_Linked()
		{
			Vent[] array = Object.FindObjectsOfType<Vent>();
			int num = array.IndexOf(this);
			int num2 = num + 1;
			int num3 = num - 1;
			Left = null;
			Right = null;
			Up = null;
			Down = null;
			if (num2 > array.Length) Left = null;
			else Left = array[num2];
			if (num3 == -1) Right = null;
			else Right = array[num3];
		}
		void Init_Pairs()
		{
			Left = null;
			Up = null;
			Down = null;
		}
		void Init_Locked()
		{
			Left = null;
			Right = null;
			Up = null;
			Down = null;
		}
		void Init_Randomized()
		{
			Vent[] array2 = Object.FindObjectsOfType<Vent>();
			Left = array2[UnityEngine.Random.Range(0, array2.Length)];
			Right = array2[UnityEngine.Random.Range(0, array2.Length)];
			Up = array2[UnityEngine.Random.Range(0, array2.Length)];
			Down = array2[UnityEngine.Random.Range(0, array2.Length)];

		}
		void Init_OneWayRandomized()
		{
			Vent[] array3 = Object.FindObjectsOfType<Vent>();
			Left = array3[UnityEngine.Random.Range(0, array3.Length)];
			Right = null;
			Up = null;
			Down = null;
		}
	}

	public void InitButtons()
	{
        var buttonList = Buttons.ToList();
        var upButton = GameObject.Instantiate(Buttons[0].gameObject);
		var downButton = GameObject.Instantiate(Buttons[0].gameObject);

		upButton.transform.parent = Buttons[0].transform.parent;
		downButton.transform.parent = Buttons[0].transform.parent;

		buttonList.Add(upButton.GetComponent<ButtonBehavior>());
        buttonList.Add(downButton.GetComponent<ButtonBehavior>());
		Buttons = buttonList.ToArray();
	}

	public void SetButtons(bool enabled)
	{
		Vent[] array = new Vent[4]
		{
			Right,
			Left,
			Up,
			Down
		};
		for (int i = 0; i < Buttons.Length; i++)
		{
			ButtonBehavior buttonBehavior = Buttons[i];
			if (enabled)
			{
				Vent vent = array[i];
				if ((bool)vent)
				{
					buttonBehavior.gameObject.SetActive(value: true);
					Vector3 localPosition = (vent.transform.position - base.transform.position).normalized * 0.7f;
					localPosition.y -= 0.08f;
					localPosition.z = -10f;
					buttonBehavior.transform.localPosition = localPosition;
					buttonBehavior.transform.LookAt2d(vent.transform);
				}
				else
				{
					buttonBehavior.gameObject.SetActive(value: false);
				}
			}
			else
			{
				buttonBehavior.gameObject.SetActive(value: false);
			}
		}
	}

	public float CanUse(GameData.PlayerInfo pc, out bool canUse, out bool couldUse)
	{
		float num = float.MaxValue;
		PlayerControl @object = pc.Object;
		couldUse = ((pc.IsImpostor || CE_RoleManager.GetRoleFromID(pc.role).CanDo(CE_Specials.Vent)) || PlayerControl.GameOptions.Venting != 0) && PlayerControl.GameOptions.Venting != 3 && !pc.IsDead && (@object.CanMove || @object.inVent);
		canUse = couldUse;
		if (canUse)
		{
			num = Vector2.Distance(@object.GetTruePosition(), base.transform.position);
			canUse &= num <= UsableDistance;
		}
		return num;
	}

	public void SetOutline(bool on, bool mainTarget)
	{
		myRend.material.SetFloat("_Outline", on ? 1 : 0);
		myRend.material.SetColor("_OutlineColor", PlayerControl.LocalPlayer.Data.IsImpostor ? Color.red : CE_RoleManager.GetRoleFromID(PlayerControl.LocalPlayer.Data.role).RoleColor);
		myRend.material.SetColor("_AddColor", mainTarget ? (PlayerControl.LocalPlayer.Data.IsImpostor ? Color.red : CE_RoleManager.GetRoleFromID(PlayerControl.LocalPlayer.Data.role).RoleColor) : Color.clear);
	}

	public void ClickRight()
	{
		if ((bool)Right)
		{
			DoMove(Right.transform.position - CollOffset);
			SetButtons(enabled: false);
			Right.SetButtons(enabled: true);
		}
	}

	public void ClickLeft()
	{
		if ((bool)Left)
		{
			DoMove(Left.transform.position - CollOffset);
			SetButtons(enabled: false);
			Left.SetButtons(enabled: true);
		}
	}

	private static void DoMove(Vector3 pos)
	{
		PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(pos);
		if (Constants.ShouldPlaySfx())
		{
			SoundManager.Instance.PlaySound(PlayerControl.LocalPlayer.VentMoveSounds.Random(), loop: false).pitch = FloatRange.Next(0.8f, 1.2f);
		}
	}

	public void Use()
	{
		CanUse(PlayerControl.LocalPlayer.Data, out var canUse, out var _);
		if (canUse)
		{
			PlayerControl localPlayer = PlayerControl.LocalPlayer;
			if (Constants.ShouldPlaySfx())
			{
				SoundManager.Instance.StopSound(localPlayer.VentEnterSound);
				SoundManager.Instance.PlaySound(localPlayer.VentEnterSound, loop: false).pitch = FloatRange.Next(0.8f, 1.2f);
			}
			if (localPlayer.inVent)
			{
				localPlayer.MyPhysics.RpcExitVent(Id);
				SetButtons(enabled: false);
			}
			else
			{
				localPlayer.MyPhysics.RpcEnterVent(Id);
				SetButtons(enabled: true);
			}
		}
	}

	internal void EnterVent()
	{
		GetComponent<SpriteAnim>().Play(EnterVentAnim);
	}

	internal void ExitVent()
	{
		GetComponent<SpriteAnim>().Play(ExitVentAnim);
	}

	static Vent()
	{
		CollOffset = new Vector3(0f, -0.3636057f, 0f);
	}
}
