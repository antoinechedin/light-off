using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour {

	public Camera cam;
	public GameObject background;
	public Vector3 targetPosition;
	public float moveDuration;
	public Canvas canvas;

	public Button storyButton;
	public Button challengeButton;
	public Button optionButton;

	public GameObject diamond;

	private float distance;
	private Coroutine introCoroutine = null;


	// Use this for initialization
	void Start () {
		if (GameManager.instance.introFinished) {
			cam.transform.position = targetPosition;
		} else {
			canvas.enabled = false;
			distance = targetPosition.y - cam.transform.position.y;
			introCoroutine = StartCoroutine (IntroAnimation ());
		}
	}
	
	// Update is called once per frame
	void Update () {
	}

	public IEnumerator IntroAnimation () {
		yield return new WaitForSeconds (0.5f);
		diamond.GetComponent<Animator> ().SetTrigger ("appear");
		yield return new WaitForSeconds (1f);

		float time = 0;
		Vector3 camOrigin = cam.transform.position;
		while (time < moveDuration) {
			time += Time.deltaTime;
			cam.transform.position = camOrigin + new Vector3 (0, Ease.InOutCubic (time, 0, distance, moveDuration), 0);
			yield return null;
		}

		OnIntroFinished ();
	}

	public void OnIntroFinished () {
		GameManager.instance.introFinished = true;
		background.GetComponent<BoxCollider2D> ().enabled = false;
		canvas.enabled = true;
	}		

	public void SkipIntro () {
		StopCoroutine (introCoroutine);
		cam.transform.position = targetPosition;
		OnIntroFinished ();
	}

	public void GoToChallengeMode() {
		SceneManager.LoadScene ("ChallengeMode");
	}
}
