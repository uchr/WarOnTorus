using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerInfo : MonoBehaviour {
	public Text resources;
	public Text units;

	private void Update() {
		resources.text = "<b>Resources: </b>" + Player.instance.resourceNumber;
		units.text = "<b>Units: </b>" + Player.instance.unitsNumber + "/" + BalanceSettings.instance.maxUnits;
	}
}
