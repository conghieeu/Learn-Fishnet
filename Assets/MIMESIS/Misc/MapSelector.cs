using System;
using UnityEngine;

public class MapSelector : MonoBehaviour
{
	[SerializeField]
	private MapButtonLevelObject leftSelectButton;

	[SerializeField]
	private MapButtonLevelObject rightSelectButton;

	[SerializeField]
	private Animator leftButtonAnimator;

	[SerializeField]
	private Animator rightButtonAnimator;

	[SerializeField]
	private string leftSelectAnim = "";

	[SerializeField]
	private string leftSelectedAnim = "";

	[SerializeField]
	private string leftDeselectedAnim = "";

	[SerializeField]
	private string rightSelectAnim = "";

	[SerializeField]
	private string rightSelectedAnim = "";

	[SerializeField]
	private string rightDeselectedAnim = "";

	[SerializeField]
	private string buttonMasterAudioKey;

	private void Start()
	{
		if (leftSelectButton != null)
		{
			MapButtonLevelObject mapButtonLevelObject = leftSelectButton;
			mapButtonLevelObject.triggerAction = (Action<MapButtonLevelObject.EButtonType>)Delegate.Combine(mapButtonLevelObject.triggerAction, new Action<MapButtonLevelObject.EButtonType>(SelectMap));
		}
		if (rightSelectButton != null)
		{
			MapButtonLevelObject mapButtonLevelObject2 = rightSelectButton;
			mapButtonLevelObject2.triggerAction = (Action<MapButtonLevelObject.EButtonType>)Delegate.Combine(mapButtonLevelObject2.triggerAction, new Action<MapButtonLevelObject.EButtonType>(SelectMap));
		}
		if (Hub.s.pdata.main as InTramWaitingScene != null)
		{
			leftButtonAnimator.Play(leftSelectedAnim);
		}
	}

	private void SelectMap(MapButtonLevelObject.EButtonType buttonType)
	{
		InTramWaitingScene inTramWaitingScene = Hub.s.pdata.main as InTramWaitingScene;
		if (inTramWaitingScene != null)
		{
			switch (buttonType)
			{
			case MapButtonLevelObject.EButtonType.Arrow_Left:
				inTramWaitingScene.MapChange(0);
				break;
			case MapButtonLevelObject.EButtonType.Arrow_Right:
				inTramWaitingScene.MapChange(1);
				break;
			}
		}
	}

	public void ChangeSelected(int index)
	{
		switch (index)
		{
		case 0:
			leftButtonAnimator.Play(leftSelectAnim);
			rightButtonAnimator.Play(rightDeselectedAnim);
			break;
		case 1:
			leftButtonAnimator.Play(leftDeselectedAnim);
			rightButtonAnimator.Play(rightSelectAnim);
			break;
		}
	}

	public void OnSelectMap(int index)
	{
		ChangeSelected(index);
		if (buttonMasterAudioKey.Length > 0)
		{
			Hub.s.audioman.PlaySfxAtTransform(buttonMasterAudioKey, base.gameObject.transform);
		}
	}
}
