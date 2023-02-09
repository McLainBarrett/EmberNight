using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

	public ClientGhost Owner;
	Rigidbody2D rb;

	public Vector2 Direction;
	//int pullForce = 50;
	//int pullRange = 10;

	private void Start() {
		rb = GetComponent<Rigidbody2D>();
		GetComponent<TrailRenderer>().enabled = true;

		//TrailRenderer TR = GetComponent<TrailRenderer>();
		//TR.enabled = true;
		//if (Owner.team != 0) {
		//	if (Owner.team == 1) {
		//		TR.startColor = Color.red;
		//	} else {
		//		TR.startColor = Color.blue;
		//	}
		//}

		StartCoroutine(Death());
	}

	private void Update() {
		//var dir = rb.velocity.normalized;
		//var ang = 45;
		//var cols = Physics2D.OverlapCircleAll(transform.position, 30);
		//List<Transform> targs = new List<Transform>();

		//foreach (var col in cols) {
		//	var tdir = (col.transform.position - transform.position).normalized;
		//	ClientGhost cg = col.gameObject.GetComponentInChildren<ClientGhost>();
		//	if (cg) {
		//		print("Client: " + cg.clientName);
		//	} else {
		//		print("Object: " + col.name);
		//	}
			
		//	if (cg && cg != Owner && (Owner.team == 0 || cg.team != Owner.team) && Vector2.Angle(tdir, dir) < ang) {
		//		targs.Add(col.transform);
		//	}
		//}

		//if (targs.Count > 0) {
		//	print(targs[0].name);
		//	var ac = 10;

		//	var targ = targs[0];
		//	var tVel = targ.GetComponent<Rigidbody2D>().velocity;
		//	var rDir = transform.position - targ.position;
		//	var rVel = rb.velocity - tVel;

		//	var time = rDir / rVel.magnitude;
		//	var tPos = tVel * time;
		//	var hVel = tPos / time;// - rb.velocity;

		//	rb.velocity = hVel;

		//	//Get relative velocity, time until hit
		//	//use time to predict position when hit
		//	//Match velocity to hit position in said time




	}





		//Collider2D[] Cols = Physics2D.OverlapCircleAll(transform.position, pullRange);
		//foreach (Collider2D col in Cols) {
		//	ClientGhost cg = col.gameObject.GetComponentInChildren<ClientGhost>();
		//	if (cg && cg != Owner && (Owner.team == 0 || cg.team != Owner.team)) {

		//		//rb.velocity += (Vector2)(transform.InverseTransformPoint(new Vector2(transform.TransformPoint(col.transform.position).x, 0).normalized) * pullForce * Time.deltaTime);
		//		//rb.velocity += Vector3.Dot(col.transform.position - transform.position, RotateBy(Direction, 90)) * RotateBy(Direction, 90);

		//		rb.velocity += (Vector2)(col.transform.position - transform.position).normalized * pullForce * Time.deltaTime;
		//	}
		//}


	private void OnCollisionEnter2D(Collision2D col) {
		try {
			if (col.gameObject.GetComponentInChildren<ClientGhost>() != Owner) {
				StartCoroutine(DeathI());
			}
		} catch { }
	}

	IEnumerator Death() {
		yield return new WaitForSeconds(6);
		Destroy(gameObject);
	}
	IEnumerator DeathI() {
		yield return new WaitForSeconds(0.2f);
		Destroy(gameObject);
	}

	//public static Vector2 RotateBy(Vector2 v, float a, bool bUseRadians = false) {
	//	if (!bUseRadians) a *= Mathf.Deg2Rad;
	//	var ca = System.Math.Cos(a);
	//	var sa = System.Math.Sin(a);
	//	var rx = v.x * ca - v.y * sa;

	//	return new Vector2((float)rx, (float)(v.x * sa + v.y * ca));
	//}
}