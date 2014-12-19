using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows;

namespace TowersOfHanoiDemo 
{
	class HanoiViewModel : INotifyPropertyChanged 
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void RaisePropertyChanged(string name) 
		{
			var pc = PropertyChanged;
			if(pc != null)
				pc(this, new PropertyChangedEventArgs(name));
		}

		int _disks;

		public int Disks 
		{
			get { return _disks; }
			set 
			{
				if(value < 1 || value > 15 || _disks == value)
					return;
				_disks = value;
				_tower = new HanoiTower(value, Pegs);
				RaisePropertyChanged("Disks");
				RaisePropertyChanged("MoveCounter");
				Moves = null;
				IsRunning = false;
				IsPaused = false;
			}
		}

		int _pegs;

		public int Pegs
		{
			get { return _pegs; }
			set
			{
				if (value < 3 || value > 5 || _pegs == value)
					return;
				_pegs = value;
				_tower = new HanoiTower(Disks, value);
				RaisePropertyChanged("Pegs");
				RaisePropertyChanged("MoveCounter");
				Moves = null;
				IsRunning = false;
				IsPaused = false;
			}
		}

		double _speed = 1.0;

		public double Speed 
		{
			get { return _speed; }
			set 
			{
				_speed = value;
				RaisePropertyChanged("Speed");
			}
		}

		public int MoveCounter 
		{
			get { return _tower.MoveCounter; }
		}

		IEnumerable<HanoiMove> _moves;
		HanoiTower _tower;
		ICommand _solveCommand;
		ICommand _resetCommand;
		ICommand _pauseCommand;

		internal IEnumerable<HanoiMove> Moves
		{
			get { return _moves; }
			set {
				if(_moves != value)
				{
					_moves = value;
					RaisePropertyChanged("Moves");
				}
			}
		}

		public HanoiViewModel(int disks, int pegs) 
		{
			_tower = new HanoiTower(disks, pegs);
			Disks = disks;
			Pegs = pegs;
		}

		public ICommand SolveCommand 
		{
			get 
			{
				return _solveCommand ?? (_solveCommand =
					new RelayCommand(() => 
					{
						Moves = _tower.Solve();
						IsRunning = true;
						IsSolved = false;
						IsPaused = false;
					}, () => !IsRunning));
			}
		}

		public ICommand ResetCommand
		{
			get
			{
				return _resetCommand ?? (_resetCommand =
					new RelayCommand(() => 
					{
						_tower = new HanoiTower(Disks, Pegs);
						IsRunning = IsPaused = false;
						IsSolved = false;
						Moves = null;
						RaisePropertyChanged("MoveCounter");
					}, () => IsRunning || IsSolved));
			}
		}

		public ICommand PauseCommand 
		{
			get 
			{
				return _pauseCommand ?? (_pauseCommand =
					new RelayCommand(() => 
					{
						IsPaused = !IsPaused;
					}, () => IsRunning && !IsSolved));
			}
		}

		bool _isRunning, _isPaused, _isSolved;

		public bool IsSolved 
		{
			get { return _isSolved; }
			set {
				if(_isSolved != value) 
				{
					_isSolved = value;
					RaisePropertyChanged("IsSolved");
				}
			}
		}

		public bool IsPaused 
		{
			get { return _isPaused; }
			set
			{
				_isPaused = value;
				RaisePropertyChanged("IsPaused");
			}
		}

		public bool IsRunning
		{
			get { return _isRunning; }
			set 
			{
				if(_isRunning != value) 
				{
					_isRunning = value;
					RaisePropertyChanged("IsRunning");
				}
			}
		}

		public void MakeMove() 
		{
			RaisePropertyChanged("MoveCounter");
		}

		internal void MovesDone() 
		{
			IsRunning = false;
			IsSolved = true;
		}
	}
}
