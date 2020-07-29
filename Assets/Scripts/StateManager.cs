using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class StateManager : MonoBehaviour
{
   public Stack<Canvas> myQ = new Stack<Canvas>();

   public void push(Canvas c){
       myQ.Push(c);
   }
   public void pop(){
       Destroy(myQ.Pop());
       FindObjectOfType<GameManager>().enabled = true;
   }
}
