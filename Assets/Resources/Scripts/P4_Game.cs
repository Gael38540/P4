using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class P4_Game : MonoBehaviour {

	struct STPile_Pion_JR{

		public GameObject[] tabgo_Pion;

		public int m_nPion_A_Jouer;

	}

	public int m_nNbJoueur_Total;
	public int[] m_nIdJoueur;

	public GameObject[] m_goBase;
	public GameObject[] m_goPion;

	STPile_Pion_JR[] stPile_JR;

	private bool m_bMaster;
	private bool m_bStart_Level;
	private bool m_bPlay;
	private bool m_bPioche_Pion;

	private int m_nJoueur_EnCours;
	private int m_nSel_Col;
	private int m_nNbJoueur_Reponse;
	private int m_nNbJoueur_MoiMeme;

	private int[,] m_tabCase;

	private GameObject m_goPion_EnCours;
	private PhotonView m_PhotonView;

	void Awake()
	{

		this.name = "P4_Game";
		m_nIdJoueur = new int[4];
		DontDestroyOnLoad(this);

	}

	void Init_Scene()
	{

		m_PhotonView = GetComponent<PhotonView>();

		m_nNbJoueur_MoiMeme = 0;

		for(int i = 0;i < m_nNbJoueur_Total;i++){

			if (m_nIdJoueur[i] == m_PhotonView.viewID)
				m_nNbJoueur_MoiMeme = i;

		}

		m_bMaster = PhotonNetwork.isMasterClient;

		m_bStart_Level = true;
		m_bPlay = false;
		m_bPioche_Pion = false;

		if (m_bMaster) {

			stPile_JR = new STPile_Pion_JR[2];

			m_tabCase = new int[6,7];
			m_nSel_Col = -1;
			m_nJoueur_EnCours = 1;

			m_nNbJoueur_Reponse = 0;

		}

		Init_Pile_Pions (m_nNbJoueur_Total);

		m_PhotonView.RPC ("Init_Partie", PhotonTargets.MasterClient);

	}

	void OnDisable()
	{
		
	}

	// Use this for initialization
	void Start () {

		//test commentaire
		/*
		m_bMaster = false;
		m_bMaster = false;
		m_bPlay = false;
		m_bStart_Level = false;
		m_nSel_Col = -1;
		m_bPioche_Pion = false;
		m_nJoueur_EnCours = 0;
		m_tabCase = new int[6,7];
		*/

	}

	void Init_Pile_Pions(int nNbJoueurs){

		GameObject goPion;
		Vector3 vctPion;
		Quaternion QuatPion;

		for (int i = 1; i <= nNbJoueurs; i++) {

			stPile_JR[(i - 1)].tabgo_Pion = new GameObject[21];
			stPile_JR [(i - 1)].m_nPion_A_Jouer = 20;

			for (int j = 1; j <= 21; j++) {
				
				goPion = Instantiate(m_goPion[(i - 1)],m_goBase[i-1].transform.position,Quaternion.identity) as GameObject;

				goPion.transform.parent = m_goBase [i - 1].transform;

				QuatPion = Quaternion.identity;
				QuatPion.eulerAngles = new Vector3 (-90, 0, 0);

				goPion.transform.rotation = QuatPion;

				goPion.transform.Translate(Vector3.forward * j * 0.1f);

				goPion.name = j.ToString();

				stPile_JR [(i - 1)].tabgo_Pion [(j - 1)] = goPion;

			}

		}

	}

	public void OnClick_Grille(){

		if (m_bPlay) {

			m_bPlay = false;

			StartCoroutine (AffichePion (7));
		
		}

	}

	IEnumerator AffichePion(float rPos){
	
		m_goPion_EnCours.transform.position = new Vector3 (m_nSel_Col, rPos, 0);

		yield return new WaitForSeconds(0.05f);

		rPos -= 0.5f;

		if (rPos >= 1)
			
			StartCoroutine (AffichePion (rPos));
		
		else {


			m_bPlay = true;
			m_bPioche_Pion = false;
		}

	}
	
	// Update is called once per frame
	void Update () {

		if (m_bStart_Level)
		{

			if (m_bPlay)
			{

				if (!m_bPioche_Pion) {

					m_goPion_EnCours = stPile_JR [m_nJoueur_EnCours].tabgo_Pion [stPile_JR [m_nJoueur_EnCours].m_nPion_A_Jouer];

					m_goPion_EnCours.transform.position = new Vector3 (0, 7, 0);
					m_goPion_EnCours.transform.rotation = Quaternion.identity;

					m_bPioche_Pion = true;
					stPile_JR [m_nJoueur_EnCours].m_nPion_A_Jouer--;

				}

				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

				m_nSel_Col = (int)((ray.direction.x + 0.35f) * 10);

				if (m_nSel_Col < 0)
					m_nSel_Col = 0;

				if (m_nSel_Col > 6)
					m_nSel_Col = 6;

				m_nSel_Col -= 3;

				m_goPion_EnCours.transform.position = new Vector3 (m_nSel_Col, 7, 0);


				/*m_Info.text = "A vous de jouer!!\nvous " + m_nPtMoi + " - " + m_nPtLui + " adversaire";
				if (m_nSel_Col != -1)
				{
					m_tabSel[m_nSel_Col].sprite = m_spt_Sel_Off;
				}   

				m_nSel_Col = ((int)Input.mousePosition.x) / (nLargCase + 5);

				if (m_nSel_Col < 0)
					m_nSel_Col = 0;

				if (m_nSel_Col > 6)
					m_nSel_Col = 6;

				m_tabSel[m_nSel_Col].sprite = m_spt_Sel_On;*/

				if (Input.GetMouseButtonDown (0)) {

					OnClick_Grille ();

				}

			}
			else{

				//m_Info.text = "Attente de l'adversaire!!\nvous " + m_nPtMoi + " - " + m_nPtLui + " adversaire";

			}



		}

	}

	[PunRPC]
	void Init_Partie()
	{

		m_nNbJoueur_Reponse++;

		if (m_nNbJoueur_Reponse == m_nNbJoueur_Total) {

			m_PhotonView.RPC ("Lance_Partie", PhotonTargets.MasterClient);

		}
			//Init_Pile_Pions (m_nNbJoueur_Total);

	}

	[PunRPC]
	void Lance_Partie()
	{
		
		m_bStart_Level = true;
		m_bPlay = true;

	}


		
}
