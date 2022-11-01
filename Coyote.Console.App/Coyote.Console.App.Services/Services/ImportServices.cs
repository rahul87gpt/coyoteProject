using Coyote.Console.App.Models;
using Coyote.Console.App.Repository.IRepository;
using Coyote.Console.App.Services.Helper;
using Coyote.Console.App.Services.IServices;
using Coyote.Console.Common;
using Coyote.Console.Common.Exceptions;
using Coyote.Console.Common.Services;
using Coyote.Console.ViewModels.ImportModels;
using Coyote.Console.ViewModels.RequestModels;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Coyote.Console.App.Services.Services
{
    public class ImportServices : IImportServices
    {
        private IUnitOfWork _unitOfWork = null;
        private ILoggerManager _iLoggerManager = null;

        public ImportServices(IUnitOfWork repo, ILoggerManager iLoggerManager)
        {
            _unitOfWork = repo;
            _iLoggerManager = iLoggerManager;
        }


        public async Task<bool> ImportCSVToTable(ImportFilterRequestModel reportRequest, ExportFilterRequestModel tableFilter, string path, int userId)
        {
            try
            {
                if (reportRequest != null)
                {
                    if (string.IsNullOrEmpty(reportRequest?.ImportPassword))
                    {
                        throw new BadRequestException(ErrorMessages.InValidPassword.ToString(CultureInfo.CurrentCulture));
                    }

                    if (reportRequest.ImportPassword != "NOW" && reportRequest.ImportPassword != "ALLCODES" && reportRequest.ImportPassword != "EPAY" && reportRequest.ImportPassword != "TOUCH" && reportRequest.ImportPassword != "EPAYV2")
                    {
                        throw new BadRequestException(ErrorMessages.InValidPassword.ToString(CultureInfo.CurrentCulture));
                    }


                    var repository = _unitOfWork?.GetRepository<Store>();
                    var filePath = Directory.GetCurrentDirectory() + path;

                    var lineCount = File.ReadLines(filePath).Count();

                    var dataTble = new DataTable();
                    var parameterName = "";
                    var typeName = "";
                    var storedProc = "";

                    if (tableFilter != null)
                    {
                        if (tableFilter.Stores)
                        {
                            dataTble = CSVReader.ConvertCSVtoDataTable<StoreImportModel>(filePath, true);
                            parameterName = "@StoreRequestModelDataTable";
                            typeName = "[dbo].[StoreImportRequestType]";
                            storedProc = StoredProcedures.ImportStoreDataToTable;
                        }
                        if (tableFilter.StoreGroups)
                        {
                            dataTble = CSVReader.ConvertCSVtoDataTable<StoreGroupImportModel>(filePath, true);
                            parameterName = "@StoreGroupRequestDataTable";
                            typeName = "[dbo].[StoreGroupImportRequestType]";
                            storedProc = StoredProcedures.ImportStoreGroupDataToTable;
                        }
                        if (tableFilter.Suppliers)
                        {
                            dataTble = CSVReader.ConvertCSVtoDataTable<SupplierImportModel>(filePath, true);
                            parameterName = "@StoreGroupRequestDataTable";
                            typeName = "[dbo].[SupplierImportRequestType]";
                            storedProc = StoredProcedures.ImportSupplierToTable;
                        }

                        if (tableFilter.SupplierProduct)
                        {
                            dataTble = CSVReader.ConvertCSVtoDataTable<SupplierProductImportModel>(filePath, true);
                            parameterName = "@SupplierProductRequestTable";
                            typeName = "[dbo].[SupplierProductImportRequestType]";
                            storedProc = StoredProcedures.ImportSupplierProductToTable;
                        };
                        if (tableFilter.Tills)
                        {
                            dataTble = CSVReader.ConvertCSVtoDataTable<TillImportModel>(filePath, true);
                            parameterName = "@TillRequestTable";
                            typeName = "[dbo].[TillImportRequestType]";
                            storedProc = StoredProcedures.ImportTilltToTable;
                        }
                        if (tableFilter.APN)
                        {
                            dataTble = CSVReader.ConvertCSVtoDataTable<APNImportModel>(filePath, true);
                            parameterName = "@APNRequestTable";
                            typeName = "[APNImportRequestType]";
                            storedProc = StoredProcedures.ImportAPNtToTable;
                        }
                        if (tableFilter.OutletProduct)
                        {
                            dataTble = CSVReader.ConvertCSVtoDataTable<OutletProductImportModel>(filePath, true);
                            parameterName = "@OutletProductRequestTable";
                            typeName = "[OutletProductImportRequestType]";
                            storedProc = StoredProcedures.ImportOutletProductToTable;
                        }
                        if (tableFilter.OutletSupplier)
                        {
                            dataTble = CSVReader.ConvertCSVtoDataTable<OutletProductImportModel>(filePath, true);
                            parameterName = "";
                            typeName = "";
                            storedProc = StoredProcedures.ImportOutletProductToTable;
                        }
                        if (tableFilter.Users)
                        {
                            dataTble = CSVReader.ConvertCSVtoDataTable<UserImportModel>(filePath, true, true);
                            parameterName = "@UserRequestTable";
                            typeName = "[UserImportRequestType]";
                            storedProc = StoredProcedures.ImportUserToTable;
                            /*Add encryption for password*/
                        }
                        if (tableFilter.Category || tableFilter.CostZones || tableFilter.SubRanges || tableFilter.PriceZones || tableFilter.Manufacturers || tableFilter.HostSettings || tableFilter.GeneralZones || tableFilter.CashierTypes)
                        {
                            dataTble = CSVReader.ConvertCSVtoDataTable<MasterListItemsImportModel>(filePath, true);
                            parameterName = "@MasterRequestTable";
                            typeName = "[MasterListItemsImportRequestType]";
                            storedProc = StoredProcedures.ImportMasterListItemsToTable;
                        }
                        if (tableFilter.Product)
                        {
                            dataTble = CSVReader.ConvertCSVtoDataTable<ProductImportModel>(filePath, true);
                            parameterName = "@ProductImportRequest";
                            typeName = "[ProductImportRequestType]";
                            storedProc = StoredProcedures.ImportProductToTable;
                        }
                        if (tableFilter.Cashiers)
                        {
                            dataTble = CSVReader.ConvertCSVtoDataTable<CashierImportModel>(filePath, true);
                            parameterName = "@CashierImportRequest";
                            typeName = "[CashierImportRequestType]";
                            storedProc = StoredProcedures.ImportCashierToTable;
                        }
                    }

                    List<SqlParameter> dbParams = new List<SqlParameter>{
                new SqlParameter("@userId", userId),
                new SqlParameter("@SetInactive", reportRequest?.AddNewAsInactive),
                new SqlParameter("@AddNewOnly", reportRequest?.AddNewOnly),
                new SqlParameter{
                  Direction = ParameterDirection.Input,
                  ParameterName = parameterName,
                  TypeName =typeName,
                  Value = dataTble,
                  SqlDbType = SqlDbType.Structured
                } };

                    var dset = await repository.ExecuteStoredProcedure(storedProc, dbParams.ToArray()).ConfigureAwait(false);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }
        }
    }
}
