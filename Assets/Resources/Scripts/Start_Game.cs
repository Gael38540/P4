using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class Start_Game : MonoBehaviour {

    public byte Version = 1;
    public bool AutoConnect = true;
    public string m_sRoomName = "P4";
    
	public int m_nJoueur;
	public int m_nNum_Joueur;
	public int[] m_nIdJoueur;


    private bool ConnectInUpdate = true;
    private PhotonView m_PhotonView;

	private P4_Game m_scpP4_Game;

	public GameObject m_goBtn_Partie;

    // Use this for initialization
    void Start () {
    
        DontDestroyOnLoad(this);
        
        m_nJoueur = 0;  
		m_nIdJoueur = new int[4];
		m_nNum_Joueur = 0;
            
        PhotonNetwork.autoJoinLobby = false;
        m_PhotonView = GetComponent <PhotonView>();

    }
    
    // Update is called once per frame
    void Update () {
    
        if (ConnectInUpdate && AutoConnect && !PhotonNetwork.connected) {

            ConnectInUpdate = false;

            PhotonNetwork.ConnectUsingSettings (Version + "." + SceneManagerHelper.ActiveSceneBuildIndex);

        }

    }

    void OnConnectedToMaster()
    {

        RoomOptions roomOptions = new RoomOptions (){ isVisible = true, maxPlayers = 4 };
        PhotonNetwork.JoinOrCreateRoom (m_sRoomName, roomOptions, TypedLobby.Default);

    }

    void OnJoinedRoom(){

		if (PhotonNetwork.isMasterClient) {

			m_nNum_Joueur = 1;
			PhotonNetwork.Instantiate ("Prefabs/Game_P4", Vector3.zero, Quaternion.identity, 0, null);

		}
			
		m_PhotonView.RPC ("Connecter", PhotonTargets.MasterClient);

    }


    [PunRPC]
	void Connecter()
    {

        m_nJoueur++;

		object objNumJr;

		objNumJr = m_nJoueur;

		m_PhotonView.RPC ("Envoi_Num", PhotonTargets.All,objNumJr);

		if(m_nJoueur == 2){
			
			m_goBtn_Partie.SetActive(true);

        }

    }

	[PunRPC]
	void Envoi_Num(object nNumJr)
	{
		if (m_nNum_Joueur == 0)
			m_nNum_Joueur = int.Parse(nNumJr.ToString());
	}

    [PunRPC]
	void Lancer_Partie_Ok(object nNbJr)
    {
        
        SceneManager.LoadScene(1);

		m_scpP4_Game = GameObject.Find("P4_Game").GetComponent <P4_Game>();

		for (int i = 0; i < m_nJoueur; i++) {

			m_scpP4_Game.m_nIdJoueur[i] = m_nIdJoueur [i];

		}

		m_scpP4_Game.m_nNbJoueur_Total = int.Parse(nNbJr.ToString());

		m_scpP4_Game.Init_Scene (m_nNum_Joueur);

    }

	public void Lance_Partie(){

		object nNbJr;
		nNbJr = m_nJoueur;

		m_PhotonView.RPC ("Lancer_Partie_Ok", PhotonTargets.All,nNbJr);

	}

}
