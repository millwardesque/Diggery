using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BombState {
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
	public BombState State {
		get { return m_state; }
		set {
			m_state = value;
			if (m_state == BombState.Exploded) {
				Attach (null);
			}
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
				if (colliders [i] == m_collider || (this.parent != null && colliders[i] == this.parent.GetComponentInChildren<Collider2D>()) || (colliders[i].attachedRigidbody == null)) {
					continue;
				}

				ColliderDistance2D distance = colliders [i].Distance (this.GetComponentInChildren<Collider2D> ());
				Vector3 direction = (distance.pointA - (Vector2)this.transform.position).normalized;
				float attenuation = Mathf.Clamp(1f - (distance.distance / explosionRadius), 0f, 1f);
				colliders [i].attachedRigidbody.AddForceAtPosition(direction * attenuation * explosionPower, distance.pointA, ForceMode2D.Impulse);
			}
			State = BombState.Exploded;
		}
	}

	public void Attach(GameObject parent) {
		if (State != BombState.Deactivated && parent != null) {
			return;
		}

		this.parent = parent;

		if (parent != null) {
			transform.SetParent (parent.transform);
			Vector3 directionFromParent = transform.position - parent.transform.position;
			transform.position = parent.transform.position + directionFromParent.normalized * distanceFromParent;
			State = BombState.Activated;
		} else {
			transform.SetParent (null);
			State = BombState.Deactivated;
		}
	}

	public void Detonate() {
		State = BombState.Detonated;
	}
}
