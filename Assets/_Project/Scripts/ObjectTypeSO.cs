using System;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "ScriptableObjects/ObjectType")]
public class ObjectTypeSO : ScriptableObject
{
    public new string name;
    public Transform prefab;
    public Direction direction;
    public Material normalMaterial;
    public Material obstacleMaterial;
    public CellObject cellObjectType;
}