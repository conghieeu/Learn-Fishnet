using System;

public interface ITimeSyncable
{
	void OnTimeSync(TimeSpan now);
}
