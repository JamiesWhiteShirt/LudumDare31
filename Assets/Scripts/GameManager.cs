using UnityEngine;
using System.Collections.Generic;


public class GameManager : MonoBehaviour
{
	public static GameManager me;

	public PlayerController player;
	public Waypoint playerWaypoint;
	public GateController[] gates;
	public List<Waypoint> waypoints;
	public LayerMask waypointVisibilityMask;
	public static List<ZombieController> zombies = new List<ZombieController>();
	private System.Random random = new System.Random();

	void Start()
	{
		me = this;

		waypoints = new List<Waypoint>(GameObject.FindObjectsOfType<Waypoint>());
		waypoints.Remove(playerWaypoint);

		for (int i = 0; i < waypoints.Count; i++)
		{
			Waypoint a = waypoints[i];
			for (int j = 0; j < waypoints.Count; j++)
			{
				Waypoint b = waypoints[j];

				Vector2 delta = b.transform.position - a.transform.position;
				RaycastHit2D raycastHit = Physics2D.Raycast(transform.position, delta.normalized, delta.magnitude, waypointVisibilityMask.value);

				if (raycastHit.collider == null)
				{
					a.connections.Add(b);
					b.connections.Add(a);
				}
			}
		}
	}

	void FixedUpdate()
	{
		for (int i = 0; i < zombies.Count; i++)
		{
			ZombieController a = zombies[i];
			for (int j = i + 1; j < zombies.Count; j++)
			{
				ZombieController b = zombies[j];

				Vector2 dist = a.transform.position - b.transform.position;

				if (dist.sqrMagnitude < 0.5f)
				{
					Vector2 force = dist.normalized * 100.0f / (0.5f + dist.magnitude);
					a.rigidbody2D.AddForce(force);
					b.rigidbody2D.AddForce(-force);

					int r = random.Next(10);
					if (r == 0 && b.currentWaypoint != null)
					{
						a.SetWaypoint(b.currentWaypoint);
					}
					else if (r == 1 && a.currentWaypoint != null)
					{
						b.SetWaypoint(a.currentWaypoint);
					}
				}
			}
		}
	}

	public void OpenGates()
	{
		foreach (GateController gate in gates)
		{
			gate.Open();
		}
	}

	public Waypoint FindWaypoint(Vector2 position)
	{
		List<Waypoint> sortedWaypoints = new List<Waypoint>(waypoints);
		sortedWaypoints.Sort(delegate(Waypoint a, Waypoint b)
		{
			float af = (transform.position - a.transform.position).magnitude;
			float bf = (transform.position - b.transform.position).magnitude;

			if (af < bf) return -1;
			else if (af > bf) return 1;
			else return 0;
		});

		foreach (Waypoint waypoint in sortedWaypoints)
		{
			Vector2 delta = waypoint.transform.position - transform.position;
			RaycastHit2D raycastHit = Physics2D.Raycast(transform.position, delta.normalized, delta.magnitude, waypointVisibilityMask.value);

			if (raycastHit.collider == null)
			{
				return waypoint;
			}
		}

		return null;
	}
}