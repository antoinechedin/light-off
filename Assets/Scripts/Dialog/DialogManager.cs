using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour, IPointerClickHandler {

	public int currentDialogLine = -1;
	public int step = 1;
	public GameObject[] dialogBubbles;

	public DialogScript dialogScript = new DialogScript();

	private Coroutine currentCoroutine;

	// Use this for initialization
	void Start () {
		DialogLine [] dialogLines = new DialogLine[4];
		dialogLines [0] = new DialogLine (DialogLine.CharPosition.LEFT, 
			"Alors ? Les recherches avancent ?");
		dialogLines [1] = new DialogLine (DialogLine.CharPosition.RIGHT,
			"Oui, nous devrions avoir accès au tombeau dans l'après midi");
		dialogLines [2] = new DialogLine (DialogLine.CharPosition.RIGHT,
			"Qu'est ce que tu vas faire en attendant ?");
		dialogLines [3] = new DialogLine (DialogLine.CharPosition.LEFT, 
			"Je vais rentrer, je ne vais pas passer l'aprèm à vous regarder bosser");

		dialogScript.dialogLineArray = dialogLines;

		dialogScript.Save ("DialogFiles/test_script.xml");
		dialogScript = DialogScript.Load ("DialogFiles/test_script.xml");
	}

	public void OnPointerClick (PointerEventData data) {
		if (step < 1) {
			step++;
		} else {
			if (currentDialogLine < dialogScript.dialogLineArray.Length - 1) {
				currentDialogLine++;
			} else {
				currentDialogLine = 0;
			}
			step = 0;
		}

		GameObject currentBubble = dialogBubbles [dialogScript.dialogLineArray [currentDialogLine].position];
		GameObject previousBubble = (currentDialogLine > 0 ? dialogBubbles [dialogScript.dialogLineArray [currentDialogLine - 1].position] : null);

		switch (step) {
		case 0:
			if (previousBubble != null) {
				previousBubble.SetActive (false);
			}
			currentBubble.SetActive(true);
			currentCoroutine = StartCoroutine (WritingAnimation (dialogScript.dialogLineArray [currentDialogLine].text, currentBubble.GetComponentInChildren<Text>()));
			break;
		case 1:
			StopCoroutine (currentCoroutine);
			currentBubble.GetComponentInChildren<Text>().text = dialogScript.dialogLineArray [currentDialogLine].text;
			break;
		}

	}

	IEnumerator WritingAnimation (string s, Text textObject) {
		for (int i = 1; i <= s.Length; i++) {
			textObject.text = s.Substring(0,i);
			textObject.text += "<color=#00000000>";
			textObject.text += s.Substring(i);
			textObject.text += "</color>";

			if (s.ToCharArray() [i - 1] == ',' || s.ToCharArray() [i - 1] == '.' || s.ToCharArray() [i - 1] == '!' || s.ToCharArray() [i - 1] == '?' || s.ToCharArray() [i - 1] == ';') {
				yield return new WaitForSeconds (0.5f);
			} else {
				yield return new WaitForSeconds (0.06f);
			}
		}
		step++;
	}

	// Update is called once per frame
	void Update () {
		
	}
}
