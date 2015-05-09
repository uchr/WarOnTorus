using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UnitFactoryMenu : BuildingMenu {
	public Text description;

	protected override void Update() {
		base.Update();
		description.text = ((UnitFactory) UserControls.instance.building).GetDescription();
	}
}
