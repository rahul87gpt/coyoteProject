using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.RequestModels
{
    public class RecipeRequestModel
    {
        public RecipeHeaderRequestModel RecipeHeader { get; set; }
        public List<RecipeDetailRequestModel> RecipeDetail { get; set; }
    }

    public class RecipeHeaderRequestModel
    {
        public long ProductID { get; set; }
        public string Description { get; set; }
        public int? OutletID { get; set; }
        public float Qty { get; set; }
    }
    public class RecipeDetailRequestModel
    {
        public long ProductID { get; set; }
        public long IngredientProductID { get; set; }
        public float Qty { get; set; }

    }

    public class RecipeSPRequestModel
    {
        public long ProductID { get; set; }
        public int? OutletID { get; set; }
        public long? IngredientProductID { get; set; }
       // public DateTime RecipeTimeStamp { get; set; }
        public string Description { get; set; }
        public float Qty { get; set; }
       
        public int IsParents { get; set; }
    }
}
