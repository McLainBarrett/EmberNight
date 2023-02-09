using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameTagScript : MonoBehaviour {

	private Text txt;
	private Obj obj;

	private void Start() {
		txt = transform.Find("Canvas").GetComponentInChildren<Text>();
		obj = GetComponent<Obj>();
	}

	private void Update() {
		txt.text = obj.objectName;
		try {
			if (obj.team == 1) {
				txt.color = Color.red;
			} else if (obj.team == 2) {
				txt.color = Color.blue;
			}
		} catch { }
	}
}