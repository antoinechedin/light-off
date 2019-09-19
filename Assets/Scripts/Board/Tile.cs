using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

	public float spriteSize;
	public Sprite[] rectoSprites;
	public Sprite[] versoSprites;


	[HideInInspector] public bool isRecto;
	[HideInInspector] public int xPos;
	[HideInInspector] public int yPos;


	public void Init (int x, int y, bool isRecto) {
		xPos = x;
		yPos = y;
		this.isRecto = isRecto;

		if (isRecto) {
			int randSpriteIndex = Random.Range (0, rectoSprites.Length);
			GetComponent<SpriteRenderer>().sprite = rectoSprites [randSpriteIndex];
		} else {
			int randSpriteIndex = Random.Range (0, versoSprites.Length);
			GetComponent<SpriteRenderer>().sprite = versoSprites [randSpriteIndex];
		}
	}
}
