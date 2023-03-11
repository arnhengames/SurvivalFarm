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
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class LevelEditor : MonoBehaviour
{
    [Header("Editor Settings")]
    [SerializeField] float cameraControlSpeed = 5f;
    [SerializeField] float cameraPanSpeed = 10f;

    //GameObjects
    GameObject selectionBoxNodeIndicator, selectionBoxHeightIndicator;
    Camera cam;
    TMP_Text gridSelectionDisplay;

    //Helper variables
    float zoom = 20f;
    bool cameraPan = false;

    Vector2 movement = Vector2.zero;
    Vector2 mousePos = Vector2.zero;
    Vector2 mouseDelta = Vector2.zero;

    public int startingWidth, startingLength, startingHeight;
    public int wStepsMin, wStepsMax, wStepsCurrent, lStepsCurrent, lStepsMin, lStepsMax;
    TMP_Text top, left, right;

    int sizeWidth, sizeLength, sizeHeight;

    GridPosition mouseGridPosition = new GridPosition(0, 0);
    GridPosition currentGridSelection = new GridPosition(0, 0, 0);
    List<GameObject> selectionDisplay = new List<GameObject>();
    int heightSelection = 0;

    GridPosition mouseStartingPosition;
    GridPosition[] selectedGridPositions;
    bool selectionMode = false;

    public List<EditorBlock> svBlocks = new List<EditorBlock>();
    GameObject svBlockGameObject;
    Transform svBlocksParent;
    int svCounter = 0;
    TMP_Text svCountDisplay;

    private void Start()
    {
        selectionBoxNodeIndicator = GameObject.Find("Selection Block Node Indicator");
        selectionBoxHeightIndicator = GameObject.Find("Selection Block Height Indicator");
        cam = GameObject.Find("MainCamera").GetComponent<Camera>(); 
        gridSelectionDisplay = GameObject.Find("Current Selection").GetComponent<TMP_Text>();
        top = GameObject.Find("Top").GetComponent<TMP_Text>();
        left = GameObject.Find("Left").GetComponent<TMP_Text>();
        right = GameObject.Find("Right").GetComponent<TMP_Text>();

        sizeWidth = startingWidth + 1;
        sizeLength = startingLength + 1;
        sizeHeight = 8;

        LevelData.InitializeSceneMap(sizeWidth, sizeLength, sizeHeight);

        svBlockGameObject = Resources.Load<GameObject>("Prefabs/EditorBlock");
        svBlocksParent = GameObject.Find("Editor Blocks").transform;
        svCountDisplay = GameObject.Find("SVC").GetComponent<TMP_Text>();
    }

    void FixedUpdate()
    {
        top.text = wStepsCurrent.ToString() + ", " + lStepsCurrent.ToString();
        left.text = (wStepsCurrent + startingWidth).ToString() + ", " + lStepsCurrent.ToString();
        right.text = wStepsCurrent.ToString() + ", " + (lStepsCurrent + startingLength).ToString();
        svCountDisplay.text = svCounter.ToString();

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

        cam.transform.position = new Vector3(Mathf.Clamp(cam.transform.position.x, -30f, 30f), Mathf.Clamp(cam.transform.position.y, -30f, 0f), -10f);

        //Selection Control
        mouseGridPosition = LevelData.TransformPosToGridPos(cam.ScreenToWorldPoint(mousePos));
        mouseGridPosition = new GridPosition(mouseGridPosition.w + wStepsCurrent, mouseGridPosition.l + lStepsCurrent);

        //If mousegridposition is on current grid
        currentGridSelection.AssignValues(mouseGridPosition, heightSelection);
        currentGridSelection = new GridPosition(Mathf.Clamp(currentGridSelection.w, wStepsCurrent, (wStepsCurrent + startingWidth)), Mathf.Clamp(currentGridSelection.l, lStepsCurrent, (lStepsCurrent + startingLength)), currentGridSelection.h);

        gridSelectionDisplay.gameObject.SetActive(true);
        gridSelectionDisplay.text = "Node Selection: " + currentGridSelection.GetValueString();

        selectionBoxNodeIndicator.transform.position = LevelData.GridIndexToTransformPos(currentGridSelection.w - wStepsCurrent, currentGridSelection.l - lStepsCurrent, 0);
        selectionBoxHeightIndicator.transform.position = LevelData.GridIndexToTransformPos(currentGridSelection.w - wStepsCurrent, currentGridSelection.l - lStepsCurrent, heightSelection);

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

        if (!GridPosition.IsWithin(mouseGridPosition, wStepsCurrent, (wStepsCurrent + startingWidth), lStepsCurrent, (lStepsCurrent + startingLength)))
        {
            selectionBoxNodeIndicator.SetActive(false);
            selectionBoxHeightIndicator.SetActive(false);
        }
    }

    public void MouseLeftClick(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            if (GridPosition.IsWithin(mouseGridPosition, wStepsCurrent, (wStepsCurrent + startingWidth), lStepsCurrent, (lStepsCurrent + startingLength)))
            {
                selectionMode = true;
                mouseStartingPosition.AssignValues(currentGridSelection);
            }
        }

        if (ctx.canceled)
        {
            if (selectionMode)
            {
                selectionMode = false;

                UpdateScene(selectedGridPositions);

                ClearSelection();
            }
        }
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
            selectionDisplay[counter].transform.position = new GridPosition(grid.w - wStepsCurrent, grid.l - lStepsCurrent, grid.h).GetTransformPosition();
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

    private void ResetScene()
    {
        ResetSVBlocks();

        int startingW = wStepsCurrent - wStepsMin;
        int endingW = startingW + startingWidth;

        int startingL = lStepsCurrent - lStepsMin;
        int endingL = startingL + startingLength;

        for (int i = startingW; i < endingW; i++)
        {
            for (int j = startingL; j < endingL; j++)
            {
                for (int k = 0; k < 8; k++)
                {
                    if (LevelData.sceneMap[i, k, k] == 1)
                    {
                        UseSVBlock(TileSprites.grassRidge[0], new GridPosition(i - startingW, j - startingL, k));
                    }
                }
            }
        }
    }

    public void UpdateScene(GridPosition[] gridPositions)
    {
        foreach (GridPosition grid in gridPositions)
        {
            LevelData.sceneMap[grid.w - wStepsMin, grid.l - lStepsMin, grid.h] = 1;
            UseSVBlock(TileSprites.grassRidge[0], new GridPosition(grid.w - wStepsCurrent, grid.l - lStepsCurrent, grid.h));
        }
    }

    public void UseSVBlock(Sprite toDisplay, GridPosition gridPos)
    {
        EditorBlock newBlock;

        if (svCounter >= svBlocks.Count)
        {
            newBlock = Instantiate(svBlockGameObject, svBlocksParent).GetComponent<EditorBlock>();
            svBlocks.Add(newBlock);
            newBlock.Initialize(toDisplay, gridPos);
            svCounter++;
        } 
        else
        {
            svBlocks[svCounter].gameObject.SetActive(true);
            svBlocks[svCounter].Initialize(toDisplay, gridPos);
            svCounter++;
        }
    }

    public void ResetSVBlocks()
    {
        svCounter--;

        for (; svCounter >= 0; svCounter--)
        {
            svBlocks[svCounter].TurnOff();
        }

        svCounter = 0;
    }

    public void UpdateMapSize(int widthChange, int lengthChange)
    {
        bool needUpdate = false;

        if (wStepsCurrent < wStepsMin)
        {
            wStepsMin = wStepsCurrent;
            needUpdate = true;
        }

        if (wStepsCurrent > wStepsMax)
        {
            wStepsMax = wStepsCurrent;
            needUpdate = true;
        }

        if (lStepsCurrent < lStepsMin)
        {
            lStepsMin = lStepsCurrent;
            needUpdate = true;
        }

        if (lStepsCurrent > lStepsMax)
        {
            lStepsMax = lStepsCurrent;
            needUpdate = true;
        }

        if (needUpdate)
        { 
            int oldWidth = sizeWidth;
            int oldLength = sizeLength;

            sizeWidth += Mathf.Abs(widthChange);
            sizeLength += Mathf.Abs(lengthChange);

            int[,,] tempMap = new int[sizeWidth, sizeLength, sizeHeight];

            if ((widthChange < 0) || (lengthChange < 0))
            {
                for (int i = 0; i < oldWidth; i++)
                {
                    for (int j = 0; j < oldLength; j++)
                    {
                        for (int k = 0; k < sizeHeight; k++)
                        {
                            tempMap[i + Mathf.Abs(widthChange), j + Mathf.Abs(lengthChange), k] = LevelData.sceneMap[i, j, k];
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < oldWidth; i++)
                {
                    for (int j = 0; j < oldLength; j++)
                    {
                        for (int k = 0; k < sizeHeight; k++)
                        {
                            tempMap[i, j, k] = LevelData.sceneMap[i, j, k];
                        }
                    }
                }
            }

            LevelData.sceneMap = tempMap;
        }
    }

    public void Move(InputAction.CallbackContext ctx)
    {
        movement = ctx.ReadValue<Vector2>();
    }

    public void Zoom(InputAction.CallbackContext ctx)
    {
        zoom += ctx.ReadValue<float>() * -Time.fixedDeltaTime * 0.5f;
        zoom = Mathf.Clamp(zoom, 2f, 20f);
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
            heightSelection = Mathf.Clamp(heightSelection, 1, startingHeight);
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

    public void StepWidth(int input)
    {
        wStepsCurrent += input;
        UpdateMapSize(input, 0);
        ResetScene();
    }

    public void StepLength(int input)
    {
        lStepsCurrent += input;
        UpdateMapSize(0, input);
        ResetScene();
    }
}