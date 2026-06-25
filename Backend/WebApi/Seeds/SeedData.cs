using BusinessObject;
using BusinessObject.Entities;
using BusinessObject.Enums;
using Infrastructure.Helpers;

namespace WebApi.Seeds;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AIWorkspaceContext>();

        // ---------------------------------------------------------------
        // STEP 0: Prompt user before deleting existing data
        // ---------------------------------------------------------------
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                   DATA SEED CONFIRMATION                 ║");
        Console.WriteLine("╠══════════════════════════════════════════════════════════╣");
        Console.WriteLine("║  This will DELETE all existing data and re-seed with     ║");
        Console.WriteLine("║  sample data. This action CANNOT be undone.              ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
        Console.ResetColor();
        Console.Write("Press Enter to continue, or type 'no' to skip: ");
        var input = Console.ReadLine()?.Trim().ToLowerInvariant();
        if (input == "no" || input == "n")
        {
            Console.WriteLine("Data seeding cancelled. No changes were made.");
            return;
        }

        Console.WriteLine("Deleting existing data...");
        context.AiInteractions.RemoveRange(context.AiInteractions);
        context.Attachments.RemoveRange(context.Attachments);
        context.Comments.RemoveRange(context.Comments);
        context.Documents.RemoveRange(context.Documents);
        context.TaskItems.RemoveRange(context.TaskItems);
        context.Projects.RemoveRange(context.Projects);
        context.TeamMembers.RemoveRange(context.TeamMembers);
        context.Teams.RemoveRange(context.Teams);
        context.RefreshTokens.RemoveRange(context.RefreshTokens);
        context.Users.RemoveRange(context.Users);
        context.SaveChanges();
        Console.WriteLine("Existing data cleared. Seeding new data...");

        // ---------------------------------------------------------------
        // Users (10 users)
        // ---------------------------------------------------------------
        var passwordHash = PasswordHelper.Hash("Password123@");

        var userKhang = new User
        {
            Id = Guid.Parse("A1B2C3D4-E5F6-7890-ABCD-EF1234567890"),
            Name = "Khang Ta",
            Email = "khang.ta@aiworkspace.com",
            PasswordHash = passwordHash,
            AvatarUrl = "https://api.dicebear.com/7.x/avataaars/svg?seed=khang",
            Language = LanguageDisplay.Vi,
            CreatedAt = new DateTime(2025, 1, 15, 0, 0, 0, DateTimeKind.Utc),
        };

        var userMinh = new User
        {
            Id = Guid.Parse("B2C3D4E5-F6A7-8901-BCDE-F12345678901"),
            Name = "Minh Nguyen",
            Email = "minh.nguyen@aiworkspace.com",
            PasswordHash = passwordHash,
            AvatarUrl = "https://api.dicebear.com/7.x/avataaars/svg?seed=minh",
            Language = LanguageDisplay.Vi,
            CreatedAt = new DateTime(2025, 2, 1, 0, 0, 0, DateTimeKind.Utc),
        };

        var userThao = new User
        {
            Id = Guid.Parse("C3D4E5F6-A7B8-9012-CDEF-123456789012"),
            Name = "Thao Tran",
            Email = "thao.tran@aiworkspace.com",
            PasswordHash = passwordHash,
            AvatarUrl = "https://api.dicebear.com/7.x/avataaars/svg?seed=thao",
            Language = LanguageDisplay.Vi,
            CreatedAt = new DateTime(2025, 3, 10, 0, 0, 0, DateTimeKind.Utc),
        };

        var userHoang = new User
        {
            Id = Guid.Parse("D4E5F6A7-B8C9-0123-DEF1-234567890123"),
            Name = "Hoang Pham",
            Email = "hoang.pham@aiworkspace.com",
            PasswordHash = passwordHash,
            AvatarUrl = "https://api.dicebear.com/7.x/avataaars/svg?seed=hoang",
            Language = LanguageDisplay.Vi,
            CreatedAt = new DateTime(2025, 4, 5, 0, 0, 0, DateTimeKind.Utc),
        };

        var userAlex = new User
        {
            Id = Guid.Parse("E5F6A7B8-C9D0-1234-EF12-345678901234"),
            Name = "Alex Jones",
            Email = "alex.jones@aiworkspace.com",
            PasswordHash = passwordHash,
            AvatarUrl = "https://api.dicebear.com/7.x/avataaars/svg?seed=alex",
            Language = LanguageDisplay.En,
            CreatedAt = new DateTime(2025, 5, 20, 0, 0, 0, DateTimeKind.Utc),
        };

        var userLinh = new User
        {
            Id = Guid.Parse("F6A7B8C9-D0E1-2345-F123-456789012345"),
            Name = "Linh Vu",
            Email = "linh.vu@aiworkspace.com",
            PasswordHash = passwordHash,
            AvatarUrl = "https://api.dicebear.com/7.x/avataaars/svg?seed=linh",
            Language = LanguageDisplay.Vi,
            CreatedAt = new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc),
        };

        var userHuy = new User
        {
            Id = Guid.Parse("0708090A-0B0C-0D0E-0F10-111213141516"),
            Name = "Huy Le",
            Email = "huy.le@aiworkspace.com",
            PasswordHash = passwordHash,
            AvatarUrl = "https://api.dicebear.com/7.x/avataaars/svg?seed=huy",
            Language = LanguageDisplay.Vi,
            CreatedAt = new DateTime(2025, 7, 15, 0, 0, 0, DateTimeKind.Utc),
        };

        var userMai = new User
        {
            Id = Guid.Parse("1718191A-1B1C-1D1E-1F20-212223242526"),
            Name = "Mai Phan",
            Email = "mai.phan@aiworkspace.com",
            PasswordHash = passwordHash,
            AvatarUrl = "https://api.dicebear.com/7.x/avataaars/svg?seed=mai",
            Language = LanguageDisplay.Vi,
            CreatedAt = new DateTime(2025, 8, 1, 0, 0, 0, DateTimeKind.Utc),
        };

        var userBao = new User
        {
            Id = Guid.Parse("2728292A-2B2C-2D2E-2F30-313233343536"),
            Name = "Bao Nguyen",
            Email = "bao.nguyen@aiworkspace.com",
            PasswordHash = passwordHash,
            AvatarUrl = "https://api.dicebear.com/7.x/avataaars/svg?seed=bao",
            Language = LanguageDisplay.Vi,
            CreatedAt = new DateTime(2025, 8, 15, 0, 0, 0, DateTimeKind.Utc),
        };

        var userChi = new User
        {
            Id = Guid.Parse("3738393A-3B3C-3D3E-3F40-414243444546"),
            Name = "Chi Pham",
            Email = "chi.pham@aiworkspace.com",
            PasswordHash = passwordHash,
            AvatarUrl = "https://api.dicebear.com/7.x/avataaars/svg?seed=chi",
            Language = LanguageDisplay.Vi,
            CreatedAt = new DateTime(2025, 9, 1, 0, 0, 0, DateTimeKind.Utc),
        };

        context.Users.AddRange(
            userKhang,
            userMinh,
            userThao,
            userHoang,
            userAlex,
            userLinh,
            userHuy,
            userMai,
            userBao,
            userChi
        );
        context.SaveChanges();
        Console.WriteLine("10 users seeded.");

        // ---------------------------------------------------------------
        // Teams (4 teams)
        // ---------------------------------------------------------------
        var teamAlpha = new Team
        {
            Id = Guid.Parse("A1000000-0000-0000-0000-000000000001"),
            Name = "Alpha Team",
            Description =
                "AI Research & Development team focused on cutting-edge machine learning models, NLP solutions, and computer vision technologies.",
        };

        var teamBeta = new Team
        {
            Id = Guid.Parse("A1000000-0000-0000-0000-000000000002"),
            Name = "Beta Team",
            Description =
                "AI Application Engineering team specializing in building production-ready AI-powered applications, chatbots, and data analytics platforms.",
        };

        var teamGamma = new Team
        {
            Id = Guid.Parse("A1000000-0000-0000-0000-000000000003"),
            Name = "Gamma Team",
            Description =
                "Data Engineering & Infrastructure team focused on data pipelines, MLOps, CI/CD, and scalable infrastructure for AI systems.",
        };

        var teamDelta = new Team
        {
            Id = Guid.Parse("A1000000-0000-0000-0000-000000000004"),
            Name = "Delta Team",
            Description =
                "AI Product & Design team specializing in product strategy, UI/UX design, user research, and AI product development lifecycle.",
        };

        context.Teams.AddRange(teamAlpha, teamBeta, teamGamma, teamDelta);
        context.SaveChanges();
        Console.WriteLine("4 teams seeded.");

        // ---------------------------------------------------------------
        // Team Members (20 members: 5 per team)
        // ---------------------------------------------------------------
        var teamMembers = new List<TeamMember>
        {
            // Alpha Team: Khang (Admin), Minh (Leader), Thao (Member), Hoang (Member), Bao (Member)
            new()
            {
                Id = Guid.Parse("B1000000-0000-0000-0000-000000000001"),
                TeamId = teamAlpha.Id,
                UserId = userKhang.Id,
                Role = TeamMemberRole.Admin,
                JoinedAt = new DateTime(2025, 1, 20, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("B1000000-0000-0000-0000-000000000002"),
                TeamId = teamAlpha.Id,
                UserId = userMinh.Id,
                Role = TeamMemberRole.Leader,
                JoinedAt = new DateTime(2025, 2, 5, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("B1000000-0000-0000-0000-000000000003"),
                TeamId = teamAlpha.Id,
                UserId = userThao.Id,
                Role = TeamMemberRole.Member,
                JoinedAt = new DateTime(2025, 3, 15, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("B1000000-0000-0000-0000-000000000004"),
                TeamId = teamAlpha.Id,
                UserId = userHoang.Id,
                Role = TeamMemberRole.Member,
                JoinedAt = new DateTime(2025, 4, 10, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("B1000000-0000-0000-0000-000000000005"),
                TeamId = teamAlpha.Id,
                UserId = userBao.Id,
                Role = TeamMemberRole.Member,
                JoinedAt = new DateTime(2025, 8, 20, 0, 0, 0, DateTimeKind.Utc),
            },
            // Beta Team: Alex (Admin), Linh (Leader), Khang (Member), Thao (Member), Chi (Member)
            new()
            {
                Id = Guid.Parse("B1000000-0000-0000-0000-000000000006"),
                TeamId = teamBeta.Id,
                UserId = userAlex.Id,
                Role = TeamMemberRole.Admin,
                JoinedAt = new DateTime(2025, 5, 25, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("B1000000-0000-0000-0000-000000000007"),
                TeamId = teamBeta.Id,
                UserId = userLinh.Id,
                Role = TeamMemberRole.Leader,
                JoinedAt = new DateTime(2025, 6, 5, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("B1000000-0000-0000-0000-000000000008"),
                TeamId = teamBeta.Id,
                UserId = userKhang.Id,
                Role = TeamMemberRole.Member,
                JoinedAt = new DateTime(2025, 6, 10, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("B1000000-0000-0000-0000-000000000009"),
                TeamId = teamBeta.Id,
                UserId = userThao.Id,
                Role = TeamMemberRole.Member,
                JoinedAt = new DateTime(2025, 6, 12, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("B1000000-0000-0000-0000-00000000000A"),
                TeamId = teamBeta.Id,
                UserId = userChi.Id,
                Role = TeamMemberRole.Member,
                JoinedAt = new DateTime(2025, 9, 10, 0, 0, 0, DateTimeKind.Utc),
            },
            // Gamma Team: Huy (Admin), Alex (Leader), Khang (Member), Minh (Member), Bao (Member)
            new()
            {
                Id = Guid.Parse("B1000000-0000-0000-0000-00000000000B"),
                TeamId = teamGamma.Id,
                UserId = userHuy.Id,
                Role = TeamMemberRole.Admin,
                JoinedAt = new DateTime(2025, 7, 20, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("B1000000-0000-0000-0000-00000000000C"),
                TeamId = teamGamma.Id,
                UserId = userAlex.Id,
                Role = TeamMemberRole.Leader,
                JoinedAt = new DateTime(2025, 8, 1, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("B1000000-0000-0000-0000-00000000000D"),
                TeamId = teamGamma.Id,
                UserId = userKhang.Id,
                Role = TeamMemberRole.Member,
                JoinedAt = new DateTime(2025, 8, 5, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("B1000000-0000-0000-0000-00000000000E"),
                TeamId = teamGamma.Id,
                UserId = userMinh.Id,
                Role = TeamMemberRole.Member,
                JoinedAt = new DateTime(2025, 8, 10, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("B1000000-0000-0000-0000-00000000000F"),
                TeamId = teamGamma.Id,
                UserId = userBao.Id,
                Role = TeamMemberRole.Member,
                JoinedAt = new DateTime(2025, 9, 5, 0, 0, 0, DateTimeKind.Utc),
            },
            // Delta Team: Mai (Admin), Linh (Leader), Thao (Member), Hoang (Member), Chi (Member)
            new()
            {
                Id = Guid.Parse("B1000000-0000-0000-0000-000000000010"),
                TeamId = teamDelta.Id,
                UserId = userMai.Id,
                Role = TeamMemberRole.Admin,
                JoinedAt = new DateTime(2025, 8, 15, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("B1000000-0000-0000-0000-000000000011"),
                TeamId = teamDelta.Id,
                UserId = userLinh.Id,
                Role = TeamMemberRole.Leader,
                JoinedAt = new DateTime(2025, 8, 20, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("B1000000-0000-0000-0000-000000000012"),
                TeamId = teamDelta.Id,
                UserId = userThao.Id,
                Role = TeamMemberRole.Member,
                JoinedAt = new DateTime(2025, 8, 25, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("B1000000-0000-0000-0000-000000000013"),
                TeamId = teamDelta.Id,
                UserId = userHoang.Id,
                Role = TeamMemberRole.Member,
                JoinedAt = new DateTime(2025, 8, 30, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("B1000000-0000-0000-0000-000000000014"),
                TeamId = teamDelta.Id,
                UserId = userChi.Id,
                Role = TeamMemberRole.Member,
                JoinedAt = new DateTime(2025, 9, 15, 0, 0, 0, DateTimeKind.Utc),
            },
        };

        context.TeamMembers.AddRange(teamMembers);
        context.SaveChanges();
        Console.WriteLine("20 team members seeded.");

        // ---------------------------------------------------------------
        // Projects (8 projects: 2 per team)
        // ---------------------------------------------------------------
        var projectNlp = new Project
        {
            Id = Guid.Parse("C1000000-0000-0000-0000-000000000001"),
            TeamId = teamAlpha.Id,
            CreatorId = userKhang.Id,
            Name = "Natural Language Processing Pipeline",
            Description =
                "Build an end-to-end NLP pipeline for text classification, sentiment analysis, and named entity recognition using transformer-based models.",
            Visibility = ProjectVisibility.Public,
        };

        var projectSpeech = new Project
        {
            Id = Guid.Parse("C1000000-0000-0000-0000-000000000002"),
            TeamId = teamAlpha.Id,
            CreatorId = userMinh.Id,
            Name = "Speech Recognition System",
            Description =
                "Develop a multilingual speech recognition system supporting Vietnamese and English, with real-time transcription and speaker diarization capabilities.",
            Visibility = ProjectVisibility.Public,
        };

        var projectChatbot = new Project
        {
            Id = Guid.Parse("C1000000-0000-0000-0000-000000000003"),
            TeamId = teamBeta.Id,
            CreatorId = userAlex.Id,
            Name = "AI Chatbot Assistant",
            Description =
                "Create an intelligent chatbot assistant powered by LLMs to handle customer inquiries, ticket routing, and FAQ automation.",
            Visibility = ProjectVisibility.Private,
        };

        var projectRecommendation = new Project
        {
            Id = Guid.Parse("C1000000-0000-0000-0000-000000000004"),
            TeamId = teamBeta.Id,
            CreatorId = userLinh.Id,
            Name = "AI Recommendation Engine",
            Description =
                "Build a personalized recommendation engine using collaborative filtering, content-based filtering, and deep learning models for e-commerce platforms.",
            Visibility = ProjectVisibility.Public,
        };

        var projectMlops = new Project
        {
            Id = Guid.Parse("C1000000-0000-0000-0000-000000000005"),
            TeamId = teamGamma.Id,
            CreatorId = userHuy.Id,
            Name = "MLOps Platform",
            Description =
                "Design and implement a comprehensive MLOps platform for model versioning, experiment tracking, automated retraining, and deployment orchestration.",
            Visibility = ProjectVisibility.Private,
        };

        var projectDataPipeline = new Project
        {
            Id = Guid.Parse("C1000000-0000-0000-0000-000000000006"),
            TeamId = teamGamma.Id,
            CreatorId = userAlex.Id,
            Name = "Real-Time Data Pipeline",
            Description =
                "Build a scalable real-time data ingestion and processing pipeline using Apache Kafka, Flink, and ClickHouse for streaming analytics.",
            Visibility = ProjectVisibility.Public,
        };

        var projectProductPortal = new Project
        {
            Id = Guid.Parse("C1000000-0000-0000-0000-000000000007"),
            TeamId = teamDelta.Id,
            CreatorId = userMai.Id,
            Name = "AI Product Portal",
            Description =
                "Design and develop a unified product portal showcasing all AI products with documentation, demos, pricing, and self-service onboarding.",
            Visibility = ProjectVisibility.Public,
        };

        var projectUserResearch = new Project
        {
            Id = Guid.Parse("C1000000-0000-0000-0000-000000000008"),
            TeamId = teamDelta.Id,
            CreatorId = userLinh.Id,
            Name = "User Research & Analytics Program",
            Description =
                "Conduct user research, usability testing, and behavioral analytics to drive data-informed product decisions and improve user experience across AI products.",
            Visibility = ProjectVisibility.Public,
        };

        context.Projects.AddRange(
            projectNlp,
            projectSpeech,
            projectChatbot,
            projectRecommendation,
            projectMlops,
            projectDataPipeline,
            projectProductPortal,
            projectUserResearch
        );
        context.SaveChanges();
        Console.WriteLine("8 projects seeded.");

        // ---------------------------------------------------------------
        // Project Members (24 members: 3 per project)
        // ---------------------------------------------------------------
        var projectMembers = new List<ProjectMember>
        {
            // NLP Pipeline: Khang, Minh, Thao
            new()
            {
                Id = Guid.Parse("D1000000-0000-0000-0000-000000000001"),
                ProjectId = projectNlp.Id,
                UserId = userKhang.Id,
                JoinedAt = new DateTime(2025, 2, 1, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("D1000000-0000-0000-0000-000000000002"),
                ProjectId = projectNlp.Id,
                UserId = userMinh.Id,
                JoinedAt = new DateTime(2025, 2, 1, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("D1000000-0000-0000-0000-000000000003"),
                ProjectId = projectNlp.Id,
                UserId = userThao.Id,
                JoinedAt = new DateTime(2025, 3, 15, 0, 0, 0, DateTimeKind.Utc),
            },
            // Speech Recognition: Minh, Thao, Hoang
            new()
            {
                Id = Guid.Parse("D1000000-0000-0000-0000-000000000004"),
                ProjectId = projectSpeech.Id,
                UserId = userMinh.Id,
                JoinedAt = new DateTime(2025, 4, 1, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("D1000000-0000-0000-0000-000000000005"),
                ProjectId = projectSpeech.Id,
                UserId = userThao.Id,
                JoinedAt = new DateTime(2025, 4, 5, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("D1000000-0000-0000-0000-000000000006"),
                ProjectId = projectSpeech.Id,
                UserId = userHoang.Id,
                JoinedAt = new DateTime(2025, 4, 10, 0, 0, 0, DateTimeKind.Utc),
            },
            // AI Chatbot: Alex, Linh, Thao
            new()
            {
                Id = Guid.Parse("D1000000-0000-0000-0000-000000000007"),
                ProjectId = projectChatbot.Id,
                UserId = userAlex.Id,
                JoinedAt = new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("D1000000-0000-0000-0000-000000000008"),
                ProjectId = projectChatbot.Id,
                UserId = userLinh.Id,
                JoinedAt = new DateTime(2025, 6, 5, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("D1000000-0000-0000-0000-000000000009"),
                ProjectId = projectChatbot.Id,
                UserId = userThao.Id,
                JoinedAt = new DateTime(2025, 6, 12, 0, 0, 0, DateTimeKind.Utc),
            },
            // Recommendation Engine: Linh, Alex, Chi
            new()
            {
                Id = Guid.Parse("D1000000-0000-0000-0000-00000000000A"),
                ProjectId = projectRecommendation.Id,
                UserId = userLinh.Id,
                JoinedAt = new DateTime(2025, 7, 1, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("D1000000-0000-0000-0000-00000000000B"),
                ProjectId = projectRecommendation.Id,
                UserId = userAlex.Id,
                JoinedAt = new DateTime(2025, 7, 5, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("D1000000-0000-0000-0000-00000000000C"),
                ProjectId = projectRecommendation.Id,
                UserId = userChi.Id,
                JoinedAt = new DateTime(2025, 9, 10, 0, 0, 0, DateTimeKind.Utc),
            },
            // MLOps Platform: Huy, Alex, Khang
            new()
            {
                Id = Guid.Parse("D1000000-0000-0000-0000-00000000000D"),
                ProjectId = projectMlops.Id,
                UserId = userHuy.Id,
                JoinedAt = new DateTime(2025, 8, 1, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("D1000000-0000-0000-0000-00000000000E"),
                ProjectId = projectMlops.Id,
                UserId = userAlex.Id,
                JoinedAt = new DateTime(2025, 8, 5, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("D1000000-0000-0000-0000-00000000000F"),
                ProjectId = projectMlops.Id,
                UserId = userKhang.Id,
                JoinedAt = new DateTime(2025, 8, 10, 0, 0, 0, DateTimeKind.Utc),
            },
            // Real-Time Data Pipeline: Huy, Bao, Minh
            new()
            {
                Id = Guid.Parse("D1000000-0000-0000-0000-000000000010"),
                ProjectId = projectDataPipeline.Id,
                UserId = userHuy.Id,
                JoinedAt = new DateTime(2025, 8, 15, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("D1000000-0000-0000-0000-000000000011"),
                ProjectId = projectDataPipeline.Id,
                UserId = userBao.Id,
                JoinedAt = new DateTime(2025, 8, 20, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("D1000000-0000-0000-0000-000000000012"),
                ProjectId = projectDataPipeline.Id,
                UserId = userMinh.Id,
                JoinedAt = new DateTime(2025, 9, 1, 0, 0, 0, DateTimeKind.Utc),
            },
            // AI Product Portal: Mai, Linh, Chi
            new()
            {
                Id = Guid.Parse("D1000000-0000-0000-0000-000000000013"),
                ProjectId = projectProductPortal.Id,
                UserId = userMai.Id,
                JoinedAt = new DateTime(2025, 9, 1, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("D1000000-0000-0000-0000-000000000014"),
                ProjectId = projectProductPortal.Id,
                UserId = userLinh.Id,
                JoinedAt = new DateTime(2025, 9, 5, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("D1000000-0000-0000-0000-000000000015"),
                ProjectId = projectProductPortal.Id,
                UserId = userChi.Id,
                JoinedAt = new DateTime(2025, 9, 15, 0, 0, 0, DateTimeKind.Utc),
            },
            // User Research: Mai, Hoang, Thao
            new()
            {
                Id = Guid.Parse("D1000000-0000-0000-0000-000000000016"),
                ProjectId = projectUserResearch.Id,
                UserId = userMai.Id,
                JoinedAt = new DateTime(2025, 9, 1, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("D1000000-0000-0000-0000-000000000017"),
                ProjectId = projectUserResearch.Id,
                UserId = userHoang.Id,
                JoinedAt = new DateTime(2025, 9, 5, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("D1000000-0000-0000-0000-000000000018"),
                ProjectId = projectUserResearch.Id,
                UserId = userThao.Id,
                JoinedAt = new DateTime(2025, 9, 10, 0, 0, 0, DateTimeKind.Utc),
            },
        };

        context.ProjectMembers.AddRange(projectMembers);
        context.SaveChanges();
        Console.WriteLine("24 project members seeded.");

        // ---------------------------------------------------------------
        // TaskItems (48 tasks across 8 projects: 6 per project)
        // ---------------------------------------------------------------
        var tasks = new List<TaskItem>
        {
            // --- NLP Pipeline tasks (6 tasks) ---
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-000000000001"),
                ProjectId = projectNlp.Id,
                Title = "Data Collection & Preprocessing",
                Description =
                    "Collect labeled datasets from public sources (IMDB, Twitter, NewsGroups) and implement text cleaning, tokenization, and normalization pipelines.",
                AssignedToId = userThao.Id,
                Priority = 1,
                Status = TaskItemStatus.Done,
                CreatedAt = new DateTime(2025, 2, 5, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2025, 3, 5, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-000000000002"),
                ProjectId = projectNlp.Id,
                Title = "Model Training & Hyperparameter Tuning",
                Description =
                    "Fine-tune BERT and RoBERTa models for text classification and NER tasks. Optimize hyperparameters using Optuna.",
                AssignedToId = userMinh.Id,
                Priority = 1,
                Status = TaskItemStatus.InProgress,
                CreatedAt = new DateTime(2025, 3, 10, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2025, 5, 15, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-000000000003"),
                ProjectId = projectNlp.Id,
                Title = "API Development & Deployment",
                Description =
                    "Build RESTful API endpoints for model inference using FastAPI. Containerize with Docker and deploy to Kubernetes.",
                AssignedToId = userKhang.Id,
                Priority = 2,
                Status = TaskItemStatus.Open,
                CreatedAt = new DateTime(2025, 4, 1, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2025, 6, 30, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-000000000004"),
                ProjectId = projectNlp.Id,
                Title = "Benchmarking & Performance Optimization",
                Description =
                    "Evaluate model performance on standard benchmarks, optimize inference latency, and reduce memory footprint via quantization.",
                AssignedToId = null,
                Priority = 3,
                Status = TaskItemStatus.Open,
                CreatedAt = new DateTime(2025, 5, 1, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2025, 7, 15, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-000000000005"),
                ProjectId = projectNlp.Id,
                Title = "Multilingual Model Expansion",
                Description =
                    "Extend the pipeline to support Vietnamese, Japanese, and French languages. Add language detection and model switching logic.",
                AssignedToId = userBao.Id,
                Priority = 2,
                Status = TaskItemStatus.Open,
                CreatedAt = new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2025, 8, 15, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-000000000006"),
                ProjectId = projectNlp.Id,
                Title = "Documentation & Model Card Generation",
                Description =
                    "Write comprehensive documentation, API reference, and model cards detailing dataset sources, bias analysis, and performance metrics.",
                AssignedToId = null,
                Priority = 3,
                Status = TaskItemStatus.Open,
                CreatedAt = new DateTime(2025, 7, 1, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2025, 9, 1, 0, 0, 0, DateTimeKind.Utc),
            },
            // --- Speech Recognition System tasks (6 tasks) ---
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-000000000007"),
                ProjectId = projectSpeech.Id,
                Title = "Audio Data Collection & Augmentation",
                Description =
                    "Gather multilingual speech datasets (Common Voice, VoxPopuli, Vivos) and implement noise injection, speed perturbation, and SpecAugment for data augmentation.",
                AssignedToId = userThao.Id,
                Priority = 1,
                Status = TaskItemStatus.Done,
                CreatedAt = new DateTime(2025, 4, 15, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2025, 5, 30, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-000000000008"),
                ProjectId = projectSpeech.Id,
                Title = "Acoustic Model Training with Conformer",
                Description =
                    "Train Conformer-based acoustic models using CTC and RNN-T loss functions. Experiment with self-supervised pretraining (wav2vec 2.0, HuBERT).",
                AssignedToId = userMinh.Id,
                Priority = 1,
                Status = TaskItemStatus.InProgress,
                CreatedAt = new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2025, 8, 1, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-000000000009"),
                ProjectId = projectSpeech.Id,
                Title = "Language Model Integration",
                Description =
                    "Integrate n-gram and Transformer-based language models for beam search decoding. Optimize for Vietnamese tone marks and compound words.",
                AssignedToId = userHoang.Id,
                Priority = 2,
                Status = TaskItemStatus.Open,
                CreatedAt = new DateTime(2025, 6, 15, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2025, 8, 15, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-00000000000A"),
                ProjectId = projectSpeech.Id,
                Title = "Speaker Diarization Module",
                Description =
                    "Implement speaker change detection and clustering (pyannote.audio) to identify who spoke when in multi-speaker audio recordings.",
                AssignedToId = null,
                Priority = 2,
                Status = TaskItemStatus.Open,
                CreatedAt = new DateTime(2025, 7, 1, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2025, 9, 1, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-00000000000B"),
                ProjectId = projectSpeech.Id,
                Title = "Real-Time Streaming Inference",
                Description =
                    "Build a low-latency streaming ASR system using WebSockets and chunked audio processing. Support partial results and end-of-utterance detection.",
                AssignedToId = userMinh.Id,
                Priority = 1,
                Status = TaskItemStatus.Open,
                CreatedAt = new DateTime(2025, 7, 15, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2025, 9, 15, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-00000000000C"),
                ProjectId = projectSpeech.Id,
                Title = "Evaluation & Accuracy Benchmarking",
                Description =
                    "Measure WER/CER on standard test sets. Conduct ABX discrimination tests and analyze performance across accents, noise levels, and languages.",
                AssignedToId = userThao.Id,
                Priority = 3,
                Status = TaskItemStatus.Open,
                CreatedAt = new DateTime(2025, 8, 1, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2025, 10, 1, 0, 0, 0, DateTimeKind.Utc),
            },
            // --- AI Chatbot Assistant tasks (6 tasks) ---
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-00000000000D"),
                ProjectId = projectChatbot.Id,
                Title = "LLM Integration & Prompt Engineering",
                Description =
                    "Integrate GPT-4 and Claude APIs. Design system prompts, few-shot examples, and guardrails for safe and accurate responses.",
                AssignedToId = userAlex.Id,
                Priority = 1,
                Status = TaskItemStatus.InProgress,
                CreatedAt = new DateTime(2025, 6, 5, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2025, 7, 15, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-00000000000E"),
                ProjectId = projectChatbot.Id,
                Title = "Conversation Memory & Context Management",
                Description =
                    "Implement short-term and long-term conversation memory using vector databases (Pinecone) and Redis for context retention across sessions.",
                AssignedToId = userLinh.Id,
                Priority = 1,
                Status = TaskItemStatus.Open,
                CreatedAt = new DateTime(2025, 6, 8, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2025, 7, 30, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-00000000000F"),
                ProjectId = projectChatbot.Id,
                Title = "Intent Classification & Entity Extraction",
                Description =
                    "Build intent classification and entity extraction to automatically route support tickets to appropriate departments and trigger follow-up actions.",
                AssignedToId = userThao.Id,
                Priority = 2,
                Status = TaskItemStatus.InProgress,
                CreatedAt = new DateTime(2025, 6, 12, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2025, 8, 10, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-000000000010"),
                ProjectId = projectChatbot.Id,
                Title = "Multi-Channel Integration (Slack, Web, Mobile)",
                Description =
                    "Deploy the chatbot across Slack, web widget, and mobile SDK. Handle message formatting, rate limiting, and channel-specific behaviors.",
                AssignedToId = null,
                Priority = 3,
                Status = TaskItemStatus.Open,
                CreatedAt = new DateTime(2025, 6, 13, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2025, 9, 1, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-000000000011"),
                ProjectId = projectChatbot.Id,
                Title = "Analytics Dashboard & Conversation Logging",
                Description =
                    "Build an analytics dashboard to track user satisfaction, conversation drop-off rates, popular topics, and agent handoff metrics.",
                AssignedToId = userLinh.Id,
                Priority = 2,
                Status = TaskItemStatus.Open,
                CreatedAt = new DateTime(2025, 7, 1, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2025, 9, 15, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-000000000012"),
                ProjectId = projectChatbot.Id,
                Title = "Sentiment Analysis & Escalation Logic",
                Description =
                    "Integrate real-time sentiment analysis to detect frustrated customers and automatically escalate to human agents with conversation context.",
                AssignedToId = userAlex.Id,
                Priority = 3,
                Status = TaskItemStatus.Open,
                CreatedAt = new DateTime(2025, 7, 15, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2025, 10, 1, 0, 0, 0, DateTimeKind.Utc),
            },
            // --- Recommendation Engine tasks (6 tasks) ---
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-000000000013"),
                ProjectId = projectRecommendation.Id,
                Title = "User Behavior Data Pipeline",
                Description =
                    "Design and implement a pipeline to collect, store, and process user interaction events (clicks, views, purchases) using Kafka and ClickHouse.",
                AssignedToId = userLinh.Id,
                Priority = 1,
                Status = TaskItemStatus.InProgress,
                CreatedAt = new DateTime(2025, 7, 10, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2025, 8, 20, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-000000000014"),
                ProjectId = projectRecommendation.Id,
                Title = "Collaborative Filtering Model",
                Description =
                    "Implement matrix factorization (ALS) and neural collaborative filtering for user-item recommendations using implicit feedback signals.",
                AssignedToId = userAlex.Id,
                Priority = 1,
                Status = TaskItemStatus.Open,
                CreatedAt = new DateTime(2025, 7, 15, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2025, 9, 1, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-000000000015"),
                ProjectId = projectRecommendation.Id,
                Title = "Content-Based & Hybrid Approach",
                Description =
                    "Build content-based filtering using item embeddings (text, image, metadata) and combine with collaborative signals for hybrid recommendations.",
                AssignedToId = null,
                Priority = 2,
                Status = TaskItemStatus.Open,
                CreatedAt = new DateTime(2025, 8, 1, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2025, 9, 20, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-000000000016"),
                ProjectId = projectRecommendation.Id,
                Title = "A/B Testing Framework",
                Description =
                    "Set up an A/B testing infrastructure to compare recommendation strategies online. Track conversion rates, CTR, and user engagement metrics.",
                AssignedToId = userChi.Id,
                Priority = 2,
                Status = TaskItemStatus.Open,
                CreatedAt = new DateTime(2025, 8, 15, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2025, 10, 1, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-000000000017"),
                ProjectId = projectRecommendation.Id,
                Title = "Real-Time Personalization API",
                Description =
                    "Build low-latency REST and gRPC APIs for serving personalized recommendations in real-time. Implement caching with Redis and precomputed embeddings.",
                AssignedToId = userAlex.Id,
                Priority = 1,
                Status = TaskItemStatus.Open,
                CreatedAt = new DateTime(2025, 8, 20, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2025, 10, 15, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-000000000018"),
                ProjectId = projectRecommendation.Id,
                Title = "Cold Start & Exploration Strategies",
                Description =
                    "Address cold-start problems for new users and items using popularity-based, demographic, and contextual bandit approaches (explore-exploit).",
                AssignedToId = userLinh.Id,
                Priority = 3,
                Status = TaskItemStatus.Open,
                CreatedAt = new DateTime(2025, 9, 1, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2025, 11, 1, 0, 0, 0, DateTimeKind.Utc),
            },
            // --- MLOps Platform tasks (6 tasks) ---
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-000000000019"),
                ProjectId = projectMlops.Id,
                Title = "MLflow Integration for Experiment Tracking",
                Description =
                    "Set up MLflow server for tracking experiments, metrics, parameters, and artifacts. Integrate with the team's existing training scripts.",
                AssignedToId = userHuy.Id,
                Priority = 1,
                Status = TaskItemStatus.Done,
                CreatedAt = new DateTime(2025, 8, 10, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2025, 9, 1, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-00000000001A"),
                ProjectId = projectMlops.Id,
                Title = "Model Versioning & Registry",
                Description =
                    "Implement model versioning with DVC and a central model registry. Support staging (dev/staging/production) and rollback capabilities.",
                AssignedToId = userAlex.Id,
                Priority = 1,
                Status = TaskItemStatus.InProgress,
                CreatedAt = new DateTime(2025, 8, 15, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2025, 9, 20, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-00000000001B"),
                ProjectId = projectMlops.Id,
                Title = "Automated Retraining Pipeline",
                Description =
                    "Build CI/CD pipelines with GitHub Actions and Jenkins for automated model retraining on new data. Implement data drift detection triggers.",
                AssignedToId = userKhang.Id,
                Priority = 2,
                Status = TaskItemStatus.Open,
                CreatedAt = new DateTime(2025, 9, 1, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2025, 10, 15, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-00000000001C"),
                ProjectId = projectMlops.Id,
                Title = "Model Monitoring & Alerting",
                Description =
                    "Set up Prometheus and Grafana for monitoring model performance in production. Configure alerts for accuracy drops, latency spikes, and data drift.",
                AssignedToId = null,
                Priority = 2,
                Status = TaskItemStatus.Open,
                CreatedAt = new DateTime(2025, 9, 10, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2025, 10, 30, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-00000000001D"),
                ProjectId = projectMlops.Id,
                Title = "Kubernetes Deployment & Scaling",
                Description =
                    "Containerize ML services with Docker and deploy on Kubernetes with auto-scaling, rolling updates, and resource quotas for GPU/CPU workloads.",
                AssignedToId = userHuy.Id,
                Priority = 1,
                Status = TaskItemStatus.Open,
                CreatedAt = new DateTime(2025, 9, 15, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2025, 11, 1, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-00000000001E"),
                ProjectId = projectMlops.Id,
                Title = "Feature Store Implementation",
                Description =
                    "Design and implement a centralized feature store using Feast. Enable feature sharing, point-in-time correct joins, and online/offline serving.",
                AssignedToId = userAlex.Id,
                Priority = 3,
                Status = TaskItemStatus.Open,
                CreatedAt = new DateTime(2025, 9, 20, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2025, 11, 15, 0, 0, 0, DateTimeKind.Utc),
            },
            // --- Real-Time Data Pipeline tasks (6 tasks) ---
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-00000000001F"),
                ProjectId = projectDataPipeline.Id,
                Title = "Kafka Cluster Setup & Schema Registry",
                Description =
                    "Deploy a production-grade Kafka cluster with Confluent Schema Registry. Define Avro schemas for all event types and ensure backward compatibility.",
                AssignedToId = userHuy.Id,
                Priority = 1,
                Status = TaskItemStatus.InProgress,
                CreatedAt = new DateTime(2025, 9, 1, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2025, 10, 1, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-000000000020"),
                ProjectId = projectDataPipeline.Id,
                Title = "Stream Processing with Flink",
                Description =
                    "Implement stream processing jobs using Apache Flink for real-time aggregations, windowed operations, and event-time processing with exactly-once semantics.",
                AssignedToId = userBao.Id,
                Priority = 1,
                Status = TaskItemStatus.Open,
                CreatedAt = new DateTime(2025, 9, 5, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2025, 10, 15, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-000000000021"),
                ProjectId = projectDataPipeline.Id,
                Title = "ClickHouse Data Warehouse Setup",
                Description =
                    "Deploy and configure ClickHouse clusters for real-time analytical queries. Design OLAP-friendly table schemas with proper partitioning and ordering keys.",
                AssignedToId = userMinh.Id,
                Priority = 2,
                Status = TaskItemStatus.Open,
                CreatedAt = new DateTime(2025, 9, 10, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2025, 10, 20, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-000000000022"),
                ProjectId = projectDataPipeline.Id,
                Title = "Data Quality Monitoring & Alerting",
                Description =
                    "Implement data quality checks (schema validation, null ratio, freshness) using Great Expectations. Configure alerts for pipeline failures and data anomalies.",
                AssignedToId = null,
                Priority = 2,
                Status = TaskItemStatus.Open,
                CreatedAt = new DateTime(2025, 9, 15, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2025, 11, 1, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-000000000023"),
                ProjectId = projectDataPipeline.Id,
                Title = "WebSocket Push Service for Live Dashboards",
                Description =
                    "Build a WebSocket gateway that pushes real-time aggregated data from Flink/ClickHouse to live dashboards with sub-second latency.",
                AssignedToId = userBao.Id,
                Priority = 1,
                Status = TaskItemStatus.Open,
                CreatedAt = new DateTime(2025, 9, 20, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2025, 11, 10, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-000000000024"),
                ProjectId = projectDataPipeline.Id,
                Title = "Disaster Recovery & Data Replication",
                Description =
                    "Implement multi-region data replication, backup strategies, and disaster recovery plans to ensure high availability and data durability.",
                AssignedToId = userHuy.Id,
                Priority = 3,
                Status = TaskItemStatus.Open,
                CreatedAt = new DateTime(2025, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2025, 12, 1, 0, 0, 0, DateTimeKind.Utc),
            },
            // --- AI Product Portal tasks (6 tasks) ---
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-000000000025"),
                ProjectId = projectProductPortal.Id,
                Title = "Portal Architecture & Design System",
                Description =
                    "Design the information architecture, component library, and design system for the product portal using Figma and Storybook.",
                AssignedToId = userMai.Id,
                Priority = 1,
                Status = TaskItemStatus.InProgress,
                CreatedAt = new DateTime(2025, 9, 10, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2025, 10, 10, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-000000000026"),
                ProjectId = projectProductPortal.Id,
                Title = "Product Catalog & Demo Pages",
                Description =
                    "Create dynamic product catalog pages with detailed descriptions, interactive demos, code snippets, and API reference documentation.",
                AssignedToId = userChi.Id,
                Priority = 1,
                Status = TaskItemStatus.Open,
                CreatedAt = new DateTime(2025, 9, 15, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2025, 10, 20, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-000000000027"),
                ProjectId = projectProductPortal.Id,
                Title = "Self-Service Onboarding & API Key Management",
                Description =
                    "Implement user registration, product activation, API key generation, and usage tracking dashboards for self-service onboarding.",
                AssignedToId = userLinh.Id,
                Priority = 2,
                Status = TaskItemStatus.Open,
                CreatedAt = new DateTime(2025, 9, 20, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2025, 11, 1, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-000000000028"),
                ProjectId = projectProductPortal.Id,
                Title = "Pricing & Subscription Management",
                Description =
                    "Design and integrate a flexible pricing engine supporting tiered plans, usage-based billing, and subscription management with Stripe.",
                AssignedToId = null,
                Priority = 2,
                Status = TaskItemStatus.Open,
                CreatedAt = new DateTime(2025, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2025, 11, 15, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-000000000029"),
                ProjectId = projectProductPortal.Id,
                Title = "SEO & Analytics Integration",
                Description =
                    "Optimize portal for search engines, implement structured data, and integrate analytics (Google Analytics, Mixpanel) for user behavior tracking.",
                AssignedToId = userChi.Id,
                Priority = 3,
                Status = TaskItemStatus.Open,
                CreatedAt = new DateTime(2025, 10, 10, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2025, 12, 1, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-00000000002A"),
                ProjectId = projectProductPortal.Id,
                Title = "User Feedback & Rating System",
                Description =
                    "Build a feedback system allowing users to rate products, leave reviews, and submit feature requests. Integrate with product roadmap planning.",
                AssignedToId = userMai.Id,
                Priority = 3,
                Status = TaskItemStatus.Open,
                CreatedAt = new DateTime(2025, 10, 15, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2025, 12, 15, 0, 0, 0, DateTimeKind.Utc),
            },
            // --- User Research & Analytics Program tasks (6 tasks) ---
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-00000000002B"),
                ProjectId = projectUserResearch.Id,
                Title = "User Persona Development",
                Description =
                    "Conduct stakeholder interviews and surveys to develop data-driven user personas representing key target segments for AI products.",
                AssignedToId = userMai.Id,
                Priority = 1,
                Status = TaskItemStatus.Done,
                CreatedAt = new DateTime(2025, 9, 5, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2025, 9, 25, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-00000000002C"),
                ProjectId = projectUserResearch.Id,
                Title = "Usability Testing Sessions",
                Description =
                    "Plan and conduct moderated and unmoderated usability testing sessions for NLP Pipeline, Chatbot, and Product Portal. Compile findings and recommendations.",
                AssignedToId = userHoang.Id,
                Priority = 1,
                Status = TaskItemStatus.InProgress,
                CreatedAt = new DateTime(2025, 9, 10, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2025, 10, 20, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-00000000002D"),
                ProjectId = projectUserResearch.Id,
                Title = "Behavioral Analytics Dashboard",
                Description =
                    "Implement product analytics (Mixpanel, Amplitude) across all products. Build dashboards for user activation, retention, and feature adoption metrics.",
                AssignedToId = userThao.Id,
                Priority = 2,
                Status = TaskItemStatus.Open,
                CreatedAt = new DateTime(2025, 9, 15, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2025, 11, 1, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-00000000002E"),
                ProjectId = projectUserResearch.Id,
                Title = "NPS & Customer Satisfaction Tracking",
                Description =
                    "Set up automated NPS surveys and CSAT tracking across product touchpoints. Analyze trends and identify drivers of satisfaction and churn.",
                AssignedToId = null,
                Priority = 2,
                Status = TaskItemStatus.Open,
                CreatedAt = new DateTime(2025, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2025, 11, 15, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-00000000002F"),
                ProjectId = projectUserResearch.Id,
                Title = "Competitive Analysis Report",
                Description =
                    "Research and document competitor AI product offerings, pricing, feature sets, and user reviews. Provide actionable insights for product differentiation.",
                AssignedToId = userMai.Id,
                Priority = 3,
                Status = TaskItemStatus.Open,
                CreatedAt = new DateTime(2025, 10, 10, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2025, 12, 1, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-000000000030"),
                ProjectId = projectUserResearch.Id,
                Title = "Research Repository & Knowledge Base",
                Description =
                    "Create a centralized repository for research artifacts: interview transcripts, survey data, usability videos, reports, and actionable recommendations.",
                AssignedToId = userHoang.Id,
                Priority = 3,
                Status = TaskItemStatus.Open,
                CreatedAt = new DateTime(2025, 10, 15, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2025, 12, 15, 0, 0, 0, DateTimeKind.Utc),
            },
        };

        context.TaskItems.AddRange(tasks);
        context.SaveChanges();
        Console.WriteLine("48 tasks seeded.");

        // ---------------------------------------------------------------
        // Summary
        // ---------------------------------------------------------------
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║              DATA SEED COMPLETED SUCCESSFULLY            ║");
        Console.WriteLine("╠══════════════════════════════════════════════════════════╣");
        Console.WriteLine("║  10 users           seeded                                ║");
        Console.WriteLine("║   4 teams           seeded                                ║");
        Console.WriteLine("║  20 team members    seeded                                ║");
        Console.WriteLine("║   8 projects        seeded                                ║");
        Console.WriteLine("║  24 project members seeded                                ║");
        Console.WriteLine("║  48 tasks           seeded                                ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
        Console.ResetColor();
    }
}
