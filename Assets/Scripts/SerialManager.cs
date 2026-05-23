using UnityEngine;

public class SerialManager : MonoBehaviour
{
    public string messageFromArduino;
    [SerializeField] private SerialHandler serialHandler;

    void Start()
    {
        serialHandler.OnDataReceived += OnDataReceived;
        messageFromArduino = "0,0,0,0,0";
    }

    private void OnDataReceived(string message)
    {
        messageFromArduino = message;
        //Debug.Log("受信データ: " + message); // 受信データをそのまま表示
    }
}
