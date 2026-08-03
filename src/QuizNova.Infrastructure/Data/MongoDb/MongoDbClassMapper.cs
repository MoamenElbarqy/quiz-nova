namespace QuizNova.Infrastructure.Data.MongoDb;

using Domain.Common;
using Domain.Entities.CourseChats;
using Domain.Entities.Courses;
using Domain.Entities.Enrollments;
using Domain.Entities.Identity;
using Domain.Entities.QuizAttempts;
using Domain.Entities.QuizAttempts.Answers.AutoGradedAnswers;
using Domain.Entities.QuizAttempts.Answers.AutoGradedAnswers.McqAnswer;
using Domain.Entities.QuizAttempts.Answers.AutoGradedAnswers.TrueFalseAnswer;
using Domain.Entities.QuizAttempts.Answers.Base;
using Domain.Entities.QuizAttempts.Answers.ManuallyGradedAnswers;
using Domain.Entities.QuizAttempts.Answers.ManuallyGradedAnswers.EssayAnswer;
using Domain.Entities.Quizzes;
using Domain.Entities.Quizzes.Questions.AutoGradedQuestions.Mcq;
using Domain.Entities.Quizzes.Questions.AutoGradedQuestions.Mcq.Choices;
using Domain.Entities.Quizzes.Questions.AutoGradedQuestions.TrueFalse;
using Domain.Entities.Quizzes.Questions.Base;
using Domain.Entities.Quizzes.Questions.ManuallyGradedQuestions;
using Domain.Entities.Users;
using Domain.Entities.Users.Admins;
using Domain.Entities.Users.Instructors;
using Domain.Entities.Users.Student;
using Domain.Entities.Users.UserPersonalInformation;

using MongoDB.Bson.Serialization;

public static class MongoDbClassMapper
{
    private static readonly Lock RegistrationLock = new();
    private static bool _registered;

    public static void RegisterClassMaps()
    {
        lock (RegistrationLock)
        {
            if (_registered)
            {
                return;
            }
        }

        lock (RegistrationLock)
        {
            if (_registered)
            {
                return;
            }

            RegisterEntityBaseClass();
            RegisterQuiz();
            RegisterQuestionHierarchy();
            RegisterChoice();
            RegisterQuizAttempt();
            RegisterQuestionAnswerHierarchy();
            RegisterCourse();
            RegisterUserHierarchy();
            RegisterEnrollment();
            RegisterCourseChatRoom();
            RegisterMessage();
            RegisterReaction();
            RegisterUserRefreshToken();
            RegisterPersonalInformation();

            _registered = true;
        }
    }

    private static void RegisterEntityBaseClass()
    {
        if (BsonClassMap.IsClassMapRegistered(typeof(Entity)))
        {
            return;
        }

        BsonClassMap.RegisterClassMap<Entity>(cm =>
        {
            cm.MapIdProperty(e => e.Id);
            cm.UnmapMember(e => e.DomainEvents);
            cm.SetIsRootClass(true);
        });
    }

    private static void RegisterQuiz()
    {
        if (BsonClassMap.IsClassMapRegistered(typeof(Quiz)))
        {
            return;
        }

        BsonClassMap.RegisterClassMap<Quiz>(cm =>
        {
            cm.MapProperty(q => q.CourseId);
            cm.MapProperty(q => q.InstructorId);
            cm.MapProperty(q => q.CourseName);
            cm.MapProperty(q => q.InstructorName);
            cm.MapProperty(q => q.Title);
            cm.MapProperty(q => q.StartsAtUtc);
            cm.MapProperty(q => q.EndsAtUtc);
            cm.MapProperty(q => q.Marks);
            cm.MapProperty(q => q.Questions).SetElementName("questions");
        });
    }

    private static void RegisterQuestionHierarchy()
    {
        if (BsonClassMap.IsClassMapRegistered(typeof(Question)))
        {
            return;
        }

        BsonClassMap.RegisterClassMap<Question>(cm =>
        {
            cm.MapProperty(q => q.QuizId);
            cm.MapProperty(q => q.QuestionText);
            cm.MapProperty(q => q.DisplayOrder);
            cm.MapProperty(q => q.Marks);
            cm.SetIsRootClass(true);
        });

        RegisterQuestionDerivedType<Mcq>("mcq", cm =>
        {
            cm.MapProperty(q => q.CorrectChoiceId);
            cm.MapProperty(q => q.Choices).SetElementName("choices");
        });

        RegisterQuestionDerivedType<Tf>("tf", cm =>
        {
            cm.MapProperty(q => q.CorrectChoice);
        });

        RegisterQuestionDerivedType<Essay>("essay", cm =>
        {
            cm.MapProperty(q => q.AnswerReference);
        });
    }

    private static void RegisterQuestionDerivedType<TQuestion>(string discriminator,
        Action<BsonClassMap<TQuestion>> configure)
        where TQuestion : Question
    {
        if (BsonClassMap.IsClassMapRegistered(typeof(TQuestion)))
        {
            return;
        }

        BsonClassMap.RegisterClassMap<TQuestion>(cm =>
        {
            configure(cm);
            cm.SetDiscriminator(discriminator);
        });
    }

    private static void RegisterChoice()
    {
        if (BsonClassMap.IsClassMapRegistered(typeof(Choice)))
        {
            return;
        }

        BsonClassMap.RegisterClassMap<Choice>(cm =>
        {
            cm.MapProperty(c => c.QuestionId);
            cm.MapProperty(c => c.Text);
            cm.MapProperty(c => c.DisplayOrder);
        });
    }

    private static void RegisterQuizAttempt()
    {
        if (BsonClassMap.IsClassMapRegistered(typeof(QuizAttempt)))
        {
            return;
        }

        BsonClassMap.RegisterClassMap<QuizAttempt>(cm =>
        {
            cm.MapProperty(a => a.StudentId);
            cm.MapProperty(a => a.QuizId);
            cm.MapProperty(a => a.StartedAt);
            cm.MapProperty(a => a.SubmittedAt);
            cm.MapProperty(a => a.Status);
            cm.MapProperty(a => a.QuizEndsAtUtc);
            cm.MapProperty(a => a.StudentAnswers).SetElementName("student_answers");
        });
    }

    private static void RegisterQuestionAnswerHierarchy()
    {
        if (BsonClassMap.IsClassMapRegistered(typeof(QuestionAnswer)))
        {
            return;
        }

        BsonClassMap.RegisterClassMap<QuestionAnswer>(cm =>
        {
            cm.MapProperty(a => a.StudentId);
            cm.MapProperty(a => a.QuestionId);
            cm.MapProperty(a => a.QuizAttemptId);
            cm.SetIsRootClass(true);
        });

        RegisterQuestionAnswerDerivedType<AutoGradedAnswer>("auto_graded", cm =>
        {
            cm.MapProperty(a => a.IsCorrect);
            cm.MapProperty(a => a.Marks);
        });

        RegisterQuestionAnswerDerivedType<ManuallyGradedAnswers>("manual_graded", cm =>
        {
            cm.MapProperty(a => a.Score);
            cm.MapProperty(a => a.MaxMarks);
            cm.MapProperty(a => a.Feedback);
            cm.MapProperty(a => a.GradedAt);
        });

        RegisterQuestionAnswerDerivedType<McqAnswer>("mcq_answer", cm =>
        {
            cm.MapProperty(a => a.SelectedChoiceId);
        });

        RegisterQuestionAnswerDerivedType<TfAnswer>("tf_answer", cm =>
        {
            cm.MapProperty(a => a.StudentChoice);
        });

        RegisterQuestionAnswerDerivedType<EssayAnswer>("essay_answer", cm =>
        {
            cm.MapProperty(a => a.StudentResponse);
        });
    }

    private static void RegisterQuestionAnswerDerivedType<TAnswer>(string discriminator,
        Action<BsonClassMap<TAnswer>> configure)
        where TAnswer : QuestionAnswer
    {
        if (BsonClassMap.IsClassMapRegistered(typeof(TAnswer)))
        {
            return;
        }

        BsonClassMap.RegisterClassMap<TAnswer>(cm =>
        {
            configure(cm);
            cm.SetDiscriminator(discriminator);
        });
    }

    private static void RegisterCourse()
    {
        if (BsonClassMap.IsClassMapRegistered(typeof(Course)))
        {
            return;
        }

        BsonClassMap.RegisterClassMap<Course>(cm =>
        {
            cm.MapProperty(c => c.InstructorId);
            cm.MapProperty(c => c.Status);
            cm.MapProperty(c => c.Name);
            cm.MapProperty(c => c.MinimumPassingMarks);
            cm.MapProperty(c => c.MaximumMarks);
            cm.MapProperty(c => c.RemainingMarks);
        });
    }

    private static void RegisterUserHierarchy()
    {
        if (BsonClassMap.IsClassMapRegistered(typeof(User)))
        {
            return;
        }

        BsonClassMap.RegisterClassMap<User>(cm =>
        {
            cm.MapProperty(u => u.PersonalInformation);
            cm.MapProperty(u => u.UserRole);
            cm.SetIsRootClass(true);
        });

        RegisterUserDerivedType<Admin>("admin");
        RegisterUserDerivedType<Instructor>("instructor");
        RegisterUserDerivedType<Student>("student");
    }

    private static void RegisterUserDerivedType<TUser>(string discriminator)
        where TUser : User
    {
        if (BsonClassMap.IsClassMapRegistered(typeof(TUser)))
        {
            return;
        }

        BsonClassMap.RegisterClassMap<TUser>(cm =>
        {
            cm.SetDiscriminator(discriminator);
        });
    }

    private static void RegisterEnrollment()
    {
        if (BsonClassMap.IsClassMapRegistered(typeof(Enrollment)))
        {
            return;
        }

        BsonClassMap.RegisterClassMap<Enrollment>(cm =>
        {
            cm.MapProperty(e => e.StudentId);
            cm.MapProperty(e => e.CourseId);
            cm.MapProperty(e => e.EnrolledOnUtc);
        });
    }

    private static void RegisterCourseChatRoom()
    {
        if (BsonClassMap.IsClassMapRegistered(typeof(CourseChatRoom)))
        {
            return;
        }

        BsonClassMap.RegisterClassMap<CourseChatRoom>(cm =>
        {
            cm.MapProperty(r => r.CourseId);
            cm.MapProperty(r => r.InstructorId);
            cm.MapField("_studentIds").SetElementName("student_ids");
            cm.MapField("_messages").SetElementName("messages");
        });
    }

    private static void RegisterMessage()
    {
        if (BsonClassMap.IsClassMapRegistered(typeof(Message)))
        {
            return;
        }

        BsonClassMap.RegisterClassMap<Message>(cm =>
        {
            cm.MapProperty(m => m.RoomId);
            cm.MapProperty(m => m.SenderId);
            cm.MapProperty(m => m.ReplyOnId);
            cm.MapProperty(m => m.CreatedAt);
            cm.MapProperty(m => m.Content);
            cm.MapField("_reacts").SetElementName("reacts");
        });
    }

    private static void RegisterUserRefreshToken()
    {
        if (BsonClassMap.IsClassMapRegistered(typeof(UserRefreshToken)))
        {
            return;
        }

        BsonClassMap.RegisterClassMap<UserRefreshToken>(cm =>
        {
            cm.MapIdProperty(t => t.Id);
            cm.MapProperty(t => t.Token);
            cm.MapProperty(t => t.UserId);
            cm.MapProperty(t => t.ExpiresOnUtc);
            cm.MapProperty(t => t.RevokedOnUtc);
        });
    }

    private static void RegisterPersonalInformation()
    {
        if (BsonClassMap.IsClassMapRegistered(typeof(PersonalInformation)))
        {
            return;
        }

        BsonClassMap.RegisterClassMap<PersonalInformation>(cm =>
        {
            cm.MapProperty(p => p.Name);
            cm.MapProperty(p => p.Email);
            cm.MapProperty(p => p.PhoneNumber);
        });
    }

    private static void RegisterReaction()
    {
        if (BsonClassMap.IsClassMapRegistered(typeof(Reaction)))
        {
            return;
        }

        BsonClassMap.RegisterClassMap<Reaction>(cm =>
        {
            cm.MapProperty(r => r.MessageId);
            cm.MapProperty(r => r.ReactorId);
            cm.MapProperty(r => r.Emoji);
            cm.MapProperty(r => r.CreatedAt);
        });
    }
}
