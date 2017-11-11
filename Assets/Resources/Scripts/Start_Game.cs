using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Start_Game : MonoBehaviour {

    public byte Version = 1;
    public bool AutoConnect = true;
    public string m_sRoomName = "P4";
    
    private int m_nJoueur;


    private bool ConnectInUpdate = true;
    private PhotonView m_PhotonView;

    // Use this for initialization
    void Start () {
    
        DontDestroyOnLoad(this);
        
        m_nJoueur = 0;  
            
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

        if(PhotonNetwork.isMasterClient)
            PhotonNetwork.Instantiate("Prefabs/Game",Vector3.zero,Quaternion.identity,0,null);

        m_PhotonView.RPC ("Connecter", PhotonTargets.MasterClient);

    }


    [PunRPC]
    void Connecter()
    {

        if (PhotonNetwork.isMasterClient)
        {

            m_nJoueur++;

            if(m_nJoueur == 2)
                 m_PhotonView.RPC ("Lancer_Partie_Ok", PhotonTargets.All);

        }

    }

    [PunRPC]
    void Lancer_Partie_Ok()
    {
        
        SceneManager.LoadScene(1);

    }
}
