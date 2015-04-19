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

    public int maxUnits = 10;

    public UnitSettigns[] units;
    public BuildingSettings[] buildings;
}