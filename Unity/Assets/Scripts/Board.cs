using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Checker
{
    White,
    Black
}

public class Cell
{
    public List<Checker> contents { get; set; }

    public Cell(List<Checker> contents)
    {
        this.contents = contents;
    }
}

public class Move
{
    public Checker kind { get; set; }

    public int from { get; set; }
    public int to { get; set; }

    public Move(Checker kind, int from, int to)
    {
        this.kind = kind;
        this.from = from;
        this.to = to;
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

        // White has 5-tower at cells 5 and 12.
        InsertCheckers(0, Checker.White, 5);
        InsertCheckers(12, Checker.White, 5);

        // Black has 5-tower at cells 11 and 18.
        InsertCheckers(11, Checker.Black, 5);
        InsertCheckers(18, Checker.Black, 5);

        // White has a 3-tower at cell 7.
        InsertCheckers(7, Checker.White, 3);

        // Black has a 3-tower at cell 16.
        InsertCheckers(16, Checker.Black, 3);

        // White has a 2-tower at cell 23.
        InsertCheckers(23, Checker.White, 2);

        // Black has a 2-tower at cell 0.
        InsertCheckers(0, Checker.Black, 2);
    }

    public void SetTurn(Checker turn)
    {
        this.turn = turn;
    }

    // Only covers non-exiting moves.
    public bool IsValidMove(Move move)
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
    }

    // Can checker of kind at idx be removed for points?
    public bool CanClaim(Checker kind, int idx)
    {
        // Checkers must only exit in home base.


        return false;
    }
}
