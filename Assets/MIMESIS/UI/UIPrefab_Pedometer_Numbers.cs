using UnityEngine;

public class UIPrefab_Pedometer_Numbers : MonoBehaviour
{
	[SerializeField]
	private UIPrefab_Pedometer_Number[] numbers;

	[SerializeField]
	[Range(0f, 9999f)]
	private int _number;

	public void SetNumber(int number)
	{
		if (numbers != null && numbers.Length != 0)
		{
			int i = 0;
			while (number > 0 && i < numbers.Length)
			{
				int number2 = number % 10;
				numbers[i].SetNumber(number2);
				number /= 10;
				i++;
			}
			for (; i < numbers.Length; i++)
			{
				numbers[i].SetNumber(0);
			}
		}
	}

	public void SetNumberEnabld(bool on)
	{
		if (on)
		{
			SetError();
			return;
		}
		UIPrefab_Pedometer_Number[] array = numbers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetNumber(13);
		}
	}

	public void SetError()
	{
		numbers[4].SetNumber(10);
		numbers[3].SetNumber(11);
		numbers[2].SetNumber(11);
		numbers[1].SetNumber(12);
		numbers[0].SetNumber(11);
	}
}
