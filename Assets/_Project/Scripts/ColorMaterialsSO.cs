using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ColorMaterials", menuName = "ColorMaterials")]
public class ColorMaterialsSO : ScriptableObject
{
    public List<Material> materials;
}