using UnityEngine;
using System.Collections;

public class WaveManager : MonoBehaviour
{
	public GameObject spawnedZombie;
	public static WaveManager me;

	public class Wave
	{
		private static System.Random random = new System.Random();

		public int number;
		public int duration = -1;
		public int amountTotal;
		public int amountToSpawn;

		public Wave(int number)
		{
			this.number = number;
			amountTotal = 20 * (2 + number);
			amountToSpawn = amountTotal;
		}

		public void Initialize()
		{
			bool side = false;
			for (int i = 0; i < amountTotal; i++)
			{
				amountToSpawn--;

				float y = (float)random.NextDouble() * me.arenaHeight - me.arenaHeight / 2.0f;
				float s = (side = !side) ? 1.0f : -1.0f;
				float x = s * (me.arenaWidth * 0.5f - 1.0f);

				GameObject o = Instantiate(me.spawnedZombie, new Vector3(x + s * (2.0f + (float)random.NextDouble() * 5.0f), y, 0.0f), Quaternion.identity) as GameObject;

				ZombieController zombie = o.GetComponent<ZombieController>();
				zombie.SetProperties(1.0f);
				zombie.SetLoiterAround(new Vector2(x, y));
			}
		}

		public void Begin()
		{
			duration = 0;
		}

		public void Update()
		{
			if(HasBegun()) duration++;
		}

		public bool HasBegun()
		{
			return duration != -1;
		}
	}

	public int currentWaveNum;
	public Wave currentWave;
	public float arenaWidth;
	public float arenaHeight;

	void Start ()
	{
		me = this;
		NextWave();
	}

	public void NextWave()
	{
		currentWave = new Wave(currentWave == null ? 0 : (currentWave.number + 1));
		currentWave.Initialize();
		currentWaveNum = currentWave.number;
	}
	
	void FixedUpdate ()
	{
		currentWave.Update();
	}

	public bool HasWaveFinishedSpawning()
	{
		return currentWave.amountToSpawn == 0;
	}
}