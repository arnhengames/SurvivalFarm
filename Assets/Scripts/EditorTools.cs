using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;

[EditorTool("Level Generator")]
class EditorTools : EditorTool
{
    static LevelManager levelManager;

    [MenuItem("Level Generation/Create Active Level Map Nodes")]
    static void CreateActiveLevelMapNodes()
    {
        levelManager = FindObjectOfType<LevelManager>();
        levelManager.CreateLevel();
    }
}
