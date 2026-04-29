public class UpgradeEffectorLevelObject : SwitchLevelObject
{
	private bool isTriggered;

	public override bool ForServer => true;

	public override void OnChangeLevelObjectStateSig(int actorID, int occupiedActorID, int prevState, int currentState)
	{
		if (Hub.s.pdata.main is MaintenanceScene { isRepaired: not false } && !isTriggered && currentState == 1)
		{
			base.OnChangeLevelObjectStateSig(actorID, occupiedActorID, prevState, currentState);
			isTriggered = true;
			if (Hub.s != null && Hub.s.tramUpgrade != null)
			{
				Hub.s.tramUpgrade.IsEnterTrigger = true;
			}
		}
	}
}
