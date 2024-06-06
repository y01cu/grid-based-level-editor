using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using QFSW.QC;
using UnityEngine;
using UnityEngine.Serialization;
using VHierarchy.Libs;

public class GridBase : MonoBehaviour
{
    // Attention! Everytime a grid layer is set the position of the next layer must increase for it to be placed on previous one.

    [SerializeField] private int width;
    [SerializeField] private int height;

    [SerializeField] private CellBase[] cellBases;

    [SerializeField] private Transform cellParentTransform;

    private int _activeGridLayerCount;
    private const float LayerZAxisIncrement = -0.1f;

    private const float XAxisAngle = -90;

    private void Start()
    {
        
        GenerateCellObjects(CellBase.ObjectColor._Empty, 4, 0, 0, 0, 0);
        GenerateCellObjects(CellBase.ObjectColor.Blue, 1, 1, 90,0,0);
        GenerateCellObjects(CellBase.ObjectColor.Red, 2, 1, 90,0,0);
        GenerateCellObjects(CellBase.ObjectColor.Green, 2, 2, 90,0,0);

    }

    private void GenerateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var cell = Instantiate(cellBases[0], new Vector3(x, y), Quaternion.Euler(XAxisAngle, 0, 0));
                cell.name = $"Cell: {x} {y}";
            }
        }
    }


    [Command]
    private async void GenerateCellObjects(CellBase.ObjectColor objectColor, int columnIndex, int layerIndex, float rotX, float rotY, float rotZ)
    {
        GameObject layerParentObject = new GameObject("L_" + layerIndex + "-C_" + objectColor);

        layerParentObject.transform.SetParent(cellParentTransform);

        if (columnIndex == width)
        {
            Debug.Log("column index = width");
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var targetPosition = new Vector3(x, y, layerIndex * LayerZAxisIncrement);
                    var targetRotation = Quaternion.Euler(XAxisAngle, 0, 0);
                    var cellBase = Instantiate(cellBases[(int)objectColor], targetPosition, targetRotation);
                    cellBase.transform.SetParent(layerParentObject.transform);

                    // Test comment out -----

                    // var cellObject = cellBase.CreateCellObjectOfTypeInTransformOnParent(CellBase.ObjectType.Frog, new Vector3(x, y, -0.15f), Quaternion.Euler(rotX, rotY, rotZ));
                    //

                    // -----

                    // cellObject.transform.SetParent(transform);
                }
            }
        }

        else
        {
            int x = columnIndex;

            for (int y = 0; y < height; y++)
            {
                var targetPosition = new Vector3(x, y, layerIndex * LayerZAxisIncrement);
                var targetRotation = Quaternion.Euler(XAxisAngle, 0, 0);
                var cellBase = Instantiate(cellBases[(int)objectColor], targetPosition, targetRotation, layerParentObject.transform);
                
                // cellBase.CreateCellObject();

                await Task.Delay(100);
            }
        }

        _activeGridLayerCount++;
    }


    [Command]
    private void DeleteTopLayer()
    {
        Destroy(cellParentTransform.GetChild(cellParentTransform.childCount - 1).gameObject);

        _activeGridLayerCount--;
    }
}