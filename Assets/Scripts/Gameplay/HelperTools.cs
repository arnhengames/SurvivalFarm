using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;

[EditorTool("Level Generator")]
class HelperTools : EditorTool
{
    static LevelManager levelManager;

    [MenuItem("Helper Tools/Create Active Level Map Nodes")]
    static void CreateActiveLevelMapNodes()
    {
        levelManager = FindObjectOfType<LevelManager>();
        levelManager.CreateActiveLevelMap();
    }
}
