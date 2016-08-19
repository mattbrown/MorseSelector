using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

/**
 * Add this to the scene so it can broadcast to everyone in the scene
 */
public class MorseKey : MonoBehaviour {
	
	private KeyCode keyButton = KeyCode.Space;
	private float keyDownTime;
	private float keyUpTime = -1f; //Need to account for very first case where this is not set
	private float sentenceStart;
	private string currentSentence;

	public GameObject morseTarget;
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
			UpdateSentence ();
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

	/**
	 * Here we need to determine if the input is a dash or dot
	 * Then stop the audio
	 * Then update listeners to our input.
	 */
	void MorseKeyUp() {
		keyUpTime = Time.time;

		//Determine dot or dash and update sentence
		char letter = DotOrDash();
		currentSentence += letter;

		if (!mute) {
			audioSource.loop = false;
			audioSource.Stop ();
		}

		UpdateLetter (letter);
	}

	char DotOrDash() {
		float keyPressTime = keyUpTime - keyDownTime;

		//Determine dot or dash
		char letter;
		if (keyPressTime <= dotThreshold) {
			letter ='.';
		} else {
			letter = '-';
		}

		return letter;
	}

	bool IsSentenceOver() {
		if (keyUpTime < 0) {
			return false;
		}

		return (Time.time - keyUpTime > sentenceThreshold);
	}

	void UpdateLetter(char letter) {
		UpdateDisplay ();
		morseTarget.BroadcastMessage ("KeyInput", letter);
	}

	void UpdateSentence() {
		morseTarget.BroadcastMessage ("SentenceInput", currentSentence);
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
