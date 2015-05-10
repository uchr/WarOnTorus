using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour {
	private static MenuManager _instance;
 
	public static MenuManager instance {
		get	{
			if(_instance == null)
				_instance = GameObject.FindObjectOfType<MenuManager>();
			return _instance;
		}
	}
	
	public GameObject troopMenu;
	public GameObject unitFactoryMenu;
	public GameObject mineralFactoryMenu;

	public void UpdateMenu() {
		Unselet();

		var mode = UserControls.instance.mode;
		switch (mode) {
			case UserControls.Mode.SelectedTroop:
				troopMenu.SetActive(true);
				break;
			case UserControls.Mode.SelectedBuilding:
				var buildingType = UserControls.instance.building.buildingType;
				if (buildingType == BuildingType.UnitFactory)
					unitFactoryMenu.SetActive(true);
				if (buildingType == BuildingType.MineralFactory)
					mineralFactoryMenu.SetActive(true);
				break;
		}
	}

	private void Unselet() {
		troopMenu.SetActive(false);
		unitFactoryMenu.SetActive(false);
		mineralFactoryMenu.SetActive(false);
	}
}
