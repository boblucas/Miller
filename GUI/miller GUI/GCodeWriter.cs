using System;
using System.IO.Ports;
using System.Collections.Generic;
using System.IO;

namespace millerGUI
{
	public class GCodeWriter
	{
		private SerialPort port;
		public SerialPort Port
		{
			get	{ return port; }
		}
		
		private Boolean arduinoReady = false;
		private List<string> commands;
		private bool connected = false;
		
		public GCodeWriter ()
		{
			commands = new List<string>();
		}
		
		public bool connect(string serialPort, int baudrate)
		{
			try
			{
				Console.WriteLine("Connecting:" + serialPort + " at " + baudrate);
				port = new SerialPort(serialPort, baudrate);
				port.Handshake = Handshake.None;
				port.Parity = Parity.None;
				port.StopBits = StopBits.One;
				port.DataBits = 8;
				port.Open();
			}
			catch(IOException e)
			{
				return false;
			}
			connected = true;
			return true;
		}
		
		public void disconnect()
		{
			if(connected)
				port.Close();
			connected = false;
		}			
		
		public bool write(String s)
		{
			if(!connected)
				return false;
			
			port.Write(s);
			return true;
		}
		
		public int[] interrogate()
		{
			int[] output = new int[3] {10,50,0};
			//dummy values, am hoping for some elves to write an actual function;)
			return output;
		}
		
		public Boolean arduinoIsReady()
		{
			if(!connected)
				return false;
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

