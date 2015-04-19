using UnityEngine;
using System.Collections;

[System.Serializable]
public class UnitSettigns {
	public GameObject prefab;
	public int cost;
	public float productionTime;
}

[System.Serializable]
public class BuildingSettings {
	public GameObject prefab;
	public int cost;
	public float productionTime;
}

public class BalanceSettings : MonoBehaviour {
	private static BalanceSettings _instance;
 
    public static BalanceSettings instance
    {
        get
        {
            if(_instance == null)
                _instance = GameObject.FindObjectOfType<BalanceSettings>();
            return _instance;
        }
    }

    public float pushForce = 10.0f;

    public int maxMinerals = 5;
    public int saturationMinerals = 2000;
    public int mineralsInTime = 10;
    public float periodProductionMinerals = 10;

    public int maxUnits = 20;

    public GameObject minerals;

    public UnitSettigns[] units;
    public BuildingSettings[] buildings;
}