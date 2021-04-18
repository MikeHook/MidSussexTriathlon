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

        Entry Get(string clientSecret);
        Entry Get(int id);
        IEnumerable<Entry> GetAll();
        IEnumerable<Entry> GetEntered();

        int Entered();
    }

    public class EntryRepository : IEntryRepository
    {
        string baseGet = @"SELECT [Id]
                                      ,[FirstName]
                                      ,[LastName]
                                      ,[DateOfBirth]
                                      ,[Gender]
                                      ,[PhoneNumber]
                                      ,[Email]
                                      ,[AddressLine1]
                                      ,[AddressLine2]
                                      ,[City]
                                      ,[County]
                                      ,[Postcode]
                                      ,[RaceType]
                                      ,[SwimTime]
                                      ,[SwimDistance]
                                      ,[BtfNumber]
                                      ,[ClubName]
                                      ,[TermsAccepted]
                                      ,[Paid]
                                      ,[PaymentFailureMessage]
                                      ,[OrderReference]
                                      ,[ClientSecret]
                                      ,[EntryDate]
                                      ,[NewToSport]
                                      ,[HowHeardAboutUs]
                                      ,[Relay2FirstName]
                                      ,[Relay2LastName]
                                      ,[Relay2BtfNumber]
                                      ,[Relay3FirstName]
                                      ,[Relay3LastName]
                                      ,[Relay3BtfNumber]
                                      ,[Wave]
                                  FROM [dbo].[Entry] ";

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
           ,[SwimDistance]
           ,[BtfNumber]
           ,[ClubName]
           ,[TermsAccepted]
           ,[Paid]            
           ,[OrderReference]
           ,[ClientSecret]
           ,[EntryDate]
           ,[NewToSport]
           ,[HowHeardAboutUs]
           ,[Relay2FirstName]
           ,[Relay2LastName]
           ,[Relay2BtfNumber]
           ,[Relay3FirstName]
           ,[Relay3LastName]
           ,[Relay3BtfNumber]
           ,[Wave])
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
           ,@SwimDistance
           ,@BtfNumber
           ,@ClubName
           ,@TermsAccepted
           ,@Paid
           ,@OrderReference
           ,@ClientSecret
           ,@EntryDate
           ,@NewToSport
           ,@HowHeardAboutUs
           ,@Relay2FirstName
           ,@Relay2LastName
           ,@Relay2BtfNumber
           ,@Relay3FirstName
           ,@Relay3LastName
           ,@Relay3BtfNumber
           ,@Wave)";

            entry.EntryDate = DateTime.Now;

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
                                    ,[SwimDistance] = @SwimDistance
                                    ,[BtfNumber] = @BtfNumber
                                    ,[ClubName] = @ClubName
                                    ,[TermsAccepted] = @TermsAccepted
                                    ,[Paid] = @Paid
                                    ,[PaymentFailureMessage] = @PaymentFailureMessage
                                    ,[OrderReference] = @OrderReference
                                    ,[EntryDate] = @EntryDate
                                    ,[NewToSport] = @NewToSport
                                    ,[HowHeardAboutUs] = @HowHeardAboutUs
                                    ,[Relay2FirstName] = @Relay2FirstName
                                    ,[Relay2LastName] = @Relay2LastName
                                    ,[Relay2BtfNumber] = @Relay2BtfNumber
                                    ,[Relay3FirstName] = @Relay3FirstName
                                    ,[Relay3LastName] = @Relay3LastName
                                    ,[Relay3BtfNumber] = @Relay3BtfNumber
                                    ,[Wave] = @Wave
                                WHERE Id = @Id";

            using (IDbConnection connection = _dataConnection.SqlConnection)
            {
                connection.Execute(query, entry);
            }
        }

        public IEnumerable<Entry> GetAll()
        {
            using (IDbConnection connection = _dataConnection.SqlConnection)
            {
                return connection.Query<Entry>(baseGet);
            }
        }

        public IEnumerable<Entry> GetEntered()
        {
            string query = $"{baseGet} Where Paid = 1 order By EntryDate DESC";

            using (IDbConnection connection = _dataConnection.SqlConnection)
            {
                return connection.Query<Entry>(query);
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

        public Entry Get(int id)
        {
            string query = $"{baseGet} Where Id = @Id";

            using (IDbConnection connection = _dataConnection.SqlConnection)
            {
                return connection.QueryFirst<Entry>(query, new { Id = id });
            }
        }

        public Entry Get(string clientSecret)
        {
            string query = $"{baseGet} Where ClientSecret = @ClientSecret";

            using (IDbConnection connection = _dataConnection.SqlConnection)
            {
                return connection.QueryFirst<Entry>(query, new { ClientSecret = clientSecret });
            }
        }
    }
}
