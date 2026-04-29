using Bifrost.AbnormalData;
using Bifrost.ConstEnum;

public class IAbnormalElementInfo
{
	public readonly AbnormalCategory Category;

	public readonly string Type;

	public readonly long ActivateDelay;

	public IAbnormalElementInfo(AbnormalData_element element)
	{
		Category = (AbnormalCategory)element.category;
		Type = element.type;
		ActivateDelay = element.activate_delay;
	}

	public IAbnormalElementInfo(AbnormalCategory category)
	{
		Category = category;
		Type = "";
		ActivateDelay = 0L;
	}
}
