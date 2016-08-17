using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MorseKey : MonoBehaviour {
	private KeyCode keyButton = KeyCode.Space;
	private float keyDownTime;
	private float keyUpTime = -1f; //Need to account for very first case where this is not set
	private float sentenceStart;
	private string currentSentence;

	public AudioSource audioSource;
	public Text morseoutput;
	public bool mute = false;

	public float dotThreshold = .14f;
	public float sentenceThreshold = .5f;
	public string testPhrase = "...";


	void Update() {
		if (Input.GetKeyDown( keyButton)) {
			MorseKeyPress ();
		}

		if (Input.GetKeyUp (keyButton)) {
			MorseKeyUp ();
		}

		if (IsSentenceOver () && currentSentence != "") {
			print (currentSentence);
			currentSentence = "";
		}
	}

	/*
	 * Several things need to happen on key press
	 * - We set is keyDown to true (may not be needed)
	 * - Set the intitial time pressed
	 * - Start the tone
	 * */
	void MorseKeyPress() {
		keyDownTime = Time.time;

		if (!mute) {
			audioSource.loop = true;
			audioSource.Play ();
		}
	}

	void MorseKeyUp() {
		keyUpTime = Time.time;
		float keyPressTime = keyUpTime - keyDownTime;

		string letter = "";
		if (keyPressTime <= dotThreshold) {
			letter = ".";
		} else {
			letter = "-";
		}
		currentSentence += letter;
		UpdateDisplay ();

		if (!mute) {
			audioSource.loop = false;
			audioSource.Stop ();
		}
	}

	bool IsSentenceOver() {
		if (keyUpTime < 0) {
			return false;
		}

		return (Time.time - keyUpTime > sentenceThreshold);
	}

	void UpdateDisplay() {
		if (morseoutput != null) {
			string displaySentence = "";
			foreach (char s in currentSentence) {
				displaySentence += s;
				displaySentence += " ";
			}
			morseoutput.text = displaySentence.TrimEnd ();
		}
	}
}
