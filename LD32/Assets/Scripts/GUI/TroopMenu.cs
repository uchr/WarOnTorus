using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TroopMenu : MonoBehaviour {
	public Text description;

	private void Update() {
		description.text = UserControls.instance.troop.GetDescription();
	}
}
