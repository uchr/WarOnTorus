using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class UserControls : MonoBehaviour {
	public enum Mode {
		Default,
		ConstructBuilding,
		SelectedBuilding,
		SelectedUnits
	};

	public Torus torus;
	public Transform tangentSpace;

	//public HorTank horTank;

	public Transform pointer;

	public GameObject factoryMenu;
	public GameObject unitsMenu;

	public Mode mode = Mode.Default;

	private CreatingObjects creatingObjects;
	private UnitsController unitsController;

	public void Construct() {
		if(!EventSystem.current.IsPointerOverGameObject()) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit, 50.0f, 1 << 8)) {
				if (!creatingObjects.rotate) {
					creatingObjects.building.position = hit.point;
					creatingObjects.building.up = torus.GetNormal(hit.point);
				}
				if (Input.GetMouseButtonDown(0)) {
					creatingObjects.rotate = true;
					tangentSpace.position = hit.point;
					tangentSpace.up = torus.GetNormal(hit.point);
				}
				if (Input.GetMouseButton(0)){
					if (creatingObjects.rotate) {
						RaycastHit tangentHit;
						if(Physics.Raycast(ray, out tangentHit, 50.0f, 1 << 9)) {
							var d = Vector3.Distance(tangentHit.point, creatingObjects.building.position);
							if (1.0f <= d && d <= 5.0f)
								creatingObjects.building.LookAt(tangentHit.point, tangentSpace.up);
						}
					}
				}
				if (Input.GetMouseButtonUp(0)) {
					creatingObjects.construct = false;
					creatingObjects.building = null;
					Map.instance.CalculateGrid();
					mode = Mode.Default;
				}
			}
			else {
				if (!creatingObjects.rotate)
					creatingObjects.building.transform.position = Vector3.down * 100.0f;
				if (Input.GetMouseButtonUp(0)) {
					mode = Mode.Default;
					creatingObjects.construct = false;
					Destroy(creatingObjects.building.gameObject);
				}
			}
		}
	}

	private void Select() {
		if(!EventSystem.current.IsPointerOverGameObject()) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Input.GetMouseButtonUp(0)) {
				DeselectAll();
				// For build
				if (Physics.Raycast(ray, out hit, 50.0f, 1 << 10)) {
					var bulding = hit.transform.GetComponent<Factory>();
					if(bulding != null) {
						creatingObjects.factory = bulding;
						mode = Mode.SelectedBuilding;
					}
				}
				// For unit
				if (Physics.Raycast(ray, out hit, 50.0f, 1 << 11)) {
					unitsController.unit = hit.transform.GetComponent<Unit>();
					mode = Mode.SelectedUnits;
				}
			}
		}
	}

	private void SetupGUI() {
		if (creatingObjects.factory != null)
			factoryMenu.SetActive(true);
		else 
			factoryMenu.SetActive(false);
		if (unitsController.unit != null)
			unitsMenu.SetActive(true);
		else
			unitsMenu.SetActive(false);
	}

	public void SetMode(int mode) {
		this.mode = (Mode) mode;
	}

	public void DeselectAll() {
		mode = Mode.Default;
		unitsController.unit = null;
		creatingObjects.factory = null;
	}

	private void Awake() {
		creatingObjects = CreatingObjects.instance;
		unitsController = UnitsController.instance;
	}

	private void Update() {
		if (mode == Mode.ConstructBuilding) {
			Construct();
		}
		if (mode == Mode.SelectedBuilding) {
			if (!EventSystem.current.IsPointerOverGameObject()) {
				if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
					DeselectAll();
			}
		}
		if (mode == Mode.SelectedUnits) {
			if (!EventSystem.current.IsPointerOverGameObject()) {
				if (Input.GetMouseButtonUp(0))
					DeselectAll();
				if (Input.GetMouseButtonUp(1) && unitsController.unit != null) {
					Map.instance.SetPath(pointer.position, unitsController.unit);
				}
			}
		}
		if (mode == Mode.Default) {
			Select();
		}
		
		SetupGUI();

		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (mode != Mode.ConstructBuilding && Physics.Raycast(ray, out hit, 50.0f, 1 << 8)) {
			pointer.position = hit.point;
			pointer.up = torus.GetNormal(hit.point);
		}
		else {
			pointer.position = Vector3.down * 100.0f;
		}
	}
}
