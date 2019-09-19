using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTile : Tile {

    public bool isRight;
    [HideInInspector] public bool selected;
    private Animator animator;

    public delegate void FlipAnimationEnded();
    public static event FlipAnimationEnded OnFlipAnimationEnded;

    void Start() {
        // Init Animator Behaviour
        animator = GetComponent<Animator>();
        if(animator != null) {
            TileAnimatorBehaviour[] behaviours = animator.GetBehaviours<TileAnimatorBehaviour>();
            for(int i = 0; i < behaviours.Length; i++) {
                behaviours[i].tile = this;
            }
        }
    }

    public void Init(int x, int y, bool isRecto, bool isRight, BoardController boardGame) {
        this.Init(x, y, isRecto);
        this.isRight = isRight;
        //this.boardGame = boardGame;
        if ((!isRecto && isRight) || (isRecto && !isRight)) {
            GetComponent<Animator>().SetTrigger("jumpToVerso");
        }
        if (GetComponentInChildren<TextMesh>() != null) {
            GetComponentInChildren<TextMesh>().text = isRight.ToString();
        }
    }

    public void TurnTile() {
        isRight = !isRight;
        GetComponent<Animator>().SetTrigger("turnTile");
        if (GetComponentInChildren<TextMesh>() != null) {
            GetComponentInChildren<TextMesh>().text = isRight.ToString();
        }
    }

    public void OnAnimatorStateEnter() {
        OnFlipAnimationEnded();
    }
}
