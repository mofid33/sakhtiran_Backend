namespace MarketPlace.API.Data.Enums
{
    public enum UserGroupEnum
    {
        Admin, //Admin
        Seller, //Shop
        Customer
    }

    static class UserGroupEnumMethods
    {
        public static string GetUserGroupEnum(string id)
        {
            switch (id.ToLower())
            {
                case GroupTypes.Admin:
                    return UserGroupEnum.Admin.ToString();
                case GroupTypes.Seller:
                    return UserGroupEnum.Seller.ToString();
                case GroupTypes.Customer:
                    return UserGroupEnum.Customer.ToString();
                default:
                    return UserGroupEnum.Customer.ToString();
            }
        }

        public static UserGroupEnum GetUserGroupRole(string groupName)
        {
            
                if( UserGroupEnum.Admin.ToString() == groupName)
                {
                    return UserGroupEnum.Admin;
                }
                else if(UserGroupEnum.Seller.ToString() == groupName)
                {
                    return UserGroupEnum.Seller;
                }
                else if(UserGroupEnum.Customer.ToString() == groupName)
                {
                    return UserGroupEnum.Customer;
                }
                else
                {
                    return UserGroupEnum.Customer;
                }
        }
    }
    static class GroupTypes
    {
        public const string Admin = "aecfce36-71f2-ea11-8164-3cd92b6a6e7b"; //Admin
        public const string Seller = "b0cfce36-71f2-ea11-8164-3cd92b6a6e7b"; //Shop
        public const string Customer = "afcfce36-71f2-ea11-8164-3cd92b6a6e7b"; //customer
    }

    static class UserAdminId
    {
        public const string ID = "31fbd9d9-71f2-ea11-8164-3cd92b6a6e7b"; //Admin

    }
}