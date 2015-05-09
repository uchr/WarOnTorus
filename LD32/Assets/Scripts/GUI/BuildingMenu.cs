using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BuildingMenu : MonoBehaviour {
	public Text textHP;

	public void Sell() {

	}

	public void Repair() {

	}

	protected virtual void Update() {
		textHP.text = "<b>HP:</b> " + UserControls.instance.building.hp;
	}
}
