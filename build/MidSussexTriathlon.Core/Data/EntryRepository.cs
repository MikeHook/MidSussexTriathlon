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
        Entry Create(Entry entry);
        void Update(Entry entry);
        int Entered();
    }

    public class EntryRepository : IEntryRepository
    {
        private readonly IDataConnection _dataConnection;

        public EntryRepository(IDataConnection dataConnection)
        {
            _dataConnection = dataConnection;
        }

        public Entry Create(Entry entry)
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
        OUTPUT Inserted.Id
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
                entry.Id = connection.Query<int>(query, entry).Single();
            }

            return entry;
        }

        public void Update(Entry entry)
        {
            string query = @"   UPDATE [dbo].[Entry]
                                SET [FirstName] = @FirstName
                                    ,[LastName] = @LastName
                                    ,[DateOfBirth] = @DateOfBirth
                                    ,[Gender] = @Gender
                                    ,[AddressLine1] = @AddressLine1
                                    ,[AddressLine2] = @AddressLine2
                                    ,[City] = @City
                                    ,[County] = @County
                                    ,[Postcode] = @Postcode
                                    ,[PhoneNumber] = @PhoneNumber
                                    ,[Email] = @Email
                                    ,[RaceType] = @RaceType
                                    ,[SwimTime] = @SwimTime
                                    ,[BtfNumber] = @BtfNumber
                                    ,[ClubName] = @ClubName
                                    ,[TermsAccepted] = @TermsAccepted
                                    ,[Paid] = @Paid
                                    ,[PaymentFailureMessage] = @PaymentFailureMessage
                                    ,[OrderReference] = @OrderReference
                                WHERE Id = @Id";

            using (IDbConnection connection = _dataConnection.SqlConnection)
            {
                connection.Execute(query, entry);
            }
        }

        public int Entered()
        {
            string query = @"Select Count(Id) FROM [dbo].[Entry] Where Paid = 1";     
            using (IDbConnection connection = _dataConnection.SqlConnection)
            {
                return connection.Query<int>(query).Single();
            }
        }
    }
}
