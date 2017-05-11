using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TreasureFinderState {
	Disabled,
	Enabled
};

public class TreasureFinder : MonoBehaviour {
	public GameObject parent;
	public float distanceFromParent = 1f;

	TreasureFinderState m_state;
	public TreasureFinderState State {
		get { return m_state; }
		set {
			m_state = value;
			if (m_state == TreasureFinderState.Disabled) {
				if (m_sprite != null) {
					m_sprite.color = new Color (m_sprite.color.r, m_sprite.color.g, m_sprite.color.b, 0f);
				}				
			} else if (m_state == TreasureFinderState.Enabled) {
				if (m_sprite != null) {
					m_sprite.color = new Color (m_sprite.color.r, m_sprite.color.g, m_sprite.color.b, 1f);
				}
			}
		}
	}

	SpriteRenderer m_sprite;

	void Awake() {
		m_sprite = GetComponentInChildren<SpriteRenderer> ();
	}

	// Update is called once per frame
	void Update () {
		if (m_state == TreasureFinderState.Enabled) {	
			// Find the nearest treasure.
			Marker[] treasures = FindObjectsOfType<Marker> ();
			float closestSqDistance = -1f;
			Marker closestTreasure = null;
			for (int i = 0; i < treasures.Length; ++i) {
				float distance = (treasures [i].transform.position - parent.transform.position).sqrMagnitude;
				if (closestTreasure == null || closestSqDistance > distance) {
					closestTreasure = treasures [i];
					closestSqDistance = distance;
				}
			}

			// Identify the nearest treasure.
			if (closestTreasure) {
				Vector2 lineToTreasure = closestTreasure.transform.position - parent.transform.position;

				// Set the distance to either the fixed distance to parent or the actual position of the treasure, whichever is closer.
				float distance = Mathf.Min (lineToTreasure.magnitude, distanceFromParent);

				// Keep consistent distance from parent.
				transform.position = parent.transform.position + (Vector3)lineToTreasure.normalized * distance;

				// Rotate to face the treasure
				if (Mathf.Abs (lineToTreasure.magnitude) > Mathf.Epsilon) {
					float rotation = Vector2.Angle (Vector2.right, lineToTreasure);
					if (Vector3.Cross (Vector2.right, lineToTreasure).z < 0) {
						rotation = 360 - rotation;
					}

					transform.rotation = Quaternion.Euler (0, 0, rotation);
				}
			} else {
				Debug.Log ("[" + parent.name + "] " + "No treasures in range");
			}
		}
	}
}
