﻿using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
	public float accelerationPotential = 10000.0f;
	public float maxSpeed = 10.0f;
	public float idleSpeedDropoff = 5000.0f;
	public Crosshair crosshair;
	public GameObject bullet;

	public int health = 1350;

	public int currentGun;
	private Gun[] guns;
	private int timeUntilFire = 0;

	private AnimatedCharacter animatedCharacter;

	void Start()
	{
		guns = new Gun[]{
			new Gun(300.0f, 0.05f, 2.0f, 4, false, false, 25, 1.0f),
			new Gun(150.0f, 0.15f, 1.0f, 1, true, false, 8, 0.7f),
			new Gun(800.0f, 0.0f, 3.0f, 8, false, true, 100, 0.6f)
		};
		SetGun(0);
		animatedCharacter = GetComponentInChildren<AnimatedCharacter>();
	}

	void FixedUpdate()
	{
		health++;
		if (health > 1350) health = 1350;

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
		if (rigidbody2D.velocity.magnitude > maxSpeed * guns[currentGun].speedMultiplier)
		{
			rigidbody2D.velocity = rigidbody2D.velocity.normalized * maxSpeed * guns[currentGun].speedMultiplier;
		}

		animatedCharacter.AddDistance(rigidbody2D.velocity.magnitude);
		animatedCharacter.SetDirection(crosshair.transform.position - transform.position);

		if (rigidbody2D.velocity.magnitude / (maxSpeed * guns[currentGun].speedMultiplier) > 0.5f)
		{
			GameManager.me.AddInterest(transform.position, 20.0f);
		}
	}

	public float GetCrosshairScale()
	{
		return 2.0f + rigidbody2D.velocity.magnitude / (maxSpeed * guns[currentGun].speedMultiplier);
	}

	public Color GetCrosshairColor()
	{
		return new Color(1.0f, 1.0f, 1.0f, (guns[currentGun].autoFire || timeUntilFire <= 0) ? 1.0f : 0.5f);
	}

	void Update()
	{
		for (int i = 0; i < guns.Length; i++)
		{
			if (Input.GetKeyDown((i + 1).ToString()))
			{
				SetGun(i);
			}
		}
		
		float swap = -Input.GetAxis("MouseScrollwheel");
		if (swap > 0.0f)
		{
			SetGun((currentGun + 1) % guns.Length);
		}
		else if (swap < 0.0f)
		{
			SetGun(currentGun <= 0 ? (guns.Length - 1) : (currentGun - 1));
		}

		if (--timeUntilFire <= 0 && ((guns[currentGun].autoFire && Input.GetButton("Fire1")) || (!guns[currentGun].autoFire && Input.GetButtonDown("Fire1"))))
		{
			timeUntilFire = guns[currentGun].fireRate;
			fire();
		}
	}

	private void fire()
	{
		Gun gun = guns[currentGun];

		GameManager.me.AddInterest(transform.position, gun.knockback * 200.0f);

		float nearestDistSqr = 0.0f;
		ZombieController nearestZombie = null;

		for (int i = 0; i < GameManager.zombies.Count; i++)
		{
			ZombieController zombie = GameManager.zombies[i];
			float distSqr = (zombie.transform.position - crosshair.transform.position).sqrMagnitude;
			if (distSqr < nearestDistSqr || nearestZombie == null)
			{
				nearestDistSqr = distSqr;
				nearestZombie = zombie;
			}
		}

		if (nearestZombie != null && nearestDistSqr > 0.25f)
		{
			nearestZombie = null;
		}

		Vector3 bulletTrajectory = (nearestZombie == null ? crosshair.transform.position : nearestZombie.transform.position) - transform.position;
		Vector2 offset = Random.insideUnitCircle * bulletTrajectory.magnitude * gun.imprecision * (1.0f + rigidbody2D.velocity.magnitude / maxSpeed);
		Vector3 normalizedBulletTrajectory = (bulletTrajectory + new Vector3(offset.x, offset.y, 0.0f)).normalized;

		GameObject firedBullet = GameObject.Instantiate(bullet) as GameObject;
		firedBullet.transform.position = new Vector3(transform.position.x, transform.position.y, -1.0f);
		firedBullet.GetComponent<Bullet>().SetProperties(normalizedBulletTrajectory, gun.velocity, gun.damage, gun.piercing, gun.knockback);

		rigidbody2D.AddForce(-normalizedBulletTrajectory * gun.knockback, ForceMode2D.Impulse);
	}

	private void SetGun(int index)
	{
		currentGun = index;
		timeUntilFire = guns[currentGun].fireRate;
	}

	void OnCollisionStay2D(Collision2D collision)
	{
		ZombieController zombie = collision.collider.gameObject.GetComponent<ZombieController>();
		if (zombie != null)
		{
			GameManager.me.AddInterest(transform.position, 10000.0f);
			Damage(30);
		}
	}

	public void Damage(int amount)
	{
		health -= amount;
		if (health < 0) health = 0;
	}

	private class Gun
	{
		public float velocity;
		public float imprecision;
		public float knockback;
		public int damage;
		public bool autoFire;
		public bool piercing;
		public int fireRate;
		public float speedMultiplier;

		public Gun(float velocity, float imprecision, float knockback, int damage, bool autoFire, bool piercing, int fireRate, float speedMultiplier)
		{
			this.velocity = velocity;
			this.imprecision = imprecision;
			this.knockback = knockback;
			this.damage = damage;
			this.autoFire = autoFire;
			this.piercing = piercing;
			this.fireRate = fireRate;
			this.speedMultiplier = speedMultiplier;
		}
	}
}