public class RandomSpawnedFieldSkillActorData : SpawnedActorData
{
	public int FieldSkillMasterID { get; private set; }

	public RandomSpawnedFieldSkillActorData(MapMarker_SpawnPoint spawnPointData)
		: base(spawnPointData)
	{
		FieldSkillMasterID = spawnPointData.masterID;
	}
}
