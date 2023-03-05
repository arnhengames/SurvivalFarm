using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.Build;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class LevelEditor : MonoBehaviour
{
    [Header("Editor Settings")]
    [SerializeField] float cameraControlSpeed = 5f;
    [SerializeField] float cameraPanSpeed = 10f;
    [SerializeField] EditorScene myScene;

    //GameObjects
    GameObject selectionBoxNodeIndicator;
    GameObject selectionBoxHeightIndicator;
    Camera cam;
    TMP_Text gridSelectionDisplay;

    //Helper variables
    float zoom = 18f;
    bool cameraPan = false;

    Vector2 movement = Vector2.zero;
    Vector2 mousePos = Vector2.zero;
    Vector2 mouseDelta = Vector2.zero;

    GridPosition mouseGridPosition = new GridPosition(0, 0);
    GridPosition currentGridSelection = new GridPosition(0, 0, 0);
    List<GameObject> selectionDisplay = new List<GameObject>();
    int heightSelection = 0;

    GridPosition mouseStartingPosition;
    GridPosition[] selectedGridPositions;
    bool selectionMode = false;

    private void Start()
    {
        myScene.Initialize();

        selectionBoxNodeIndicator = GameObject.Find("Selection Block Node Indicator");
        selectionBoxHeightIndicator = GameObject.Find("Selection Block Height Indicator");
        cam = GameObject.Find("MainCamera").GetComponent<Camera>(); 
        gridSelectionDisplay = GameObject.Find("Current Selection").GetComponent<TMP_Text>();
    }

    void FixedUpdate()
    {
        //Camera Control
        cam.orthographicSize = zoom;

        cam.transform.position = new Vector3(cam.transform.position.x + (movement.x * Time.fixedDeltaTime * cameraControlSpeed),
                                            cam.transform.position.y + (movement.y * Time.fixedDeltaTime * cameraControlSpeed),
                                            cam.transform.position.z);

        if (cameraPan)
        {
            float variablePan = cameraPanSpeed * (zoom * 0.1f);

            cam.transform.position = new Vector3(cam.transform.position.x + (mouseDelta.x * Time.fixedDeltaTime * variablePan),
                                        cam.transform.position.y + (mouseDelta.y * Time.fixedDeltaTime * variablePan),
                                        cam.transform.position.z);
        }

        //Selection Control
        mouseGridPosition = LevelData.TransformPosToGridPos(cam.ScreenToWorldPoint(mousePos));

        //If mousegridposition is on current grid
        currentGridSelection.AssignValues(mouseGridPosition, heightSelection);
        gridSelectionDisplay.text = currentGridSelection.GetValueString();

        selectionBoxNodeIndicator.transform.position = LevelData.GridIndexToTransformPos(currentGridSelection.w, currentGridSelection.l, 0);
        selectionBoxHeightIndicator.transform.position = LevelData.GridIndexToTransformPos(currentGridSelection);

        //Placement Control
        if (selectionMode)
        {
            selectionBoxNodeIndicator.SetActive(false);
            selectionBoxHeightIndicator.SetActive(false);

            selectedGridPositions = GridPosition.GetArrayBetween(mouseStartingPosition, currentGridSelection);
            ShowSelection(selectedGridPositions);
        } 
        else
        {
            selectionBoxNodeIndicator.SetActive(true);
            selectionBoxHeightIndicator.SetActive(true);
        }

        cam.transform.position = new Vector3(Mathf.Clamp(cam.transform.position.x, -30f, 30f), Mathf.Clamp(cam.transform.position.y, -20f, 20f), -10f);
    }

    public void MouseLeftClick(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            Debug.Log("Mouse pressed");
            selectionMode = true;
            mouseStartingPosition.AssignValues(currentGridSelection);
        }

        if (ctx.canceled)
        {
            Debug.Log("Mouse released");

            selectionMode = false;

            List<EditorBlock> placedBlocks = new List<EditorBlock>();

            foreach (GridPosition grid in selectedGridPositions)
            {
                placedBlocks.Add(PlaceNewBlock(grid, TileTypes.grassSprite));
            }

            myScene.UpdateScene(placedBlocks.ToArray());
            ClearSelection();
        }
    }

    EditorBlock PlaceNewBlock(GridPosition inputPos, Sprite blockType)
    {
        GameObject editorBlock = Resources.Load<GameObject>("Level Editor/EditorBlock");
        EditorBlock newBlock = GameObject.Instantiate(editorBlock).GetComponent<EditorBlock>();
        newBlock.transform.position = LevelData.GridIndexToTransformPos(inputPos);
        newBlock.gridPosition = inputPos;
        newBlock.spriteRenderer.sprite = TileTypes.grassSprite;
        newBlock.transform.position = new Vector3(newBlock.transform.position.x, newBlock.transform.position.y, newBlock.GetBlockSortingIndex());

        return newBlock;
    }

    void ShowSelection(GridPosition[] selection)
    {
        int boxesAvailable = selectionDisplay.Count;
        int boxesNeeded = selection.Length - boxesAvailable;

        for (int i = boxesNeeded; i > 0; i--)
        {
            GameObject newDisplay = GameObject.Instantiate(selectionBoxHeightIndicator);
            selectionDisplay.Add(newDisplay);
        }

        int counter = 0;

        foreach(GridPosition grid in selection)
        {
            selectionDisplay[counter].SetActive(true);
            selectionDisplay[counter].transform.position = grid.GetTransformPosition();
            counter++;
        }

        for (int i = counter; i < selectionDisplay.Count; i++)
        {
            selectionDisplay[i].SetActive(false);
        }
    }

    void ClearSelection()
    {
        for (int i = 0; i < selectionDisplay.Count; i++)
        {
            Destroy(selectionDisplay[i]);
        }

        selectionDisplay = new List<GameObject>();
    }

    public void Move(InputAction.CallbackContext ctx)
    {
        movement = ctx.ReadValue<Vector2>();
    }

    public void Zoom(InputAction.CallbackContext ctx)
    {
        zoom += ctx.ReadValue<float>() * -Time.fixedDeltaTime * 0.5f;
        zoom = Mathf.Clamp(zoom, 2f, 18f);
    }

    public void MousePos(InputAction.CallbackContext ctx)
    {
        mousePos = ctx.ReadValue<Vector2>();
    }

    public void MouseDelta(InputAction.CallbackContext ctx)
    {
        mouseDelta = -ctx.ReadValue<Vector2>();
    }

    public void MiddleClickForPan(InputAction.CallbackContext ctx)
    {
        if (ctx.canceled)
        {
            cameraPan = false;
        }
        else
        {
            cameraPan = true;
        }
    }

    public void UpLevel(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            heightSelection++;
            heightSelection = Mathf.Clamp(heightSelection, 0, 8);
        }
    }

    public void DownLevel(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            heightSelection--;
            heightSelection = Mathf.Clamp(heightSelection, 0, 8);
        }
    }
}