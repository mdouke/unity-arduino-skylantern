using UnityEngine;

public class CombinedCubeMovement3D : MonoBehaviour
{
    public GameObject cubePrefab; // 立方体のプレハブ
    public int cubeCount = 100; // 立方体の数
    public float spaceSize = 30f; // 空間のサイズ
    public float minDistance = 1f; // 立方体間の最小距離
    public float maxStepSize = 1f; // 一回の移動の最大距離
    public float amplitude = 0.5f; // 単振動の振幅
    public float speed = 2.0f; // 単振動の速度
    public float upperLimitRate = 0.8f; // 立方体の上限
    public float shakingAmpRate = 0.1f; // 立方体の振動振幅
    public float shakingSpeed = 0.1f; // 立方体の振動速度
    public float rotationSpeedRange = 1f; // 立方体の回転速度の範囲

    private GameObject[] cubes; // 立方体を格納する配列
    private Vector3[] basePositions; // 各立方体の基準位置
    private Vector3[] randomOffsets; // 各立方体の単振動のランダム位相（XYZそれぞれ独立）
    private float[] rotationSpeeds; // 各立方体の回転速度

    // Arduinoからのメッセージを格納する変数
    GameObject serial;
    SerialManager serialManagerScript;
    private string[] messageArray;
    private float shakingOffset;

    void Start()
    {
        cubes = new GameObject[cubeCount];
        basePositions = new Vector3[cubeCount];
        randomOffsets = new Vector3[cubeCount];
        rotationSpeeds = new float[cubeCount];

        // 立方体をランダムに配置
        for (int i = 0; i < cubeCount; i++)
        {
            cubes[i] = Instantiate(cubePrefab, GetRandomPosition(), Quaternion.identity);
            basePositions[i] = cubes[i].transform.position;

            // ランダム位相をXYZ軸それぞれに設定
            randomOffsets[i] = new Vector3(
                Random.Range(0f, Mathf.PI * 2),
                Random.Range(0f, Mathf.PI * 2),
                Random.Range(0f, Mathf.PI * 2)
            );

            // 立方体の回転速度を設定
            rotationSpeeds[i] = Random.Range(-rotationSpeedRange, rotationSpeedRange);
        }

        // SerialManagerスクリプトを取得
        serial = GameObject.Find("Serial");
        serialManagerScript = serial.GetComponent<SerialManager>();

    }

    void Update()
    {
        // Arduinoからのメッセージを取得
        getMessage();
        
        //messageArray[2]が"1"のとき、shakeOffsetXの値を増やす
        if (messageArray[2] == "1" || Input.GetKey(KeyCode.Alpha3))
        {
            shakingOffset += shakingSpeed;
            Debug.Log("The Button Three is pressed");
        }
        
        if (messageArray[3] == "1" || Input.GetKey(KeyCode.Alpha4))
        {
            Debug.Log("The Button Four is pressed");
            //立方体をy軸で回転させる
            for (int i = 0; i < cubeCount; i++)
            {
                cubes[i].transform.Rotate(0, rotationSpeeds[i], 0);
            }
        }

        // 立方体の移動処理
        for (int i = 0; i < cubeCount; i++)
        {
            Vector3 currentPosition = basePositions[i];
            Vector3 moveDirection = GetRandomDirection();

            // 立方体同士が近づきすぎないように調整
            for (int j = 0; j < cubeCount; j++)
            {
                if (i == j) continue;

                Vector3 otherCubePosition = basePositions[j];
                float distance = Vector3.Distance(currentPosition, otherCubePosition);

                if (distance < minDistance)
                {
                    // 最小距離を保つために移動方向を調整
                    Vector3 directionToOtherCube = (currentPosition - otherCubePosition).normalized;
                    moveDirection += directionToOtherCube * (minDistance - distance);
                }
            }

            float upperLimit = spaceSize * upperLimitRate;
            if (basePositions[i].y > upperLimit){
                basePositions[i].y = 0;
            }

            // 基準位置を移動方向に基づいて更新
            basePositions[i] += moveDirection * Time.deltaTime;

            // 空間内に収まるように基準位置を調整
            basePositions[i] = ClampPosition(basePositions[i]);

            // 3次元的な単振動を加味した位置に立方体を配置
            Vector3 offset = new Vector3(
                Mathf.Sin(Time.time * speed + randomOffsets[i].x) * amplitude,
                Mathf.Sin(Time.time * speed + randomOffsets[i].y) * amplitude,
                Mathf.Sin(Time.time * speed + randomOffsets[i].z) * amplitude
            );
            cubes[i].transform.position = basePositions[i] + offset;
        }
    }

    // ランダムな位置を生成
    Vector3 GetRandomPosition()
    {
        float x = Random.Range(-spaceSize / 2f, spaceSize / 2f);
        //float y = Random.Range(0, spaceSize);
        float y = 0;
        float z = Random.Range(-spaceSize / 2f, spaceSize / 2f);
        return new Vector3(x, y, z);
    }

    // ランダムな移動方向を生成
    Vector3 GetRandomDirection()
    {
        return new Vector3(
            Random.Range(-maxStepSize, maxStepSize),
            Random.Range(-maxStepSize, maxStepSize),
            Random.Range(-maxStepSize, maxStepSize)
        );
    }

    // 立方体の位置が空間内に収まるように調整
    Vector3 ClampPosition(Vector3 position)
    {
        float amp = shakingAmpRate * spaceSize * Mathf.Sin(0.05f * Mathf.PI * shakingOffset);
        float ampZ = shakingAmpRate * spaceSize * (1 - Mathf.Cos(0.05f * Mathf.PI * shakingOffset));
        position.x = Mathf.Clamp(position.x, -spaceSize / 2f + amp, spaceSize / 2f + amp);
        position.y = Mathf.Clamp(position.y, ampZ / 7.0f, spaceSize);
        position.z = Mathf.Clamp(position.z, -spaceSize / 2f + amp, spaceSize / 2f + amp);
        return position;
    }

    //arduinoからのメッセージを取得
    void getMessage()
    {
        string message = serialManagerScript.messageFromArduino;
        //Debug.Log(message);

        //メッセージを”,”で分割
        messageArray = message.Split(',');
        //Debug.Log(messageArray[2]);
    }
}
