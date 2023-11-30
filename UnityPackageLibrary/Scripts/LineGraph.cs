using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Globalization;
using System.Linq;

public class LineGraph : MonoBehaviour
{
    // 샘플링할 데이터의 수 
    public int SAMPLE_RATE = 20;

    // 그래프 높이의 절반을 구하는 데 사용되는 값
    private const float HALF_HEIGHT_FACTOR = 0.5f;

    // 그래프의 Y축 최대값
    public int maxYValue = 24;

    // 그래프에 사용될 점 오브젝트
    public GameObject dot;

    // 그래프의 선분을 생성할 때 사용되는 프리팹
    public GameObject linePrefab;

    // 점 오브젝트들을 묶을 부모 객체
    public Transform dotGroup;

    // 선분 오브젝트들을 묶을 부모 객체
    public Transform lineGroup;

    // 사용자 점수의 색상
    public Color userScoreColor;

    // 선분의 색상
    public Color lineColor;

    // 그래프가 그려질 영역
    public RectTransform graphArea;

    // CSV 파일이 할당될 객체
    public TextAsset csvFile;

    // 사용자 점수를 담는 리스트
    public List<float> userScore = new List<float>();

    // 그래프의 가로 길이
    private float graph_Width;

    // 그래프의 세로 길이
    private float graph_Height;

    // 스크립트가 시작될 때 호출되는 함수
    void Start()
    {
        // CSV 데이터를 파싱
        ParseCSVData();

        // 그래프의 가로 길이를 계산
        graph_Width = graphArea.rect.width;

        // 그래프의 세로 길이를 계산
        graph_Height = graphArea.rect.height;

        // 그래프를 그림
        DrawGoldGraph();
    }


    private void ParseCSVData()
    {
        // CSV 파일을 줄바꿈으로 분리해서 각 줄을 data 배열에 저장
        string[] data = csvFile.text.Split('\n');

        // 찾으려는 특정 값
        string dateValue = "2023-08-09";


        // 각 행마다 실행
        foreach (string row in data)
        {
            // 줄을 ',' 기호로 분리해서 각 값을 values 배열에 저장
            string[] values = row.Split(',');

            // 첫 번째 열의 값이 특정 값과 일치하면 해당 행의 정해진 열부터 값을 리스트에 추가
            if (values[0] == dateValue) //values.Length > startColumnIndex &&
            {
                values = values.Where(value => !string.IsNullOrEmpty(value)).ToArray();

                Debug.Log(values[2]);
                // 시작 열의 인덱스 
                // (예: 3열부터 시작하려면 2로 설정
                // 끝에서 SAMPLE_RATE만큼 출력하고 싶으면 values.Length-SAMPLE_RATE)
                int startColumnIndex = 3;


                for (int i = startColumnIndex; i < SAMPLE_RATE + startColumnIndex; i++)
                {
                    try
                    {
                        if (float.TryParse(values[i], NumberStyles.Any, CultureInfo.InvariantCulture, out float result))
                        {
                            // 그 값을 사용자 점수 리스트에 추가

                            userScore.Add(result);
                        }
                    }
                    catch (IndexOutOfRangeException e)
                    {
                        userScore.Add(0);
                        continue;
                    }
                }

                // 만약 하나의 행만 찾고 싶다면, 여기서 반환하거나 break로 루프를 종료
                break;
            }
        }
    }


    // 그래프를 그리는 함수
    private void DrawGoldGraph()
    {
        // 그래프의 시작점의 x 위치
        float startPosition = -graph_Width / 2;

        // 그래프의 최대 y 위치
        float maxYPosition = graph_Height;

        // 이전 점의 위치
        Vector2 previousPos = Vector2.zero;

        // 샘플링 수만큼 반복
        for (int i = 0; i < SAMPLE_RATE; i++)
        {
            if (userScore[i] == 0)
                continue;

            // y 위치의 비율 계산
            float yPosOffset = userScore[i] / maxYValue;

            // 현재 점의 위치 계산
            Vector2 currentPos = new Vector2(startPosition + (graph_Width / (SAMPLE_RATE - 1) * i),
                maxYPosition * yPosOffset - maxYPosition * HALF_HEIGHT_FACTOR);

            // 점 오브젝트 생성
            GameObject dot = CreateDot(currentPos, userScoreColor, userScore[i].ToString("0.0"), dotGroup);

            // 첫번째 점이 아니라면, 이전 점과 현재 점 사이에 선을 그림
            if (i > 0)
            {
                CreateLine(previousPos, currentPos, lineColor, lineGroup);
            }

            // 이전 점의 위치를 현재 점의 위치로 업데이트
            previousPos = currentPos;
        }
    }

    // 점을 생성하는 함수
    private GameObject CreateDot(Vector2 position, Color color, string text, Transform parent)
    {
        // dot 프리팹으로부터 점 오브젝트를 생성
        GameObject dot = Instantiate(this.dot);

        // 점 오브젝트의 부모를 설정
        dot.transform.SetParent(parent, false);

        // 점 오브젝트의 스케일을 1로 설정
        dot.transform.localScale = new Vector3(1, 1, 1);

        // 점 오브젝트의 RectTransform 컴포넌트와 Image 컴포넌트를 가져옴
        RectTransform dotRT = dot.GetComponent<RectTransform>();
        Image dotImage = dot.GetComponent<Image>();

        // 점의 색상을 설정
        dotImage.color = color;

        // 점의 위치를 설정
        dotRT.anchoredPosition = position;

        // 점 오브젝트의 자식으로 있는 TextMeshProUGUI 컴포넌트를 가져와서 텍스트를 설정
        TextMeshProUGUI textComponent = dot.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        textComponent.text = text;

        return dot;
    }

    // 선을 생성하는 함수
    private void CreateLine(Vector2 startPos, Vector2 endPos, Color color, Transform parent)
    {
        // linePrefab으로부터 선 오브젝트를 생성하고 부모를 설정
        GameObject line = Instantiate(linePrefab, parent);

        // 선 오브젝트의 스케일을 1로 설정
        line.transform.localScale = new Vector3(1, 1, 1);

        // 선 오브젝트의 Image 컴포넌트를 가져와서 색상을 설정
        Image lineImage = line.GetComponent<Image>();
        lineImage.color = color;

        // 선 오브젝트의 RectTransform 컴포넌트를 가져와서 위치와 크기를 설정
        RectTransform lineRT = line.GetComponent<RectTransform>();
        lineRT.anchoredPosition = startPos + (endPos - startPos) / 2;

        // 선의 두께 설정
        float thickness = 2;

        // 선의 길이 계산
        float lineLength = Vector2.Distance(startPos, endPos);

        // 선의 크기 설정
        lineRT.sizeDelta = new Vector2(lineLength, thickness);

        // 선의 각도 계산
        float angle = Mathf.Atan2((endPos.y - startPos.y), endPos.x - startPos.x) * Mathf.Rad2Deg;

        // 선의 회전 설정
        lineRT.localRotation = Quaternion.Euler(0, 0, angle);
    }
}