using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Playables;

public class SlotColumn : MonoBehaviour
{
    [SerializeField] private GameObject slotElementPrefab;

    private List<GameObject> _createdSlotElementObjs;
    private List<SlotElement> _createdSlotElements;
    private int _rowSize;
    private bool _hasStopped = true;
    private int _currentRowID = 0;      // _rowSize + 1 is the extra row for the translate in rotate animation
    private int[] _columnInfo;


    private void Update()
    {

    }


    public void CreateColumn(int rowSize, int[] columnInfo)
    {
        SetRowSize(rowSize);
        SetColumnInfo(columnInfo);
        CreateSlotElements();
        SetColumnRectHeight();
    }

    private void SetRowSize(int rowSize) => _rowSize = rowSize;

    private void SetColumnInfo(int[] columnInfo)
    {
        _columnInfo = new int[_rowSize + 1];
        for(int i = 0; i < columnInfo.Length; i++)
        {
            _columnInfo[i] = i != _rowSize ? columnInfo[i] : Random.Range(0, 10);
        }
    }

    private void CreateSlotElements()
    {
        _createdSlotElementObjs = new List<GameObject>();
        _createdSlotElements = new List<SlotElement>();
        float slotElementHeight = slotElementPrefab.GetComponent<RectTransform>().rect.height;
        GameObject instSlotElementObject;
        SlotElement instSlotElement;
        Vector2 instPos;

        for(int i = 0;  i < _rowSize + 1; i++)
        {
            instPos = new Vector2(0, i * slotElementHeight);
            instSlotElementObject = Instantiate(slotElementPrefab, gameObject.transform, false);
            instSlotElementObject.transform.localPosition = instPos;
            instSlotElementObject.transform.name = $"SlotElement {i}";
            _createdSlotElementObjs.Add(instSlotElementObject);
            instSlotElement = instSlotElementObject.GetComponent<SlotElement>();
            _createdSlotElements.Add(instSlotElement);
            SetRowValue(i);
        }
    }

    private void SetRowValue(int rowID)
    {
        _createdSlotElements[rowID].SetNumberText(_columnInfo[rowID]);
    }

    private void SetColumnRectHeight()
    {
        float slotElementHeight = slotElementPrefab.GetComponent<RectTransform>().rect.height;
        RectTransform slotElementRT = gameObject.GetComponent<RectTransform>();
        slotElementRT.sizeDelta = new Vector2(slotElementRT.sizeDelta.x, slotElementHeight * _rowSize);
    }

    public bool CheckColumnStopped() => _hasStopped;

    public IEnumerator StartRotate(int[] newColumnInfo, int turnCount, float rotateTime, int secondTurnCount = 0, float secondRotateTime = 0f)      //Stops after turnCount at exact the rotateTime      
    {
        _hasStopped = false;
        yield return StartCoroutine(RotateCoroutine(newColumnInfo, turnCount, rotateTime));
        if(secondTurnCount > 0)
        {
            yield return StartCoroutine(RotateCoroutine(newColumnInfo, secondTurnCount, secondRotateTime));
        }
        _hasStopped = true;
        yield return null;
    }

    private IEnumerator RotateCoroutine(int[] newColumnInfo, int turnCount, float rotateTime)
    {
        float slotElementHeight = slotElementPrefab.GetComponent<RectTransform>().rect.height;
        float columnHeight = (_rowSize + 1) * slotElementHeight;
        float totalDistance = columnHeight * turnCount;
        float rotateSpeed =  totalDistance / (rotateTime * 60);
        float distanceTranslated = 0f;
        while(distanceTranslated < totalDistance)
        {
            distanceTranslated += rotateSpeed;
            foreach(GameObject slotElementObj in _createdSlotElementObjs)
            {
                RotateSlotElementObj(slotElementObj, rotateSpeed);
            }
            if (_createdSlotElementObjs[_currentRowID].transform.localPosition.y <= -slotElementHeight)
            {
                _createdSlotElementObjs[_currentRowID].transform.localPosition = new Vector2(_createdSlotElementObjs[_currentRowID].transform.localPosition.x, slotElementHeight * _rowSize);   //Transport passed obj to top of the column
                if(_currentRowID < _rowSize && distanceTranslated >= columnHeight * (turnCount - 1))
                {
                    SetSlotElementText(_currentRowID, newColumnInfo[_currentRowID]);
                }
                else
                {
                    SetSlotElementText(_currentRowID);
                }
                _currentRowID = _currentRowID == _rowSize ? 0 : _currentRowID + 1;
            }
            yield return null;
        }
        ResetSlotElementsPositions();
    }


    private void RotateSlotElementObj(GameObject slotElementObj, float rotateSpeed) => slotElementObj.transform.localPosition = new Vector2(slotElementObj.transform.localPosition.x, slotElementObj.transform.localPosition.y - rotateSpeed);

    private void ResetSlotElementsPositions()
    {
        _currentRowID = 0;
        float slotElementHeight = slotElementPrefab.GetComponent<RectTransform>().rect.height;
        for (int i = 0; i < _createdSlotElementObjs.Count; i++)
        {
            _createdSlotElementObjs[i].transform.localPosition = new Vector2(_createdSlotElementObjs[i].transform.localPosition.x, slotElementHeight * i);
        }
    }

    private void SetSlotElementText(int rowID, int value = -1)  // -1 means random
    {
        value = value == -1 ? Random.Range(0, 10) : value;
        _createdSlotElements[rowID].SetNumberText(value);
    }
    
}
