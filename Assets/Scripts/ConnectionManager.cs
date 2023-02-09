using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Linq;
using System.Threading;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class ConnectionManager : MonoBehaviour {
	#region Variables
	public Connection clientReciever;
	//
	public bool host;
	public static bool teams;
	public IPAddress IP;
	//
	public GameObject ClientGhost;
	public GameObject AI;
	public static float time = 9999;
	//
	public List<Connection> Connections = new List<Connection>();
	public Vector2 teamCount = new Vector2(1, 0);
	//
	public static IPEndPoint IPEP;
	//
	public static Player player;
	//
	public IPEndPoint newClient;
	//
	public int Trys = 3;
	//
	public static List<Obj> Game;
	//
	public static bool setGame;
	public static List<ObjectPacket> gameToSet;
	//
	private string msgPing = "";
	private IPEndPoint ipepPing;
	private float packetTime = 0;
	#endregion

	private void Start() {
		player = GameObject.Find("Controller").GetComponent<Player>();
		if (host) {
			teams = PreLoad.Team;
			StartServer();
			time = PreLoad.Time * 60;
		} else {
			try {
				IP = IPAddress.Parse(PreLoad.ip);
			} catch { }
			IPEP = PreLoad.IPEP;
			StartClient();
		}
	}
	private void FixedUpdate() {
		if (time < 0) {
			time = -1;
		} else {
			time -= Time.fixedDeltaTime;
		}
		Game = GetGame();
		if (setGame) {
			setGame = false;
			SetGame(gameToSet);
		}
	}
	private void Update() {
		if (newClient != null) {
			NewClient(new Connection(newClient, this));
			newClient = null;
		}
		if (msgPing != "") {
			print(msgPing != "");
			print(msgPing);
			SL(msgPing, ipepPing);
			msgPing = "";
		}
	}
	private void OnDestroy() {
		foreach (Connection conn in Connections) {
			conn.End();
		}
		clientReciever.End();
	}

	public void StartServer() {
		foreach (var ip in Dns.GetHostEntry("").AddressList) {
			if (ip.AddressFamily == AddressFamily.InterNetwork) {
				IP = ip;
				IPEP = new IPEndPoint(IP, 8080);
				break;
			}
		}

		clientReciever = new Connection(8080, this);

		if (PreLoad.AI == 1)
			SpawnAI(PreLoad.AICount);
		clientReciever.UDPClient.BeginReceive(ServerListen, null);
	}
	public void ServerListen(IAsyncResult ar) {
		Debug.Log("Heard Something");

		IPEndPoint ipep = null;
		byte[] data = clientReciever.UDPClient.EndReceive(ar, ref ipep);
		string msg = Encoding.UTF8.GetString(data);

		Debug.Log("Heard: " + msg);

		if (msg == "Connect") {
			Debug.Log("Client Connecting");
			newClient = ipep;
		} else if (msg == "EN_Ping_For_Server") {
			msgPing = msg;
			ipepPing = ipep;
		}

		clientReciever.UDPClient.BeginReceive(ServerListen, null);
	}
	public void NewClient(Connection CE) {
		Connections.Add(CE);
		ClientGhost CG = SpawnNewClientGhost(CE);

		if (teams) {
			if (teamCount.x > teamCount.y) {
				CG.team = 2;
				teamCount.y += 1;
			} else {
				CG.team = 1;
				teamCount.x += 1;
			}
		}
	}
	public void SpawnAI(int Count) {
		for (int i = 0; i < Count; i++) {
			GameObject GO = Instantiate(AI, Vector3.zero, Quaternion.identity);

			if (teams) {
				ClientGhost CG = GO.GetComponent<ClientGhost>();
				if (teamCount.x > teamCount.y) {
					CG.team = 2;
					teamCount.y += 1;
				} else {
					CG.team = 1;
					teamCount.x += 1;
				}
			}
		}
	}

	public void StartClient() {
		print(PreLoad.IPEP);
		clientReciever = new Connection(PreLoad.IPEP, this);
		clientReciever.Send("Connect", false);
		player = GameObject.Find("Controller").GetComponent<Player>();

		//ThreadStart childref = new ThreadStart(CL);
		//Thread childThread = new Thread(childref);
		//childThread.Start();

		////ClientListenAsync();
		clientReciever.UDPClient.BeginReceive(ClientListen, null);
	}
	public void CL() {
		clientReciever.UDPClient.BeginReceive(ClientListen, null);
	}
	public void ClientListen(IAsyncResult ar) {
		IPEndPoint ipep = null;

		try {
			byte[] data = clientReciever.UDPClient.EndReceive(ar, ref ipep);
			string msg = Encoding.UTF8.GetString(data);

			string timeFrame = msg.Split('>')[0] + ">";
			msg = msg.Replace(timeFrame, "");
			float time = float.Parse(timeFrame.Replace(">", "").Replace("<",  ""));

			if (time >= packetTime) {
				packetTime = time;
				gameToSet = ParseGame(msg);
				setGame = true;

				clientReciever.IPEP = ipep;
				clientReciever.Send(player.input, false);
			}
		} catch (Exception e) {
			Debug.Log(e);
			Trys--;
			if (Trys <= 0) {
				clientReciever.End();
			}
		}

		clientReciever.UDPClient.BeginReceive(ClientListen, null);
	}
	public void ClientListenAsync() {
		try {
			string msg = clientReciever.Recieve();

			string timeFrame = msg.Split('>')[0] + ">";
			msg = msg.Replace(timeFrame, "");
			float time = float.Parse(timeFrame.Replace(">", "").Replace("<", ""));

			if (time >= packetTime) {
				packetTime = time;
				gameToSet = ParseGame(msg);
				setGame = true;
				
				clientReciever.Send(player.input, false);
			}
		} catch (Exception e) {
			Debug.Log(e);
			Trys--;
			if (Trys <= 0) {
				clientReciever.End();
			}
		}

		ClientListenAsync();
	}

	public void SL(string msg, IPEndPoint ipep) {
		clientReciever.SendTo(GetSummary(), ipep);
	}
	public string GetSummary() {
		string summary = player.gameObject.GetComponent<ClientGhost>().clientName + "'s Server*Players: " + (Connections.Count + 1) + " - Bots: ";
		if (PreLoad.AI == 1) {
			summary += PreLoad.AICount;
		} else {
			summary += "0";
		}
		summary += " - Time Left: " + Mathf.Floor(time / 60).ToString() + ":" + Mathf.Floor(time % 60).ToString("00");
		if (teams)
			summary += " - Teams";
		summary += "*" + IPEP.ToString();
		return summary;
	}
	public static List<string> PingForServers() {
		List<string> list = new List<string>();
		Connection pinger = new Connection();
		pinger.Send("EN_Ping_For_Server", false);
		list = pinger.RecievePingReponse();
		pinger.End();
		return list;
	}

	#region Syncing
	public ClientGhost SpawnNewClientGhost(Connection Client) {
		print("SPAWNING");
		GameObject GO = Instantiate(ClientGhost, Vector3.zero, Quaternion.identity);
		GO.GetComponent<ClientGhost>().client = Client;
		return GO.GetComponent<ClientGhost>();
	}

	public static List<ClientGhost> GetCGs(int team) {
		List<GameObject> GO = new List<GameObject>();
		List<ClientGhost> List = new List<ClientGhost>();
		GO.AddRange(GameObject.FindGameObjectsWithTag("Player"));
		GO.AddRange(GameObject.FindGameObjectsWithTag("Ghost"));
		foreach (GameObject Object in GO) {
			ClientGhost CG = Object.GetComponent<ClientGhost>();
			if (CG.team == team)
				List.Add(CG);
		}
		return List;
	}
	public static List<Obj> GetGame() {
		List<GameObject> GO = new List<GameObject>();
		List<Obj> ObjList = new List<Obj>();
		GO.AddRange(GameObject.FindGameObjectsWithTag("Object"));
		GO.AddRange(GameObject.FindGameObjectsWithTag("Bullet"));
		foreach (GameObject Object in GO) {
			Obj Ob = Object.GetComponent<Obj>();
			Ob.Up();
			ObjList.Add(Ob);
		}
		//printList(ObjList, "OL: ");
		return ObjList;
	}
	public static void SetGame(List<ObjectPacket> List) {
		List<Obj> Objects = Game;
		Objects = Objects.OrderBy(f => f.ID).ToList();
		List = List.OrderBy(f => f.ID).ToList();

		//printList(List, "List: ");
		//printList(Objects, "Objects: ");

		for (int i = 0; i < List.Count; i++) {
			redo:
			if (Objects.Count - 1 >= i) {
				if (List[i].ID == Objects[i].ID) {//Sync
					Objects[i].objectName = List[i].objectName;
					Objects[i].Pos = List[i].Pos;
					Objects[i].Vel = List[i].Vel;

				} else if (List[i].ID < Objects[i].ID) {//Create
					GameObject Object = Instantiate((GameObject)Resources.Load(List[i].Type));
					Objects.Insert(i, Object.GetComponent<Obj>());
					Objects[i].objectName = List[i].objectName;
					Objects[i].ID = List[i].ID;
					Objects[i].Pos = List[i].Pos;
					Objects[i].Vel = List[i].Vel;
					Objects[i].team = List[i].team;

				} else {//Destroy
					Obj Ob = Objects[i];
					Objects.Remove(Ob);
					//print("Destroying Object: " + Ob.ID);
					Ob.Kill();
					goto redo;

				}
			} else {//Create2 - Objects wasn't long enough
				GameObject Object = Instantiate((GameObject)Resources.Load(List[i].Type));
				Objects.Insert(i, Object.GetComponent<Obj>());
				Objects[i].objectName = List[i].objectName;
				Objects[i].ID = List[i].ID;
				Objects[i].Pos = List[i].Pos;
				Objects[i].Vel = List[i].Vel;
				Objects[i].team = List[i].team;


			}
		}

		if (Objects.Count > List.Count) {//Destroy2 - More Objects than List
			for (int i = 0; i < Objects.Count; i++) {
				redo:
				if (List.Count - 1 >= i) {
					if (List[i].ID > Objects[i].ID) {//Destroy
						Obj Ob = Objects[i];
						Objects.Remove(Ob);
						//print("Destroying Object: " + Ob.ID);
						Ob.Kill();
						goto redo;
					}
				}
			}
		}

		foreach (var item in Objects) {
			item.Down();
		}
	}

	public static List<ObjectPacket> ParseGame(string game) {
		game = game.Split('}')[0].Replace("{", "");
		string[] Objects = game.Replace("[", "").Split('>')[1].Split(']');
		List<ObjectPacket> ObjectList = new List<ObjectPacket>();

		try {
			string[] input = game.Split('>')[0].Replace("<", "").Split(',');
			player.BodyID = int.Parse(input[0]);
			player.HealthRT = float.Parse(input[1]);
			player.coolDown = float.Parse(input[2]);
			player.score = float.Parse(input[3]);
			player.time = float.Parse(input[4]);
			player.scoreBoard = input[5];
		} catch (Exception e) { print(e); }

		foreach (string Object in Objects) {
			if (Object != "") {
				ObjectPacket obj = new ObjectPacket();
				ObjectList.Add(obj);
				string[] split = Object.Split(')');

				for (int i = 0; i < 6; i++) {
					split[i] = split[i].Replace("(", "");

					if (i == 0) {
						obj.objectName = split[i];
					} else if (i == 1) {
						obj.ID = int.Parse(split[i]);
					} else if (i == 2) {
						obj.Type = split[i];
					} else if (i == 3) {
						string[] sp = split[i].Split(',');
						obj.Pos = new Vector3(float.Parse(sp[0]), float.Parse(sp[1]), float.Parse(sp[2]));
					} else if (i == 4) {
						string[] sp = split[i].Split(',');
						obj.Vel = new Vector3(float.Parse(sp[0]), float.Parse(sp[1]), float.Parse(sp[2]));
					} else if (i == 5) {
						obj.team = int.Parse(split[i]);
					}
				}
			}
		}
		return ObjectList;
	}
	public static string ParseGame(List<Obj> List, int ID, float HRT, float CD, float SCR, float tme, string SB) {
		string Game = "{<";
		Game += ID + "," + HRT + "," + CD + "," + SCR + "," + tme + "," + SB + ">";
		foreach (Obj Object in List) {
			Game += "[(" + Object.objectName + ")";
			Game += "(" + Object.ID.ToString() + ")";
			Game += "(" + Object.Type + ")";
			Vector3 vec = Object.Pos;
			Game += "(" + vec.x + "," + vec.y + "," + vec.z + ")";
			vec = Object.Vel;
			Game += "(" + vec.x + "," + vec.y + "," + vec.z + ")";
			Game += "(" + Object.team + ")";
			Game += "]";
		}
		Game += "}";
		return Game;
	}

	public static Obj GetObj(int ID) {
		return GetGame().Find(x => x.ID == ID);
	}
	#endregion
}

public class Connection {
	public ConnectionManager CM;
	public UdpClient UDPClient;
	public IPEndPoint IPEP;

	public Connection(IPEndPoint ipep, ConnectionManager manager) {//Client/Server Connection
		CM = manager;
		UDPClient = new UdpClient();
		UDPClient.Client.ReceiveTimeout = 5000;
		//UDPClient.Connect(ipep);
		IPEP = ipep;
		Debug.Log("Connection made");
	}
	public Connection(int port, ConnectionManager manager) {//Server
		Debug.Log("Server Connection object made");
		CM = manager;
		UDPClient = new UdpClient(port);
		UDPClient.Client.ReceiveTimeout = 5000;
		IPEP = null;
	}
	public Connection() {//Pinger
		Debug.Log("Pinger made");
		UDPClient = new UdpClient();
		UDPClient.Client.ReceiveTimeout = 500;
		IPEP = new IPEndPoint(IPAddress.Broadcast, 8080);
	}

	public void Send(string message, bool sendTimeStamp) {
		byte[] data = Encoding.UTF8.GetBytes(message);
		if (sendTimeStamp)
			data = Encoding.UTF8.GetBytes("<" + Time.unscaledTime / 1000 + ">" + message);
		UDPClient.Send(data, data.Length, IPEP);
	}
	public void SendTo(string message, IPEndPoint ipep) {
		//Debug.Log("sending: " + message);
		byte[] data = Encoding.UTF8.GetBytes(message);
		UDPClient.Send(data, data.Length, ipep);
	}
	public string Recieve() {
		IPEndPoint ipep = null;
		byte[] data = UDPClient.Receive(ref ipep);
		return Encoding.UTF8.GetString(data);
	}
	public async Task<string> RecieveAsync() {
		UdpReceiveResult data = await UDPClient.ReceiveAsync();
		return Encoding.UTF8.GetString(data.Buffer);
	}
	public List<string> RecievePingReponse() {
		List<string> list = new List<string>();
		Debug.Log("Awaiting Responses");

		CancellationTokenSource CTS = new CancellationTokenSource(500);
		while (!CTS.Token.IsCancellationRequested) {
			try {
				Task<string> task = Task.Factory.StartNew(() => {
					return Recieve();
				}, CTS.Token);
				list.Add(task.Result);
				Debug.Log("Added response");
			} catch (Exception e){ Debug.Log(e); }
		}

		Debug.Log("Finished Awaiting Responses");
		return list;
	}

	public void End() {
		UDPClient.Close();
	}
}

public class ObjectPacket {
	public string objectName;
	public int ID;
	public string Type;
	public Vector3 Pos;
	public Vector3 Vel;
	public int team;
}