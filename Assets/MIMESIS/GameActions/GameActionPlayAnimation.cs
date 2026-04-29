using Bifrost.ConstEnum;

public class GameActionPlayAnimation : GameAction
{
	public string AnimationName { get; private set; }

	public GameActionPlayAnimation(string animationName)
		: base(DefAction.PLAY_ANIMATION)
	{
		AnimationName = animationName;
	}

	public override void Clone(ref IGameAction? action)
	{
		action = new GameActionPlayAnimation(AnimationName);
	}

	public override IGameAction Clone()
	{
		return new GameActionPlayAnimation(AnimationName);
	}

	public override bool Correct(IGameAction action)
	{
		if (action is GameActionPlayAnimation gameActionPlayAnimation)
		{
			return gameActionPlayAnimation.AnimationName == AnimationName;
		}
		return false;
	}

	public override string ToString()
	{
		return $"{base.ActionType}:{AnimationName}";
	}
}
