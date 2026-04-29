public interface ISpaceActor : IActor
{
	void OnEnterSpace(VSpace space);

	void OnExitSpace(VSpace space, bool Exclude = false);
}
