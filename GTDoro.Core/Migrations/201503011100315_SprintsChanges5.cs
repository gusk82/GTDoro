namespace GTDoro.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SprintsChanges5 : DbMigration
    {
        public override void Up()
        {
            //RenameTable(name: "dbo.SprintTag", newName: "SprintTags");
            //RenameColumn(table: "dbo.SprintTags", name: "Sprint_ID", newName: "SprintID");
            //RenameColumn(table: "dbo.SprintTags", name: "Tag_ID", newName: "TagID");
            //RenameIndex(table: "dbo.SprintTags", name: "IX_Sprint_ID", newName: "IX_SprintID");
            //RenameIndex(table: "dbo.SprintTags", name: "IX_Tag_ID", newName: "IX_TagID");
        }
        
        public override void Down()
        {
            //RenameIndex(table: "dbo.SprintTags", name: "IX_TagID", newName: "IX_Tag_ID");
            //RenameIndex(table: "dbo.SprintTags", name: "IX_SprintID", newName: "IX_Sprint_ID");
            //RenameColumn(table: "dbo.SprintTags", name: "TagID", newName: "Tag_ID");
            //RenameColumn(table: "dbo.SprintTags", name: "SprintID", newName: "Sprint_ID");
            //RenameTable(name: "dbo.SprintTags", newName: "SprintTag");
        }
    }
}
//USE [GTDoro]
//GO

///****** Object:  Table [dbo].[SprintTags]    Script Date: 03/01/2015 11:21:11 ******/
//SET ANSI_NULLS ON
//GO

//SET QUOTED_IDENTIFIER ON
//GO

//CREATE TABLE [dbo].[SprintTags](
//    [SprintID] [int] NOT NULL,
//    [TagID] [int] NOT NULL,
// CONSTRAINT [PK_dbo.SprintTags] PRIMARY KEY CLUSTERED 
//(
//    [SprintID] ASC,
//    [TagID] ASC
//)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
//) ON [PRIMARY]

//GO

//ALTER TABLE [dbo].[SprintTags]  WITH CHECK ADD  CONSTRAINT [FK_dbo.SprintTags_dbo.Sprint_SprintID] FOREIGN KEY([SprintID])
//REFERENCES [dbo].[Sprint] ([ID])
//ON DELETE CASCADE
//GO

//ALTER TABLE [dbo].[SprintTags] CHECK CONSTRAINT [FK_dbo.SprintTags_dbo.Sprint_SprintID]
//GO

//ALTER TABLE [dbo].[SprintTags]  WITH CHECK ADD  CONSTRAINT [FK_dbo.SprintTags_dbo.Tag_TagID] FOREIGN KEY([TagID])
//REFERENCES [dbo].[Tag] ([ID])
//ON DELETE CASCADE
//GO

//ALTER TABLE [dbo].[SprintTags] CHECK CONSTRAINT [FK_dbo.SprintTags_dbo.Tag_TagID]
//GO


