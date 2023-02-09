using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Linq;
using System.Threading;

public class Connector : MonoBehaviour {
	//public static GameObject CGI;

	//public bool host;
	//public static bool teams;
	//public Server server;
	//public Client client;
	//public IPAddress IP;
	////
	//public GameObject ClientGhost;
	//public GameObject AI;
	//public static float time = 9999; //Seconds

	//#region printList
	//public static void printList(List<Obj> List, string Identifier) {
	//	string str = "";
	//	foreach (var item in List) {
	//		str += item.ID + ", ";
	//	}
	//	print(Identifier + str);
	//}
	//public static void printList(List<ObjectPacket> List, string Identifier) {
	//	string str = "";
	//	foreach (var item in List) {
	//		str += item.ID + ", ";
	//	}
	//	print(Identifier + str);
	//}
	//#endregion
	//private void Start() {
	//	CGI = (GameObject)Resources.Load("ClientGhost");
	//	print("CGI: " + CGI);

	//	if (host) {
	//		teams = PreLoad.Team;
	//		StartServer();
	//		time = PreLoad.Time * 60;
	//	} else {
	//		IP = IPAddress.Parse(PreLoad.ip);
	//		StartClient();
	//	}
	//}
	//private void FixedUpdate() {
	//	if (time < 0) {
	//		time = -1;
	//	} else {
	//		time -= Time.fixedDeltaTime;
	//	}
	//}

	//public void StartServer() {
	//	server = gameObject.AddComponent<Server>();
	//	IP = server.invoke();
	//	if (PreLoad.AI == 1)
	//		SpawnAI(PreLoad.AICount, server);
	//}
	//public void StartClient() {
	//	client = gameObject.AddComponent<Client>();
	//	client.invoke(IP);
	//}
	//public void SpawnAI(int Count, Server server) {
	//	for (int i = 0; i < Count; i++) {
	//		GameObject GO = Instantiate(AI, Vector3.zero, Quaternion.identity);
			
	//		if (teams) {
	//			ClientGhost CG = GO.GetComponent<ClientGhost>();
	//			if (server.teams.x > server.teams.y) {
	//				CG.team = 2;
	//				server.teams.y += 1;
	//			} else {
	//				CG.team = 1;
	//				server.teams.x += 1;
	//			}
	//		}
	//	}
	//}

	//#region Syncing
	//public static ClientGhost SpawnNewClientGhost(Connection Client) {
	//	print("Client Spawning");
	//	print(CGI);
	//	GameObject GO = Instantiate(CGI, Vector3.zero, Quaternion.identity);
	//	GO.GetComponent<ClientGhost>().client = Client;
	//	return GO.GetComponent<ClientGhost>();
	//}

	//public static List<ClientGhost> GetCGs(int team) {
	//	List<GameObject> GO = new List<GameObject>();
	//	List<ClientGhost> List = new List<ClientGhost>();
	//	GO.AddRange(GameObject.FindGameObjectsWithTag("Player"));
	//	GO.AddRange(GameObject.FindGameObjectsWithTag("Ghost"));
	//	foreach (GameObject Object in GO) {
	//		ClientGhost CG = Object.GetComponent<ClientGhost>();
	//		if (CG.team == team)
	//			List.Add(CG);
	//	}
	//	return List;
	//}
	//public static List<Obj> GetGame() {
	//	List<GameObject> GO = new List<GameObject>();
	//	List<Obj> ObjList = new List<Obj>();
	//	GO.AddRange(GameObject.FindGameObjectsWithTag("Object"));
	//	GO.AddRange(GameObject.FindGameObjectsWithTag("Bullet"));
	//	foreach (GameObject Object in GO) {
	//		Obj Ob = Object.GetComponent<Obj>();
	//		Ob.Up();
	//		ObjList.Add(Ob);
	//	}
	//	//printList(ObjList, "OL: ");
	//	return ObjList;
	//}
	//public static void SetGame(List<ObjectPacket> List) {
	//	List<Obj> Objects = GetGame();
	//	Objects = Objects.OrderBy(f => f.ID).ToList();
	//	List = List.OrderBy(f => f.ID).ToList();

	//	//printList(List, "List: ");
	//	//printList(Objects, "Objects: ");

	//	for (int i = 0; i < List.Count; i++) {
	//		redo:
	//		if (Objects.Count-1 >= i) {
	//			if (List[i].ID == Objects[i].ID) {//Sync
	//				Objects[i].objectName = List[i].objectName;
	//				Objects[i].Pos = List[i].Pos;
	//				Objects[i].Vel = List[i].Vel;

	//			} else if (List[i].ID < Objects[i].ID) {//Create
	//				GameObject Object = Instantiate((GameObject)Resources.Load(List[i].Type));
	//				Objects.Insert(i, Object.GetComponent<Obj>());
	//				Objects[i].objectName = List[i].objectName;
	//				Objects[i].ID = List[i].ID;
	//				Objects[i].Pos = List[i].Pos;
	//				Objects[i].Vel = List[i].Vel;

	//			} else {//Destroy
	//				Obj Ob = Objects[i];
	//				Objects.Remove(Ob);
	//				//print("Destroying Object: " + Ob.ID);
	//				Ob.Kill();
	//				goto redo;

	//			}
	//		} else {//Create2 - Objects wasn't long enough
	//			GameObject Object = Instantiate((GameObject)Resources.Load(List[i].Type));
	//			Objects.Insert(i, Object.GetComponent<Obj>());
	//			Objects[i].objectName = List[i].objectName;
	//			Objects[i].ID = List[i].ID;
	//			Objects[i].Pos = List[i].Pos;
	//			Objects[i].Vel = List[i].Vel;


	//		}
	//	}

	//	if (Objects.Count > List.Count) {//Destroy2 - More Objects than List
	//		for (int i = 0; i < Objects.Count; i++) {
	//			redo:
	//			if (List.Count - 1 >= i) {
	//				if (List[i].ID > Objects[i].ID) {//Destroy
	//					Obj Ob = Objects[i];
	//					Objects.Remove(Ob);
	//					//print("Destroying Object: " + Ob.ID);
	//					Ob.Kill();
	//					goto redo;
	//				}
	//			}
	//		}
	//	}

	//	foreach (var item in Objects) {
	//		item.Down();
	//	}
	//}

	//public static List<ObjectPacket> ParseGame(string Game) {
	//	Game = Game.Split('}')[0].Replace("{", "");
	//	string[] Objects = Game.Replace("[", "").Split('>')[1].Split(']');
	//	List<ObjectPacket> ObjectList = new List<ObjectPacket>();

	//	try {
	//		string[] input = Game.Split('>')[0].Replace("<", "").Split(',');
	//		Player player = GameObject.Find("Controller").GetComponent<Player>();
	//		player.BodyID = int.Parse(input[0]);
	//		player.HealthRT = float.Parse(input[1]);
	//		player.coolDown = float.Parse(input[2]);
	//		player.score = float.Parse(input[3]);
	//		player.time = float.Parse(input[4]);
	//		player.scoreBoard = input[5];
	//	} catch (Exception e) { print(e); }

	//	foreach (string Object in Objects) {
	//		if (Object != "") {
	//			ObjectPacket obj = new ObjectPacket();
	//			ObjectList.Add(obj);
	//			string[] split = Object.Split(')');

	//			for (int i = 0; i < 5; i++) {
	//				split[i] = split[i].Replace("(", "");

	//				if (i == 0) {
	//					obj.objectName = split[i];
	//				}  else if (i == 1) {
	//					obj.ID = int.Parse(split[i]);
	//				} else if (i == 2) {
	//					obj.Type = split[i];
	//				} else if (i == 3) {
	//					string[] sp = split[i].Split(',');
	//					obj.Pos = new Vector3(float.Parse(sp[0]), float.Parse(sp[1]), float.Parse(sp[2]));
	//				} else if (i == 4) {
	//					string[] sp = split[i].Split(',');
	//					obj.Vel = new Vector3(float.Parse(sp[0]), float.Parse(sp[1]), float.Parse(sp[2]));
	//				} else if (i == 5) {
	//					obj.team = int.Parse(split[i]);
	//				}
	//			}
	//		}
	//	}
	//	return ObjectList;
	//}
	//public static string ParseGame(List<Obj> List, int ID, float HRT, float CD, float SCR, float tme, string SB) {
	//	string Game = "{<";
	//	Game += ID + "," + HRT + "," + CD + "," + SCR + "," + tme + "," + SB + ">";
	//	foreach (Obj Object in List) {
	//		Game += "[(" + Object.objectName + ")";
	//		Game += "(" + Object.ID.ToString() + ")";
	//		Game += "(" + Object.Type + ")";
	//		Vector3 vec = Object.Pos;
	//		Game += "(" + vec.x + "," + vec.y + "," + vec.z + ")";
	//		vec = Object.Vel;
	//		Game += "(" + vec.x + "," + vec.y + "," + vec.z + ")";
	//		Game += "]";
	//	}
	//	Game += "}";
	//	return Game;
	//}

	//public static Obj GetObj(int ID) {
	//	return GetGame().Find(x => x.ID == ID);
	//}
	//#endregion
}

//public class Server : MonoBehaviour {
//	public IPAddress IP;
//	Connection receiver;

//	public List<Connection> clients = new List<Connection>();
//	public Vector2 teams = new Vector2(1,0);

//	public IPAddress invoke() {
//		foreach (var ip in Dns.GetHostEntry("").AddressList) {
//			if (ip.AddressFamily == AddressFamily.InterNetwork) {
//				IP =  ip;
//				break;
//			}
//		}
//		receiver = new Connection(8080, this);
//		return IP;
//	}

//	public void Update() {
//		try {
//			print(receiver + " -C0: " + clients[0] + " -Connector: " + GameObject.Find("HOST").GetComponent<Connector>() + " -CGI: " + Connector.CGI);
//		} catch (Exception e) { print(e); }
//	}

//	public void NewClient(Connection client) {

//		print("Server New Client");
//		clients.Add(client);
//		print("Client Added");
//		print("CONNECTOR: " + GameObject.Find("HOST"));
//		print("CONNECTOR: " + GameObject.Find("HOST").GetComponent<Connector>());
//		ClientGhost CG = Connector.SpawnNewClientGhost(client);
//		print("CG Spawned");

//		if (Connector.teams) {
//			if (teams.x > teams.y) {
//				CG.team = 2;
//				teams.y += 1;
//			} else {
//				CG.team = 1;
//				teams.x += 1;
//			}
//		}
//	}
//}

//public class Client : MonoBehaviour {
//	bool Active;
//	bool Ticking;
//	IPAddress IP;
//	Connection client;
//	Player player;

//	public void invoke(IPAddress ip) {
//		player = GameObject.Find("Controller").GetComponent<Player>();
//		client = new Connection(new IPEndPoint(ip, 8080));
//		client.Send("Connect");
//		IP = ip;
//		Active = true;
//		Debug.LogError("Active");
//	}
//	void FixedUpdate() {
//		if (!Ticking) {
//			Tick();
//		}
//	}

//	void Tick() {
//		if (!Active)
//			return;
//		Ticking = true;

//		try {
//			Debug.Log("Tick");
//			string msg = client.Recieve();
//			Debug.Log("msg: " + msg);
//			List<ObjectPacket> Game = Connector.ParseGame(msg);
//			Connector.SetGame(Game);

//			string input = player.input;
//			client.Send(input);
//		} catch (Exception e) { Debug.Log(e); }
//		Ticking = false;
//	}

//	//Communicates with ClientGhost
//}

//public class Connection {
//	public Server server;

//	public UdpClient UDPClient;
//	public IPEndPoint IPEP;

//	public Connection(IPEndPoint ipep, string s) {
//		UDPClient = new UdpClient();
//		UDPClient.Client.ReceiveTimeout = 5;
//		IPEP = ipep;
//	}
//	public Connection(IPEndPoint ipep) {//Client
//		UDPClient = new UdpClient();
//		UDPClient.Client.ReceiveTimeout = 5;
//		IPEP = ipep;
//	}
//	public Connection(int port, Server srvr) {//Server
//		Debug.Log("Connection object made");
//		UDPClient = new UdpClient(port);
//		UDPClient.Client.ReceiveTimeout = 5;
//		IPEP = null; server = srvr;
//		UDPClient.BeginReceive(ServerListen, null);
//	}

//	public void ServerListen(IAsyncResult ar) {
//		Debug.Log("Heard Something");
//		//try {
//			IPEndPoint ipep = null;
//			byte[] data = UDPClient.EndReceive(ar, ref ipep);
//			string msg = Encoding.UTF8.GetString(data);

//			if (msg == "Connect") {
//				Debug.Log("Client Connecting");
//				server.NewClient(new Connection(ipep));
//			}

//		//} catch (SocketException e) {
//		//	Debug.Log(e);
//		//	// This happens when a client disconnects, as we fail to send to that port.
//		//}
//		Debug.Log("SERVER CONNECTOR BR");
//		UDPClient.BeginReceive(ServerListen, null);
//	}

//	public void Send(string message) {
//		byte[] data = Encoding.UTF8.GetBytes(message);
//		UDPClient.Send(data, data.Length, IPEP);
//	}
//	public string Recieve() {
//		byte[] data = UDPClient.Receive(ref IPEP);
//		return Encoding.UTF8.GetString(data);
//	}

//	public void End() {
//		UDPClient.Close();
//	}
//}

//public class ObjectPacket {
//	public string objectName;
//	public int ID;
//	public string Type;
//	public Vector3 Pos;
//	public Vector3 Vel;
//	public int team;
//}