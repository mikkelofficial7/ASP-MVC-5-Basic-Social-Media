CREATE TABLE [dbo].[Friend] (
    [IdPertemanan] NVARCHAR (150) NOT NULL,
    [IdAkun]       NVARCHAR (150) NULL,
    [IdTeman]      NVARCHAR (150) NULL,
    PRIMARY KEY CLUSTERED ([IdPertemanan] ASC)
);

