import cv2
import mediapipe as mp
import socket

# Thiết lập socket server
HOST = '127.0.0.1'
PORT = 65432
server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
server.bind((HOST, PORT))
server.listen()
print("Server Python đang chờ kết nối từ Unity...")

conn, addr = server.accept()
print(f"Đã kết nối với {addr}")

# Khởi tạo Mediapipe Pose
mp_pose = mp.solutions.pose
mp_drawing = mp.solutions.drawing_utils
pose = mp_pose.Pose()

# Mở camera
cap = cv2.VideoCapture(0)

previous_command = None  # Lưu lệnh trước đó để tránh gửi liên tục cùng một lệnh

while cap.isOpened():
    ret, frame = cap.read()
    if not ret:
        break

    # Lật hình để giống chế độ gương
    frame = cv2.flip(frame, 1)
    h, w, _ = frame.shape
    frame = cv2.resize(frame, (320, 240))  # Điều chỉnh kích thước về 320x240

    # Xử lý ảnh với Mediapipe
    rgb_frame = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
    result = pose.process(rgb_frame)

    command = None  # Lệnh sẽ gửi cho Unity

    if result.pose_landmarks:
        mp_drawing.draw_landmarks(frame, result.pose_landmarks, mp_pose.POSE_CONNECTIONS)

        # Lấy tọa độ tay trái và tay phải
        left_wrist = result.pose_landmarks.landmark[mp_pose.PoseLandmark.LEFT_WRIST]
        right_wrist = result.pose_landmarks.landmark[mp_pose.PoseLandmark.RIGHT_WRIST]

        left_shoulder = result.pose_landmarks.landmark[mp_pose.PoseLandmark.LEFT_SHOULDER]
        right_shoulder = result.pose_landmarks.landmark[mp_pose.PoseLandmark.RIGHT_SHOULDER]

        # Xác định tay giơ không
        if left_wrist.y < left_shoulder.y and right_wrist.y < right_shoulder.y:  # Cả hai tay giơ lên
            command = "UP"
        elif left_wrist.y < left_shoulder.y:  # Nếu cổ tay trái cao hơn vai trái
            command = "RIGHT"
        elif right_wrist.y < right_shoulder.y:  # Nếu cổ tay phải cao hơn vai phải
            command = "LEFT"
        else:
            command = "STOP"

    # Chỉ gửi lệnh mới nếu khác lệnh trước đó
    if command and command != previous_command:
        conn.sendall(f"{command}\n".encode())
        print("Gửi lệnh:", command)
        previous_command = command

    # Hiển thị khung hình
    cv2.imshow("Pose Detection", frame)

    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

# Đóng kết nối
cap.release()
cv2.destroyAllWindows()
conn.close()
server.close()