using Bifrost.ConstEnum;
using ReluProtocol;

public class AbnormalCCInputArgs : AbnormalCommonInputArgs
{
	public PosWithRot? CurrentPos;

	public CCType CCType;

	public PosWithRot? TargetPos;

	public long PushTime;

	public long DownTime;
}
