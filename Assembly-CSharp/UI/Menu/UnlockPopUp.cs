using System;
using System.Collections;
using UnityEngine;

public class UnlockPopUp : MonoBehaviour
{
	public IEnumerator Show()
	{
		DateTime utcNow = DateTime.UtcNow;
		if ((utcNow.DayOfYear >= 350 || utcNow.DayOfYear <= 8) && !SaveManager.GetPurchase("hats_newyears2018"))
		{
			base.gameObject.SetActive(value: true);
			SaveManager.SetPurchased("hats_newyears2018");
			while (base.isActiveAndEnabled)
			{
				yield return null;
			}
			/*
		if ((utcNow.DayOfYear >= 120 || utcNow.DayOfYear <= 8) && !SaveManager.GetPurchase("hats_newyears2018"))
		{
			base.gameObject.SetActive(value: true);
			SaveManager.SetPurchased("hats_newyears2018");
			while (base.isActiveAndEnabled)
			{
				yield return null;
			}
		}
		/*
	}
}
