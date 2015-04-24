using UnityEngine;
using System.Collections;

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

    public float updateGoalLength = 10.0f;

    public float pushForce = 10.0f;

    public int maxMinerals = 5;
    public int saturationMinerals = 2000;
    public int mineralsInTime = 10;
    public float periodProductionMinerals = 10;

    public int maxUnits = 20;

    public GameObject minerals;

    public Material red;
    public Material blue;
}