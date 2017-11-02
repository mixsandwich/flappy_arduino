using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System.Threading;

public class SerialHandler : MonoBehaviour
{
	public delegate void SerialDataReceivedEventHandler(string message);
	public event SerialDataReceivedEventHandler OnDataReceived;

	private const string portName = "/dev/cu.usbmodem1421";
	private const int baudRate = 9600;

	private SerialPort serial_port_;
	private Thread thread_;
	private bool isRunning_is_running_ = false;

	private string message_;
	private bool is_message_received_ = false;

	void Awake()
	{
		open();
	}

	void Update()
	{
		if (is_message_received_) {
			OnDataReceived(message_);
			is_message_received_ = false;
		}
	}

	void OnDestroy()
	{
		close();
	}

	private void open()
	{
		serial_port_ = new SerialPort(portName, baudRate,
			Parity.None,
			8,
			StopBits.One);
		serial_port_.Open();
		serial_port_.DtrEnable = true;
		serial_port_.RtsEnable = true;
		isRunning_is_running_ = true;

		thread_ = new Thread(read_entry);
		thread_.Start();
	}

	private void close()
	{
		is_message_received_ = false;
		isRunning_is_running_ = false;

		if (thread_ != null && thread_.IsAlive) {
			thread_.Abort();
			thread_.Join();
		}

		if (serial_port_ != null && serial_port_.IsOpen) {
			serial_port_.Close();
			serial_port_.Dispose();
		}
	}

	private void read_entry()
	{
		while (isRunning_is_running_ &&
			serial_port_ != null &&
			serial_port_.IsOpen) {
			try {
				const int max_loop_num = 2;
				for (var i = 0;
					serial_port_.BytesToRead > 0 && i < max_loop_num;
					++i) {
					message_ = serial_port_.ReadLine();
					is_message_received_ = true;
				}
			} catch (System.Exception e) {
				Debug.LogWarning(e.Message);
			}
			Thread.Sleep(16);
		}
	}

	public void Write(string message)
	{
		try {
			serial_port_.Write(message);
		} catch (System.Exception e) {
			Debug.LogWarning(e.Message);
		}
	}
}