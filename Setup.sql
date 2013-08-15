/* (1) Create a database, say, named "CustomMembershipWithRoles"
 * (2) Run this sql script to create the tables required for this project
 * (3) Change the connection string "UsersContext" in the "Web.config" file, if required.
 *
 */ 
 
/*  Users table */
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Users](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[CompanyId] [int] NOT NULL,
	[RoleId] [int] NOT NULL,
	[UserName] [nvarchar](50) NOT NULL,
	[Password] [nvarchar](50) NOT NULL,
	[FirstName] [nvarchar](50) NULL,
	[LastName] [nvarchar](50) NULL,
	[LastLogin] [datetime] NULL,
	[CreatedBy] [nvarchar](50) NULL,
	[EmailAddress] [nvarchar](50) NULL,
	[PhoneNumber] [nchar](10) NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK_Users_Companies] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[Companies] ([CompanyId])
GO

ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK_Users_Companies]
GO

ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK_Users_Roles] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Roles] ([RoleId])
GO

ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK_Users_Roles]
GO


/* Insert default user - password: "administrator" */
INSERT INTO dbo.Users VALUES('administrator','200ceb26807d6bf99fd6f4f0d1ca54d4','test@mail.com');

/* Roles table */
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Roles](
	[RoleId] [int] IDENTITY(0,1) NOT NULL,
	[RoleName] [nvarchar](50) NULL,
	[RoleDescription] [nvarchar](50) NULL,
 CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED 
(
	[RoleId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

/* Roles table entries */
INSERT INTO dbo.Roles VALUES(0,'SuperAdmin','Vendita Admin');
INSERT INTO dbo.Roles VALUES(1, 'Admin', 'Company Admin');
INSERT INTO dbo.Roles VALUES(2, 'Author', 'Companuy User');

/* Company table */
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Companies](
	[CompanyId] [int] IDENTITY(1,1) NOT NULL,
	[CompanyName] [nvarchar](50) NULL,
	[CompanyAddress] [nvarchar](50) NULL,
	[CompanyCity] [nvarchar](50) NULL,
	[CompanyState] [nvarchar](50) NULL,
	[CompanyZip] [nvarchar](50) NULL,
 CONSTRAINT [PK_Companies] PRIMARY KEY CLUSTERED 
(
	[CompanyId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

