using System;
using Gtk;
using System.IO.Ports;
using millerGUI;
using System.IO;
using System.Threading;

public partial class MainWindow: Gtk.Window
{	
	private GCodeWriter gcodeWriter;
	private bool buttonIsInStartState;
	Thread thread;
	private Timer timeout;
	
	public MainWindow (): base (Gtk.WindowType.Toplevel)
	{
		Build ();
		gcodeWriter = new GCodeWriter();
		buttonIsInStartState = true;
		
		thread = new Thread(gcodeWriter.parseGcodeFile);
		thread.IsBackground = true;
	}
	
	private void handleFailedSerialConnection()
	{
		printToTerminalView("A working serial connection could not be established, check serial port and baud rate.");
	}
	
	public void printToTerminalView(string s)
	{
		terminalView1.Buffer.Text += s + "\n";
		terminalView2.Buffer.Text += s + "\n";
		terminalView3.Buffer.Text += s + "\n";
		textAddedToTerminalView();
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
		this.startStopButton.Sensitive = true;
	}
	
	protected void onStartStopClicked (object sender, System.EventArgs e)
	{
		if (buttonIsInStartState) {
			
			//button is in the 'start' state
			if (gcodeWriter.arduinoIsReady ()) 
			{
				buttonIsInStartState = false;
				this.abortButton.Sensitive = true;
				////
				this.startStopButton.Label = "Pause";
				gcodeWriter.loadFile(fileChooserButton.Filename);
				thread.Start ();
			} 
			else 
			{
				arduinoNotReady();
			}
		} 
		else 
		{
			//button is in the 'pause' state
			thread.Abort ();
			writeVerboseGCode("G01 Z5");
			buttonIsInStartState = true;
			this.startStopButton.Label = "Start";
			abortButton.Sensitive = false;
		}
	}

	protected void turnMotorOnButtonWasClicked (object sender, System.EventArgs e)
	{
		if(gcodeWriter.arduinoIsReady())
		{
			//send m3 and make sensitive/not sensitive
			writeVerboseGCode("M3");//turn motor on, as of yet to be implemented on Arduino
			writeVerboseGCode("G04 P3");
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
				writeVerboseGCode("G01 " + GCodeCommandEntry.Text);
			}
			else
			{
				writeVerboseGCode(GCodeCommandEntry.Text);
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
			writeVerboseGCode(
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
			writeVerboseGCode(
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
			writeVerboseGCode(
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
			writeVerboseGCode(
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
			writeVerboseGCode(
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
			writeVerboseGCode(
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
		
		while(true)
			if(gcodeWriter.Port.BytesToRead > 0)
				break;
		
		//timeout = new Timer(onTimeout, null, 5000, 5000);
		//gcodeWriter.Port.DataReceived += new SerialDataReceivedEventHandler(HandleGcodeWriterPortDataReceived);
	}
	
	private void onTimeout(object sender)
	{
		timeout.Dispose();
		timeout = null;
		gcodeWriter.Port.DataReceived -= HandleGcodeWriterPortDataReceived;
		//we have hit our timeout
		handleFailedSerialConnection();
	}

	void HandleGcodeWriterPortDataReceived (object sender, SerialDataReceivedEventArgs e)
	{
		Console.WriteLine("Any data");
		timeout.Dispose();
		timeout = null;
		gcodeWriter.Port.DataReceived -= HandleGcodeWriterPortDataReceived;
		printToTerminalView("result:" + gcodeWriter.Port.ReadLine());
		if(gcodeWriter.Port.ReadLine().Contains("W"))
		{
			//WOHOO arduino worked
			portConnectButton.Sensitive = false;
			portDisconnectButton.Sensitive = true;
			printToTerminalView("Succesfully opened serial connection on port " + portEntry.ActiveText + " with baud rate " + baudrateEntry.ActiveText + ".");
		}
		else
		{
			//We got some crap, probably because of the wrong baudrate, or a different device.
			handleFailedSerialConnection();
		}
	}

	protected void portDisconnectButtonClicked (object sender, System.EventArgs e)
	{
		gcodeWriter.disconnect();
		portConnectButton.Sensitive = true;
		portDisconnectButton.Sensitive = false;
		printToTerminalView("Serial connection succesfully closed.");
	}

	protected void searchForPortsButtonClicked (object sender, System.EventArgs e)
	{
		throw new System.NotImplementedException ();
	}
}
