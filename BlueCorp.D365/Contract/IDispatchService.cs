using BlueCorp.Common;

namespace BlueCorp.D365.Contract
{
    public interface IDispatchService
    {
        Task<(bool,string)> ProcessAsync(PayLoad payload);
    }
}