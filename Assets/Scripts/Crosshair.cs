using UnityEngine;
using System.Collections;

public class Crosshair : MonoBehaviour
{
	public Sprite[] sprites;
	private SpriteRenderer spriteRenderer;

	void Start()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	void Update()
	{
		Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		mousePos.z = 0.0f;
		transform.position = mousePos;

		PlayerController player = GameManager.me.player;

		transform.localScale = new Vector3(1.0f, 1.0f, 1.0f) * player.GetCrosshairScale();
		spriteRenderer.color = player.GetCrosshairColor();
		spriteRenderer.sprite = sprites[player.currentGun];
	}
}