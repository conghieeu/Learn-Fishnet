using System.Diagnostics;

namespace ReluReplay
{
	public static class ReplayHelper
	{
		public static string GetGitRevisionForEditor()
		{
			Process process = new Process();
			process.StartInfo.FileName = "git";
			process.StartInfo.Arguments = "rev-parse HEAD";
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.CreateNoWindow = true;
			process.Start();
			string text = process.StandardOutput.ReadToEnd().Trim();
			process.WaitForExit();
			if (string.IsNullOrEmpty(text))
			{
				return "unknown";
			}
			return text.Substring(0, 8);
		}
	}
}
