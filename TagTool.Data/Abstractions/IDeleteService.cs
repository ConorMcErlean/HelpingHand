using System.Threading.Tasks;

namespace TagTool.Data.Services
{
    public interface IDeleteService
    {
        Task CleanRecordsASync();
    }
}