using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Класс создания игрового поля и завершения игры
/// </summary>
public class CreateGameFiled : MonoBehaviour
{
    private static GameObject[] _plates;
    public GameObject _plate;
    public GameObject _BlockCell;
    public GameObject _canvas;

    public GameObject[] _playerCells;
    private static Text _lblWin;

    private void Awake()
    {
        _lblWin = GameObject.Find("lblWin").GetComponent<Text>();
        _lblWin.enabled = false;
    }

    void Start()
    {
        _plates = new GameObject[25];
        StartCoroutine(CreatePlate());
    }

    public static GameObject[] GetArrayPlats { get { return _plates; } }
    /// <summary>
    /// Метод создания фоновых плиток, на которых будут отображатся игровые ячейки
    /// </summary>
    /// <returns></returns>
    private IEnumerator CreatePlate()
    {
        float paddingPlate = 0.1f;
        Vector3 sizePlate = _plate.transform.localScale;

        float posY_StartDrawPlate = -(5 * sizePlate.y) / 2;
        float pozY = posY_StartDrawPlate;
        for (int i = 0; i < 25; i++)
        {
            float posX_StartDrawPlate = -(5 * sizePlate.x) / 2;
            float pozX = posX_StartDrawPlate+ i % 5 * (sizePlate.x + paddingPlate) ;

            _plates[i]= Instantiate(_plate, new Vector3(pozX, pozY, 0.1f), Quaternion.identity);
            _plates[i].transform.SetParent(_canvas.transform, true);
            _plates[i].GetComponent<Plate>().IndexPlateInArray = i;

            //делаем поле квадратным и каждые 5 ячеек переносим отрисовку на новую строку
            if (i %5== 4)
                pozY += (sizePlate.y + paddingPlate);
            yield return new WaitForSeconds(0.2f);
        }
        
       StartCoroutine(CreateBlockCells());
       CreateWinnerCondition(pozY);
    }

    /// <summary>
    /// Установка заблокированных ячеек.
    /// </summary>
    /// <returns></returns>
    private IEnumerator CreateBlockCells()
    {
        //плитки повторяют своё местоположение каждые 2 ряда, по этому i умножается на 10. 
       for( int i=0; i<3;i++) 
        {
            GameObject objFirst = _plates[i * 10 + 1];
            GameObject objSecond = _plates[i * 10 + 3];
            objFirst.GetComponent<Plate>().Cell= Instantiate(_BlockCell, objFirst.transform.position, Quaternion.identity);
            objFirst.GetComponent<Plate>().Cell.transform.SetParent(_canvas.transform, true);

            objSecond.GetComponent<Plate>().Cell= Instantiate(_BlockCell, objSecond.transform.position, Quaternion.identity);
            objSecond.GetComponent<Plate>().Cell.transform.SetParent(_canvas.transform, true);

            yield return new WaitForSeconds(0.2f);
        }

        CreatePlayerCells();
    }

    /// <summary>
    /// Метод инициализации создания ячеек, которыми управляет пользователь.
    /// </summary>
    private void CreatePlayerCells()
    {
       for (int i=0; i<_playerCells.Length; i++)
        {
            StartCoroutine(CreateCell(_playerCells[i]));
        }
    }

    /// <summary>
    /// Метод добавления ячеек на сцену по указанному типу.
    /// </summary>
    /// <param name="cellType"></param>
    /// <returns></returns>
    private IEnumerator CreateCell(GameObject cellType)
    {
        for (int i=0; i<5; i++)
        {
            bool statusCreate=true;
            while (statusCreate)
            {
                GameObject plate = _plates[UnityEngine.Random.Range(0,_plates.Length)];
                Plate scriptsPlate = plate.GetComponent<Plate>();
                //если Cell == null, значит там не добавлен элемент Cell, следовательно на данной Plate нет никаких плиток (Cell)
                if (scriptsPlate.Cell == null)
                {
                    scriptsPlate.Cell= Instantiate(cellType, new Vector3(-1f, 0, -2f), Quaternion.identity); 
                    scriptsPlate.Cell.transform.SetParent(plate.transform, true); 
                    statusCreate = false;
                    yield return new WaitForSeconds(0.2f);
                }
            }
        }
       
    }

    /// <summary>
    /// Генерация условия победы игрока, т.е. определение при каких позициях ячеек игрок выиграет.
    /// </summary>
    /// <param name="posFinishDraw"> указаыет где закончилась отрисовка игрового поля, требуется для отрисовки "Игравого условия победы"</param>
    private void CreateWinnerCondition(float posFinishDraw)
    {
        //создание массива вариантов плиток, которыми управляет игрок
        List<string> listTypePlayerCell = new List<string>();
        for (int i = 0; i < _playerCells.Length; i++)
        {
            listTypePlayerCell.Add(_playerCells[i].tag);
        }
        //Определяем в каких калонках какие должны быть наборы игровых плиток
        string columnFirst = listTypePlayerCell[UnityEngine.Random.Range(0, listTypePlayerCell.Count)];
        listTypePlayerCell.Remove(columnFirst);

        string columnSecond = listTypePlayerCell[UnityEngine.Random.Range(0, listTypePlayerCell.Count)];
        listTypePlayerCell.Remove(columnSecond);

        string columnThrid = listTypePlayerCell[UnityEngine.Random.Range(0, listTypePlayerCell.Count)];
        listTypePlayerCell.Remove(columnThrid);

        Win.SetWinnerCombination(columnFirst, columnSecond, columnThrid); //отправляем данные, для создания "победной" комбинации, с которой будет сравниваться текущие положени плиток

        DrawWinnerCondition(columnFirst, columnSecond, columnThrid, posFinishDraw);
    }

    /// <summary>
    /// Отображаем "игровое условие победы". Т.е. отрисовываем над колонками где и какие должны быть плитки
    /// </summary>
    /// <param name="columnFirst"></param>
    /// <param name="columnSecond"></param>
    /// <param name="columnThrid"></param>
    /// <param name="posY_FinishDraw"></param>
    private void DrawWinnerCondition(string columnFirst, string columnSecond, string columnThrid, float posY_FinishDraw)
    {
        Vector3 sizePlate = _plate.transform.localScale;
        float paddingPlate = 0.1f;

        float posX_ColumnFirst = -(5 * sizePlate.x) / 2f;
        //Рассчёт координат. 2 и 4 - индекс в массиве 
        float posX_ColumnSecond = posX_ColumnFirst + 2 * (sizePlate.x + paddingPlate); // не работает =0
        float posX_ColumnThrid = posX_ColumnFirst + 4 * (sizePlate.x + paddingPlate); // и =(5 * sizePlate.x) / 2f.  ХЗ почему

        Vector3 posColumnFirst = new Vector3(posX_ColumnFirst, posY_FinishDraw, 1f);
        Vector3 posColumnSecond = new Vector3(posX_ColumnSecond, posY_FinishDraw, 1f);
        Vector3 posColumnThrid = new Vector3(posX_ColumnThrid, posY_FinishDraw, 1f);

        //Исходя из данных переданных в метод, отрисовываем попорядку плитки
        for (int i = 0; i < _playerCells.Length; i++)
        {
            if (_playerCells[i].tag == columnFirst)
            {
                GameObject playerFirstCell = Instantiate(_playerCells[i], posColumnFirst, Quaternion.identity);
                playerFirstCell.GetComponent<Cell>().enabled = false;
                playerFirstCell.transform.SetParent(_canvas.transform, true);
            }

            if (_playerCells[i].tag == columnSecond)
            {
                GameObject playerSecondCell = Instantiate(_playerCells[i], posColumnSecond, Quaternion.identity);
                playerSecondCell.transform.SetParent(_canvas.transform, true);
                playerSecondCell.GetComponent<Cell>().enabled = false;
            }

            if (_playerCells[i].tag == columnThrid)
            {
                GameObject playerThridCell = Instantiate(_playerCells[i], posColumnThrid, Quaternion.identity);
                playerThridCell.transform.SetParent(_canvas.transform, true);
                playerThridCell.GetComponent<Cell>().enabled = false;
            }
        }
    }



    /// <summary>
    /// Метод завершения игры, который убирает игровые элементы с поля
    /// </summary>
    public static void FinishGame ()
    {
        _lblWin.enabled = true;
        DeletePlats();
    }

    /// <summary>
    /// Метод удаления плиток и ячеек
    /// </summary>
    private static void DeletePlats()
    {
        //удаление ячеек
        MoveCell(GameObject.FindGameObjectsWithTag("CellBlock"));
        MoveCell(GameObject.FindGameObjectsWithTag("CellGreen"));
        MoveCell(GameObject.FindGameObjectsWithTag("CellRed"));
        MoveCell(GameObject.FindGameObjectsWithTag("CellYellow"));

        //удаление фоновых плиток.
        foreach (var plate in _plates)
        {
            Destroy(plate);
        }
    }

    /// <summary>
    /// Удаление ячеек, которые отображаюся поверх плиток
    /// </summary>
    /// <param name="arrCells"></param>
    private static void MoveCell(GameObject[] arrCells)
    {
        GameObject poleCanvas = GameObject.Find("canvas");
        Vector3 posMove = new Vector3(poleCanvas.transform.position.x, -10f, poleCanvas.transform.position.z);
        foreach( var cell in arrCells)
        {
            cell.transform.SetParent(poleCanvas.transform, true);
            cell.GetComponent<Cell>().enabled = true; //требуется т.к. у "победного игрового условия" данный метод отключен, чтобы плитки не перемещались.
            cell.GetComponent<Cell>().FinishPozition = posMove;
        }
    }
}
