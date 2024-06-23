using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Data.SqlClient;
using Dapper;

namespace EasyGames.Pages.Clients
{
    public class ViewTransactionModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public string connectionstring { get; private set; }

        public ClientInfo clientInfo = new ClientInfo();
        //public TransactionInfo transactionInfo = new TransactionInfo();
        public List<TransactionInfo> listTransactions = new List<TransactionInfo>();

        public String errorMessage = "";
        


        public ViewTransactionModel(IConfiguration configuration)
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
                String connectionstring = _configuration.GetConnectionString("DefaultConnection"); //get connection string from json


                using (SqlConnection connection = new SqlConnection(connectionstring))
                {

                    connection.Open();


                    String sql = "SELECT * FROM Transactions WHERE ClientID = @clientid";
                    
               


                   
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        
                        command.Parameters.AddWithValue("@clientid", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            //command.Parameters.AddWithValue("@clientid", id);
                            while (reader.Read())
                            {
                                //TransactionInfo tranactioninfo = new TransactionInfo();
                                TransactionInfo transtinfo = new TransactionInfo();

                                transtinfo.Transactionid = reader.GetInt64(0).ToString();
                                transtinfo.Amount = reader.GetDecimal(1).ToString("F2");
                                transtinfo.Transactiontypeid = reader.GetInt16(2).ToString();
                                transtinfo.Clientid = reader.GetInt32(3).ToString();
                                transtinfo.Comment = reader.GetString(4);

                                //dontforget to add to list
                                listTransactions.Add(transtinfo);

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


        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            int clientId = 0;
            try
            {
                string connectionstring = _configuration.GetConnectionString("DefaultConnection");

                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    connection.Open();

                    // Get the transaction details before deleting
                    string selectSql = "SELECT Amount, TransactionTypeID, ClientID FROM Transactions WHERE TransactionID = @transactionID";
                    decimal amount = 0;
                    int transactionType = 0;
                    //string clientId = "";

                    using (SqlCommand selectCommand = new SqlCommand(selectSql, connection))
                    {
                        selectCommand.Parameters.AddWithValue("@transactionID", id);
                        using (SqlDataReader reader = selectCommand.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                amount = reader.GetDecimal(0);
                                transactionType = reader.GetInt16(1);
                                clientId = reader.GetInt32(2);
                            }
                        }
                    }

                    // Delete the transaction
                    string deleteSql = "DELETE FROM Transactions WHERE TransactionID = @transactionID";
                    using (SqlCommand deleteCommand = new SqlCommand(deleteSql, connection))
                    {
                        deleteCommand.Parameters.AddWithValue("@transactionID", id);
                        await deleteCommand.ExecuteNonQueryAsync();
                    }

                    // Update the client's balance
                    string updateBalanceSql = "";
                    if (transactionType == 1) // Debit
                    {
                        updateBalanceSql = "UPDATE Client SET ClientBalance = ClientBalance + @amount WHERE ClientID = @clientId";
                    }
                    else if (transactionType == 2) // Credit
                    {
                        updateBalanceSql = "UPDATE Client SET ClientBalance = ClientBalance - @amount WHERE ClientID = @clientId";
                    }

                    using (SqlCommand updateCommand = new SqlCommand(updateBalanceSql, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@amount", amount);
                        updateCommand.Parameters.AddWithValue("@clientId", clientId);
                        await updateCommand.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return BadRequest();
            }

            return RedirectToPage(new { id = clientId });
        }


    }
}
