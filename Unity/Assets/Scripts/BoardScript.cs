using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

//for communication between DragObject
public enum Kind
{
    White,
    Black
}

public class BoardScript : MonoBehaviour
{ 
    //FIX
    [SerializeField] Text textObject;

    [SerializeField] GameObject boardUI;

    private static Board CheckerBoard;

    private class Board
    {

        private class Cell
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
        private Kind turn;
        private int dice1;
        private int dice2;
        private int whiteEscapes;
        private int blackEscapes;
        private int turns;
        private bool dice1IsUsed;
        private bool dice2IsUsed;

        //used for hierarchy
        private GameObject parentObject;
        //used for spikedata
        private GameObject boardUI;

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
        
        private void InsertCheckers(int idx, Kind kind, int num)
        {
            float checkerDiameter = 0.05f;
            Vector3 firstCheckerDistance = new Vector3(0f, checkerDiameter / 2, 0f);
            Vector3 distanceBetweenCheckers = new Vector3(0f, checkerDiameter, 0f);
            for (int i = 0; i < num; i++)
            {
                GameObject checkerObject;

                if (kind == Kind.White)
                {
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
            whiteEscapes = 0;
            blackEscapes = 0;
            dice1IsUsed = false;
            dice2IsUsed = false;
            turns = 0;

            for (int i = 0; i < cells.Length; i++)
            {
                cells[i] = new Cell();
            }

            // White has 5-tower at cells 5 and 12.
            InsertCheckers(11, Kind.White, 5);
            InsertCheckers(18, Kind.White, 5);

            // Black has 5-tower at cells 11 and 18.
            InsertCheckers(5, Kind.Black, 5);
            InsertCheckers(12, Kind.Black, 5);

            // White has a 3-tower at cell 7.
            InsertCheckers(16, Kind.White, 3);

            // Black has a 3-tower at cell 16.
            InsertCheckers(7, Kind.Black, 3);

            // White has a 2-tower at cell 23.
            InsertCheckers(0, Kind.White, 2);

            // Black has a 2-tower at cell 0.
            InsertCheckers(23, Kind.Black, 2);
        }

        private GameObject RemoveChecker(int idx)
        {
            int lastCheckerIndex = cells[idx].contents.Count - 1;
            GameObject currentChecker = cells[idx].contents[lastCheckerIndex];
            cells[idx].contents.RemoveAt(lastCheckerIndex);
            return currentChecker;
        }

        private bool ValidateMove(int from, int to)
        {
            // from doesnt equal to
            if (from == to)
            {
                return false;
            }
            // from count doesnt equal 0
            if (cells[from].contents.Count == 0)
            {
                return false;
            }
            // from checker doesnt equal turn's kind
            if (cells[from].contents[cells[from].contents.Count - 1].GetComponent<CheckerData>().getKind() != turn)
            {
                return false;
            }
            // to checker doesnt equal turn's kind
            if (cells[to].contents.Count != 0 && cells[to].contents[cells[to].contents.Count - 1].GetComponent<CheckerData>().getKind() != turn)
            {
                return false;
            }
            int distance = Mathf.Abs(to - from);

            // if double
            if (dice1 == dice2)
            {
                // if distance doesnt equal the dice or turns is more than 4
                if (distance != dice1 || turns > 3)
                {
                    return false;
                }
            } else
            {
                // exceeded turns (this case doesnt matter)
                if (turns > 1)
                {
                    return false;
                }
                // dice 1 is choosen
                if (distance == dice1 && !dice1IsUsed)
                {
                    dice1IsUsed = true;
                } else if(dice1IsUsed && distance == dice1) // dice 1 is already used 
                {
                    return false;
                }
                // dice 2 is choosen
                if (distance == dice2 && !dice2IsUsed)
                {
                    dice2IsUsed = true;
                }
                else if (dice2IsUsed && distance == dice2) // dice 2 is already used 
                {
                    return false;
                }
                // no dice is used
                if (distance != dice1 && distance != dice2)
                {
                    return false;
                }
            }


            turns++;
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
        private class checkerC
        {
            public GameObject gobj;
            public Vector3 oldPos;
            public int from;
            public int to;
            public checkerC(GameObject gobj, Vector3 oldPos, int from, int to)
            {
                this.gobj = gobj;
                this.oldPos = oldPos;
                this.from = from;
                this.to = to;
            }
        }
        private List<checkerC> CheckerCache = new List<checkerC>();

        private Vector3 locatePosition(int to)
        {
            Vector3 movePostion = boardUI.GetComponent<BoardUI>().spikePositions[to];
            float checkerDiameter = 0.05f;
            Vector3 firstCheckerDistance = new Vector3(0f, checkerDiameter / 2, 0f);
            Vector3 distanceBetweenCheckers = new Vector3(0f, checkerDiameter, 0f);
            int d = cells[to].contents.Count;
            if (to > 11)
            {
                movePostion += -firstCheckerDistance - distanceBetweenCheckers * d;
            }
            else
            {
                movePostion += firstCheckerDistance + distanceBetweenCheckers * d;
            }
            return movePostion;
        }
        public bool MoveChecker(int to, GameObject currentCheckerObject)
        {
            int from = currentCheckerObject.GetComponent<CheckerData>().getPosition();
            Vector3 movePostion = new Vector3();
            bool successfulMove = false;
            if (ValidateMove(from, to))
            {
                movePostion = boardUI.GetComponent<BoardUI>().spikePositions[to];
                float checkerDiameter = 0.05f;
                Vector3 firstCheckerDistance = new Vector3(0f, checkerDiameter / 2, 0f);
                Vector3 distanceBetweenCheckers = new Vector3(0f, checkerDiameter, 0f);
                int d = cells[to].contents.Count;
                if (to > 11)
                {
                    movePostion += -firstCheckerDistance - distanceBetweenCheckers * d;
                }
                else
                {
                    movePostion += firstCheckerDistance + distanceBetweenCheckers * d;
                }
                cells[to].contents.Add(currentCheckerObject);
                cells[from].contents.Remove(currentCheckerObject);
                CheckerCache.Add(new checkerC(currentCheckerObject, currentCheckerObject.GetComponent<DemoAnimation>().getStartPos(), from, to));

                currentCheckerObject.GetComponent<CheckerData>().setPosition(to);

                updateCellCheckerPositions(from);
                successfulMove = true;
            }
            currentCheckerObject.SendMessage("OnCustomDragEnd", new DataPackage(movePostion, successfulMove));

            
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

        public void SetTurn()
        {
            if(turn == Kind.White)
            {
                turn = Kind.Black;
            } else
            {
                turn = Kind.White;
            }
        }

        public bool ValidateEscape(int from)
        {
            if (cells[from].contents.Count == 0)
            {
                return false;
            }
            if (cells[from].contents[cells[from].contents.Count - 1].GetComponent<CheckerData>().getKind() != turn)
            {
                return false;
            }
            

            return true;
        }
        public bool EscapeChecker(GameObject currentCheckerObject, GameObject escapeObject)
        {
            int from = currentCheckerObject.GetComponent<CheckerData>().getPosition();
            Vector3 escapePos = new Vector3();
            bool successfulEscape = false;
            if (ValidateEscape(from))
            {
                if(currentCheckerObject.GetComponent<CheckerData>().getKind() == Kind.White)
                {
                    escapePos = new Vector3(escapeObject.transform.position.x + 0.05f * (whiteEscapes/5), escapeObject.transform.position.y + 0.05f * (whiteEscapes % 5), currentCheckerObject.transform.position.z);
                    whiteEscapes++;
                } else
                {
                    escapePos = new Vector3(escapeObject.transform.position.x, escapeObject.transform.position.y - 0.05f * blackEscapes, currentCheckerObject.transform.position.z);
                    blackEscapes++;
                }
                
                cells[from].contents.Remove(currentCheckerObject);
                
                updateCellCheckerPositions(from);
                successfulEscape = true;
            }

            Destroy(currentCheckerObject.GetComponent<DragObject>());
            currentCheckerObject.SendMessage("OnCustomDragEnd", new DataPackage(escapePos, successfulEscape));
            return true;
        }

        public void StartTurn()
        {
            SetTurn();
        }
        public void ResetTurn()
        {
            //move the objects back first
            for (int i = CheckerCache.Count - 1; i >= 0; i--)
            {
                GameObject currentCheckerObject = CheckerCache[i].gobj;
                currentCheckerObject.SendMessage("OnCustomDragStart");
                Vector3 movePostion = locatePosition(CheckerCache[i].from);
                currentCheckerObject.SendMessage("OnCustomDragEnd", new DataPackage(movePostion, true));
                cells[CheckerCache[i].from].contents.Add(currentCheckerObject);
                cells[CheckerCache[i].to].contents.Remove(currentCheckerObject);
                currentCheckerObject.GetComponent<CheckerData>().setPosition(CheckerCache[i].from);
                CheckerCache.RemoveAt(i);
            }
            dice1IsUsed = false;
            dice2IsUsed = false;
            turns = 0;
        }
    }
    
    
    public void GetMoveData(DataPackage dataPackage)
    {
        if(!dataPackage.escape)
        {
            int to = dataPackage.moveTo;
            print("  To: " + to);

            bool valid = CheckerBoard.MoveChecker(to, dataPackage.checkerObject);
        } else
        {
            bool valid = CheckerBoard.EscapeChecker(dataPackage.checkerObject, dataPackage.escapeObject);
        }
        
    }

    public void RollDice()
    {
        int[] diceVals = CheckerBoard.RollDice();
        int dice1 = diceVals[0]; int dice2 = diceVals[1];
        textObject.text = dice1.ToString() + " " + dice2.ToString();
    }

    public void startTurn()
    {
        RollDice();
        CheckerBoard.StartTurn();

    }
    public void resetTurn()
    {
        CheckerBoard.ResetTurn();
    }

    // Start is called before the first frame update
    void Start()
    {
        CheckerBoard = new Board(gameObject, boardUI);
        startTurn();


    }


    // Update is called once per frame
    void Update()
    {

    }
}
