using System;
using System.Data;
using Npgsql;

public class Db
{
    private string connectionString;

    public Db(){
        var host = "localhost";
        var port = 5432;
        var username = "postgres";
        var password = "22945";
        var database = "PetDb";
        connectionString = $"Host={host};Port={port};Username={username};Password={password};Database={database}";
    }

    private NpgsqlConnection GetConnection()
    {
        return new NpgsqlConnection(connectionString);
    }

    public DataTable ExecuteQuery(string query, params NpgsqlParameter[] parameters)
    {
        using var connection = new NpgsqlConnection(connectionString);
        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddRange(parameters);

        using var adapter = new NpgsqlDataAdapter(command);
        var dataTable = new DataTable();
        adapter.Fill(dataTable);

        return dataTable;
    }
    public int ExecuteNonQuery(string query, params NpgsqlParameter[] parameters)
    {
        using var connection = GetConnection();
        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddRange(parameters);

        connection.Open();
        int rowsAffected = command.ExecuteNonQuery();
        connection.Close();

        return rowsAffected;
    }

    
}
