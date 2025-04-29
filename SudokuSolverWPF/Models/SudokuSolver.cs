using System;
using System.Collections.Generic;
using System.Linq;

namespace SudokuSolverWPF.Models
{
    public class SudokuSolver
    {
        private const int Size = 9;
        private const int BoxSize = 3;
        private readonly Random _random = new Random();

        public int[,] Solve(int[,] puzzle)
        {
            int[,] board = (int[,])puzzle.Clone();
            InitializeRandomBoard(board);

            int maxAttempts = 100000;
            int attempts = 0;
            int sameConflictCount = 0;
            int lastConflictCount = GetTotalConflicts(board);

            while (attempts < maxAttempts)
            {
                if (lastConflictCount == 0)
                    return board;

                var (row, col) = GetRandomConflictCell(board);
                int bestValue = FindBestValueForCell(board, row, col);
                board[row, col] = bestValue;

                int newConflictCount = GetTotalConflicts(board);

                if (newConflictCount >= lastConflictCount)
                {
                    sameConflictCount++;
                    if (sameConflictCount > 100)
                    {
                        RandomizeConflictingCell(board);
                        sameConflictCount = 0;
                        lastConflictCount = GetTotalConflicts(board);
                    }
                }
                else
                {
                    sameConflictCount = 0;
                    lastConflictCount = newConflictCount;
                }

                attempts++;
            }

            return board;
        }

        private void InitializeRandomBoard(int[,] board)
        {
            for (int row = 0; row < Size; row++)
            {
                for (int col = 0; col < Size; col++)
                {
                    if (board[row, col] == 0)
                    {
                        board[row, col] = _random.Next(1, 10);
                    }
                }
            }
        }

        private (int row, int col) GetRandomConflictCell(int[,] board)
        {
            var conflictCells = new List<(int row, int col)>();

            for (int row = 0; row < Size; row++)
            {
                for (int col = 0; col < Size; col++)
                {
                    if (CountConflicts(board, row, col, board[row, col]) > 0)
                    {
                        conflictCells.Add((row, col));
                    }
                }
            }

            return conflictCells.Count > 0 ?
                conflictCells[_random.Next(conflictCells.Count)] :
                (_random.Next(Size), _random.Next(Size));
        }

        private int FindBestValueForCell(int[,] board, int row, int col)
        {
            int originalValue = board[row, col];
            int minConflicts = int.MaxValue;
            var bestValues = new List<int>();

            for (int value = 1; value <= Size; value++)
            {
                if (value == originalValue) continue;

                int conflicts = CountConflicts(board, row, col, value);

                if (conflicts < minConflicts)
                {
                    minConflicts = conflicts;
                    bestValues.Clear();
                    bestValues.Add(value);
                }
                else if (conflicts == minConflicts)
                {
                    bestValues.Add(value);
                }
            }

            return bestValues.Count > 0 ?
                bestValues[_random.Next(bestValues.Count)] :
                originalValue;
        }

        private void RandomizeConflictingCell(int[,] board)
        {
            var (row, col) = GetRandomConflictCell(board);
            board[row, col] = _random.Next(1, 10);
        }

        private int GetTotalConflicts(int[,] board)
        {
            int totalConflicts = 0;

            for (int row = 0; row < Size; row++)
            {
                for (int col = 0; col < Size; col++)
                {
                    totalConflicts += CountConflicts(board, row, col, board[row, col]);
                }
            }

            return totalConflicts;
        }

        private int CountConflicts(int[,] board, int row, int col, int value)
        {
            if (value == 0) return 0;

            int conflicts = 0;

            // Check row
            for (int c = 0; c < Size; c++)
            {
                if (c != col && board[row, c] == value)
                    conflicts++;
            }

            // Check column
            for (int r = 0; r < Size; r++)
            {
                if (r != row && board[r, col] == value)
                    conflicts++;
            }

            // Check box
            int boxRow = row - row % BoxSize;
            int boxCol = col - col % BoxSize;

            for (int r = boxRow; r < boxRow + BoxSize; r++)
            {
                for (int c = boxCol; c < boxCol + BoxSize; c++)
                {
                    if (r != row && c != col && board[r, c] == value)
                        conflicts++;
                }
            }

            return conflicts;
        }
    }
}