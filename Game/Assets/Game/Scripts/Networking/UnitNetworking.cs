﻿using UnityEngine;
using System.Collections;

public class UnitNetworking : MonoBehaviour
{
	private PhotonView _my_photon_view;
	
	// Use this for initialization
	void Start ()
	{
		_my_photon_view = this.gameObject.GetPhotonView();
	}


	public void UpdateUnitPosition()
	{
		// Get focused object
		if(GM.instance.CurrentFocus == this.gameObject)
		{
			// Send RPC call to update unit's position
			_my_photon_view.RPC("UpdatePosition", PhotonTargets.OthersBuffered, this.gameObject.transform.position);	
		}
	}

	[RPC]
	void UpdatePosition(Vector3 position)
	{
		if(GM.instance.IsOn)
		{
			// Get the unit in which to move
			this.gameObject.transform.position = position;
		}
	}


	/*public void UpdateUnitToPlayerContainer(GameObject unit)
	{
		Debug.Log("HERE I AM 1");
		// Get focused object
		if(unit == this.gameObject)
		{
			Debug.Log("HERE I AM 2");
			_my_photon_view.RPC("ParentUnitToCurrentPlayerContainer", PhotonTargets.AllBuffered);	
		}
	}*/

	[RPC]
	void ParentUnitToCurrentPlayerContainer()
	{
		Debug.Log("HERE I AM 3");
		if(GM.instance.IsOn)
		{
			GM.instance.AddUnitToCurrentPlayerContainer(this.gameObject);
		}
	}
}