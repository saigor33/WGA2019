using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class Win  : MonoBehaviour
{
    public  GameObject _cellBlock;
    public GameObject _cellRed;
    public GameObject _cellGreen;
    public GameObject _cellYellow;

    private static List<string> _listTypeCell;
    private static int[] _winnerCombination;

    private void Start()
    {
        // Создание списка названия ячеек, которые могут быть добавлены на плитку. "" - значит, что плитка пустая
        _listTypeCell = new List<string>();

        _listTypeCell.Add(_cellBlock.tag); //Нужно оставить на этом месте, т.к. метод SetWinnerCombination ссылает на этот тип ячейки по индексу "0"
        _listTypeCell.Add(_cellRed.tag);
        _listTypeCell.Add(_cellGreen.tag);
        _listTypeCell.Add(_cellYellow.tag);
        _listTypeCell.Add("");

    }
    /// <summary>
    /// Метод проверки выполнения условия победы
    /// </summary>
    public static void TestWin()
    {
        CreateArrayIndexTypePlate();
    }

    /// <summary>
    /// Создания массива текущего положения игровых плиток.
    /// </summary>
    private static void CreateArrayIndexTypePlate()
    {
        GameObject[] platsArr = CreateGameFiled.GetArrayPlats;
        int[] arrayIndexTypePlate =new int[platsArr.Length];

        for (int i = 0; i < platsArr.Length; i++)
        {
            arrayIndexTypePlate[i] = GetIndexTypeCell(platsArr[i].GetComponent<Plate>().GetNameTypeCell);
        }

       FinishGame(CheckWinningCombination(arrayIndexTypePlate));
    }

    /// <summary>
    /// Метод получения индека(Номер типа) плитки, по её названию тега(типа)
    /// </summary>
    /// <param name="typeCellValue"></param>
    /// <returns></returns>
    private static int GetIndexTypeCell (string typeCellValue)
    {
        int indexType = -1;
        foreach (var typeCell in _listTypeCell)
        {
            if (typeCellValue == typeCell)
            {
                break;
            }
            indexType++;
        }
        return indexType;
    }
    /// <summary>
    /// Проверка совпадения текущей комбинации ячеек и выигршной.
    /// </summary>
    /// <param name="arrayIndexTypePlate"></param>
    /// <returns></returns>
    private static bool CheckWinningCombination(int[] arrayIndexTypePlate)
    {
        bool statusWin = true;

        for (int i = 0; i < _winnerCombination.Length; i++)
        {
            if (_winnerCombination[i] != arrayIndexTypePlate[i])
            {
                statusWin = false;
                break;
            }
        }
        return statusWin;
    }

    /// <summary>
    /// Метод проверки активации завершения игры
    /// </summary>
    /// <param name="statusFinishGame"></param>
    private static void FinishGame (bool statusFinishGame)
    {
        if (statusFinishGame)
        {
            CreateGameFiled.FinishGame();
        }
    }

    /// <summary>
    /// Метод создания выиграшной комбинации
    /// </summary>
    /// <param name="typeCellColumnFirs"></param>
    /// <param name="typeCellColumnSecond"></param>
    /// <param name="typeCellColumnThrid"></param>
    public static void SetWinnerCombination (string typeCellColumnFirs, string typeCellColumnSecond, string typeCellColumnThrid)
    {
        _winnerCombination = new int[CreateGameFiled.GetArrayPlats.Length];
        for (int i=0; i< _winnerCombination.Length; i++)
        {
            switch (i%10) //10 - т.к. заблокированные ячейки повторяются каждые 2 ряда (2*5=10 ячеек)
            {
                case 0: case 5:
                    {
                        _winnerCombination[i] = GetIndexTypeCell(typeCellColumnFirs);
                        break;
                    }
                case 2: case 7:
                    {
                        _winnerCombination[i] = GetIndexTypeCell(typeCellColumnSecond);
                        break;
                    }
                case 4: case 9:
                    {
                        _winnerCombination[i] = GetIndexTypeCell(typeCellColumnThrid);
                        break;
                    }
                case 1: case 3:
                    {
                        _winnerCombination[i] = GetIndexTypeCell(_listTypeCell[0]);  //индекс ячейки, куда походить нельзя
                        break;
                    }
                default:
                    {
                        _winnerCombination[i] = GetIndexTypeCell(""); // индекс пустой ячейки
                        break;
                    }
            }
        }
    }
}
