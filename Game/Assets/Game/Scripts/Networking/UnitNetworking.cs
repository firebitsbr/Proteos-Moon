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
	
	public void UpdateUnitNetwork()
	{
		// Get focused object
		if(GM.instance.CurrentFocus == this.gameObject){
			// Send RPC call to update unit's position
			_my_photon_view.RPC("UpdatePosition", PhotonTargets.OthersBuffered, this.gameObject.transform.position);	
		}
	}
	
	[RPC]
	void UpdatePosition(Vector3 position, PhotonMessageInfo mi)
	{
		if(GM.instance.IsOn)
		{
			// Get the unit in which to move
			//GameObject unit = PhotonView.Find(photon_id).gameObject;
			Debug.Log(string.Format("Sender: {0}  Recv: {1}", mi.sender.name, _my_photon_view.owner.name));
			this.gameObject.transform.position = position;
		}
	}
}