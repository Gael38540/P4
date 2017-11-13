using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class Game : MonoBehaviour {

    public Sprite m_spt_Sel_Off;
    public Sprite m_spt_Sel_On;

    public Sprite m_spt_Jaune;
    public Sprite m_spt_Rouge;
    public Sprite m_spt_Aucun;

    public Image[] m_tabSel;
    public Text m_Info;

	public GridLayoutGroup gridGrille;

    private int m_nSel_Col;
    private int[,] m_tabCase;
    private bool m_bMaster;
    private PhotonView m_PhotonView;
    private bool m_bPlay;
    private bool m_bStart_Level;
    private EventTrigger m_evntClick; 
    private EventTrigger m_evntRejouer;
    private GameObject m_goRejouer;

    private int m_nPtMoi;
    private int m_nPtLui;

    private int m_nDemande_Rejouer;

    private bool m_bGagne;
	private int nLargCase;

    void Awake()
    {
                
        DontDestroyOnLoad(this);

    }

    /*void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoadingold;
    }
    
    void OnDisable()
    {
		SceneManager.sceneLoadedold -= OnLevelFinishedLoadingold;
    }*/

	// Use this for initialization
	void Start () {
		
        m_nSel_Col = -1;
        m_tabCase = new int[6,7];
        m_bMaster = false;
        m_bPlay = false;
        m_bStart_Level = false;
        m_nPtMoi = 0;
        m_nPtLui = 0;
        m_nDemande_Rejouer = 0;

	}

	void OnLevelFinishedLoadingold(Scene scene, LoadSceneMode mode)
    {
		float rHaut_Screen = Screen.height - 100;

		nLargCase = (int)rHaut_Screen / 6;

		gridGrille = GameObject.Find("Grille").GetComponent <GridLayoutGroup>();

		gridGrille.cellSize = new Vector2(nLargCase,nLargCase);

		RectTransform trEntete;

		for (int i = 0; i < 7; i++) {
			
			trEntete = GameObject.Find ("Col_Titre" + (i + 1)).GetComponent <RectTransform> ();

			trEntete.sizeDelta = new Vector2 (nLargCase, 20);
			trEntete.position = new Vector3( nLargCase / 2 + i * nLargCase + i * 5,rHaut_Screen+40,0);

		}



        m_PhotonView = GetComponent<PhotonView>();
        m_bMaster = PhotonNetwork.isMasterClient;

        m_Info = GameObject.Find("Info").GetComponent <Text>();

        m_evntClick = GameObject.Find("Plateau").GetComponent <EventTrigger>();
        m_goRejouer = GameObject.Find("Btn_Rejouer");

        
        m_evntRejouer = m_goRejouer.GetComponent <EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((data) => { OnClick(); });
        m_evntClick.triggers.Add(entry);

        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((data) => { Rejouer(); });
        m_evntRejouer.triggers.Add(entry);
        
        m_goRejouer.SetActive(false);

        for (int i = 0; i < 7; i++)
        {
            m_tabSel[i] = GameObject.Find("Col_" + (i + 1)).GetComponent <Image>();
        }
       

        if(m_bMaster)
            m_bPlay = true;

        m_bStart_Level = true;

    }
	
	// Update is called once per frame
	void Update()
    {

        if (m_bStart_Level)
        {

            if (m_bPlay)
            {
                
                m_Info.text = "A vous de jouer!!\nvous " + m_nPtMoi + " - " + m_nPtLui + " adversaire";
                if (m_nSel_Col != -1)
                {
                    m_tabSel[m_nSel_Col].sprite = m_spt_Sel_Off;
                }   
        
				m_nSel_Col = ((int)Input.mousePosition.x) / (nLargCase + 5);
                
                if (m_nSel_Col < 0)
                    m_nSel_Col = 0;
        
                if (m_nSel_Col > 6)
                    m_nSel_Col = 6;
              
                m_tabSel[m_nSel_Col].sprite = m_spt_Sel_On;
    
            }
            else{

                m_Info.text = "Attente de l'adversaire!!\nvous " + m_nPtMoi + " - " + m_nPtLui + " adversaire";

            }

        }

	}

    public void OnClick()
    {

        if (m_bPlay)
        {

            m_tabSel[m_nSel_Col].sprite = m_spt_Sel_Off;

            m_bPlay = false;      
    
            int i = 0;
            bool bStop = false;
    
            while (i <= 5 && bStop == false)
            {
                
                if (m_tabCase[i, m_nSel_Col] == 0)
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
                Params[2] = m_bMaster;
                
                m_PhotonView.RPC ("Joue_Coup", PhotonTargets.MasterClient,Params);

            }
            else
                m_bPlay = true;

        }

    }

    public void Rejouer()
    {
        
        m_goRejouer.SetActive(false);

        m_Info.text = "Attente de l'adversaire!!\nvous " + m_nPtMoi + " - " + m_nPtLui + " adversaire";

        m_PhotonView.RPC ("Demande_Rejouer", PhotonTargets.MasterClient);

    }

    IEnumerator AffichePion(int Col, int LgDep, int LgArr, bool Admin, bool bGagne, Image Old)
    {

        if (Old != null)
            Old.sprite = m_spt_Aucun;   

        Image imgPion = GameObject.Find(LgDep + "_" + Col).GetComponent<Image>();
        
        if (Admin)
            imgPion.sprite = m_spt_Jaune;
        else
            imgPion.sprite = m_spt_Rouge;

        Old = imgPion;

        if (LgDep != LgArr)
        {

            yield return new WaitForSeconds(0.1f);
            
            StartCoroutine(AffichePion(Col, (LgDep - 1), LgArr, Admin, bGagne, Old));

        }
        else
        {

            if (!bGagne)
            {
    
                if (Admin == m_bMaster)
                {
                    m_bPlay = true;
                }

            }
            else
            {
    
                m_bStart_Level = false;
                m_bGagne = !Admin;

                if (Admin == m_bMaster)
                {

                    m_nPtLui += 1; 
                    m_Info.text = "Domage perdu!!\nvous " + m_nPtMoi + " - " + m_nPtLui + " adversaire";
                                       
                }
                else
                {

                    m_nPtMoi += 1;
                    m_Info.text = "Bravo Gagné!!\nvous " + m_nPtMoi + " - " + m_nPtLui + " adversaire";
                   
                }

                m_goRejouer.SetActive(true);
                
            }            

        }

    }

    [PunRPC]
    void Joue_Coup(int Col,int Lg,bool Admin)
    {

        //Controle si gagné
        bool bGagne;
        bGagne = false;

        int nJeton = 1;
        
        if(Admin)
            nJeton++;

        bGagne = Controle_Gagne(Col,Lg,nJeton);
        
        object[] Params;

        Params = new object[4];

        Params[0] = Col;
        Params[1] = Lg;
        Params[2] = !Admin;
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
    void Joue_Coup_Transmit(int Col, int Lg, bool Admin, bool bGagne)
    {

        if(Admin == m_bMaster)
            m_tabCase[Lg, Col] = 1;
        else
            m_tabCase[Lg, Col] = 2;

        StartCoroutine(AffichePion(Col + 1, 6, Lg + 1, Admin, bGagne, null));        

    }

    [PunRPC]
    void Demande_Rejouer()
    {
        m_nDemande_Rejouer++;
    
        if (m_nDemande_Rejouer == 2)
        {

            m_nDemande_Rejouer = 0;

            m_PhotonView.RPC ("Rejouter_OK", PhotonTargets.All);

        }
    }

    [PunRPC]
    void Rejouter_OK()
    {
        m_nSel_Col = -1;
        m_tabCase = new int[6, 7];

        for (int i = 0; i < 7; i++)
        {

            for (int j = 0; j < 6; j++)
            {

                m_tabCase[j, i] = 0;
                Image imgPion = GameObject.Find((j + 1) + "_" + (i + 1)).GetComponent<Image>();
        
                imgPion.sprite = m_spt_Aucun;
        
            }

        }    

        if(m_bMaster == m_bGagne)
            m_bPlay = true;

        m_bStart_Level = true;

    }

}
