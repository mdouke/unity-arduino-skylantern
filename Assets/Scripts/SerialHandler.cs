using UnityEngine;
using System.IO.Ports;
using System.Threading;

public class SerialHandler : MonoBehaviour
{
    public delegate void SerialDataReceivedEventHandler(string message);
    public event SerialDataReceivedEventHandler OnDataReceived;

    [SerializeField] private string portName = "/dev/cu.usbmodem11101";
    [SerializeField] private int baudRate = 9600;

    private SerialPort serialPort_;
    private Thread thread_;
    private bool isRunning_;
    private string message_;
    private bool isNewMessageReceived_;

    void Awake()
    {
        Open();
    }

    void Update()
    {
        if (isNewMessageReceived_)
        {
            OnDataReceived?.Invoke(message_);
            isNewMessageReceived_ = false;
        }
    }

    void OnDestroy()
    {
        Close();
    }

    private void Open()
    {
        try
        {
            serialPort_ = new SerialPort(portName, baudRate);
            serialPort_.Open();

            serialPort_.ReadTimeout = 1000;

            isRunning_ = true;
            thread_ = new Thread(Read);
            thread_.Start();
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Serial port unavailable: {e.Message}");
        }
    }

    private void Close()
    {
        isRunning_ = false; // スレッドを終了させるフラグ

        if (thread_ != null && thread_.IsAlive)
        {
            thread_.Join(); // スレッドの終了を待機
        }

        if (serialPort_ != null && serialPort_.IsOpen)
        {
            serialPort_.Close();
            serialPort_.Dispose();
        }
    }

    private void Read()
    {
        while (isRunning_ && serialPort_?.IsOpen == true)
        {
            try
            {
                string data = serialPort_.ReadLine();
                message_ = data;
                isNewMessageReceived_ = true;
            }
            catch (System.Exception e)
            {
                // TimeoutExceptionをSystem.Exceptionに変更
                Debug.LogWarning($"Read Error: {e.Message}");
            }
        }
    }

    public void Write(string message)
    {
        try
        {
            serialPort_?.WriteLine(message);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Write Error: {e.Message}");
        }
    }
}
