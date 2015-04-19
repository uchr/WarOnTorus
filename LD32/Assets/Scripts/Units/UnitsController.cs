using UnityEngine;
using System.Collections;

public class UnitsController : MonoBehaviour {
	public Unit unit;

	private static UnitsController _instance;

	public static UnitsController instance
	{
		get
		{
			if(_instance == null)
				_instance = GameObject.FindObjectOfType<UnitsController>();
			return _instance;
		}
	}
}
