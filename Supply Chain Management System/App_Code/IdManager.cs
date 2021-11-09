using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Data.SqlClient;


namespace autouniv
{
    /// <summary>
    /// Summary description for IdManager.
    /// </summary>
    public class IdManager
    {
        private static decimal TotalUnitPrice = 0.0M;
        
        public static int GetNextID(string tableName, string idField)
        {
            int val;
            String ConnectionString = DataManager.OraConnString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);

            string Query = "select max(convert(int," + idField + ")) from " + tableName;
            myConnection.Open();
            SqlCommand myCommand = new SqlCommand(Query, myConnection);
            object maxValue = myCommand.ExecuteScalar();
            myConnection.Close();
            if (maxValue == DBNull.Value) return 1;
            else
                val = int.Parse((maxValue).ToString());
            return val + 1;
        }

        public static int GetNextSl(string tableName, string idField,string criField, Int32 criteria)
        {
            int val;
            String ConnectionString = DataManager.OraConnString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);

            string Query = "select max(" + idField + ") from gl_trans_dtl where vch_sys_no = "+criteria;
            myConnection.Open();
            SqlCommand myCommand = new SqlCommand(Query, myConnection);
            object maxValue = myCommand.ExecuteScalar();
            myConnection.Close();
            if (maxValue == DBNull.Value) return 1;
            else
                val = int.Parse((maxValue).ToString());
            return val + 1;
        }

        public static int GetNextEmpSl(string criteria)
        {
            int val;
            String ConnectionString = DataManager.OraConnString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);

            string Query = "select count(*) from pmis_personnel where convert(nvarchar,emp_no) = '" + criteria + "'";
            myConnection.Open();
            SqlCommand myCommand = new SqlCommand(Query, myConnection);
            object maxValue = myCommand.ExecuteScalar();
            myConnection.Close();
            if (maxValue == DBNull.Value) return 1;
            else
                val = int.Parse((maxValue).ToString());
            return val + 1;
        }

        public static string GetItemSl(string criteria)
        {
            String ConnectionString = DataManager.OraConnString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);

            string Query = "select ltrim(to_char(nvl(max(to_number(substr(item_code,-3))),0)+1,'000')) from item_mst where item_code like '" + criteria + "%'";
            myConnection.Open();
            SqlCommand myCommand = new SqlCommand(Query, myConnection);
            object maxValue = myCommand.ExecuteScalar();
            myConnection.Close();

            return maxValue.ToString();
        }
        public static string DateFormat()
        {
            string Format = "MMM-dd-yyyy";
            return Format;
        }
        public static string GetTransSl(string tableName, string idField)
        {
            String ConnectionString = DataManager.OraConnString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);

            string Query = "select rtrim(ltrim(to_char(sysdate,'RR')))||ltrim(to_char(nvl(max(to_number(substr(rtrim(ltrim(" + idField + ")),4))),0)+1,'000000')) from " + tableName;
            myConnection.Open();
            SqlCommand myCommand = new SqlCommand(Query, myConnection);
            object maxValue = myCommand.ExecuteScalar();
            myConnection.Close();

            return maxValue.ToString();
        }

        public static int GetNextSlStd()
        {
            //int val;
            String ConnectionString = DataManager.OraConnString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);

            //string Query = "select IsNull(max(to_number(substr(student_id,5,5))),0) sid from student_info ";
            //myConnection.Open();
            //SqlCommand myCommand = new SqlCommand(Query, myConnection);
            //object maxValue = myCommand.ExecuteScalar();
            //myConnection.Close();
            //if (maxValue == DBNull.Value) return 1;
            //else
            //    val = int.Parse((maxValue).ToString());
            //return val + 1;
            
            try
            {
                myConnection.Open();
                string Query = "select isnull(max(student_id),0) sid from student_info ";
                SqlCommand command = new SqlCommand(Query, myConnection);
                int maxValue = Convert.ToInt32(command.ExecuteScalar());
                if (maxValue >= 0)
                {
                    return maxValue + 1;
                }
                else
                {
                    return 1;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (myConnection.State == ConnectionState.Open)
                    myConnection.Close();
            }
        }
        
        public static decimal GetUnitPrice(decimal Price)
        {
            if (Price != 0)
            {
                TotalUnitPrice += Price;
            }
            else
            {
                Price = 0;
            }
            return Price;
            
        }

        public static decimal GetTotal()
        {
            return TotalUnitPrice;
        }
        public static string getColName(string comm)
        {
            string colname = "";
            string connectionString = DataManager.OraConnString();
            SqlDataReader dReader;
            SqlCommand cmd;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = connectionString;
            conn.Open();
            cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select lower(table_name)||'.'||lower(column_name) col_name from user_col_comments where comments= '" + comm + "' ";
            dReader = cmd.ExecuteReader();
            if (dReader.HasRows == true)
            {
                while (dReader.Read())
                {
                    colname = dReader["col_name"].ToString();
                }
            }
            return colname;
        }
        // ********************* Get Show Value Int  *************************//

        public static int GetShowSingleValueInt(string ShowField, string SearchField, string TableName, string Parameter)
        {
            SqlConnection connection = new SqlConnection(DataManager.OraConnString());
            try
            {
                connection.Open();
                string Query = "select " + ShowField + " from " + TableName + " where " + SearchField + "='" + Parameter + "' ";
                SqlCommand command = new SqlCommand(Query, connection);
                object Val = command.ExecuteScalar();
                if (Val==null)
                {
                    return 0;
                }
                return Convert.ToInt32(Val);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }
        public static int GetShowSingleValueInt(string ShowField, string SearchField, string TableName, string Parameter, string OrderBy)
        {
            SqlConnection connection = new SqlConnection(DataManager.OraConnString());
            try
            {
                connection.Open();
                string Query = "select " + ShowField + " from " + TableName + " where " + SearchField + "='" + Parameter + "' " + OrderBy + " ";
                SqlCommand command = new SqlCommand(Query, connection);
                object Val = command.ExecuteScalar();
                if (Val == null)
                {
                    return 0;
                }
                return Convert.ToInt32(Val);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }
        public static int GetShowSingleValueInt(string ShowField, string SearchField, string TableName, string Parameter,string CodeField,int id)
        {
            SqlConnection connection = new SqlConnection(DataManager.OraConnString());
            try
            {
                connection.Open();
                string Query = "select " + ShowField + " from " + TableName + " where " + SearchField + "='" + Parameter + "' and " + CodeField + " != " + id;
                SqlCommand command = new SqlCommand(Query, connection);
                object Val = command.ExecuteScalar();
                if (Val == null)
                {
                    return 0;
                }
                return Convert.ToInt32(Val);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }
        public static int GetShowSingleValueInt(string ShowField, string TableName)
        {
            SqlConnection connection = new SqlConnection(DataManager.OraConnString());
            try
            {
                connection.Open();
                string Query = "select " + ShowField + " from " + TableName + " ";
                SqlCommand command = new SqlCommand(Query, connection);
                object Val = command.ExecuteScalar();
                if (Val == null)
                {
                    return 0;
                }
                return Convert.ToInt32(Val);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }
       
        // ********************* Get Show Value String  *************************//

        public static string GetShowSingleValueString(string ShowField, string SearchField, string TableName, string Parameter)
        {
            SqlConnection connection = new SqlConnection(DataManager.OraConnString());
            try
            {
                connection.Open();
                string Query = "select " + ShowField + " from " + TableName + " where " + SearchField + "='" + Parameter + "' ";
                SqlCommand command = new SqlCommand(Query, connection);
                object Value = command.ExecuteScalar();
                if (Value == null)
                    return "";
                else
                    return Value.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }

        public static string GetShowSingleValueString(string ShowField, string SearchField, string TableName, string Parameter, string OrderBy)
        {
            SqlConnection connection = new SqlConnection(DataManager.OraConnString());
            try
            {
                connection.Open();
                string Query = "select " + ShowField + " from " + TableName + " where " + SearchField + "='" + Parameter + "' " + OrderBy + " ";
                SqlCommand command = new SqlCommand(Query, connection);
                object Value = command.ExecuteScalar();
                if (Value == null)
                    return "";
                else
                    return Value.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }

        // ********************* Get Show Value Currency *************************//

        public static double GetShowSingleValueCurrency(string ShowField, string SearchField, string TableName, string Parameter)
        {
            SqlConnection connection = new SqlConnection(DataManager.OraConnString());
            try
            {
                connection.Open();
                string Query = "select isnull(" + ShowField + ",0) from " + TableName + " where " + SearchField + "='" + Parameter + "' ";
                SqlCommand command = new SqlCommand(Query, connection);
                return Convert.ToDouble(command.ExecuteScalar());

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }
        public static double GetShowSingleValueCurrency(string ShowField, string SearchField, string TableName, string Parameter, string OrderBy)
        {
            SqlConnection connection = new SqlConnection(DataManager.OraConnString());
            try
            {
                connection.Open();
                string Query = "select isnull(" + ShowField + ",0) from " + TableName + " where " + SearchField + "='" + Parameter + "' " + OrderBy + " ";
                SqlCommand command = new SqlCommand(Query, connection);

                return Convert.ToDouble(command.ExecuteScalar());

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }
        //************************* Auto Id ******************************//
        public static string getAutoIdWithParameter(string FixNumber, string TableName, string SearchField, string Parameter, string Lavel, string Index)
        {
            SqlConnection connection = new SqlConnection(DataManager.OraConnString());
            try
            {
                connection.Open();
                string Query = "SELECT '" + FixNumber + "'+ RIGHT('" + Lavel + "'+Convert(varchar,ISNULL(Max(Convert(integer, RIGHT(" + SearchField + ", " + Index + "))),0)+ 1)," + Index + ") FROM " + TableName + " where PARENT_CODE='" + Parameter + "'";
                SqlCommand command = new SqlCommand(Query, connection);
                return command.ExecuteScalar().ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }

        public static int GetShowSingleValueIntNotParameter(string ShowField, string TableName)
        {
            SqlConnection connection = new SqlConnection(DataManager.OraConnString());
            try
            {
                connection.Open();
                string Query = "select " + ShowField + " from " + TableName + " ";
                SqlCommand command = new SqlCommand(Query, connection);
                return Convert.ToInt32(command.ExecuteScalar());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }



        // ********************************************************************
        public static byte[] GetShowImage(string ShowField, string SearchField, string TableName, string Parameter)
        {
            SqlConnection connection = new SqlConnection(DataManager.OraConnString());
            try
            {
                byte[] img = null;
                String ConnectionString = DataManager.OraConnString();
                SqlConnection myConnection = new SqlConnection(ConnectionString);
                string Query = "select " + ShowField + " from " + TableName + " where " + SearchField + "='" + Parameter + "' ";
                myConnection.Open();
                SqlCommand myCommand = new SqlCommand(Query, myConnection);
                object maxValue = myCommand.ExecuteScalar();
                myConnection.Close();
                if (maxValue != System.DBNull.Value)
                {
                    img = (byte[])maxValue;
                }
                return img;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }
        public static DataTable GetShowDataTable(string query)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection oracon = new SqlConnection(connectionString);
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Table");
            return dt;
        }

        public static string GetTransSl1(string tableName, string idField)
        {
            String ConnectionString = DataManager.OraConnString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);

            string Query = "select right('0000'+cast(isnull(max(convert(nvarchar,SUBSTRING(" + idField + ",3,len(" + idField + ")-2))),0)+1 as varchar),4) from " + tableName;
            myConnection.Open();
            SqlCommand myCommand = new SqlCommand(Query, myConnection);
            object maxValue = myCommand.ExecuteScalar();
            myConnection.Close();

            return maxValue.ToString();
        }
        public static string GetDateTimeWiseSerial(string Vl, string Field, string Table)
        {
            SqlConnection connection = new SqlConnection(DataManager.OraConnString());
            try
            {
                connection.Open();
                string Query = "SELECT '" + Vl + "'+Convert(varchar,Right(Year('" + Globals._localTime.ToString() + "'),2), 2)+RIGHT('0' + RTRIM(MONTH('" + Globals._localTime.ToString() + "')), 2)+''+ Right('0000' + convert(varchar,ISNULL(Max(Convert(integer, RIGHT(" + Field + ", 4))),0)+ 1), 4) FROM " + Table;
                SqlCommand command = new SqlCommand(Query, connection);
                return command.ExecuteScalar().ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }

        public static int GetShowSingleValueIntTowParameter(string ShowField, string SearchField, string TableName, string Parameter)
        {
            SqlConnection connection = new SqlConnection(DataManager.OraConnString());
            try
            {
                connection.Open();
                string Query = "select " + ShowField + " from " + TableName + " where " + Parameter;
                SqlCommand command = new SqlCommand(Query, connection);
                return Convert.ToInt32(command.ExecuteScalar());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }

        public static int GetShowSingleValueInt1(string ShowField, string SearchField, string TableName, string Parameter, string CheckField, int Id)
        {
            SqlConnection connection = new SqlConnection(DataManager.OraConnString());
            try
            {
                connection.Open();
                string Query = "select " + ShowField + " from " + TableName + " where " + SearchField + "='" + Parameter + "' and " + CheckField + " !=" + Id + " ";
                SqlCommand command = new SqlCommand(Query, connection);
                return Convert.ToInt32(command.ExecuteScalar());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }
    }
}
