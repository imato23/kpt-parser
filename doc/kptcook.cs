class LocalizedString {
    en: string
    de: string
    es: string
    fr: string
    pt: string
}

class Unit {
    de: LocalizedUnit
    en: LocalizedUnit
}

class LocalizedUnit{
    quantity: int
    measure: string
}

class StepIngredient {
    unit: Unit
    ingredientId: string
    title: LocalizedString
}

class Step {
    title: LocalizedString
    ingredients: List<StepIngredient>
}

class Image {
    name: string
    url: string
    type: string
}

class Date {
    date: DateTime
}

class Author {
    _id: string
    name: string
    link: string
    title: string
    localizedDesc: LocalizedString
    instagram: string
    sponsor: string
    authorImage: Image
    creationDate: Date
    updateDate: Date
}

class Retailer {
    _id: string
    rkey: string
    key: string
    name: string
    status: string
    typ: string
    country: string
    priceUpdate: string
    mapStatus: string
    onlineOrderingState: bool
    creationDate: Date
    updateDate: Date
}

class RecipeIngredient {
    _id: string
    quantity: int
    measure: string
    quantityUS: double
    measureUS: string
    quantityUSProd: int
    measureUSProd: string
    ingredient: RecipeIngredientDetails
}

class RecipeIngredientDetails{
    _id: string
    typ: string
    localizedTitle: LocalizedString
    numberTitle: NumberTitle
    uncountableTitle: LocalizedString
    category: string
    key: string
    desc: string
    image: Image
    products: List<Product>
    isSponsored: bool
    measures: Measure
    synonym: Synonym
    creationDate: Date
    updateDate: Date
}

class Measure
{
    de: List<string>
    us: List<string>
}

class Synonym
{
    en: string
    de: string
}

class NumberTitle{
    singular: LocalizedString
    plural: LocalizedString
}

class PublishDate {
    en: DateTime
    de: DateTime
}

class PublishDuration {
    en: int
    de: int
}

class Recipe {
    _id: string
    localizedTitle: LocalizedString
    rtype: string
    gdocs: string
    authorComment: LocalizedString
    uid: string
    country: string
    preparationTime: int
    cookingTime: int
    recipeNutrition: RecipeNutrition
    steps: List<Step>
    authors: List<Author>
    retailers: List<Retailer>
    ingredients: List<RecipeIngredient>
    imageList: List<Image>
    localizedPublishDate: LocalizedDate
    publishDates: LocalizedDate
    trackingMode: string
    isStandRecipe: string
    publishDuration: PublishDuration
    ingredientTags: string
    favoriteCount: int
    creationDate: Date
    updateDate: Date
}

class RecipeNutrition {
    calories: int
    protein: int
    fat: int
    carbohydrate: int
}