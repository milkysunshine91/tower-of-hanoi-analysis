using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.IO;
using Microsoft.Research.DynamicDataDisplay; // Core functionality
using Microsoft.Research.DynamicDataDisplay.DataSources; // EnumerableDataSource
using Microsoft.Research.DynamicDataDisplay.PointMarkers; // CirclePointMarker
using System.Text.RegularExpressions;
using System.Globalization;

using System.Windows.Media.Animation;
using System.ComponentModel;

namespace TowersOfHanoiDemo
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public MainWindow()
		{
			InitializeComponent();
			DataContext = _vm = new HanoiViewModel(3,3);
			_animation = (Storyboard)FindResource("moveStoryboard");
			_vm.PropertyChanged += _vm_PropertyChanged;
			Init();
		}

		public static bool UseFrameStewart { get; set; }

		public static bool UseRecursive { get; set; }

		#region Events
		#region Handle Textboxes Values
		private void textBoxesInput_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			e.Handled = !IsTextAllowed(e.Text);
		}

		private void textBoxes_LostFocus(object sender, RoutedEventArgs e)
		{
			string value = ((TextBox)e.Source).Text;
			string tag = (string)((TextBox)e.Source).Tag;
			int enteredValue = 0;
			int min = 0;
			int max = 0;

			if (tag == "disk")
			{
				min = 1;
				max = 30;
			}
			else if (tag == "peg")
			{
				min = 3;
				max = 30;
			}

			try
			{
				if (value.Length > 0)
				{
					enteredValue = Int32.Parse((String)value);
					if (enteredValue < min || enteredValue > max)
					{
						MessageBox.Show(
						  "Please enter a value in the range: " + min + " -> " + max);
						((TextBox)e.Source).Undo();
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Illegal characters or " + ex.Message);
				((TextBox)e.Source).Undo();
			}
		}

		private void textBoxes_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				// call the LostFocus event to validate the TextBox
				((TextBox)sender).RaiseEvent(new RoutedEventArgs(TextBox.LostFocusEvent));
			}
		} 
		#endregion

		#region Buttons
		private void buttonStartPlotting_Click(object sender, RoutedEventArgs e)
		{
			/*buttonStartPlotting.IsEnabled = false;
			buttonStopPlotting.IsEnabled = true;*/

			buttonStopPlotting.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

			int[] vMovesFS;
			int[] vMovesRecursive;
			int[] hNumber;

			// Your logic here
			if (radioButtonFreezePegs.IsChecked == true)
			{
				((HorizontalAxisTitle)chartPlotter.Children[12]).Content = "Number of Disks";
				int pegsToUse = Int32.Parse(textBoxPegsUse.Text);
				int endingDisks = Int32.Parse(textBoxEndingDisks.Text);
				vMovesRecursive = new int[endingDisks];
				vMovesFS = new int[endingDisks];
				hNumber = new int[endingDisks];
				for (int i = 0; i < endingDisks; i++)
				{
					//////////////////////////////////////////////////////////////////////////
					if (pegsToUse > 3)
					{
						vMovesRecursive[i] = (int)(Math.Pow(2, Math.Sqrt(i)) / 2) * (int)MovesCount(i + 1, pegsToUse, Find_k(i + 1, pegsToUse));
					}
					else vMovesRecursive[i] = (int)MovesCount(i + 1, pegsToUse, Find_k(i + 1, pegsToUse));
					vMovesFS[i] = (int)MovesCount(i + 1, pegsToUse, Find_k(i + 1, pegsToUse));
					hNumber[i] = i + 1;
				}
			}
			else
			{
				((HorizontalAxisTitle)chartPlotter.Children[12]).Content = "Number of Pegs";
				int disksToUse = Int32.Parse(textBoxDisksUse.Text);
				int endingPegs = Int32.Parse(textBoxEndingPegs.Text);
				vMovesRecursive = new int[endingPegs - 2];
				vMovesFS = new int[endingPegs - 2];
				hNumber = new int[endingPegs - 2];
				for (int i = 2; i < endingPegs; i++)
				{
					//////////////////////////////////////////////////////////////////////////
					if (i > 2 && disksToUse > i + 1)
					{
						vMovesRecursive[i - 2] = (int)(Math.Pow(2, Math.Sqrt(disksToUse)) / 2) * ((int)MovesCount(disksToUse, i + 1, Find_k(disksToUse, i + 1)));;
					}
					else vMovesRecursive[i - 2] = ((int)MovesCount(disksToUse, i + 1, Find_k(disksToUse, i + 1)));
					vMovesFS[i - 2] = (int)MovesCount(disksToUse, i + 1, Find_k(disksToUse, i + 1));
					hNumber[i - 2] = i + 1;
				}
			}

			var horizontalDataSource = new EnumerableDataSource<int>(hNumber);
			horizontalDataSource.SetXMapping(x => x);

			var verticalDataSourceFS = new EnumerableDataSource<int>(vMovesFS);
			verticalDataSourceFS.SetYMapping(y => y);

			var verticalDataSourceRec = new EnumerableDataSource<int>(vMovesRecursive);
			verticalDataSourceRec.SetYMapping(y => y);

			CompositeDataSource compositeDataSourceFS = new
			  CompositeDataSource(horizontalDataSource, verticalDataSourceFS);
			CompositeDataSource compositeDataSourceRec = new
			  CompositeDataSource(horizontalDataSource, verticalDataSourceRec);

			chartPlotter.AddLineGraph(compositeDataSourceFS,
			  new Pen(Brushes.Blue, 2),
			  new CirclePointMarker { Size = 10.0, Fill = Brushes.Red },
			  new PenDescription("Frame-Stewart Algorithm"));
			chartPlotter.AddLineGraph(compositeDataSourceRec,
			  new Pen(Brushes.Green, 2),
			  new TrianglePointMarker
			  {
				  Size = 10.0,
				  Pen = new Pen(Brushes.Black, 2.0),
				  Fill = Brushes.GreenYellow
			  },
			  new PenDescription("Recursive Algorithm"));

			chartPlotter.Viewport.FitToView();
		}

		private void buttonStopPlotting_Click(object sender, RoutedEventArgs e)
		{
			/*buttonStartPlotting.IsEnabled = true;
			buttonStopPlotting.IsEnabled = false;*/

			// Your logic here
			((HorizontalAxisTitle)chartPlotter.Children[12]).Content = "X-Axis";
			while (chartPlotter.Children.Count > 13)
			{
				chartPlotter.Children.RemoveAt(13);
			}
		}

		private void buttonStartDemo_Click(object sender, RoutedEventArgs e)
		{
			/*buttonStartDemo.IsEnabled = false;
			buttonStopDemo.IsEnabled = true;
			buttonPauseResumeDemo.IsEnabled = true;*/

		}

		private void buttonPauseResumeDemo_Click(object sender, RoutedEventArgs e)
		{
			string state = (string)buttonPauseResumeDemo.Content;
			if (state == "Pause")
				buttonPauseResumeDemo.Content = "Resume";
			else buttonPauseResumeDemo.Content = "Pause";
		}

		private void buttonStopDemo_Click(object sender, RoutedEventArgs e)
		{
			/*buttonStartDemo.IsEnabled = true;
			buttonStopDemo.IsEnabled = false;
			buttonPauseResumeDemo.IsEnabled = false;*/
			buttonPauseResumeDemo.Content = "Pause";
		} 
		#endregion

		private void tabAnalysis_Loaded(object sender, RoutedEventArgs e)
		{
			((Legend)chartPlotter.Children[7]).LegendRight = double.NaN;
			((Legend)chartPlotter.Children[7]).LegendTop = 10.0;
			((Legend)chartPlotter.Children[7]).LegendLeft = 10.0;
		}


		private void radioButtonFS_Checked(object sender, RoutedEventArgs e)
		{
			if (radioButtonFS.IsChecked == true)
			{
				UseFrameStewart = true;
			}
			else if (radioButtonFS.IsChecked == false)
			{
				UseFrameStewart = false;
			}
		}

		private void radioButtonRec_Checked(object sender, RoutedEventArgs e)
		{
			if (radioButtonRec.IsChecked == true)
			{
				UseRecursive = true;
			}
			else if (radioButtonRec.IsChecked == false)
			{
				UseRecursive = false;
			}
		}
		#endregion

		#region Supporting Functions
		private static bool IsTextAllowed(string text)
		{
			Regex regex = new Regex("[^0-9]+"); //regex that matches disallowed text
			return !regex.IsMatch(text);
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

		private static double Sigma(double p, double x)
		{
			double s = 0;
			for (int i = 0; i <= x - 1; i++)
			{
				s = s + Combinations(p, i) * Math.Pow(2, i);
			}
			return s;
		}

		private static double MovesCount(double n, double p, double x)
		{
			if ((int)n == 1) return 1;
			if ((int)n == 2) return 3;
			if ((int)n == 3) return 5;
			if ((int)p == 3) return Math.Pow(2, n) - 1;
			if (n < p)
			{
				return 2 * n - 1;
			}
			else if (n == p)
			{
				return 2 * n + 1;
			}
			else return Sigma(p, x) + ((n - Factorial(p - 3 + x) / (Factorial(p - 2) * Factorial(x - 1))) * Math.Pow(2, x));
		}

		private static double Find_k(double n, double p)
		{
			/*if (p == 4)
			{
				return Convert.ToInt32(n - Math.Floor(Math.Sqrt(2 * n) + 0.5));
			}
			else
			{*/
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
				return x;
			/*}*/
		}

		private static double Combinations(double p, double x)
		{
			double t;
			t = Factorial(p - 3 + x) / (Factorial(p - 3) * Factorial(x));
			return Factorial(p - 3 + x) / (Factorial(p - 3) * Factorial(x));
		}
		#endregion

		#region Framework
		struct PoleInfo
		{
			Stack<FrameworkElement> _disks;

			public void Add(FrameworkElement element)
			{
				if (_disks == null)
					_disks = new Stack<FrameworkElement>();
				_disks.Push(element);
			}

			public FrameworkElement Top
			{
				get { return _disks.Peek(); }
			}

			internal void Remove()
			{
				if (_disks != null)
					_disks.Pop();
			}

			public int Count
			{
				get { return _disks == null ? 0 : _disks.Count; }
			}
		}

		HanoiViewModel _vm;
		Storyboard _animation;

		const double DiskHeight = 60;

		//////////////////////////////////////////////////////////////////

		IEnumerator<HanoiMove> _currentMoveEnum;
		PoleInfo[] _poles;

		void _vm_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case "Disks":
					Init();
					break;

				case "Pegs":
					Init();
					break;

				case "Moves":
					Init();
					if (_vm.Moves != null)
					{
						_currentMoveEnum = _vm.Moves.GetEnumerator();
						if (_currentMoveEnum.MoveNext())
							MakeMove(_currentMoveEnum.Current);
					}
					break;

				case "IsPaused":
					if (_movingDisk != null)
					{
						if (_vm.IsPaused)
							_animation.Pause(_movingDisk);
						else
							_animation.Resume(_movingDisk);
					}
					break;

			}
		}

		FrameworkElement _movingDisk;

		private void MakeMove(HanoiMove move)
		{
			int from = move.From - 1, to = move.To - 1;
			var disk = _movingDisk = _poles[from].Top;
			var start = GetPositionInPole(disk, from);
			var end = GetPositionInPole(disk, to);
			var g = FindResource("animGeometry") as PathGeometry;
			g.Figures[0].StartPoint = start;
			// update geometry
			var poly = g.Figures[0].Segments[0] as PolyLineSegment;
			poly.Points[0] = new Point(start.X, 1100);
			poly.Points[1] = new Point(end.X, 1100);
			poly.Points[2] = end;

			/*Storyboard storyboard = (Storyboard)this.FindResource("moveStoryboard");

			DoubleAnimationUsingKeyFrames animationX = (DoubleAnimationUsingKeyFrames)storyboard.Children[0];
			DoubleAnimationUsingKeyFrames animationY = (DoubleAnimationUsingKeyFrames)storyboard.Children[1];

			TimeSpan timeSpan1 = new TimeSpan(0, 0, 1);
			TimeSpan timeSpan2 = new TimeSpan(0, 0, 2);
			TimeSpan timeSpan3 = new TimeSpan(0, 0, 3);

			KeySpline keySpline = new KeySpline(0.6, 0.0, 0.4, 1.0);

			animationX.KeyFrames.Add(new SplineDoubleKeyFrame(start.X, KeyTime.FromTimeSpan(timeSpan1), keySpline));
			animationX.KeyFrames.Add(new SplineDoubleKeyFrame(end.X, KeyTime.FromTimeSpan(timeSpan2), keySpline));
			animationX.KeyFrames.Add(new SplineDoubleKeyFrame(end.X, KeyTime.FromTimeSpan(timeSpan3), keySpline));

			animationY.KeyFrames.Add(new SplineDoubleKeyFrame(1100, KeyTime.FromTimeSpan(timeSpan1), keySpline));
			animationY.KeyFrames.Add(new SplineDoubleKeyFrame(1100, KeyTime.FromTimeSpan(timeSpan2), keySpline));
			animationY.KeyFrames.Add(new SplineDoubleKeyFrame(end.Y, KeyTime.FromTimeSpan(timeSpan3), keySpline));*/

			EventHandler completed = null;
			completed = (s, e) =>
			{
				_animation.Completed -= completed;
				if (_vm.Moves != null)
				{
					_poles[to].Add(disk);
					_poles[from].Remove();
					Canvas.SetLeft(disk, Canvas.GetLeft(disk));
					Canvas.SetBottom(disk, Canvas.GetBottom(disk));
					// next move
					if (_currentMoveEnum != null)
						if (_currentMoveEnum.MoveNext())
							MakeMove(_currentMoveEnum.Current);
						else
						{
							_vm.MovesDone();
							CommandManager.InvalidateRequerySuggested();
						}
				}
			};
			_animation.Completed += completed;
			_vm.MakeMove();
			// start a controllable animation
			_animation.Begin(disk, true);
		}

		private Point GetPositionInPole(FrameworkElement disk, int pole)
		{
			return new Point(200 + 400 * pole - disk.Width / 2, DiskHeight * _poles[pole].Count);
		}

		private void Init()
		{
			_currentMoveEnum = null;
			if (_movingDisk != null)
				_animation.Remove(_movingDisk);
			_movingDisk = null;

			_canvas.Children.Clear();
			_canvas.Width = 400 * _vm.Pegs;
			for (int i = 0; i < _vm.Pegs; i++)
			{
				Line newLine = new Line();
				newLine.StrokeThickness = 40;
				newLine.X1 = 200 + i * 400;
				newLine.Y1 = 120;
				newLine.X2 = 200 + i * 400;
				newLine.Y2 = 1200;
				newLine.Stroke = (Brush)this.FindResource("poleFill");
				_canvas.Children.Add(newLine);
			}

			double width = 250, left = 200 - width / 2, bottom = 0;
			_poles = new PoleInfo[_vm.Pegs];
			for (int i = 0; i < _vm.Disks; i++)
			{
				var disk = new DiskControl
				{
					Text = (_vm.Disks - i).ToString(),
					FontSize = 30,
					Width = width,
					Height = DiskHeight,
					Foreground = Brushes.White
				};
				_canvas.Children.Add(disk);
				Canvas.SetLeft(disk, left);
				Canvas.SetBottom(disk, bottom);
				width -= 10;
				bottom += DiskHeight;
				left += 5;
				_poles[0].Add(disk);
			}
		}
		#endregion

	}
}
