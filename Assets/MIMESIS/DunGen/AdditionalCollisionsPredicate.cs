using UnityEngine;

namespace DunGen
{
	public delegate bool AdditionalCollisionsPredicate(Bounds tileBounds, bool isCollidingWithDungeon);
}
