using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Spawner))]
public class SpawnerEditor : Editor
{
    private SerializedProperty spawnables;
    private SerializedProperty spawningProbabilities;


    public void OnEnable()
    {
        spawnables = serializedObject.FindProperty("spawnables");
        spawningProbabilities = serializedObject.FindProperty("spawningProbabilities");

    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(spawnables);
        DrawProbs();
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
        //
        // try
        // {
        //     SerializedProperty itter = spawningProbabilities;
        //     itter.Next(true);
        //     while (itter.Next(false))
        //     {
        //         EditorGUILayout.PropertyField(itter);
        //     }    
        // }    catch{}    
        //
    }

    void DrawProbs()
    {
        //     float maximum;
        //     if (probs0To1.boolValue)
        //     {
        //         maximum = 1;
        //         if (GUILayout.Button("Fractions"))
        //         {
        //             probs0To1.boolValue = false;
        //             maximum = 100;
        //             for (int i = 0; i < spawningProbabilities.arraySize; i++)
        //             {
        //                 spawningProbabilities.GetArrayElementAtIndex(i).floatValue *= 100;
        //             }
        //         }
        //         
        //     }
        //     else
        //     {
        //         maximum = 100;
        //         if (GUILayout.Button("Percentages"))
        //         {
        //             probs0To1.boolValue = true;
        //             maximum = 1;
        //             
        //             for (int i = 0; i < spawningProbabilities.arraySize; i++)
        //             {
        //                 spawningProbabilities.GetArrayElementAtIndex(i).floatValue /= 100;
        //             }
        //         }
        //     }

        int probsSize = spawningProbabilities.arraySize;
        int spawsSize = spawnables.arraySize;

        if (spawsSize == 0)
        {
            spawningProbabilities.arraySize = spawsSize;
            return;
        }

        if (probsSize != spawsSize)
        {
            spawningProbabilities.arraySize = spawsSize;
            if (probsSize > spawsSize)
            {
                float newLimit = 0;
                for (int i = 0; i < spawsSize; i++)
                {
                    newLimit += spawningProbabilities.GetArrayElementAtIndex(i).floatValue;
                }


                float fraction = newLimit / 100;
                for (int i = 0; i < spawningProbabilities.arraySize; i++)
                {
                    spawningProbabilities.GetArrayElementAtIndex(i).floatValue *= fraction;
                }
                Debug.Log("Updated Probs by SizeChange");
            }
        }

        float newMax = 100;
        for (int i = 0; i < spawningProbabilities.arraySize; i++)
        {
            Object objec = spawnables.GetArrayElementAtIndex(i).objectReferenceValue;
            if (objec == null) continue;

            float value = EditorGUILayout.Slider($"{objec.name} [0-{newMax}]", spawningProbabilities.GetArrayElementAtIndex(i).floatValue, 0, newMax);
            spawningProbabilities.GetArrayElementAtIndex(i).floatValue = value;
            newMax -= value;
        }

        if (newMax == 100)
        {
            spawningProbabilities.GetArrayElementAtIndex(0).floatValue = newMax;
        }
        else
        {
            spawningProbabilities.GetArrayElementAtIndex(spawningProbabilities.arraySize - 1).floatValue = newMax;
        }

    }
}