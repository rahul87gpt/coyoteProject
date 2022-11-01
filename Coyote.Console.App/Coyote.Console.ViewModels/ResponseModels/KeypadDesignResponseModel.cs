using Coyote.Console.ViewModels.RequestModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class KeypadDesignResponseModel : KeypadResponseModel
    {
        public List<KeypadLevelResponseModel> KeypadLevels { get; } = new List<KeypadLevelResponseModel>();
    }
    public class DesignKeypadResponseModel : KeypadRequestModel
    {
        public KeypadLevelsResponseModel Buttons { get; } = new KeypadLevelsResponseModel();
    }
    public class KeypadLevelsResponseModel
    {
        public List<KeypadButtonResponseModel> Levels { get; } = new List<KeypadButtonResponseModel>();
    }
    public class KeypadLevelResponseModel
    {
        public int LevelId { get; set; }
        public int LevelIndex { get; set; }
        public string LevelDesc { get; set; }

        //Button 
        public List<KeypadButtonResponseModel> KeypadButtons { get; } = new List<KeypadButtonResponseModel>();
    }


    public class KeypadButtonResponseModel
    {
        public int Id { get; set; }
        public int Type { get; set; }
        public int ButtonIndex { get; set; }
        public string TypeCode { get; set; }
        public string TypeDesc { get; set; }
        public string ShortDesc { get; set; }
        public string Desc { get; set; }
        public string Color { get; set; }
        public int Size { get; set; }
        public string SizeDesc { get; set; }
        public int PriceLevel { get; set; }
        public int CashierLevel { get; set; }
        public int? ButtonLevelIndex { get; set; }
        public int? LevelId { get; set; }

        public long? ProductId { get; set; }
        public int? CategoryId { get; set; }
        public string AttributesDetails { get; set; }
    }
}
