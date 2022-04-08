namespace Tgs.GameProviders.Core.Saba.Models
{
    public enum SabaCreateMemberApiError
    {
        Ok = 0,
        Fail = 1,
        DuplicatedUsername = 2,
        WrongOperatorId = 3,
        WrongOddsType = 4,
        InvalidCurrency = 5,
        DuplicatedVendorMemberId = 6,
        MinTransferGreaterThanMaxTransfer = 7,
        InvalidArgument = 8,
        InvalidVendorId = 9,
        SystemUnderMaintenance = 10,
        MinTransferLessThanOne = 11,
        IdExceedLengthLimit = 12
    }
}
