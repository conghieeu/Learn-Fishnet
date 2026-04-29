public class BTData
{
	public readonly BTKey Key;

	public readonly int PhaseID;

	public readonly IComposite CompositeRoot;

	public BTData(BTKey key, int phaseID, IComposite compositeRoot)
	{
		Key = key;
		PhaseID = phaseID;
		CompositeRoot = compositeRoot;
	}
}
