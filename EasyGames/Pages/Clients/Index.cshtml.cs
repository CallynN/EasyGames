using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Dapper;


namespace EasyGames.Pages.Clients
{
    public class IndexModel : PageModel
    {

        private readonly IConfiguration _configuration;
        public string connectionstring { get; private set; }
        public List<ClientInfo> listClients = new List<ClientInfo>();
        public String errorMessage = "";
        public string ClientID { get; set; }


        public IndexModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void OnGet(string clientID, string filtertype)
        {
            ClientID = clientID;

            try
            {
                String connectionstring = _configuration.GetConnectionString("DefaultConnection"); //get connection string from json



                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    connection.Open();
                    String sql = "SELECT * FROM Client";
                    // Sort by selected option
                    switch (filtertype)
                    {
                        case "NameAsc":
                            sql += " ORDER BY Name ASC";
                            break;
                        case "NameDesc":
                            sql += " ORDER BY Name DESC";
                            break;
                        case "SurnameAsc":
                            sql += " ORDER BY Surname ASC";
                            break;
                        case "SurnameDesc":
                            sql += " ORDER BY Surname DESC";
                            break;
                        case "ClientBalanceAsc":
                            sql += " ORDER BY ClientBalance ASC";
                            break;
                        case "ClientBalanceDesc":
                            sql += " ORDER BY ClientBalance DESC";
                            break;
                        default:
                            break;
                    }


                    if (!string.IsNullOrEmpty(clientID))
                    {
                        sql = "SELECT * FROM Client WHERE ClientID=@clientID";

                    }

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {

                        if (!string.IsNullOrEmpty(clientID))
                        {
                            command.Parameters.AddWithValue("@ClientID", clientID);

                        }

                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                ClientInfo clientinfo = new ClientInfo();

                                clientinfo.clientid = reader.GetInt64(0).ToString();
                                clientinfo.Name = reader.GetString(1);
                                clientinfo.Surname = reader.GetString(2);
                                clientinfo.ClientBalance = reader.GetDecimal(3).ToString("F2");

                                listClients.Add(clientinfo);

                                //for debuging uncomment below code
                                //Console.WriteLine($"ClientID: {clientinfo.clientid}, Name: {clientinfo.Name}, Surname: {clientinfo.Surname}, Balance: {clientinfo.ClientBalance}");
                            }

                        }

                    }

                }

            }
            catch (Exception ex)
            {

                Console.WriteLine("Exception " + ex.ToString());
            }
        }



    }



    public class ClientInfo
    {
        public string clientid { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string ClientBalance { get; set; }

    }

    public class TransactionInfo
    {
        public string Transactionid { get; set; }
        public string Amount { get; set; }
        public string Transactiontypeid { get; set; }
        public string Clientid { get; set; }
        public string Comment { get; set; }


    }

}
