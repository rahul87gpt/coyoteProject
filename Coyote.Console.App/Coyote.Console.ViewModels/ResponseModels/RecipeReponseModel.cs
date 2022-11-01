using Coyote.Console.ViewModels.RequestModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class RecipeSPReponseModel
    {
        public int ID { get; set; }
        public long ProductID { get; set; }
        public long ProductNumber { get; set; }
        public string ProductDescription { get; set; }
        public int? OutletID { get; set; }
        public string OutletCode { get; set; }
        public string OutletDesc { get; set; }
        public string Description { get; set; }
        public float Qty { get; set; }
        public long? IngredientProductID { get; set; }
        public long? IngredientNumber { get; set; }
        public string IngredientDescription { get; set; }
        public int IsParents { get; set; }
    }


    public class RecipeReponseModel
    {
        public RecipeHeaderResponseModel RecipeHeader { get; set; }
        public List<RecipeDetailResponseModel> RecipeDetail { get; set; } = new List<RecipeDetailResponseModel>();
    }

    public class RecipeHeaderResponseModel : RecipeHeaderRequestModel
    {
        public int ID { get; set; }
        public long ProductNumber { get; set; }
        public string ProductDescription { get; set; }
        public string OutletCode { get; set; }
        public string OutletDesc { get; set; }
       // public string RecipeDescription { get; set; }
    }
    public class RecipeDetailResponseModel : RecipeDetailRequestModel
    {
        public int ID { get; set; }
        public long? IngredientNumber { get; set; }
        public string IngredientDescription { get; set; }
    }

}
