using UnityEngine;
using System;
using System.Net.Sockets;
using System.Text;

public class PlayerMovement : MonoBehaviour
{
    public float initialPlayerSpeed = 6f;  // Tốc độ ban đầu (di chuyển về phía trước)
    public float initialHorizontalSpeed = 4f;  // Tốc độ ban đầu (di chuyển ngang)
    public float rightLimit = 6.5f;
    public float leftLimit = -6.5f;
    public float jumpHeight = 5f;  // Chiều cao tối đa khi nhảy
    public float jumpDuration = 1f;  // Thời gian để hoàn thành một cú nhảy (lên và xuống)

    // Tốc độ tối đa (giới hạn để không tăng quá nhanh)
    public float maxPlayerSpeed = 30f;  // Tốc độ tối đa (di chuyển về phía trước)
    public float maxHorizontalSpeed = 5f;  // Tốc độ tối đa (di chuyển ngang)

    // Tốc độ tăng mỗi giây
    public float speedIncreaseRate = 0.5f;  // Tốc độ tăng mỗi giây (có thể điều chỉnh)

    // Tốc độ hiện tại (sẽ tăng dần)
    private float playerspeed;
    private float horizontalSpeed;

    private TcpClient client;
    private NetworkStream stream;
    private string serverIP = "127.0.0.1";
    private int port = 65432;

    private string currentCommand = "STOP";  // Lưu trạng thái di chuyển hiện tại
    private bool isJumping = false;  // Kiểm tra xem nhân vật đang nhảy không
    private float jumpTime = 0f;  // Thời gian đã trôi qua khi nhảy
    private float initialY;  // Vị trí Y ban đầu trước khi nhảy

    void Start()
    {
        // Khởi tạo tốc độ ban đầu
        playerspeed = initialPlayerSpeed;
        horizontalSpeed = initialHorizontalSpeed;

        try
        {
            client = new TcpClient(serverIP, port);
            stream = client.GetStream();
            Debug.Log("Kết nối thành công với Python server");
        }
        catch (Exception e)
        {
            Debug.Log("Lỗi kết nối: " + e.Message);
        }
    }

    void Update()
    {
        // Tăng tốc độ theo thời gian
        playerspeed += speedIncreaseRate * Time.deltaTime;
        horizontalSpeed += speedIncreaseRate * Time.deltaTime;

        // Giới hạn tốc độ tối đa
        playerspeed = Mathf.Min(playerspeed, maxPlayerSpeed);
        horizontalSpeed = Mathf.Min(horizontalSpeed, maxHorizontalSpeed);

        // Hiển thị tốc độ hiện tại để debug
        Debug.Log($"Current Player Speed: {playerspeed}, Current Horizontal Speed: {horizontalSpeed}");

        // Di chuyển về phía trước
        transform.Translate(Vector3.forward * playerspeed * Time.deltaTime, Space.World);

        // Kiểm tra bàn phím cho di chuyển ngang
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.left * horizontalSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.right * horizontalSpeed * Time.deltaTime);
        }

        // Nhảy khi nhấn Space hoặc nhận lệnh "UP"
        if ((Input.GetKeyDown(KeyCode.Space) || currentCommand == "UP") && !isJumping)
        {
            StartJump();
        }

        // Xử lý nhảy
        if (isJumping)
        {
            HandleJump();
        }

        // Nhận dữ liệu từ Python
        if (client != null && stream != null && stream.DataAvailable)
        {
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string message = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
            Debug.Log("Dữ liệu nhận được: " + message);
            currentCommand = message;
        }

        // Xử lý di chuyển từ lệnh Python
        if (currentCommand == "LEFT")
        {
            transform.Translate(Vector3.left * horizontalSpeed * Time.deltaTime);
        }
        if (currentCommand == "RIGHT")
        {
            transform.Translate(Vector3.right * horizontalSpeed * Time.deltaTime);
        }

        // Giới hạn vị trí nhân vật
        float clampedX = Mathf.Clamp(transform.position.x, leftLimit, rightLimit);
        transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
    }

    // Bắt đầu nhảy
    void StartJump()
    {
        isJumping = true;
        jumpTime = 0f;
        initialY = transform.position.y;  // Lưu vị trí Y ban đầu
    }

    // Xử lý chuyển động nhảy
    void HandleJump()
    {
        jumpTime += Time.deltaTime;
        float progress = jumpTime / jumpDuration;  // Tiến độ nhảy (0 -> 1)

        if (progress < 1f)
        {
            // Tính toán vị trí Y mới theo đường parabol (lên rồi xuống)
            float height = jumpHeight * 4 * (progress - progress * progress);  // Công thức parabol
            transform.position = new Vector3(
                transform.position.x,
                initialY + height,
                transform.position.z
            );
        }
        else
        {
            // Kết thúc nhảy, đặt lại vị trí Y về ban đầu
            transform.position = new Vector3(
                transform.position.x,
                initialY,
                transform.position.z
            );
            isJumping = false;
        }
    }

    void OnApplicationQuit()
    {
        if (stream != null) stream.Close();
        if (client != null) client.Close();
    }
}