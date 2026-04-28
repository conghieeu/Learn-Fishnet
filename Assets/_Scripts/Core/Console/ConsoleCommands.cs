using QFSW.QC;

namespace MellowAbelson.Core.Console
{
    public static class ConsoleCommands
    {
        [Command("settimescale")]
        public static void SetTimeScale(float timeScale)
        {
            UnityEngine.Time.timeScale = timeScale;
        }
    }
}
