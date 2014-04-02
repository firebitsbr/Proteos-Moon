﻿using UnityEngine;
using System.Collections;

[RequireComponent (typeof(UnitSelected))]
public class BaseClass : MonoBehaviour 
{
	[SerializeField] 
	private BaseStat _base_stat;

	[SerializeField] 
	private Vital _vital;

	[SerializeField] 
	private MovementStat _movement;

	[SerializeField] 
	private UnitStatus _unit_status;

	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public BaseStat base_stat
	{
		get { return _base_stat; }
	}

	public Vital vital
	{
		get { return _vital; }
	}

	public MovementStat movement
	{
		get { return _movement; }
	}

	public UnitStatus unit_status
	{
		get { return _unit_status; }
	}
}
