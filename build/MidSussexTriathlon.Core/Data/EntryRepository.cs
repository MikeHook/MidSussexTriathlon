using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using MidSussexTriathlon.Core.Domain;

namespace MidSussexTriathlon.Core.Data
{
    public interface IEntryRepository
    {
        void Create(Entry entry);
    }

    public class EntryRepository : IEntryRepository
    {
        private readonly IDataConnection _dataConnection;

        public EntryRepository(IDataConnection dataConnection)
        {
            _dataConnection = dataConnection;
        }

        public void Create(Entry entry)
        {
            string query = @"INSERT INTO [dbo].[Entry]
           ([FirstName]
           ,[LastName]
           ,[DateOfBirth]
           ,[Gender]
           ,[AddressLine1]
           ,[AddressLine2]
           ,[City]
           ,[County]
           ,[Postcode]
           ,[PhoneNumber]
           ,[Email]
           ,[RaceType]
           ,[SwimTime]
           ,[BtfNumber]
           ,[ClubName]
           ,[TermsAccepted]
           ,[Paid]
           ,[OrderReference])
     VALUES
           (@FirstName
           ,@LastName
           ,@DateOfBirth
           ,@Gender
           ,@AddressLine1
           ,@AddressLine2
           ,@City
           ,@County
           ,@Postcode
           ,@PhoneNumber
           ,@Email
           ,@RaceType
           ,@SwimTime
           ,@BtfNumber
           ,@ClubName
           ,@TermsAccepted
           ,@Paid
           ,@OrderReference)";

            using (IDbConnection connection = _dataConnection.SqlConnection)
            {
                connection.Execute(query, entry);
            }
        }
    }
}
