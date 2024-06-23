using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using Dapper;

namespace EasyGames.Pages.Clients
{
    public class EditModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public string connectionstring { get; private set; }

        public ClientInfo clientInfo = new ClientInfo();
        public String errorMessage = "";
        public String successMessage2 = "";


        public EditModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnGet()
        {
            //get current client 
            string id = Request.Query["id"];

            if (string.IsNullOrEmpty(id))
            {
                errorMessage = "Client ID is missing.";
                return;
            }

            try
            {
                String connectionstring = _configuration.GetConnectionString("DefaultConnection");
                //String connectionstring = "server=.\\sqlexpress; Database=EasyGames;User=easy;Password=easy123;";
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    connection.Open();
                    String sql = "SELECT * FROM Client WHERE ClientID=@clientid";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@clientid", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            if (reader.Read())
                            {
                                clientInfo.clientid = reader.GetInt64(0).ToString();
                                clientInfo.Name = reader.GetString(1);
                                clientInfo.Surname = reader.GetString(2);
                                clientInfo.ClientBalance = reader.GetDecimal(3).ToString("F2");

                                //Console.WriteLine($"ClientID: {clientinfo.clientid}, Name: {clientinfo.Name}, Surname: {clientinfo.Surname}, Balance: {clientinfo.ClientBalance}");

                            }

                        }

                    }

                }



            }
            catch (Exception ex)
            {

                errorMessage = ex.Message;
                return;
            }

        }

        public void OnPost()
        {
            //Update data 
            clientInfo.clientid = Request.Form["id"];
            clientInfo.Name = Request.Form["name"];
            clientInfo.Surname = Request.Form["surname"];
            clientInfo.ClientBalance = Request.Form["clientbalance"];

            if (clientInfo.clientid.Length == 0 || clientInfo.Name.Length == 0 || clientInfo.Surname.Length == 0 || clientInfo.ClientBalance.Length == 0)
            {
                errorMessage = "All Fields are required";
                return;
            }

            try
            {
                String connectionstring = _configuration.GetConnectionString("DefaultConnection");
                //String connectionstring = "server=.\\sqlexpress; Database=EasyGames;User=easy;Password=easy123;";
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    connection.Open();
                    String sql = "UPDATE Client SET Name = @name, Surname = @surname, ClientBalance = @clientbalance WHERE ClientID = @clientid";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {

                        command.Parameters.AddWithValue("@name", clientInfo.Name);
                        command.Parameters.AddWithValue("@surname", clientInfo.Surname);
                        command.Parameters.AddWithValue("@clientbalance", Convert.ToDecimal(clientInfo.ClientBalance));
                        command.Parameters.AddWithValue("@clientid", clientInfo.clientid);

                        command.ExecuteNonQuery();

                    }

                }


            }
            catch (Exception ex)
            {
                //catch expetion and display in output 
                errorMessage = ex.Message;
                return;

            }

            //go back to main page
            Response.Redirect("/Clients/Index");

        }

    }
}
