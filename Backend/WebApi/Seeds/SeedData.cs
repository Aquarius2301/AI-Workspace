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
            Language = LanguageDisplay.vi,
            CreatedAt = new DateTimeOffset(2025, 1, 15, 0, 0, 0, TimeSpan.Zero),
        };

        var userMinh = new User
        {
            Id = Guid.Parse("B2C3D4E5-F6A7-8901-BCDE-F12345678901"),
            Name = "Minh Nguyen",
            Email = "minh.nguyen@aiworkspace.com",
            PasswordHash = passwordHash,
            AvatarUrl = "https://api.dicebear.com/7.x/avataaars/svg?seed=minh",
            Language = LanguageDisplay.vi,
            CreatedAt = new DateTimeOffset(2025, 2, 1, 0, 0, 0, TimeSpan.Zero),
        };

        var userThao = new User
        {
            Id = Guid.Parse("C3D4E5F6-A7B8-9012-CDEF-123456789012"),
            Name = "Thao Tran",
            Email = "thao.tran@aiworkspace.com",
            PasswordHash = passwordHash,
            AvatarUrl = "https://api.dicebear.com/7.x/avataaars/svg?seed=thao",
            Language = LanguageDisplay.vi,
            CreatedAt = new DateTimeOffset(2025, 3, 10, 0, 0, 0, TimeSpan.Zero),
        };

        var userHoang = new User
        {
            Id = Guid.Parse("D4E5F6A7-B8C9-0123-DEF1-234567890123"),
            Name = "Hoang Pham",
            Email = "hoang.pham@aiworkspace.com",
            PasswordHash = passwordHash,
            AvatarUrl = "https://api.dicebear.com/7.x/avataaars/svg?seed=hoang",
            Language = LanguageDisplay.vi,
            CreatedAt = new DateTimeOffset(2025, 4, 5, 0, 0, 0, TimeSpan.Zero),
        };

        var userAlex = new User
        {
            Id = Guid.Parse("E5F6A7B8-C9D0-1234-EF12-345678901234"),
            Name = "Alex Jones",
            Email = "alex.jones@aiworkspace.com",
            PasswordHash = passwordHash,
            AvatarUrl = "https://api.dicebear.com/7.x/avataaars/svg?seed=alex",
            Language = LanguageDisplay.en,
            CreatedAt = new DateTimeOffset(2025, 5, 20, 0, 0, 0, TimeSpan.Zero),
        };

        var userLinh = new User
        {
            Id = Guid.Parse("F6A7B8C9-D0E1-2345-F123-456789012345"),
            Name = "Linh Vu",
            Email = "linh.vu@aiworkspace.com",
            PasswordHash = passwordHash,
            AvatarUrl = "https://api.dicebear.com/7.x/avataaars/svg?seed=linh",
            Language = LanguageDisplay.vi,
            CreatedAt = new DateTimeOffset(2025, 6, 1, 0, 0, 0, TimeSpan.Zero),
        };

        var userHuy = new User
        {
            Id = Guid.Parse("0708090A-0B0C-0D0E-0F10-111213141516"),
            Name = "Huy Le",
            Email = "huy.le@aiworkspace.com",
            PasswordHash = passwordHash,
            AvatarUrl = "https://api.dicebear.com/7.x/avataaars/svg?seed=huy",
            Language = LanguageDisplay.vi,
            CreatedAt = new DateTimeOffset(2025, 7, 15, 0, 0, 0, TimeSpan.Zero),
        };

        var userMai = new User
        {
            Id = Guid.Parse("1718191A-1B1C-1D1E-1F20-212223242526"),
            Name = "Mai Phan",
            Email = "mai.phan@aiworkspace.com",
            PasswordHash = passwordHash,
            AvatarUrl = "https://api.dicebear.com/7.x/avataaars/svg?seed=mai",
            Language = LanguageDisplay.vi,
            CreatedAt = new DateTimeOffset(2025, 8, 1, 0, 0, 0, TimeSpan.Zero),
        };

        var userBao = new User
        {
            Id = Guid.Parse("2728292A-2B2C-2D2E-2F30-313233343536"),
            Name = "Bao Nguyen",
            Email = "bao.nguyen@aiworkspace.com",
            PasswordHash = passwordHash,
            AvatarUrl = "https://api.dicebear.com/7.x/avataaars/svg?seed=bao",
            Language = LanguageDisplay.vi,
            CreatedAt = new DateTimeOffset(2025, 8, 15, 0, 0, 0, TimeSpan.Zero),
        };

        var userChi = new User
        {
            Id = Guid.Parse("3738393A-3B3C-3D3E-3F40-414243444546"),
            Name = "Chi Pham",
            Email = "chi.pham@aiworkspace.com",
            PasswordHash = passwordHash,
            AvatarUrl = "https://api.dicebear.com/7.x/avataaars/svg?seed=chi",
            Language = LanguageDisplay.vi,
            CreatedAt = new DateTimeOffset(2025, 9, 1, 0, 0, 0, TimeSpan.Zero),
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
                JoinedAt = new DateTimeOffset(2025, 1, 20, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("B1000000-0000-0000-0000-000000000002"),
                TeamId = teamAlpha.Id,
                UserId = userMinh.Id,
                Role = TeamMemberRole.Leader,
                JoinedAt = new DateTimeOffset(2025, 2, 5, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("B1000000-0000-0000-0000-000000000003"),
                TeamId = teamAlpha.Id,
                UserId = userThao.Id,
                Role = TeamMemberRole.Member,
                JoinedAt = new DateTimeOffset(2025, 3, 15, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("B1000000-0000-0000-0000-000000000004"),
                TeamId = teamAlpha.Id,
                UserId = userHoang.Id,
                Role = TeamMemberRole.Member,
                JoinedAt = new DateTimeOffset(2025, 4, 10, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("B1000000-0000-0000-0000-000000000005"),
                TeamId = teamAlpha.Id,
                UserId = userBao.Id,
                Role = TeamMemberRole.Member,
                JoinedAt = new DateTimeOffset(2025, 8, 20, 0, 0, 0, TimeSpan.Zero),
            },
            // Beta Team: Alex (Admin), Linh (Leader), Khang (Member), Thao (Member), Chi (Member)
            new()
            {
                Id = Guid.Parse("B1000000-0000-0000-0000-000000000006"),
                TeamId = teamBeta.Id,
                UserId = userAlex.Id,
                Role = TeamMemberRole.Admin,
                JoinedAt = new DateTimeOffset(2025, 5, 25, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("B1000000-0000-0000-0000-000000000007"),
                TeamId = teamBeta.Id,
                UserId = userLinh.Id,
                Role = TeamMemberRole.Leader,
                JoinedAt = new DateTimeOffset(2025, 6, 5, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("B1000000-0000-0000-0000-000000000008"),
                TeamId = teamBeta.Id,
                UserId = userKhang.Id,
                Role = TeamMemberRole.Member,
                JoinedAt = new DateTimeOffset(2025, 6, 10, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("B1000000-0000-0000-0000-000000000009"),
                TeamId = teamBeta.Id,
                UserId = userThao.Id,
                Role = TeamMemberRole.Member,
                JoinedAt = new DateTimeOffset(2025, 6, 12, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("B1000000-0000-0000-0000-00000000000A"),
                TeamId = teamBeta.Id,
                UserId = userChi.Id,
                Role = TeamMemberRole.Member,
                JoinedAt = new DateTimeOffset(2025, 9, 10, 0, 0, 0, TimeSpan.Zero),
            },
            // Gamma Team: Huy (Admin), Alex (Leader), Khang (Member), Minh (Member), Bao (Member)
            new()
            {
                Id = Guid.Parse("B1000000-0000-0000-0000-00000000000B"),
                TeamId = teamGamma.Id,
                UserId = userHuy.Id,
                Role = TeamMemberRole.Admin,
                JoinedAt = new DateTimeOffset(2025, 7, 20, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("B1000000-0000-0000-0000-00000000000C"),
                TeamId = teamGamma.Id,
                UserId = userAlex.Id,
                Role = TeamMemberRole.Leader,
                JoinedAt = new DateTimeOffset(2025, 8, 1, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("B1000000-0000-0000-0000-00000000000D"),
                TeamId = teamGamma.Id,
                UserId = userKhang.Id,
                Role = TeamMemberRole.Member,
                JoinedAt = new DateTimeOffset(2025, 8, 5, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("B1000000-0000-0000-0000-00000000000E"),
                TeamId = teamGamma.Id,
                UserId = userMinh.Id,
                Role = TeamMemberRole.Member,
                JoinedAt = new DateTimeOffset(2025, 8, 10, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("B1000000-0000-0000-0000-00000000000F"),
                TeamId = teamGamma.Id,
                UserId = userBao.Id,
                Role = TeamMemberRole.Member,
                JoinedAt = new DateTimeOffset(2025, 9, 5, 0, 0, 0, TimeSpan.Zero),
            },
            // Delta Team: Mai (Admin), Linh (Leader), Thao (Member), Hoang (Member), Chi (Member)
            new()
            {
                Id = Guid.Parse("B1000000-0000-0000-0000-000000000010"),
                TeamId = teamDelta.Id,
                UserId = userMai.Id,
                Role = TeamMemberRole.Admin,
                JoinedAt = new DateTimeOffset(2025, 8, 15, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("B1000000-0000-0000-0000-000000000011"),
                TeamId = teamDelta.Id,
                UserId = userLinh.Id,
                Role = TeamMemberRole.Leader,
                JoinedAt = new DateTimeOffset(2025, 8, 20, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("B1000000-0000-0000-0000-000000000012"),
                TeamId = teamDelta.Id,
                UserId = userThao.Id,
                Role = TeamMemberRole.Member,
                JoinedAt = new DateTimeOffset(2025, 8, 25, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("B1000000-0000-0000-0000-000000000013"),
                TeamId = teamDelta.Id,
                UserId = userHoang.Id,
                Role = TeamMemberRole.Member,
                JoinedAt = new DateTimeOffset(2025, 8, 30, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("B1000000-0000-0000-0000-000000000014"),
                TeamId = teamDelta.Id,
                UserId = userChi.Id,
                Role = TeamMemberRole.Member,
                JoinedAt = new DateTimeOffset(2025, 9, 15, 0, 0, 0, TimeSpan.Zero),
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
                JoinedAt = new DateTimeOffset(2025, 2, 1, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("D1000000-0000-0000-0000-000000000002"),
                ProjectId = projectNlp.Id,
                UserId = userMinh.Id,
                JoinedAt = new DateTimeOffset(2025, 2, 1, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("D1000000-0000-0000-0000-000000000003"),
                ProjectId = projectNlp.Id,
                UserId = userThao.Id,
                JoinedAt = new DateTimeOffset(2025, 3, 15, 0, 0, 0, TimeSpan.Zero),
            },
            // Speech Recognition: Minh, Thao, Hoang
            new()
            {
                Id = Guid.Parse("D1000000-0000-0000-0000-000000000004"),
                ProjectId = projectSpeech.Id,
                UserId = userMinh.Id,
                JoinedAt = new DateTimeOffset(2025, 4, 1, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("D1000000-0000-0000-0000-000000000005"),
                ProjectId = projectSpeech.Id,
                UserId = userThao.Id,
                JoinedAt = new DateTimeOffset(2025, 4, 5, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("D1000000-0000-0000-0000-000000000006"),
                ProjectId = projectSpeech.Id,
                UserId = userHoang.Id,
                JoinedAt = new DateTimeOffset(2025, 4, 10, 0, 0, 0, TimeSpan.Zero),
            },
            // AI Chatbot: Alex, Linh, Thao
            new()
            {
                Id = Guid.Parse("D1000000-0000-0000-0000-000000000007"),
                ProjectId = projectChatbot.Id,
                UserId = userAlex.Id,
                JoinedAt = new DateTimeOffset(2025, 6, 1, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("D1000000-0000-0000-0000-000000000008"),
                ProjectId = projectChatbot.Id,
                UserId = userLinh.Id,
                JoinedAt = new DateTimeOffset(2025, 6, 5, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("D1000000-0000-0000-0000-000000000009"),
                ProjectId = projectChatbot.Id,
                UserId = userThao.Id,
                JoinedAt = new DateTimeOffset(2025, 6, 12, 0, 0, 0, TimeSpan.Zero),
            },
            // Recommendation Engine: Linh, Alex, Chi
            new()
            {
                Id = Guid.Parse("D1000000-0000-0000-0000-00000000000A"),
                ProjectId = projectRecommendation.Id,
                UserId = userLinh.Id,
                JoinedAt = new DateTimeOffset(2025, 7, 1, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("D1000000-0000-0000-0000-00000000000B"),
                ProjectId = projectRecommendation.Id,
                UserId = userAlex.Id,
                JoinedAt = new DateTimeOffset(2025, 7, 5, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("D1000000-0000-0000-0000-00000000000C"),
                ProjectId = projectRecommendation.Id,
                UserId = userChi.Id,
                JoinedAt = new DateTimeOffset(2025, 9, 10, 0, 0, 0, TimeSpan.Zero),
            },
            // MLOps Platform: Huy, Alex, Khang
            new()
            {
                Id = Guid.Parse("D1000000-0000-0000-0000-00000000000D"),
                ProjectId = projectMlops.Id,
                UserId = userHuy.Id,
                JoinedAt = new DateTimeOffset(2025, 8, 1, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("D1000000-0000-0000-0000-00000000000E"),
                ProjectId = projectMlops.Id,
                UserId = userAlex.Id,
                JoinedAt = new DateTimeOffset(2025, 8, 5, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("D1000000-0000-0000-0000-00000000000F"),
                ProjectId = projectMlops.Id,
                UserId = userKhang.Id,
                JoinedAt = new DateTimeOffset(2025, 8, 10, 0, 0, 0, TimeSpan.Zero),
            },
            // Real-Time Data Pipeline: Huy, Bao, Minh
            new()
            {
                Id = Guid.Parse("D1000000-0000-0000-0000-000000000010"),
                ProjectId = projectDataPipeline.Id,
                UserId = userHuy.Id,
                JoinedAt = new DateTimeOffset(2025, 8, 15, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("D1000000-0000-0000-0000-000000000011"),
                ProjectId = projectDataPipeline.Id,
                UserId = userBao.Id,
                JoinedAt = new DateTimeOffset(2025, 8, 20, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("D1000000-0000-0000-0000-000000000012"),
                ProjectId = projectDataPipeline.Id,
                UserId = userMinh.Id,
                JoinedAt = new DateTimeOffset(2025, 9, 1, 0, 0, 0, TimeSpan.Zero),
            },
            // AI Product Portal: Mai, Linh, Chi
            new()
            {
                Id = Guid.Parse("D1000000-0000-0000-0000-000000000013"),
                ProjectId = projectProductPortal.Id,
                UserId = userMai.Id,
                JoinedAt = new DateTimeOffset(2025, 9, 1, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("D1000000-0000-0000-0000-000000000014"),
                ProjectId = projectProductPortal.Id,
                UserId = userLinh.Id,
                JoinedAt = new DateTimeOffset(2025, 9, 5, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("D1000000-0000-0000-0000-000000000015"),
                ProjectId = projectProductPortal.Id,
                UserId = userChi.Id,
                JoinedAt = new DateTimeOffset(2025, 9, 15, 0, 0, 0, TimeSpan.Zero),
            },
            // User Research: Mai, Hoang, Thao
            new()
            {
                Id = Guid.Parse("D1000000-0000-0000-0000-000000000016"),
                ProjectId = projectUserResearch.Id,
                UserId = userMai.Id,
                JoinedAt = new DateTimeOffset(2025, 9, 1, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("D1000000-0000-0000-0000-000000000017"),
                ProjectId = projectUserResearch.Id,
                UserId = userHoang.Id,
                JoinedAt = new DateTimeOffset(2025, 9, 5, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("D1000000-0000-0000-0000-000000000018"),
                ProjectId = projectUserResearch.Id,
                UserId = userThao.Id,
                JoinedAt = new DateTimeOffset(2025, 9, 10, 0, 0, 0, TimeSpan.Zero),
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
                CreatedAt = new DateTimeOffset(2025, 2, 5, 0, 0, 0, TimeSpan.Zero),
                DueDate = new DateTimeOffset(2025, 3, 5, 0, 0, 0, TimeSpan.Zero),
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
                CreatedAt = new DateTimeOffset(2025, 3, 10, 0, 0, 0, TimeSpan.Zero),
                DueDate = new DateTimeOffset(2025, 5, 15, 0, 0, 0, TimeSpan.Zero),
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
                CreatedAt = new DateTimeOffset(2025, 4, 1, 0, 0, 0, TimeSpan.Zero),
                DueDate = new DateTimeOffset(2025, 6, 30, 0, 0, 0, TimeSpan.Zero),
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
                CreatedAt = new DateTimeOffset(2025, 5, 1, 0, 0, 0, TimeSpan.Zero),
                DueDate = new DateTimeOffset(2025, 7, 15, 0, 0, 0, TimeSpan.Zero),
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
                CreatedAt = new DateTimeOffset(2025, 6, 1, 0, 0, 0, TimeSpan.Zero),
                DueDate = new DateTimeOffset(2025, 8, 15, 0, 0, 0, TimeSpan.Zero),
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
                CreatedAt = new DateTimeOffset(2025, 7, 1, 0, 0, 0, TimeSpan.Zero),
                DueDate = new DateTimeOffset(2025, 9, 1, 0, 0, 0, TimeSpan.Zero),
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
                CreatedAt = new DateTimeOffset(2025, 4, 15, 0, 0, 0, TimeSpan.Zero),
                DueDate = new DateTimeOffset(2025, 5, 30, 0, 0, 0, TimeSpan.Zero),
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
                CreatedAt = new DateTimeOffset(2025, 6, 1, 0, 0, 0, TimeSpan.Zero),
                DueDate = new DateTimeOffset(2025, 8, 1, 0, 0, 0, TimeSpan.Zero),
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
                CreatedAt = new DateTimeOffset(2025, 6, 15, 0, 0, 0, TimeSpan.Zero),
                DueDate = new DateTimeOffset(2025, 8, 15, 0, 0, 0, TimeSpan.Zero),
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
                CreatedAt = new DateTimeOffset(2025, 7, 1, 0, 0, 0, TimeSpan.Zero),
                DueDate = new DateTimeOffset(2025, 9, 1, 0, 0, 0, TimeSpan.Zero),
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
                CreatedAt = new DateTimeOffset(2025, 7, 15, 0, 0, 0, TimeSpan.Zero),
                DueDate = new DateTimeOffset(2025, 9, 15, 0, 0, 0, TimeSpan.Zero),
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
                CreatedAt = new DateTimeOffset(2025, 8, 1, 0, 0, 0, TimeSpan.Zero),
                DueDate = new DateTimeOffset(2025, 10, 1, 0, 0, 0, TimeSpan.Zero),
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
                CreatedAt = new DateTimeOffset(2025, 6, 5, 0, 0, 0, TimeSpan.Zero),
                DueDate = new DateTimeOffset(2025, 7, 15, 0, 0, 0, TimeSpan.Zero),
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
                CreatedAt = new DateTimeOffset(2025, 6, 8, 0, 0, 0, TimeSpan.Zero),
                DueDate = new DateTimeOffset(2025, 7, 30, 0, 0, 0, TimeSpan.Zero),
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
                CreatedAt = new DateTimeOffset(2025, 6, 12, 0, 0, 0, TimeSpan.Zero),
                DueDate = new DateTimeOffset(2025, 8, 10, 0, 0, 0, TimeSpan.Zero),
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
                CreatedAt = new DateTimeOffset(2025, 6, 13, 0, 0, 0, TimeSpan.Zero),
                DueDate = new DateTimeOffset(2025, 9, 1, 0, 0, 0, TimeSpan.Zero),
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
                CreatedAt = new DateTimeOffset(2025, 7, 1, 0, 0, 0, TimeSpan.Zero),
                DueDate = new DateTimeOffset(2025, 9, 15, 0, 0, 0, TimeSpan.Zero),
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
                CreatedAt = new DateTimeOffset(2025, 7, 15, 0, 0, 0, TimeSpan.Zero),
                DueDate = new DateTimeOffset(2025, 10, 1, 0, 0, 0, TimeSpan.Zero),
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
                CreatedAt = new DateTimeOffset(2025, 7, 10, 0, 0, 0, TimeSpan.Zero),
                DueDate = new DateTimeOffset(2025, 8, 20, 0, 0, 0, TimeSpan.Zero),
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
                CreatedAt = new DateTimeOffset(2025, 7, 15, 0, 0, 0, TimeSpan.Zero),
                DueDate = new DateTimeOffset(2025, 9, 1, 0, 0, 0, TimeSpan.Zero),
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
                CreatedAt = new DateTimeOffset(2025, 8, 1, 0, 0, 0, TimeSpan.Zero),
                DueDate = new DateTimeOffset(2025, 9, 20, 0, 0, 0, TimeSpan.Zero),
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
                CreatedAt = new DateTimeOffset(2025, 8, 15, 0, 0, 0, TimeSpan.Zero),
                DueDate = new DateTimeOffset(2025, 10, 1, 0, 0, 0, TimeSpan.Zero),
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
                CreatedAt = new DateTimeOffset(2025, 8, 20, 0, 0, 0, TimeSpan.Zero),
                DueDate = new DateTimeOffset(2025, 10, 15, 0, 0, 0, TimeSpan.Zero),
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
                CreatedAt = new DateTimeOffset(2025, 9, 1, 0, 0, 0, TimeSpan.Zero),
                DueDate = new DateTimeOffset(2025, 11, 1, 0, 0, 0, TimeSpan.Zero),
            },
            // --- MLOps Platform tasks (6 tasks) ---
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-000000000019"),
                ProjectId = projectMlops.Id,
                Title = "MLflow Integration & Experiment Tracking",
                Description =
                    "Set up MLflow tracking server, configure experiment logging, and integrate with existing training pipelines for automated metric and artifact tracking.",
                AssignedToId = userHuy.Id,
                Priority = 1,
                Status = TaskItemStatus.Done,
                CreatedAt = new DateTimeOffset(2025, 8, 5, 0, 0, 0, TimeSpan.Zero),
                DueDate = new DateTimeOffset(2025, 9, 1, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-00000000001A"),
                ProjectId = projectMlops.Id,
                Title = "Model Versioning & Registry",
                Description =
                    "Implement a model registry with versioning, staging (staging/production), rollback capabilities, and automated model validation gates.",
                AssignedToId = userAlex.Id,
                Priority = 1,
                Status = TaskItemStatus.InProgress,
                CreatedAt = new DateTimeOffset(2025, 8, 10, 0, 0, 0, TimeSpan.Zero),
                DueDate = new DateTimeOffset(2025, 9, 20, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-00000000001B"),
                ProjectId = projectMlops.Id,
                Title = "Automated Retraining Pipeline",
                Description =
                    "Build a CI/CD pipeline for automated model retraining triggered by data drift detection, schedule, or manual approval. Include data validation, training, evaluation, and deployment stages.",
                AssignedToId = userKhang.Id,
                Priority = 2,
                Status = TaskItemStatus.Open,
                CreatedAt = new DateTimeOffset(2025, 8, 15, 0, 0, 0, TimeSpan.Zero),
                DueDate = new DateTimeOffset(2025, 10, 15, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-00000000001C"),
                ProjectId = projectMlops.Id,
                Title = "Model Monitoring & Alerting",
                Description =
                    "Implement real-time model monitoring for prediction drift, data drift, accuracy degradation, and infrastructure health. Set up Slack/PagerDuty alerts.",
                AssignedToId = null,
                Priority = 2,
                Status = TaskItemStatus.Open,
                CreatedAt = new DateTimeOffset(2025, 9, 1, 0, 0, 0, TimeSpan.Zero),
                DueDate = new DateTimeOffset(2025, 11, 1, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-00000000001D"),
                ProjectId = projectMlops.Id,
                Title = "Rolling Deployment & Canary Releases",
                Description =
                    "Set up Kubernetes-based rolling deployments with canary releases, traffic splitting, and automatic rollback based on error rate thresholds.",
                AssignedToId = userAlex.Id,
                Priority = 3,
                Status = TaskItemStatus.Open,
                CreatedAt = new DateTimeOffset(2025, 9, 10, 0, 0, 0, TimeSpan.Zero),
                DueDate = new DateTimeOffset(2025, 11, 15, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-00000000001E"),
                ProjectId = projectMlops.Id,
                Title = "Compliance & Audit Trail",
                Description =
                    "Implement audit logging for model decisions, data access tracking, and compliance reporting to meet regulatory requirements (GDPR, HIPAA).",
                AssignedToId = null,
                Priority = 3,
                Status = TaskItemStatus.Open,
                CreatedAt = new DateTimeOffset(2025, 9, 15, 0, 0, 0, TimeSpan.Zero),
                DueDate = new DateTimeOffset(2025, 12, 1, 0, 0, 0, TimeSpan.Zero),
            },
            // --- Real-Time Data Pipeline tasks (6 tasks) ---
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-00000000001F"),
                ProjectId = projectDataPipeline.Id,
                Title = "Kafka Cluster Setup & Topic Design",
                Description =
                    "Provision a Kafka cluster, design topic partitioning strategy, configure replication and retention policies for high-throughput event streaming.",
                AssignedToId = userHuy.Id,
                Priority = 1,
                Status = TaskItemStatus.Done,
                CreatedAt = new DateTimeOffset(2025, 8, 20, 0, 0, 0, TimeSpan.Zero),
                DueDate = new DateTimeOffset(2025, 9, 10, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-000000000020"),
                ProjectId = projectDataPipeline.Id,
                Title = "Real-Time ETL with Flink",
                Description =
                    "Implement real-time ETL jobs using Apache Flink for data enrichment, filtering, aggregation, and transformation before loading into ClickHouse.",
                AssignedToId = userBao.Id,
                Priority = 1,
                Status = TaskItemStatus.InProgress,
                CreatedAt = new DateTimeOffset(2025, 9, 1, 0, 0, 0, TimeSpan.Zero),
                DueDate = new DateTimeOffset(2025, 10, 15, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-000000000021"),
                ProjectId = projectDataPipeline.Id,
                Title = "ClickHouse Schema Design & Optimization",
                Description =
                    "Design ClickHouse table schemas using MergeTree engine, optimize partitioning and ordering keys for analytical queries and time-series data.",
                AssignedToId = userMinh.Id,
                Priority = 2,
                Status = TaskItemStatus.Open,
                CreatedAt = new DateTimeOffset(2025, 9, 5, 0, 0, 0, TimeSpan.Zero),
                DueDate = new DateTimeOffset(2025, 10, 20, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-000000000022"),
                ProjectId = projectDataPipeline.Id,
                Title = "Data Quality & Schema Validation",
                Description =
                    "Implement schema registry (Avro/Protobuf), data quality checks, and dead letter queue for handling malformed or invalid events without data loss.",
                AssignedToId = null,
                Priority = 2,
                Status = TaskItemStatus.Open,
                CreatedAt = new DateTimeOffset(2025, 9, 10, 0, 0, 0, TimeSpan.Zero),
                DueDate = new DateTimeOffset(2025, 11, 1, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-000000000023"),
                ProjectId = projectDataPipeline.Id,
                Title = "Dashboard & Real-Time Visualization",
                Description =
                    "Build real-time dashboards using Grafana connected to ClickHouse for visualizing streaming metrics, pipeline health, and business KPIs.",
                AssignedToId = userBao.Id,
                Priority = 3,
                Status = TaskItemStatus.Open,
                CreatedAt = new DateTimeOffset(2025, 9, 15, 0, 0, 0, TimeSpan.Zero),
                DueDate = new DateTimeOffset(2025, 11, 15, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-000000000024"),
                ProjectId = projectDataPipeline.Id,
                Title = "Disaster Recovery & Backup Strategy",
                Description =
                    "Design multi-region replication, Kafka mirroring, ClickHouse backup/restore procedures, and chaos engineering tests for fault tolerance validation.",
                AssignedToId = null,
                Priority = 3,
                Status = TaskItemStatus.Open,
                CreatedAt = new DateTimeOffset(2025, 10, 1, 0, 0, 0, TimeSpan.Zero),
                DueDate = new DateTimeOffset(2025, 12, 1, 0, 0, 0, TimeSpan.Zero),
            },
            // --- AI Product Portal tasks (6 tasks) ---
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-000000000025"),
                ProjectId = projectProductPortal.Id,
                Title = "Portal Architecture & Tech Stack Setup",
                Description =
                    "Design the portal architecture using Next.js, Tailwind CSS, and Headless CMS. Set up the monorepo, CI/CD, and deployment infrastructure.",
                AssignedToId = userMai.Id,
                Priority = 1,
                Status = TaskItemStatus.Done,
                CreatedAt = new DateTimeOffset(2025, 9, 5, 0, 0, 0, TimeSpan.Zero),
                DueDate = new DateTimeOffset(2025, 9, 30, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-000000000026"),
                ProjectId = projectProductPortal.Id,
                Title = "Product Showcase & Documentation Hub",
                Description =
                    "Build product listing pages with search, filtering, and detailed documentation for each AI product including API references, tutorials, and use cases.",
                AssignedToId = userLinh.Id,
                Priority = 1,
                Status = TaskItemStatus.InProgress,
                CreatedAt = new DateTimeOffset(2025, 9, 10, 0, 0, 0, TimeSpan.Zero),
                DueDate = new DateTimeOffset(2025, 11, 1, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-000000000027"),
                ProjectId = projectProductPortal.Id,
                Title = "Interactive API Playground & Demo",
                Description =
                    "Create an interactive API playground where users can test endpoints directly in the browser. Build live demo sandboxes for key AI capabilities.",
                AssignedToId = userChi.Id,
                Priority = 2,
                Status = TaskItemStatus.Open,
                CreatedAt = new DateTimeOffset(2025, 9, 15, 0, 0, 0, TimeSpan.Zero),
                DueDate = new DateTimeOffset(2025, 11, 15, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-000000000028"),
                ProjectId = projectProductPortal.Id,
                Title = "Pricing & Subscription Management",
                Description =
                    "Implement dynamic pricing pages with tier comparison, usage-based billing integration (Stripe), and self-service subscription management.",
                AssignedToId = null,
                Priority = 2,
                Status = TaskItemStatus.Open,
                CreatedAt = new DateTimeOffset(2025, 10, 1, 0, 0, 0, TimeSpan.Zero),
                DueDate = new DateTimeOffset(2025, 12, 1, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-000000000029"),
                ProjectId = projectProductPortal.Id,
                Title = "User Dashboard & API Key Management",
                Description =
                    "Build a user dashboard for API key generation, usage analytics, rate limit monitoring, and account settings with multi-tenant access control.",
                AssignedToId = userMai.Id,
                Priority = 3,
                Status = TaskItemStatus.Open,
                CreatedAt = new DateTimeOffset(2025, 10, 15, 0, 0, 0, TimeSpan.Zero),
                DueDate = new DateTimeOffset(2025, 12, 15, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-00000000002A"),
                ProjectId = projectProductPortal.Id,
                Title = "SEO & Performance Optimization",
                Description =
                    "Optimize for Core Web Vitals, implement SSR/ISR for documentation pages, add structured data, sitemap generation, and i18n support for multilingual content.",
                AssignedToId = null,
                Priority = 3,
                Status = TaskItemStatus.Open,
                CreatedAt = new DateTimeOffset(2025, 11, 1, 0, 0, 0, TimeSpan.Zero),
                DueDate = new DateTimeOffset(2026, 1, 15, 0, 0, 0, TimeSpan.Zero),
            },
            // --- User Research & Analytics Program tasks (6 tasks) ---
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-00000000002B"),
                ProjectId = projectUserResearch.Id,
                Title = "Research Methodology & Study Design",
                Description =
                    "Define research methodologies (qualitative and quantitative), create study protocols, recruit participant panels, and set up consent management.",
                AssignedToId = userMai.Id,
                Priority = 1,
                Status = TaskItemStatus.Done,
                CreatedAt = new DateTimeOffset(2025, 9, 10, 0, 0, 0, TimeSpan.Zero),
                DueDate = new DateTimeOffset(2025, 10, 1, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-00000000002C"),
                ProjectId = projectUserResearch.Id,
                Title = "Usability Testing Sessions",
                Description =
                    "Conduct moderated and unmoderated usability testing sessions for AI Product Portal, Chatbot, and other products. Record sessions and analyze findings.",
                AssignedToId = userHoang.Id,
                Priority = 1,
                Status = TaskItemStatus.InProgress,
                CreatedAt = new DateTimeOffset(2025, 10, 1, 0, 0, 0, TimeSpan.Zero),
                DueDate = new DateTimeOffset(2025, 11, 15, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-00000000002D"),
                ProjectId = projectUserResearch.Id,
                Title = "Behavioral Analytics Pipeline",
                Description =
                    "Implement product analytics (Mixpanel/PostHog) to track user journeys, feature adoption, drop-off points, and conversion funnels across all AI products.",
                AssignedToId = userThao.Id,
                Priority = 2,
                Status = TaskItemStatus.Open,
                CreatedAt = new DateTimeOffset(2025, 10, 10, 0, 0, 0, TimeSpan.Zero),
                DueDate = new DateTimeOffset(2025, 12, 1, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-00000000002E"),
                ProjectId = projectUserResearch.Id,
                Title = "Survey & NPS Program",
                Description =
                    "Design and deploy in-product surveys (CSAT, NPS, SUS) and feedback collection widgets. Build automated reporting and trend analysis dashboards.",
                AssignedToId = null,
                Priority = 2,
                Status = TaskItemStatus.Open,
                CreatedAt = new DateTimeOffset(2025, 10, 20, 0, 0, 0, TimeSpan.Zero),
                DueDate = new DateTimeOffset(2025, 12, 15, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-00000000002F"),
                ProjectId = projectUserResearch.Id,
                Title = "Competitive Analysis & Market Research",
                Description =
                    "Conduct competitive analysis of leading AI platforms (OpenAI, Google Vertex AI, AWS Bedrock) and produce actionable market positioning recommendations.",
                AssignedToId = userMai.Id,
                Priority = 3,
                Status = TaskItemStatus.Open,
                CreatedAt = new DateTimeOffset(2025, 11, 1, 0, 0, 0, TimeSpan.Zero),
                DueDate = new DateTimeOffset(2026, 1, 15, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-000000000030"),
                ProjectId = projectUserResearch.Id,
                Title = "Research Repository & Knowledge Base",
                Description =
                    "Create a centralized research repository with findings, personas, journey maps, and design recommendations accessible to product teams across the organization.",
                AssignedToId = null,
                Priority = 3,
                Status = TaskItemStatus.Open,
                CreatedAt = new DateTimeOffset(2025, 11, 15, 0, 0, 0, TimeSpan.Zero),
                DueDate = new DateTimeOffset(2026, 2, 1, 0, 0, 0, TimeSpan.Zero),
            },
        };

        context.TaskItems.AddRange(tasks);
        context.SaveChanges();
        Console.WriteLine("48 tasks seeded.");

        // ---------------------------------------------------------------
        // Documents (8 documents: 1 per project)
        // ---------------------------------------------------------------
        var documents = new List<Document>
        {
            new()
            {
                Id = Guid.Parse("F1000000-0000-0000-0000-000000000001"),
                ProjectId = projectNlp.Id,
                CreatorId = userKhang.Id,
                Title = "NLP Pipeline Architecture Overview",
                Content =
                    "# NLP Pipeline Architecture\n\n## Overview\nThe NLP Pipeline is designed to handle text classification, sentiment analysis, and named entity recognition using state-of-the-art transformer models.\n\n## Components\n1. **Data Ingestion Layer** - Handles data collection from various sources\n2. **Preprocessing Module** - Text cleaning, tokenization, normalization\n3. **Model Inference** - BERT/RoBERTa based models\n4. **Post-processing** - Result aggregation and formatting\n\n## Architecture Decisions\n- Use FastAPI for serving inference endpoints\n- Containerize with Docker\n- Deploy on Kubernetes with auto-scaling",
                CreatedAt = new DateTimeOffset(2025, 2, 10, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("F1000000-0000-0000-0000-000000000002"),
                ProjectId = projectSpeech.Id,
                CreatorId = userMinh.Id,
                Title = "Speech Recognition Technical Spec",
                Content =
                    "# Speech Recognition System\n\n## Technical Specifications\n\n### Model Architecture\n- Conformer encoder with CTC/RRN-T decoders\n- Self-supervised pretraining with wav2vec 2.0\n- Language model integration for beam search\n\n### Supported Languages\n- English (en-US)\n- Vietnamese (vi-VN)\n\n### Performance Targets\n- Word Error Rate (WER) < 5% for English\n- Word Error Rate (WER) < 8% for Vietnamese\n- Real-time factor (RTF) < 0.3",
                CreatedAt = new DateTimeOffset(2025, 4, 20, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("F1000000-0000-0000-0000-000000000003"),
                ProjectId = projectChatbot.Id,
                CreatorId = userAlex.Id,
                Title = "Chatbot Conversation Flow Design",
                Content =
                    "# AI Chatbot Assistant\n\n## Conversation Flow\n\n1. **Greeting & Intent Detection**\n2. **Context Gathering**\n3. **Resolution Path**\n4. **Escalation Logic**\n\n## Guardrails\n- PII detection and redaction\n- Toxicity filtering\n- Token limits for context window\n- Rate limiting per user",
                CreatedAt = new DateTimeOffset(2025, 6, 10, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("F1000000-0000-0000-0000-000000000004"),
                ProjectId = projectRecommendation.Id,
                CreatorId = userLinh.Id,
                Title = "Recommendation Engine Algorithm Design",
                Content =
                    "# Recommendation Engine\n\n## Approach\n- **Collaborative Filtering**: Matrix factorization with implicit feedback\n- **Content-Based**: Item embeddings from text, image, and metadata\n- **Hybrid**: Weighted combination of both approaches\n\n## Evaluation Metrics\n- Precision@K, Recall@K\n- Mean Average Precision (MAP)\n- Normalized Discounted Cumulative Gain (nDCG)\n- Click-Through Rate (CTR) for A/B tests",
                CreatedAt = new DateTimeOffset(2025, 7, 10, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("F1000000-0000-0000-0000-000000000005"),
                ProjectId = projectMlops.Id,
                CreatorId = userHuy.Id,
                Title = "MLOps Platform Requirements",
                Content =
                    "# MLOps Platform\n\n## Core Requirements\n\n1. **Experiment Tracking** - MLflow integration\n2. **Model Registry** - Versioning, staging, promotion\n3. **Automated Retraining** - Scheduled and trigger-based\n4. **Model Monitoring** - Drift detection, alerting\n5. **Deployment Orchestration** - Canary, rolling, blue/green\n\n## Infrastructure\n- Kubernetes (EKS)\n- GPU nodes for training\n- CPU nodes for inference\n- S3 for artifact storage",
                CreatedAt = new DateTimeOffset(2025, 8, 10, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("F1000000-0000-0000-0000-000000000006"),
                ProjectId = projectDataPipeline.Id,
                CreatorId = userHuy.Id,
                Title = "Real-Time Pipeline Architecture",
                Content =
                    "# Real-Time Data Pipeline\n\n## Architecture Components\n\n### Data Sources\n- Application events (clickstream, API calls)\n- IoT device telemetry\n- Third-party webhooks\n\n### Processing\n- Apache Kafka for event streaming\n- Apache Flink for real-time ETL\n- ClickHouse for analytical storage\n\n### Monitoring\n- Prometheus metrics\n- Grafana dashboards\n- PagerDuty alerts",
                CreatedAt = new DateTimeOffset(2025, 8, 25, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("F1000000-0000-0000-0000-000000000007"),
                ProjectId = projectProductPortal.Id,
                CreatorId = userMai.Id,
                Title = "Product Portal UX Design Guidelines",
                Content =
                    "# AI Product Portal\n\n## Design Guidelines\n\n### Brand Colors\n- Primary: #2563EB (Blue 600)\n- Secondary: #7C3AED (Violet 600)\n- Accent: #10B981 (Emerald 500)\n\n### Typography\n- Headings: Inter Bold\n- Body: Inter Regular\n- Code: JetBrains Mono\n\n### Components\n- Navigation with mega menu\n- Product cards with hover effects\n- Interactive code snippets\n- Responsive layout for mobile",
                CreatedAt = new DateTimeOffset(2025, 9, 10, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("F1000000-0000-0000-0000-000000000008"),
                ProjectId = projectUserResearch.Id,
                CreatorId = userMai.Id,
                Title = "User Research Study Plan Q4 2025",
                Content =
                    "# User Research Study Plan\n\n## Q4 2025 Studies\n\n1. **AI Chatbot Usability Study** (October)\n   - 15 participants\n   - Task completion rate, SUS score\n   \n2. **Product Portal Beta Testing** (November)\n   - 30 participants\n   - Feature adoption, NPS\n   \n3. **Cross-Product Journey Mapping** (December)\n   - 10 participants\n   - Diary study, longitudinal analysis\n\n## Tools\n- UserTesting for remote studies\n- Hotjar for session recordings\n- Dovetail for analysis",
                CreatedAt = new DateTimeOffset(2025, 9, 15, 0, 0, 0, TimeSpan.Zero),
            },
        };

        context.Documents.AddRange(documents);
        context.SaveChanges();
        Console.WriteLine("8 documents seeded.");

        // ---------------------------------------------------------------
        // Comments (40 comments across various references)
        // ---------------------------------------------------------------
        var comments = new List<Comment>
        {
            // Comments on NLP Pipeline tasks
            new()
            {
                Id = Guid.Parse("F2000000-0000-0000-0000-000000000001"),
                CreatorId = userKhang.Id,
                ReferenceType = ReferenceType.TaskItem,
                ReferenceId = Guid.Parse("E1000000-0000-0000-0000-000000000001"),
                Content =
                    "Great progress on data collection! The preprocessing pipeline is handling edge cases well.",
                CreatedAt = new DateTimeOffset(2025, 2, 20, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("F2000000-0000-0000-0000-000000000002"),
                CreatorId = userMinh.Id,
                ReferenceType = ReferenceType.TaskItem,
                ReferenceId = Guid.Parse("E1000000-0000-0000-0000-000000000001"),
                Content =
                    "The tokenizer is working well. I noticed some issues with special characters in the text. Let me fix that.",
                CreatedAt = new DateTimeOffset(2025, 2, 22, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("F2000000-0000-0000-0000-000000000003"),
                CreatorId = userThao.Id,
                ReferenceType = ReferenceType.TaskItem,
                ReferenceId = Guid.Parse("E1000000-0000-0000-0000-000000000002"),
                Content =
                    "I've started the hyperparameter tuning with Optuna. The initial results look promising with BERT-base achieving 92% accuracy on the validation set.",
                CreatedAt = new DateTimeOffset(2025, 3, 20, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("F2000000-0000-0000-0000-000000000004"),
                CreatorId = userKhang.Id,
                ReferenceType = ReferenceType.TaskItem,
                ReferenceId = Guid.Parse("E1000000-0000-0000-0000-000000000002"),
                Content =
                    "Let me know if you need more GPU hours for the hyperparameter search. The cluster is currently underutilized.",
                CreatedAt = new DateTimeOffset(2025, 3, 25, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("F2000000-0000-0000-0000-000000000005"),
                CreatorId = userThao.Id,
                ReferenceType = ReferenceType.TaskItem,
                ReferenceId = Guid.Parse("E1000000-0000-0000-0000-000000000002"),
                Content =
                    "I could use an additional 100 GPU hours for the final tuning phase. The search space is quite large.",
                CreatedAt = new DateTimeOffset(2025, 3, 26, 0, 0, 0, TimeSpan.Zero),
            },
            // Comments on Speech Recognition tasks
            new()
            {
                Id = Guid.Parse("F2000000-0000-0000-0000-000000000006"),
                CreatorId = userMinh.Id,
                ReferenceType = ReferenceType.TaskItem,
                ReferenceId = Guid.Parse("E1000000-0000-0000-0000-000000000007"),
                Content =
                    "Audio augmentation pipeline is complete. The SpecAugment implementation is giving us 15% relative improvement in WER.",
                CreatedAt = new DateTimeOffset(2025, 5, 1, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("F2000000-0000-0000-0000-000000000007"),
                CreatorId = userThao.Id,
                ReferenceType = ReferenceType.TaskItem,
                ReferenceId = Guid.Parse("E1000000-0000-0000-0000-000000000007"),
                Content =
                    "I've verified the augmentation pipeline with noisy environments. The data quality is excellent.",
                CreatedAt = new DateTimeOffset(2025, 5, 5, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("F2000000-0000-0000-0000-000000000008"),
                CreatorId = userHoang.Id,
                ReferenceType = ReferenceType.TaskItem,
                ReferenceId = Guid.Parse("E1000000-0000-0000-0000-000000000009"),
                Content =
                    "I'm researching the best approach for Vietnamese language model integration. The compound word segmentation is challenging.",
                CreatedAt = new DateTimeOffset(2025, 7, 1, 0, 0, 0, TimeSpan.Zero),
            },
            // Comments on AI Chatbot tasks
            new()
            {
                Id = Guid.Parse("F2000000-0000-0000-0000-000000000009"),
                CreatorId = userAlex.Id,
                ReferenceType = ReferenceType.TaskItem,
                ReferenceId = Guid.Parse("E1000000-0000-0000-0000-00000000000D"),
                Content =
                    "LLM integration is going well. GPT-4 is handling complex queries well, but we need to optimize the prompt templates for cost efficiency.",
                CreatedAt = new DateTimeOffset(2025, 6, 20, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("F2000000-0000-0000-0000-00000000000A"),
                CreatorId = userLinh.Id,
                ReferenceType = ReferenceType.TaskItem,
                ReferenceId = Guid.Parse("E1000000-0000-0000-0000-00000000000E"),
                Content =
                    "I've set up the vector database schema. We're using cosine similarity for embedding search with a hybrid retrieval strategy.",
                CreatedAt = new DateTimeOffset(2025, 6, 25, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("F2000000-0000-0000-0000-00000000000B"),
                CreatorId = userThao.Id,
                ReferenceType = ReferenceType.TaskItem,
                ReferenceId = Guid.Parse("E1000000-0000-0000-0000-00000000000F"),
                Content =
                    "Intent classification model is showing 94% accuracy on our internal test set. Need to improve edge cases for ambiguous queries.",
                CreatedAt = new DateTimeOffset(2025, 7, 5, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("F2000000-0000-0000-0000-00000000000C"),
                CreatorId = userAlex.Id,
                ReferenceType = ReferenceType.TaskItem,
                ReferenceId = Guid.Parse("E1000000-0000-0000-0000-000000000012"),
                Content =
                    "We should consider using a multi-step escalation flow with sentiment analysis thresholds. Happy to brainstorm on the approach.",
                CreatedAt = new DateTimeOffset(2025, 8, 1, 0, 0, 0, TimeSpan.Zero),
            },
            // Comments on Recommendation Engine tasks
            new()
            {
                Id = Guid.Parse("F2000000-0000-0000-0000-00000000000D"),
                CreatorId = userLinh.Id,
                ReferenceType = ReferenceType.TaskItem,
                ReferenceId = Guid.Parse("E1000000-0000-0000-0000-000000000013"),
                Content =
                    "Data pipeline is ingesting 10M events/day from the e-commerce platform. Latency is under 5 seconds from event to ClickHouse.",
                CreatedAt = new DateTimeOffset(2025, 7, 25, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("F2000000-0000-0000-0000-00000000000E"),
                CreatorId = userAlex.Id,
                ReferenceType = ReferenceType.TaskItem,
                ReferenceId = Guid.Parse("E1000000-0000-0000-0000-000000000014"),
                Content =
                    "I've started implementing the ALS model. The initial offline metrics show 0.45 MAP@10, which is a good baseline.",
                CreatedAt = new DateTimeOffset(2025, 8, 1, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("F2000000-0000-0000-0000-00000000000F"),
                CreatorId = userChi.Id,
                ReferenceType = ReferenceType.TaskItem,
                ReferenceId = Guid.Parse("E1000000-0000-0000-0000-000000000016"),
                Content =
                    "I've configured the A/B testing framework. We'll be able to run up to 4 concurrent experiments with proper statistical significance tracking.",
                CreatedAt = new DateTimeOffset(2025, 9, 1, 0, 0, 0, TimeSpan.Zero),
            },
            // Comments on MLOps Platform tasks
            new()
            {
                Id = Guid.Parse("F2000000-0000-0000-0000-000000000010"),
                CreatorId = userHuy.Id,
                ReferenceType = ReferenceType.TaskItem,
                ReferenceId = Guid.Parse("E1000000-0000-0000-0000-000000000019"),
                Content =
                    "MLflow server is up and running. We've integrated it with the training pipeline and it's logging metrics and artifacts successfully.",
                CreatedAt = new DateTimeOffset(2025, 8, 20, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("F2000000-0000-0000-0000-000000000011"),
                CreatorId = userAlex.Id,
                ReferenceType = ReferenceType.TaskItem,
                ReferenceId = Guid.Parse("E1000000-0000-0000-0000-00000000001A"),
                Content =
                    "Model registry is working. We have staging and production aliases set up with automated validation gates.",
                CreatedAt = new DateTimeOffset(2025, 8, 25, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("F2000000-0000-0000-0000-000000000012"),
                CreatorId = userKhang.Id,
                ReferenceType = ReferenceType.TaskItem,
                ReferenceId = Guid.Parse("E1000000-0000-0000-0000-00000000001B"),
                Content =
                    "I've drafted the retraining pipeline design. We should discuss the data drift detection thresholds in tomorrow's standup.",
                CreatedAt = new DateTimeOffset(2025, 9, 5, 0, 0, 0, TimeSpan.Zero),
            },
            // Comments on Real-Time Pipeline tasks
            new()
            {
                Id = Guid.Parse("F2000000-0000-0000-0000-000000000013"),
                CreatorId = userHuy.Id,
                ReferenceType = ReferenceType.TaskItem,
                ReferenceId = Guid.Parse("E1000000-0000-0000-0000-00000000001F"),
                Content =
                    "Kafka cluster is provisioned with 3 brokers. Topic partitioning strategy is designed for 10 partitions per topic with replication factor 3.",
                CreatedAt = new DateTimeOffset(2025, 9, 1, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("F2000000-0000-0000-0000-000000000014"),
                CreatorId = userBao.Id,
                ReferenceType = ReferenceType.TaskItem,
                ReferenceId = Guid.Parse("E1000000-0000-0000-0000-000000000020"),
                Content =
                    "Flink ETL jobs are processing 50K events/second with exactly-once semantics. Checkpointing is configured for every 30 seconds.",
                CreatedAt = new DateTimeOffset(2025, 9, 20, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("F2000000-0000-0000-0000-000000000015"),
                CreatorId = userMinh.Id,
                ReferenceType = ReferenceType.TaskItem,
                ReferenceId = Guid.Parse("E1000000-0000-0000-0000-000000000021"),
                Content =
                    "ClickHouse schema is optimized for time-series queries. We're seeing sub-millisecond query times for recent data with the right partitioning.",
                CreatedAt = new DateTimeOffset(2025, 10, 1, 0, 0, 0, TimeSpan.Zero),
            },
            // Comments on AI Product Portal tasks
            new()
            {
                Id = Guid.Parse("F2000000-0000-0000-0000-000000000016"),
                CreatorId = userMai.Id,
                ReferenceType = ReferenceType.TaskItem,
                ReferenceId = Guid.Parse("E1000000-0000-0000-0000-000000000025"),
                Content =
                    "Portal architecture is set up with Next.js 14 and Tailwind. The monorepo structure using Turborepo is working well for code sharing.",
                CreatedAt = new DateTimeOffset(2025, 9, 15, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("F2000000-0000-0000-0000-000000000017"),
                CreatorId = userLinh.Id,
                ReferenceType = ReferenceType.TaskItem,
                ReferenceId = Guid.Parse("E1000000-0000-0000-0000-000000000026"),
                Content =
                    "Product showcase pages are looking great. We have search with Elasticsearch and filtering by category, pricing model, and API availability.",
                CreatedAt = new DateTimeOffset(2025, 10, 1, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("F2000000-0000-0000-0000-000000000018"),
                CreatorId = userChi.Id,
                ReferenceType = ReferenceType.TaskItem,
                ReferenceId = Guid.Parse("E1000000-0000-0000-0000-000000000027"),
                Content =
                    "The interactive API playground is working! Users can try endpoints with sample data and see live responses. The code snippet generator supports 5 languages.",
                CreatedAt = new DateTimeOffset(2025, 10, 10, 0, 0, 0, TimeSpan.Zero),
            },
            // Comments on User Research tasks
            new()
            {
                Id = Guid.Parse("F2000000-0000-0000-0000-000000000019"),
                CreatorId = userMai.Id,
                ReferenceType = ReferenceType.TaskItem,
                ReferenceId = Guid.Parse("E1000000-0000-0000-0000-00000000002B"),
                Content =
                    "Study protocols are approved by the research board. Participant recruitment is underway with clear consent procedures.",
                CreatedAt = new DateTimeOffset(2025, 9, 20, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("F2000000-0000-0000-0000-00000000001A"),
                CreatorId = userHoang.Id,
                ReferenceType = ReferenceType.TaskItem,
                ReferenceId = Guid.Parse("E1000000-0000-0000-0000-00000000002C"),
                Content =
                    "First round of usability testing is complete. The AI Chatbot received an average SUS score of 72, which is good but we have room for improvement.",
                CreatedAt = new DateTimeOffset(2025, 10, 20, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("F2000000-0000-0000-0000-00000000001B"),
                CreatorId = userThao.Id,
                ReferenceType = ReferenceType.TaskItem,
                ReferenceId = Guid.Parse("E1000000-0000-0000-0000-00000000002D"),
                Content =
                    "Product analytics are live! We're tracking key events across all products. The funnel analysis shows a 40% drop-off at the sign-up step.",
                CreatedAt = new DateTimeOffset(2025, 11, 1, 0, 0, 0, TimeSpan.Zero),
            },
            // Comments on Documents (to seed non-TaskItem comments)
            new()
            {
                Id = Guid.Parse("F2000000-0000-0000-0000-00000000001C"),
                CreatorId = userMinh.Id,
                ReferenceType = ReferenceType.Document,
                ReferenceId = Guid.Parse("F1000000-0000-0000-0000-000000000001"),
                Content =
                    "The architecture overview looks comprehensive. I suggest we add a section on the monitoring and observability stack.",
                CreatedAt = new DateTimeOffset(2025, 2, 20, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("F2000000-0000-0000-0000-00000000001D"),
                CreatorId = userKhang.Id,
                ReferenceType = ReferenceType.Document,
                ReferenceId = Guid.Parse("F1000000-0000-0000-0000-000000000001"),
                Content =
                    "Good point! I'll add a section on Prometheus metrics and Grafana dashboards for the inference endpoints.",
                CreatedAt = new DateTimeOffset(2025, 2, 21, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("F2000000-0000-0000-0000-00000000001E"),
                CreatorId = userAlex.Id,
                ReferenceType = ReferenceType.Document,
                ReferenceId = Guid.Parse("F1000000-0000-0000-0000-000000000003"),
                Content =
                    "The conversation flow is well thought out. We should also consider multi-language support for the chatbot from the start.",
                CreatedAt = new DateTimeOffset(2025, 6, 20, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("F2000000-0000-0000-0000-00000000001F"),
                CreatorId = userLinh.Id,
                ReferenceType = ReferenceType.Document,
                ReferenceId = Guid.Parse("F1000000-0000-0000-0000-000000000004"),
                Content =
                    "The hybrid approach looks solid. We'll need to carefully tune the weight between collaborative and content-based signals.",
                CreatedAt = new DateTimeOffset(2025, 7, 20, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("F2000000-0000-0000-0000-000000000020"),
                CreatorId = userHuy.Id,
                ReferenceType = ReferenceType.Document,
                ReferenceId = Guid.Parse("F1000000-0000-0000-0000-000000000005"),
                Content =
                    "We should also include a disaster recovery plan. Let me draft the backup and restore procedures for the MLflow server and model registry.",
                CreatedAt = new DateTimeOffset(2025, 8, 20, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("F2000000-0000-0000-0000-000000000021"),
                CreatorId = userBao.Id,
                ReferenceType = ReferenceType.Document,
                ReferenceId = Guid.Parse("F1000000-0000-0000-0000-000000000006"),
                Content =
                    "The pipeline architecture looks scalable. We might want to add a section on data retention policies and cold storage archiving.",
                CreatedAt = new DateTimeOffset(2025, 9, 10, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("F2000000-0000-0000-0000-000000000022"),
                CreatorId = userChi.Id,
                ReferenceType = ReferenceType.Document,
                ReferenceId = Guid.Parse("F1000000-0000-0000-0000-000000000007"),
                Content =
                    "Love the design guidelines! The color palette is modern and accessible. Let's make sure we have dark mode support as well.",
                CreatedAt = new DateTimeOffset(2025, 9, 20, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("F2000000-0000-0000-0000-000000000023"),
                CreatorId = userHoang.Id,
                ReferenceType = ReferenceType.Document,
                ReferenceId = Guid.Parse("F1000000-0000-0000-0000-000000000008"),
                Content =
                    "The research plan is comprehensive. I can help with recruiting participants from the existing user base for the usability studies.",
                CreatedAt = new DateTimeOffset(2025, 10, 1, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("F2000000-0000-0000-0000-000000000024"),
                CreatorId = userMai.Id,
                ReferenceType = ReferenceType.Document,
                ReferenceId = Guid.Parse("F1000000-0000-0000-0000-000000000008"),
                Content =
                    "That would be great! I'll share the participant screening criteria with you so we can target the right user segments.",
                CreatedAt = new DateTimeOffset(2025, 10, 2, 0, 0, 0, TimeSpan.Zero),
            },
            // Additional comments from the past week (to make recent comments appear)
            new()
            {
                Id = Guid.Parse("F2000000-0000-0000-0000-000000000025"),
                CreatorId = userKhang.Id,
                ReferenceType = ReferenceType.TaskItem,
                ReferenceId = Guid.Parse("E1000000-0000-0000-0000-000000000003"),
                Content =
                    "I've started working on the API deployment. The FastAPI application is containerized and running in the dev cluster.",
                CreatedAt = new DateTimeOffset(2026, 6, 20, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("F2000000-0000-0000-0000-000000000026"),
                CreatorId = userThao.Id,
                ReferenceType = ReferenceType.TaskItem,
                ReferenceId = Guid.Parse("E1000000-0000-0000-0000-00000000000F"),
                Content =
                    "The intent classification model is now at 96% accuracy. I've deployed the updated model to the staging environment for testing.",
                CreatedAt = new DateTimeOffset(2026, 6, 22, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("F2000000-0000-0000-0000-000000000027"),
                CreatorId = userAlex.Id,
                ReferenceType = ReferenceType.Document,
                ReferenceId = Guid.Parse("F1000000-0000-0000-0000-000000000003"),
                Content =
                    "Updated the guardrails section to include the new content moderation policies. Ready for review.",
                CreatedAt = new DateTimeOffset(2026, 6, 23, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("F2000000-0000-0000-0000-000000000028"),
                CreatorId = userMinh.Id,
                ReferenceType = ReferenceType.TaskItem,
                ReferenceId = Guid.Parse("E1000000-0000-0000-0000-000000000008"),
                Content =
                    "Conformer model training is progressing well. Current WER is at 6.2% for English. We're on track to meet the 5% target.",
                CreatedAt = new DateTimeOffset(2026, 6, 24, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("F2000000-0000-0000-0000-000000000029"),
                CreatorId = userBao.Id,
                ReferenceType = ReferenceType.TaskItem,
                ReferenceId = Guid.Parse("E1000000-0000-0000-0000-000000000020"),
                Content =
                    "Flink job performance improved after tuning the parallelism and checkpointing interval. Processing 80K events/second now.",
                CreatedAt = new DateTimeOffset(2026, 6, 25, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("F2000000-0000-0000-0000-00000000002A"),
                CreatorId = userMai.Id,
                ReferenceType = ReferenceType.Document,
                ReferenceId = Guid.Parse("F1000000-0000-0000-0000-000000000007"),
                Content =
                    "Dark mode support is implemented! The design system is now fully responsive with both light and dark themes.",
                CreatedAt = new DateTimeOffset(2026, 6, 25, 0, 0, 0, TimeSpan.Zero),
            },
        };

        context.Comments.AddRange(comments);
        context.SaveChanges();
        Console.WriteLine("42 comments seeded.");

        // ---------------------------------------------------------------
        // Attachments (optional, 10 sample attachments)
        // ---------------------------------------------------------------
        var attachments = new List<Attachment>
        {
            new()
            {
                Id = Guid.Parse("F3000000-0000-0000-0000-000000000001"),
                ReferenceType = ReferenceType.TaskItem,
                ReferenceId = Guid.Parse("E1000000-0000-0000-0000-000000000001"),
                FileName = "data_preprocessing_report.pdf",
                ContentType = "application/pdf",
                Url = "/uploads/data_preprocessing_report.pdf",
                UploadedAt = new DateTimeOffset(2025, 2, 25, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("F3000000-0000-0000-0000-000000000002"),
                ReferenceType = ReferenceType.TaskItem,
                ReferenceId = Guid.Parse("E1000000-0000-0000-0000-000000000001"),
                FileName = "dataset_statistics.csv",
                ContentType = "text/csv",
                Url = "/uploads/dataset_statistics.csv",
                UploadedAt = new DateTimeOffset(2025, 3, 1, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("F3000000-0000-0000-0000-000000000003"),
                ReferenceType = ReferenceType.TaskItem,
                ReferenceId = Guid.Parse("E1000000-0000-0000-0000-000000000007"),
                FileName = "audio_samples_analysis.wav",
                ContentType = "audio/wav",
                Url = "/uploads/audio_samples_analysis.wav",
                UploadedAt = new DateTimeOffset(2025, 5, 10, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("F3000000-0000-0000-0000-000000000004"),
                ReferenceType = ReferenceType.TaskItem,
                ReferenceId = Guid.Parse("E1000000-0000-0000-0000-00000000000D"),
                FileName = "prompt_templates_v2.json",
                ContentType = "application/json",
                Url = "/uploads/prompt_templates_v2.json",
                UploadedAt = new DateTimeOffset(2025, 6, 25, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("F3000000-0000-0000-0000-000000000005"),
                ReferenceType = ReferenceType.Document,
                ReferenceId = Guid.Parse("F1000000-0000-0000-0000-000000000001"),
                FileName = "architecture_diagram_v3.png",
                ContentType = "image/png",
                Url = "/uploads/architecture_diagram_v3.png",
                UploadedAt = new DateTimeOffset(2025, 3, 1, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("F3000000-0000-0000-0000-000000000006"),
                ReferenceType = ReferenceType.Document,
                ReferenceId = Guid.Parse("F1000000-0000-0000-0000-000000000005"),
                FileName = "mlops_requirements.xlsx",
                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                Url = "/uploads/mlops_requirements.xlsx",
                UploadedAt = new DateTimeOffset(2025, 8, 15, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("F3000000-0000-0000-0000-000000000007"),
                ReferenceType = ReferenceType.TaskItem,
                ReferenceId = Guid.Parse("E1000000-0000-0000-0000-000000000019"),
                FileName = "mlflow_setup_guide.md",
                ContentType = "text/markdown",
                Url = "/uploads/mlflow_setup_guide.md",
                UploadedAt = new DateTimeOffset(2025, 8, 22, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("F3000000-0000-0000-0000-000000000008"),
                ReferenceType = ReferenceType.TaskItem,
                ReferenceId = Guid.Parse("E1000000-0000-0000-0000-000000000025"),
                FileName = "portal_mockups_v2.fig",
                ContentType = "application/figma",
                Url = "/uploads/portal_mockups_v2.fig",
                UploadedAt = new DateTimeOffset(2025, 9, 12, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("F3000000-0000-0000-0000-000000000009"),
                ReferenceType = ReferenceType.TaskItem,
                ReferenceId = Guid.Parse("E1000000-0000-0000-0000-00000000002B"),
                FileName = "study_protocol_v1.pdf",
                ContentType = "application/pdf",
                Url = "/uploads/study_protocol_v1.pdf",
                UploadedAt = new DateTimeOffset(2025, 9, 22, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("F3000000-0000-0000-0000-00000000000A"),
                ReferenceType = ReferenceType.TaskItem,
                ReferenceId = Guid.Parse("E1000000-0000-0000-0000-000000000013"),
                FileName = "event_schema_v2.avsc",
                ContentType = "application/octet-stream",
                Url = "/uploads/event_schema_v2.avsc",
                UploadedAt = new DateTimeOffset(2025, 8, 1, 0, 0, 0, TimeSpan.Zero),
            },
        };

        context.Attachments.AddRange(attachments);
        context.SaveChanges();
        Console.WriteLine("10 attachments seeded.");

        // ---------------------------------------------------------------
        // AI Interactions (10 sample interactions)
        // ---------------------------------------------------------------
        var aiInteractions = new List<AiInteraction>
        {
            new()
            {
                Id = Guid.Parse("F4000000-0000-0000-0000-000000000001"),
                UserId = userKhang.Id,
                ProjectId = projectNlp.Id,
                Model = "gpt-4",
                Prompt = "Explain the concept of attention mechanism in transformer models.",
                ResponseText =
                    "The attention mechanism allows the model to focus on different parts of the input sequence when producing each part of the output. In transformers, we use self-attention where each position can attend to all positions in the previous layer, enabling the model to capture long-range dependencies effectively.",
                CreatedAt = new DateTimeOffset(2025, 3, 15, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("F4000000-0000-0000-0000-000000000002"),
                UserId = userMinh.Id,
                ProjectId = projectSpeech.Id,
                Model = "gpt-4",
                Prompt = "What are the best practices for data augmentation in speech recognition?",
                ResponseText =
                    "Key data augmentation techniques for ASR include: (1) SpecAugment - frequency and time masking, (2) Noise injection with various background noises, (3) Speed perturbation (0.9x, 1.0x, 1.1x), (4) Room impulse response simulation for reverberation, and (5) SpecAugment with adaptive masking based on frequency characteristics.",
                CreatedAt = new DateTimeOffset(2025, 5, 20, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("F4000000-0000-0000-0000-000000000003"),
                UserId = userAlex.Id,
                ProjectId = projectChatbot.Id,
                Model = "gpt-4",
                Prompt =
                    "Design a prompt template for a customer support chatbot that handles refund requests.",
                ResponseText =
                    "Here's a prompt template design for refund requests:\n\nSystem: You are a helpful customer support agent. Follow these steps:\n1. Verify the order information\n2. Understand the reason for the refund\n3. Check the refund policy based on the product category\n4. Inform the customer about the refund process and timeline\n5. If the request is outside policy, politely explain and offer alternatives",
                CreatedAt = new DateTimeOffset(2025, 7, 1, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("F4000000-0000-0000-0000-000000000004"),
                UserId = userLinh.Id,
                ProjectId = projectRecommendation.Id,
                Model = "gpt-4",
                Prompt =
                    "Compare collaborative filtering vs content-based filtering for an e-commerce recommendation system.",
                ResponseText =
                    "Collaborative filtering leverages user-item interactions to find patterns and make recommendations based on similar users' preferences. It's effective but suffers from cold-start problems. Content-based filtering uses item features to recommend similar items to what a user has liked. It handles cold-start better but can lead to filter bubbles. A hybrid approach combining both typically yields the best results.",
                CreatedAt = new DateTimeOffset(2025, 7, 20, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("F4000000-0000-0000-0000-000000000005"),
                UserId = userHuy.Id,
                ProjectId = projectMlops.Id,
                Model = "claude-3-opus",
                Prompt = "What metrics should I monitor for a production ML model?",
                ResponseText =
                    "Essential ML model monitoring metrics:\n\n1. Data Drift: Feature distribution changes (PSI, KL divergence)\n2. Prediction Drift: Changes in model output distribution\n3. Model Performance: Accuracy, precision, recall when ground truth is available\n4. Data Quality: Missing values, outliers, schema violations\n5. Operational: Latency, throughput, memory usage, error rates\n6. Business Impact: Conversion rates, revenue impact, user engagement",
                CreatedAt = new DateTimeOffset(2025, 9, 1, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("F4000000-0000-0000-0000-000000000006"),
                UserId = userBao.Id,
                ProjectId = projectDataPipeline.Id,
                Model = "gpt-4",
                Prompt =
                    "Explain the difference between exactly-once and at-least-once semantics in stream processing.",
                ResponseText =
                    "Exactly-once semantics ensure that each event is processed exactly once, with no duplicates and no data loss. This is achieved through idempotent writes, transactional boundaries, and checkpointing. At-least-once semantics guarantee that each event is processed at least once, but duplicates may occur. Exactly-once provides stronger guarantees but has higher overhead, while at-least-once is simpler and more performant but requires downstream deduplication.",
                CreatedAt = new DateTimeOffset(2025, 9, 10, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("F4000000-0000-0000-0000-000000000007"),
                UserId = userMai.Id,
                ProjectId = projectProductPortal.Id,
                Model = "claude-3-opus",
                Prompt =
                    "What are the key UX principles for designing an API documentation portal?",
                ResponseText =
                    "Key UX principles for API documentation portals:\n\n1. Interactive examples: Let users try API calls directly\n2. Clear navigation: Well-organized sidebar with search\n3. Code samples: Provide examples in multiple languages\n4. Consistency: Use consistent naming conventions\n5. Progressive disclosure: Start simple, then reveal complexity\n6. Visual hierarchy: Clear typography and spacing\n7. Responsive design: Works on all devices\n8. Versioning: Clearly indicate API version and changes",
                CreatedAt = new DateTimeOffset(2025, 10, 1, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("F4000000-0000-0000-0000-000000000008"),
                UserId = userKhang.Id,
                ProjectId = projectNlp.Id,
                Model = "gpt-4",
                Prompt = "How do I handle class imbalance in text classification?",
                ResponseText =
                    "Strategies for handling class imbalance in text classification:\n\n1. Resampling: Oversample minority classes (SMOTE) or undersample majority classes\n2. Class weights: Assign higher weights to minority classes in the loss function\n3. Data augmentation: Generate synthetic samples for minority classes using back-translation or LLM-based generation\n4. Ensemble methods: Use balanced batch sampling during training\n5. Evaluation: Use precision, recall, and F1-score instead of accuracy\n6. Threshold tuning: Adjust decision thresholds for each class",
                CreatedAt = new DateTimeOffset(2025, 10, 15, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("F4000000-0000-0000-0000-000000000009"),
                UserId = userAlex.Id,
                ProjectId = projectChatbot.Id,
                Model = "claude-3-opus",
                Prompt =
                    "What are the best practices for RAG (Retrieval-Augmented Generation) in chatbots?",
                ResponseText =
                    "Best practices for RAG in chatbots:\n\n1. Chunking strategy: Split documents into meaningful chunks with overlap\n2. Embedding quality: Use high-quality embedding models (e.g., text-embedding-3-large)\n3. Hybrid search: Combine dense (vector) and sparse (keyword) search\n4. Re-ranking: Use a cross-encoder to re-rank top results\n5. Context window management: Fit relevant chunks within the LLM context limit\n6. Citation and attribution: Always cite sources in responses\n7. Query rewriting: Rewrite user queries for better retrieval\n8. Caching: Cache frequent queries and responses for latency reduction",
                CreatedAt = new DateTimeOffset(2026, 1, 15, 0, 0, 0, TimeSpan.Zero),
            },
            new()
            {
                Id = Guid.Parse("F4000000-0000-0000-0000-00000000000A"),
                UserId = userHoang.Id,
                ProjectId = projectUserResearch.Id,
                Model = "gpt-4",
                Prompt = "What statistical methods should I use for analyzing A/B test results?",
                ResponseText =
                    "Key statistical methods for A/B testing:\n\n1. Hypothesis testing: Use two-sample t-test for continuous metrics, chi-square test for conversion rates\n2. Confidence intervals: Report results with 95% confidence intervals\n3. Sample size calculation: Calculate required sample size before starting\n4. Multiple testing correction: Use Bonferroni or FDR correction for multiple metrics\n5. Sequential testing: Use always-valid p-values for continuous monitoring\n6. Bayesian methods: Use Bayesian A/B testing for more intuitive interpretations\n7. Power analysis: Ensure adequate statistical power to detect meaningful effects",
                CreatedAt = new DateTimeOffset(2026, 3, 1, 0, 0, 0, TimeSpan.Zero),
            },
        };

        context.AiInteractions.AddRange(aiInteractions);
        context.SaveChanges();
        Console.WriteLine("10 AI interactions seeded.");

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                   SEEDING COMPLETE!                       ║");
        Console.WriteLine("╠══════════════════════════════════════════════════════════╣");
        Console.WriteLine("║  10 users, 4 teams, 20 members, 8 projects, 24 project members,  ║");
        Console.WriteLine("║  48 tasks, 8 documents, 42 comments, 10 attachments,               ║");
        Console.WriteLine(
            "║  10 AI interactions seeded successfully!                            ║"
        );
        Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
        Console.ResetColor();
    }
}
