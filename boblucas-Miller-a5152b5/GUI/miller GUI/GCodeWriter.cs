using System;
using System.IO.Ports;
using System.Collections.Generic;

namespace millerGUI
{
	public class GCodeWriter
	{
		private SerialPort port;
		private Boolean arduinoReady = false;
		private List<string> commands;
		
		public GCodeWriter (String portname, int baudrate)
		{
			port = new SerialPort(portname, baudrate);
			port.Open();
			
			commands = new List<string>();
		}
		
		public void write(String s)
		{
			port.Write(s);
		}
		
		public int[] interrogate()
		{
			int[] output = new int[3] {10,50,0};
			//dummy values, am hoping for some elves to write an actual function;)
			return output;
		}
		
		public Boolean arduinoIsReady()
		{
			while(port.BytesToRead > 0 && !arduinoReady)
			{
				int c = port.ReadChar();
				arduinoReady = c == 'C' || c == 'W';
			}
			return arduinoReady;
		}
		
		public void loadFile(String filename)
		{
			commands = new List<string>(System.IO.File.ReadAllLines(filename));
		}
		
		public void parseGcodeFile()
		{
			try
			{
				while(commands.Count > 0)
				{
					while(!arduinoIsReady()){}
					arduinoReady = false;
					write(commands[0]);
					commands.RemoveAt(0);
				}
			}
			catch(System.Threading.ThreadAbortException) 
			{
				
			}
			
		}
		
	}
}

