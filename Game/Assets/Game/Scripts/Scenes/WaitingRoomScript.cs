﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class WaitingRoomScript : Photon.MonoBehaviour {
	
	public GUISkin skin;
	public Texture2D mena_texture, menaBio;
	public Texture2D seita_texture, seitaBio;
	private ProteusChat proteusChat;
	public GameObject mena, seita;
	private GUIStyle header, loading, question, portrait, readyButton, checkmark, leaderInfo;
	private bool leader_chosen = false;
	public bool gameReady = false;
	private bool animatinglabels = false;
	private int leaderClicked = 0;
	private float labelHeight;
	private float startTime;
	private float offSet = 0;
	private float delayTime = 0.0f;
	private bool letsDoThis = false;
	private bool once = true;
	private GameObject leader, otherLeader;
	private string menaSpecialText, seitaSpecialText;
	public int counter;
	public GameObject leftSpawn, rightSpawn, magic;
	
	private string _selected_leader;
	
	void Awake(){
		QuickConnect qc;
		qc = this.GetComponent<QuickConnect>();
		if (qc.enabled == true){
			this.enabled = false;
		}
	}
	// Use this for initialization
	void Start () {
		proteusChat = this.GetComponent<ProteusChat>();
		header = skin.FindStyle("Header");
		loading = skin.FindStyle("Loading");
		question = skin.FindStyle("Question");
		portrait = skin.FindStyle ("Portrait");
		readyButton = skin.FindStyle("ReadyButton");
		checkmark = skin.FindStyle("Checkmark");
		leaderInfo = skin.FindStyle("LeaderInfo");
		startTime = 0.0f;
		counter = 0;
		menaSpecialText = "\n\n\n\n\t\t\t\t\t\t\t\tSpecial Attack:\n\t\t\t\t\t\t\t\t\t\tEwigen Nacht\n\n\t\tCaptain Mena is the sister of the Altier Seita. A living legend among the Saren Military, she has a reputation for never missing a shot. Over the years she alone ended wars by firing a single bullet over great distances to rout enemy commanders.\n\t\tBorn in the town of Galleone, she was orphaneddue to war and was adopted by Count Emmen, where she earned the nickname,\n\t\t\t\t\"Bellflower of Galleone\"\nbecause they only grew in the places she left in her wake.";
		seitaSpecialText = "\n\n\n\n\t\t\t\t\t\t\t\tSpecial Attack:\n\t\t\t\t\t\t\t\t\t\tFall of the Altier\n\n\t\tBrother to Captain Mena, the Altier is considered a sacred weapon used by the Ralsian People. Seita is one of the only Sarens to ever be chosen by the pendulum hammer.\n\n\t\tA stalwart and compassionate person, he seeks to restore balance between the Ralsian and Saren people. Imbued with the secrets of the Ralse, he has phenomenal strength and unearthly power.";

		proteusChat.photonView.RPC ("GameChat", PhotonTargets.All, "Joined");
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Backspace)){
			PhotonNetwork.LeaveRoom();
		}
		startTime += Time.deltaTime;
		if (startTime >= 0.5f){
			startTime -= 0.5f;
			if (counter == 3)
				counter = 0;
			else
				counter++;
		}
		if (PhotonNetwork.playerList.Length == 2 && leader_chosen && gameReady){
			delayTime += Time.deltaTime;
			if (delayTime >= 3.0f){
				letsDoThis = true;
			}
		}
	}

	void OnLeftRoom(){
		PhotonNetwork.LoadLevel(1);
	}

	void OnGUI(){
		if (!Application.CanStreamedLevelBeLoaded(3) ||  !Application.CanStreamedLevelBeLoaded(2) || Application.GetStreamProgressForLevel(2) < 1 || Application.GetStreamProgressForLevel(3) < 1)
		{
			GUI.skin = skin; 
			PreLoadingGUI();
			return;
		}
		GUI.skin = skin;
		if (PhotonNetwork.playerList.Length <= 1)
			WaitingForOtherPlayer();
		else if (PhotonNetwork.playerList.Length == 2 && !leader_chosen)
			MainGUI();
		else if (animatinglabels){
			AnimateLabels();
			if (labelHeight == 35.0f)
				animatinglabels = false;
		}
		else if (PhotonNetwork.playerList.Length == 2 && leader_chosen && !gameReady){
			ReadyGUI();
		}
		else{
			if (once){
				Instantiate(magic, rightSpawn.transform.position, Quaternion.identity);
				otherLeader = Instantiate((PhotonNetwork.otherPlayers[0].customProperties["Leader"].ToString() == "Altier_Seita"?seita:mena), 
				                          rightSpawn.transform.position, rightSpawn.transform.rotation) as GameObject;
				once = false;
			}
			LoadingGUI();
			if (letsDoThis){
				Destroy(leader);
				Destroy(otherLeader);
				PhotonNetwork.LoadLevel(Application.loadedLevel + 1);
			}
		}
	}
	
	void WaitingForOtherPlayer(){
		switch (counter){
		case 0: GUI.Label(new Rect(Screen.width / 2 - 125, Screen.height / 2 - 25, 256, 50), "Waiting For Other Player...", loading);
			break;
		case 1:	GUI.Label(new Rect(Screen.width / 2 - 125, Screen.height / 2 - 25, 256, 50), "Waiting For Other Player", loading);
			break;
		case 2:	GUI.Label(new Rect(Screen.width / 2 - 125, Screen.height / 2 - 25, 256, 50), "Waiting For Other Player.", loading);
			break;
		case 3:	GUI.Label(new Rect(Screen.width / 2 - 125, Screen.height / 2 - 25, 256, 50), "Waiting For Other Player..", loading);
			break;
		default:
			Debug.LogError("The counter in WaitingForOtherPlayer went out of bounds");
			break;
		}
	}
	
	void MainGUI(){
		GUI.Label(new Rect(100, 30, Screen.width - 200, 50), "Choose Your Leader", header);
		GUI.Label(new Rect(Screen.width / 2 - (256 + 100), Screen.height / 2 + 50, 256, 50), PhotonNetwork.playerName, loading);
		GUI.Label(new Rect(Screen.width / 2 - 25, Screen.height / 2 + 55, 50, 50), "VS", header);
		if (PhotonNetwork.otherPlayers.Length != 0)
			GUI.Label(new Rect(Screen.width / 2 + 100, Screen.height / 2 + 50, 256, 50), PhotonNetwork.otherPlayers[0].name, loading);
		if (leaderClicked == 0){
			GUI.Box(new Rect(Screen.width / 2 - (256 + 100), Screen.height / 2 - 256, 256, 256), "?", question);
		}
		if (!gameReady){
			GUI.Box(new Rect(Screen.width / 2 + 100, Screen.height / 2 - 256, 256, 256), "?", question);
		}
		else{
			GUI.Box(new Rect(Screen.width / 2 + 100, Screen.height / 2 - 256, 256, 256), "\u2714", checkmark);
		}
		if(GUI.Button(new Rect(Screen.width / 2 - (256 + 105), Screen.height / 2 + 100, 256 / 2, 256 / 2), mena_texture, portrait)){
			leaderClicked = 1;
			offSet = 0.0f;
		}
		
		if(GUI.Button(new Rect((Screen.width / 2 - (256 + 95)) + (256 / 2), Screen.height / 2  + 100, 256 / 2, 256 / 2), seita_texture, portrait)){
			leaderClicked = 2;
			offSet = 138;
		}
		if (leaderClicked != 0){
			if(GUI.Button(new Rect(Screen.width / 2 - (256 + 105) + offSet, Screen.height / 2 + 240, 128, 50), "Ready?", readyButton)){
				animatinglabels = true;
				proteusChat.photonView.RPC("GameChat", PhotonTargets.All, "Ready");
					
				ExitGames.Client.Photon.Hashtable player_props = new ExitGames.Client.Photon.Hashtable();
				player_props.Add("Leader", _selected_leader);
				PhotonNetwork.player.SetCustomProperties(player_props);
					
				this.photonView.RPC("ActivateOtherPlayer", PhotonTargets.Others);
				Instantiate(magic, leftSpawn.transform.position, Quaternion.identity);
				if (leaderClicked == 1){
					//mena.SetActive(true);
					leader = Instantiate(mena, leftSpawn.transform.position, leftSpawn.transform.rotation) as GameObject;
				}
				else if (leaderClicked == 2){
					//seita.SetActive(true);
					leader = Instantiate(seita, leftSpawn.transform.position, leftSpawn.transform.rotation) as GameObject;
				}
				leader_chosen = true;
			}
			if (leaderClicked == 1){
				leaderInfo.normal.background = menaBio;
				GUI.Box(new Rect(Screen.width / 2 - (256 + 100), Screen.height / 2 - 256, 256, 256), "?", question);
				GUI.Box(new Rect(Screen.width / 2 - (256 + 100 + 330), Screen.height / 2 - 256, 300, 470), menaSpecialText, leaderInfo);
				_selected_leader = "Captain_Mena";
			}
			if (leaderClicked == 2){
				leaderInfo.normal.background = seitaBio;
				GUI.Box(new Rect(Screen.width / 2 - (256 + 100 + 330), Screen.height / 2 - 256, 300, 470), seitaSpecialText, leaderInfo);
				GUI.Box(new Rect(Screen.width / 2 - (256 + 100), Screen.height / 2 - 256, 256, 256), "?", question);
				_selected_leader = "Altier_Seita";
			}
		}
	}
	
	void ReadyGUI(){
		GUI.Label(new Rect(Screen.width / 2 - (256 + 100), 30, 256, 50), PhotonNetwork.playerName, loading);
		GUI.Label(new Rect(Screen.width / 2 - 25, 35, 50, 50), "VS", header);
		if (PhotonNetwork.otherPlayers.Length != 0)
			GUI.Label(new Rect(Screen.width / 2 + 100, 30, 256, 50), PhotonNetwork.otherPlayers[0].name, loading);
		
		WaitingForOtherPlayer();
	}
	
	void LoadingGUI()
	{
		GUI.Label(new Rect(Screen.width / 2 - 128, Screen.height / 2 - 25, 256, 50), "Loading: " + (int)(Application.GetStreamProgressForLevel(3) * 100) + "%", loading);
	}
	
	void PreLoadingGUI(){
		GUI.Label(new Rect(Screen.width / 2 - 128, Screen.height / 2 - 25, 256, 50), "Loading: " + (int)(Application.GetStreamProgressForLevel(2) * 100) + "%", loading);
	}
	
	void AnimateLabels(){
		labelHeight = Mathf.Lerp((Screen.height / 2 + 50), 35, (Time.time - startTime) / 3.5f);
		GUI.Label(new Rect(Screen.width / 2 - (256 + 100), labelHeight, 256, 50), PhotonNetwork.playerName, loading);
		GUI.Label(new Rect(Screen.width / 2 - 25, (labelHeight+5), 50, 50), "VS", header);
		if (PhotonNetwork.otherPlayers.Length != 0)
			GUI.Label(new Rect(Screen.width / 2 + 100, labelHeight, 256, 50), PhotonNetwork.otherPlayers[0].name, loading);
	}
	
	[RPC]
	void ActivateOtherPlayer(PhotonMessageInfo mi)
	{
		gameReady = true;
	}
}
