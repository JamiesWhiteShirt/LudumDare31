using UnityEngine;
using System.Collections;

public class ZombieController : MonoBehaviour
{
	public int health = 4;

	public float accelerationPotential = 10000.0f;
	public float runningMaxSpeed = 1.0f;
	public float chasingMaxSpeed = 3.0f;
	public float loiteringMaxSpeed = 2.0f;
	public LayerMask sightMask;
	public float sightRange = 10.0f;
	public int maxIdleness = 10;

	private static System.Random random = new System.Random();

	public Waypoint currentWaypoint;
	private Vector3 pathCurve;
	private Waypoint previousWaypoint;
	public int idleness = 0;

	private Vector2 loiterAround;
	private Vector2 loiterTo;
	private int ticksToNewLoiter = 0;

	public int playerInSight = 0;

	void Start()
	{
		SetWaypoint(currentWaypoint);

		GameManager.zombies.Add(this);
	}
	
	void FixedUpdate()
	{
		Vector3 targetPoint;

		HandleIdleness();

		HandlePlayerSight();
		
		if(currentWaypoint != null && currentWaypoint.IsReached(transform.position))
		{
			SetWaypoint(currentWaypoint.SuggestConnection(previousWaypoint));
		}

		if (currentWaypoint == null && random.Next(100) == 0)
		{
			SetWaypoint(GameManager.me.FindWaypoint(transform.position));
		}

		if (currentWaypoint != null)
		{
			//Zombie will head for waypoint
			Vector2 delta = currentWaypoint.transform.position - transform.position;
			targetPoint = currentWaypoint.transform.position + delta.sqrMagnitude * pathCurve;
		}
		else
		{
			//Zombie will loiter
			ticksToNewLoiter--;

			if (ticksToNewLoiter < 0)
			{
				ticksToNewLoiter = random.Next(50) + 50;
				FindNewLoiterPosition();
			}

			targetPoint = loiterTo;
		}

		Vector2 distanceFromTargetPoint = targetPoint - transform.position;

		Vector2 acceleration = distanceFromTargetPoint.normalized * accelerationPotential;

		if (acceleration.magnitude * Time.fixedDeltaTime / accelerationPotential > distanceFromTargetPoint.magnitude)
		{
			acceleration = -rigidbody2D.velocity;
			transform.position = targetPoint;
		}

		rigidbody2D.AddForce(acceleration * Time.fixedDeltaTime, ForceMode2D.Impulse);

		float currentMaxSpeed = currentWaypoint == null ? loiteringMaxSpeed : (currentWaypoint == GameManager.me.playerWaypoint ? chasingMaxSpeed : runningMaxSpeed);

		//Enforce speed limit
		if (rigidbody2D.velocity.magnitude > currentMaxSpeed)
		{
			rigidbody2D.velocity = rigidbody2D.velocity.normalized * currentMaxSpeed;
		}
	}

	public void SetWaypoint(Waypoint waypoint)
	{
		if (previousWaypoint != null)
		{
			previousWaypoint.users--;
		}
		if (waypoint != null)
		{
			waypoint.users++;
		}

		previousWaypoint = currentWaypoint;
		currentWaypoint = waypoint;
		if (currentWaypoint == null)
		{
			loiterAround = transform.position;
			FindNewLoiterPosition();
		}
		else if(previousWaypoint != waypoint)
		{
			if (waypoint == GameManager.me.playerWaypoint)
			{
				pathCurve = new Vector3(0.0f, 0.0f, 0.0f);
			}
			else
			{
				pathCurve = Random.insideUnitCircle * 0.5f / (waypoint.transform.position - transform.position).magnitude;
			}
		}
	}

	public void FindNewLoiterPosition()
	{
		loiterTo = loiterAround + Random.insideUnitCircle;
	}

	private void HandlePlayerSight()
	{
		if (CanSee(GameManager.me.playerWaypoint))
		{
			playerInSight++;
		}
		else
		{
			playerInSight--;
			if (playerInSight == 25)
			{
				SetWaypoint(null);
			}
		}

		if (playerInSight > 50)
		{
			playerInSight = 50;
		}
		else if (playerInSight < 0)
		{
			playerInSight = 0;
		}


		if (playerInSight > 25 && currentWaypoint != GameManager.me.playerWaypoint)
		{
			SetWaypoint(GameManager.me.playerWaypoint);
			GameManager.me.AddInterest(currentWaypoint.transform.position, 100.0f);
		}
	}

	public bool CanSee(Waypoint waypoint)
	{
		if (waypoint == null) return false;

		Vector2 delta = waypoint.transform.position - transform.position;
		bool isPlayer = GameManager.me.playerWaypoint;

		if (isPlayer && delta.sqrMagnitude > sightRange * sightRange)
		{
			return false;
		}

		RaycastHit2D raycastHit = Physics2D.Raycast(transform.position, delta.normalized, sightRange, sightMask.value);

		if(isPlayer)
		{
			return raycastHit.collider != null && raycastHit.collider.gameObject.layer == LayerMask.NameToLayer("Player");
		}
		else
		{
			return raycastHit.collider == null;
		}
	}

	private void HandleIdleness()
	{
		if (currentWaypoint != null && rigidbody2D.velocity.magnitude < 0.5f)
		{
			idleness++;
		}
		else
		{
			idleness = 0;
		}

		if (idleness > maxIdleness)
		{
			Waypoint w = GameManager.me.FindWaypoint(transform.position);

			Debug.Log("Silly me");
			SetWaypoint(w);
		}
	}

	public void Damage(int amount, Vector2 impact)
	{
		rigidbody2D.AddForce(impact, ForceMode2D.Impulse);

		health -= amount;

		if (health <= 0)
		{
			GameManager.me.AddInterest(transform.position, 40.0f);
			Destroy(gameObject);
			GameManager.zombies.Remove(this);
		}
	}
}