using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Класс фоновой плитки, на которую крепится ячейка Cell
/// </summary>
public class Plate : MonoBehaviour
{
    private GameObject _cell; //У каждой плитки своя ячейка, если _cell==null, значит на текущий момент плитка пуста
    private string _nameTypeCell = ""; //названия ячейки, которая находится на плитки

    public GameObject Cell {
        get 
        {
            return _cell;
        }
        set 
        {
            if (value != null) //если ячейку не обнуляют
            {
                value.GetComponent<Cell>().FinishPozition = new Vector3(transform.position.x, transform.position.y, -1f);
                _nameTypeCell = value.tag;
            }
            else
            {
                _nameTypeCell = "";
            }

            _cell = value;

        } 
    }
    /// <summary>
    /// Получить название типа ячейки
    /// </summary>
    public string GetNameTypeCell { get { return _nameTypeCell; } }

    /// <summary>
    /// Индекс положения плитки 
    /// </summary>
    public int IndexPlateInArray { get; set; }


}
