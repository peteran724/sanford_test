using BlueCorp.ThirdPL.Contract;
using Hangfire;

namespace BlueCorp.ThirdPL.Job
{
    public class FileHandlerJob
    {
        private IFileHandlerService _srv;
        private readonly IRecurringJobManager _jobMgr;
        public FileHandlerJob(IFileHandlerService srv, IRecurringJobManager jobMgr)
        {
            _srv = srv;
            _jobMgr = jobMgr;
        }


        public void ScheduleJobs()
        {
            _jobMgr.AddOrUpdate(
                "move-file-job-each-5-min",
                () => _srv.MoveFiles()
                //,"*/5 * * * *"  
                , "*/1 * * * *" //1 min for test
            );
        }
    }
}
