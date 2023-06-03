using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

//for communication between DragObject
public enum Checker
{
    White,
    Black
}

public class DemoScript : MonoBehaviour
{
    public List<GameObject> checkers = new List<GameObject>();

    public class Cell
    {
        public List<Checker> contents { get; set; }

        public Cell()
        {
            contents = new List<Checker>();
        }
        public Cell(List<Checker> contents)
        {
            this.contents = contents;
        }
    }
    public class Board
    {
        
        public class Cell
        {
            public List<GameObject> contents { get; set; }
            public Cell()
            {
                this.contents = new List<GameObject>();
            }
            public Cell(List<GameObject> contents)
            {
                this.contents = contents;
            }
        }
        private Cell[] cells;
        private Checker turn;
        private int dice1;
        private int dice2;

        //used for hierarchy
        public GameObject parentObject;
        public GameObject boardUI;

        public Board(GameObject parentObject, GameObject boardUI)
        {
            this.parentObject = parentObject;
            this.boardUI = boardUI;
            Reset();
            RollDice();
        }
        public void ResetGame()
        {
            Reset();
        }
        public int[] RollDice()
        {
            dice1 = Random.Range(1, 7);
            dice2 = Random.Range(1, 7);
            int[] diceVals = { dice1, dice2 };
            return diceVals;
        }
        
        private void InsertCheckers(int idx, Checker kind, int num)
        {
            float checkerDiameter = 0.05f;
            Vector3 firstCheckerDistance = new Vector3(0f, checkerDiameter / 2, 0f);
            Vector3 distanceBetweenCheckers = new Vector3(0f, checkerDiameter, 0f);
            for (int i = 0; i < num; i++)
            {
                GameObject checkerObject;

                if (kind == Checker.White)
                {
                    print(parentObject);
                    checkerObject = Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefab/WhiteChecker.prefab"), parentObject.transform);
                }
                else
                {
                    checkerObject = Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefab/BlackChecker.prefab"), parentObject.transform);
                }
                cells[idx].contents.Add(checkerObject);

                Vector3 spikePostion = boardUI.GetComponent<BoardUI>().spikePositions[idx];
                int downSpace = (cells[idx].contents.Count - 1);
                if (idx > 11)
                {
                    checkerObject.transform.position = spikePostion - firstCheckerDistance - distanceBetweenCheckers * downSpace;
                }
                else
                {
                    checkerObject.transform.position = spikePostion + firstCheckerDistance + distanceBetweenCheckers * downSpace;
                }

                checkerObject.SetActive(true);

                CheckerData checkerData = checkerObject.GetComponent<CheckerData>();
                checkerData.setKind(kind);
                checkerData.setPosition(idx);
            }
        }
        private void Reset()
        {
            cells = new Cell[24];

            for (int i = 0; i < cells.Length; i++)
            {
                cells[i] = new Cell();
            }

            // White has 5-tower at cells 5 and 12.
            InsertCheckers(11, Checker.White, 5);
            InsertCheckers(18, Checker.White, 5);

            // Black has 5-tower at cells 11 and 18.
            InsertCheckers(5, Checker.Black, 5);
            InsertCheckers(12, Checker.Black, 5);

            // White has a 3-tower at cell 7.
            InsertCheckers(16, Checker.White, 3);

            // Black has a 3-tower at cell 16.
            InsertCheckers(7, Checker.Black, 3);

            // White has a 2-tower at cell 23.
            InsertCheckers(0, Checker.White, 2);

            // Black has a 2-tower at cell 0.
            InsertCheckers(23, Checker.Black, 2);
        }

        public Cell[] GetCells()
        {
            return cells;
        }
        private GameObject RemoveChecker(int idx)
        {
            int lastCheckerIndex = cells[idx].contents.Count - 1;
            GameObject currentChecker = cells[idx].contents[lastCheckerIndex];
            cells[idx].contents.RemoveAt(lastCheckerIndex);
            return currentChecker;
        }
        //Polish validatemove
        private bool ValidateMove(int from, int to)
        {
            if (from == to)
            {
                return false;
            }
            if (cells[from].contents.Count == 0)
            {
                return false;
            }
            if (cells[from].contents[cells[from].contents.Count - 1].GetComponent<CheckerData>().getKind() != turn)
            {
                return false;
            }
            if (cells[to].contents.Count != 0 && cells[to].contents[cells[to].contents.Count - 1].GetComponent<CheckerData>().getKind() != turn)
            {
                return false;
            }



            return true;
        }
        public void updateCellCheckerPositions(int cellIndex)
        {
            for(int i = 0; i < cells[cellIndex].contents.Count; i++)
            {
                GameObject currentChecker = cells[cellIndex].contents[i];

                currentChecker.SendMessage("OnCustomDragStart");
                Vector3 spikePostion = boardUI.GetComponent<BoardUI>().spikePositions[cellIndex];
                Vector3 outPos = spikePostion;
                float checkerDiameter = 0.05f;
                Vector3 firstCheckerDistance = new Vector3(0f, checkerDiameter / 2, 0f);
                Vector3 distanceBetweenCheckers = new Vector3(0f, checkerDiameter, 0f);
                int d = i;
                if (cellIndex > 11)
                {
                    outPos += -firstCheckerDistance - distanceBetweenCheckers * d;
                }
                else
                {
                    outPos += firstCheckerDistance + distanceBetweenCheckers * d;
                }
                DataPackage outData = new DataPackage(outPos, true);
                currentChecker.SendMessage("OnCustomDragEnd", outData);
            }
        }

        public bool MoveChecker(int from, int to, GameObject currentChecker)
        {
            DataPackage outData = new DataPackage(new Vector3(), false);
            bool successfulMove = false;
            if (ValidateMove(from, to))
            {
                successfulMove = true;
                Checker kind = currentChecker.GetComponent<CheckerData>().getKind();

                Vector3 outPos = boardUI.GetComponent<BoardUI>().spikePositions[to];
                float checkerDiameter = 0.05f;
                Vector3 firstCheckerDistance = new Vector3(0f, checkerDiameter / 2, 0f);
                Vector3 distanceBetweenCheckers = new Vector3(0f, checkerDiameter, 0f);
                int d = cells[to].contents.Count;
                if (to > 11)
                {
                    outPos += -firstCheckerDistance - distanceBetweenCheckers * d;
                }
                else
                {
                    outPos += firstCheckerDistance + distanceBetweenCheckers * d;
                }
                cells[to].contents.Add(currentChecker);
                cells[from].contents.Remove(currentChecker);
                currentChecker.GetComponent<CheckerData>().setPosition(to);

                outData = new DataPackage(outPos, successfulMove);
            }
            currentChecker.SendMessage("OnCustomDragEnd", outData);

            updateCellCheckerPositions(from);
            return successfulMove;
        }

        void clearCheckerObjects()
        {
            for (int i = 0; i < cells.Length; i++)
            {
                for(int d = cells[i].contents.Count-1; d >= 0; d--)
                {
                    Destroy(cells[i].contents[d]);
                }
                cells[i] = null;
            }
        }


        public void SetTurn(Checker turn)
        {
            this.turn = turn;
        }
    }
    
    

    //FIX
    [SerializeField] Text textObject;

    [SerializeField] GameObject boardUI;

    
    private static Board CheckerBoard;
    public void GetMoveData(DataPackage dataPackage)
    {
        int from = dataPackage.moveData[0];
        int to = dataPackage.moveData[1];
        print("From: " + from + "  To: " + to);

        bool valid = CheckerBoard.MoveChecker(from, to, dataPackage.gObj);
    }


    public global::Board board;

    public void RollDice()
    {
        int[] diceVals = CheckerBoard.RollDice();
        int dice1 = diceVals[0]; int dice2 = diceVals[1];
        textObject.text = dice1.ToString() + " " + dice2.ToString();
    }

    // Start is called before the first frame update
    void Start()
    {
        CheckerBoard = new Board(gameObject, boardUI);
        RollDice();


    }


    // Update is called once per frame
    void Update()
    {

    }
}
