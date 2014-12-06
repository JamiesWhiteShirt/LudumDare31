using UnityEngine;
using System.Collections;

public class ZombieController : MonoBehaviour
{
	public float accelerationPotential = 10000.0f;
	public float runningMaxSpeed = 1.0f;
	public float chasingMaxSpeed = 3.0f;
	public float loiteringMaxSpeed = 2.0f;
	public LayerMask sightMask;
	public float sightRange = 10.0f;

	private static System.Random random = new System.Random();

	public Waypoint currentWaypoint;
	private Vector3 pathCurve;
	private Waypoint previousWaypoint;
	private Vector2 loiterAround;
	private Vector2 loiterTo;
	private int ticksToNewLoiter = 0;

	private int playerInSight = 0;

	void Start()
	{
		SetWaypoint(currentWaypoint);

		GameManager.zombies.Add(this);
	}
	
	void FixedUpdate()
	{
		Vector3 targetPoint;

		Vector2 distanceFromPlayer = GameManager.me.player.transform.position - transform.position;
		playerInSight--;

		if (distanceFromPlayer.sqrMagnitude <= sightRange * sightRange)
		{
			RaycastHit2D raycastHit = Physics2D.Raycast(transform.position, distanceFromPlayer.normalized, sightRange, sightMask.value);

			if (raycastHit.collider != null && raycastHit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
			{
				playerInSight = 10;
			}
		}

		if (playerInSight == 0)
		{
			SetWaypoint(null);
		}

		if (playerInSight > 0 && currentWaypoint != GameManager.me.playerWaypoint)
		{
			SetWaypoint(GameManager.me.playerWaypoint);
		}
		
		if(currentWaypoint != null && currentWaypoint.IsReached(transform.position))
		{
			SetWaypoint(currentWaypoint.SuggestConnection(previousWaypoint));
		}

		if (currentWaypoint == null && random.Next(50) == 0)
		{
			SetWaypoint(GameManager.me.FindWaypoint(transform.position));
		}

		if (currentWaypoint != null)
		{
			Vector2 delta = currentWaypoint.transform.position - transform.position;
			targetPoint = currentWaypoint.transform.position + delta.magnitude * pathCurve;
		}
		else
		{
			ticksToNewLoiter--;

			if (ticksToNewLoiter < 0)
			{
				ticksToNewLoiter = random.Next(50) + 50;
				FindNewLoiterPosition();
			}

			targetPoint = loiterTo;
		}

		Vector2 distanceFromTargetPoint = targetPoint - transform.position;

		//if (Physics2D.Raycast(transform.position, distanceFromWaypoint.normalized, sightRange, sightMask.value).collider == null)
		{
			Vector2 acceleration = distanceFromTargetPoint.normalized * accelerationPotential;

			if (acceleration.magnitude * Time.fixedDeltaTime / accelerationPotential > distanceFromTargetPoint.magnitude)
			{
				acceleration = -rigidbody2D.velocity;
				transform.position = targetPoint;
			}

			rigidbody2D.AddForce(acceleration * Time.fixedDeltaTime, ForceMode2D.Impulse);
		}

		float currentMaxSpeed = currentWaypoint == null ? loiteringMaxSpeed : (currentWaypoint == GameManager.me.playerWaypoint ? chasingMaxSpeed : runningMaxSpeed);

		//Enforce speed limit
		if (rigidbody2D.velocity.magnitude > currentMaxSpeed)
		{
			rigidbody2D.velocity = rigidbody2D.velocity.normalized * currentMaxSpeed;
		}
	}

	public void SetWaypoint(Waypoint waypoint)
	{
		previousWaypoint = currentWaypoint;
		currentWaypoint = waypoint;
		if (currentWaypoint == null)
		{
			loiterAround = transform.position;
			FindNewLoiterPosition();
		}
		else if(previousWaypoint != waypoint)
		{
			pathCurve = Random.insideUnitCircle * 0.5f;
		}
	}

	public void FindNewLoiterPosition()
	{
		loiterTo = loiterAround + Random.insideUnitCircle;
	}
}