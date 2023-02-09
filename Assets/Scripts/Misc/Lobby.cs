using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Lobby : MonoBehaviour {

	public GameObject ServerButton;
	
	public Toggle AI;
	public InputField AICount;
	public InputField Time;
	public Toggle Team;

	public InputField Name;
	public InputField Sensitivity;
	public InputField Deadzone;

	private void Start() {
		PreLoad.LoadSet();
		Name.text = PreLoad.Name;
		Sensitivity.text = PreLoad.Sensitivity.ToString();
		Deadzone.text = PreLoad.Deadzone.ToString();
		Time.text = PreLoad.Time.ToString();
		AICount.text = PreLoad.AICount.ToString();
		Team.isOn = PreLoad.Team;

		if (PreLoad.AI == 1) {
			AI.isOn = true;
		}
	}
	 
	public void RefreshServerList() {
		RSL();
	}

	private void RSL() {
		List<string> list = ConnectionManager.PingForServers();
		Transform SBs = transform.Find("Joining").Find("ServerButtons");

		for (int i = SBs.childCount - 1; i >= 0; i--) {
			Destroy(SBs.GetChild(i).gameObject);
		}

		for (int i = 0; i < list.Count; i++) {
			buttonScript button = Instantiate(ServerButton, SBs).GetComponent<buttonScript>();
			button.transform.localPosition = new Vector2(0, -75 * i);
			string[] shards = list[i].Split('*');
			button.ServerNameText.text = shards[0];
			button.ServerDetailText.text = shards[1];
			string[] ipep = shards[2].Split(':');
			button.IPEP = new IPEndPoint(IPAddress.Parse(ipep[0]), int.Parse(ipep[1]));

		}
	}

	public void ButtonHost() {
		SceneManager.LoadScene("Host");
		PreLoad.SaveSet();
	}
	public void ButtonJoin() {
		SceneManager.LoadScene("Client");
	}

	public void SetName() {
		PreLoad.Name = Name.text;
	}
	public void SetTime() {
		float e;
		if (float.TryParse(Time.text, out e)) {
			PreLoad.Time = e;
		}
	}
	public void SetAIbool() {
		if (!AI.isOn) {
			PreLoad.AI = 0;
		} else {
			PreLoad.AI = 1;
		}
	}
	public void SetAIint() {
		int e;
		if (int.TryParse(AICount.text, out e)) {
			PreLoad.AICount = e;
		}
	}
	public void SetTeambool() {
		PreLoad.Team = Team.isOn;
	}
	public void SetIP() {
		foreach (var ip in Dns.GetHostEntry("").AddressList) {
			if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) {
				PreLoad.ip = ip.ToString();
				break;
			}
		}
	}
	public void SetSensitivity() {
		PreLoad.Sensitivity = float.Parse(Sensitivity.text);
		PreLoad.SaveSet();
	}
	public void SetDeadzone() {
		PreLoad.Deadzone = float.Parse(Deadzone.text);
		PreLoad.SaveSet();
	}
}