	using FishNet.Managing.Scened;
	
	public class LoadingScreenSceneProcessor : DefaultSceneProcessor
	{
	    public override void LoadStart(LoadQueueData queueData)
	    {
	        base.LoadStart(queueData);
	        LoadingScreen.ShowLoadingScreen();
	    }
	
	    public override void LoadEnd(LoadQueueData queueData)
	    {
	        base.LoadEnd(queueData);
	        LoadingScreen.HideLoadingScreen();
	    }
	}