using Microsoft.EntityFrameworkCore;

namespace ExamSystem.Entity
{
    public class exam_system_entities : DbContext
    {
        public exam_system_entities(DbContextOptions<exam_system_entities> options) : base(options) { }

        public DbSet<m_admin_user> m_admin_users { get; set; }
        public DbSet<m_grade> m_grades { get; set; }
        public DbSet<m_subject> m_subjects { get; set; }
        public DbSet<m_question> m_questions { get; set; }
        public DbSet<m_answer_option> m_answer_options { get; set; }
        public DbSet<m_marking_rule> m_marking_rules { get; set; }
        public DbSet<t_exam> t_exams { get; set; }
        public DbSet<t_exam_question> t_exam_questions { get; set; }
        public DbSet<m_token> m_tokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<m_admin_user>(e =>
            {
                e.HasKey(x => x.id);
                e.HasIndex(x => x.username).IsUnique();
                e.HasIndex(x => x.email).IsUnique();
                e.Property(x => x.username).IsRequired().HasMaxLength(100);
                e.Property(x => x.email).HasMaxLength(200);
                e.Property(x => x.password_hash).IsRequired();
                e.Property(x => x.full_name).HasMaxLength(255);
            });

            modelBuilder.Entity<m_grade>(e =>
            {
                e.HasKey(x => x.id);
                e.HasIndex(x => x.name).IsUnique();
                e.Property(x => x.name).IsRequired().HasMaxLength(200);
            });

            modelBuilder.Entity<m_subject>(e =>
            {
                e.HasKey(x => x.id);
                e.HasIndex(x => x.code).IsUnique();
                e.Property(x => x.name).IsRequired().HasMaxLength(200);
                e.Property(x => x.code).IsRequired().HasMaxLength(50);
                e.HasOne(x => x.grade)
                 .WithMany(x => x.subjects)
                 .HasForeignKey(x => x.grade_id)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<m_question>(e =>
            {
                e.HasKey(x => x.id);
                e.Property(x => x.question_text).IsRequired();
                e.HasOne(x => x.subject)
                 .WithMany(x => x.questions)
                 .HasForeignKey(x => x.subject_id)
                 .OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.grade)
                 .WithMany()
                 .HasForeignKey(x => x.grade_id)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<m_answer_option>(e =>
            {
                e.HasKey(x => x.id);
                e.HasOne(x => x.question)
                 .WithMany(x => x.answer_options)
                 .HasForeignKey(x => x.question_id)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<m_marking_rule>(e =>
            {
                e.HasKey(x => x.id);
                e.HasOne(x => x.subject)
                 .WithMany(x => x.marking_rules)
                 .HasForeignKey(x => x.subject_id)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<t_exam>(e =>
            {
                e.HasKey(x => x.id);
                e.HasIndex(x => x.exam_code).IsUnique();
                e.Property(x => x.exam_code).IsRequired().HasMaxLength(50);
                e.Property(x => x.title).IsRequired().HasMaxLength(255);
                e.HasOne(x => x.subject)
                 .WithMany(x => x.exams)
                 .HasForeignKey(x => x.subject_id)
                 .OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.grade)
                 .WithMany()
                 .HasForeignKey(x => x.grade_id)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<t_exam_question>(e =>
            {
                e.HasKey(x => x.id);
                e.HasOne(x => x.exam)
                 .WithMany(x => x.exam_questions)
                 .HasForeignKey(x => x.exam_id)
                 .OnDelete(DeleteBehavior.Cascade);
                e.HasOne(x => x.question)
                 .WithMany(x => x.exam_questions)
                 .HasForeignKey(x => x.question_id)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<m_token>(e =>
            {
                e.HasKey(x => x.id);
                e.HasIndex(x => x.session_token).IsUnique();
                e.HasOne(x => x.user)
                 .WithMany(x => x.created_tokens)
                 .HasForeignKey(x => x.user_id)
                 .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
