using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.IMESports.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum IMESportsGameApiError
    {
        Success = 0,
        PlayerAlreadyExist = 503,
        PlayerDoesNotExist = 504,
        InvalidPlayerId = 506,
        InvalidCurrency = 507,
        InsufficientAmount = 510,
        InvalidToken = 531,
        GameNotActive = 533,
        PlayerIsInactive = 542,
        TransactionIdNotFound = 545,
        GameNotExist = 577,
        ConcurrentRequestDetected = 579,
        InvalidArguments = 612,
        SystemError = 999,
    }
}
