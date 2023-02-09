using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PreLoad : MonoBehaviour {

	public static string Name;

	public static System.Net.IPEndPoint IPEP;

	public static int AI;
	public static int AICount;
	public static string ip;
	public static float Time;
	public static bool Team;

	public static float Sensitivity;
	public static float Deadzone;

	public static readonly List<string> Adjectives = new List<string>() {
"attractive",
"bald",
"beautiful",
"chubby",
"clean",
"dazzling",
"drab",
"elegant",
"fancy",
"fit",
"flabby",
"glamorous",
"gorgeous",
"handsome",
"long",
"magnificent",
"muscular",
"plain",
"plump",
"quaint",
"scruffy",
"shapely",
"short",
"skinny",
"stocky",
"ugly",
"unkempt",
"enraged",
"unsightly"
};
	public static readonly List<string> Animals = new List<string>() {
"cat",
"dog",
"bird",
"birb",
"bear",
"lion",
"monkey",
"snake",
"dolphin",
"fish",
"koala",
"whale",
"elephant",
"tiger",
"god"
};

	public static void SaveSet() {
		PlayerPrefs.SetFloat("Sensitivity", Sensitivity);
		PlayerPrefs.SetFloat("Deadzone", Deadzone);
		PlayerPrefs.SetInt("AI", AI);
		PlayerPrefs.SetInt("AICount", AICount);
		PlayerPrefs.SetFloat("Time", Time);
		PlayerPrefs.SetString("Name", Name);
		PlayerPrefs.SetInt("Team", Team ? 1 : 0);
	}
	public static void LoadSet() {
		Sensitivity = PlayerPrefs.GetFloat("Sensitivity", 1);
		Deadzone = PlayerPrefs.GetFloat("Deadzone", 1);
		AI = PlayerPrefs.GetInt("AI", 0);
		AICount = PlayerPrefs.GetInt("AICount", 0);
		Time = PlayerPrefs.GetFloat("Time", 5);
		Team = PlayerPrefs.GetInt("Team", 0) == 1 ? true : false;

		if (PlayerPrefs.HasKey("Name")) {
			Name = PlayerPrefs.GetString("Name");
		} else {
			Name = Adjectives[Random.Range(0, Adjectives.Count - 1)]
				+ Animals[Random.Range(0, Animals.Count - 1)]
				+ Random.Range(0, 9999).ToString();
		}
	}
}