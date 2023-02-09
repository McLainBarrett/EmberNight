using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class buttonScript : MonoBehaviour {

	public Text ServerNameText;
	public Text ServerDetailText;
	public IPEndPoint IPEP;
	public Image Indicatior;

	public playerCanvas pC;
	private Player player;
	[SerializeField] public buttonType bT;
	private Text text;

	private void Start() {
		try {
			player = pC.player;
			text = GetComponentInChildren<Text>();
		} catch { }
	}
	private void Update() {
		if (!player) {
			Start();
		}
		try {
			if (bT == buttonType.Fire) {
				if (player.fire == 1)
					text.color = Color.white;
				else
					text.color = Color.black;
			} else {
				if ((bT == buttonType.Thrust && player.thrust == 1) || (bT == buttonType.Break && player.thrust == -1))
					text.color = Color.white;
				else
					text.color = Color.black;
			}
		} catch { }
	}

	public void SetIPEP() {
		if (IPEP != null) {
			if (PreLoad.IPEP == IPEP) {
				PreLoad.IPEP = null;
				foreach (buttonScript item in transform.parent.GetComponentsInChildren<buttonScript>()) {
					item.Indicatior.gameObject.SetActive(false);
				}
			} else {
				PreLoad.IPEP = IPEP;
				foreach (buttonScript item in transform.parent.GetComponentsInChildren<buttonScript>()) {
					item.Indicatior.gameObject.SetActive(false);
				}
				Indicatior.gameObject.SetActive(true);
			}
		}
	}
	public void TouchDown() {
		switch(bT) {
			case buttonType.Fire:
				player.SetFire(1);
				break;
			case buttonType.Thrust:
				player.SetThrust(1);
				break;
			case buttonType.Break:
				player.SetThrust(-1);
				break;
			case buttonType.Back:
				SceneManager.LoadScene("Lobby");
				break;
		}
	}
	public void TouchUp() {
		switch (bT) {
			case buttonType.Fire:
				player.SetFire(0);
				break;
			case buttonType.Thrust:
				player.SetThrust(0);
				break;
			case buttonType.Break:
				player.SetThrust(0);
				break;
		}
	}
}

public enum buttonType {
	Fire,
	Thrust,
	Break,
	Back,
	None
}