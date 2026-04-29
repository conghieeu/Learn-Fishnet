using UnityEngine;
using UnityEngine.UI;

public class UIPrefab_Pedometer_Number : MonoBehaviour
{
	[SerializeField]
	private Image numberImg;

	[SerializeField]
	private Sprite[] numberSprites;

	[SerializeField]
	[Range(0f, 12f)]
	private int _number;

	public void SetNumber(int number)
	{
		if (!(numberImg == null) && numberSprites != null && number >= 0 && number < numberSprites.Length)
		{
			numberImg.sprite = numberSprites[number];
			_number = number;
		}
	}

	public Image GetNumber()
	{
		return numberImg;
	}
}
