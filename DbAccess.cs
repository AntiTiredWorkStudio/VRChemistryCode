using System;
using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;
using System.IO;
using System.Collections.Generic;
public class DbAccess

{
    public const string DBName = "Atom";
    /// <summary>
    /// 数据库连接
    /// </summary>
    public static DbAccess L
    {
        get
        {

            string targetDB = @"Data Source=" + Application.streamingAssetsPath + "/"+ DBName+ ".db";
           // Debug.Log(targetDB);
            //System.IO.File.WriteAllText(System.Environment.CurrentDirectory + "/log.txt", targetDB);
            return  new DbAccess(targetDB);//"data source=" + System.Environment.CurrentDirectory + "/Main.db");
        }
    }

    private SqliteConnection dbConnection;

    private SqliteCommand dbCommand;

    private SqliteDataReader reader;


    public DbAccess (string connectionString)

    {

        OpenDB (connectionString);

    }


    public DbAccess ()
	{
		
	}

    public void OpenDB (string connectionString)

    {
		//try
   //		 {
           if (!File.Exists(Application.streamingAssetsPath + "/" + DBName + ".db"))
            {
             SqliteConnection.CreateFile(Application.streamingAssetsPath + "/" + DBName + ".db");
            }
            dbConnection = new SqliteConnection (connectionString);

            dbConnection.Open ();
			
	//	 }
 ///   	catch(Exception e)
//    	{
 //      		string temp1 = e.ToString();
            
   // 	}

    }



    public void CloseSqlConnection ()

    {

        if (dbCommand != null) {

            dbCommand.Dispose ();

        }

        dbCommand = null;

        if (reader != null) {

            reader.Dispose ();

        }

        reader = null;

        if (dbConnection != null) {

            dbConnection.Close ();

        }

        dbConnection = null;


    }
    
    public static  Dictionary<string, int> GetName2indexHash(SqliteDataReader sdr)
    {
        Dictionary<string, int> result = new Dictionary<string, int>();
        for (int i = 0; i < sdr.FieldCount; i++)
        {
            string name =
                sdr.GetName(i);
            result.Add(name, i);
        }
        return result;
    }
    
    public SqliteDataReader ExecuteQuery (string sqlQuery)

    {

        dbCommand = dbConnection.CreateCommand ();

        dbCommand.CommandText = sqlQuery;

        Debug.Log(sqlQuery);
        //Console.WriteLine(sqlQuery);

        reader = dbCommand.ExecuteReader ();

        return reader;

    }

    public SqliteDataReader ReadLineByFieldValue(string tableName,string FieldName , string FieldValue)
    {
        string query = "SELECT * FROM " + tableName +" WHERE "+FieldName+"="+FieldValue;

        return ExecuteQuery(query);
    }

    public SqliteDataReader ReadFullTableOrderByField(string tableName, string OrderField)
    {
        string query = "SELECT * FROM " + tableName + " ORDER BY " + OrderField;

        return ExecuteQuery(query);
    }

   

    public SqliteDataReader ReadFullTable (string tableName)
    {
        string query = "SELECT * FROM " + tableName;

        return ExecuteQuery (query);

    }




    public SqliteDataReader InsertInto (string tableName, string[] values)

    {

        string query = "INSERT INTO " + tableName + " VALUES (" + values[0];
      //  System.Windows.Forms.MessageBox.Show(query);
        for (int i = 1; i < values.Length; ++i) {

            query += ", " + values[i];

        }

        query += ")";


        return ExecuteQuery (query);

    }

	
	public SqliteDataReader UpdateInto (string tableName, string []cols,string []colsvalues,string selectkey,string selectvalue)
	{
		
		
		string query = "UPDATE "+tableName+" SET "+cols[0]+" = "+colsvalues[0];
	
		for (int i = 1; i < colsvalues.Length; ++i) {
		
		 	 query += ", " +cols[i]+" ="+ colsvalues[i];
		}
		
		 query += " WHERE "+selectkey+" = "+selectvalue+" ";

        System.IO.File.WriteAllText(System.Environment.CurrentDirectory + "/" + tableName + ".txt", query);

		return ExecuteQuery (query);
	}
	

    public SqliteDataReader Delete(string tableName,string []cols,string []colsvalues)
	{
			string query = "DELETE FROM "+tableName + " WHERE " +cols[0] +" = " + colsvalues[0];
		
			for (int i = 1; i < colsvalues.Length; ++i) {
		
		 	    query += " or " +cols[i]+" = "+ colsvalues[i];
			}
		return ExecuteQuery (query);
	}
	
    
	
    public SqliteDataReader InsertIntoSpecific (string tableName, string[] cols, string[] values)

    {

        if (cols.Length != values.Length) {

            throw new SqliteException ("columns.Length != values.Length");

        }

        string query = "INSERT INTO " + tableName + "(" + cols[0];

        for (int i = 1; i < cols.Length; ++i) {

            query += ", " + cols[i];

        }

        query += ") VALUES (" + values[0];

        for (int i = 1; i < values.Length; ++i) {

            query += ", " + values[i];

        }

        query += ")";

        return ExecuteQuery (query);

    }

    public void DeleteTable(string tableName)

    {

        string query = "DROP TABLE " + tableName ;

        ExecuteQuery(query).Close();

    }

    public SqliteDataReader DeleteContents (string tableName)

    {

        string query = "DELETE FROM " + tableName;

        return ExecuteQuery (query);

    }

    public bool HasTable(string tableName)
    {
        try
        {
            SqliteDataReader sdr = ExecuteQuery("SELECT * from " + tableName);
        }
        catch
        {
            return false;
        }
        return true;
    }
    public SqliteDataReader CreateTable (string name, string[] col, string[] colType, string[] defaultValue)

    {

        if (col.Length != colType.Length) {
		
            throw new SqliteException ("columns.Length != colType.Length");

        }

        string query = "CREATE TABLE " + name + " (" + col[0] + " " + colType[0];

        for (int i = 1; i < col.Length; ++i) {

            query += ", " + col[i] + " " + colType[i]+" DEFAULT '"+defaultValue[i]+"'";

        }

        query += ")";

        System.IO.File.WriteAllText(System.Environment.CurrentDirectory+"/command.txt",query);

        return ExecuteQuery (query);
    }


    public List<Dictionary<string, string>> ReadFullTableReturnList(string tableName)
    {
        List<Dictionary<string, string>> resultArray = new List<Dictionary<string, string>>();
        SqliteDataReader sdr = ReadFullTable(tableName);
        do
        {
            Dictionary<string, string> singleLine = new Dictionary<string, string>();
            for (int i = 0; i < sdr.FieldCount; i++)
            {
                switch (sdr.GetFieldType(i).ToString())
                {
                    case "System.String":
                        singleLine.Add(sdr.GetName(i), sdr.GetValue(i) as string);
                        break;
                    case "System.Double":
                        singleLine.Add(sdr.GetName(i), sdr.GetValue(i).ToString());
                        break;
                    case "System.Int64":
                        singleLine.Add(sdr.GetName(i), sdr.GetValue(i).ToString());
                        break;
                    default:
                        break;
                }
               // Debug.Log(sdr.GetFieldType(i).ToString() + "--"+sdr.GetName(i)+":"+sdr.GetValue(i));
            }
            resultArray.Add(singleLine);
        } while (sdr.Read());
        resultArray.Remove(resultArray[0]);//移出莫名其妙多出来的氢
        return resultArray;
    }
	
	

    public SqliteDataReader SelectWhere (string tableName, string[] items, string[] col, string[] operation, string[] values)

    {

        if (col.Length != operation.Length || operation.Length != values.Length) {

            throw new SqliteException ("col.Length != operation.Length != values.Length");

        }

        string query = "SELECT " + items[0];

        for (int i = 1; i < items.Length; ++i) {

            query += ", " + items[i];

        }

        query += " FROM " + tableName + " WHERE " + col[0] + operation[0] + "'" + values[0] + "' ";

        for (int i = 1; i < col.Length; ++i) {

            query += " AND " + col[i] + operation[i] + "'" + values[0] + "' ";

        }


        return ExecuteQuery (query);

        

    }

    

    

    

}