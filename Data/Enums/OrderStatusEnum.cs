namespace MarketPlace.API.Data.Enums
{
    public enum OrderStatusEnum
    {
        Cart = 0,
        Ongoing = 1,
        Confirmed = 2,
        Processing = 3,
        Shipping = 4,
        Shipped = 5,
        Delivered = 6,
        Completed = 7,
        Cancelled = 100,
        ReturnProcessing = 200,
        ReturnComplete = 201
    }
    static class OrderStatusEnumMethods
    {
        public static bool CheckCanChangeToNewStatus(OrderStatusEnum oldStatus, OrderStatusEnum newStatus, UserGroupEnum rule)
        {
            switch (oldStatus)
            {
                case OrderStatusEnum.Cart:
                    if ((newStatus == OrderStatusEnum.Ongoing && rule == UserGroupEnum.Customer))
                    {
                        return true;
                    }
                    return false;
                case OrderStatusEnum.Ongoing:
                    if ( (newStatus == OrderStatusEnum.Confirmed && (rule == UserGroupEnum.Admin||rule == UserGroupEnum.Seller)))
                    {
                        return true;
                    }
                    return false;
                case OrderStatusEnum.Confirmed:
                    if ((newStatus == OrderStatusEnum.Processing && (rule == UserGroupEnum.Admin||rule == UserGroupEnum.Seller)))
                    {
                        return true;
                    }
                    return false;
                case OrderStatusEnum.Processing:
                    if ((newStatus == OrderStatusEnum.Shipping && (rule == UserGroupEnum.Admin||rule == UserGroupEnum.Seller)))
                    {
                        return true;
                    }
                    return false;
                case OrderStatusEnum.Shipping:
                    if ((newStatus == OrderStatusEnum.Shipped && (rule == UserGroupEnum.Admin||rule == UserGroupEnum.Seller)))
                    {
                        return true;
                    }
                    return false;
                case OrderStatusEnum.Shipped:
                    if ((newStatus == OrderStatusEnum.Delivered && (rule == UserGroupEnum.Admin||rule == UserGroupEnum.Seller)))
                    {
                        return true;
                    }
                    return false;
                case OrderStatusEnum.Delivered:
                    if ((newStatus == OrderStatusEnum.Completed && (rule == UserGroupEnum.Admin)))
                    {
                        return true;
                    }
                    return false;
                default:
                    return false;
            }
        }
    }
}