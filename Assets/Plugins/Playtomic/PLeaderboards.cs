using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class PLeaderboards
{
	private const string SECTION = "leaderboards";
	private const string SAVEANDLIST = "saveandlist";
	private const string SAVE = "save";
	private const string LIST = "list";
	
	/**
	 * Saves a player's score
	 * @param	score	PlayerScore	The PlayerScore object 
	 * @param	callback	Action<PResponse> Your callback method
	 */
	public void Save(PlayerScore score, Action<PResponse> callback)
	{
		Save<PlayerScore>(score,callback);
	}
	
	public void Save<T>(T score, Action<PResponse> callback) where T : PlayerScore
	{
		Playtomic.API.StartCoroutine(SendSaveRequest(SECTION, SAVE, score, callback));
	}

	public void SaveAndList(PlayerScore score, Action<List<PlayerScore>, int, PResponse> callback)
	{
		SaveAndList<PlayerScore>(score,callback);
	}
	
	public void SaveAndList<T>(T score, Action<List<T>, int, PResponse> callback) where T : PlayerScore
	{
		Playtomic.API.StartCoroutine(SendListRequest(SECTION, SAVEANDLIST, score, callback));
	}
	
	/**
	 * Lists scores
	 * @param	options	Dictionary<string,object>	The listing options
	 * @param	callback	Action<List<PlayerScore>, int, PResponse>	Your callback function
	 */
	
	public void List(PLeaderboardOptions options, Action<List<PlayerScore>, int, PResponse> callback)
	{
		List<PlayerScore>(options,callback);		
	}
	
	public void List<T>(PLeaderboardOptions options, Action<List<T>, int, PResponse> callback)  where T : PlayerScore
	{	
		Playtomic.API.StartCoroutine(SendListRequest<T>(SECTION, LIST, options, callback));
	}
	
	private IEnumerator SendSaveRequest(string section, string action, Dictionary<string,object> postdata, Action<PResponse> callback)
	{ 
		var www = PRequest.Prepare (section, action, postdata);
		yield return www;
		
		var response = PRequest.Process(www);
		callback(response);
	}
	
	
	private IEnumerator SendListRequest<T>(string section, string action, Dictionary<string,object> postdata, Action<List<T>, int, PResponse> callback) where T : PlayerScore
	{ 
		var www = PRequest.Prepare (section, action, postdata);
		yield return www;
		
		var response = PRequest.Process(www);
		var data = response.json;
		List<T> scores;
		int numscores;
		ProcessScores<T>(response, data, out scores, out numscores);
		
		callback(scores, numscores, response);
	}
	
	private void ProcessScores<T>(PResponse response, Dictionary<string,object> data, out List<T> scores, out int numitems) where T : PlayerScore
	{
		scores = new List<T>();
		numitems = 0;
		
		if (response.success)
		{
			numitems = (int)(double)data["numscores"];
			var scorearr = (List<object>) data["scores"];
			
			for(var i=0; i<scorearr.Count; i++)
			{
				scores.Add((T) Activator.CreateInstance(typeof(T), new object[] { scorearr[i] }));
			}
		}
	}
}
