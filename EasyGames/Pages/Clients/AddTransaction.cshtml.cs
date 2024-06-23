using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System;
using System.Data.SqlClient;
using Dapper;
using System.Globalization;

namespace EasyGames.Pages.Clients
{
    public class AddTransactionModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public ClientInfo clientInfo = new ClientInfo();
        public TransactionInfo transactionInfo = new TransactionInfo();
        public string errorMessage = "";
        public string successMessage2 = "";
        public int lastid = 0;
        public string lastTransactionID { get; set; }


        public AddTransactionModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnGet()
        {
            string id = Request.Query["id"];

            if (string.IsNullOrEmpty(id))
            {
                errorMessage = "Client ID is missing.";
                return;
            }

            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM Client WHERE ClientID=@clientid";

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
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }

        public void OnPost()
        {
            transactionInfo.Clientid = Request.Form["id"];
            clientInfo.ClientBalance = Request.Form["clientbal"];
            transactionInfo.Comment = Request.Form["comment"];
            transactionInfo.Transactiontypeid = Request.Form["transactiontype"];
            transactionInfo.Amount = Request.Form["amount"];

            if (transactionInfo.Transactiontypeid != "1" && transactionInfo.Transactiontypeid != "2")
            {
                errorMessage = "Please select a valid transaction type.";
                return;
            }

            if (string.IsNullOrEmpty(transactionInfo.Amount) || string.IsNullOrEmpty(transactionInfo.Comment))
            {
                errorMessage = "All fields are required.";
                return;
            }

            string normalizedAmount = transactionInfo.Amount.Replace(',', '.');

            decimal amount = Convert.ToDecimal(transactionInfo.Amount, CultureInfo.InvariantCulture);
            decimal currentBalance = Convert.ToDecimal(clientInfo.ClientBalance);

            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    //here
                    try
                    {
                        String sqlLastclient = "SELECT MAX(TransactionID) AS LasttransactionID FROM Transactions";
                        using (SqlCommand command = new SqlCommand(sqlLastclient, connection))
                        {
                            object results = command.ExecuteScalar();
                            lastid = Convert.ToInt32(results) + 1;
                            lastTransactionID = lastid.ToString();
                        }
                    }
                    catch (Exception ex)
                    {

                        errorMessage = ex.Message;
                        return;
                    }
                    try
                    {
                        String sqlLastclient = "SELECT MAX(TransactionID) AS LasttransactionID FROM Transactions";
                        using (SqlCommand command = new SqlCommand(sqlLastclient, connection))
                        {
                            object results = command.ExecuteScalar();
                            lastid = Convert.ToInt32(results) + 1;
                            lastTransactionID = lastid.ToString();
                        }
                    }
                    catch (Exception ex)
                    {

                        errorMessage = ex.Message;
                        return;
                    }


                    // Insert transaction
                    string insertTransactionSql = "INSERT INTO Transactions " + "([TransactionID], [Amount], [TransactionTypeID], [ClientID], [Comment]) VALUES" + "(@transactionID ,@amount, @transactiontype, @clientid, @comment) ";
                                                  

                    using (SqlCommand command = new SqlCommand(insertTransactionSql, connection))
                    {
                        command.Parameters.AddWithValue("@transactionID", lastTransactionID);
                        command.Parameters.AddWithValue("@amount", amount);
                        command.Parameters.AddWithValue("@transactiontype", transactionInfo.Transactiontypeid);
                        command.Parameters.AddWithValue("@clientid", transactionInfo.Clientid);
                        command.Parameters.AddWithValue("@comment", transactionInfo.Comment);


                        command.ExecuteNonQuery();
                    }

                    // Update client balance based on transaction type
                    string updateClientBalanceSql = "";

                    if (transactionInfo.Transactiontypeid == "1")
                    {
                        updateClientBalanceSql = "UPDATE Client SET ClientBalance = ClientBalance - @amount WHERE ClientID = @clientid";
                    }
                    else if (transactionInfo.Transactiontypeid == "2")
                    {
                        updateClientBalanceSql = "UPDATE Client SET ClientBalance = ClientBalance + @amount WHERE ClientID = @clientid";
                    }

                    string tempAmount = amount.ToString();
                    char firstChar =  tempAmount[0];

                    //input validation to check if user eneters a - before number  
                    if (firstChar == '-')
                    {
                        String modifyAmount = tempAmount.Substring(1);
                        amount = decimal.Parse(modifyAmount);
                    }

                    using (SqlCommand command = new SqlCommand(updateClientBalanceSql, connection))
                    {
                        command.Parameters.AddWithValue("@amount", amount);
                        command.Parameters.AddWithValue("@clientid", transactionInfo.Clientid);

                        command.ExecuteNonQuery();
                    }

                    successMessage2 = "New Transaction Added Successfully";
                    Response.Redirect("/Clients/Index");
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }
    }
}
