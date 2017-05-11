using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marker : MonoBehaviour {
	public float startCoverDepth = 3f;
	float currentCoverDepth = 0f;

	SpriteRenderer m_sprite;

	void Awake() {
		m_sprite = GetComponentInChildren<SpriteRenderer> ();
	}

	void Start() {
		currentCoverDepth = startCoverDepth;
		UpdateVisibility ();
	}

	public void Uncover(float uncoverAmount, PlayerController digger) {
		if (Mathf.Abs (currentCoverDepth) < Mathf.Epsilon) {
			return;
		}

		currentCoverDepth = Mathf.Clamp (currentCoverDepth - uncoverAmount, 0f, currentCoverDepth);
		UpdateVisibility ();
		if (Mathf.Abs(currentCoverDepth) < Mathf.Epsilon) {
			Debug.Log ("Uncovered by '" + digger.name + "'");
		}
	}

	void UpdateVisibility() {
		float alpha = startCoverDepth > Mathf.Epsilon ? 1f - currentCoverDepth / startCoverDepth : 1f;
		m_sprite.color = new Color (m_sprite.color.r, m_sprite.color.g, m_sprite.color.b, alpha);
	}
}
