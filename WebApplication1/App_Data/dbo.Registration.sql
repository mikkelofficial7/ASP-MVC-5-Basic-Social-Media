CREATE TABLE [dbo].[Registration]
(
	[Id] NVARCHAR(100) NOT NULL PRIMARY KEY, 
    [Username] NVARCHAR(150) NULL, 
    [Password] NTEXT NULL, 
    [Email] NVARCHAR(200) NULL, 
    [Telepon] NVARCHAR(15) NULL
)
