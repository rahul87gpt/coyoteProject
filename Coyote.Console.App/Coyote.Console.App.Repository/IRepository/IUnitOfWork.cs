using System.Threading.Tasks;

namespace Coyote.Console.App.Repository.IRepository
{
    public interface IUnitOfWork
    {
        //IAPNRepository APNRepository { get; }
        //ICommodityRepository CommodityRepository { get; }
        //IDepartmentRepository DepartmentRepository { get; }
        //IEmailTemplateRepository EmailTemplateRepository { get; }
        //ILoginRepository LoginRepository { get; }
        //IMasterListRepository MasterListRepository { get; }
        //IMasterListItemRepository MasterListItemRepository { get; }
        //IOutletProductRepository OutletProductRepository { get; }
        //IProductRepository ProductRepository { get; }
        //IRoleRepository RoleRepository { get; }
        //ISendEmailRepository SendEmailRepository { get; }
        //IStoreGroupRepository StoreGroupRepository { get; }
        //IStoreRepository StoreRepository { get; }
        //ISupplierProductRepository SuppplierProductRepository { get; }
        //ISupplierRepository SuppplierRepository { get; }
        //ITaxRepository TaxRepository { get; }
        //IUserRepository UserRepository { get; }
        //IUserRoleRepository UserRoleRepository { get; }
        //IWarehouseRepositry WarehouseRepositry { get; }
        //IZoneOutletRepository ZoneOutletRepository { get; }

        //int Commit();
        //Task<int> CommitAsync();
        //Task RollbackAsync();
        //void Rollback();

        IGenericRepository<T> GetRepository<T>() where T : class;
        bool SaveChanges();
        Task<bool> SaveChangesAsync();
        void Dispose();
    }
}
