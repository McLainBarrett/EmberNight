using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AI : Player {

	private Rigidbody2D RB;
	private ClientGhost CG;

	private float MinRng = 4;
	private float MaxRng = 12;

	private void Awake() {
		CG = gameObject.GetComponent<ClientGhost>();
		playerName = PreLoad.Adjectives[Random.Range(0, PreLoad.Adjectives.Count - 1)]
			+ PreLoad.Animals[Random.Range(0, PreLoad.Animals.Count - 1)]
			+ Random.Range(0, 9999).ToString();
	}

	void Update() {
		if (!RB) {
			RB = gameObject.GetComponentInParent<Rigidbody2D>();
		}

		if (CG.Body) {
			GameObject Target = FindTarget("Object");
			if (Target) {
				Vector2 AP = GetAimPoint(Target, 25 + RB.velocity.magnitude);
				direction = GetAngle(AP);

				Vector2 vDir = Target.transform.position - transform.position;
				float dis = vDir.magnitude;
				Vector2 relvel = (RB.velocity - Target.GetComponent<Rigidbody2D>().velocity);

				float relDir = transform.parent.rotation.eulerAngles.z;
				if (relDir > 180) { relDir -= 360; } else if (relDir < -180) { relDir += 360; }


				if (coolDown <= 0) {
					if (Mathf.Abs(direction - relDir) < 2) {
						thrust = -1;
						fire = 1;

					} else {
						fire = 0;
						thrust = -1;
					}
				} else {
					fire = 0;

					if (dis < MinRng) {//Too close
						if ((relvel * vDir.normalized).magnitude > 0)
							thrust = -1;
						else
							thrust = 1;
						direction *= -1;

					} else if (dis > MaxRng) {//Out of range
						if (relvel.magnitude < dis)
							thrust = 1;
						else
							thrust = 0;

					} else {//In range
						thrust = 1;
						direction += 90;
						if (direction > 180)
							direction -= 360;

					}
				}
			}
		}
	}

	private void OldAI() {
		if (CG.Body) {
			GameObject Target = FindTarget("Object");
			if (Target) {
				Vector2 AP = GetAimPoint(Target, 25 + RB.velocity.magnitude);
				direction = GetAngle(AP);

				float dis = Vector2.Distance(Target.transform.position, transform.position);
				float relvel = (RB.velocity - Target.GetComponent<Rigidbody2D>().velocity).magnitude;

				if (dis < MinRng) {
					thrust = 1;
					direction *= -1;
					fire = 0;

				} else if (dis > MaxRng) {
					if (relvel < dis)
						thrust = 1;
					else if (relvel > dis)
						thrust = -1;
					fire = 1;

				} else {
					fire = 1;
				}

			}

		}
	}

	private Vector2 GetAimPoint(GameObject Target, float ProjectileVelocity) {
		float time = 0;
		try { time = Vector2.Distance(transform.position, Target.transform.position) / (ProjectileVelocity); } catch { return Vector2.zero; }
		Vector2 tpos = Target.transform.position;
		Vector2 Aimpoint = tpos + (Target.GetComponent<Rigidbody2D>().velocity * time - RB.velocity * time);
		return Aimpoint;
	}
	private GameObject FindTarget(string targettag) {
		GameObject[] Targarray = GameObject.FindGameObjectsWithTag(targettag);
		List<GameObject> Targlist = Targarray.OrderBy(x => Vector2.Distance(transform.position, x.transform.position)).ToList();
		List<GameObject> NewTarglist = new List<GameObject>();
		if (ConnectionManager.teams) {
			foreach (GameObject GO in Targlist) {
				if (GO == gameObject || GO.GetComponentInChildren<ClientGhost>().team != CG.team) {
					NewTarglist.Add(GO);
				}
			}
		} else {
			NewTarglist = Targlist;
		}
		if (NewTarglist.Count >= 2)
			return NewTarglist[1];
		else
			return null;
	}
}