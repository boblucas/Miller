using System;
using Gtk;
using System.IO.Ports;
using millerGUI;
using System.IO;
using System.Threading;

public partial class MainWindow: Gtk.Window
{	
	private GCodeWriter gcodeWriter;
	Thread thread;
	Thread waitForArduino;
	private Timer timeout;
	
	public MainWindow (): base (Gtk.WindowType.Toplevel)
	{
		Build ();
		gcodeWriter = new GCodeWriter();
		
		thread = new Thread(gcodeWriter.parseGcodeFile);
		thread.IsBackground = true;
	}
	
	//perhaps on initialisation set the gcode streamer tab and the manual control tab to non sensitive?
	
	private void handleFailedSerialConnection()
	{
		printToTerminalView("A working serial connection could not be established, please check serial port and baud rate.");
		printToStatusBar("Serial connection failed.");
	}
	
	public void printToTerminalView(string s)
	{
		terminalView1.Buffer.Text += s + "\n";
		terminalView2.Buffer.Text += s + "\n";
		terminalView3.Buffer.Text += s + "\n";
		textAddedToTerminalView();
	}
	
	public void printToStatusBar(string s)
	{
		statusbar.Pop(0);
		statusbar.Push(0, s);
	}
	
	protected void textAddedToTerminalView ()
	{
		this.terminalView1.ScrollToIter(terminalView1.Buffer.EndIter, 0.0, false, 0.0, 0.0);
		this.terminalView2.ScrollToIter(terminalView2.Buffer.EndIter, 0.0, false, 0.0, 0.0);
		this.terminalView3.ScrollToIter(terminalView3.Buffer.EndIter, 0.0, false, 0.0, 0.0);
	}
	
	public void writeVerboseGCode(String s)
	{
		gcodeWriter.write(s);
		terminalView1.Buffer.Text += s + "\n";
		terminalView2.Buffer.Text += s + "\n";
		terminalView3.Buffer.Text += s + "\n";
		textAddedToTerminalView();
	}
	
	public void writeVeryVerboseGCode(String s)
	{
		writeVerboseGCode(s);
		printToStatusBar("Command '" + s + "' sent.");
	}
	
	protected void clearTerminalButtonClicked (object sender, System.EventArgs e)
	{
		terminalView1.Buffer.Text = "";
		terminalView2.Buffer.Text = "";
		terminalView3.Buffer.Text = "";
	}
	
	public void arduinoNotReady()
	{
		terminalView1.Buffer.Text += "\nArduino is not ready.\n";
		terminalView2.Buffer.Text += "\nArduino is not ready.\n";
		terminalView3.Buffer.Text += "\nArduino is not ready.\n";
		textAddedToTerminalView();
	}
	
	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

	protected void onFileActivated (object sender, System.EventArgs e)
	{
		this.startButton.Sensitive = true;
		printToTerminalView("File: '" + fileChooserButton.Filename + "' loaded.");
		printToStatusBar("File: '" + fileChooserButton.Filename + "' loaded.");
		//we should check for file type?
		//bug when closing fileChooserDialogWindow: this functon triggers multiple times with trash as fileChooserButton.Filename
	}
	
	protected void turnMotorOnButtonWasClicked (object sender, System.EventArgs e)
	{
		if(gcodeWriter.arduinoIsReady())
		{
			//send m3 and make sensitive/not sensitive
			writeVerboseGCode("M3");//turn motor on, as of yet to be implemented on Arduino
			writeVerboseGCode("G04 P3");
			printToStatusBar("Motor turned on.");
			this.motorOnButton.Sensitive = false;
			this.motorOffButton.Sensitive = true;
		}
		else
		{
			arduinoNotReady();
		}
	}

	protected void turnMotorOffButtonWasClicked (object sender, System.EventArgs e)
	{
		if(gcodeWriter.arduinoIsReady())
		{
			//send m5 and make sensitive/not sensitive
			writeVerboseGCode("M5");//turn motor off, as of yet to be implemented on Arduino
			writeVerboseGCode("G04 P3");
			printToStatusBar("Motor turned off.");
			this.motorOnButton.Sensitive = true;
			this.motorOffButton.Sensitive = false;
		}
		else
		{
			arduinoNotReady();
		}
	}

	protected void sendButtonClicked (object sender, System.EventArgs e)
	{
		if(gcodeWriter.arduinoIsReady())
		{
			if(prependG01Checkbutton.Active)
			{
				writeVeryVerboseGCode("G01 " + GCodeCommandEntry.Text);
			}
			else
			{
				writeVeryVerboseGCode(GCodeCommandEntry.Text);
			}
		}
		else
		{
			arduinoNotReady();
		}
	}

	protected void xplusButtonClicked (object sender, System.EventArgs e)
	{
		if(gcodeWriter.arduinoIsReady())
		{
			writeVeryVerboseGCode(
				"G01 X" + 
				Convert.ToString(
						gcodeWriter.interrogate()[0] +
						Convert.ToDouble(
							directControlMillimetersComboBoxEntry.ActiveText
						)
				)
			);
		}
		else
		{
			arduinoNotReady();	
		}
	}

	protected void xminusButtonClicked (object sender, System.EventArgs e)
	{
		if(gcodeWriter.arduinoIsReady())
		{
			writeVeryVerboseGCode(
				"G01 X" + 
				Convert.ToString(
						gcodeWriter.interrogate()[0] -
						Convert.ToDouble(
							directControlMillimetersComboBoxEntry.ActiveText
						)
				)
			);
		}
		else
		{
			arduinoNotReady();
		}
	}

	protected void yplusButtonClicked (object sender, System.EventArgs e)
	{
		if(gcodeWriter.arduinoIsReady())
		{
			writeVeryVerboseGCode(
				"G01 Y" + 
				Convert.ToString(
						gcodeWriter.interrogate()[1] +
						Convert.ToDouble(
							directControlMillimetersComboBoxEntry.ActiveText
						)
				)
			);
		}
		else
		{
			arduinoNotReady();
		}
	}

	protected void yminusButtonClicked (object sender, System.EventArgs e)
	{
		if(gcodeWriter.arduinoIsReady())
		{
			writeVeryVerboseGCode(
				"G01 Y" + 
				Convert.ToString(
						gcodeWriter.interrogate()[1] -
						Convert.ToDouble(
							directControlMillimetersComboBoxEntry.ActiveText
						)
				)
			);
		}
		else
		{
			arduinoNotReady();	
		}
	}

	protected void zplusButtonClicked (object sender, System.EventArgs e)
	{
		if(gcodeWriter.arduinoIsReady())
		{
			writeVeryVerboseGCode(
				"G01 Z" + 
				Convert.ToString(
						gcodeWriter.interrogate()[2] +
						Convert.ToDouble(
							directControlMillimetersComboBoxEntry.ActiveText
						)
				)
			);
		}
		else
		{
			arduinoNotReady();
		}
	}

	protected void zminusButtonClicked (object sender, System.EventArgs e)
	{
		if(gcodeWriter.arduinoIsReady())
		{
			writeVeryVerboseGCode(
				"G01 Z" + 
				Convert.ToString(
						gcodeWriter.interrogate()[2] -
						Convert.ToDouble(
							directControlMillimetersComboBoxEntry.ActiveText
						)
				)
			);
		}
		else
		{
			arduinoNotReady();
		}
	}

	protected void portConnectButtonClicked (object sender, System.EventArgs e)
	{
		try
		{
			gcodeWriter.connect(portEntry.ActiveText, Convert.ToInt32(baudrateEntry.ActiveText));
		}
		catch(IOException e2)
		{
			printToTerminalView(e2.Message);
			return;
		}
		printToStatusBar("Attempting to connect, please wait...");
		
		timeout = new Timer(onTimeout, null, 5000, 5000);
		waitForArduino = new Thread(waitTillLoaded);
		waitForArduino.Start();
	}
	
	private void waitTillLoaded()
	{
		try
		{
			while(!gcodeWriter.arduinoIsReady())
			{
			}
		}
		catch(System.Threading.ThreadAbortException) 
		{
		}
		
		
		if(gcodeWriter.arduinoIsReady())
		{
			//WOHOO arduino worked
			timeout.Dispose();
			timeout = null;	
			portConnectButton.Sensitive = false;
			portDisconnectButton.Sensitive = true;
			printToTerminalView("Succesfully opened serial connection on port " + portEntry.ActiveText + " with baud rate " + baudrateEntry.ActiveText + ".");
			printToStatusBar("Serial connection opened. (port " + portEntry.ActiveText + "@" + baudrateEntry.ActiveText + "baud)");
		}
	}

	private void onTimeout(object sender)
	{
		timeout.Dispose();
		timeout = null;
		thread.Abort();
		//we have hit our timeout
		handleFailedSerialConnection();
	}

	void HandleGcodeWriterPortDataReceived (object sender, SerialDataReceivedEventArgs e)
	{
	}

	protected void portDisconnectButtonClicked (object sender, System.EventArgs e)
	{
		gcodeWriter.disconnect();
		portConnectButton.Sensitive = true;
		portDisconnectButton.Sensitive = false;
		printToTerminalView("Serial connection succesfully closed.");
		printToStatusBar("Serial connection closed.");
	}

	protected void searchForPortsButtonClicked (object sender, System.EventArgs e)
	{
		string[] ports = System.IO.Ports.SerialPort.GetPortNames();
		foreach(string port in ports)
			printToTerminalView(port);
		printToStatusBar("Serial ports retrieved and listed.");
		//Not all ports get listed, only the ones that start with /dev/ttyS
		//perhaps make a new class out of System.IO.Ports.SerialPort.GerPortNames()?
	}

	protected void startButtonClicked (object sender, System.EventArgs e)
	{
		//begin milling from file
		if (gcodeWriter.arduinoIsReady ())
			{
				this.startButton.Sensitive = false;
				this.abortButton.Sensitive = true;
				this.pauseButton.Sensitive = true;
				this.resumeButton.Sensitive = false;
				this.panicButton1.Sensitive = true;
				printToStatusBar("Milling...");
				/*
				gcodeWriter.loadFile(fileChooserButton.Filename);
				thread.Start ();
				*/
			} 
			else 
			{
				arduinoNotReady();
			}
	}

	protected void abortButtonClicked (object sender, System.EventArgs e)
	{
		//abort milling, finish current command
		this.startButton.Sensitive = true;
		this.abortButton.Sensitive = false;
		this.pauseButton.Sensitive = false;
		this.resumeButton.Sensitive = false;
		this.panicButton1.Sensitive = false;
		//thread.Abort ();//right code?
		//writeVerboseGCode("G01 Z5");
		printToStatusBar("Milling aborted.");
	}

	protected void resumeButtonClicked (object sender, System.EventArgs e)
	{
		//should let milling resume at the command before 'pauseButton' or 'panicButton1' were clicked.
		this.startButton.Sensitive = false;
		this.abortButton.Sensitive = true;
		this.pauseButton.Sensitive = true;
		this.resumeButton.Sensitive = false;
		this.panicButton1.Sensitive = true;
		//code for resuming?
		printToStatusBar("Milling resumed; milling...");
	}

	protected void pauseButtonClicked (object sender, System.EventArgs e)
	{
		//Interrupt milling, finish current command
		//should be able to resume at previous command when 'resumeButton' is clicked
		this.startButton.Sensitive = false;
		this.abortButton.Sensitive = true;
		this.pauseButton.Sensitive = false;
		this.resumeButton.Sensitive = true;
		this.panicButton1.Sensitive = false;
		//thread.Abort ();
		//writeVerboseGCode("G01 Z5");
		printToStatusBar("Milling paused.");
	}

	protected void panicButton1Clicked (object sender, System.EventArgs e)
	{
		//Interrupt milling, don't even finish current command
		//should be able to resume at previous command when 'resumeButton' is clicked
		this.startButton.Sensitive = false;
		this.abortButton.Sensitive = true;
		this.pauseButton.Sensitive = false;
		this.resumeButton.Sensitive = true;
		this.panicButton1.Sensitive = false;
		//interrupt code here?
		printToStatusBar("Milling halted.");
	}

	protected void panicButton2Clicked (object sender, System.EventArgs e)
	{
		//Interrupt milling, don't even finish current command
		//interrupt code here?
		printToStatusBar("Milling halted.");
	}
}
