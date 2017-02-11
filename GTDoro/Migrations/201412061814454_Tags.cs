namespace GTDoro.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Tags : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProjectTags",
                c => new
                    {
                        ProjectID = c.Int(nullable: false),
                        TagID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ProjectID, t.TagID })
                .ForeignKey("dbo.Project", t => t.ProjectID, cascadeDelete: true)
                .ForeignKey("dbo.Tag", t => t.TagID, cascadeDelete: true)
                .Index(t => t.ProjectID)
                .Index(t => t.TagID);
            
            CreateTable(
                "dbo.ActionTags",
                c => new
                    {
                        ActionID = c.Int(nullable: false),
                        TagID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ActionID, t.TagID })
                .ForeignKey("dbo.Action", t => t.ActionID, cascadeDelete: true)
                .ForeignKey("dbo.Tag", t => t.TagID, cascadeDelete: true)
                .Index(t => t.ActionID)
                .Index(t => t.TagID);
            
            AddColumn("dbo.Tag", "IconCssClass", c => c.String());
            AddColumn("dbo.Tag", "GroupName", c => c.String());

            Sql("INSERT INTO Tag (Code, Name, CreationDate, IsFixed, IconCssClass, GroupName) VALUES ('shopping', 'shopping', GETDATE(), 1, 'fa-shopping-cart', 'type')");
            Sql("INSERT INTO Tag (Code, Name, CreationDate, IsFixed, IconCssClass, GroupName) VALUES ('people', 'people', GETDATE(), 1, 'fa-users', 'type')");
            Sql("INSERT INTO Tag (Code, Name, CreationDate, IsFixed, IconCssClass, GroupName) VALUES ('computer', 'computer', GETDATE(), 1, 'fa-laptop', 'type')");
            Sql("INSERT INTO Tag (Code, Name, CreationDate, IsFixed, IconCssClass, GroupName) VALUES ('software', 'software', GETDATE(), 1, 'fa-code', 'type')");
            Sql("INSERT INTO Tag (Code, Name, CreationDate, IsFixed, IconCssClass, GroupName) VALUES ('home', 'home', GETDATE(), 1, 'fa-home', 'place')");
            Sql("INSERT INTO Tag (Code, Name, CreationDate, IsFixed, IconCssClass, GroupName) VALUES ('office', 'office', GETDATE(), 1, 'fa-building-o', 'place')");
            Sql("INSERT INTO Tag (Code, Name, CreationDate, IsFixed, IconCssClass, GroupName) VALUES ('week day', 'week day', GETDATE(), 1, 'fa-calendar', 'date')");
            Sql("INSERT INTO Tag (Code, Name, CreationDate, IsFixed, IconCssClass, GroupName) VALUES ('weekend', 'weekend', GETDATE(), 1, 'fa-calendar-o', 'date')");
            Sql("INSERT INTO Tag (Code, Name, CreationDate, IsFixed, IconCssClass, GroupName) VALUES ('daytime', 'daytime', GETDATE(), 1, 'fa-sun-o', 'time')");
            Sql("INSERT INTO Tag (Code, Name, CreationDate, IsFixed, IconCssClass, GroupName) VALUES ('night', 'night', GETDATE(), 1, 'fa-moon-o', 'time')");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ActionTags", "TagID", "dbo.Tag");
            DropForeignKey("dbo.ActionTags", "ActionID", "dbo.Action");
            DropForeignKey("dbo.ProjectTags", "TagID", "dbo.Tag");
            DropForeignKey("dbo.ProjectTags", "ProjectID", "dbo.Project");
            DropIndex("dbo.ActionTags", new[] { "TagID" });
            DropIndex("dbo.ActionTags", new[] { "ActionID" });
            DropIndex("dbo.ProjectTags", new[] { "TagID" });
            DropIndex("dbo.ProjectTags", new[] { "ProjectID" });
            DropColumn("dbo.Tag", "GroupName");
            DropColumn("dbo.Tag", "IconCssClass");
            DropTable("dbo.ActionTags");
            DropTable("dbo.ProjectTags");

            Sql("DELETE FROM Tag WHERE Code IN ('shopping', 'people', 'computer', 'software', 'home', 'office', 'week day', 'weekend', 'daytime', 'night')");
        }
    }
}

//CREATE TABLE [dbo].[ProjectTags] (
//    [ProjectID] [int] NOT NULL,
//    [TagID] [int] NOT NULL,
//    CONSTRAINT [PK_dbo.ProjectTags] PRIMARY KEY ([ProjectID], [TagID])
//)
//CREATE INDEX [IX_ProjectID] ON [dbo].[ProjectTags]([ProjectID])
//CREATE INDEX [IX_TagID] ON [dbo].[ProjectTags]([TagID])
//CREATE TABLE [dbo].[ActionTags] (
//    [ActionID] [int] NOT NULL,
//    [TagID] [int] NOT NULL,
//    CONSTRAINT [PK_dbo.ActionTags] PRIMARY KEY ([ActionID], [TagID])
//)
//CREATE INDEX [IX_ActionID] ON [dbo].[ActionTags]([ActionID])
//CREATE INDEX [IX_TagID] ON [dbo].[ActionTags]([TagID])
//ALTER TABLE [dbo].[Tag] ADD [IconCssClass] [nvarchar](max)
//ALTER TABLE [dbo].[Tag] ADD [GroupName] [nvarchar](max)
//INSERT INTO Tag (Code, Name, CreationDate, IsFixed, IconCssClass, GroupName) VALUES ('shopping', 'shopping', GETDATE(), 1, 'fa-shopping-cart', 'type')
//INSERT INTO Tag (Code, Name, CreationDate, IsFixed, IconCssClass, GroupName) VALUES ('people', 'people', GETDATE(), 1, 'fa-users', 'type')
//INSERT INTO Tag (Code, Name, CreationDate, IsFixed, IconCssClass, GroupName) VALUES ('computer', 'computer', GETDATE(), 1, 'fa-laptop', 'type')
//INSERT INTO Tag (Code, Name, CreationDate, IsFixed, IconCssClass, GroupName) VALUES ('software', 'software', GETDATE(), 1, 'fa-code', 'type')
//INSERT INTO Tag (Code, Name, CreationDate, IsFixed, IconCssClass, GroupName) VALUES ('home', 'home', GETDATE(), 1, 'fa-home', 'place')
//INSERT INTO Tag (Code, Name, CreationDate, IsFixed, IconCssClass, GroupName) VALUES ('office', 'office', GETDATE(), 1, 'fa-building-o', 'place')
//INSERT INTO Tag (Code, Name, CreationDate, IsFixed, IconCssClass, GroupName) VALUES ('week day', 'week day', GETDATE(), 1, 'fa-calendar', 'date')
//INSERT INTO Tag (Code, Name, CreationDate, IsFixed, IconCssClass, GroupName) VALUES ('weekend', 'weekend', GETDATE(), 1, 'fa-calendar-o', 'date')
//INSERT INTO Tag (Code, Name, CreationDate, IsFixed, IconCssClass, GroupName) VALUES ('daytime', 'daytime', GETDATE(), 1, 'fa-sun-o', 'time')
//INSERT INTO Tag (Code, Name, CreationDate, IsFixed, IconCssClass, GroupName) VALUES ('night', 'night', GETDATE(), 1, 'fa-moon-o', 'time')
//ALTER TABLE [dbo].[ProjectTags] ADD CONSTRAINT [FK_dbo.ProjectTags_dbo.Project_ProjectID] FOREIGN KEY ([ProjectID]) REFERENCES [dbo].[Project] ([ID]) ON DELETE CASCADE
//ALTER TABLE [dbo].[ProjectTags] ADD CONSTRAINT [FK_dbo.ProjectTags_dbo.Tag_TagID] FOREIGN KEY ([TagID]) REFERENCES [dbo].[Tag] ([ID]) ON DELETE CASCADE
//ALTER TABLE [dbo].[ActionTags] ADD CONSTRAINT [FK_dbo.ActionTags_dbo.Action_ActionID] FOREIGN KEY ([ActionID]) REFERENCES [dbo].[Action] ([ID]) ON DELETE CASCADE
//ALTER TABLE [dbo].[ActionTags] ADD CONSTRAINT [FK_dbo.ActionTags_dbo.Tag_TagID] FOREIGN KEY ([TagID]) REFERENCES [dbo].[Tag] ([ID]) ON DELETE CASCADE
//INSERT [dbo].[__MigrationHistory]([MigrationId], [ContextKey], [Model], [ProductVersion])
//VALUES (N'201412061814454_Tags', N'GTDoro.DAL.GTDoroContext',  0x1F8B0800000000000400ED3DDB6EE4BA91EF01F61F1AFDB409E6B87D3973303BB01338F6F81C237331C63E49909781DCA26D65D4524752CFB111EC97EDC37ED2FEC2929228F152BC49945AED310618B845B2582C16AB8AC52AF2FFFEE77F8FFFF4B88A67DF5096476972323FD8DB9FCF50B24CC328B93F996F8ABB1FDECCFFF4C7FFF8DDF1BB70F538FB2BAD7744EAE196497E327F288AF5DBC5225F3EA05590EFADA26596E6E95DB1B74C578B204C1787FBFBFFB5383858200C628E61CD66C79F374911AD50F903FF3C4B93255A179B20FE908628CEEBEFB8E4BA843AFB18AC50BE0E96E864FEF3CD799AA57BE7A7EFE7B3D3380A3006D728BE9BCF8224498BA0C0F8BDFD3547D7459626F7D76BFC21886F9ED608D7BB0BE21CD578BF6DABDB0E61FF900C61D136A4A0969BBC48578E000F8E6A9A2CC4E69D283B6F6886A9F60E53B77822A32E2977323F5D5680C5AEDE9EC519A9D690B5A2FF5E55FFD5ACFAFAAA9978CC1FE4DFABD9D9262E36193A49D0A6C882F8D5EC6A731B47CBBFA0A79BF42B4A4E924D1CB32861A47019F7017FBACAD235CA8AA7CFE8AE46F4F27C3E5BF0ED1662C3A619D3A61AC465521C1DCE671F71E7C16D8C9A1967067C5DA419FA1925280B0A145E054581323C6197212A6926F52EF445FEA7BD6116C3AB643EFB103CBE47C97DF180D7C54FF3D945F48842FAA146E0D724C26B0AB729B20D0210D4777A8EF26516ADAB2954F6FD7A7FDFAA737D5F67192A09758EC9433B237FDF442B63DB7749D8A9598E6501D3AE9E45134D82308E12E7CEAE311F6CF2868C5872D4CC4E0B1CE7E632BF226B232F30FF50A87F4ED318058933ACAB2C4AB3920919ECDEA36F287EF788E1872874067913E45FCDEB4380F131F816DD975C202298AED2100B044CA6CF282E6BE40FD1BAC694967EA1D2E6224B579FD3986958177DB909B27B44E895C2E5D7E9265B3A207613DCC33835FD91F2161FE673D317C5852DA378DAE3917F05F12005F5E03844D8EF1255B8428824C78B56D26BE53FA5AFBD06A02D5E74002442B2C283DCA124EE2879FA48EA8AA9FC0906BAE2FB49057125AAA446A72580D7B33DF7E3CA2F8C2F731C268EC600C17F0E61FD184CAE817AB5B3B96C3B37D810CB3439CBF3B338C8F3C13BFB394B376B4F341D4E405DE665F7CE1695414275361244D50C1910D6565496FE132D0B851155154AC8B0DF65F3892D74B79DF2AF302EA5FD2122D27C84CD954E28E0FD7AA6C0E0FE4B55C822507D93D44553D04B4FD4B474B094AA062FFAC2555F1CFCB80575F1B243EFD00C325E2BAE276BDED67E75DFBD590943C96A8424A52761D80807411EB67D022291167A938AA7EB35161FE5D752DEB50A85515E8A3AB2265355ECB5E1AC36C2F6E676FEF5457EBACACFC317F9F95DC9CFE1BD853552BEFD021A816A7289C9361EE02F7334BCBB8B77101B51F6F7D67A06B31B4442A9ECAC25B6A007EC85379566A25ADA86400F3B08F45067A21EBE1942DC5DE0EA0639FBDA4ED4B91E076061F58F3441DA410FE45119C05F6910D6AB208A07F77294BD9CA5C95D94AD3A382C441115E4F96F6916FE12E40F83A37E8D961BA241B0025AAD07EFEDEA0133DEC7CDEA960897F1FAF2363537BFA517C1125B74EF12D2AA37BCF7E9F26BBA296A93E2D762E9BA281A005ED0395D2E519E5F606646E159BA694F332D6D0060F3328C97B1CB2184B8C991CF22E01AF2E1205CCD55F79FC541B4523823851E685535AA550D23AA75355754DFA7F791CA6F2AF440ABAA51AD6A1851ADAB75B4EFEC9075DA349B109676D7B628137076F8D635D5C896158C9856B57A598AD4E023F04AA6D2D98A1F9AA0A5D37CFD1115ADB958C1BDC8304CACF1BEEE49605FCDAC1BB736E6A1AD8D7974707B77F4E6F54F4178F4D38FE8E8F5F8F6E6F00E04424A2FA69EC1B223B345FA18A7A7BF06F1C677579D98BF1453FE99BF043B7DE6A7CCE5BA008C4C39D0A6AB242BFEF62D0AC7B040EB8E3035A6C0AA44ECFBE7540275FA8C4AB09419D52B4FD32EC6E6E971D69235C79DA5718CCD2014DE3C9488D8BA90F8762F2701AE4EF9619C457A874D679FB0F2588BE702E9DC1F2896AC5DA84E2F5B173341696E5C054488152E7E51A9E90B5F0F74C2A5EF6312E6A27FFDBB1BBA77573CF223BBAA886082B7FDECF47EA9ABB532502E95442050A59704BC5E636A38844355F55F641D1C36DE4D97DA9E42BBE6AE24517196AE564CEACA70C724493856574E074D1D22812A0EFF5257681727FB5D5A965CA1AB97506932D550455389F9AC42A48369B4594919853474E132BF8883FB36C5D22DCFB082D257646026C1FBEDF8093315BBA479127F40E44C8819C3378C6D69219CCCF7A509E16A63F65DC7D88E0B9B0607860641B24471CC343894495D1155436821A2A32BA53930E393FA639AAD82D896D4BF44F70FB6542675515EA8682C567F9FFEC6D6FEC15CBDAD7BD061FAC4249FAEF3C7C3197F02AFE22089EE2286990D73F8376C7496F2DDD362312FC7237D835F93657B145C37F9B1CB8CCA915B9D27550435791928CD92615A3F25BFA4F170737A9A2D1F30FA8E137A9AE7E9322A674658A5F46098EF12DB2E3343C69A9CD5870D1B3C6311397DC3DAF464FE0769246AB04D8CAE98A42C023DE031C5403F25E788907056352053962F8350367C306542FE0B36A551466CD980C4B3E498CBB0A120DBDD51B28CD641AC475D686669B013B49A0EC49273B426CA2B29F47361D3731B4E24F7DF742390CB449DE305C356066E63C3DA953C01C6B84B01A44E6C06E610B520CB14492D38874172E19D2A8CE0584F162312463E20CF2B50B15E8032B93BAD22880E232C2168CC36DDD25B05B6B27AB8485DED6C4A61BB7AC6D22D1D38CBC3B81AB72F9F21C4C7E22C81FE36DD3211E8DB632EAD5C0602B17BB2D528E2581101A742CC140EC7C845316EDC7EF486603AA3F0DDDFDB93CDCF4EAB448FC908EB454FEF1DB06914618BB6532FC6305AF317C803C67E001603A2DA865A7B759CA42DCE62D0E450B411A22E15B4A9839E86A24D1396698BB51CA339147DE4144A576BBC075DAAF04F5B548558D0A128C207932A18A63AFAF4B7ADA82303D42A558C0D70D1A830284823B912D265EE99FB28945301DD60E5BE5B81010E6D8C40911E2ABCB4611F2D7E62E891FDC0753123234D3870E8ABC25677022CAF3F68ED19D6B8E6FC789405CE9DAEA990848FDA5AF4E8A1B5D3C8C1233A37E3DE7D9C5ADE07CFF14CA3D4F13A74F837088F576788D8A82CB049D9B86B2BE736F98A1EA19002DC69ED29CFEBA35E712004EA352AC4F4E2F6C852907F12257800CC85861288D6916B0052A5EA4AED4B8969EABF494291BBA71685B1F7F2181AE89E70ABA1711D6022934F6404031CC97886800216B603589A5CA4055B1BA70E60EBB41A2DD44AC01980F26A040229AA280340297E108209C4271AC09A066C35D84A8E4060A84C12003062425E7D4D3A1E534B717BA0B8B5351FCD34A873AB5DDA239B0F6318408DDC117D76FC306D48C0DD70038C5F7954603E2C6047DEC819DDC0A1E30106488563EF21F3F73EC843561F1C988F0E386C4BB9A8192FE8F11F6496F9CB251443065DDA66A7B6FB904537B6059F741DB38AAD156E5683A3D57DA803B3B22AAD581EB08DFBD5C501CB8CA5D6E11A5218FCAC83B0BC2A8DD94C1AC873E8E23BEC451AC13FC8C0824C1CEF54A2468E994A900FD1C58BD88B4A82A75041253A18EF546AA594994EB03FD1CDA3D88B5692D7701871AB4817371308702C3AB8167B9186771F2AB8A81E88077D44538A20750439A1F46E458336D13B124D64EBC200EC25B6C0B4AB7C8A46AFA2593D18FD88FE352F982B268FDAE868B47635324390767A1A6AE89C8B033001943D2253C5E46EB4753832030057A9AD5F71E095CFC7EBCBE4507B1CCD3E4706F5664FAC2102E8623499B5DD87AC5A164ADFA3D1FBD86DBCBED89EE626347EC5A6EC7851BD14557F385E289E943AFE10ACD764D5B62DEB2FB3EBEA7DA9B31FAEDD1F605A5530164B8E81442F68D3539166C13D124AAB6CC88B28CB499650701B9048E2B3702555E3BDA80A470DED8B7794CA3345DD37B43EF99B75D69287B6F6E0DD4A4BBB0B3C1C92DA538E0C490A436E38232F7B05719001F95858646E56893AAA42DDBA4AE263DB575FEC2170F790B280B8027B787C2A120B902FB187D8E47EB1C09A8F0E709A47973840CD57179AD197987882D1AFF69068383D0B877EB387C2BFC8C4711357620FB1BD699585D67EB5874403275938AA604A22C584F5252EDF85B47E05492A8A032B61D1BA493B8A8BE6E4C45D60A89B0E2332EAB77C048ECB9CD8C30FDBFA17156D44180B4D1D27B63586536C4AAC788D9CB2B9B319D86A180EABAE38E0E63395C37D7510A6A7D6F8076B38DA7025F6109957695870CCE76DAEA4E6351A5E9BD41F27B38E34BE2E3BB90D9F40D8886D55CB9735B5FBA6A29B7EDB9A0E8177EF964A24FFDA498B40CD5E58FE7B63F9E1F70D4C5A0C0F4A992DB3B585A8F227DA391A84B8A30E1E071304E5F214D53BA8D955ADDB1BFD5918ED57872D2273453FB74D64BEEFD6AE42B958ABFBF9B9A55A7D7284C15CF12E0163CA1C961B770B3FB7E2B8120781C25FB5CFC915BEC8014BF6427D0E49B6A0133C0545E11A0EBC2D5DA1CF71B8546A0F19B84C9F050D1477800DE02C96B9AC23E9BE7D7E4149C5F6B089E493A551FB7532CA0208AAE8A839E4E05277DD61016318ED41AF711567CB51D6B7D7707382BEFDEC08ABBE395102567F9F241729E3629CB9A88A25EEC7450A18C3F28170D1352FAEB8222703B4BDCE5A3041DB824972842AC0C59921CAC8E87EFC008350119DDE2ACDD25B7599B52FA6DAD2848931151DA74B08B1779F2C1380A91E5C76B7F3B734E1407643C739975320DCA7DD02C654DD3A80929E9A7EF622817B4A5F37C9DBCF9CB35FCE5B9A101ABAD3714AEA1C1FF7C950351CF06C571688CCE7F11D8BDC95BBDCD0D80227BC4070ECF7E7A045F89030F0D8AB0E7DD59D6CD55514A757C0A1300965835DB15C8CAC4C0E2B76D65E76C4F4EC8894F2D61A2BA430083784C4783DE7F963328594A7338A99234576D326651375268FEAEA33A70993B2937666B654893D36FEFCA6B2B3E70520A13E0DC8969E82A2178182EF06B94DB53EC5A8239ECE789DA54918111C669739B926BEB9ABD66DF4DED987260959B10FADECEC72B198163E3F6ABAECC3E75E59E2E9E203D849366A33A9AC18A9AD6E19C962313162DA5847161A8A6FC43CB48E0AC759F75971CC586C52A79458F1485DD7D59D6731155C0ADD74250D979E6789A69BCBF23B11566DEAA12A84B52EB60853056D493EB16CCB8247CC94FC3E040D9B7DA9C9A151EC63AA42BBE90672343B92587765A89BE490733E77663F0366925A1C2E2896ACFE0001A09D264374CB0B59930EFB5DAC692899D6E8A2A6153D980DEACCDBCE9B4606A0070651A7F43E5B6BC140C2DE3CC7E72A6BBCEFB48A4D4C323075503273C7455D83F2A046A0ECE849491AE5507DCDBB46FB7035EC0E56D4249E909601B2CC2735E79DB58B94AC2E5669CE4CEA2FCDEF2659BD4E14E732D84B6A907CF4920A799DB42E668E5755E6331A9E83E9FC941768B5472AEC5DFF2B3E8BA3F2708856F8409EF74279513D26353FDCDF7F339F9DC65190577706D439F16FC53B3BAD92E40F8E48923C0A570BB1B97BAA3D8192E721F744287370C959A4E33F235ADE7D687C28B4C7E3B4C9B7205B3E0419F76EE6D14FCE30B9940D0DE8D7FBFB2CECAEEF6F86F8EF4278A3D4F2D9502F609ABC76669ADC40B409EDFD50A16924202276CFB67269EC159CDBC81D4E9B88D21D177A1CC640906EF5BD4C42F47832FF77D9E4EDECF2EF5FAA56AF669F322C79DECEF667FF6DE8D8FAB562387F7C67577E9D8BBE6D8EF3B9A25BA7831BCFD07643708DE478D95986A9A2C77879FE9FABE0F1F75E944E37481A552301B4E1203EC7DC034026CBDC03349F8BA5C931EF2EE51BFB5554F2166BAE6ECB2F39C63838387CA31F91BDDC06DF737B4EAB9027DC8F2F46E0E846607F3DB81B6B4976013DDF8574F8B290767121F9D87A5CB5D1936E9664D3D0C1940488D0FA02DDBA6F1AAABAEFB1F4B5D9EF965200707519A5002012D5A2CD8ABC6DEABC7639B96F5899347A1FC6ED2E6C8DAC44439579EFC1FE15D3EE7B3828B8647B0FA809A9F61E2072A9F67EE17921A19C53DF1D169044DF8FDFC5E4F9EEA801D9F2FDACBC8EBB416B416D08781E43540F62B0D193571FFBE8366FDD17B43A31CE0338286AACC3A6400633C2FEC0102D6DC77BF011BB99FF6006E96D2E0889ED3E24319BD8FEC2319AE0153B8651C57578E42DDA8567DE1A886527C81090BA10E25D3A6028C018815F7541783BEB0B19623764BB7B7946DE3143AAFFCEB287A3ABCC66F17735979CF54A579D32115FC5D09B0528BA6B67F99449BFEF296F2CBC9976F1146CEABD0F0708937B3FB1F3C4DD10D1578A7C7C3B9EBFD264D5CBB5C18403F322D9B2F3B9C6DA35EAE67EA8F009200BDF6EB294E9F4DE666AA408A5A9CF9122C3C86E96348942DEE669C4B8A089CD1513484CA58BEED56BE9AD3D8A37F7F0FD874D5C446467883BC4B2590A0DFF949CA3189BBFB3AA01563341BE0CE4ABA7CA006A55FF4DB41D8B41FB91C7E10F1268CC332823664440CE0BF2220B22F9E2982B6CFB2CA37510F36316AA595A3F643C0D40B1E41CAD5142CC1A7174367DE972E91AC002654DE3E762CC0D1C63BA95467EE79B9F34FA6D1CBE69153C8044F97910DE01A38C06621EE59D42CE46CBE8FC032586F0AF9972D306CCD733E31BDB591C976794B9B223F04B797F11F8AAA0F004BBC029E4C328AC62AD1ABDB108F88488771E71507FEAABAAC6621038AA734A9AC88A45774FFB5873E3B6F58EFE5AB689C812BABF15BB1F52E18C234D14F7E74D549C4CDB38D906978C6296B831C9F66C12F124D4D7E6797F6FEF409A30068E1073C90114CB767C4B6C7E9F69723B63C5159403CCA6814FE4902F1636503A0CAF74984138CBDF9A696C9E6601D8C6EA82B32D3010F4C2C8E80C54C56DA918A82E7D8E0CA47A95658718E80A7CA675141672D870ED2EBBB8ECB3A6C519C04309A34B963216432558AAC2E7C228168FFB4C58AC34D7468ECF2F169BACDDE50ADBADCE964587E116C8099D12B647CF320A0372CE789B22D5E59D93DC0E19AEEA9C842B653B1C33923BC58D5DB6E950812E3C1D5FD908C1DC2C60B1E8B9A820E3337653D346C00DA8265B5369674236E60ED8AF8677CAA661BC5ADC003B02B7703798AAE6B40E7A6667927E72B455C73BE1533E8CB6BD033EEDEDACE3CDF59674873513EDAEAE70E0B93175C43BFEEE53264985BBA754E289FA4A5BCE5A9DCFDAC049610353DD737A320F6F491062157A49CB24AEE061B7718A12F4B60882DF961A7A284D550978F915825B169890A6CE3A19675A02A24C0B8D1813D908A04C3EC33893120354690DCBD329D600E755AC64E8163852913A06EA405D03D51C3AAFDDF1DACEEB3AA6CEEB6A0E9D57268FB6EFAA8AA9EBAA96A167D12297FA152B40BD8A750C7DCA197D72B7401DA867A09A25A90D643693D88ABC547949FDD002A8075A66275A60A1C5166A044CF5BA8985905189C6BA44256AACE0337E0285DE50F6D114CBBDF0D77D2B02F5674C2D419780E108A09FAE198F4A9571AD443556BD62A65251C2ADE536430422CBA1619A9F4585CEA1589C611D05B563E6567CC0D5F3806B179969C0F055FAA22F8DC1184075F4817231BCC008D531BEC0068B1B9B6C137463F4AE836A58493128C509A10706B52246D7611996A0E989DB5E7326086DEEF55D9FC3532F38D393B93D56DBB08353C4A50143B48960F3A03214F678D5DC6064F71F3E7D03D83C7C2854CBD74094FE436EA7A0789CD83F51E8CBB666A240E147831385DBC1289EDCF54F942650C6822C7050CD208471D00D1E8850C584585000081E199C2FD85D0FFCCC990FDD40DFDA837402742C35C8B02D744A97D9968FF1A1A9363EF9D85F27889B26E15D4ABF43556B79F353923DF4FCF08304DF889447693C591D848561E70CF46C657F42406F22CA74309D1B6A4E0E01A103099CAD8A2EFEA93E79F8EA8330F014834116F6E98CB215E11EA2538E692496B6268D66A0D2436D4DD9F1A27212D51FF04FE941B6E3C5E74D42AEE2A97E9DA33CBA6F419027E612B4E40E479A3A97C95D4A0F69048C6815E19A8A0FA808C2A0084EB322BA0B96052E26F7DE962BB6F45B92DB976F5178997CDA14EB4D81878C56B7F1134B0C72D6A3EBFF7821E17CFCA9BCD93EF731048C66446E2FFA94FC7913C56183F705705F8602043944AA2FA4227359908BA9EE9F1A481FD3C412504DBEE6ECEB06ADD63106967F4AAE836FA80B6E9861DFA3FB60F974D5DC45AA02629E089EECC7E751709F05ABBC86D1B6C73F310F87ABC73FFE3F9FF5D871D2190100 , N'6.1.0-30225')



