using System.Linq;
using Mimic;
using Mimic.Actors;
using UnityEngine;

public class SocketAttachable : MonoBehaviour
{
	private const string animParamFire = "Fire";

	private const string animParamReload = "Reload";

	private const string animParamDetect = "Detect";

	private const string animParamMoveForward = "MoveForward";

	private const string animParamMoveStrafe = "MoveStrafe";

	private const string animParamMoveSpeed = "MoveSpeed";

	[SerializeField]
	protected Animator animator;

	protected ProtoActor owner { get; private set; }

	protected InventoryItem item { get; private set; }

	public void Initialize(ProtoActor owner, InventoryItem item)
	{
		this.owner = owner;
		this.item = item;
	}

	public virtual void OnAttachToSocket()
	{
	}

	public virtual void OnDetachFromSocket()
	{
	}

	public virtual void OnPuppetMove(float forward, float strafe, float speed)
	{
		if (!(animator == null))
		{
			if (HasParameter(animator, "MoveForward"))
			{
				animator.SetFloat("MoveForward", forward);
			}
			if (HasParameter(animator, "MoveStrafe"))
			{
				animator.SetFloat("MoveStrafe", strafe);
			}
			if (HasParameter(animator, "MoveSpeed"))
			{
				animator.SetFloat("MoveSpeed", speed);
			}
		}
	}

	public void OnFire()
	{
		if (!(animator == null) && HasParameter(animator, "Fire"))
		{
			animator.SetTrigger("Fire");
		}
	}

	public void OnReload()
	{
		if (!(animator == null) && HasParameter(animator, "Reload"))
		{
			animator.SetTrigger("Reload");
		}
	}

	public void OnDetect()
	{
		if (!(animator == null) && HasParameter(animator, "Detect"))
		{
			animator.SetTrigger("Detect");
		}
	}

	public static bool HasParameter(Animator animator, string paramName)
	{
		return animator.parameters.Any((AnimatorControllerParameter p) => p.name == paramName);
	}
}
