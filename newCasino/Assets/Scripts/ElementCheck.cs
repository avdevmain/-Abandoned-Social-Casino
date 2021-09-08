using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementCheck : MonoBehaviour
{
    public static bool catchLastOnes;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.name.StartsWith(name)) //Обработка коллизий только на той же линии
        {
            ButtonHandler.lastElementsID[int.Parse(name)] = int.Parse(collision.collider.name.Substring(1));
            
        }
    }

}
