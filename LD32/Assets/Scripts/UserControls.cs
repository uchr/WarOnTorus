using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UserControls : MonoBehaviour {
	public enum Mode {
		Default,
		ConstructBuilding,
		SelectedBuilding,
		SelectedUnits
	};

	public Torus torus;
	public Transform tangentSpace;

	public Transform pointer;

	public GameObject factoryMenu;
	public GameObject unitsMenu;

	public bool field = false;
	public GameObject fieldSelection;
	public RectTransform transformField;
	private Vector3 selectFrom;
	private Vector3 selectTo;

	public Mode mode = Mode.Default;

	public LayerMask layerMask;

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
		//Field
		if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
			field = true;
			selectFrom = Input.mousePosition;
		}

		if (Input.GetMouseButton(0) && selectFrom != Vector3.zero) {
			selectTo = Input.mousePosition;
		}

		if (Input.GetMouseButtonUp(0)) {
			unitsController.SelectUnits(selectFrom, selectTo);
			if (unitsController.units != null)
			mode = Mode.SelectedUnits;
			field = false;
		}

		if(!EventSystem.current.IsPointerOverGameObject()) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Input.GetMouseButtonUp(0)) {
				DeselectAll();
				// For build
				if (Physics.Raycast(ray, out hit, 100.0f, 1 << 10)) {
					var bulding = hit.transform.GetComponent<Factory>();
					if(bulding != null) {
						creatingObjects.factory = bulding;
						mode = Mode.SelectedBuilding;
					}
				}
				// For unit
				if (Physics.Raycast(ray, out hit, 50.0f, 1 << 11) && unitsController.units == null) {
					unitsController.units = new List<Unit>();
					unitsController.units.Add(hit.transform.GetComponent<Unit>());
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

		if (unitsController.units != null)
			unitsMenu.SetActive(true);
		else
			unitsMenu.SetActive(false);

		if (field && Vector3.Distance(selectFrom, selectTo) > 20.0f) {
			fieldSelection.SetActive(true);
			var min = new Vector2(Mathf.Min(selectFrom.x, selectTo.x), Mathf.Min(selectFrom.y, selectTo.y));
			var max = new Vector2(Mathf.Max(selectFrom.x, selectTo.x), Mathf.Max(selectFrom.y, selectTo.y));
			min.x -= Screen.width / 2;
			max.x -= Screen.width / 2;
			min.y -= Screen.height / 2;
			max.y -= Screen.height / 2;
			transformField.offsetMin = min;
			transformField.offsetMax = max;
		}
		else {
			fieldSelection.SetActive(false);
		}
	}

	public void SetMode(int mode) {
		this.mode = (Mode) mode;
	}

	public void DeselectAll() {
		mode = Mode.Default;
		unitsController.units = null;
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
				if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
					DeselectAll();
			}
		}
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (mode == Mode.SelectedUnits) {
			if (!EventSystem.current.IsPointerOverGameObject()) {
				if (Input.GetMouseButtonDown(0))
					DeselectAll();
				if (Input.GetMouseButtonDown(1) && unitsController.units != null && Physics.Raycast(ray, out hit, 50.0f, 1 << 8)) {
					foreach (var unit in unitsController.units) {
						Map.instance.SetPath(pointer.position, unit);
					}
				}
			}
		}
		if (mode == Mode.Default) {
			Select();
		}

		
		SetupGUI();

		if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonDown(0) && !Physics.Raycast(ray, 50.0f, layerMask.value))
			DeselectAll();
		if (mode != Mode.ConstructBuilding && Physics.Raycast(ray, out hit, 50.0f, 1 << 8)) {
			pointer.position = hit.point;
			pointer.up = torus.GetNormal(hit.point);
		}
		else {
			pointer.position = Vector3.down * 100.0f;
		}
	}
}
