CREATE TABLE [dbo].[Like] (
    [IdLike]  NVARCHAR (50) NOT NULL,
    [IdPesan] NVARCHAR (50) NULL,
    [IdUser]  NVARCHAR (50) NULL,
    PRIMARY KEY CLUSTERED ([IdLike] ASC)
);

