namespace BlueCorp.D365.DataRepository
{
    public interface IPayLoadRepo
    {
        void Insert(int controlNum, string payLaod);

        int SelectMaxControlNum();

        (int, string) SelectByPayLoad(string payload);
    }
}
