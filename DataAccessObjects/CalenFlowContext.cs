using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using BusinessObjects.Models;
using Microsoft.Extensions.Configuration;
namespace DataAccessObjects;

public partial class CalenFlowContext : DbContext
{
    public CalenFlowContext()
    {
    }

    public CalenFlowContext(DbContextOptions<CalenFlowContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Candidate> Candidates { get; set; }

    public virtual DbSet<CandidateInvite> CandidateInvites { get; set; }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<Hiring> Hirings { get; set; }

    public virtual DbSet<HiringAvailable> HiringAvailables { get; set; }

    public virtual DbSet<Interview> Interviews { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Reschedule> Reschedules { get; set; }

    public virtual DbSet<User> Users { get; set; }

    private string GetConnectionString()
    {
        IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true).Build();
        return configuration["ConnectionStrings:CalenFlow"];
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(GetConnectionString());
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.AdminId).HasName("PK__Admin__719FE48861ABAFA1");

            entity.ToTable("Admin");

            entity.Property(e => e.AdminId).ValueGeneratedNever();

            entity.HasOne(d => d.AdminNavigation).WithOne(p => p.Admin)
                .HasForeignKey<Admin>(d => d.AdminId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Admin_User");
        });

        modelBuilder.Entity<Candidate>(entity =>
        {
            entity.HasKey(e => e.CandidateId).HasName("PK__Candidat__DF539B9C37C8418A");

            entity.ToTable("Candidate");

            entity.Property(e => e.CandidateId).ValueGeneratedNever();
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UrlCv)
                .HasMaxLength(255)
                .HasColumnName("UrlCV");

            entity.HasOne(d => d.CandidateNavigation).WithOne(p => p.Candidate)
                .HasForeignKey<Candidate>(d => d.CandidateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Candidate_User");
        });

        modelBuilder.Entity<CandidateInvite>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Candidat__3214EC07AD4EBA4C");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ExpireAt).HasColumnType("datetime");
            entity.Property(e => e.IsUsed).HasDefaultValue(false);
            entity.Property(e => e.Token).HasMaxLength(200);

            entity.HasOne(d => d.Candidate).WithMany(p => p.CandidateInvites)
                .HasForeignKey(d => d.CandidateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Candidate__Candi__4AB81AF0");

            entity.HasOne(d => d.Hiring).WithMany(p => p.CandidateInvites)
                .HasForeignKey(d => d.HiringId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Candidate__Hirin__4BAC3F29");
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.CompanyId).HasName("PK__Company__2D971CAC98AA2EE3");

            entity.ToTable("Company");

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(150);
        });

        modelBuilder.Entity<Hiring>(entity =>
        {
            entity.HasKey(e => e.HiringId).HasName("PK__Hiring__B277DBA576E050E8");

            entity.ToTable("Hiring");

            entity.Property(e => e.HiringId).ValueGeneratedNever();
            entity.Property(e => e.Position).HasMaxLength(150);

            entity.HasOne(d => d.HiringNavigation).WithOne(p => p.Hiring)
                .HasForeignKey<Hiring>(d => d.HiringId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Hiring_User");
        });

        modelBuilder.Entity<HiringAvailable>(entity =>
        {
            entity.HasKey(e => e.HiringAvailableId).HasName("PK__HiringAv__E2FB4255F1EFF280");

            entity.ToTable("HiringAvailable");

            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Hiring).WithMany(p => p.HiringAvailables)
                .HasForeignKey(d => d.HiringId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HiringAvailable_Hiring");
        });

        modelBuilder.Entity<Interview>(entity =>
        {
            entity.HasKey(e => e.InterviewId).HasName("PK__Intervie__C97C585285B52762");

            entity.ToTable("Interview");

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.LinkMeet).HasMaxLength(255);
            entity.Property(e => e.Position).HasMaxLength(150);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Type).HasMaxLength(100);

            entity.HasOne(d => d.Candidate).WithMany(p => p.Interviews)
                .HasForeignKey(d => d.CandidateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Interview_Candidate");

            entity.HasOne(d => d.Hiring).WithMany(p => p.Interviews)
                .HasForeignKey(d => d.HiringId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Interview_Hiring");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E12D798503C");

            entity.ToTable("Notification");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Message).HasMaxLength(500);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(150);
            entity.Property(e => e.Type).HasMaxLength(100);

            entity.HasOne(d => d.Candidate).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.CandidateId)
                .HasConstraintName("FK_Notification_Candidate");

            entity.HasOne(d => d.Hiring).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.HiringId)
                .HasConstraintName("FK_Notification_Hiring");
        });

        modelBuilder.Entity<Reschedule>(entity =>
        {
            entity.HasKey(e => new { e.HiringId, e.CandidateId });

            entity.ToTable("Reschedule");

            entity.Property(e => e.IsAccept).HasColumnName("isAccept");
            entity.Property(e => e.OrginalDate).HasColumnType("datetime");
            entity.Property(e => e.Reason).HasMaxLength(255);
            entity.Property(e => e.RescheduleDate).HasColumnType("datetime");

            entity.HasOne(d => d.Candidate).WithMany(p => p.Reschedules)
                .HasForeignKey(d => d.CandidateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reschedule_Candidate");

            entity.HasOne(d => d.Hiring).WithMany(p => p.Reschedules)
                .HasForeignKey(d => d.HiringId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reschedule_Hiring");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CC4C6ED34AB2");

            entity.ToTable("User");

            entity.Property(e => e.Bio).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Role).HasMaxLength(50);
            entity.Property(e => e.UserName).HasMaxLength(100);

            entity.HasOne(d => d.Company).WithMany(p => p.Users)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_Company");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
