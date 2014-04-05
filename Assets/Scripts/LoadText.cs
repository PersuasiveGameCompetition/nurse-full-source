using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

///<SUMMARY>
/// Right now this is basically all the ingame logic...since it's pretty simple,
/// I don't imagine I'll be refactoring much, but I could make it more modular.
///</SUMMARY>

[RequireComponent(typeof(AudioSource))]
public class LoadText : MonoBehaviour {
	public StringReader reader;
	public TextAsset room; //Don't touch for now. Will probably change later.

	public GUIStyle txtStyle; //Change font, color...etc.
	public GUIStyle btnStyle; //Change button style
	public Color txtColor;
	//public Texture btnTexture;
	public int lineSpacing; //Play with this to find a 
							//good spacing between lines.

	private float timer;
	public float timeBetweenLines; //Play around with this to see what
								   //a good appearance rate of lines is
    public float fadeInTime;


	private int xLoc; //Set in Start()
	private int yStart; //Set in Start()
	public int lineWidth;
	public int lineHeight;
	public float btnHeight = 50;
	public float btnWidth;

	private int curLine;
	private List<GUILabelFade> lines;
	private List<string> choices;
	private List<string> locations;
	private List<string> images;

	private Texture2D curImg;
	private GUITextureFade curImgFade;

	public AudioClip[] heartbeats;

	private int time; //Represents the user's time taken to get the medication
	private int prevTime; //Indicates if the time has changed.

	private const int TIME_TO_WIN = 6;
	private const string WIN_ROOM = "win";
	private const string LOSE_ROOM = "lose";
	private const string LAST_ROOM = "endgame";
	// Use this for initialization
	void Start () {
		//Init lists
		lines = new List<GUILabelFade>();
		choices = new List<string>();
		locations = new List<string>();
		images = new List<string>();
		txtColor = Color.white;
		lineWidth = (Screen.width / 2) ; //Text covers 2/3 of screen.
		xLoc = Screen.width / 4;
		yStart = Screen.height / 4 ;
		time = 0;
		prevTime = 0;
		curImg = null;
		loadNewFile("introscreen");

		//This is garbage and I don't know what I'm doing.
		//audio.clip = heartbeats[0];
		audio.volume = 0.35f;
		audio.loop = true;
		audio.Play();
	}

	//This is very inefficient. However for now it is fine, since the amount of
	//text we're working with is so low.
	void parseSection(string section, List<string> sectionList) {
		reader = new StringReader(room.text);

		//Search for the beginning of the section.
		for(string txt = reader.ReadLine ();
			 txt != null && !txt.Equals (section + ":"); 
			 txt = reader.ReadLine()) {
			//Spin through the lines until we find section.
		}

		for(string txt = reader.ReadLine(); txt != null && 
				!txt.Equals("End " + section + ":");
				txt = reader.ReadLine()) {
			sectionList.Add(txt);
		}
	}

	void parseTime() {
		reader = new StringReader(room.text);

		//Search for beginning of the section.
		for(string txt = reader.ReadLine();
			txt != null && !txt.Equals("Time" + ":");
			txt = reader.ReadLine()) {

		}
		for(string txt = reader.ReadLine();
			txt != null && !txt.Equals("End Time:");
			txt = reader.ReadLine()) {
			try {
				time += Convert.ToInt32(txt);
				Debug.Log("Time is: " + time);
			} catch (FormatException e) {
	            Debug.Log("Input string is not a sequence of digits.");
	        } catch (OverflowException e) {
				Debug.Log("Too big for an int");
			}
		}
	}
	
	// Update is called once per frame
	void Update() {
		if (curLine < lines.Count) {
			if(timer > timeBetweenLines) {
				curLine++;
				timer = 0;
			} else {
				timer += Time.deltaTime;
			}
			if(prevTime != time) { 
				if(time == 2) {
					audio.Stop();
					audio.clip = heartbeats[1];
					audio.Play();
				} else if (time == 4) {
					audio.Stop();
					audio.clip = heartbeats[2];
					audio.Play();
				} else if(time == 6) {
					audio.Stop();
					audio.clip = heartbeats[3];
					audio.Play();
				} else if (time == 8) {
					audio.Stop();
					audio.clip = heartbeats[4];
					audio.Play();
				}  else if (time == 10) {
					audio.Stop();
					audio.clip = heartbeats[5];
					audio.Play();
				} else if (time == 12) {
					audio.Stop();
					audio.clip = heartbeats[6];
					audio.Play();
				}

				prevTime = time;
			}
		}
	}

	void OnGUI() {
		if(curImg != null) {
			if(curImgFade == null) {
				Debug.Log("null!!!");
				curImgFade = new GUITextureFade(fadeInTime, curImg, 
					new Rect(Screen.width / 2, 0, Screen.width / 2, Screen.height));
				//curImgFade.Render(Color.white);
			} else {
				//curImgFade.Render(Color.white);
			}
			GUI.DrawTexture(new Rect(Screen.width / 2, 0, Screen.width / 2, Screen.height), curImg, ScaleMode.ScaleToFit);
		}
		drawText ();
		//Draw choice buttons once all the text has been drawn on screen.
		if (curLine == lines.Count){
			handleButtons();
		}
	}

	void handleButtons() {
		if (choices.Count == 1) {
			Rect btnRect = new Rect(Screen.width / 2 - btnWidth / 2,
									yPosition(lines.Count + 2),
									btnWidth, btnHeight);
			if(GUI.Button(btnRect, choices[0], btnStyle)) {
				Debug.Log("Player chose " + choices[0]);
				loadNewFile(locations[0]);
			} else if(btnRect.Contains(Event.current.mousePosition) &&
					  Event.current.button == 0) { //Mouse Down
			}

		} else { //There are two choices, user can only have one or two choices 
				 //for this game.
			if (GUI.Button(new Rect(Screen.width / 3 - btnWidth / 2, yPosition(lines.Count + 2),
				btnWidth, btnHeight), choices[0], btnStyle)) {
				Debug.Log("Player chose " + choices[0]);
				loadNewFile(locations[0]);
			} else if (GUI.Button(new Rect((Screen.width * 2) / 3 - btnWidth / 2,
			 			yPosition(lines.Count + 2), btnWidth, btnHeight),
			 			choices[1], btnStyle)) {
				Debug.Log("Player chose " + choices[1]);
				loadNewFile(locations[1]);
			}
		}
	}

	int yPosition(int lineNum) {
		return (lineNum * lineSpacing) + yStart;
	}

	//Draw as many lines as been have exposed by the counter.
	void drawText() {
		for (int i = 0; i <= curLine && i < lines.Count; i++) {
			GUI.color = txtColor;
			(lines[i]).Render(txtColor);
		}

	}

	void loadNewFile(string fileName) {
		lines.Clear();
		choices.Clear();
		images.Clear();
		locations.Clear();
		Debug.Log("Entered room: " + fileName);
		try {
			room = (TextAsset)Resources.Load ("text/" + fileName,
												typeof(TextAsset));
		} catch (Exception e) {
			Debug.Log("error opening file " + fileName);
		}
		parseSection("Images", images);
		Debug.Log("Images Count:" + images.Count);
		//TODO: Handle Image display logic here.
		if(images.Count > 0) {
			 curImg = (Texture2D)Resources.Load("images/loRes/" + images[0]); // Only 1 image per room.
			 Debug.Log("Image is " + curImg);
		} else {
			curImg = null;
		}

		reader = new StringReader(room.text);
		//Loop until choices block is reached.
		int lineNum = 0;
		for(string txt = reader.ReadLine ();
		 	txt != null && !txt.Equals("Choices:"); txt = reader.ReadLine ()) {
			if(txt != "") {
				Rect lblRect = new Rect(xLoc, yPosition(lineNum), lineWidth, lineHeight);
				GUILabelFade line = 
					new GUILabelFade(fadeInTime, txt,
									 lblRect, txtStyle);
				lines.Add(line);
				++lineNum;
			}
		}

		parseSection("Choices", choices);
		parseSection("SendTo", locations);
		if(locations.Contains("endgame")) {
			Debug.Log("Game won");
			endGame(time <= TIME_TO_WIN);
			return;
		}
		parseTime();
		curLine = 0;
	}


	void endGame(bool hasWon) {
		audio.Stop();
		if(hasWon) {
			loadNewFile(WIN_ROOM);
		} else {
			loadNewFile(LOSE_ROOM);
		}
	}
}