using UnityEngine;
using System.Collections;

public class WeaponUI : MonoBehaviour
{
	public Sprite[] iconSprites;

	private SpriteRenderer[] icons;

	void Start()
	{
		icons = new SpriteRenderer[iconSprites.Length];
		for (int i = 0; i < icons.Length; i++)
		{
			GameObject iconObject = new GameObject();
			iconObject.transform.parent = transform;
			iconObject.transform.position = transform.position + new Vector3(i * 2.0f, 0.0f, 0.0f);

			icons[i] = iconObject.AddComponent<SpriteRenderer>();
			icons[i].sortingOrder = 5;
			icons[i].sprite = iconSprites[i];
		}
	}

	void Update()
	{
		for (int i = 0; i < icons.Length; i++)
		{
			icons[i].color = new Color(1.0f, 1.0f, 1.0f, i == GameManager.me.player.currentGun ? 1.0f : 0.25f);
		}
	}
}