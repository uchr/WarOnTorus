using UnityEngine;
using System.Collections;
using System.Text;

public class MineralFactory : Building {
	public Mineral mineral;

	private float timer = 0.0f;

	public string GetDescription() {
		StringBuilder builder = new StringBuilder();

		builder.Append("<b>Saturation: </b>");
		builder.Append(mineral.saturation);
		builder.Append('\n');
		builder.Append("<b>Mining</b>: ");
		builder.Append(100 - (int) (100.0f * timer / BalanceSettings.instance.periodProductionMinerals));
		builder.Append('%');
		builder.Append('\n');
		
		return builder.ToString();
	}

	protected override void Awake() {
		base.Awake();
		timer = BalanceSettings.instance.periodProductionMinerals;
	}

	protected override void Update() {
		base.Update();
		if (mineral != null && mineral.saturation > 0) {
			if (timer <= 0.0f) {
				int profit = Mathf.Min(BalanceSettings.instance.mineralsInTime, mineral.saturation);
				if (owner == 0)
					Player.instance.resourceNumber += profit;
				else 
					AI.instance.resourceNumber += profit;

				mineral.saturation -= profit;

				timer = BalanceSettings.instance.periodProductionMinerals;
			}

			if (timer > 0.0f)
				timer -= Time.deltaTime;
		}
	}
}
