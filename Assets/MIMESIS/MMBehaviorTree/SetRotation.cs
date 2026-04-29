using System;
using System.Globalization;
using UnityEngine;

namespace MMBehaviorTree
{
	public class SetRotation : BehaviorAction
	{
		private Vector3 _rotation;

		public SetRotation(string[] param)
			: base(param)
		{
			if (param.Length != 3)
			{
				throw new ArgumentException("SetRotation needs 3 parameters for rotation");
			}
			if (!float.TryParse(param[0], NumberStyles.Any, CultureInfo.InvariantCulture, out var result) || !float.TryParse(param[1], NumberStyles.Any, CultureInfo.InvariantCulture, out var result2) || !float.TryParse(param[2], NumberStyles.Any, CultureInfo.InvariantCulture, out var result3))
			{
				throw new ArgumentException("Invalid rotation parameters " + param[0] + ", " + param[1] + ", " + param[2]);
			}
			_rotation = new Vector3(result, result2, result3);
		}

		public override IComposite Clone()
		{
			return new SetRotation(new string[3]
			{
				_rotation.x.ToString(CultureInfo.InvariantCulture),
				_rotation.y.ToString(CultureInfo.InvariantCulture),
				_rotation.z.ToString(CultureInfo.InvariantCulture)
			});
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			(state as BehaviorTreeState).Self.SetRotation(_rotation);
			return BehaviorResult.SUCCESS;
		}
	}
}
