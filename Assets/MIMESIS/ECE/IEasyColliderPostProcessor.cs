using UnityEngine;

namespace ECE
{
	public interface IEasyColliderPostProcessor
	{
		void PostProcessCollider(BoxCollider boxCollider, EasyColliderProperties properties);

		void PostProcessCollider(CapsuleCollider capsuleCollider, EasyColliderProperties properties);

		void PostProcessCollider(MeshCollider meshCollider, EasyColliderProperties properties);

		void PostProcessCollider(SphereCollider sphereCollider, EasyColliderProperties properties);
	}
}
