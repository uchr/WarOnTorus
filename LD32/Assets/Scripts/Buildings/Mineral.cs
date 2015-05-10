using UnityEngine;
using System.Collections;

public class Mineral : Building {
	public int saturation = 2000;

	protected override void Awake() {
		base.Awake();
		saturation = BalanceSettings.instance.saturationMinerals;
	}
}
