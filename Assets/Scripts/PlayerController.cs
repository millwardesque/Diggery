using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour {
	public float maxSpeed = 1f;
	public float digPower = 1f;
	public int playerId;

	public int health = 10;

	public TreasureFinder treasureFinderPrefab;

	SpriteRenderer m_avatar;
	Rigidbody2D m_rb;
	Marker m_currentMarker = null;
	TreasureFinder m_treasureFinder;
	Bomb m_currentItem = null;

	void Awake() {
		m_rb = GetComponent<Rigidbody2D> ();
		m_treasureFinder = Instantiate<TreasureFinder> (treasureFinderPrefab);
		m_treasureFinder.transform.localPosition = new Vector2 (1f, 0f);
		m_treasureFinder.parent = gameObject;
		m_treasureFinder.State = TreasureFinderState.Enabled;
		m_treasureFinder.name = "Player " + playerId + " Treasure Finder";

		m_avatar = GetComponentInChildren<SpriteRenderer> ();
	}

	void Start() {
		if (playerId == 0) {
			m_avatar.color = new Color (0f, 1f, 0f);
		}
		else if (playerId == 1) {
			m_avatar.color = new Color (1f, 0f, 0f);
		}

	}

	void Update () {
		Vector2 newVelocity = Vector2.zero;

		if (playerId == 0) {
			newVelocity.x = Input.GetAxis ("Horizontal");
			newVelocity.y = Input.GetAxis ("Vertical");

			if (Input.GetKey(KeyCode.RightShift)) {
				Dig ();
			}

		} else if (playerId == 1) {
			newVelocity.x = Input.GetAxis ("P2-Horizontal");
			newVelocity.y = Input.GetAxis ("P2-Vertical");

			if (Input.GetKey(KeyCode.LeftShift)) {
				Dig ();
			}
		}

		m_rb.velocity = newVelocity * maxSpeed;
	}

	void FixedUpdate() {
		Vector2 direction = m_rb.velocity.normalized;
		if (Mathf.Abs (direction.magnitude) > Mathf.Epsilon) {
			float rotation = Vector2.Angle (Vector2.right, direction);
			if (Vector3.Cross (Vector2.right, direction).z < 0) {
				rotation = 360 - rotation;
			}

			transform.rotation = Quaternion.Euler (0, 0, rotation);
		}
	}

	void OnTriggerEnter2D(Collider2D col) {
		if (col.GetComponent<Marker> ()) {
			m_currentMarker = col.GetComponent<Marker> ();
		} else if (col.GetComponent<Bomb> ()) {
			Bomb newBomb = col.GetComponent<Bomb> ();
			if (m_currentItem != newBomb) {
				if (newBomb.State == BombState.Activated) {
					newBomb.Detonate ();
				} else if (newBomb.State == BombState.Deactivated) {
					if (m_currentItem != null) {
						m_currentItem.Attach (null);
					}

					m_currentItem = newBomb;
					m_currentItem.Attach (this.gameObject);
				}
			}
		}
	}

	void OnTriggerExit2D(Collider2D col) {
		if (col.GetComponent<Marker> () && col.GetComponent<Marker>() == m_currentMarker) {
			m_currentMarker = null;
		}
	}

	void Dig() {
		if (m_currentMarker != null) {
			m_currentMarker.Uncover (digPower * Time.deltaTime, this);
		}
	}

	void OnDestroy() {
		if (m_treasureFinder != null) {
			Destroy (m_treasureFinder.gameObject);
		}
	}
}
