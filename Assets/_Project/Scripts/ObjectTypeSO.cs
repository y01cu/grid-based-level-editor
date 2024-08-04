using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ObjectType")]
public class ObjectTypeSO : ScriptableObject
{
    public new string name;
    public Transform prefab;
    public Sprite spriteForLevelEditor;
    public Direction direction;
    public List<Material> normalMaterials;
    public Material obstacleMaterial;
    public CellObject cellObjectType;
    public int materialIndex;
}