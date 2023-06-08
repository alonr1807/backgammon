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

        private GameObject captureObject;
        private GameObject whiteCaptureObject;
        private GameObject blackCaptureObject;

        public Board(GameObject parentObject, GameObject boardUI, GameObject whiteCaptureObject, GameObject blackCaptureObject)
        {
            this.parentObject = parentObject;
            this.boardUI = boardUI;
            this.whiteCaptureObject = whiteCaptureObject;
            this.blackCaptureObject = blackCaptureObject;
            Reset();
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
        
        private void CreateCheckers(int idx, Kind kind, int num)
        {
            float checkerDiameter = 0.05f;
            Vector3 firstCheckerDistance = new Vector3(0f, checkerDiameter / 2, 0f);
            Vector3 distanceBetweenCheckers = new Vector3(0f, checkerDiameter, 0f);
            for (int i = 0; i < num; i++)
            {
                GameObject checkerObject;

                //creation
                if (kind == Kind.White)
                {
                    checkerObject = Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefab/WhiteChecker.prefab"), parentObject.transform);
                }
                else
                {
                    checkerObject = Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefab/BlackChecker.prefab"), parentObject.transform);
                }

                //physical position
                checkerObject.transform.position = locatePosition(idx);

                //add the checker after to avoid the locateposition function thinking there is already a checker there
                cells[idx].contents.Add(checkerObject);

                checkerObject.SetActive(true);

                //checker's internal data
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
            RollDice();

            for (int i = 0; i < cells.Length; i++)
            {
                cells[i] = new Cell();
            }
            blackCaptured = new List<GameObject>();
            whiteCaptured = new List<GameObject>();

            
            // White has 5-tower at cells 5 and 12.
            CreateCheckers(11, Kind.White, 5);
            CreateCheckers(18, Kind.White, 5);

            // Black has 5-tower at cells 11 and 18.
            CreateCheckers(5, Kind.Black, 5);
            CreateCheckers(12, Kind.Black, 5);

            // White has a 3-tower at cell 7.
            CreateCheckers(16, Kind.White, 3);

            // Black has a 3-tower at cell 16.
            CreateCheckers(7, Kind.Black, 3);

            // White has a 2-tower at cell 23.
            CreateCheckers(0, Kind.White, 2);

            // Black has a 2-tower at cell 0.
            CreateCheckers(23, Kind.Black, 2);

            //CreateCheckers(5, Kind.Black, 5);
            //CreateCheckers(18, Kind.White, 5);
            //CreateCheckers(23, Kind.Black, 2);
            //CreateCheckers(22, Kind.White, 1);
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
            // direction must be correct
            // if turn is white's
            if(turn == Kind.White)
            {
                if(from > to)
                {
                    return false;
                }
            } else // if turn is black's 
            {
                if (from < to)
                {
                    return false;
                }
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

        private class checkerMoveCache : checkerCacheObject
        {
            public GameObject checkerObject;
            public checkerMoveCache(GameObject checkerObject, int from, int to) : base(from, to)
            {
                this.checkerObject = checkerObject;
            }
        }
        private class checkerCaptureCache: checkerCacheObject
        {
            public GameObject capturer;
            public GameObject captured;
            public checkerCaptureCache(GameObject capturer, GameObject captured, int from, int to) : base(from, to)
            {
                this.capturer = capturer;
                this.captured = captured;
            }
        }
        private class checkerInsertCache: checkerCacheObject
        {
            public GameObject checkerObject;
            public checkerInsertCache(GameObject checkerObject, int from, int to) : base(from, to)
            {
                this.checkerObject = checkerObject;
            }
        }
        private class checkerEscapeCache: checkerCacheObject
        {
            public GameObject checkerObject;
            public checkerEscapeCache(GameObject checkerObject, int from, int to) : base(from, to)
            {
                this.checkerObject = checkerObject;
            }
        }
        private class checkerCacheObject
        {
            public int from;
            public int to;
            public checkerCacheObject(int from, int to)
            {
                this.from = from;
                this.to = to;
            }
            public override string ToString()
            {
                return " from"  + from + " to" + to;
            }
        }
        private List<checkerCacheObject> CheckerCache = new List<checkerCacheObject>();

        private Vector3 locatePosition(int to)
        {
            float checkerDiameter = 0.05f;
            Vector3 firstCheckerDistance = new Vector3(0f, checkerDiameter / 2, 0f);
            Vector3 distanceBetweenCheckers = new Vector3(0f, checkerDiameter, 0f);

            //handle captured checkers
            Vector3 capturePosition;
            if (to == 24)
            {
                if (turn == Kind.White)
                {
                    capturePosition = blackCaptureObject.transform.position;
                    capturePosition -= distanceBetweenCheckers * (blackCaptured.Count -1); // subtracting the current
                    return capturePosition;
                } else
                {
                    capturePosition = whiteCaptureObject.transform.position;
                    capturePosition += distanceBetweenCheckers * (whiteCaptured.Count - 1); // subtracting the current
                    return capturePosition;
                }
            }

            //retrieve the physical position of the spike
            Vector3 movePostion = boardUI.GetComponent<BoardUI>().spikePositions[to];

            
            int moveDown = cells[to].contents.Count;

            //positon is dependant on the side of the board
            if (to > 11)
            {
                movePostion += -firstCheckerDistance - distanceBetweenCheckers * moveDown;
            }
            else
            {
                movePostion += firstCheckerDistance + distanceBetweenCheckers * moveDown;
            }
            return movePostion;
        }


        public bool MoveChecker(int to, GameObject currentCheckerObject)
        {
            Vector3 movePostion = new Vector3();
            bool successfulMove = false;
            int from = currentCheckerObject.GetComponent<CheckerData>().getPosition();
            if (ValidateMove(from, to))
            {
                //physical movement
                movePostion = locatePosition(to);
                cells[to].contents.Add(currentCheckerObject);
                cells[from].contents.Remove(currentCheckerObject);

                //cache checker
                CheckerCache.Add(new checkerMoveCache(currentCheckerObject, from, to));

                //update the checker's position value
                currentCheckerObject.GetComponent<CheckerData>().setPosition(to);

                //update the other checker's locations in the old cell based on the movement of the current checker
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
            //really isnt necessary
            if (cells[from].contents.Count == 0)
            {
                return false;
            }
            //checks if the grabbed checker is the same kind as the turn
            if (cells[from].contents[cells[from].contents.Count - 1].GetComponent<CheckerData>().getKind() != turn)
            {
                return false;
            }
            int distance;
            int highestescapee= 0;
            //checks direction
            // if the turn is whtie's
            if (turn == Kind.White)
            {
                distance = 24 - from;
                if (from < 18)
                {
                    print("1");
                    return false;
                }
                for (int i = 0; i < 18; i++)
                {

                    if(cells[i].contents.Count > 0)
                    {
                        if (cells[i].contents[0].GetComponent<CheckerData>().getKind() == Kind.White)
                        {
                            print("2");
                            return false;
                        }
                    }
                }
                for(int i = 18; i < 24; i++)
                {
                    if(cells[i].contents.Count > 0 && cells[i].contents[0].GetComponent<CheckerData>().getKind() == Kind.White)
                    {
                        highestescapee = 24 - i;
                        break;
                    }
                }
            }
            else // if turn is black's 
            {
                distance = from + 1;
                if (from > 5)
                {
                    print("3");
                    return false;
                }
                for (int i = 6; i < cells.Length; i++)
                {
                    if (cells[i].contents.Count > 0)
                    {
                        if(cells[i].contents[0].GetComponent<CheckerData>().getKind() == Kind.Black)
                        {
                            print("4");
                            return false;
                        }
                        
                    }
                }
                for (int i = 5; i >= 0; i--)
                {
                    if (cells[i].contents.Count > 0 && cells[i].contents[0].GetComponent<CheckerData>().getKind() == Kind.Black)
                    {
                        highestescapee = i + 1;
                        break;
                    }
                }
            }
            // fix for checking max val
            if (dice1 == dice2)
            {
                // if distance doesnt equal the dice or turns is more than 4
                if (distance > dice1 || turns > 3)
                {
                    print("5");
                    return false;
                }
            }
            else
            {
                // exceeded turns (this case doesnt matter)
                if (turns > 1)
                {
                    print("6");
                    return false;
                }
                //if dice1 covers it, if dice 2 covers it, and then there is no option of covering it
                if(distance == dice1 && !dice1IsUsed)
                {
                    print("matchingdiceanddistance");
                    dice1IsUsed = true;

                } else
                if(distance == dice2 && !dice2IsUsed)
                {
                    print("matchingdiceanddistance");
                    dice2IsUsed = true;
                } else
                if(highestescapee > distance) // true means the dice cant be used as less
                {
                    print("dice cant be used as less");
                    return false;
                } else
                if(dice1 > distance && dice2 > distance)
                {
                    if(dice1 > dice2)
                    {
                        if(!dice2IsUsed)
                        {
                            dice2IsUsed = true;
                        } else
                        {
                            dice1IsUsed = true;
                        }
                    } else
                    {
                        if (!dice1IsUsed)
                        {
                            dice1IsUsed = true;
                        }
                        else
                        {
                            dice2IsUsed = true;
                        }
                    }
                } else if(dice1> distance)
                {
                    if(!dice1IsUsed)
                    {
                        dice1IsUsed = true;
                    }
                } else if(dice2 > distance)
                {
                    if (!dice2IsUsed)
                    {
                        dice2IsUsed = true;
                    }
                } else
                {
                    return false;
                }
                

            }

            print("successfulescape");
            print(dice1IsUsed);
            print(dice2IsUsed);
            turns++;


            return true;
        }

        public bool EscapeChecker(GameObject currentCheckerObject, GameObject escapeObject)
        {
            Vector3 escapePosition = new Vector3();
            bool successfulEscape = false;
            int from = currentCheckerObject.GetComponent<CheckerData>().getPosition();
            if (ValidateEscape(from))
            {
                if(currentCheckerObject.GetComponent<CheckerData>().getKind() == Kind.White)
                {
                    escapePosition = new Vector3(escapeObject.transform.position.x + 0.05f * (whiteEscapes/5), escapeObject.transform.position.y + 0.05f * (whiteEscapes % 5), currentCheckerObject.transform.position.z);
                    whiteEscapes++;
                } else
                {
                    escapePosition = new Vector3(escapeObject.transform.position.x + 0.05f * (blackEscapes / 5), escapeObject.transform.position.y - 0.05f *(blackEscapes % 5), currentCheckerObject.transform.position.z);
                    blackEscapes++;
                }
                
                cells[from].contents.Remove(currentCheckerObject);
                
                updateCellCheckerPositions(from);
                successfulEscape = true;
                //destroy interaction with the checker as it is free
                Destroy(currentCheckerObject.GetComponent<DragObject>());
            }

            currentCheckerObject.SendMessage("OnCustomDragEnd", new DataPackage(escapePosition, successfulEscape));
            return true;
        }

        public void StartTurn()
        {
            SetTurn();
        }

        public void handleBackwardsMovement(GameObject currentCheckerObject, int from, int to) 
        {
            currentCheckerObject.SendMessage("OnCustomDragStart");
            Vector3 movePostion = locatePosition(from);
            currentCheckerObject.SendMessage("OnCustomDragEnd", new DataPackage(movePostion, true));
            if(from != 24)
            {
                cells[from].contents.Add(currentCheckerObject);
            }
            if(to != 24)
            {
                cells[to].contents.Remove(currentCheckerObject);
            }
            currentCheckerObject.GetComponent<CheckerData>().setPosition(from);
        }

        public void ResetTurn()
        {
            //backwards loop also accounts for the fact that there are multiple cached moves in the list of the same obj
            for (int i = CheckerCache.Count - 1; i >= 0; i--)
            {
                //print(CheckerCache[i]);
                int from = CheckerCache[i].from;
                int to = CheckerCache[i].to;
                //means its a capture
                if(CheckerCache[i] is checkerCaptureCache captureCache)
                {
                    GameObject capturer = captureCache.capturer;
                    GameObject captured = captureCache.captured;
                    cells[to].contents.Remove(capturer);
                    handleBackwardsMovement(capturer, from, to);
                    handleBackwardsMovement(captured, to, 24);
                    if (turn == Kind.White)
                    {
                        blackCaptured.Remove(captured);
                    }
                    else
                    {
                        whiteCaptured.Remove(captured);
                    }
                } else if(CheckerCache[i] is checkerMoveCache movecache)
                {
                    handleBackwardsMovement(movecache.checkerObject, from, to);
                } else if(CheckerCache[i] is checkerEscapeCache escache) 
                {
                    print("escapeback");
                    handleBackwardsMovement(escache.checkerObject, from, to);
                    escache.checkerObject.AddComponent<DragObject>();
                } else if(CheckerCache[i] is checkerInsertCache incache)
                {
                    handleBackwardsMovement(incache.checkerObject, from, to);
                    if(turn == Kind.White)
                    {
                        whiteCaptured.Add(incache.checkerObject);
                    } else
                    {
                        blackCaptured.Add(incache.checkerObject);
                    }
                }
                CheckerCache.RemoveAt(i);
            }
            dice1IsUsed = false;
            dice2IsUsed = false;
            turns = 0;
        }

        private void clearCheckerCache()
        {
            for (int i = CheckerCache.Count - 1; i >= 0; i--)
            {
                CheckerCache.RemoveAt(i);
            }
        }

        public bool EndTurn()
        {
            if(dice1 == dice2 && turns == 4)
            {
            } else if(turns == 2 && dice1 != dice2)
            {
            } else if(!canMove())
            {
                if(checkWinner())
                {
                    print( winner() + " won");
                }
                print("cantmove");
            } else
            {
                return false;
            }
            clearCheckerCache();
            //Fix change this
            parentObject.SendMessage("startTurn");
            dice1IsUsed = false;
            dice2IsUsed = false;
            turns = 0;
            print("d");
            return true;
        }
        public bool checkerCheckMoveable(int nextPos, int currentPos)
        {

            if(turn == Kind.White)
            {
                if (nextPos < 24)
                {
                    if (cells[nextPos].contents.Count == 0)
                    {
                        return true;
                    }
                    else if (cells[nextPos].contents.Count > 0 && cells[nextPos].contents[0].GetComponent<CheckerData>().getKind() == turn)
                    {
                        return true;
                    }
                }
                else
                {
                    int furthestChecker = 0;
                    for (int o = 0; o < 19; o++)
                    {
                        if (cells[o].contents.Count > 0 && cells[o].contents[0].GetComponent<CheckerData>().getKind() == turn)
                        {
                            furthestChecker = o;
                            break;
                        }
                    }
                    if (furthestChecker == currentPos)
                    {
                        return true;
                    }
                }
            } else
            {
                if (nextPos > -1)
                {
                    if (cells[nextPos].contents.Count == 0)
                    {
                        return true;
                    }
                    else if (cells[nextPos].contents.Count > 0 && cells[nextPos].contents[0].GetComponent<CheckerData>().getKind() == turn)
                    {
                        return true;
                    }
                }
                else
                {
                    int furthestChecker = 0;
                    for (int o = 23; o > 5; o--)
                    {
                        if (cells[o].contents.Count > 0 && cells[o].contents[0].GetComponent<CheckerData>().getKind() == turn)
                        {
                            furthestChecker = o;
                            break;
                        }
                    }
                    if (furthestChecker == currentPos)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public bool canMove()
        {
            GameObject currentChecker;
            int currentpos;
            int check1;
            int check2;

            //check board objs
            for(int i = 0; i < cells.Length;i++)
            {
                //if there is a checker, check if it can move
                if (cells[i].contents.Count > 0 && cells[i].contents[0].GetComponent<CheckerData>().getKind() == turn)
                {
                    currentChecker = cells[i].contents[0];
                    currentpos = currentChecker.GetComponent<CheckerData>().getPosition();
                    
                    if(turn == Kind.White)
                    {
                        check1 = currentpos + dice1;
                        check2 = currentpos + dice2;
                    } else
                    {
                        check1 = currentpos - dice1;
                        check2 = currentpos - dice2;
                    }
                    

                    if (checkerCheckMoveable(check1, currentpos) || checkerCheckMoveable(check2, currentpos))
                    {
                        return true;
                    }


                }
            }
            //check captured objs
            if (turn == Kind.White)
            {
                for (int i = 0; i < whiteCaptured.Count; i++)
                {
                    currentChecker = whiteCaptured[i];
                    currentpos = currentChecker.GetComponent<CheckerData>().getPosition();
                    
                    check1 = dice1 + 1;
                    check2 = dice2 + 1;
                    

                    if (checkerCheckMoveable(check1, currentpos) || checkerCheckMoveable(check2, currentpos))
                    {
                        return true;
                    }
                }
            } else
            {
                for (int i = 0; i < blackCaptured.Count; i++)
                {
                    currentChecker = blackCaptured[i];
                    currentpos = currentChecker.GetComponent<CheckerData>().getPosition();

                    check1 = 24 - dice1;
                    check2 = 24 - dice2;


                    if (checkerCheckMoveable(check1, currentpos) || checkerCheckMoveable(check2, currentpos))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool validateCapture(int from, int to)
        {
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
            if (turn == Kind.White)
            {
                if (from > to)
                {
                    return false;
                }
            }
            else // if turn is black's 
            {
                if (from < to)
                {
                    return false;
                }
            }
            int distance = Mathf.Abs(to - from);
            if (dice1 == dice2)
            {
                if(distance != dice1 || turns > 3)
                {
                    return false;
                }
            } else
            {
                if(turns > 1)
                {
                    return false;
                }
                // dice 1 is choosen
                if (distance == dice1 && !dice1IsUsed)
                {
                    dice1IsUsed = true;
                }
                else if (dice1IsUsed && distance == dice1) // dice 1 is already used 
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
            }

            turns++;
            return true;
        }
        public bool CaptureChecker(int to, GameObject currentCheckerObject)
        {
            GameObject capturingCheckerObject = cells[to].contents[0];
            Vector3 movePositionCurrent = new Vector3();
            Vector3 movePositionCapture = new Vector3();
            bool successfulEscape = false;
            int from = currentCheckerObject.GetComponent<CheckerData>().getPosition();
            if(validateCapture(from, to))
            {
                if(turn == Kind.White)
                {
                    //cache this and in the reset make a condition
                    blackCaptured.Add(capturingCheckerObject);
                } else
                {
                    whiteCaptured.Add(capturingCheckerObject);
                }
                
                //must be done before
                cells[to].contents.Remove(capturingCheckerObject);

                //fix locate position
                movePositionCurrent = locatePosition(to);
                movePositionCapture = locatePosition(24);

                //must be done after
                cells[from].contents.Remove(currentCheckerObject);
                cells[to].contents.Add(currentCheckerObject);
                

                CheckerData captureCheckerData = capturingCheckerObject.GetComponent<CheckerData>();
                CheckerCache.Add(new checkerCaptureCache(currentCheckerObject, capturingCheckerObject, from, to));


                captureCheckerData.setPosition(24);
                currentCheckerObject.GetComponent<CheckerData>().setPosition(to);
                updateCellCheckerPositions(from);

                successfulEscape = true;

            }


            //update both positions
            currentCheckerObject.SendMessage("OnCustomDragEnd", new DataPackage(movePositionCurrent, successfulEscape));
            capturingCheckerObject.SendMessage("OnCustomDragStart");
            capturingCheckerObject.SendMessage("OnCustomDragEnd", new DataPackage(movePositionCapture, successfulEscape));
            return successfulEscape;
        }
        public bool checkCaptureInCell(int cellIndex)
        {
            if (cells[cellIndex].contents.Count == 0)
            {
                return false;
            }
            // check if there is one captureable piece
            if (cells[cellIndex].contents[0].GetComponent<CheckerData>().getKind() != turn && cells[cellIndex].contents.Count == 1)
            {
                return true;
            }
            return false;
        }
        public bool checkInsertChecker()
        {
            if(turn == Kind.White)
            {
                if(whiteCaptured.Count > 0)
                {
                    return true;
                }
            } else
            {
                if(blackCaptured.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }
        public bool validateInsertChecker(int to)
        {
            int distance;
            if (turn == Kind.White)
            {
                distance = to + 1;
            }
            else
            {
                distance = 24 - to;
                
            }

            if (dice1 == dice2)
            {
                // if distance doesnt equal the dice or turns is more than 4
                if (distance != dice1 || turns > 3)
                {
                    return false;
                }
            }
            else
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
                }
                else if (dice1IsUsed && distance == dice1) // dice 1 is already used 
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
        private List<GameObject> whiteCaptured { get; set; }
        private List<GameObject> blackCaptured { get; set; }
        public bool InsertChecker(int to)
        {
            GameObject currentCheckerObject;
            Vector3 movePostion = new Vector3();
            bool successfulEscape = false;
            if (validateInsertChecker(to))
            {
                print("insert valid");
                //remove the amount captured
                if (turn == Kind.White)
                {
                    currentCheckerObject = whiteCaptured[0];
                    whiteCaptured.RemoveAt(0);
                } else
                {
                    currentCheckerObject = blackCaptured[0];
                    blackCaptured.RemoveAt(0);
                }
                

                //physical movement
                movePostion = locatePosition(to);
                cells[to].contents.Add(currentCheckerObject);

                //fix cache with 24 and locate position
                CheckerCache.Add(new checkerInsertCache(currentCheckerObject, 24, to));

                //update teh checker's positon value
                currentCheckerObject.GetComponent<CheckerData>().setPosition(to);

                successfulEscape = true;
                currentCheckerObject.SendMessage("OnCustomDragEnd", new DataPackage(movePostion, successfulEscape));
            }
            return successfulEscape;
        }
        public Kind getTurn()
        {
            return turn;
        }

        public bool checkWinner()
        {
            if (whiteEscapes == 15)
            {
                return true;
            }
            if (blackEscapes == 15)
            {
                return true;
            }
            return false;
        }
        public Kind winner()
        {
            if(whiteEscapes == 15)
            {
                return Kind.White;
            }
            if(blackEscapes == 15)
            {
                return Kind.Black;
            }
            return Kind.White;
        }
    }

    [SerializeField] GameObject whtiecapturedObject;
    [SerializeField] GameObject blackcapturedObject;
    public void GetMoveData(DataPackage dataPackage)
    {
        bool valid;
        //if the checker is escaping
        if (!dataPackage.escape)
        {
            int to = dataPackage.moveTo;
            print("  To: " + to);
            //check insert, check capture, check move
            if(CheckerBoard.checkInsertChecker())
            {
                print("InsertChecker");
                valid = CheckerBoard.InsertChecker(to);
            } else if(CheckerBoard.checkCaptureInCell(to))
            {
                print("CaptureChecker");
                valid = CheckerBoard.CaptureChecker(to, dataPackage.checkerObject);
            } else
            {
                print("MoveChecker");
                valid = CheckerBoard.MoveChecker(to, dataPackage.checkerObject);
            }
            
        } else
        {
            print("EscapeChecker");
            valid = CheckerBoard.EscapeChecker(dataPackage.checkerObject, dataPackage.escapeObject);
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
        print("s");

    }
    public void resetTurn()
    {
        CheckerBoard.ResetTurn();
    }

    public void endTurn()
    {
        if(CheckerBoard.EndTurn())
        {
            print("turn ended");
        } else
        {
            print("turn didnt end");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        CheckerBoard = new Board(gameObject, boardUI, whtiecapturedObject, blackcapturedObject);
        startTurn();


    }


    // Update is called once per frame
    void Update()
    {

    }
}
