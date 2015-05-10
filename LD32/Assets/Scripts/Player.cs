using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	private static Player _instance;

	public static Player instance {
		get {
			if(_instance == null)
				_instance = GameObject.FindObjectOfType<Player>();
			return _instance;
		}
	}

	public int resourceNumber = 2000;
	public int unitsNumber = 0;
}
