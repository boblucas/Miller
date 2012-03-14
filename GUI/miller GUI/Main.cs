using System;
using System.Collections;
using System.Linq;
using Gtk;

namespace millerGUI
{
	class MainClass
	{
		
		public static void Main (string[] args)
		{
			Application.Init ();
			MainWindow win = new MainWindow ();
			win.Show ();
			Application.Run ();
		}
	}
}
