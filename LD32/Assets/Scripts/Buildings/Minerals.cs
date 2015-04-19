using UnityEngine;
using System.Collections;

public class Minerals : Building {
	public int saturation = 2000;
	public bool busy = false;

	public MineralsFactory mineralsFactory;

	private float timer;

	private BalanceSettings bs;

	private void Awake() {
		bs = BalanceSettings.instance;
		saturation = bs.saturationMinerals;
	}

	private void Update () {
		if (mineralsFactory != null && saturation > bs.mineralsInTime) {
			if (timer < 0.0f) {
				// ADD RESOURCE FOR PARENT FRUCTION
				timer = bs.periodProductionMinerals;
			}
			timer -= Time.deltaTime;
		}
	}
}
