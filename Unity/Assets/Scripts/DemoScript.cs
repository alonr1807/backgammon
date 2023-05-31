using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DemoScript : MonoBehaviour
{
    public GameObject checkerStart;
    public GameObject checkerDown;
    public GameObject checkerLeft;

    public Material blackMaterial;
    public Material whiteMaterial;

    //public List<Checker> data = new List<Checker>();
    public List<GameObject> checkers = new List<GameObject>();

    [SerializeField] Transform[] positions;

    public int from;
    public int to;
    public bool go = false;

    //public List<Vector3> startPos = new List<Vector3>();


    /*
     * void DrawBoard(Cell[] board) {
     *   
        // Delete all children

        // Loop over board array
        // Find position of cell in the world
        // Create N checkers (set color by setting material)
     * }
     */

    
    public enum Checker
    {
        White,
        Black
    }

    public class Cell
    {
        public List<Checker> contents { get; set; }

        public Cell()
        {
            this.contents = new List<Checker>();
        }
        public Cell(List<Checker> contents)
        {
            this.contents = contents;
        }
    }

    public class Board
    {
        private Cell[] cells;
        private Checker turn;

        public Board()
        {
            Reset();
        }

        private void InsertCheckers(int idx, Checker kind, int num)
        {
            for (int i = 0; i < num; ++i)
            {
                cells[idx].contents.Add(kind);
            }
        }

        public void Reset()
        {
            cells = new Cell[24];

            for(int i = 0; i < cells.Length; i++)
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
        /*
            board[0].color = 0;
            board[0].count = 2;
            board[5].color = 1;
            board[5].count = 5;
            board[7].color = 1;
            board[7].count = 3;
            board[11].color = 0;
            board[11].count = 5;
            board[12].color = 1;
            board[12].count = 5;
            board[16].color = 0;
            board[16].count = 3;
            board[18].color = 0;
            board[18].count = 5;
            board[23].color = 1;
            board[23].count = 2;
        */
        public Cell[] GetCells()
        {
            return cells;
        }

        public void SetTurn(Checker turn)
        {
            this.turn = turn;
        }

        // Only covers non-exiting moves.
        /*public bool IsValidMove(Move move)
        {
            // From and to must be in range.
            if (move.from < 0 || move.from > cells.Length - 1 || move.to < 0 || move.to > cells.Length - 1)
            {
                return false;
            }

            // There must be a checker in the from slot.
            if (cells[move.from].contents.Count > 0)
            {
                return false;
            }

            // The checker in the from slot must be the same kind.
            if (cells[move.from].contents[0] != move.kind)
            {
                return false;
            }

            // If there are checkers in the to slot, they must be the same kind,
            // or only one of enemy.
            if (cells[move.to].contents.Count > 0)
            {
                if (cells[move.to].contents[0] != move.kind && cells[move.to].contents.Count > 1)
                {
                    return false;
                }
            }

            return true;
        }*/

        // Can checker of kind at idx be removed for points?
        public bool CanClaim(Checker kind, int idx)
        {
            // Checkers must only exit in home base.


            return false;
        }
    }
    void createCheckerObjects(Cell[] cells)
    {
        Vector3 startPosition = checkerStart.transform.position;
        Vector3 distanceDown = checkerDown.transform.position - checkerStart.transform.position;
        Vector3 distanceLeft = checkerLeft.transform.position - checkerStart.transform.position;
        /*for (int i = 0; i < 30; i++) {
            Vector3 startPosition = checkerStart.transform.position;
            Vector3 distanceDown = checkerDown.transform.position - checkerStart.transform.position;
            Vector3 distanceLeft = checkerLeft.transform.position - checkerStart.transform.position;
            GameObject copy;
            if (i%2 ==0)
            {
                copy = Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefab/BlackChecker.prefab"));

            } else
            {
                copy = Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefab/WhiteChecker.prefab"));
            }
            

            copy.SetActive(true);
            checkers.Add(copy);

       
        }*/

        

        for (int i = 0; i < 24; i++)
        {
            if (cells[i] != null)
            {
                List<Checker> cell = cells[i].contents;
                for (int d = 0; d < cell.Count; d++)
                {
                    GameObject copy;

                    Checker kind = cell[0];
                    if (kind == Checker.White)
                    {
                        copy = Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefab/WhiteChecker.prefab"));
                    }
                    else
                    {
                        copy = Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefab/BlackChecker.prefab"));
                    }
                    

                    if(i > 11)
                    {
                        copy.transform.position = positions[i].position + distanceDown * d;
                    } else
                    {
                        copy.transform.position = positions[i].position - distanceDown * d;
                    }
                    

                    copy.SetActive(true);
                    checkers.Add(copy);
                    CheckerData checkerData = copy.GetComponent<CheckerData>();

                    checkerData.setKind((global::Checker)kind);
                    checkerData.setPosition(i);

                    //Debug.Log(checkerData.getKind() + "" + checkerData.getPosition());
                }
            }
        }


    }

    void updatePositon(Cell[] cells, GameObject checker, int nextPositon)
    {
        CheckerData checkerData = checker.GetComponent<CheckerData>();
        int previousPosition = checkerData.getPosition();
        List<Checker> previousCell = cells[previousPosition].contents;
        Checker currentChecker = previousCell[0];
        previousCell.RemoveAt(0);
        if (cells[nextPositon] != null)
        {
            List<Checker> nextCell = cells[nextPositon].contents;
            nextCell.Add(currentChecker);
            checkerData.setPosition(nextPositon);
            int d = nextCell.Count;
            Vector3 distanceDown = checkerDown.transform.position - checkerStart.transform.position;
            if (nextPositon > 11)
            {
                checker.transform.position = positions[nextPositon].position + distanceDown * d;
            }
            else
            {
                checker.transform.position = positions[nextPositon].position - distanceDown * d;
            }
        } else
        {

        }
    }


    // Start is called before the first frame update
    void Start()
    {
        Board board = new Board();
        createCheckerObjects(board.GetCells());
        updatePositon(board.GetCells(), checkers[0], 2);


        /*
        Vector3 startPosition = checkerStart.transform.position;
        Vector3 distanceDown = checkerDown.transform.position - checkerStart.transform.position;
        Vector3 distanceLeft = checkerLeft.transform.position - checkerStart.transform.position;

        for (int j = 0; j < 6; ++j)
        {
            for (int i = 0; i < 5; ++i)
            {
                GameObject copy = Instantiate(checkerStart);
                copy.SetActive(true);
                copy.transform.position = checkerStart.transform.position + distanceDown * i + distanceLeft * j;
                checkers.Add(copy);
                copy.transform.GetChild(0).GetComponent<MeshRenderer>().material = (j % 2 == 0) ? blackMaterial : whiteMaterial;

                //startPos.Add(copy.transform.position);
            }
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        if (go)
        {
            Debug.Log("You want to move from " + from + " to " + to);
            go = false;
        }
    }
}
