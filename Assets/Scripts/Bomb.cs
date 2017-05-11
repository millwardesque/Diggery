using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum BombState {
	Deactivated,
	Activated,
	Detonated,
	Exploded,
};

public class Bomb : MonoBehaviour {
	public GameObject parent;
	public float distanceFromParent = 1f;
	public float explosionPower = 25f;
	public float explosionRadius = 3f;

	BombState m_state;
	BombState State {
		get { return m_state; }
		set {
			m_state = value;
		}
	}

	SpriteRenderer m_sprite;
	Collider2D m_collider;

	void Awake() {
		m_sprite = GetComponentInChildren<SpriteRenderer> ();
		m_collider = GetComponentInChildren<Collider2D> ();
	}

	void Update() {
		if (Input.GetKeyDown (KeyCode.T)) {
			Detonate ();
		}
	}

	void FixedUpdate() {
		if (State == BombState.Detonated) {
			Collider2D[] colliders = Physics2D.OverlapCircleAll (transform.position, explosionRadius);
			for (int i = 0; i < colliders.Length; ++i) {
				if (colliders [i] == m_collider) {
					continue;
				}
	
				Debug.Log ("Explosion contacts " + colliders [i].name);
				ColliderDistance2D distance = colliders [i].Distance (this.GetComponentInChildren<Collider2D> ());
				Vector3 direction = (distance.pointA - distance.pointB).normalized;

				Debug.Log("[" + colliders[i].name + "] " + "Distance: " + distance.distance);
				Debug.Log("[" + colliders[i].name + "] " + "Direction: " + direction);

				float attenuation = 1f - distance.distance / explosionPower;
				colliders [i].attachedRigidbody.AddForceAtPosition(direction * attenuation * explosionPower, distance.pointA, ForceMode2D.Impulse);
			}
			State = BombState.Exploded;
		}
	}

	public void Attach(GameObject parent) {
		this.parent = parent;

		Vector3 directionFromParent = transform.position - parent.transform.position;
		transform.position = parent.transform.position + directionFromParent.normalized * distanceFromParent;
		State = BombState.Activated;
	}

	public void Detonate() {
		State = BombState.Detonated;
	}
}
