public abstract class CRDecorator : IComposite
{
	public readonly IComposite Composite;

	public readonly int DecoratorID;

	public CRDecorator(IComposite behavior)
	{
		Composite = behavior;
		DecoratorID = Hub.s.vworld?.GenerateNewDecoratorID() ?? 0;
	}
}
