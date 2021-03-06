using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventHelper : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IUpdateSelectedHandler
{
  private bool buttonPressed;
  public float waitTime = 0.45f;
  private static bool moved;
  public static bool Moved { get => moved; set => moved = value; }

  public void OnUpdateSelected(BaseEventData data)
  {
    if (buttonPressed && !moved)
    {
      if (data.selectedObject.name == "right")
      {
        moved = true;
        Snake.Instance.onClickRight();
        StartCoroutine(Wait());
      }
      if (data.selectedObject.name == "left")
      {
        moved = true;
        Snake.Instance.onClickLeft();
        StartCoroutine(Wait());
      }
      if (data.selectedObject.name == "up")
      {
        moved = true;
        Snake.Instance.onClickUp();
        StartCoroutine(Wait());
      }
      if (data.selectedObject.name == "down")
      {
        moved = true;
        Snake.Instance.onClickDown();
        StartCoroutine(Wait());
      }
    }
  }
  IEnumerator Wait()
  {
    yield return new WaitForSeconds(waitTime);
    moved = false;
  }
  public void OnPointerDown(PointerEventData eventData)
  {
    buttonPressed = true;
  }
  public void OnPointerUp(PointerEventData eventData)
  {
    buttonPressed = false;
  }
}
