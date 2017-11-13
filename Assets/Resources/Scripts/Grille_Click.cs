using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grille_Click : MonoBehaviour {

	private P4_Game m_scpP4_G;

	// Use this for initialization
	void Start () {

		m_scpP4_G = GameObject.Find("Game_P4").GetComponent <P4_Game>();

	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetMouseButtonDown (0)) {
		
			m_scpP4_G.OnClick_Grille ();

		}
		
	}
}
