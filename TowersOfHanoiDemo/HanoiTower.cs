using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TowersOfHanoiDemo
{
	class HanoiTower
	{
		public int Disks { get; private set; }
		public int Pegs { get; private set; }
		int _moveCounter;

		public HanoiTower(int disks, int pegs)
		{
			Disks = disks;
			Pegs = pegs;
		}

		public IEnumerable<HanoiMove> Solve()
		{
			_moveCounter = 0;
			if (Pegs == 3) return MoveDisks3(Disks, 1, 2, 3);
			else
			{
				/*int[] temp = new int[Pegs - 2];
				for (int i = 0; i < temp.Length; i++)
				{
					temp[i] = i + 2;
				}
				return MoveDisksFS(Disks, 1, Pegs, temp);*/
				if (MainWindow.UseRecursive)
				{
					if (Pegs == 4) return MoveDisks4(Disks, 1, 2, 3, 4);
					else return MoveDisks5(Disks, 1, 2, 3, 4, 5);
				}
				else
				{
					/*if (Pegs == 4) return MoveDisksFS(Disks, 1, 4, 2, 3);*/
					if (Pegs == 4) return MoveDisks4FS(Disks, 1, 2, 3, 4);
					else return MoveDisks5FS(Disks, 1, 2, 3, 4, 5);
				}
			}
		}

		private static double Factorial(double n)
		{
			double s = 1;
			for (int i = 1; i <= n; i++)
			{
				s = s * i;
			}
			return s;
		}

		private static double Find_k(double n, double p)
		{
			if (p == 4)
			{
				return Convert.ToInt32(n - Math.Floor(Math.Sqrt(2 * n) + 0.5));
			}
			else
			{
				int x = 0;
				if (n == p) return 2;
				for (int i = 1; i < n - 1; i++)
				{
					double h = Factorial(p - 3 + i) / (Factorial(p - 2) * Factorial(i - 1));
					if (h - n > 0)
					{
						x = i - 1;
						break;
					}
				}
				//if (x == 0) return 1;
				return x;
			}
		}

		public int MoveCounter
		{
			get { return _moveCounter; }
		}

		private IEnumerable<HanoiMove> MoveDisks3(int disks, int from, int via, int to)
		{
			if(disks == 1) 
			{
				_moveCounter++;
				yield return new HanoiMove(from, to);
				yield break;
			}
			foreach (var move in MoveDisks3(disks - 1, from, to, via)) 
			{
				yield return move;
			}
			foreach (var move in MoveDisks3(1, from, 0, to))
			{
				yield return move;
			}
			foreach (var move in MoveDisks3(disks - 1, via, from, to))
			{
				yield return move;
			}
		}

		private IEnumerable<HanoiMove> MoveDisks4(int disks, int from, int via1, int via2, int to)
		{
			if (disks == 1)
			{
				_moveCounter++;
				yield return new HanoiMove(from, to);
				yield break;
			}
			else if (disks == 2)
			{
				_moveCounter++;
				yield return new HanoiMove(from, via1);
				_moveCounter++;
				yield return new HanoiMove(from, to);
				_moveCounter++;
				yield return new HanoiMove(via1, to);
				yield break;
			}
			foreach (var move in MoveDisks4(disks - 2, from, via2, to, via1))
			{
				yield return move;
			}
			foreach (var move in MoveDisks4(1, from, 0, 0, via2))
			{
				yield return move;
			}
			foreach (var move in MoveDisks4(1, from, 0, 0, to))
			{
				yield return move;
			}
			foreach (var move in MoveDisks4(1, via2, 0, 0, to))
			{
				yield return move;
			}
			foreach (var move in MoveDisks4(disks - 2, via1, from, via2, to))
			{
				yield return move;
			}
		}

		private IEnumerable<HanoiMove> MoveDisks4FS(int disks, int from, int via1, int via2, int to)
		{
			int k = (int)Find_k(disks, 4);
			if (disks == 1)
			{
				_moveCounter++;
				yield return new HanoiMove(from, to);
				yield break;
			}
			else if (disks == 2)
			{
				_moveCounter++;
				yield return new HanoiMove(from, via1);
				_moveCounter++;
				yield return new HanoiMove(from, to);
				_moveCounter++;
				yield return new HanoiMove(via1, to);
				yield break;
			}
			foreach (var move in MoveDisks4FS(k, from, via2, to, via1))
			{
				yield return move;
			}
			foreach (var move in MoveDisks3(disks - k, from, via2, to))
			{
				yield return move;
			}
			foreach (var move in MoveDisks4FS(k, via1, from, via2, to))
			{
				yield return move;
			}
		}

		private IEnumerable<HanoiMove> MoveDisks5(int disks, int from, int via1, int via2, int via3, int to)
		{
			if (disks == 1)
			{
				_moveCounter++;
				yield return new HanoiMove(from, to);
				yield break;
			}
			else if (disks == 2)
			{
				_moveCounter++;
				yield return new HanoiMove(from, via1);
				_moveCounter++;
				yield return new HanoiMove(from, to);
				_moveCounter++;
				yield return new HanoiMove(via1, to);
				yield break;
			}
			else if (disks == 3)
			{
				_moveCounter++;
				yield return new HanoiMove(from, via1);
				_moveCounter++;
				yield return new HanoiMove(from, via2);
				_moveCounter++;
				yield return new HanoiMove(from, to);
				_moveCounter++;
				yield return new HanoiMove(via2, to);
				_moveCounter++;
				yield return new HanoiMove(via1, to);
				yield break;
			}
			foreach (var move in MoveDisks5(disks - 3, from, via2, via3, to, via1))
			{
				yield return move;
			}
			foreach (var move in MoveDisks5(1, from, 0, 0, 0, via3))
			{
				yield return move;
			}
			foreach (var move in MoveDisks5(1, from, 0, 0, 0, via2))
			{
				yield return move;
			}
			foreach (var move in MoveDisks5(1, from, 0, 0, 0, to))
			{
				yield return move;
			}
			foreach (var move in MoveDisks5(1, via2, 0, 0, 0, to))
			{
				yield return move;
			}
			foreach (var move in MoveDisks5(1, via3, 0, 0, 0, to))
			{
				yield return move;
			}
			foreach (var move in MoveDisks5(disks - 3, via1, from, via2, via3, to))
			{
				yield return move;
			}
		}

		private IEnumerable<HanoiMove> MoveDisks5FS(int disks, int from, int via1, int via2, int via3, int to)
		{
			int k = (int)Find_k(disks, 5);
			if (disks == 1)
			{
				_moveCounter++;
				yield return new HanoiMove(from, to);
				yield break;
			}
			else if (disks == 2)
			{
				_moveCounter++;
				yield return new HanoiMove(from, via1);
				_moveCounter++;
				yield return new HanoiMove(from, to);
				_moveCounter++;
				yield return new HanoiMove(via1, to);
				yield break;
			}
			else if (disks == 3)
			{
				_moveCounter++;
				yield return new HanoiMove(from, via1);
				_moveCounter++;
				yield return new HanoiMove(from, via2);
				_moveCounter++;
				yield return new HanoiMove(from, to);
				_moveCounter++;
				yield return new HanoiMove(via2, to);
				_moveCounter++;
				yield return new HanoiMove(via1, to);
				yield break;
			}
			foreach (var move in MoveDisks5FS(k, from, via2, via3, to, via1))
			{
				yield return move;
			}
			foreach (var move in MoveDisks4FS(disks - k, from, via2, via3, to))
			{
				yield return move;
			}
			foreach (var move in MoveDisks5FS(k, via1, from, via2, via3, to))
			{
				yield return move;
			}
		}

		private IEnumerable<HanoiMove> MoveDisksFS(int disks, int from, int to, params int[] via)
		{
			int k = (int)Find_k(disks, 2 + via.Length);
			if (disks == 1)
			{
				_moveCounter++;
				yield return new HanoiMove(from, to);
				yield break;
			}
			else
			{
				for (int i = 2; i <= disks - 2; i++)
				{
					if (disks == i)
					{
						for (int j = 1; j <= i * 2 - 1; j++)
						{
							_moveCounter++;
							if (j < (i * 2 - 1) / 2 + 1)
							{
								yield return new HanoiMove(from, via[j - 1]);
							}
							else if (j == (i * 2 - 1) / 2 + 1)
							{
								yield return new HanoiMove(from, to);
							}
							else
							{
								yield return new HanoiMove(via[i * 2 - j - 1], to);
							}
						}
						yield break;
					}
				}
			}

			int[] step1Params = new int[via.Length];
			step1Params[step1Params.Length - 1] = to;
			int[] step2Params = new int[via.Length - 1];
			int[] step3Params = new int[via.Length];
			step3Params[0] = from;
			for (int i = 0; i < step1Params.Length - 1; i++)
			{
				step1Params[i] = via[i + 1];
			}
			for (int i = 0; i < step2Params.Length; i++)
			{
				step2Params[i] = via[i + 1];
			}
			for (int i = 1; i < step3Params.Length; i++)
			{
				step3Params[i] = via[i];
			}
			foreach (var move in MoveDisksFS(k, from, via[0], step1Params))
			{
				yield return move;
			}
			foreach (var move in MoveDisksFS(disks - k, from, to, step2Params))
			{
				yield return move;
			}
			foreach (var move in MoveDisksFS(k, via[0], to, step3Params))
			{
				yield return move;
			}

			/*foreach (var move in MoveDisksFS(k, from, via[0], via[1], via[2], to))
			{
				yield return move;
			}
			foreach (var move in MoveDisksFS(disks - k, from, to, via[1], via[2]))
			{
				yield return move;
			}
			foreach (var move in MoveDisksFS(k, via[0], to, from, via[1], via[2]))
			{
				yield return move;
			}*/
		}
	}
}
