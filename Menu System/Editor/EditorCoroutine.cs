using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[InitializeOnLoad]
public class EditorCoroutine
{
    private static readonly List<IEnumerator> CoroutineInProgress = new List<IEnumerator>();
    private static int CurrentExecute = 0;

    static EditorCoroutine()
    {
        EditorApplication.update += ExecuteCoroutine;
    }
    
    private static void ExecuteCoroutine()
    {
        if (CoroutineInProgress.Count <= 0)
        {
            return;
        }

        CurrentExecute = (CurrentExecute + 1) % CoroutineInProgress.Count;
        if (CoroutineInProgress[CurrentExecute].MoveNext() == false)
        {
            CoroutineInProgress.RemoveAt(CurrentExecute);
        }
    }
    
    public static IEnumerator StartCoroutine(IEnumerator newCorou)
    {
        CoroutineInProgress.Add(newCorou);
        return newCorou;
    }
    
    public static void StopCoroutine(IEnumerator corou)
    {
        CoroutineInProgress.Remove(corou);
    }

}