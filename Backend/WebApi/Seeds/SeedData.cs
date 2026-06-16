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
        // Users (6 users)
        // ---------------------------------------------------------------
        var passwordHash = PasswordHelper.Hash("password123");

        var userKhang = new User
        {
            Id = Guid.Parse("A1B2C3D4-E5F6-7890-ABCD-EF1234567890"),
            Name = "Khang Ta",
            Email = "khang.ta@aiworkspace.com",
            PasswordHash = passwordHash,
            AvatarUrl = "https://api.dicebear.com/7.x/avataaars/svg?seed=khang",
            CreatedAt = new DateTime(2025, 1, 15, 0, 0, 0, DateTimeKind.Utc),
        };

        var userMinh = new User
        {
            Id = Guid.Parse("B2C3D4E5-F6A7-8901-BCDE-F12345678901"),
            Name = "Minh Nguyen",
            Email = "minh.nguyen@aiworkspace.com",
            PasswordHash = passwordHash,
            AvatarUrl = "https://api.dicebear.com/7.x/avataaars/svg?seed=minh",
            CreatedAt = new DateTime(2025, 2, 1, 0, 0, 0, DateTimeKind.Utc),
        };

        var userThao = new User
        {
            Id = Guid.Parse("C3D4E5F6-A7B8-9012-CDEF-123456789012"),
            Name = "Thao Tran",
            Email = "thao.tran@aiworkspace.com",
            PasswordHash = passwordHash,
            AvatarUrl = "https://api.dicebear.com/7.x/avataaars/svg?seed=thao",
            CreatedAt = new DateTime(2025, 3, 10, 0, 0, 0, DateTimeKind.Utc),
        };

        var userHoang = new User
        {
            Id = Guid.Parse("D4E5F6A7-B8C9-0123-DEF1-234567890123"),
            Name = "Hoang Pham",
            Email = "hoang.pham@aiworkspace.com",
            PasswordHash = passwordHash,
            AvatarUrl = "https://api.dicebear.com/7.x/avataaars/svg?seed=hoang",
            CreatedAt = new DateTime(2025, 4, 5, 0, 0, 0, DateTimeKind.Utc),
        };

        var userAlex = new User
        {
            Id = Guid.Parse("E5F6A7B8-C9D0-1234-EF12-345678901234"),
            Name = "Alex Jones",
            Email = "alex.jones@aiworkspace.com",
            PasswordHash = passwordHash,
            AvatarUrl = "https://api.dicebear.com/7.x/avataaars/svg?seed=alex",
            CreatedAt = new DateTime(2025, 5, 20, 0, 0, 0, DateTimeKind.Utc),
        };

        var userLinh = new User
        {
            Id = Guid.Parse("F6A7B8C9-D0E1-2345-F123-456789012345"),
            Name = "Linh Vu",
            Email = "linh.vu@aiworkspace.com",
            PasswordHash = passwordHash,
            AvatarUrl = "https://api.dicebear.com/7.x/avataaars/svg?seed=linh",
            CreatedAt = new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc),
        };

        context.Users.AddRange(userKhang, userMinh, userThao, userHoang, userAlex, userLinh);
        context.SaveChanges();
        Console.WriteLine("6 users seeded.");

        // ---------------------------------------------------------------
        // Teams (2 teams)
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

        context.Teams.AddRange(teamAlpha, teamBeta);
        context.SaveChanges();
        Console.WriteLine("2 teams seeded.");

        // ---------------------------------------------------------------
        // Team Members
        // ---------------------------------------------------------------
        var teamMembers = new List<TeamMember>
        {
            // Alpha Team: Khang (Admin), Minh (Leader), Thao (Member), Hoang (Member)
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
            // Beta Team: Alex (Admin), Linh (Leader), Khang (Member), Thao (Member)
            new()
            {
                Id = Guid.Parse("B1000000-0000-0000-0000-000000000005"),
                TeamId = teamBeta.Id,
                UserId = userAlex.Id,
                Role = TeamMemberRole.Admin,
                JoinedAt = new DateTime(2025, 5, 25, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("B1000000-0000-0000-0000-000000000006"),
                TeamId = teamBeta.Id,
                UserId = userLinh.Id,
                Role = TeamMemberRole.Leader,
                JoinedAt = new DateTime(2025, 6, 5, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("B1000000-0000-0000-0000-000000000007"),
                TeamId = teamBeta.Id,
                UserId = userKhang.Id,
                Role = TeamMemberRole.Member,
                JoinedAt = new DateTime(2025, 6, 10, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("B1000000-0000-0000-0000-000000000008"),
                TeamId = teamBeta.Id,
                UserId = userThao.Id,
                Role = TeamMemberRole.Member,
                JoinedAt = new DateTime(2025, 6, 12, 0, 0, 0, DateTimeKind.Utc),
            },
        };

        context.TeamMembers.AddRange(teamMembers);
        context.SaveChanges();
        Console.WriteLine("8 team members seeded.");

        // ---------------------------------------------------------------
        // Projects (4 projects)
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

        var projectCv = new Project
        {
            Id = Guid.Parse("C1000000-0000-0000-0000-000000000002"),
            TeamId = teamAlpha.Id,
            CreatorId = userMinh.Id,
            Name = "Computer Vision Platform",
            Description =
                "Develop a computer vision platform capable of object detection, image segmentation, and facial recognition for real-time video streams.",
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

        var projectAnalytics = new Project
        {
            Id = Guid.Parse("C1000000-0000-0000-0000-000000000004"),
            TeamId = teamBeta.Id,
            CreatorId = userLinh.Id,
            Name = "Data Analytics Dashboard",
            Description =
                "Design and implement a real-time data analytics dashboard with interactive visualizations, drill-down capabilities, and AI-driven insights.",
            Visibility = ProjectVisibility.Public,
        };

        context.Projects.AddRange(projectNlp, projectCv, projectChatbot, projectAnalytics);
        context.SaveChanges();
        Console.WriteLine("4 projects seeded.");

        // ---------------------------------------------------------------
        // Project Members
        // ---------------------------------------------------------------
        var projectMembers = new List<ProjectMember>
        {
            // NLP Pipeline project members
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
            // Computer Vision project members
            new()
            {
                Id = Guid.Parse("D1000000-0000-0000-0000-000000000004"),
                ProjectId = projectCv.Id,
                UserId = userMinh.Id,
                JoinedAt = new DateTime(2025, 3, 1, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("D1000000-0000-0000-0000-000000000005"),
                ProjectId = projectCv.Id,
                UserId = userHoang.Id,
                JoinedAt = new DateTime(2025, 4, 10, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("D1000000-0000-0000-0000-000000000006"),
                ProjectId = projectCv.Id,
                UserId = userKhang.Id,
                JoinedAt = new DateTime(2025, 4, 15, 0, 0, 0, DateTimeKind.Utc),
            },
            // AI Chatbot project members
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
            // Data Analytics project members
            new()
            {
                Id = Guid.Parse("D1000000-0000-0000-0000-000000000010"),
                ProjectId = projectAnalytics.Id,
                UserId = userLinh.Id,
                JoinedAt = new DateTime(2025, 6, 10, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("D1000000-0000-0000-0000-000000000011"),
                ProjectId = projectAnalytics.Id,
                UserId = userKhang.Id,
                JoinedAt = new DateTime(2025, 6, 10, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("D1000000-0000-0000-0000-000000000012"),
                ProjectId = projectAnalytics.Id,
                UserId = userAlex.Id,
                JoinedAt = new DateTime(2025, 6, 12, 0, 0, 0, DateTimeKind.Utc),
            },
        };

        context.ProjectMembers.AddRange(projectMembers);
        context.SaveChanges();
        Console.WriteLine("12 project members seeded.");

        // ---------------------------------------------------------------
        // TaskItems (16 tasks across 4 projects)
        // ---------------------------------------------------------------
        var tasks = new List<TaskItem>
        {
            // --- NLP Pipeline tasks (4 tasks) ---
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
            // --- Computer Vision Platform tasks (4 tasks) ---
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-000000000005"),
                ProjectId = projectCv.Id,
                Title = "Object Detection Model Integration",
                Description =
                    "Integrate YOLOv8 and DETR models for real-time object detection. Implement non-max suppression and confidence thresholding.",
                AssignedToId = userHoang.Id,
                Priority = 1,
                Status = TaskItemStatus.InProgress,
                CreatedAt = new DateTime(2025, 3, 15, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2025, 5, 20, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-000000000006"),
                ProjectId = projectCv.Id,
                Title = "Video Stream Processing Pipeline",
                Description =
                    "Build a pipeline to capture, decode, and process video streams from IP cameras using OpenCV and FFmpeg with GPU acceleration.",
                AssignedToId = userMinh.Id,
                Priority = 1,
                Status = TaskItemStatus.Done,
                CreatedAt = new DateTime(2025, 3, 1, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2025, 4, 15, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-000000000007"),
                ProjectId = projectCv.Id,
                Title = "Face Recognition Module",
                Description =
                    "Implement face detection, embedding extraction (FaceNet/ArcFace), and similarity search using FAISS for large-scale face recognition.",
                AssignedToId = userKhang.Id,
                Priority = 2,
                Status = TaskItemStatus.Blocked,
                CreatedAt = new DateTime(2025, 4, 20, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2025, 7, 1, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-000000000008"),
                ProjectId = projectCv.Id,
                Title = "Dashboard & Visualization UI",
                Description =
                    "Create a web dashboard to display detection results, stats, and live video feeds using React and WebSocket connections.",
                AssignedToId = userHoang.Id,
                Priority = 3,
                Status = TaskItemStatus.Open,
                CreatedAt = new DateTime(2025, 5, 10, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2025, 8, 1, 0, 0, 0, DateTimeKind.Utc),
            },
            // --- AI Chatbot Assistant tasks (4 tasks) ---
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-000000000009"),
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
                Id = Guid.Parse("E1000000-0000-0000-0000-000000000010"),
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
                Id = Guid.Parse("E1000000-0000-0000-0000-000000000011"),
                ProjectId = projectChatbot.Id,
                Title = "Ticket Routing & Workflow Automation",
                Description =
                    "Build intent classification and entity extraction to automatically route support tickets to appropriate departments and trigger follow-up actions.",
                AssignedToId = userThao.Id,
                Priority = 2,
                Status = TaskItemStatus.Open,
                CreatedAt = new DateTime(2025, 6, 12, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2025, 8, 10, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-000000000012"),
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
            // --- Data Analytics Dashboard tasks (4 tasks) ---
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-000000000013"),
                ProjectId = projectAnalytics.Id,
                Title = "Data Pipeline & ETL Design",
                Description =
                    "Design and implement ETL pipelines to ingest data from multiple sources (SQL, APIs, CSV) into a data warehouse (ClickHouse).",
                AssignedToId = userKhang.Id,
                Priority = 1,
                Status = TaskItemStatus.InProgress,
                CreatedAt = new DateTime(2025, 6, 12, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2025, 7, 20, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-000000000014"),
                ProjectId = projectAnalytics.Id,
                Title = "Interactive Visualization Components",
                Description =
                    "Develop interactive charts, tables, and heatmaps using D3.js and ECharts with drill-down, filtering, and export capabilities.",
                AssignedToId = userLinh.Id,
                Priority = 1,
                Status = TaskItemStatus.Open,
                CreatedAt = new DateTime(2025, 6, 13, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2025, 8, 1, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-000000000015"),
                ProjectId = projectAnalytics.Id,
                Title = "AI-Driven Insight Generation",
                Description =
                    "Implement anomaly detection, trend forecasting, and natural language summaries of key metrics using statistical models and LLMs.",
                AssignedToId = userAlex.Id,
                Priority = 2,
                Status = TaskItemStatus.Open,
                CreatedAt = new DateTime(2025, 6, 13, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2025, 8, 20, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = Guid.Parse("E1000000-0000-0000-0000-000000000016"),
                ProjectId = projectAnalytics.Id,
                Title = "Real-Time Streaming & WebSocket Setup",
                Description =
                    "Set up real-time data streaming using Apache Kafka and WebSocket connections to push live updates to the dashboard without page refresh.",
                AssignedToId = null,
                Priority = 3,
                Status = TaskItemStatus.Open,
                CreatedAt = new DateTime(2025, 6, 13, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2025, 9, 1, 0, 0, 0, DateTimeKind.Utc),
            },
        };

        context.TaskItems.AddRange(tasks);
        context.SaveChanges();
        Console.WriteLine("16 tasks seeded.");

        // ---------------------------------------------------------------
        // Summary
        // ---------------------------------------------------------------
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║              DATA SEED COMPLETED SUCCESSFULLY            ║");
        Console.WriteLine("╠══════════════════════════════════════════════════════════╣");
        Console.WriteLine("║  6 users           seeded                                ║");
        Console.WriteLine("║  2 teams           seeded                                ║");
        Console.WriteLine("║  8 team members    seeded                                ║");
        Console.WriteLine("║  4 projects        seeded                                ║");
        Console.WriteLine("║ 12 project members seeded                                ║");
        Console.WriteLine("║ 16 tasks           seeded                                ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
        Console.ResetColor();
    }
}
