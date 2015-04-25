﻿using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UserControls : MonoBehaviour {
	public enum Mode {
		Default,
		SelectedBuilding,
		SelectedTroop,
		CreateBuilding,
		RotateBuilding
	};

	public enum EventSelection {
		None,
		Unselected,
		Selected
	}

	public Torus torus;

	public Transform pointer;

	public Text checkMode;

	// Menu
	public GameObject[] buildingMenus;
	public GameObject unitsMenu;

	public Mode mode = Mode.Default;

	// Mode.RotateBuilding && Mode.CreateBuilding
	public Transform tangentSpace;
	private Building createdBuilding;

	// Mode.SelectedTroop
	private Troop troop;

	// Mode.SelectedBuilding
	private Building building;

	// SelectionArea
	public GameObject selectionArea;
	private bool isSelectionArea = false;
	private RectTransform selectionAreaTransform;
	private Vector3 selectionFrom;
	private Vector3 selectionTo;

	public void CreateBuilding(int id) {
		Unselect();
		createdBuilding = BuildingsManager.instance.CreateBuilding(id);
		mode = Mode.CreateBuilding;
	}

	public void CreateUnit(int id) {
		if (building.buildingType == BuildingType.UnitFactory)
			((UnitFactory) building).Production(id);
	}

	public void SetMode(int mode) {
		this.mode = (Mode) mode;
	}

	private EventSelection Select() {
		if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
			isSelectionArea = true;
			selectionArea.SetActive(true);
			selectionFrom = Input.mousePosition;
		}

		if (Input.GetMouseButton(0) && isSelectionArea) {
			selectionTo = Input.mousePosition;
			var min = new Vector2(Mathf.Min(selectionFrom.x, selectionTo.x), Mathf.Min(selectionFrom.y, selectionTo.y));
			var max = new Vector2(Mathf.Max(selectionFrom.x, selectionTo.x), Mathf.Max(selectionFrom.y, selectionTo.y));
			min.x -= Screen.width / 2.0f;
			max.x -= Screen.width / 2.0f - 1.0f;
			min.y -= Screen.height / 2.0f;
			max.y -= Screen.height / 2.0f - 1.0f;
			selectionAreaTransform.offsetMin = min;
			selectionAreaTransform.offsetMax = max;
		}

		if (Input.GetMouseButtonUp(0) && isSelectionArea) {
			Unselect();
			isSelectionArea = false;
			selectionArea.SetActive(false);
			troop = UnitsManager.instance.GetTroopFromSelectionArea(selectionFrom, selectionTo);
			if (troop != null) {
				mode = Mode.SelectedTroop;
				return EventSelection.Selected;
			}
		}

		// TODO FIX IT Может быть подвинуть?
		if(!EventSystem.current.IsPointerOverGameObject() && !isSelectionArea) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Input.GetMouseButtonUp(0)) {
				Unselect();
				if (Physics.Raycast(ray, out hit, 100.0f, 1 << 10)) { // Building layer
					building = hit.transform.GetComponent<Building>();
					mode = Mode.SelectedBuilding;
					return EventSelection.Selected;
				}

				if (Physics.Raycast(ray, out hit, 50.0f, 1 << 11)) { // Unit layer
					var unit = hit.transform.GetComponent<Unit>();
					if (unit.owner == 0) {
						troop = new Troop();
						troop.units = new Unit[1];
						troop.units[0] = unit;
						troop.Select();
						mode = Mode.SelectedTroop;
						return EventSelection.Selected;
					}
				}
				return EventSelection.Unselected;
			}
		}
		return EventSelection.None;
	}

	private void Unselect() {
		mode = Mode.Default;
		foreach (var menu in buildingMenus)
			menu.SetActive(false);
		unitsMenu.SetActive(false);

		if (troop != null)
			troop.Unselect();

		troop = null;
		building = null;
	}

	private void UpdatePointer(bool active = true) {
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out hit, 50.0f, 1 << 8) && active) {
			pointer.position = hit.point;
			pointer.up = torus.GetNormalFromCartesian(hit.point);
		}
		else
			pointer.position = Vector3.down * 100.0f;
	}

	private void RotateBuilding() {
		UpdatePointer();
		RaycastHit tangentHit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Input.GetMouseButton(0)) {
			if(Physics.Raycast(ray, out tangentHit, 50.0f, 1 << 9)) { // TangentSpace layer
				var d = Vector3.Distance(tangentHit.point, createdBuilding.cachedTransform .position);
				if (1.0f <= d && d <= 5.0f)
					createdBuilding.cachedTransform.LookAt(tangentHit.point, tangentSpace.up);
			}
		}

		if (Input.GetMouseButtonUp(0)) {
			var colliders = createdBuilding.GetComponentsInChildren<SphereCollider>();
			foreach (var c in colliders)
				c.enabled = true;
			createdBuilding.Init(0);

			createdBuilding = null;
			Map.instance.CalculateGrid();
			mode = Mode.Default;
		}
	}

	private void CreateBuilding() {
		UpdatePointer(false);
		if(!EventSystem.current.IsPointerOverGameObject()) {
			if (Input.GetMouseButtonUp(1)) {
				Destroy(createdBuilding.gameObject);
				mode = Mode.Default;
			}

			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit, 50.0f, 1 << 8)) { // Torus layer
				if (createdBuilding.PossibleToBuild() && !Physics.Raycast(ray, 50.0f, 1 << 15)) { // BuldingIntersection layer
					createdBuilding.cachedTransform.position = hit.point;
					createdBuilding.cachedTransform.up = torus.GetNormalFromCartesian(hit.point);

					if (Input.GetMouseButtonDown(0)) {
						tangentSpace.position = hit.point;
						tangentSpace.up = torus.GetNormalFromCartesian(hit.point);
						mode = Mode.RotateBuilding;
						return;
					}
				}
			}
			else {
				createdBuilding.cachedTransform.position = Vector3.down * 100.0f;
				if (Input.GetMouseButtonUp(0)) {
					Destroy(createdBuilding.gameObject);
					mode = Mode.Default;
				}
			}
		}
	}

	private void SelectedTroop() {
		UpdatePointer();

		EventSelection eventSelection = Select();
		if (eventSelection == EventSelection.Selected) return;
		if (eventSelection == EventSelection.Unselected) {
			Unselect();
			return;
		}

		if(!EventSystem.current.IsPointerOverGameObject() && !isSelectionArea) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit, 50.0f, 1 << 8)) { // Torus layer
				if (Input.GetMouseButtonDown(1)) {
					if (Physics.Raycast(ray, out hit, 50.0f, 1 << 11)) { // Units layer
						var unit = hit.transform.GetComponent<Unit>();
						if (unit.owner == 1) {
							troop.AttackUnit(unit);
							return;
						}
					}

					if (Physics.Raycast(ray, out hit, 50.0f, 1 << 10)) { // Building layer
						var building = hit.transform.GetComponent<Building>();
						if (building.owner == 1) {
							troop.AttackBuilding(building);
							return;
						}
					}

					if (Physics.Raycast(ray, out hit, 50.0f, 1 << 8)) { // Torus layer
						troop.Move(torus.CartesianToTorus(hit.point));
						return;
					}
				}
			}
		}

		// TODO ADD UPDATE
		unitsMenu.SetActive(true);
	}

	private void SelectedBuilding() {
		UpdatePointer();

		EventSelection eventSelection = Select();
		if (eventSelection == EventSelection.Selected) return;
		if (eventSelection == EventSelection.Unselected) {
			Unselect();
			return;
		}

		// TODO FIX IT
		// TODO ADD UPDATE
		if (building != null)
			buildingMenus[(int) building.buildingType].SetActive(true);
	}

	private void Default() {
		UpdatePointer();
		Select();
	}

	private void Awake() {
		selectionAreaTransform = selectionArea.GetComponentInChildren<RectTransform>();
		selectionArea.SetActive(false);
	}

	private void Update() {
		switch (mode) {
			case Mode.RotateBuilding:
				RotateBuilding();
				checkMode.text = "Mode.RotateBuilding";
				break;
			case Mode.CreateBuilding:
				CreateBuilding();
				checkMode.text = "Mode.CreateBuilding";
				break;
			case Mode.SelectedTroop:
				checkMode.text = "Mode.SelectedTroop";
				SelectedTroop();
				break;
			case Mode.SelectedBuilding:
				checkMode.text = "Mode.SelectedBuilding";
				SelectedBuilding();
				break;
			case Mode.Default:
				checkMode.text = "Mode.Default";
				Default();
				break;
		}
	}
}
