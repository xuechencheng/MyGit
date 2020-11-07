using UnityEngine;

public class Recipe : MonoBehaviour
{
    public Ingredient potionResult;
    public Ingredient[] potionIngredients;
    [MyRangeAttribute(1.5f,12.4f)]
    public int number;
}