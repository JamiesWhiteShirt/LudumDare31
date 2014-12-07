using UnityEngine;
using System.Collections;

public class HealthBar : MonoBehaviour
{
	public Sprite[] sprites = new Sprite[6];
	private SpriteRenderer spriteRenderer;

	void Start()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	void Update()
	{
		int index = (int)Mathf.Ceil(GameManager.me.player.health / 300.0f);

		spriteRenderer.sprite = sprites[index];
	}
}