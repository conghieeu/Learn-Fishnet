using System.Collections.Generic;
using Bifrost.Cooked;
using UnityEngine;

namespace Mimic.Animation
{
	public class EquipmentSocketDeactivator : StateMachineBehaviour
	{
		[SerializeField]
		[Tooltip("각 AnimatorController 별로 카운터 변수가 필요한데, 이를 위해 사용할 파라미터 이름입니다.")]
		private string counterParameterName = "EquipmentSocketDeactivatorCounter";

		private List<string>? cachedEquipmentSocketNames;

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (TryGetEquipmentSocketNames(out List<string> socketNames) && socketNames != null)
			{
				int integer = animator.GetInteger(counterParameterName);
				if (integer == 0)
				{
					SetActive(animator.transform, socketNames, active: false);
				}
				animator.SetInteger(counterParameterName, integer + 1);
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (TryGetEquipmentSocketNames(out List<string> socketNames) && socketNames != null)
			{
				int integer = animator.GetInteger(counterParameterName);
				if (integer == 1)
				{
					SetActive(animator.transform, socketNames, active: true);
				}
				animator.SetInteger(counterParameterName, integer - 1);
			}
		}

		private bool TryGetEquipmentSocketNames(out List<string>? socketNames)
		{
			if (cachedEquipmentSocketNames != null)
			{
				socketNames = cachedEquipmentSocketNames;
				return true;
			}
			if (Hub.s != null && Hub.s.dataman != null)
			{
				socketNames = new List<string>();
				foreach (ItemMasterInfo value in Hub.s.dataman.ExcelDataManager.ItemInfoDict.Values)
				{
					if (value != null && !(value is ItemMiscellanyInfo { AccessoryGroup: not 0 }) && !socketNames.Contains(value.AttachSocketName))
					{
						socketNames.Add(value.AttachSocketName);
					}
				}
				return true;
			}
			socketNames = null;
			return false;
		}

		private static void SetActive(Transform root, List<string> socketNames, bool active)
		{
			foreach (string socketName in socketNames)
			{
				List<Transform> list = SocketNodeMarker.FindAll(root, socketName);
				if (list == null)
				{
					continue;
				}
				foreach (Transform item in list)
				{
					item.gameObject.SetActive(active);
				}
			}
		}
	}
}
