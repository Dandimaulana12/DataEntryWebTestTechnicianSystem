using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace DataEntryWeb.Pages;

public class IndexModel : PageModel
{

    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public List<ClientInfo> listClients = new List<ClientInfo>();

    public void OnGet()
    {
        FetchData();
    }

    public void OnPost()
    {
        string nama = Request.Form["Nama"];
        string alamat = Request.Form["Alamat"];
        string umurText = Request.Form["Umur"];

        if (!string.IsNullOrEmpty(nama) && !string.IsNullOrEmpty(alamat) && int.TryParse(umurText, out int umur))
        {
            try
            {
                string connectionString = "Data Source=DESKTOP-3E3SUCK\\SQLEXPRESS;Initial Catalog=WebDataEntryDB;Integrated Security=True;";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT INTO dbo.People (Nama, Alamat, Umur) VALUES (@Nama, @Alamat, @Umur)";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Nama", nama);
                        command.Parameters.AddWithValue("@Alamat", alamat);
                        command.Parameters.AddWithValue("@Umur", umur);
                        command.ExecuteNonQuery();
                    }
                }

                Console.WriteLine("Data berhasil disimpan: " + nama + ", " + alamat + ", " + umur);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error saat menyimpan data: " + ex.Message);
            }
        }
        else
        {
            Console.Error.WriteLine("Error: Data tidak valid atau ada field kosong.");
        }

        FetchData();
    }


    private void FetchData()
    {
        try
        {
            string connectionString = "Data Source=DESKTOP-3E3SUCK\\SQLEXPRESS;Initial Catalog=WebDataEntryDB;Integrated Security=True;";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM dbo.People ORDER BY Nama ASC";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        listClients.Clear();
                        while (reader.Read())
                        {
                            ClientInfo clientInfo = new ClientInfo
                            {
                                id = reader.GetInt32(0),
                                Nama = reader.GetString(1),
                                Alamat = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                Umur = reader.IsDBNull(3) ? 0 : reader.GetInt32(3)
                            };

                            listClients.Add(clientInfo);
                            Console.WriteLine("Data ditemukan: " + clientInfo.Nama + ", " + clientInfo.Alamat + ", " + clientInfo.Umur);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("Error saat mengambil data: " + ex.Message);
        }
    }


    public class ClientInfo
    {
        public int id;
        public string Nama;
        public string Alamat;
        public int Umur;
    }
}
