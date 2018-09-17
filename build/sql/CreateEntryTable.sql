CREATE TABLE [dbo].[Entry](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] varchar(255) NOT NULL,
	[LastName] varchar(255) NOT NULL,
	[DateOfBirth] datetime NOT NULL,
	[Gender] varchar(255) NOT NULL,
	[PhoneNumber] varchar(255) NULL,
	[Email] varchar(255) NOT NULL,
	[AddressLine1] varchar(255) NOT NULL,
	[AddressLine2] varchar(255) NULL,
	[City] varchar(255) NOT NULL,
	[County] varchar(255) NULL,
	[Postcode] varchar(255) NULL,
	[RaceType] varchar(255) NOT NULL,
	[SwimTime] varchar(255) NOT NULL,
	[BtfNumber] varchar(255) NULL,
	[ClubName] varchar(255) NULL,
	[TermsAccepted] bit Not Null,
	[Paid] bit Not Null,
	[OrderReference] varchar(255) Not NULL
 CONSTRAINT [PK_Entry] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO