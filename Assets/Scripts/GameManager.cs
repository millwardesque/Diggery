using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	public PlayerController playerPrefab;
	public Marker treasurePrefab;
	public Vector2 levelDimensions;
	public float minDepth = 1f;
	public float maxDepth = 5f;

	Rect m_levelBounds;
	Marker m_treasure;
	PlayerController[] m_players;

	void Awake() {
		m_players = new PlayerController[(int)PlayerId.Player2 + 1];
	}

	// Use this for initialization
	void Start () {
		CreateNewLevel ();
	}

	void Update() {
		if (Input.GetKeyDown (KeyCode.R)) {
			CreateNewLevel ();
		}
	}

	/// <summary>
	/// Creates a treasure somewhere on the map
	/// </summary>
	Marker CreateTreasure() {
		// Create treasure
		Marker treasure = GameObject.Instantiate<Marker> (treasurePrefab);
		treasure.name = "Treasure";
		treasure.startCoverDepth = Random.Range (minDepth, maxDepth);

		// Position treasure
		Vector2 position = Vector2.zero;
		bool isFarEnough = false;
		while (!isFarEnough) {
			position = new Vector2 (Random.Range (m_levelBounds.xMin, m_levelBounds.xMax), Random.Range (m_levelBounds.yMin, m_levelBounds.yMax));
			bool isTooClose = false;
			for (int i = 0; i < m_players.Length; ++i) {
				if ((position - (Vector2)m_players [i].transform.position).magnitude < levelDimensions.magnitude / 3f) {
					isTooClose = true;
					break;
				}
			}

			isFarEnough = !isTooClose;
		}
		treasure.transform.position = position;

		return treasure;
	}

	/// <summary>
	/// Creates a new player.
	/// </summary>
	/// <returns>The player.</returns>
	/// <param name="playerId">Player identifier.</param>
	PlayerController CreatePlayer(PlayerId playerId) {
		PlayerController player = GameObject.Instantiate<PlayerController> (playerPrefab);
		player.name = playerId.ToString ();
		player.playerId = playerId;

		if (player.playerId == PlayerId.Player1) {
			player.transform.position = new Vector3 (m_levelBounds.xMin + 1f, m_levelBounds.yMax - 1f, 0f);
		} else {
			player.transform.position = new Vector3 (m_levelBounds.xMin + 1f, m_levelBounds.yMin + 1f, 0f);
		}

		return player;
	}

	/// <summary>
	/// Creates an entirely new level.
	/// </summary>
	void CreateNewLevel() {
		Debug.Log ("Creating new level.");
	
		// Calculate the level area.
		m_levelBounds = new Rect(-levelDimensions.x / 2f, -levelDimensions.y / 2f, levelDimensions.x, levelDimensions.y);

		// Cleanup and create new players.
		for (int i = 0; i < m_players.Length; ++i) {
			if (m_players [i] != null) {
				Destroy (m_players [i].gameObject);
			}
		}
		m_players[(int)PlayerId.Player1] = CreatePlayer (PlayerId.Player1);
		m_players[(int)PlayerId.Player2] = CreatePlayer (PlayerId.Player2);

		// Cleanup and make new treasure.
		if (m_treasure) {
			Destroy (m_treasure.gameObject);
			m_treasure = null;
		}
		m_treasure = CreateTreasure ();
	}
}
