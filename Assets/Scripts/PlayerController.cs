using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
	public float accelerationPotential = 10000.0f;
	public float maxSpeed = 10.0f;
	public float idleSpeedDropoff = 5000.0f;

	void Start()
	{
		
	}
	
	void FixedUpdate()
	{
		Vector2 acceleration = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
		if (acceleration.magnitude > 1.0f)
		{
			acceleration.Normalize();
		}

		if (acceleration.magnitude > 0.01f)
		{
			//Acceleration is sufficient 
			acceleration *= accelerationPotential;
		}
		else
		{
			//Acceleration is not sufficient, player will be brought to stop

			acceleration = (-rigidbody2D.velocity.normalized / maxSpeed) * idleSpeedDropoff;
			if (acceleration.magnitude * Time.fixedDeltaTime > rigidbody2D.velocity.magnitude)
			{
				acceleration = -rigidbody2D.velocity / Time.fixedDeltaTime;
			}
		}

		rigidbody2D.AddForce(acceleration * Time.fixedDeltaTime, ForceMode2D.Impulse);

		//Enforce speed limit
		if (rigidbody2D.velocity.magnitude > maxSpeed)
		{
			rigidbody2D.velocity = rigidbody2D.velocity.normalized * maxSpeed;
		}
	}
}