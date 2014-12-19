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

namespace TowersOfHanoiDemo
{
	/// <summary>
	/// Interaction logic for DiskControl.xaml
	/// </summary>
	public partial class DiskControl : UserControl
	{
		public DiskControl()
		{
			InitializeComponent();
		}

		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}

		public static readonly DependencyProperty TextProperty =
			 DependencyProperty.Register("Text", typeof(string), typeof(DiskControl), new UIPropertyMetadata(null));
		
	}
}
