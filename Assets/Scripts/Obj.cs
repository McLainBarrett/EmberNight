using System.Collections;
using UnityEngine;

public class Obj : MonoBehaviour {
	public int ID = 0;
	public string Type;
	public Vector3 Pos;
	public Vector3 Vel;
	public int team;
	public string objectName = "";

	private Rigidbody2D RB;

	private void Start() {
		if (ID == 0) {
			ID = Random.Range(1, 999999999);
		}
		RB = GetComponent<Rigidbody2D>();
		StartCoroutine(Trail());
		try {
			objectName = GetComponentInChildren<ClientGhost>().clientName;
		} catch { }
	}

	public void Up() {
		if (!RB)
			RB = GetComponent<Rigidbody2D>();
		Pos = new Vector3(RB.position.x, RB.position.y, RB.rotation);
		Vel = new Vector3(RB.velocity.x, RB.velocity.y, RB.angularVelocity);
		try {
			team = GetComponentInChildren<Player>().team;
		} catch { }
	}
	public void Down() {
		try {
			if (!RB)
				RB = GetComponent<Rigidbody2D>();
			RB.position = new Vector3(Pos.x, Pos.y, 0);
			RB.rotation = Pos.z;
			RB.velocity = new Vector3(Vel.x, Vel.y, 0);
			RB.angularVelocity = Vel.z;
			GetComponentInChildren<Player>().team = team;
		} catch { }
	}

	public void Kill() {
		Body body = gameObject.GetComponent<Body>();
		if (body) {
			body.Kill();
		} else {
			if (gameObject.GetComponentInChildren<Player>()) {
				transform.Find("Controller").SetParent(null, false);
			}
			Destroy(gameObject);
		}
	}
	IEnumerator Trail() {
		yield return new WaitForSeconds(0.1f);
		try {
			TrailRenderer TR = GetComponent<TrailRenderer>();
			TR.enabled = true;
			if (team != 0) {
				if (team == 1) {
					TR.startColor = Color.red;
				} else {
					TR.startColor = Color.blue;
				}
			}
		} catch { }
	}
}