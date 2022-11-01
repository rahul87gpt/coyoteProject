using Coyote.Console.Common;
using Coyote.Console.Common.Exceptions;
using Coyote.Console.ViewModels.ImportModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;

namespace Coyote.Console.App.Services.Helper
{
    public static class CSVReader
    {
        public static DataTable ConvertCSVtoDataTable<T>(string strFilePath, bool isIdentityRowRequired = false, bool isUserTable = false)
        {
            try
            {
                DataTable dataTable = new DataTable(typeof(T).Name);
                using (StreamReader sr = new StreamReader(strFilePath))
                {
                    if (isIdentityRowRequired)
                    {
                        dataTable.Columns.Add("RowIndex", typeof(System.Int64));
                    }

                    PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    foreach (PropertyInfo prop in Props)
                    {
                        //Setting column names as Property names  and their types
                        dataTable.Columns.Add(prop.Name);
                    }

                    string[] headers = sr.ReadLine().Split(',');
                    int itemCounter = 0;
                    while (!sr.EndOfStream)
                    {
                        string[] rows = sr.ReadLine().Split(',');
                        string password = "";
                        itemCounter++;
                        var values = new object[isIdentityRowRequired == true ? (Props.Length + 1) : Props.Length];
                        for (int i = 0; i < Props.Length; i++)
                        {
                            if (i == 0)
                            {
                                values[i] = itemCounter;
                            }


                            if (isUserTable && i == Props.Length - 1)
                            {
                                if (Props[i].Name == "EncryptedPassword")
                                {

                                    values[i + 1] = EncryptDecryptAlgorithm.EncryptString(password);
                                }
                            }
                            else
                            {
                                Type type = Nullable.GetUnderlyingType(Props[i].PropertyType) ?? Props[i].PropertyType;
                                var safeValue = (string.IsNullOrEmpty(rows[i])) ? null : Convert.ChangeType(rows[i], type);

                                if (isIdentityRowRequired)
                                {
                                    if (Props[i].Name == "Password")
                                    {
                                        password = safeValue.ToString();
                                    }

                                    values[i + 1] = safeValue;
                                }
                            }
                        }
                        dataTable.Rows.Add(values);
                    }
                }
                return dataTable;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }
        }


        public static DataTable ConvertCSVtoDataTable<T>(string strFilePath, bool isIdentityRowRequired = false)
        {
            try
            {
                DataTable dataTable = new DataTable(typeof(T).Name);
                using (StreamReader sr = new StreamReader(strFilePath))
                {
                    if (isIdentityRowRequired)
                    {
                        dataTable.Columns.Add("RowIndex", typeof(System.Int64));
                    }
                    // DataTable Header = new DataTable();

                    //foreach (string header in headers)
                    //{
                    //    Header.Columns.Add(header);
                    //}

                    PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    foreach (PropertyInfo prop in Props)
                    {
                        //Setting column names as Property names  and their types
                        dataTable.Columns.Add(prop.Name);
                    }

                    string[] headers = sr.ReadLine().Split(',');
                    //Checking Column count 
                    if (headers.Length != dataTable.Columns.Count - 1)
                    {
                        throw new BadRequestException(ErrorMessages.InvalidColumn.ToString(CultureInfo.CurrentCulture));
                    }

                    //Checking column names and order
                    for (int i = 0; i < Props.Length; i++)
                    {
                        // if (Header.Columns[i].ColumnName != Props[i + 1].Name)
                        if (headers[i] != Props[i].Name)
                        {
                            throw new BadRequestException($"{ErrorMessages.InvalidColumn.ToString(CultureInfo.CurrentCulture)} : {headers[i]}");
                        }
                    }


                    int itemCounter = 0;
                    while (!sr.EndOfStream)
                    {
                        string[] rows = sr.ReadLine().Split(',');
                        itemCounter++;
                        var values = new object[isIdentityRowRequired == true ? (Props.Length + 1) : Props.Length];
                        for (int i = 0; i < Props.Length; i++)
                        {
                            if (i == 0)
                            {
                                values[i] = itemCounter;
                            }

                            Type type = Nullable.GetUnderlyingType(Props[i].PropertyType) ?? Props[i].PropertyType;
                            var safeValue = (string.IsNullOrEmpty(rows[i])) ? null : Convert.ChangeType(rows[i], type);

                            if (isIdentityRowRequired)
                            {
                                values[i + 1] = safeValue;
                            }
                        }
                        dataTable.Rows.Add(values);
                    }
                }
                return dataTable;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }
        }

        public static DataTable ConvertPromoCSVtoDataTable(string strFilePath)
        {
            try
            {
                DataTable dataTable = new DataTable(typeof(PromoProductImportModel).Name);
                using (StreamReader sr = new StreamReader(strFilePath))
                {
                    PropertyInfo[] Props = typeof(PromoProductImportModel).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    foreach (PropertyInfo prop in Props)
                    {
                        //Setting column names as Property names  and their types
                        dataTable.Columns.Add(prop.Name);
                    }

                    string[] headers = sr.ReadLine().Split(',');
                    //Checking Column count 

                    int itemCounter = 0;
                    while (!sr.EndOfStream)
                    {
                        itemCounter++;

                        string[] rows = sr.ReadLine().Split(',');
                        DataRow dr = dataTable.NewRow();
                        //for (int i = 0; i < headers.Length; i++)
                        //{
                          //  dr["Id"] = i;
                            dr["Desc"] = rows[1];
                            dr["APN"] = rows[2];
                            dr["COST"] = rows[10];
                            dr["Price1"] = rows[11];
                            dr["Price2"] = rows[12];
                            dr["Price3"] = rows[13];
                            dr["Price4"] = rows[14];
                            dr["AmtOff"] = rows[15];
                            dr["Group"] = rows[16];
                            dr["Supplier"] = rows[17];
                            dr["CostIsPromo"] = rows[18];
                       // }
                        dataTable.Rows.Add(dr);
                    }
                }
                return dataTable;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }
        }
    }
}
