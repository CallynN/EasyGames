using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Dapper;

namespace EasyGames.Pages.Clients
{
    public class CreateModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public string connectionstring { get; private set; }

        public ClientInfo clientInfo = new ClientInfo();
        public String errorMessage = "";
        public String successMessage2 = "";
        public int lastid = 0;
        public string lastClientID { get; set; }

        public CreateModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public void OnGet()
        {
        }

        public void OnPost()
        {


            clientInfo.Name = Request.Form["name"];
            clientInfo.Surname = Request.Form["surname"];
            clientInfo.ClientBalance = Request.Form["clientbalance"];

            //validate field filled in 
            if (clientInfo.Name.Length == 0 || clientInfo.Surname.Length == 0 || clientInfo.ClientBalance.Length == 0)
            {
                errorMessage = "All the Fields are required";
                return;
            }

            //save data
            try
            {
                String connectionstring = _configuration.GetConnectionString("DefaultConnection");
                //String connectionstring = "server=.\\sqlexpress; Database=EasyGames;User=easy;Password=easy123;";
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    connection.Open();

                    //try getting last id 
                    try
                    {
                        String sqlLastclient = "SELECT MAX(ClientID) AS LastClientID FROM Client";
                        using (SqlCommand command = new SqlCommand(sqlLastclient, connection))
                        {
                            object results = command.ExecuteScalar();
                            lastid = Convert.ToInt32(results) + 1;
                            lastClientID = lastid.ToString();
                        }
                    }
                    catch (Exception ex)
                    {

                        errorMessage = ex.Message;
                        return;
                    }


                    //write data to table

                    String sql = "INSERT INTO Client " + "([ClientID],[Name],[Surname],[ClientBalance]) VALUES" + "(@clientID ,@name, @surname, @clientbalance)";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@clientID", lastClientID);
                        command.Parameters.AddWithValue("@name", clientInfo.Name);
                        command.Parameters.AddWithValue("@surname", clientInfo.Surname);
                        command.Parameters.AddWithValue("@clientbalance", clientInfo.ClientBalance);

                        command.ExecuteNonQuery();

                    }

                }


            }
            catch (Exception ex)
            {

                errorMessage = ex.Message;
                return;
            }

            clientInfo.Name = ""; clientInfo.Surname = ""; clientInfo.ClientBalance = "";
            successMessage2 = "New Client Added Correctly";

            Response.Redirect("/Clients/Index");

        }

    }
}
