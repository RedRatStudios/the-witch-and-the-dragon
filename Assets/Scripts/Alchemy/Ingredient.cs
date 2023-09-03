using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This class is contained in every "ingredient" gameobject, it contains all the info
/// we may need from it.
/// Removed and migrated from Alchemy.cs
/// </summary>
public class Ingredient : MonoBehaviour
{
    public string ingredientName;
    public Dictionary<string, string[]> combinations;
}

