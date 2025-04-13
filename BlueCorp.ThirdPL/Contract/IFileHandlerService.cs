using BlueCorp.Common;

namespace BlueCorp.ThirdPL.Contract
{
    public interface IFileHandlerService
    {
        bool CheckAuthorized(string apiKey, string ip);
        Task<bool> UploadToInComingFolder(PayLoad payLoad);
        void MoveFiles();
    }
}
