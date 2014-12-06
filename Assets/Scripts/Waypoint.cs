using UnityEngine;
using System.Collections.Generic;

public class Waypoint : MonoBehaviour
{
	public List<Waypoint> connections;
	public float radius = 0.5f;
	public int returnChance = 10;

	private static System.Random random = new System.Random();

	void Start()
	{
		
	}
	
	void Update()
	{
		
	}

	public bool IsReached(Vector2 position)
	{
		return (position - new Vector2(transform.position.x, transform.position.y)).magnitude <= radius;
	}

	public Waypoint SuggestConnection(Waypoint previousWaypoint)
	{
		if (connections == null || connections.Count == 0)
		{
			return null;
		}
		else
		{
			while (true)
			{
				int index = random.Next(connections.Count);

				if (connections[index] != previousWaypoint || random.Next(returnChance) == 0)
				{
					return connections[index];
				}
			}
		}
	}
}