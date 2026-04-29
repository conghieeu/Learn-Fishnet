using UnityEngine;

public class UIButton_ShowNext : MonoBehaviour
{
	public GameObject[] GameObjectsList;

	private int shownGameObjectIndex = -1;

	private void Start()
	{
		for (int i = 0; i < GameObjectsList.Length; i++)
		{
			GameObjectsList[i].SetActive(value: false);
		}
		SelectNextGameObject();
	}

	public void SelectNextGameObject()
	{
		int num = ((shownGameObjectIndex >= GameObjectsList.Length - 1) ? (-1) : shownGameObjectIndex);
		SelectGameObject(num + 1);
	}

	public void SelectPreviousGameObject()
	{
		int num = ((shownGameObjectIndex <= 0) ? GameObjectsList.Length : shownGameObjectIndex);
		SelectGameObject(num - 1);
	}

	public void SelectGameObject(int index)
	{
		if (shownGameObjectIndex >= 0)
		{
			GameObjectsList[shownGameObjectIndex].SetActive(value: false);
		}
		shownGameObjectIndex = index;
		GameObjectsList[shownGameObjectIndex].SetActive(value: true);
	}
}
