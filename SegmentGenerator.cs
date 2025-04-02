using UnityEngine;
using System.Collections;

public class SegmentGenerator : MonoBehaviour
{
    public GameObject[] segment; // Mảng chứa các prefab cảnh (1-7)
    [SerializeField] int zPos = 50; // Vị trí Z ban đầu
    [SerializeField] bool creatingSegment = false; // Trạng thái tạo cảnh
    [SerializeField] int segmentNum; // Số thứ tự cảnh được chọn
    private float gameTime = 0f; // Thời gian chơi game

    void Start()
    {
        // Đảm bảo gameTime bắt đầu từ 0 khi game khởi động
        gameTime = 0f;
    }

    void Update()
    {
        // Cập nhật thời gian chơi
        gameTime += Time.deltaTime;

        // Nếu không đang tạo segment, bắt đầu quá trình tạo
        if (!creatingSegment)
        {
            creatingSegment = true;
            StartCoroutine(SegmentGen());
        }
    }

    IEnumerator SegmentGen()
    {
        // Chọn segment dựa trên thời gian chơi
        if (gameTime <= 30f) // 0-30 giây
        {
            segmentNum = Random.Range(0, 3); // Chỉ sinh cảnh 1, 2, 3 (index 0-2)
        }
        else if (gameTime <= 120f) // 30 giây - 2 phút
        {
            segmentNum = Random.Range(0, 5); // Sinh cảnh 1, 2, 3, 4, 5 (index 0-4)
        }
        else // Sau 2 phút
        {
            segmentNum = Random.Range(0, 7); // Sinh cảnh 1, 2, 3, 4, 5, 6, 7 (index 0-6)
        }

        // Tạo segment tại vị trí zPos
        Instantiate(segment[segmentNum], new Vector3(0, 0, zPos), Quaternion.identity);
        zPos += 50; // Tăng vị trí Z cho segment tiếp theo

        // Đợi 3 giây trước khi tạo segment mới
        yield return new WaitForSeconds(1);
        creatingSegment = false;
    }
}