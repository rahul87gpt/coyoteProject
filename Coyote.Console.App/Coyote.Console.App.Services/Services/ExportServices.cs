using Coyote.Console.App.Models;
using Coyote.Console.App.Repository.IRepository;
using Coyote.Console.App.Services.IServices;
using Coyote.Console.Common;
using Coyote.Console.Common.Exceptions;
using Coyote.Console.Common.Services;
using Coyote.Console.ViewModels.RequestModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Coyote.Console.App.Services.Services
{
    public class ExportServices : IExportServices
    {
        private IUnitOfWork _unitOfWork = null;
        private ILoggerManager _iLoggerManager = null;

        public ExportServices(IUnitOfWork repo, ILoggerManager iLoggerManager)
        {
            _unitOfWork = repo;
            _iLoggerManager = iLoggerManager;
        }


        public async Task<byte[]> GetExportedFiles(ExportFilterRequestModel inputfilter)
        {
            try
            {
                if (inputfilter != null)
                {
                    DataSet dset = new DataSet();

                    string zipPath = $"{Directory.GetCurrentDirectory()}\\Resources\\Exports\\{DateTime.Now.ToFileTimeUtc().ToString()}";

                    var repository = _unitOfWork?.GetRepository<Store>();

                    List<SqlParameter> dbParams = new List<SqlParameter>{
                        new SqlParameter("@Product",inputfilter?.Product),
                        new SqlParameter("@OutletProduct",inputfilter?.OutletProduct),
                        new SqlParameter("@APN",inputfilter?.APN),
                        new SqlParameter("@SupplierProduct",inputfilter?.SupplierProduct),
                        new SqlParameter("@StoreGroups",inputfilter?.StoreGroups),
                        new SqlParameter("@Stores",inputfilter?.Stores ),
                        new SqlParameter("@Tills",inputfilter?.Tills),
                        new SqlParameter("@Groups",inputfilter?.Groups),
                        new SqlParameter("@Commodieties",inputfilter?.Commodieties),
                        new SqlParameter("@Category",inputfilter?.Category),
                        new SqlParameter("@Department" ,inputfilter?.Department),
                        new SqlParameter("@SubRanges",inputfilter?.SubRanges),
                        new SqlParameter("@Manufacturers",inputfilter?.Manufacturers),
                        new SqlParameter("@Suppliers",inputfilter?.Suppliers),
                        new SqlParameter("@TaxCodes",inputfilter?.TaxCodes),
                        new SqlParameter("@Users",inputfilter?.Users),
                        new SqlParameter("@Cashiers",inputfilter?.Cashiers),
                        new SqlParameter("@PriceZones",inputfilter?.PriceZones ),
                        new SqlParameter("@CostZones",inputfilter?.CostZones),
                        new SqlParameter("@GeneralZones",inputfilter?.GeneralZones),
                        new SqlParameter("@SystemPathSettings",inputfilter?.SystemPathSettings),
                        new SqlParameter("@UserTypes",inputfilter?.UserTypes),
                        new SqlParameter("@CashierTypes",inputfilter?.CashierTypes),
                        new SqlParameter("@HostSettings",inputfilter?.HostSettings),
                        new SqlParameter("@SystemSettings",inputfilter?.SystemSettings),
                        new SqlParameter("@OutletSupplier",inputfilter?.OutletSupplier)
                    };

                    dset = await repository.ExecuteStoredProcedure(StoredProcedures.GetDataToExport, dbParams.ToArray()).ConfigureAwait(false);

                    if (!Directory.Exists(zipPath))
                    {
                        Directory.CreateDirectory(zipPath);
                    }

                    DirectoryInfo dInfo = new DirectoryInfo(zipPath);
                    DirectorySecurity dSecurity = dInfo.GetAccessControl();
                    dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                    dInfo.SetAccessControl(dSecurity);


                    var response = ConvertToCSV(dset, zipPath);

                    if (Directory.Exists(zipPath))
                    {
                        Directory.Delete(zipPath, true);
                    }

                    return response;
                }

                throw new NotFoundException(ErrorMessages.NoneSelected.ToString(CultureInfo.CurrentCulture));
            }
            catch (NotFoundException)
            {
                throw new NotFoundException(ErrorMessages.StockTakeHeaderNotFound.ToString(CultureInfo.CurrentCulture));
            }
            catch (Exception ex)
            {
                if (ex is NotFoundException)
                {
                    throw new NotFoundException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        /// <summary>
        /// Convert all dataTables to CSV File
        /// </summary>
        /// <param name="dset"></param>
        /// <param name="zipPath"></param>
        /// <returns></returns>
        public static byte[] ConvertToCSV(DataSet dset, string zipPath)
        {
            using (var ms = new MemoryStream())
            {
                using (var archive =
                new ZipArchive(ms, ZipArchiveMode.Create, true))
                {
                    if (dset?.Tables.Count > 0)
                    {
                        foreach (DataTable tbl in dset.Tables)
                        {

                            string exportType = tbl.TableName;
                            string textPath = $"{zipPath}\\{exportType}.csv";
                            StreamWriter swCSVconvert = new StreamWriter(textPath, true);

                            swCSVconvert.Write(Environment.NewLine);

                            //Headers
                            for (int i = 0; i < tbl.Columns.Count; i++)
                            {
                                swCSVconvert.Write(tbl.Columns[i].ToString());
                                if (i < tbl.Columns.Count - 1)
                                {
                                    swCSVconvert.Write(",");
                                }
                            }
                            swCSVconvert.Write(Environment.NewLine);

                            //Data
                            foreach (DataRow dr in tbl.Rows)
                            {
                                for (int i = 0; i < tbl.Columns.Count; i++)
                                {
                                    if (!Convert.IsDBNull(dr[i]))
                                    {
                                        string value = dr[i].ToString();
                                        if (value.Contains(','))
                                        {
                                            value = String.Format("\"{0}\"", value);
                                            swCSVconvert.Write(value);
                                        }
                                        else
                                        {
                                            swCSVconvert.Write(dr[i].ToString());
                                        }
                                    }
                                    if (i < tbl.Columns.Count - 1)
                                    {
                                        swCSVconvert.Write(",");
                                    }
                                }
                                swCSVconvert.Write(Environment.NewLine);
                            }

                            swCSVconvert.Flush();
                            swCSVconvert.Close();


                            byte[] textFile = null;
                            if (File.Exists(textPath))
                            {
                                textFile = System.IO.File.ReadAllBytes(textPath);
                            }


                            var zipEntry = archive.CreateEntry($"{exportType}.csv",
                           CompressionLevel.Fastest);
                            using (var zipStream = zipEntry.Open())
                            {
                                zipStream.Write(textFile, 0, textFile.Length);
                            }

                        }
                    }
                    return ms.ToArray();
                }
            }


        }
    }
}
