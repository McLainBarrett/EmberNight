using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPointer : MonoBehaviour {


	public GameObject Arrow;
	[HideInInspector]
	public SpriteRenderer SR;
	[HideInInspector]
	public GameObject Arrowsprite;
	[HideInInspector]
	public GameObject player;

	[HideInInspector]
	public Rigidbody2D prb;
	[HideInInspector]
	public Rigidbody2D srb;
	Player playerPlayer;
	void Start () {
		Arrowsprite = Instantiate(Arrow, transform.position, Quaternion.identity);
		player = GameObject.FindGameObjectWithTag("Player");
		SR = Arrowsprite.GetComponent<SpriteRenderer>();
		playerPlayer = player.GetComponent<Player>();
	}

	void Update () {
		if (!srb) {
			try {
				srb = GetComponent<Rigidbody2D>();
			} catch { }
		} if (!prb) {
			try {
				prb = player.GetComponentInParent<Rigidbody2D>();
			} catch { }
		}

		//GetComponent<Obj>().Up();

		bool ally = false;
		if (GetComponent<Obj>().team == playerPlayer.team && playerPlayer.team != 0) {
			Arrowsprite.transform.GetChild(0).gameObject.SetActive(true);
			ally = true;
		} else {
			Arrowsprite.transform.GetChild(0).gameObject.SetActive(false);
		}

		if (gameObject.GetComponent<SpriteRenderer>().isVisible) {
			Arrowsprite.SetActive(false);
		} else {
			Arrowsprite.SetActive(true);

			//Angle
			var x0 = transform.position.x - player.transform.position.x;
			var y0 = transform.position.y - player.transform.position.y;
			float ang = Mathf.Atan2(y0, x0) * Mathf.Rad2Deg;

			//Position
			int dist;
			if (ally)
				dist = 5;
			else
				dist = 7;

			var x = dist * Mathf.Cos(ang * Mathf.Deg2Rad) + player.transform.position.x;
			var y = dist * Mathf.Sin(ang * Mathf.Deg2Rad) + player.transform.position.y;
			Vector2 pos = new Vector2(x, y);

			//Scale
			float d = Vector3.Distance(transform.position, player.transform.position);
			float Sca = 1 / (0.9f + Mathf.Exp(-2.5f + 0.01f * d)) + 0.1f;
			Sca = 1 / (0.01f + d/100);
			if (Sca >= 1.2f) { Sca = 1.2f; }
			if (ally) {
				Sca *= 0.75f;
			}

			//Color
			Color color = Color.white;
			if (srb && prb) {
				Vector2 direction = transform.position - player.transform.position;
				float vel = Vector2.Dot(srb.velocity, -direction) + Vector2.Dot(prb.velocity, direction);

				Vector3 Away = new Vector3(1,0,0);
				Vector3 Still = new Vector3(0.5f, 0.5f, 0.5f);
				Vector3 To = new Vector3(0, 0, 1);
				Vector3 cV = Still;
				if (vel > 0) {//Towards
					cV = Vector3.Lerp(Still, To, Mathf.Abs(vel) / 5000);
				} else if (vel < 0) {//To
					cV = Vector3.Lerp(Still, Away, Mathf.Abs(vel) / 5000);
				}
				//cV = Vector3.Lerp(Away, To, (vel + 5000) / 10000);
				color = new Color(cV.x, cV.y, cV.z);
			}

			SR.color = color;
			Arrowsprite.transform.localScale = new Vector2(Sca, Sca);
			Arrowsprite.transform.position = pos;
			Arrowsprite.transform.eulerAngles = new Vector3(0, 0, ang - 90);
		}
	}

	private void OnDestroy() {
		Destroy(Arrowsprite);
	}
}