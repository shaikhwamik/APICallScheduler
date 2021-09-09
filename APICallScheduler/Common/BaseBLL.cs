
using APICallScheduler.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;

namespace APICallScheduler.Common
{
    public class BaseBLL
    {

        string ConnectionString = string.Empty;
        SqlConnection SqlConn = new SqlConnection();
        
        public BaseBLL()
        {
            ConnectionString = ConfigurationManager.ConnectionStrings["HealthAssureConnectionString"].ConnectionString;

        }
        public SqlConnection GetConnection()
        {
            try
            {
                SqlConn = new SqlConnection(ConnectionString);
                SqlConn.Open();
            }
            catch
            {
                throw;
            }
            return SqlConn;
        }

        public void CloseConnection()
        {
            try
            {
                if (SqlConn.State == ConnectionState.Open)
                {
                    SqlConn.Close();
                    SqlConn.Dispose();
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                SqlConn.Close();
                SqlConn.Dispose();
            }
        }
        public void uspSaveErrorLog(string Code, string ErrorMessage, string StackTrace, string IsActive, string CreatedBy)
        {
            try
            {
                uspSaveErrorLogDAL(Code, ErrorMessage, StackTrace, IsActive, CreatedBy);
            }
            catch
            {
                throw;
            }
        }

        public static DataTable ConvertToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties =
            TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }
        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Defining type of data column gives proper data table 
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name, type);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }
        public void uspSaveErrorLogDAL(string Code, string ErrorMessage, string StackTrace, string IsActive, string CreatedBy)
        {
            DataTable dt = new DataTable();
            SqlDataAdapter dap;
            try
            {
                SqlConnection MyConn = GetConnection();
                SqlCommand cmd = new SqlCommand("USPSAVEERRORLOG", MyConn);
                cmd.Parameters.Add("@Code", SqlDbType.VarChar).Value = Code;
                cmd.Parameters.Add("@ErrorMessage", SqlDbType.VarChar).Value = ErrorMessage;
                cmd.Parameters.Add("@StackTrace", SqlDbType.VarChar).Value = StackTrace;
                cmd.Parameters.Add("@IsActive", SqlDbType.VarChar).Value = IsActive;
                cmd.Parameters.Add("@CreatedBy", SqlDbType.VarChar).Value = CreatedBy;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandTimeout = 50000;
                dap = new SqlDataAdapter(cmd);
                dap.Fill(dt);
            }
            catch
            {
                throw;
            }
            finally
            {
                CloseConnection();
            }
        }
        public string GetSystemCodePath(string CodeType, string Code)
        {
            string PrefixPath = Convert.ToString(ConfigurationManager.AppSettings["CommonPath"]);
            string Path = "";
            try
            {
                SchedulerDAL DAL = new SchedulerDAL();
                Path = DAL.GetSystemCodePath(CodeType, Code);
                Path = String.IsNullOrEmpty(Path) ? Path : Path.Replace(@"\\103.1.112.179\", "");
                Path = PrefixPath + Path;
            }
            catch
            {
                throw;
            }
            return Path;
        }
        public string GetFilePath(string FilePath)
        {
            string FinalPath = string.Empty;
            string PrefixPath = GetSystemCodePath("FILESAVEPATH", "PURGEPATH");  // @"\\103.1.112.179\UploadDoc\";
            // string PrefixPath = ConfigurationManager.AppSettings["CommonPath"];
            FinalPath = PrefixPath + "\\" + FilePath;
            return FinalPath;

        }

    }
}