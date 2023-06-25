using System.Security.Cryptography;

namespace Imato.KptParser.Mealie.DomainModel;

public class Recipe
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public string GroupId { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public string Image { get; set; }
    public string RecipeYield { get; set; }
    public string TotalTime { get; set; }
    public string PrepTime { get; set; }
    public string CookTime { get; set; }
    public string PerformTime { get; set; }
    public string Description { get; set; }
    public IEnumerable<RecipeCategory> RecipeCategories { get; set; }
    public IEnumerable<Tag> Tags { get; set; }
    public IEnumerable<string>? Tools { get; set; }
    public int? Rating { get; set; }
    public string OrgUrl { get; set; }
    public DateTime? DateAdded { get; set; }
    public DateTime? DateUpdated { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdateAt { get; set; }
    public DateTime? LastMade { get; set; }

    /*
"id": "a4e7dece-91ca-47b4-ae70-25d630c99ce0",
"userId": "000d70c6-cd03-468a-b0d5-59201cfd4640",
"groupId": "7b1942d7-5be7-476b-a948-ab163bd8cc28",
"name": "Klassischer Mojito",
"slug": "klassischer-mojito",
"image": "QQIJ",
"recipeYield": "2 Portionen ",
"totalTime": "5 Minutes",
"prepTime": "none",
"cookTime": null,
"performTime": "5 Minutes",
"description": "Der Mojito gehört zu den beliebtesten Cocktail-Klassikern. Herrlich erfrischend bringt er direkt kubanisches Feeling ins Glas!",
"recipeCategory": [
{
    "id": "1a866996-7f74-4d9b-a5a3-6d7f9a05dcc4",
    "name": "Getränke",
    "slug": "getranke"
}
],
"tags": [],
"tools": [],
"rating": 4,
"orgURL": "https://www.lecker.de/klassischer-mojito-21671.html",
"dateAdded": "2023-06-11",
"dateUpdated": "2023-06-15T07:24:02.678214",
"createdAt": "2023-06-11T20:11:35.519278",
"updateAt": "2023-06-15T07:24:02.680504",
"lastMade": "2023-06-14T21:59:59"
    */
}

public class RecipeCategory
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
}

public class Tag
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
}