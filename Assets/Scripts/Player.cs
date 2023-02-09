using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour {

	public float Sensitivity;
	public float Deadzone;

	public bool AI;
	public int team;

	public float direction;
	public int thrust;
	public int fire;
	public string input;

	public bool mounted;
	public int BodyID = -1;

	public float HealthRT = 0;
	public float coolDown = 0;
	public float score = 0;
	public float time = 9999;

	playerCanvas pC;
	ParticleSystem PS;

	public string playerName;
	protected float timeOrg = 999;
	protected bool ending;
	protected float roll;
	protected bool timeSet;
	private float oldScore;
	private float oldHealth;
	private float hit;
	private float damage;
	public string scoreBoard;
	private float scoreChangeIndicatorTimer;

	private float cDTime;

	protected void Start() {
		if (!AI) {
			playerName = PreLoad.Name;
			pC = GameObject.FindGameObjectWithTag("Canvas").GetComponent<playerCanvas>();
			PS = gameObject.GetComponentInChildren<ParticleSystem>();
			PS.Stop();

			if (SystemInfo.deviceType == DeviceType.Handheld) {
				pC.MobileControls.SetActive(true);
			} else {
				pC.MobileControls.SetActive(false);
			}

			pC.Compass.transform.Find("Deadzone").localScale = new Vector3(Deadzone * 2 * 100, 1, 1);
		}
		try {
			pC.IPI.text = GameObject.Find("HOST").GetComponent<ConnectionManager>().IP.ToString();
		} catch { }

		if (!AI) {
			StartCoroutine(InitGyro());
			PS.Play();
		}
	}

	private void Update() {
		if (!timeSet && time != 9999 && time != 0) {
			timeOrg = time;
			timeSet = true;
		}

		//pC.timeI.text = Mathf.RoundToInt((time - time % 60) / 60) + ":" + Mathf.RoundToInt(time % 60);
		pC.timeI.text = Mathf.Floor(time / 60).ToString() + ":" + Mathf.Floor(time % 60).ToString("00");

		scoreChangeIndicatorTimer -= Time.deltaTime;
		if (pC.scoreI.text != Mathf.RoundToInt(score).ToString()) {
			pC.scoreChangeI.text = Mathf.RoundToInt(score - float.Parse(pC.scoreI.text)).ToString();
			if (score - float.Parse(pC.scoreI.text) > 0) {
				pC.scoreChangeI.color = Color.green;
			} else {
				pC.scoreChangeI.color = Color.red;
			}
			scoreChangeIndicatorTimer = 3;
		}

		Color tempColor = pC.scoreChangeI.color;
		tempColor.a = scoreChangeIndicatorTimer / 3;
		pC.scoreChangeI.color = tempColor;
		pC.scoreI.text = Mathf.RoundToInt(score).ToString();
		pC.ScoreBoard.text = scoreBoard;
		pC.CDI.fillAmount = coolDown/2;
		float timePercent = time / timeOrg;

		if (score > oldScore) {
			hit = score - oldScore;
		}
		oldScore = score;
		pC.HitIndicator.color = new Color(1,1,1, hit / 50);
		hit -= Time.deltaTime * 20;

		if (mounted) {
			//team = GetComponentInParent<Obj>().team;
			if (HealthRT < oldHealth) {
				damage = oldHealth - HealthRT;
			}
			oldHealth = HealthRT;
		}
		pC.DamageIndicator.color = new Color(1, 0, 0, damage / 50);
		damage -= Time.deltaTime * 20;

		#region Gyrocontrol
		//else {
		//	if (Input.gyro != null) {
		//		Input.gyro.enabled = true;
		//		roll = Input.acceleration.x * -10 * Sensitivity;
		//		pC.Compass.transform.Find("Needle").localPosition = new Vector3(-roll * 100, 0, 0);
		//		if (Mathf.Abs(roll) > Mathf.Abs(Deadzone))
		//			direction += roll;
		//		pC.GText.text = roll.ToString();

		//	}
		//}

		//if (direction > 180)
		//	direction -= 360;
		//else if (direction < -180)
		//	direction += 360;
		#endregion

		pC.orientationIndicator.transform.localRotation = transform.rotation;
		pC.orientationIndicator.SetActive(mounted);
		pC.directionIndicator.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, direction));
		pC.directionIndicator.SetActive(mounted);
		if (direction > -190 && direction < 190) {
			if (!pC.directionIndicator.activeSelf)
				pC.directionIndicator.SetActive(true);
			
			var PSc = PS.colorOverLifetime;
			Gradient G = new Gradient();
			G.SetKeys(new GradientColorKey[] { new GradientColorKey(Color.Lerp(Color.grey, new Color(1, 0.5f, 0), timePercent), 0.0f), new GradientColorKey(Color.Lerp(Color.black, Color.red, timePercent), 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f) });
			PSc.color = G;
		} else {
			pC.directionIndicator.SetActive(false);
		}

		if (mounted) {
			pC.HRTI.fillAmount = HealthRT / 100;
			PS.gameObject.SetActive(true);

			if (SystemInfo.deviceType != DeviceType.Handheld) {
				direction = GetAngle(Camera.main.ScreenToWorldPoint(Input.mousePosition));
				if (Input.GetButton("Fire"))
					fire = 1;
				else
					fire = 0;
				if (Input.GetKey(KeyCode.W))
					thrust = -1;
				else if (Input.GetButton("Move"))
					thrust = 1;
				else
					thrust = 0;

			}
			Camera.main.orthographicSize = 15;
		} else if (BodyID != -1) {
			try {
				transform.SetParent(ConnectionManager.GetObj(BodyID).gameObject.transform, false);
				transform.localPosition = Vector3.zero;
				transform.localRotation = Quaternion.Euler(0, 0, 0);
			} catch (System.Exception e) { print(e); }
		} else {
			try {
				PS.gameObject.SetActive(false);
				Camera.main.orthographicSize = 200;
				pC.HRTI.fillAmount = HealthRT / 5;
			} catch (System.Exception e) { print(e); }
		}
		input = "(" + direction.ToString() + ")(" + thrust.ToString() + ")(" + fire.ToString() + ")(" + playerName + ")";
	}
	protected void FixedUpdate() {


		float t = cDTime - coolDown;
		//if (t != 0)
		cDTime = coolDown;


		if (time < 0 && !ending) {
			ending = true;
			StartCoroutine(Endgame());
		}
		if (transform.parent) {
			mounted = true;
			gameObject.GetComponentInParent<Obj>().objectName = playerName;
		} else {
			mounted = false;
		}
		//if (!mounted && direction != -909) {
		//	gameObject.transform.position += new Vector3(Mathf.Cos((direction + 90) * Mathf.Deg2Rad), Mathf.Sin((direction + 90) * Mathf.Deg2Rad), 0) * 10 * Time.fixedDeltaTime;
		//}
	}

	protected IEnumerator InitGyro() {
		if (SystemInfo.supportsGyroscope) {
			Input.gyro.enabled = false;
			Input.gyro.enabled = true;
		}
		yield return null;
	}
	protected float GetAngle(Vector2 pos) {
		var x0 = transform.position.x - pos.x;
		var y0 = transform.position.y - pos.y;
		float angle = (Mathf.Atan2(y0, x0) * Mathf.Rad2Deg) + 90;
		if (angle > 180) {
			angle -= 360;
		} else if (angle < -180) {
			angle += 360;
		}
		return angle;
	}
	public void SetThrust(int state) {
		if (thrust == state)
			thrust = 0;
		else
			thrust = state;
	}
	public void SetFire(int state) {
		fire = state;
	}

	protected IEnumerator Endgame() {
		if (time == -707) {
			pC.VictorScreen.SetActive(true);
		} else {
			pC.LossScreen.SetActive(true);
		}
		pC.ScoreBoard.color = Color.white;
		yield return new WaitForSeconds(10);
		SceneManager.LoadScene("Lobby");
	}
	public void End() {
		SceneManager.LoadScene("Lobby");
	}
}