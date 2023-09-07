CREATE TABLE [dbo].[Appointments] (
    [AppointmentId] INT            IDENTITY (1, 1) NOT NULL,
    [UserId]        NVARCHAR (128) NULL,
    [DateTime]      DATETIME       NOT NULL,
    [Status]        INT            NOT NULL,
    CONSTRAINT [PK_dbo.Appointments] PRIMARY KEY CLUSTERED ([AppointmentId] ASC),
    CONSTRAINT [FK_dbo.Appointments_dbo.AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_UserId]
    ON [dbo].[Appointments]([UserId] ASC);

