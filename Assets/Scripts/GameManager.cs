using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	LevelGenerator m_levelGenerator;

	void Awake() {
		m_levelGenerator = FindObjectOfType<LevelGenerator> ();
	}

	// Use this for initialization
	void Start () {
		m_levelGenerator.CreateNewLevel (2);
	}

	void Update() {
		if (Input.GetKeyDown (KeyCode.R)) {
			m_levelGenerator.CreateNewLevel (2);
		}
	}
}
