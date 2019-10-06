
using UnityEngine;

/// <summary>
/// Класс ячейки, которой управляет игрок
/// </summary>
public class Cell : MonoBehaviour
{
    public float _speed = 1f;
    private GameObject _moveObj;

    /// <summary>
    /// Перемещаем ячейку, когда зажата мышь и когда был сделан клик на не заблокированную ячейку
    /// </summary>
    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (_moveObj != null)
            {
                Vector3 mouse = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
                Vector3 posCam = Camera.main.ScreenToWorldPoint(mouse);
                _moveObj.transform.position = new Vector3(posCam.x, posCam.y, 0);
            }
        }
    }

    /// <summary>
    /// при клике на ячейку получаем значения экзепляра этой ячейки, для дальнейшего перемещения
    /// </summary>
    private void OnMouseDown()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            _moveObj = hit.collider.gameObject;
        }
    }

    /// <summary>
    /// В случаее если игрок переместил ячейку на пустую плитку и плитка является соседней (+/-1), то присваем этой плитке перемещённую ячейку
    /// </summary>
    private void OnMouseUp()
    {
        Vector3 forward = _moveObj.transform.TransformDirection(Vector3.forward) * 10;

        Ray ray = new Ray(_moveObj.transform.position, forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            GameObject objOnUp = hit.collider.gameObject; //элемент над которым отпустили ячейку

            if (objOnUp.tag == "Plate")
            {
                if (objOnUp.GetComponent<Plate>().Cell == null) //если ячейка пуста
                {
                    int indexCurrentPlate = _moveObj.GetComponentInParent<Plate>().IndexPlateInArray;
                    int indexPlateOnUp = objOnUp.GetComponent<Plate>().IndexPlateInArray;

                    // try- catch т.к. +/-1 и +/-5 может выйти за границы массива
                    try
                    {
                        if (indexPlateOnUp == indexCurrentPlate + 1 || indexPlateOnUp == indexCurrentPlate - 1 ||
                           indexPlateOnUp == indexCurrentPlate + 5 || indexPlateOnUp == indexCurrentPlate - 5)
                        {
                            _moveObj.GetComponentInParent<Plate>().Cell = null;

                            objOnUp.GetComponentInParent<Plate>().Cell = _moveObj;
                            _moveObj.transform.SetParent(objOnUp.transform);
                        }
                    }
                    catch
                    {

                    }
                }
            }
        }

        _moveObj = null; //обнуляем ссылку на перемещаемую ячейку
        Win.TestWin(); 
    }

    /// <summary>
    /// Перемещаем ячейку в направлении координат FinishPozition, пока она не будет на растояния _speed, от FinishPozition
    /// </summary>
    private void FixedUpdate()
    {
        float distance = Vector3.Distance(transform.position, FinishPozition);
        Vector3 direction = FinishPozition - transform.position;
        if (distance > _speed)
        {
            transform.Translate(direction * _speed * Time.deltaTime); 
        }
        else
        {
            transform.position = FinishPozition;
        }
    }

    /// <summary>
    /// Когда будет выполнено услове поеды. Запустится метод улаения элементов. В тот момент когда элемент выйдет за пределы canvas, он удалится
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionExit(Collision collision)
    {
       if( collision.gameObject.tag =="canvas")
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Позиция куда должна переместится ячейка
    /// </summary>
    public Vector3 FinishPozition { get; set; }
}
