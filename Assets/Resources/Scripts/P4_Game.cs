using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
	public GameObject[] m_goScore;
	public Mesh[] m_goScoreChiffres;
	public GameObject m_goPionBois;

	STPile_Pion_JR[] stPile_JR;

	private bool m_bMaster;
	private bool m_bStart_Level;
	private bool m_bPlay;
	private bool m_bPioche_Pion;

	private int m_nJoueur_EnCours;
	private int m_nSel_Col;
	private int m_nNbJoueur_Reponse;
	private int m_nNum_Joueur;

	private int[] m_tabScore;

	private int[,] m_tabCase;

	private GameObject m_goPion_EnCours;
	private PhotonView m_PhotonView;

	void Awake()
	{

		DontDestroyOnLoad(this);

		this.name = "P4_Game";
		m_nIdJoueur = new int[4];
		m_goScore = new GameObject[4];
		m_tabScore = new int[4];



		//a supprimer
		//m_nNbJoueur_Total = 2;
		//Init_Scene (1);

	}

	public void Init_Scene(int nNumJr)
	{

		m_nNum_Joueur = nNumJr;

		m_PhotonView = GetComponent<PhotonView>();

		/*m_nNbJoueur_MoiMeme = 0;

		for(int i = 0;i < m_nNbJoueur_Total;i++){

			if (m_nIdJoueur[i] == m_PhotonView.viewID)
				m_nNbJoueur_MoiMeme = i;

		}*/

		m_bMaster = PhotonNetwork.isMasterClient;

		m_bStart_Level = true;
		m_bPlay = false;
		m_bPioche_Pion = false;

		stPile_JR = new STPile_Pion_JR[m_nNbJoueur_Total];


		m_nSel_Col = -1;

		if (m_bMaster) {

			m_nJoueur_EnCours = 1;

			m_nNbJoueur_Reponse = 0;

		}

		Init_Pile_Pions (m_nNbJoueur_Total,false);

		//a remettre
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
		
	void Init_Pile_Pions(int nNbJoueurs,bool Reset){
		
		GameObject goPion;
		Vector3 vctPion;
		Quaternion QuatPion;

		m_tabCase = new int[6,7];

		int nNbJeton = 0;

		if (m_nNbJoueur_Total == 2)
			nNbJeton = 21;
		else if (m_nNbJoueur_Total == 3)
			nNbJeton = 24;
		else if (m_nNbJoueur_Total == 4)
			nNbJeton = 18;
		
		for (int i = 1; i <= nNbJoueurs; i++) {

			if (!Reset) {
				
				stPile_JR [(i - 1)].tabgo_Pion = new GameObject[nNbJeton];

			}

			stPile_JR [(i - 1)].m_nPion_A_Jouer = nNbJeton - 1;

			for (int j = 1; j <= nNbJeton; j++) {

				if (!Reset) {
					
					goPion = Instantiate (m_goPion [(i - 1)], m_goBase [i - 1].transform.position, Quaternion.identity) as GameObject;
					goPion.transform.parent = m_goBase [i - 1].transform;

					QuatPion = Quaternion.identity;
					QuatPion.eulerAngles = new Vector3 (-90, 0, 0);

					goPion.transform.rotation = QuatPion;

					goPion.transform.Translate(Vector3.forward * j * 0.1f);

					goPion.name = j.ToString();

					stPile_JR [(i - 1)].tabgo_Pion [(j - 1)] = goPion;

				} else {
				
					goPion = stPile_JR [(i - 1)].tabgo_Pion [(j - 1)];

					QuatPion = Quaternion.identity;
					QuatPion.eulerAngles = new Vector3 (-90, 0, 0);

					goPion.transform.rotation = QuatPion;

					goPion.transform.position = m_goBase [i - 1].transform.position;

					goPion.transform.Translate(Vector3.forward * j * 0.1f);

				}

			}

		}

		if (m_nNbJoueur_Total == 2 && !Reset) {

			Vector3 vctPionBois = Vector3.zero;

			float x_off = -5.5f;
			float y_off = 1;

			for (int i = 0; i < 3; i++) {

				for (int j = 0; j < 6; j++) {
					
					vctPionBois = new Vector3 (x_off + i, y_off + j, 0);

					goPion = Instantiate (m_goPionBois, vctPionBois, Quaternion.identity) as GameObject;

					goPion.transform.parent = m_goBase [0].transform.parent.parent;

				}

			}

			x_off = 3.5f;
			y_off = 1;

			for (int i = 0; i < 3; i++) {

				for (int j = 0; j < 6; j++) {

					vctPionBois = new Vector3 (x_off + i, y_off + j, 0);

					goPion = Instantiate (m_goPionBois, vctPionBois, Quaternion.identity) as GameObject;

					goPion.transform.parent = m_goBase [0].transform.parent.parent;

				}

			}

		}

	}

	public void OnClick_Grille(){

		if (m_bPlay) {

			m_bPlay = false;

			int i = 0;
			bool bStop = false;

			while (i <= 5 && bStop == false)
			{
				
				if (m_tabCase[i, (m_nSel_Col)] == 0)
					bStop = true;
				else
					i++;            

			}

			if (bStop == true)
			{

				object[] Params;

				Params = new object[3];

				Params[0] = m_nSel_Col;
				Params[1] = i;
				Params[2] = m_nNum_Joueur;

				m_PhotonView.RPC ("Joue_Coup", PhotonTargets.MasterClient,Params);

			}
			else
				m_bPlay = true;		
		}

	}

	IEnumerator AffichePion(float rPos,int nLgArr,int nJeton,bool bGagne){
	
		m_goPion_EnCours.transform.position = new Vector3 (m_nSel_Col + 0.5f, rPos, 0);

		yield return new WaitForSeconds(0.05f);

		rPos -= 0.5f;

		if (rPos >= (nLgArr + 1))
			
			StartCoroutine (AffichePion (rPos,nLgArr,nJeton,bGagne));
		
		else {

			object[] Params;

			Params = new object[2];

			Params[0] = nJeton;
			Params[1] = bGagne;

			m_PhotonView.RPC ("Fini_Coup", PhotonTargets.MasterClient,Params);

		}

	}
	
	// Update is called once per frame
	void Update () {
		
		//a supprimer
		//m_bStart_Level = true;
		//m_bPlay = true;

		if (m_bStart_Level)
		{

			if (m_bPlay)
			{

				if (!m_bPioche_Pion) {
					
					m_goPion_EnCours = stPile_JR [(m_nNum_Joueur - 1)].tabgo_Pion [stPile_JR [(m_nNum_Joueur - 1)].m_nPion_A_Jouer];

					m_goPion_EnCours.transform.position = new Vector3 (0.5f, 7, 0);
					m_goPion_EnCours.transform.rotation = Quaternion.identity;
					m_goPion_EnCours.transform.Rotate (new Vector3 (0,0,Random.Range(0,360)));

					m_bPioche_Pion = true;
					stPile_JR [(m_nNum_Joueur - 1)].m_nPion_A_Jouer--;

				}

				//Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
					
				//Debug.Log ((ray.direction.x + 0.3f) * 10);

				if (m_nNbJoueur_Total == 2) {
					
					m_nSel_Col = (int)((Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane)).x + 3 * 0.025f) / 0.025f);

					if (m_nSel_Col < 0)
						m_nSel_Col = 0;

					if (m_nSel_Col > 5)
						m_nSel_Col = 5;
					
					m_goPion_EnCours.transform.position = new Vector3 (m_nSel_Col + 0.5f - 3, 7, 0);
				
				} else {
					
					m_nSel_Col = (int)((Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane)).x + 6 * 0.025f) / 0.025f);

					if (m_nSel_Col < 0)
						m_nSel_Col = 0;

					if (m_nSel_Col > 11)
						m_nSel_Col = 11;

					m_goPion_EnCours.transform.position = new Vector3 (m_nSel_Col + 0.5f - 6, 7, 0);

				}



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

			m_nNbJoueur_Reponse = 0;

			m_PhotonView.RPC ("Init_Score", PhotonTargets.All);

			m_PhotonView.RPC ("Lance_Partie", PhotonTargets.MasterClient);

		}

	}

	[PunRPC]
	void Init_Score()
	{

		for (int i = 1; i < 5; i++) {

			m_goScore [i - 1] = GameObject.Find ("sc" + i);

			if(i > m_nNbJoueur_Total)
				m_goScore [i - 1].SetActive (false);

		}


	}

	[PunRPC]
	void Lance_Partie()
	{
		
		m_bStart_Level = true;
		m_bPlay = true;

	}

	[PunRPC]
	void Joue_Coup(int Col,int Lg,int nJeton)
	{

		//Controle si gagné
		bool bGagne;
		bGagne = false;

		bGagne = Controle_Gagne(Col,Lg,nJeton);

		object[] Params;

		Params = new object[4];

		Params[0] = Col;
		Params[1] = Lg;
		Params[2] = nJeton;
		Params[3] = bGagne;

		m_PhotonView.RPC ("Joue_Coup_Transmit", PhotonTargets.All,Params);

	}

	bool Controle_Gagne(int Col, int Lg, int nJeton)
	{
		bool bGagne;
		bGagne = false;

		int i = 0;
		int j = 0;

		//horizontal

		int nbAlign = 0;
		for (i = Col - 3; i <= Col + 3; i++)
		{

			if (i >= 0 && i <= 6)
			{          

				if (i == Col || m_tabCase[Lg, i] == nJeton)
					nbAlign++;
				else if(nbAlign < 4)
					nbAlign = 0;

			}

		}

		if (nbAlign >= 4)
			return true;
		else
		{

			nbAlign = 0;
			for (j = Lg - 3; j <= Lg + 3; j++)
			{

				if (j >= 0 && j <= 5)
				{          

					if (j == Lg || m_tabCase[j, Col] == nJeton)
						nbAlign++;
					else if(nbAlign < 4)
						nbAlign = 0;

				}

			}

			if (nbAlign >= 4)
				return true;
			else
			{

				nbAlign = 0;

				j = Lg - 3;
				for (i = Col - 3; i <= Col + 3; i++)
				{

					if (i >= 0 && i <= 6 && j >= 0 && j <= 5)
					{          

						if ((i == Col && j == Lg) || m_tabCase[j, i] == nJeton)
							nbAlign++;
						else if(nbAlign < 4)
							nbAlign = 0;

					}

					j++;
				}

				if (nbAlign >= 4)
					return true;
				else
				{

					j = Lg + 3;
					for (i = Col - 3; i <= Col + 3; i++)
					{

						if (i >= 0 && i <= 6 && j >= 0 && j <= 5)
						{          

							if ((i == Col && j == Lg) || m_tabCase[j, i] == nJeton)
								nbAlign++;
							else if(nbAlign < 4)
								nbAlign = 0;

						}

						j--;
					}

					if (nbAlign >= 4)
						return true;
					else
					{

						return false;

					}   

				}   

			}

		}


	}

	[PunRPC]
	void Joue_Coup_Transmit(int Col, int Lg, int nJeton, bool bGagne)
	{

		m_tabCase[Lg, Col] = nJeton;

		if (nJeton != m_nNum_Joueur) {

			m_goPion_EnCours = stPile_JR [(nJeton - 1)].tabgo_Pion [stPile_JR [(nJeton - 1)].m_nPion_A_Jouer];

			m_goPion_EnCours.transform.position = new Vector3 (0, 7, 0);
			m_goPion_EnCours.transform.rotation = Quaternion.identity;

			m_bPioche_Pion = true;
			stPile_JR [(nJeton - 1)].m_nPion_A_Jouer--;

		}

		if (m_nNbJoueur_Total == 2) {
			m_nSel_Col = Col - 3;
		} else {
			m_nSel_Col = Col - 6;
		}


		StartCoroutine (AffichePion (7,Lg,nJeton,bGagne));
		//StartCoroutine(AffichePion(Col + 1, 6, Lg + 1, Admin, bGagne, null));        

	}

	[PunRPC]
	void Fini_Coup(int nJeton, bool bGagne){

		m_nNbJoueur_Reponse++;

		if (m_nNbJoueur_Reponse == m_nNbJoueur_Total) {
			
			m_nNbJoueur_Reponse = 0;

			if (!bGagne) {

				nJeton++;
				if (nJeton > m_nNbJoueur_Total)
					nJeton = 1;
			
				object nJr;

				nJr = nJeton;

				m_PhotonView.RPC ("Joueur_Suivant", PhotonTargets.All, nJr);

			} else {

				m_PhotonView.RPC ("Score_Actu", PhotonTargets.All, nJeton);

			}

		}

	}

	[PunRPC]
	void Joueur_Suivant(int nJeton){
		
		if (nJeton == m_nNum_Joueur) {

			m_bPlay = true;
			m_bPioche_Pion = false;

		}

	}

	[PunRPC]
	void Score_Actu(int nJeton){
		
		MeshFilter mfChiffre;

		mfChiffre = m_goScore [nJeton].GetComponent<MeshFilter> ();

		m_tabScore [nJeton]++;

		if (m_tabScore [nJeton] < 10) {

			mfChiffre.mesh = m_goScoreChiffres [m_tabScore [nJeton]];

		}

		StartCoroutine (Reset_Partie(nJeton));

	}

	IEnumerator Reset_Partie(int nJeton){

		yield return new WaitForSeconds(5f);

		Init_Pile_Pions (m_nNbJoueur_Total,true);

		if (nJeton == m_nNum_Joueur) {

			m_bPlay = true;
			m_bPioche_Pion = false;

		}

	}

		
}
