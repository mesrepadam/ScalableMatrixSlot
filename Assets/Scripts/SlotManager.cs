using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotManager : MonoBehaviour
{
    [Header("Basic Slot Settings")]
    [SerializeField] private int slotRow = 3;
    [SerializeField] private int slotColumn = 5;
    [SerializeField] private GameObject slotColumnPrefab;
    [SerializeField] private Vector2 slotColumnInstPos;
    [Header("Column Rotation Settings")]
    [SerializeField] private int standartColumnTurns = 3; //Each column turn count except last two (Each column add turns before the previous column)
    [SerializeField] private float standartColumnTurnTime = 2; //Each column turns in value(Each column add turnTime before the previous column's turn time
    [SerializeField] private int lastTwoColumnTurns = 3; //When other columns stops, last two column turns this much turn end stop.
    [SerializeField] private float lastTwoColumnTurnTime = 1; //When other columns stops, last two column turns this much time end stop.

    private List<SlotColumn> _slotColumns;
    private int[,] _slotInfo;

    private void Start()
    {
        SetSlotInfo();
        CreateAllSlotColumns();
    }





    private void SetSlotInfo()
    {
        _slotInfo = new int[slotColumn, slotRow];
        RandomizeSlotInfo();
    }

    private void RandomizeSlotInfo()
    {
        for (int i = 0; i < slotColumn; i++)
        {
            for (int j = 0; j < slotRow; j++)
            {
                _slotInfo[i, j] = Random.Range(0, 10);
            }
        }
    }

    private void CreateAllSlotColumns()
    {
        _slotColumns = new List<SlotColumn>();
        for (int i = 0; i < slotColumn; i++)
        {
            CreateSlotColumn(i);
        }
    }

    private void CreateSlotColumn(int columnID)
    {
        GameObject instSlotColumnPrefab;
        SlotColumn instSlotColumn;
        float slotColumnPrefabWidth = slotColumnPrefab.GetComponent<RectTransform>().rect.width;
        Vector2 instPos = new Vector2(slotColumnInstPos.x + columnID * slotColumnPrefabWidth, slotColumnInstPos.y);
        instSlotColumnPrefab = Instantiate(slotColumnPrefab, instPos, Quaternion.identity, gameObject.transform);
        instSlotColumnPrefab.transform.GetComponent<RectTransform>().anchoredPosition = instPos;
        instSlotColumn = instSlotColumnPrefab.GetComponent<SlotColumn>();
        instSlotColumn.CreateColumn(slotRow, GetColumnInfo(columnID));
        _slotColumns.Add(instSlotColumn);
    }

    private int[] GetColumnInfo(int columnID)
    {
        int[] columnInfo = new int[slotRow];
        for(int i = 0; i < slotRow;i++)
        {
            columnInfo[i] = _slotInfo[columnID,i];
        }
        return columnInfo;
    }

    public void RotateSlotColumns()
    {
        if (CheckAllSlotsStopped())
        {
            int turn = 0;
            float turnTime = 0;
            int secondTurn = 0;
            float secondTurnTime = 0;
            RandomizeSlotInfo();
            for (int i = 0; i < slotColumn; i++)
            {
                if(i < slotColumn - 2)
                {
                    turn += standartColumnTurns;
                    turnTime += standartColumnTurnTime;
                }
                else
                {
                    secondTurn = lastTwoColumnTurns;
                    secondTurnTime = lastTwoColumnTurnTime;
                }
                StartCoroutine(_slotColumns[i].StartRotate(GetColumnInfo(i), turn, turnTime, secondTurn, secondTurnTime));
            }
        }
    }

    private bool CheckAllSlotsStopped()
    {
        return _slotColumns.TrueForAll(member => member.CheckColumnStopped());
    }
}
