﻿using UnityEngine;
using Random = UnityEngine.Random;


public class LoginScript : MonoBehaviour
{
	public Vector2 guiSize = new Vector2(350, 200);
	private string playerName = "";
	private Rect guiCenteredRect;
	//public Texture tex;
	public GUISkin loginSkin;
	public MonoBehaviour componentToEnable;
	private string loginText = "Please Log In";
	private char[] arr = new char[] { '\n', ' ' };
	
	
	public void Awake()
	{
		this.guiCenteredRect = new Rect(Screen.width/2-guiSize.x/2, Screen.height/2-100, guiSize.x, guiSize.y);
		playerName = "";
		if (this.componentToEnable == null || this.componentToEnable.enabled)
		{
			Debug.LogError("To use the Login, the ComponentToEnable should be defined in inspector and disabled initially.");
		}
		playerName = PlayerPrefs.GetString("playername");
		if (string.IsNullOrEmpty(playerName))
		{
			PhotonNetwork.playerName = "Guest" + Random.Range(1, 9999);
			playerName = PhotonNetwork.playerName;
		}
	}
	
	public void OnGUI()
	{
		// Enter-Key handling:
		if (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.KeypadEnter || Event.current.keyCode == KeyCode.Return))
		{
			if (!string.IsNullOrEmpty(playerName))
			{
				this.ConnectToLobby();
				return;
			}
		}
		GUI.skin.label.wordWrap = true;
		GUILayout.BeginArea(guiCenteredRect);
		//GUILayout.Box(tex);
		//GUI.skin = loginSkin;
		GUILayout.Box(this.loginText);
		
		GUILayout.BeginHorizontal();
		GUI.SetNextControlName("NameInput");
		playerName = GUILayout.TextField(playerName, 20);
		GUILayout.EndHorizontal();
		playerName = playerName.TrimStart(arr);
		playerName = playerName.TrimEnd(arr);
		PhotonNetwork.playerName = playerName;
		/*if (GUI.changed)
		{
			// Save name
			PlayerPrefs.SetString("playername", PhotonNetwork.playerName);
		}*/
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Connect"))
		{
			this.ConnectToLobby();
		}
		GUI.FocusControl("NameInput");
		if (GUILayout.Button("Back To Main Menu"))
		{
			PhotonNetwork.LoadLevel("TitleScene");
		}
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
	
	private void ConnectToLobby()
	{
		PhotonNetwork.playerName = playerName;
		PlayerPrefs.SetString ("playername", PhotonNetwork.playerName);
		this.componentToEnable.enabled = true;
		this.enabled = false;
	}
}

