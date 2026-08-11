using OnlineConsulting.BusinessLogic.Abstractions.IStorageServices.IBaseStorages;

namespace OnlineConsulting.BusinessLogic.Abstractions.IStorageServices.IStorages;

public interface IStorageService : IBaseStorage
{
    string StorageName { get; }
}
