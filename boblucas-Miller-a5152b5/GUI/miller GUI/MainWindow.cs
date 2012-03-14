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
	
	public MainWindow (): base (Gtk.WindowType.Toplevel)
	{
		Build ();
		//gcodeWriter = new GCodeWriter ("/dev/ttyACM0", 9600);
		buttonIsInStartState = true;
		
		//thread = new Thread(gcodeWriter.parseGcodeFile);
		//thread.IsBackground = true;
		
		
	}
	
	public void writeVerboseGCode(String s)
	{
		gcodeWriter.write(s);
		terminalView.Buffer.Text += s + "\n";
		textAddedToTerminalView();
	}
	
	public void arduinoNotReady()
	{
		terminalView.Buffer.Text += "\nArduino is not ready\n";
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
		//Works
	}
	
	protected void onStartStopClicked (object sender, System.EventArgs e)
	{
		if (buttonIsInStartState) {
			
			//button is in the 'start' state
			if (gcodeWriter.arduinoIsReady ()) 
			{
				terminalView.Buffer.Text += "\nArduino is ready\n";
				textAddedToTerminalView();
				buttonIsInStartState = false;
				this.abortButton.Sensitive = true;
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

	protected void textAddedToTerminalView ()
	{
		this.terminalView.ScrollToIter(terminalView.Buffer.EndIter, 0.0, false, 0.0, 0.0);
	}

	protected void GCodeCommandEntrykeyPressed (object o, Gtk.KeyPressEventArgs args)
	{
		terminalView.Buffer.Text += "\nArduino is not ready\n";
	}
	
	
	private int getRealHeight(Container container)
	{
		int total = 0;
		foreach(Widget child in container.AllChildren)
		{
			if(child is Box)
				total += getRealHeight(child as Container);
			else
				total += child.Allocation.Height + 3;
			
			if(container is HBox && total > 10)
				break;
		}
		return total;
	
	}

	protected void onPageSwitch (object o, Gtk.SwitchPageArgs args)
	{
		tabView.HeightRequest = getRealHeight(tabView.GetNthPage((int)args.PageNum) as Container);
	}
}
