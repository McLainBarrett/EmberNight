using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

#pragma warning disable CS4014

public class ClientGhost : MonoBehaviour {

	public bool isServerSide;

	public GameObject GameBody;
	public Connection client;
	public Body Body;

	int BodyID = -1;
	public float HealthRT = 0;
	public float coolDown = 0;
	public float score = 0;

	public int team = 0;
	public string clientName;
	string input;
	bool mounted = true;
	public Player HostPlayer;
	public bool Communicating;
	float respawn;
	bool ded;
	public float time;
	public string ScoreBoard;
	//
	public int Trys = 3;

	private void Start() {
		if (ConnectionManager.teams && isServerSide && !HostPlayer.AI) {
			team = 1;
		}
	}

	private void FixedUpdate() {
		time = ConnectionManager.time;

		if (team != 0) {
			List<ClientGhost> Reds = ConnectionManager.GetCGs(1);
			List<ClientGhost> Blues = ConnectionManager.GetCGs(2);
			Reds = Reds.OrderBy(f => -f.score).ToList();
			Blues = Blues.OrderBy(f => -f.score).ToList();

			Vector2 teamScore = new Vector2();
			foreach (ClientGhost CG in Reds)
				teamScore.x += CG.score;
			foreach (ClientGhost CG in Blues)
				teamScore.y += CG.score;

			ScoreBoard = "";
			if (teamScore.y > teamScore.x) {
				ScoreBoard += "--Blu: " + Mathf.RoundToInt(teamScore.y) + "\n";
				LeaderBoardAssemble(Blues);
				ScoreBoard += "--Red: " + Mathf.RoundToInt(teamScore.x) + "\n";
				LeaderBoardAssemble(Reds);
			} else {
				ScoreBoard += "--Red: " + Mathf.RoundToInt(teamScore.x) + "\n";
				LeaderBoardAssemble(Reds);
				ScoreBoard += "--Blu: " + Mathf.RoundToInt(teamScore.y) + "\n";
				LeaderBoardAssemble(Blues);
			}

			if (time < 0) {
				if ((teamScore.x >= teamScore.y && team == 1) || (teamScore.y > teamScore.x && team == 2)) {
					time = -707;
				}
			}
		} else {
			List<ClientGhost> CGL = ConnectionManager.GetCGs(0);
			CGL = CGL.OrderBy(f => -f.score).ToList();

			ScoreBoard = "";
			LeaderBoardAssemble(CGL);

			if (time < 0) {
				if (CGL[0] == gameObject.GetComponent<ClientGhost>()) {
					time = -707;
				}
			}
		}

		if (Body) {
			coolDown = Body.cooldown;
			mounted = true;
			BodyID = Body.gameObject.GetComponent<Obj>().ID;
			Body.gameObject.GetComponent<Obj>().team = team;
		} else if (mounted) {
			coolDown = 0;
			mounted = false;
			BodyID = -1;
			ded = true;
			respawn = 5;
		}
		
		if (!isServerSide && !Communicating)
			Communicate();
		else if (isServerSide)
			HostC();

		if (ded) {
			respawn -= Time.fixedDeltaTime;
			HealthRT = respawn;
			if (respawn <= 0)
				Respawn();
		}
	}
	private void LeaderBoardAssemble(List<ClientGhost> list) {
		for (int i = 0; i < list.Count; i++) {
			ScoreBoard += i.ToString() + ")   " + Mathf.RoundToInt(list[i].score) + " - " + list[i].clientName;
			if (list[i] == this)
				ScoreBoard += " <";
			ScoreBoard += "\n";
		}
	}

	private async Task Communicate() {
		Communicating = true;
		string Game = ConnectionManager.ParseGame(ConnectionManager.Game, BodyID, HealthRT, coolDown, score, time, ScoreBoard);
		try {
			client.Send(Game, true);
			string msg = await client.RecieveAsync();
				
			if (mounted && msg != "") {
				string[] Msg = msg.Replace("(", "").Split(')');
				Body.Direction = float.Parse(Msg[0]);
				Body.Thrust = int.Parse(Msg[1]);
				Body.Fire = int.Parse(Msg[2]);
				clientName = Msg[3];
				GetComponentInParent<Obj>().objectName = clientName;
			}
		} catch (System.Exception e) { Debug.Log(e);
			Trys--;
			if (Trys <= 0) {
				print("ENDING CONNECTION");
				EndConnection();
			}
		}
		Communicating = false;
	}
	private void HostC() {
		if (Body) {
			Body.Direction = HostPlayer.direction;
			Body.Thrust = HostPlayer.thrust;
			Body.Fire = HostPlayer.fire;
		}
		HostPlayer.HealthRT = HealthRT;
		HostPlayer.coolDown = coolDown;
		HostPlayer.score = score;
		HostPlayer.time = time;
		HostPlayer.scoreBoard = ScoreBoard;
		HostPlayer.team = team;
		clientName = HostPlayer.playerName;
	}

	private void Respawn() {
		ded = false;
		HealthRT = 100;
		GameObject b = Instantiate(GameBody, new Vector3(Random.Range(-150f, 150f), Random.Range(-150f, 150f), 0), Quaternion.identity);
		Body = b.GetComponent<Body>();
		transform.SetParent(b.transform, false);
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.Euler(0,0,0);
	}
	public void EndConnection() {
		ConnectionManager CM = GameObject.Find("HOST").GetComponent<ConnectionManager>();
		CM.Connections.Remove(client);
		client.End();
		if (ConnectionManager.teams) {
			if (team == 1)
				CM.teamCount.x -= 1;
			else if (team == 2)
				CM.teamCount.y -= 1;
		}
		if (transform.parent)
			Destroy(transform.parent.gameObject);
		else
			Destroy(gameObject);
	}
}