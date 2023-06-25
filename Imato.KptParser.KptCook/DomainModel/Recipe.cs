using System.Text.Json.Serialization;

namespace Imato.KptCookImporter.KptCook.DomainModel;

public class CptCookModel
{
    [JsonPropertyName("_id")] 
    public Id Id { get; set; }
}

public class Recipe : CptCookModel
{
    public LocalizedSstring LocalizedTitle { get; set; }
    public string RType { get; set; }
    public LocalizedSstring AuthorComment { get; set; }
    public string Country { get; set; }
    public int PreparationTime { get; set; }
    public int CookingTime { get; set; }
    public RecipeNutrition RecipeNutrition { get; set; }
    public string[] StepsDE { get; set; }
    public IEnumerable<Author> Authors { get; set; }
    public IEnumerable<IngredientInfo> Ingredients { get; set; }
    public IEnumerable<Image> ImageList { get; set; }
}

public class Id
{
    [JsonPropertyName("$oid")] public string Oid { get; set; }
}

public class LocalizedSstring
{
    public string En { get; set; }
    public string De { get; set; }
    public string Es { get; set; }
    public string Fr { get; set; }
    public string Pt { get; set; }
}

public class RecipeNutrition
{
    public int Calories { get; set; }
    public int Protein { get; set; }
    public int Fat { get; set; }
    public int Carbohydrate { get; set; }
}

public class Author : CptCookModel
{
    public string Name { get; set; }
    public string Link { get; set; }
    public string Title { get; set; }
    public LocalizedSstring LocalizedDesc { get; set; }
    public string Sponsor { get; set; }
    public Image AuthorImage { get; set; }

    // CreationDate
    // UpdateDate
}

public class Image
{
    public string Name { get; set; }
    public string Url { get; set; }
    public string Type { get; set; }
}

public class IngredientInfo
{
    public double Quantity { get; set; }
    public string Measure { get; set; }
    public Ingredient Ingredient { get; set; }
}

public class Ingredient : CptCookModel
{
    public string Typ { get; set; }
    public LocalizedSstring LocalizedTitle { get; set; }
    public NumberTitle NumberTitle { get; set; }
    public LocalizedSstring UncountableTitle { get; set; }
    public string Category { get; set; }
    public string Key { get; set; }
    public string Desc { get; set; }

    public Image Image { get; set; }
    // CreationDate
    // UpdateDate
}

public class NumberTitle
{
    public LocalizedSstring Singular { get; set; }
    public LocalizedSstring Plural { get; set; }
}