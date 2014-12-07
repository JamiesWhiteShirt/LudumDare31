using UnityEngine;
using System.Collections.Generic;

public class Waypoint : MonoBehaviour
{
	public List<Waypoint> connections;
	public float radius = 0.5f;

	private float turnInterest = 1.0f;
	public float incrementalInterest = 0.0f;
	public int users = 0;


	private static System.Random random = new System.Random();

	void Start()
	{
		
	}
	
	void FixedUpdate()
	{
		foreach (Waypoint waypoint in connections)
		{
			waypoint.incrementalInterest += incrementalInterest * 0.01f / connections.Count;
		}

		incrementalInterest *= 0.98f;
		if (incrementalInterest < 0.0f) incrementalInterest = 0.0f;
	}

	public bool IsReached(Vector2 position)
	{
		return (position - new Vector2(transform.position.x, transform.position.y)).magnitude <= radius;
	}

	public void AddInterest(float interest)
	{
		incrementalInterest += interest;
	}

	public Waypoint SuggestConnection(Waypoint previousWaypoint)
	{
		if (connections.Count == 0)
		{
			return null;
		}
		else
		{
			int index = 0;
			float mostInterest = 0.0f;
			Waypoint mostInteresting = null;

			do
			{
				Waypoint waypoint = connections[index];
				float interest = (float)(random.NextDouble() + 1.0) * (waypoint.turnInterest + waypoint.incrementalInterest) / (1 + users);
				if (waypoint == previousWaypoint) interest /= 10.0f;

				if (interest > mostInterest || mostInteresting == null)
				{
					mostInterest = interest;
					mostInteresting = waypoint;
				}

			} while (++index < connections.Count);

			return mostInteresting;
		}
	}
}