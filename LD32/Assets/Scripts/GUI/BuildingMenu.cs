using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BuildingMenu : MonoBehaviour {
	public Text textHP;

	protected virtual void Update() {
		textHP.text = "<b>HP:</b> " + UserControls.instance.building.hp;
	}
}
