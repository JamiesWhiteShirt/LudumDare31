using UnityEngine;
using System.Collections;

public class WeaponUI : MonoBehaviour
{
	public Sprite[] iconSprites;

	private GameObject[] icons;

	void Start()
	{
		icons = new GameObject[iconSprites.Length];
		for (int i = 0; i < icons.Length; i++)
		{
			GameObject icon = new GameObject();
			icon.transform.parent = transform;
			icon.transform.position = transform.position + new Vector3(i * 2.0f, 0.0f, 0.0f);

			icon.AddComponent<SpriteRenderer>().sprite = iconSprites[i];

			icons[i] = icon;
		}
	}

	void Update()
	{
		for (int i = 0; i < icons.Length; i++)
		{
			icons[i].GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, i == GameManager.me.player.currentGun ? 1.0f : 0.25f);
		}
	}
}