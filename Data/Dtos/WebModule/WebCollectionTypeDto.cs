using System;
using System.Collections.Generic;
using MarketPlace.API.Data.Models;

namespace MarketPlace.API.Data.Dtos.WebModule
{
    public partial class WebCollectionTypeDto 
    {
        public WebCollectionTypeDto()
        {
        }

        public WebCollectionTypeDto(WebCollectionType type)
        {
            CollectionTypeId = type.CollectionTypeId;
            CollectionTypeTitle = type.CollectionTypeTitle;
            ForCustomer = type.ForCustomer;
            SelectCategory = type.SelectCategory;
            SelectGoods = type.SelectGoods;
            SelectSpecialSale = type.SelectSpecialSale;
        }

        public int CollectionTypeId { get; set; }
        public string CollectionTypeTitle { get; set; }
        public bool ForCustomer { get; set; }
        public bool SelectCategory { get; set; }
        public bool SelectGoods { get; set; }
        public bool SelectSpecialSale { get; set; }
    }
}
