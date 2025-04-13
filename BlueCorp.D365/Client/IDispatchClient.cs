using BlueCorp.Common;

namespace BlueCorp.D365.Client;

public interface IDispatchClient
{
    Task<HttpResponseMessage> UploadJsonFileTo3PL(PayLoad reqPayLoad);
}