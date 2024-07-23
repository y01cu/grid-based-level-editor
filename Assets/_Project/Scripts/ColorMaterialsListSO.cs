using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ColorMaterialsList", menuName = "ColorMaterialsList")]
public class ColorMaterialsListSO : ScriptableObject
{
    public List<ColorMaterialsSO> colorMaterials;
    
}