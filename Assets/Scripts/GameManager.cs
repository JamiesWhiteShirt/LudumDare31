using UnityEngine;
using System.Collections.Generic;


public class GameManager : MonoBehaviour
{
	public static GameManager me;

	public float arenaWidth;
	public float arenaHeight;
	public WaveManager waveManager;
	public PlayerController player;
	public Waypoint playerWaypoint;
	public GateController[] gates;
	public GameObject waypointContainer;
	public List<Waypoint> waypoints;
	public LayerMask waypointVisibilityMask;
	public Waypoint waypointL;
	public Waypoint waypointR;
	public static List<ZombieController> zombies = new List<ZombieController>();
	private System.Random random = new System.Random();

	void Start()
	{
		me = this;

		waypoints = new List<Waypoint>(waypointContainer.GetComponentsInChildren<Waypoint>());

		for (int i = 0; i < waypoints.Count; i++)
		{
			Waypoint a = waypoints[i];
			for (int j = i + 1; j < waypoints.Count; j++)
			{
				Waypoint b = waypoints[j];

				Vector2 delta = b.transform.position - a.transform.position;

				if (delta.magnitude <= 10.0f)
				{
					RaycastHit2D raycastHit = Physics2D.Raycast(a.transform.position, delta.normalized, delta.magnitude, waypointVisibilityMask.value);

					if (raycastHit.collider == null)
					{
						a.connections.Add(b);
						b.connections.Add(a);
					}
				}
			}
		}
	}

	void FixedUpdate()
	{
		bool canCloseGates = true;

		for (int i = 0; i < zombies.Count; i++)
		{
			ZombieController a = zombies[i];
			for (int j = i + 1; j < zombies.Count; j++)
			{
				ZombieController b = zombies[j];

				Vector2 dist = a.transform.position - b.transform.position;

				if (dist.sqrMagnitude < 0.5f)
				{
					Vector2 force = dist.normalized * 50.0f / (0.5f + dist.magnitude);
					a.rigidbody2D.AddForce(force);
					b.rigidbody2D.AddForce(-force);

					/*int r = random.Next(20);
					if (r == 0 && a.CanSee(b.currentWaypoint))
					{
						a.SetWaypoint(b.currentWaypoint);
					}
					else if (r == 1 && b.CanSee(a.currentWaypoint))
					{
						b.SetWaypoint(a.currentWaypoint);
					}
					else if (r == 2)
					{
						a.SetWaypoint(FindWaypoint(a.transform.position));
						b.SetWaypoint(FindWaypoint(b.transform.position));
					}*/
				}
			}

			if (a.transform.position.x < -arenaWidth / 2.0f + 3.0f)
			{
				canCloseGates = false;
				/*if (waveManager.currentWave.HasBegun() && (a.currentWaypoint == null || (a.currentWaypoint != waypointL && !a.CanSee(a.currentWaypoint))))
				{
					a.SetWaypoint(waypointL);
				}*/
			}
			else if (a.transform.position.x > arenaWidth / 2.0f - 3.0f)
			{
				canCloseGates = false;
			}
		}

		if (canCloseGates)
		{
			CloseGates();
		}

		if (zombies.Count == 0 && AreGatesClosed() && waveManager.HasWaveFinishedSpawning())
		{
			waveManager.NextWave();
		}
	}

	public bool AreGatesClosed()
	{
		foreach (GateController gate in gates)
		{
			if (!gate.isClosed()) return false;
		}
		return true;
	}

	public void OpenGates()
	{
		foreach (GateController gate in gates)
		{
			gate.Open();
		}

		waveManager.currentWave.Begin();
	}

	public void CloseGates()
	{
		foreach (GateController gate in gates)
		{
			gate.Close();
		}
	}

	public Waypoint FindWaypoint(Vector2 position)
	{
		List<Waypoint> sortedWaypoints = new List<Waypoint>();

		if (waveManager.currentWave.HasBegun())
		{
			sortedWaypoints.Add(waypointL);
			sortedWaypoints.Add(waypointR);
			sortedWaypoints.AddRange(waypoints);
		}
		
		sortedWaypoints.Sort(delegate(Waypoint a, Waypoint b)
		{
			float af = (transform.position - a.transform.position).magnitude;
			float bf = (transform.position - b.transform.position).magnitude;

			if (af > bf) return -1;
			else if (af < bf) return 1;
			else return 0;
		});

		foreach (Waypoint waypoint in sortedWaypoints)
		{
			Vector2 delta = new Vector2(waypoint.transform.position.x, waypoint.transform.position.y) - position;
			RaycastHit2D raycastHit = Physics2D.Raycast(position, delta.normalized, delta.magnitude, waypointVisibilityMask.value);

			if (raycastHit.collider == null)
			{
				return waypoint;
			}
		}

		return null;
	}

	public void AddInterest(Vector2 origin, float interest)
	{
		List<Waypoint> visibleWaypoints = new List<Waypoint>();
		float totalDistance = 0.0f;

		foreach (Waypoint waypoint in waypoints)
		{
			Vector2 delta = new Vector2(waypoint.transform.position.x, waypoint.transform.position.y) - origin;
			if (delta.magnitude < 6.0f)
			{
				RaycastHit2D raycastHit = Physics2D.Raycast(origin, delta.normalized, delta.magnitude, waypointVisibilityMask.value);

				if (raycastHit.collider == null)
				{
					visibleWaypoints.Add(waypoint);
					totalDistance += (waypoint.transform.position - new Vector3(origin.x, origin.y, 0.0f)).magnitude;
				}
			}
		}

		foreach (Waypoint waypoint in visibleWaypoints)
		{
			float distance = (waypoint.transform.position - new Vector3(origin.x, origin.y, 0.0f)).magnitude;
			waypoint.AddInterest(interest * distance / totalDistance);
		}
	}

	public void RemoveZombie(ZombieController zombie)
	{
		zombies.Remove(zombie);
	}
}