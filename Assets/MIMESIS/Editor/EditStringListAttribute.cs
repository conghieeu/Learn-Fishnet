using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class EditStringListAttribute : PropertyAttribute
{
	public char Separator { get; private set; }

	public EditStringListAttribute(char separator = ',')
	{
		Separator = separator;
	}
}
