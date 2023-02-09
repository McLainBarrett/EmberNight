using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerCanvas : MonoBehaviour {

	public Player player;

	public Image CDI;
	public Image HRTI;
	public Text scoreI;
	public Text scoreChangeI;
	public Text timeI;
	public Text IPI;
	public Image HitIndicator;
	public Image DamageIndicator;
	public GameObject Compass;

	public GameObject VictorScreen;
	public GameObject LossScreen;
	public GameObject MobileControls;
	public GameObject directionIndicator;
	public GameObject orientationIndicator;

	public Text ScoreBoard;
	public Text GText;

	private void Start() {
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
	}
}