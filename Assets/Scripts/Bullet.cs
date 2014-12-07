using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
	public LayerMask hitMask;
	private int killMe = 20;

	private Vector3 angle;
	private float velocity;
	private int damage;
	private bool piercing;
	private float knockback;

	public void SetProperties(Vector2 angle, float velocity, int damage, bool piercing, float knockback)
	{
		this.angle = angle;
		this.velocity = velocity;
		this.damage = damage;
		this.piercing = piercing;
		this.knockback = knockback;
	}

	void FixedUpdate ()
	{
		if (killMe-- <= 0)
		{
			Destroy(gameObject);
			return;
		}

		RaycastHit2D[] hits;

		if (piercing)
		{
			hits = Physics2D.RaycastAll(transform.position, angle, velocity * Time.fixedDeltaTime, hitMask.value);
		}
		else
		{
			hits = new RaycastHit2D[]{ Physics2D.Raycast(transform.position, angle, velocity * Time.fixedDeltaTime, hitMask.value) };
		}

		Vector3 newPos = transform.position + angle * velocity * Time.fixedDeltaTime;

		foreach (RaycastHit2D raycastHit in hits)
		{
			if (raycastHit.collider != null)
			{
				bool isDestroyed = false;

				ZombieController zombie = raycastHit.collider.gameObject.GetComponent<ZombieController>();
				if (zombie != null)
				{
					zombie.Damage(damage, angle * knockback * 4.0f);
					isDestroyed = !piercing;
				}
				else
				{
					isDestroyed = true;
				}

				if (isDestroyed)
				{
					newPos = raycastHit.point;
					killMe = 0;
					break;
				}
			}
		}

		transform.position = newPos;
	}
}