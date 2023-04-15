namespace MarketPlace.API.Data.Enums
{
    public enum ReturningStatusEnum
    {
        Processing = 101,
        Authorized = 102,
        BlockAmount = 103,
        ReceivedProdut = 104,
        ResendProduct = 105,
        Delivered = 106,
        RefundComplete = 201,
        ExchangeComplete = 202,
        PartiallyRejected = 301,
        FullyRejected = 302,
        Cancelled = 401,
    }
    static class ReturningStatusEnumMethods
    {
        public static bool CheckCanChangeToNewStatus(ReturningStatusEnum oldStatus, ReturningStatusEnum newStatus, UserGroupEnum rule)
        {
            switch (oldStatus)
            {
                case ReturningStatusEnum.Processing:
                    if ((newStatus == ReturningStatusEnum.PartiallyRejected && rule == UserGroupEnum.Seller) ||
                        (newStatus == ReturningStatusEnum.Processing) || (newStatus == ReturningStatusEnum.Authorized))
                    {
                        return true;
                    }
                    return false;
                case ReturningStatusEnum.Authorized:
                    if ((rule == UserGroupEnum.Admin && newStatus == ReturningStatusEnum.BlockAmount) || (newStatus == ReturningStatusEnum.Authorized))
                    {
                        return true;
                    }
                    return false;
                case ReturningStatusEnum.BlockAmount:
                    if ((newStatus == ReturningStatusEnum.ReceivedProdut) || (newStatus == ReturningStatusEnum.BlockAmount))
                    {
                        return true;
                    }
                    return false;
                case ReturningStatusEnum.ReceivedProdut:
                    if ((rule == UserGroupEnum.Admin && (newStatus == ReturningStatusEnum.RefundComplete)) ||
                     (newStatus == ReturningStatusEnum.ResendProduct) || (newStatus == ReturningStatusEnum.ReceivedProdut))
                    {
                        return true;
                    }
                    return false;
                case ReturningStatusEnum.ResendProduct:
                    if ((newStatus == ReturningStatusEnum.Delivered) || (newStatus == ReturningStatusEnum.ResendProduct))
                    {
                        return true;
                    }
                    return false;
                case ReturningStatusEnum.Delivered:
                    if ((rule == UserGroupEnum.Admin && (newStatus == ReturningStatusEnum.ExchangeComplete)) ||
                      (newStatus == ReturningStatusEnum.Delivered))
                    {
                        return true;
                    }
                    return false;
                case ReturningStatusEnum.RefundComplete:
                    if ((newStatus == ReturningStatusEnum.RefundComplete))
                    {
                        return true;
                    }
                    return false;
                case ReturningStatusEnum.ExchangeComplete:
                    if ((newStatus == ReturningStatusEnum.ExchangeComplete))
                    {
                        return true;
                    }
                    return false;
                case ReturningStatusEnum.PartiallyRejected:
                    if ((rule == UserGroupEnum.Admin && (newStatus == ReturningStatusEnum.FullyRejected )) ||
                        (newStatus == ReturningStatusEnum.PartiallyRejected)||(newStatus == ReturningStatusEnum.Authorized))
                    {
                        return true;
                    }
                    return false;
                case ReturningStatusEnum.FullyRejected:
                    if ((newStatus == ReturningStatusEnum.FullyRejected))
                    {
                        return true;
                    }
                    return false;
                case ReturningStatusEnum.Cancelled:
                    if ((newStatus == ReturningStatusEnum.Cancelled))
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