using System;

public class CommandLineAnalyzer
{
	public string exexcutionName = "";

	public bool canExcute;

	public bool hostMode;

	public bool participantMode;

	public string participantAddress = "";

	public int participantPort = -1;

	public void Analyze(string[] cmdLine)
	{
		exexcutionName = cmdLine[0];
		if (cmdLine.Length <= 1)
		{
			return;
		}
		string text = cmdLine[1];
		if (text.ToLower() == "host")
		{
			canExcute = true;
			hostMode = true;
			return;
		}
		string[] array = text.Split(':');
		if (array.Length == 2)
		{
			participantAddress = array[0];
			participantPort = Convert.ToInt32(array[1]);
			canExcute = true;
			participantMode = true;
		}
	}
}
