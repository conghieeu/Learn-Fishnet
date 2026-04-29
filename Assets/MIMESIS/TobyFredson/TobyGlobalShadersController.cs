using System.Collections.Generic;
using UnityEngine;

namespace TobyFredson
{
	[ExecuteInEditMode]
	public class TobyGlobalShadersController : MonoBehaviour
	{
		[SerializeField]
		private TobyWindType windType;

		[SerializeField]
		[Range(0f, 1f)]
		private float windStrength;

		[SerializeField]
		[Range(1f, 3f)]
		private float windSpeed;

		[Space]
		[SerializeField]
		[Range(-2f, 2f)]
		private float season;

		protected TobyShaderValuesModel cachedAppliedValue;

		private readonly List<Material> matsGrassFoliage = new List<Material>();

		private readonly List<Material> matsTreeBark = new List<Material>();

		private readonly List<Material> matsTreeFoliage = new List<Material>();

		private readonly List<Material> matsTreeBillboard = new List<Material>();

		private readonly List<Material> matsGlobalController = new List<Material>();

		protected void Start()
		{
			RefreshList();
		}

		protected void Update()
		{
			ApplyValues(GetNewValues());
		}

		[ContextMenu("TobyFredson - Refresh")]
		public virtual void Refresh()
		{
			RefreshList();
			ApplyValues(GetNewValues(), isForced: true);
		}

		protected virtual void RefreshList()
		{
			matsGrassFoliage.Clear();
			matsTreeBark.Clear();
			matsTreeFoliage.Clear();
			matsTreeBillboard.Clear();
			matsGlobalController.Clear();
			Renderer[] array = Object.FindObjectsOfType<Renderer>();
			for (int i = 0; i < array.Length; i++)
			{
				Material[] sharedMaterials = array[i].sharedMaterials;
				foreach (Material mat in sharedMaterials)
				{
					RegisterMaterial(mat);
				}
			}
		}

		protected virtual void RegisterMaterial(Material mat)
		{
			if (!(mat == null) && !(mat.shader == null))
			{
				if (mat.shader.name.Equals("Toby Fredson/The Toby Foliage Engine/(TTFE) Grass Foliage"))
				{
					matsGrassFoliage.Add(mat);
				}
				else if (mat.shader.name.Equals("Toby Fredson/The Toby Foliage Engine/(TTFE) Tree Bark"))
				{
					matsTreeBark.Add(mat);
				}
				else if (mat.shader.name.Equals("Toby Fredson/The Toby Foliage Engine/(TTFE) Tree Foliage"))
				{
					matsTreeFoliage.Add(mat);
				}
				else if (mat.shader.name.Equals("Toby Fredson/The Toby Foliage Engine/(TTFE) Tree Billboard"))
				{
					matsTreeBillboard.Add(mat);
				}
				else if (mat.shader.name.Equals("Toby Fredson/The Toby Foliage Engine/Utility/(TTFE) Global Controller"))
				{
					matsGlobalController.Add(mat);
				}
			}
		}

		protected virtual void ApplyValues(TobyShaderValuesModel model, bool isForced = false)
		{
			if ((matsGrassFoliage.Count == 0 && matsTreeBark.Count == 0 && matsTreeFoliage.Count == 0) || (!isForced && cachedAppliedValue.Equals(model)))
			{
				return;
			}
			List<Material> list = new List<Material>();
			list.AddRange(matsTreeFoliage);
			list.AddRange(matsTreeBark);
			list.AddRange(matsGrassFoliage);
			list.AddRange(matsTreeBillboard);
			list.AddRange(matsGlobalController);
			foreach (Material item in list)
			{
				ApplyValuesToMaterial(item, model);
			}
			cachedAppliedValue = model;
		}

		private void ApplyValuesToMaterial(Material mat, TobyShaderValuesModel model)
		{
			SetShaderVariable(mat, "_SeasonChangeGlobal", model.season);
			SetShaderVariable(mat, "_GlobalWindStrength", model.windStrength);
			SetShaderVariable(mat, "_StrongWindSpeed", model.windSpeed);
			switch (model.windType)
			{
			case TobyWindType.GentleBreeze:
				mat.EnableKeyword("_WINDTYPE_GENTLEBREEZE");
				mat.DisableKeyword("_WINDTYPE_WINDOFF");
				break;
			case TobyWindType.WindOff:
				mat.EnableKeyword("_WINDTYPE_WINDOFF");
				mat.DisableKeyword("_WINDTYPE_GENTLEBREEZE");
				break;
			}
		}

		protected void SetShaderVariable(Material mat, string shaderVar, float value)
		{
			mat.SetFloat(shaderVar, value);
		}

		protected void SetShaderToggle(Material mat, string shaderToggle, bool value)
		{
			if (value)
			{
				mat.EnableKeyword(shaderToggle);
			}
			else
			{
				mat.DisableKeyword(shaderToggle);
			}
		}

		protected TobyShaderValuesModel GetNewValues()
		{
			return new TobyShaderValuesModel
			{
				season = season,
				windType = windType,
				windStrength = windStrength,
				windSpeed = windSpeed
			};
		}
	}
}
