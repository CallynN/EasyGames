# Project Setup Instructions

## Setting Up the Database

Before starting the project, follow these steps to set up the database:

1. **Create Database Before Opening Project:**
   - Execute the SQL script provided below to create the database `EasyGames`.

2. **Modify `appsettings.json`:**
   - Update the `appsettings.json` file in your project to connect to the newly created database. You can find the database connection string under `ConnectionStrings` in the file.
   - **Note:** You don't need to change the database name (`EasyGames`), username (`easy`), and password (`easy123`) due to my script creating them automatically.

   Example of `appsettings.json`:
   ```json
   //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! Database Connection String HERE !!!!!!!!!!!!!!!!!!!!!!!!!!!!!
   "ConnectionStrings": {
     "DefaultConnection": "server=.\\sqlexpress; Database=EasyGames;User=easy;Password=easy123;"
   }


-----------------BEGINNING OF SCRIPT----------------------------------
-- Create the database EasyGames if it does not exist

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'EasyGames')
BEGIN
    CREATE DATABASE EasyGames;
END
GO

-- Use the EasyGames database
USE EasyGames;
GO

-- Create TransactionType table
CREATE TABLE TransactionType (
    TransactionTypeID SMALLINT PRIMARY KEY,
    TransactionTypeName NVARCHAR(50) NOT NULL
);
GO

-- Create Client table
CREATE TABLE Client (
    ClientID INT PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL,
    Surname NVARCHAR(50) NOT NULL,
    ClientBalance DECIMAL(18, 2) NOT NULL
);
GO

-- Create Transactions table
CREATE TABLE Transactions (
    TransactionID BIGINT PRIMARY KEY,
    Amount DECIMAL(18, 2) NOT NULL,
    TransactionTypeID SMALLINT NOT NULL,
    ClientID INT NOT NULL,
    Comment NVARCHAR(50) NULL
);
GO

CREATE LOGIN easy WITH PASSWORD = 'easy123', CHECK_POLICY = OFF;
GO

-- Create user 'easy' mapped to login 'easy' with default schema 'dbo'
CREATE USER easy FOR LOGIN easy WITH DEFAULT_SCHEMA = dbo;
GO

-- Assign necessary permissions to 'easy'
GRANT SELECT, INSERT, UPDATE, DELETE ON Schema::dbo TO easy;
GO

---------------------------------------END OF SCRIPT----------------------
