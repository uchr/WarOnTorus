using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MineralFactoryMenu : BuildingMenu {
	public Text description;

	protected override void Update() {
		base.Update();
		description.text = ((MineralFactory) UserControls.instance.building).GetDescription();
	}
}
