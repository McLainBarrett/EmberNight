using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour {

	public float Direction;
	public int Thrust;
	public int Fire;

	public GameObject Projectile;
	private Rigidbody2D RB;
	private ClientGhost CG;
	public float cooldown;

	private ClientGhost Lastcg;//Last cg to damage this body

	private void Start() {
		RB = GetComponent<Rigidbody2D>();
		CG = GetComponentInChildren<ClientGhost>();
	}

	private void FixedUpdate() {
		cooldown -= Time.fixedDeltaTime;

		if (Fire == 1 && cooldown <= 0) {
			cooldown = 2;
			Fire1();
		}

		if (Direction != -909) {
			float rot = (transform.eulerAngles.z + 180) % 360 - 180;
			float dir = Direction - rot;
			if (dir > 180)
				dir -= 360;
			else if (dir < -180)
				dir += 360;

			float D = 0;
			if (dir > 2)
				D = 1;
			else if (dir < -2)
				D = -1;
			if (Thrust != -1)
				RB.angularVelocity = 120 * D;
			else
				RB.angularVelocity = 120 * D * 1.5f;

		} else {
			RB.angularVelocity = 0;
		}

		if (Thrust == 1) {
			RB.AddRelativeForce(Vector2.up * 10);
		} 
		if (Thrust == -1) {
			RB.drag = 1.5f;
		} else {
			RB.drag = 0.1f;
		}
		
		if (CG.HealthRT <= 0) {
			Kill();
		}
	}

	private void OnCollisionEnter2D(Collision2D collision) {
		try {
			if (collision.gameObject.tag == "Wall")
				return;
			if (CG.team != 0 && CG.team == collision.gameObject.GetComponent<Bullet>().Owner.team)
				return;
		} catch { }
		try {
			if (CG.team != 0 && CG.team == collision.gameObject.GetComponentInChildren<ClientGhost>().team)
				return;
		} catch { }

		float damage = collision.relativeVelocity.magnitude;
		//float damage = 0.2f * collision.gameObject.GetComponent<Rigidbody2D>().mass * Mathf.Pow(collision.relativeVelocity.magnitude, 2);
		CG.HealthRT -= damage;

		ClientGhost cg;
		if (collision.gameObject.tag == "Bullet") {
			cg = collision.gameObject.GetComponent<Bullet>().Owner;
		} else {
			cg = collision.gameObject.GetComponentInChildren<ClientGhost>();
		}
		if (cg && CG && cg != CG) {
			cg.score += damage;
			Lastcg = cg;
		}
	}
	private void Fire1() {
		Vector2 Pos = transform.TransformPoint(Vector2.up);
		Vector2 Dir = transform.TransformDirection(Vector2.up);
		GameObject Bul = Instantiate(Projectile, Pos, Quaternion.identity);
		//Bul.GetComponent<Rigidbody2D>().velocity = Dir * 25 + (Dir * RB.velocity.magnitude);
		Bul.GetComponent<Rigidbody2D>().velocity = Dir * 25 + RB.velocity;
		Bul.GetComponent<Bullet>().Owner = CG;
		Bul.GetComponent<Bullet>().Direction = Dir;
		Bul.GetComponent<Obj>().team = GetComponent<Obj>().team;
	}
	public void Kill() {
		if (Lastcg) {
			Lastcg.score += 50;
		}
		CG.score *= 0.9f;
		transform.GetChild(1).SetParent(null, false);
		Destroy(gameObject);
	}
}