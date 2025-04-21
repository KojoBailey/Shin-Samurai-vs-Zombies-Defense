using UnityEngine;

public class FromTheShadowsAttack : MonoBehaviour
{
	public Character target { get; set; }

	private void DeliverAttack()
	{
		if (target != null)
		{
			target.ReceivedAttack(EAttackType.Stealth, (!target.isBoss) ? target.maxHealth : (target.maxHealth * 0.25f));
			base.enabled = false;
		}
	}

	private void Update()
	{
		if (target != null)
		{
			base.transform.position = target.position;
		}
	}
}
