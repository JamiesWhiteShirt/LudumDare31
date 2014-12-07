using UnityEngine;
using System.Collections;

public class OverlayController : MonoBehaviour
{
	private bool touched = false;
	private int transparency = 0;

	public SpriteRenderer showOnTouch;
	public SpriteRenderer hideOnTouch;

	void FixedUpdate()
	{
		if (touched) transparency++;
		else transparency--;

		if (transparency > 16)
		{
			transparency = 16;
		}
		else if (transparency < 0)
		{
			transparency = 0;
		}

		showOnTouch.color = new Color(1.0f, 1.0f, 1.0f, transparency / 16.0f);
		hideOnTouch.color = new Color(1.0f, 1.0f, 1.0f, (16 - transparency) / 16.0f);
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject == GameManager.me.player.gameObject)
		{
			touched = true;
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject == GameManager.me.player.gameObject)
		{
			touched = false;
		}
	}
}