using UnityEngine;
using System.Collections;

public class AnimatedCharacter : MonoBehaviour
{
	public Sprite[] step1 = new Sprite[4];
	public Sprite[] step2 = new Sprite[4];
	
	private SpriteRenderer spriteRenderer;
	private int direction;
	private bool step;
	private float distance;
	
	void Start()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	private void UpdateSprite()
	{
		Sprite[] sprites = step ? step1 : step2;
		spriteRenderer.sprite = sprites[direction];
	}

	public void AddDistance(float f)
	{
		distance += f;
		while (distance > 50.0f)
		{
			Step();
			distance -= 50.0f;
		}
	}

	public void SetDirection(Vector2 vecDirection)
	{
		float angle = Mathf.Atan2(vecDirection.x, vecDirection.y) * Mathf.Rad2Deg;
		int newDirection = (int)Mathf.Round((450.0f - angle) / 90.0f) % 4;
		if (newDirection != direction)
		{
			Step();
		}
		direction = newDirection;

		UpdateSprite();
	}

	public void Step()
	{
		step = !step;

		UpdateSprite();
	}
}