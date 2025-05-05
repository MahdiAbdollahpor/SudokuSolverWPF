using System;
using System.Collections.Generic;
using System.Linq;

namespace SudokuSolverWPF.Models
{
	public class SudokuSolver
	{
		private const int Size = 9;  // اندازه استاندارد جدول سودوکو (9x9)
		private const int BoxSize = 3;  // اندازه هر جعبه 3x3 در سودوکو
		private readonly Random _random = new Random();  // شیء تصادفی برای تولید اعداد

		public int[,] Solve(int[,] puzzle)
		{
			// 1. ایجاد یک کپی از پازل ورودی برای جلوگیری از تغییر داده اصلی
			int[,] board = (int[,])puzzle.Clone();

			// 2. مقداردهی اولیه تصادفی سلول‌های خالی
			InitializeRandomBoard(board);

			// 3. تنظیم پارامترهای الگوریتم
			int maxAttempts = 100000;  // حداکثر تعداد تلاش برای حل
			int attempts = 0;  // شمارنده تلاش‌ها
			int sameConflictCount = 0;  // تعداد دفعاتی که وضعیت بهبود نیافته
			int lastConflictCount = GetTotalConflicts(board);  // تعداد کل تضادهای اولیه

			// 4. حلقه اصلی الگوریتم
			while (attempts < maxAttempts)
			{
				// اگر هیچ تضادی وجود نداشت، پازل حل شده است
				if (lastConflictCount == 0)
					return board;

				// 5. انتخاب تصادفی یک سلول دارای تضاد
				var (row, col) = GetRandomConflictCell(board);

				// 6. یافتن بهترین مقدار برای این سلول
				int bestValue = FindBestValueForCell(board, row, col);

				// 7. اعمال مقدار جدید به سلول
				board[row, col] = bestValue;

				// 8. محاسبه تعداد تضادهای جدید
				int newConflictCount = GetTotalConflicts(board);

				// 9. بررسی بهبود وضعیت
				if (newConflictCount >= lastConflictCount)
				{
					sameConflictCount++;
					// اگر برای 100 بار متوالی بهبودی نداشتیم
					if (sameConflictCount > 100)
					{
						// 10. تغییر تصادفی یک سلول مشکل‌دار برای خروج از بهینه محلی
						RandomizeConflictingCell(board);
						sameConflictCount = 0;
						lastConflictCount = GetTotalConflicts(board);
					}
				}
				else
				{
					// اگر بهبود داشتیم، شمارنده را ریست می‌کنیم
					sameConflictCount = 0;
					lastConflictCount = newConflictCount;
				}

				attempts++;
			}

			// بعد از حداکثر تلاش‌ها، جدول فعلی را برمی‌گرداند
			return board;
		}

		private void InitializeRandomBoard(int[,] board)
		{
			for (int row = 0; row < Size; row++)
			{
				for (int col = 0; col < Size; col++)
				{
					// فقط سلول‌های خالی (مقدار 0) را پر می‌کنیم
					if (board[row, col] == 0)
					{
						// تولید عدد تصادفی بین 1 تا 9
						board[row, col] = _random.Next(1, 10);
					}
				}
			}
		}

		private (int row, int col) GetRandomConflictCell(int[,] board)
		{
			var conflictCells = new List<(int row, int col)>();

			// 1. پیدا کردن تمام سلول‌هایی که دارای تضاد هستند
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

			// 2. اگر سلول تضاددار وجود داشت، یکی را تصادفی انتخاب می‌کنیم
			// در غیر این صورت یک سلول کاملاً تصادفی انتخاب می‌شود
			return conflictCells.Count > 0 ?
				conflictCells[_random.Next(conflictCells.Count)] :
				(_random.Next(Size), _random.Next(Size));
		}

		private int FindBestValueForCell(int[,] board, int row, int col)
		{
			int originalValue = board[row, col];  // مقدار فعلی سلول
			int minConflicts = int.MaxValue;  // حداقل تعداد تضاد ممکن
			var bestValues = new List<int>();  // لیست مقادیری که کمترین تضاد را ایجاد می‌کنند

			// بررسی تمام مقادیر ممکن از 1 تا 9
			for (int value = 1; value <= Size; value++)
			{
				// از بررسی مقدار فعلی صرف‌نظر می‌کنیم
				if (value == originalValue) continue;

				// شمارش تعداد تضادهای این مقدار
				int conflicts = CountConflicts(board, row, col, value);

				// اگر تضاد کمتری داشت، لیست را ریست می‌کنیم
				if (conflicts < minConflicts)
				{
					minConflicts = conflicts;
					bestValues.Clear();
					bestValues.Add(value);
				}
				// اگر همان مقدار حداقل را داشت، به لیست اضافه می‌کنیم
				else if (conflicts == minConflicts)
				{
					bestValues.Add(value);
				}
			}

			// اگر مقادیر مناسبی پیدا شد، یکی را تصادفی انتخاب می‌کنیم
			// در غیر این صورت مقدار اصلی را حفظ می‌کنیم
			return bestValues.Count > 0 ?
				bestValues[_random.Next(bestValues.Count)] :
				originalValue;
		}

		private void RandomizeConflictingCell(int[,] board)
		{
			// انتخاب یک سلول تصادفی دارای تضاد
			var (row, col) = GetRandomConflictCell(board);
			// قرار دادن یک مقدار تصادفی جدید در آن
			board[row, col] = _random.Next(1, 10);
		}

		private int GetTotalConflicts(int[,] board)
		{
			int totalConflicts = 0;

			// جمع تعداد تضادهای تمام سلول‌ها
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
			// سلول‌های خالی تضادی ندارند
			if (value == 0) return 0;

			int conflicts = 0;

			// بررسی تضاد در سطر
			for (int c = 0; c < Size; c++)
			{
				if (c != col && board[row, c] == value)
					conflicts++;
			}

			// بررسی تضاد در ستون
			for (int r = 0; r < Size; r++)
			{
				if (r != row && board[r, col] == value)
					conflicts++;
			}

			// بررسی تضاد در جعبه 3x3
			int boxRow = row - row % BoxSize;  // محاسبه سطر شروع جعبه
			int boxCol = col - col % BoxSize;  // محاسبه ستون شروع جعبه

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