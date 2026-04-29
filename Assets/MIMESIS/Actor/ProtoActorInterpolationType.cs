using System;

[Serializable]
public enum ProtoActorInterpolationType
{
	None = 0,
	deprecated_Sticky = 1,
	deprecated_CurrAndFuture = 2,
	PathQueue = 3,
	DebugByPass = 4
}
