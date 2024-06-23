using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using Dapper;
using Microsoft.AspNetCore.Http;

namespace EasyGames.Pages.Clients
{
    public class EditCommentModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public string connectionstring { get; private set; }

        public TransactionInfo transactionInfo = new TransactionInfo();
        public ClientInfo clientInfo = new ClientInfo();
        public String errorMessage = "";
        public String successMessage2 = "";


        public EditCommentModel(IConfiguration configuration)
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
                    String sql = "SELECT * FROM Transactions WHERE TransactionID=@transid";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@transid", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            if (reader.Read())
                            {
                                //transactionInfo.Comment = reader.GetInt64(0).ToString();
                                //clientInfo.Name = reader.GetString(1);
                                //clientInfo.Surname = reader.GetString(2);
                                //clientInfo.ClientBalance = reader.GetDecimal(3).ToString("F2");
                                transactionInfo.Transactionid = reader.GetInt64(0).ToString();
                                transactionInfo.Amount = reader.GetDecimal(1).ToString("F2");
                                transactionInfo.Transactiontypeid = reader.GetInt16(2).ToString();
                                transactionInfo.Clientid = reader.GetInt32(3).ToString();
                                transactionInfo.Comment = reader.GetString(4);

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
            transactionInfo.Comment = Request.Form["comment"];
            string id = Request.Query["id"];
            //clientInfo.Name = Request.Form["name"];
            //clientInfo.Surname = Request.Form["surname"];
            //clientInfo.ClientBalance = Request.Form["clientbalance"];

            if (transactionInfo.Comment.Length == 0 )
            {
                errorMessage = "Comment is required";
                return;
            }

            try
            {
                String connectionstring = _configuration.GetConnectionString("DefaultConnection");
                //String connectionstring = "server=.\\sqlexpress; Database=EasyGames;User=easy;Password=easy123;";
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    connection.Open();
                    String sql = "UPDATE Transactions SET Comment = @comment WHERE TransactionID =@transid";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {

                        command.Parameters.AddWithValue("@comment", transactionInfo.Comment);
                        command.Parameters.AddWithValue("@transid", id);
                        

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
