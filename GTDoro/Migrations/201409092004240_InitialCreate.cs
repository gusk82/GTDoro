namespace GTDoro.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Action",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 36),
                        Description = c.String(),
                        CreationDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        Estimate = c.Int(),
                        Deadline = c.DateTime(),
                        Status = c.Int(nullable: false),
                        TaskID = c.Int(nullable: false),
                        IsPersistent = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Task", t => t.TaskID, cascadeDelete: true)
                .Index(t => t.TaskID);
            
            CreateTable(
                "dbo.Pomodoro",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Start = c.DateTime(),
                        Status = c.Int(nullable: false),
                        CreationDate = c.DateTime(),
                        ActionID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Action", t => t.ActionID, cascadeDelete: true)
                .Index(t => t.ActionID);
            
            CreateTable(
                "dbo.Task",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 24),
                        Name = c.String(),
                        Description = c.String(),
                        CreationDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        Status = c.Int(nullable: false),
                        Priority = c.Int(nullable: false),
                        ProjectID = c.Int(nullable: false),
                        Sprint_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Project", t => t.ProjectID, cascadeDelete: true)
                .ForeignKey("dbo.Sprint", t => t.Sprint_ID)
                .Index(t => t.ProjectID)
                .Index(t => t.Sprint_ID);
            
            CreateTable(
                "dbo.Project",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 14),
                        Name = c.String(),
                        Description = c.String(),
                        CreationDate = c.DateTime(),
                        Status = c.Int(nullable: false),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.ApplicationUser", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.ApplicationUser",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FullName = c.String(),
                        EmailAddress = c.String(),
                        CreationDate = c.DateTime(),
                        ActionID = c.Int(),
                        Email = c.String(),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Action", t => t.ActionID)
                .Index(t => t.ActionID);
            
            CreateTable(
                "dbo.IdentityUserClaim",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ApplicationUser", t => t.ApplicationUser_Id)
                .Index(t => t.ApplicationUser_Id);
            
            CreateTable(
                "dbo.IdentityUserLogin",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        LoginProvider = c.String(),
                        ProviderKey = c.String(),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.ApplicationUser", t => t.ApplicationUser_Id)
                .Index(t => t.ApplicationUser_Id);
            
            CreateTable(
                "dbo.IdentityUserRole",
                c => new
                    {
                        RoleId = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ApplicationUser_Id = c.String(maxLength: 128),
                        IdentityRole_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => new { t.RoleId, t.UserId })
                .ForeignKey("dbo.ApplicationUser", t => t.ApplicationUser_Id)
                .ForeignKey("dbo.IdentityRole", t => t.IdentityRole_Id)
                .Index(t => t.ApplicationUser_Id)
                .Index(t => t.IdentityRole_Id);
            
            CreateTable(
                "dbo.Tag",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        CreationDate = c.DateTime(),
                        IsFixed = c.Boolean(nullable: false),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.ApplicationUser", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.CollectedThing",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.ApplicationUser", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.IdentityRole",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Sprint",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        InitComment = c.String(),
                        EndComment = c.String(),
                        CreationDate = c.DateTime(),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.ApplicationUser", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.TaskTags",
                c => new
                    {
                        TaskID = c.Int(nullable: false),
                        TagID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.TaskID, t.TagID })
                .ForeignKey("dbo.Task", t => t.TaskID, cascadeDelete: true)
                .ForeignKey("dbo.Tag", t => t.TagID, cascadeDelete: true)
                .Index(t => t.TaskID)
                .Index(t => t.TagID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Sprint", "User_Id", "dbo.ApplicationUser");
            DropForeignKey("dbo.Task", "Sprint_ID", "dbo.Sprint");
            DropForeignKey("dbo.IdentityUserRole", "IdentityRole_Id", "dbo.IdentityRole");
            DropForeignKey("dbo.CollectedThing", "User_Id", "dbo.ApplicationUser");
            DropForeignKey("dbo.TaskTags", "TagID", "dbo.Tag");
            DropForeignKey("dbo.TaskTags", "TaskID", "dbo.Task");
            DropForeignKey("dbo.Tag", "User_Id", "dbo.ApplicationUser");
            DropForeignKey("dbo.IdentityUserRole", "ApplicationUser_Id", "dbo.ApplicationUser");
            DropForeignKey("dbo.Project", "User_Id", "dbo.ApplicationUser");
            DropForeignKey("dbo.IdentityUserLogin", "ApplicationUser_Id", "dbo.ApplicationUser");
            DropForeignKey("dbo.IdentityUserClaim", "ApplicationUser_Id", "dbo.ApplicationUser");
            DropForeignKey("dbo.ApplicationUser", "ActionID", "dbo.Action");
            DropForeignKey("dbo.Task", "ProjectID", "dbo.Project");
            DropForeignKey("dbo.Action", "TaskID", "dbo.Task");
            DropForeignKey("dbo.Pomodoro", "ActionID", "dbo.Action");
            DropIndex("dbo.TaskTags", new[] { "TagID" });
            DropIndex("dbo.TaskTags", new[] { "TaskID" });
            DropIndex("dbo.Sprint", new[] { "User_Id" });
            DropIndex("dbo.CollectedThing", new[] { "User_Id" });
            DropIndex("dbo.Tag", new[] { "User_Id" });
            DropIndex("dbo.IdentityUserRole", new[] { "IdentityRole_Id" });
            DropIndex("dbo.IdentityUserRole", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.IdentityUserLogin", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.IdentityUserClaim", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.ApplicationUser", new[] { "ActionID" });
            DropIndex("dbo.Project", new[] { "User_Id" });
            DropIndex("dbo.Task", new[] { "Sprint_ID" });
            DropIndex("dbo.Task", new[] { "ProjectID" });
            DropIndex("dbo.Pomodoro", new[] { "ActionID" });
            DropIndex("dbo.Action", new[] { "TaskID" });
            DropTable("dbo.TaskTags");
            DropTable("dbo.Sprint");
            DropTable("dbo.IdentityRole");
            DropTable("dbo.CollectedThing");
            DropTable("dbo.Tag");
            DropTable("dbo.IdentityUserRole");
            DropTable("dbo.IdentityUserLogin");
            DropTable("dbo.IdentityUserClaim");
            DropTable("dbo.ApplicationUser");
            DropTable("dbo.Project");
            DropTable("dbo.Task");
            DropTable("dbo.Pomodoro");
            DropTable("dbo.Action");
        }
    }
}
