using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class BoardController : MonoBehaviour {
    public enum PlanMode {
        Random
    };

    public PlanMode planMode;
    public int boardSize;
    public int turnNumber;

    public PlayerTile playerTilePrefab;
    public RectTransform playerRectTransform;

    public Tile planTilePrefab;
    public RectTransform planRectTransform;

    private PlayerTile[,] playerTileMatrix;
    private Tile[,] planTileMatrix;
    private List<Vector2Int> turnPositionList;

    public Text turnText;

    private void OnEnable() {
        PlayerTile.OnFlipAnimationEnded += OnFlipAnimationEnded;
    }

    private void Start() {
        SetupBoard(GameManager.instance.level);
    }

    private void OnDisable() {
        PlayerTile.OnFlipAnimationEnded -= OnFlipAnimationEnded;
    }

    public void SetupBoard(int level) {
        // Transform Game level number into size and turn number
        turnNumber = level / 9 + 1;
        boardSize = (level / 3) % 3 + 4;

        // Adjust Tile scale
        float planTileScale = (planRectTransform.rect.width / boardSize) / planTilePrefab.spriteSize;
        float playerTileScale = (playerRectTransform.rect.width / boardSize) / playerTilePrefab.spriteSize;
        //planTilePrefab.spriteSize *= planTileScale;
        //playerTilePrefab.spriteSize *= playerTileScale;

        // Inititialize Matrix and list
        planTileMatrix = new Tile[boardSize, boardSize];
        playerTileMatrix = new PlayerTile[boardSize, boardSize];

        bool[,] planInitMatrix = new bool[boardSize, boardSize];
        List<Vector2Int> turnPositionChoosedList = new List<Vector2Int>();
        bool[,] playerInitMatrix = new bool[boardSize, boardSize];

        turnPositionList = new List<Vector2Int>();

        // Fill planInitMatrix and playerInitMatrix
        switch (planMode) {
            case PlanMode.Random:
                for (int xx = 0; xx < planTileMatrix.GetLength(0); xx++) {
                    for (int yy = 0; yy < planTileMatrix.GetLength(1); yy++) {
                        planInitMatrix[xx, yy] = Random.value < 0.5;
                        playerInitMatrix[xx, yy] = true;
                    }
                }
                break;
        }

        // Selected Turn
        int c = 0;
        while (c < turnNumber) {
            Vector2Int turnPostion = new Vector2Int(Random.Range(0, boardSize), Random.Range(0, boardSize));
            if (!turnPositionChoosedList.Contains(turnPostion)) {
                turnPositionChoosedList.Add(turnPostion);
                c++;
            }
        }

        // Apply turnPositionChoosedList to the playerInitMatrix
        for (int i = 0; i < turnPositionChoosedList.Count; i++) {
            int xMin = turnPositionChoosedList[i].x - 1 >= 0 ? (int) turnPositionChoosedList[i].x - 1 : 0;
            int xMax = turnPositionChoosedList[i].x + 1 <= boardSize - 1 ? (int) turnPositionChoosedList[i].x + 1 : boardSize - 1;
            int yMin = turnPositionChoosedList[i].y - 1 >= 0 ? (int) turnPositionChoosedList[i].y - 1 : 0;
            int yMax = turnPositionChoosedList[i].y + 1 <= boardSize - 1 ? (int) turnPositionChoosedList[i].y + 1 : boardSize - 1;

            for (int xx = xMin; xx <= xMax; xx++) {
                for (int yy = yMin; yy <= yMax; yy++) {
                    playerInitMatrix[xx, yy] = !playerInitMatrix[xx, yy];
                }
            }
        }

        //Instanciate Tile Matrix
        for (int xx = 0; xx < planTileMatrix.GetLength(0); xx++) {
            for (int yy = 0; yy < planTileMatrix.GetLength(1); yy++) {
                planTileMatrix[xx, yy] = Instantiate(planTilePrefab, new Vector2(planTilePrefab.spriteSize * planTileScale * xx, planTilePrefab.spriteSize * planTileScale * yy), Quaternion.identity).GetComponent<Tile>();
                planTileMatrix[xx, yy].Init(xx, yy, planInitMatrix[xx, yy]);
                planTileMatrix[xx, yy].transform.SetParent(planRectTransform.transform, false);
                planTileMatrix[xx, yy].transform.localScale = new Vector2(planTileScale, planTileScale);
                playerTileMatrix[xx, yy] = Instantiate(playerTilePrefab, new Vector2(playerTilePrefab.spriteSize * playerTileScale * xx, playerTilePrefab.spriteSize * playerTileScale * yy), Quaternion.identity).GetComponent<PlayerTile>();
                playerTileMatrix[xx, yy].Init(xx, yy, planInitMatrix[xx, yy], playerInitMatrix[xx, yy], this);
                playerTileMatrix[xx, yy].transform.SetParent(playerRectTransform.transform, false);
                playerTileMatrix[xx, yy].transform.localScale = new Vector2(playerTileScale, playerTileScale);
                playerTileMatrix[xx, yy].name = "Tile " + xx + " " + yy;
                SuscribeToTileTriggers(playerTileMatrix[xx, yy]);
            }
        }

        // Setup UI
        turnText.text = "Level: " + GameManager.instance.level + "\nTurn: " + (turnNumber - turnPositionList.Count);
    }

    public void OnTileClick(PointerEventData data) {
        PlayerTile tile = data.pointerPress.GetComponent<PlayerTile>();
        tile.selected = true;
        turnPositionList.Add(new Vector2Int(tile.xPos, tile.yPos));
        turnText.text = "Level: " + GameManager.instance.level + "\nTurn: " + (turnNumber - turnPositionList.Count);
        TurnTiles(tile.xPos, tile.yPos);
    }

    private void TurnTiles(int x, int y) {
        int xMin = x - 1 >= 0 ? x - 1 : 0;
        int xMax = x + 1 <= boardSize - 1 ? x + 1 : boardSize - 1;
        int yMin = y - 1 >= 0 ? y - 1 : 0;
        int yMax = y + 1 <= boardSize - 1 ? y + 1 : boardSize - 1;

        for (int xx = xMin; xx <= xMax; xx++) {
            for (int yy = yMin; yy <= yMax; yy++) {
                playerTileMatrix[xx, yy].TurnTile();
            }
        }
    }

    public void CheckTurn() {
        if (turnPositionList.Count >= turnNumber) {
            if (CheckWin()) {
                Debug.Log("It's a win");
                GameManager.instance.level++;
                ReloadBoard();
            } else {
                Debug.Log("Try again");
                StartCoroutine(LoseCoroutine());
            }
        }
    }

    public bool CheckWin() {
        for (int xx = 0; xx < boardSize; xx++) {
            for (int yy = 0; yy < boardSize; yy++) {
                if (!playerTileMatrix[xx, yy].isRight) {
                    return false;
                }
            }
        }
        return true;
    }


    public IEnumerator LoseCoroutine() {
        GameManager.instance.DisablePlayerInput();
        for (int xx = 0; xx < boardSize; xx++) {
            for (int yy = 0; yy < boardSize; yy++) {
                if (!playerTileMatrix[xx, yy].isRight) {
                    playerTileMatrix[xx, yy].GetComponent<Animator>().SetTrigger("error");
                }
            }
        }

        yield return new WaitForSeconds(2.5f);
        for (int i = turnPositionList.Count - 1; i >= 0; i--) {
            TurnTiles((int) turnPositionList[i].x, (int) turnPositionList[i].y);
            yield return new WaitForSeconds(0.7f);
        }
        turnPositionList = new List<Vector2Int>();
        turnText.text = "Level: " + GameManager.instance.level + "\nTurn: " + (turnNumber - turnPositionList.Count);
        GameManager.instance.EnablePlayerInput();
        yield return null;
    }

    public void SuscribeToTileTriggers(PlayerTile tile) {
        // Click event trigger
        EventTrigger trigger = tile.GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((data) => { OnTileClick((PointerEventData) data); });
        trigger.triggers.Add(entry);
    }

    private void OnFlipAnimationEnded() {
        CheckTurn();
    }

    public void ReloadBoard() {
        SceneManager.LoadScene("ChallengeMode");
    }
}
