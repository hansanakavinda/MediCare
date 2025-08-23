using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MediCare.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<APPOINTMENT> APPOINTMENTs { get; set; }

    public virtual DbSet<CLINIC> CLINICs { get; set; }

    public virtual DbSet<DOCTOR> DOCTORs { get; set; }

    public virtual DbSet<FEEDBACK> FEEDBACKs { get; set; }

    public virtual DbSet<PATIENT> PATIENTs { get; set; }

    public virtual DbSet<PAYMENT> PAYMENTs { get; set; }

    public virtual DbSet<PRESCRIPTION> PRESCRIPTIONs { get; set; }

    public virtual DbSet<SCHEDULE> SCHEDULEs { get; set; }

    public virtual DbSet<SPECIALTY> SPECIALTies { get; set; }

    public virtual DbSet<USER> USERs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasDefaultSchema("APPUSER")
            .UseCollation("USING_NLS_COMP");

        modelBuilder.Entity<APPOINTMENT>(entity =>
        {
            entity.HasKey(e => e.APPOINTMENT_ID).HasName("SYS_C0013596");

            entity.ToTable("APPOINTMENT");

            entity.HasIndex(e => e.DOCTOR_ID, "IDX_APPOINTMENT_DOCTOR");

            entity.HasIndex(e => e.PATIENT_ID, "IDX_APPOINTMENT_PATIENT");

            entity.Property(e => e.APPOINTMENT_ID)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER");
            entity.Property(e => e.CREATED_AT)
                .HasPrecision(6)
                .HasDefaultValueSql("SYSTIMESTAMP ");
            entity.Property(e => e.DOCTOR_ID).HasColumnType("NUMBER");
            entity.Property(e => e.NOTES).HasColumnType("CLOB");
            entity.Property(e => e.PATIENT_ID).HasColumnType("NUMBER");
            entity.Property(e => e.SCHEDULED_AT).HasPrecision(6);
            entity.Property(e => e.SCHEDULE_ID).HasColumnType("NUMBER");
            entity.Property(e => e.STATUS)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValueSql("'Pending' ");
            entity.Property(e => e.UPDATED_AT).HasPrecision(6);

            entity.HasOne(d => d.DOCTOR).WithMany(p => p.APPOINTMENTs)
                .HasForeignKey(d => d.DOCTOR_ID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_APPOINTMENT_DOCTOR");

            entity.HasOne(d => d.PATIENT).WithMany(p => p.APPOINTMENTs)
                .HasForeignKey(d => d.PATIENT_ID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_APPOINTMENT_PATIENT");

            entity.HasOne(d => d.SCHEDULE).WithMany(p => p.APPOINTMENTs)
                .HasForeignKey(d => d.SCHEDULE_ID)
                .HasConstraintName("FK_APPOINTMENT_SCHEDULE");
        });

        modelBuilder.Entity<CLINIC>(entity =>
        {
            entity.HasKey(e => e.CLINIC_ID).HasName("SYS_C0013568");

            entity.ToTable("CLINIC");

            entity.Property(e => e.CLINIC_ID)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER");
            entity.Property(e => e.ADDRESS)
                .HasMaxLength(400)
                .IsUnicode(false);
            entity.Property(e => e.NAME)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.PHONE)
                .HasMaxLength(40)
                .IsUnicode(false);
        });

        modelBuilder.Entity<DOCTOR>(entity =>
        {
            entity.HasKey(e => e.DOCTOR_ID).HasName("SYS_C0013571");

            entity.ToTable("DOCTOR");

            entity.HasIndex(e => e.CLINIC_ID, "IDX_DOCTOR_CLINIC");

            entity.HasIndex(e => e.SPECIALTY_ID, "IDX_DOCTOR_SPECIALTY");

            entity.HasIndex(e => e.USER_ID, "UQ_DOCTOR_USER").IsUnique();

            entity.Property(e => e.DOCTOR_ID)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER");
            entity.Property(e => e.BIO).HasColumnType("CLOB");
            entity.Property(e => e.CLINIC_ID).HasColumnType("NUMBER");
            entity.Property(e => e.FEE)
                .HasDefaultValueSql("0")
                .HasColumnType("NUMBER(10,2)");
            entity.Property(e => e.SPECIALTY_ID).HasColumnType("NUMBER");
            entity.Property(e => e.USER_ID).HasColumnType("NUMBER");

            entity.HasOne(d => d.CLINIC).WithMany(p => p.DOCTORs)
                .HasForeignKey(d => d.CLINIC_ID)
                .HasConstraintName("FK_DOCTOR_CLINIC");

            entity.HasOne(d => d.SPECIALTY).WithMany(p => p.DOCTORs)
                .HasForeignKey(d => d.SPECIALTY_ID)
                .HasConstraintName("FK_DOCTOR_SPECIALTY");

            entity.HasOne(d => d.USER).WithOne(p => p.DOCTOR)
                .HasForeignKey<DOCTOR>(d => d.USER_ID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DOCTOR_USER");
        });

        modelBuilder.Entity<FEEDBACK>(entity =>
        {
            entity.HasKey(e => e.FEEDBACK_ID).HasName("SYS_C0013618");

            entity.ToTable("FEEDBACK");

            entity.Property(e => e.FEEDBACK_ID)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER");
            entity.Property(e => e.APPOINTMENT_ID).HasColumnType("NUMBER");
            entity.Property(e => e.CREATED_AT)
                .HasPrecision(6)
                .HasDefaultValueSql("SYSTIMESTAMP ");
            entity.Property(e => e.DOCTOR_ID).HasColumnType("NUMBER");
            entity.Property(e => e.MSG).HasColumnType("CLOB");
            entity.Property(e => e.PATIENT_ID).HasColumnType("NUMBER");

            entity.HasOne(d => d.APPOINTMENT).WithMany(p => p.FEEDBACKs)
                .HasForeignKey(d => d.APPOINTMENT_ID)
                .HasConstraintName("FK_FEEDBACK_APPOINTMENT");

            entity.HasOne(d => d.DOCTOR).WithMany(p => p.FEEDBACKs)
                .HasForeignKey(d => d.DOCTOR_ID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FEEDBACK_DOCTOR");

            entity.HasOne(d => d.PATIENT).WithMany(p => p.FEEDBACKs)
                .HasForeignKey(d => d.PATIENT_ID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FEEDBACK_PATIENT");
        });

        modelBuilder.Entity<PATIENT>(entity =>
        {
            entity.HasKey(e => e.PATIENT_ID).HasName("SYS_C0013578");

            entity.ToTable("PATIENT");

            entity.HasIndex(e => e.USER_ID, "UQ_PATIENT_USER").IsUnique();

            entity.Property(e => e.PATIENT_ID)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER");
            entity.Property(e => e.ADDRESS)
                .HasMaxLength(400)
                .IsUnicode(false);
            entity.Property(e => e.DOB).HasColumnType("DATE");
            entity.Property(e => e.GENDER)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.USER_ID).HasColumnType("NUMBER");

            entity.HasOne(d => d.USER).WithOne(p => p.PATIENT)
                .HasForeignKey<PATIENT>(d => d.USER_ID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PATIENT_USER");
        });

        modelBuilder.Entity<PAYMENT>(entity =>
        {
            entity.HasKey(e => e.PAYMENT_ID).HasName("SYS_C0013605");

            entity.ToTable("PAYMENT");

            entity.HasIndex(e => e.APPOINTMENT_ID, "IDX_PAYMENT_APPOINTMENT");

            entity.Property(e => e.PAYMENT_ID)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER");
            entity.Property(e => e.AMOUNT).HasColumnType("NUMBER(12,2)");
            entity.Property(e => e.APPOINTMENT_ID).HasColumnType("NUMBER");
            entity.Property(e => e.METHOD)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValueSql("'credit card'");
            entity.Property(e => e.PAID_AT).HasPrecision(6);
            entity.Property(e => e.STATUS)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValueSql("'Pending'");
            entity.Property(e => e.TXN_REF)
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.HasOne(d => d.APPOINTMENT).WithMany(p => p.PAYMENTs)
                .HasForeignKey(d => d.APPOINTMENT_ID)
                .HasConstraintName("FK_PAYMENT_APPOINTMENT");
        });

        modelBuilder.Entity<PRESCRIPTION>(entity =>
        {
            entity.HasKey(e => e.PRESCRIPTION_ID).HasName("SYS_C0013610");

            entity.ToTable("PRESCRIPTION");

            entity.Property(e => e.PRESCRIPTION_ID)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER");
            entity.Property(e => e.APPOINTMENT_ID).HasColumnType("NUMBER");
            entity.Property(e => e.CONTENT).HasColumnType("CLOB");
            entity.Property(e => e.CREATED_AT)
                .HasPrecision(6)
                .HasDefaultValueSql("SYSTIMESTAMP ");

            entity.HasOne(d => d.APPOINTMENT).WithMany(p => p.PRESCRIPTIONs)
                .HasForeignKey(d => d.APPOINTMENT_ID)
                .HasConstraintName("FK_PRESCRIPTION_APPOINTMENT");
        });

        modelBuilder.Entity<SCHEDULE>(entity =>
        {
            entity.HasKey(e => e.SCHEDULE_ID).HasName("SYS_C0013587");

            entity.ToTable("SCHEDULE");

            entity.HasIndex(e => e.DOCTOR_ID, "IDX_SCHEDULE_DOCTOR");

            entity.Property(e => e.SCHEDULE_ID)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER");
            entity.Property(e => e.DAY_OF_WEEK).HasColumnType("NUMBER(1)");
            entity.Property(e => e.DOCTOR_ID).HasColumnType("NUMBER");
            entity.Property(e => e.END_TIME)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.SLOT_MINUTES)
                .HasPrecision(3)
                .HasDefaultValueSql("30");
            entity.Property(e => e.START_TIME)
                .HasMaxLength(5)
                .IsUnicode(false);

            entity.HasOne(d => d.DOCTOR).WithMany(p => p.SCHEDULEs)
                .HasForeignKey(d => d.DOCTOR_ID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SCHEDULE_DOCTOR");
        });

        modelBuilder.Entity<SPECIALTY>(entity =>
        {
            entity.HasKey(e => e.SPECIALTY_ID).HasName("SYS_C0013564");

            entity.ToTable("SPECIALTY");

            entity.HasIndex(e => e.NAME, "UQ_SPECIALTY_NAME").IsUnique();

            entity.Property(e => e.SPECIALTY_ID)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER");
            entity.Property(e => e.NAME)
                .HasMaxLength(120)
                .IsUnicode(false);
        });

        modelBuilder.Entity<USER>(entity =>
        {
            entity.HasKey(e => e.USER_ID).HasName("SYS_C0013560");

            entity.ToTable("USERS");

            entity.HasIndex(e => e.EMAIL, "UQ_USERS_EMAIL").IsUnique();

            entity.Property(e => e.USER_ID)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER");
            entity.Property(e => e.CREATED_AT)
                .HasPrecision(6)
                .HasDefaultValueSql("SYSTIMESTAMP ");
            entity.Property(e => e.EMAIL)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FULL_NAME)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.IS_ACTIVE)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValueSql("'Y' ")
                .IsFixedLength();
            entity.Property(e => e.PHONE)
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.PWD)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ROLE)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
