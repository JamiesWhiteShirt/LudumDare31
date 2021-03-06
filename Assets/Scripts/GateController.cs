﻿using UnityEngine;
using System.Collections;

public class GateController : MonoBehaviour
{
	public bool open;
	private int doorsStage;

	public int openingTicks = 100;
	public GameObject doorA;
	private Vector3 originA;
	public GameObject doorB;
	private Vector3 originB;

	void Start()
	{
		originA = doorA.transform.position;
		originB = doorB.transform.position;
	}
	
	void FixedUpdate()
	{
		if (open) doorsStage++;
		else doorsStage--;

		if (doorsStage > openingTicks) doorsStage = openingTicks;
		else if (doorsStage < 0) doorsStage = 0;

		doorA.transform.position = originA + new Vector3(0.0f, 2.0f * doorsStage / openingTicks, 0.0f);
		doorB.transform.position = originB + new Vector3(0.0f, -2.0f * doorsStage / openingTicks, 0.0f);
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject == GameManager.me.player.gameObject)
		{
			GameManager.me.OpenGates();
		}
	}

	public bool isClosed()
	{
		if (open)
		{
			return doorsStage == openingTicks;
		}
		else
		{
			return doorsStage == 0;
		}
	}

	public void Open()
	{
		open = true;
	}

	public void Close()
	{
		open = false;
	}
}