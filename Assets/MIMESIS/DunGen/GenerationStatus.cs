namespace DunGen
{
	public enum GenerationStatus
	{
		NotStarted = 0,
		PreProcessing = 1,
		TileInjection = 2,
		MainPath = 3,
		Branching = 4,
		BranchPruning = 5,
		InstantiatingTiles = 6,
		PostProcessing = 7,
		Complete = 8,
		Failed = 9
	}
}
