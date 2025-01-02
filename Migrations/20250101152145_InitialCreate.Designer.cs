﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using five_birds_be.Data;

#nullable disable

namespace five_birds_be.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20250101152145_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("five_birds_be.Models.Answer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Answer1")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Answer2")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Answer3")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Answer4")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("CorrectAnswer")
                        .HasColumnType("int");

                    b.Property<DateTime>("Create_at")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("QuestionId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Update_at")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("QuestionId");

                    b.ToTable("Answer");
                });

            modelBuilder.Entity("five_birds_be.Models.Candidate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("CvFilePath")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Education")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Experience")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("Candidates");
                });

            modelBuilder.Entity("five_birds_be.Models.CandidateTest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("ExamId")
                        .HasColumnType("int");

                    b.Property<bool>("IsPast")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("Point")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ExamId");

                    b.HasIndex("UserId");

                    b.ToTable("CandidateTests");
                });

            modelBuilder.Entity("five_birds_be.Models.Exam", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("Create_at")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Duration")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("Update_at")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.ToTable("Exam");
                });

            modelBuilder.Entity("five_birds_be.Models.Question", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("Create_at")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("ExamId")
                        .HasColumnType("int");

                    b.Property<int>("Point")
                        .HasColumnType("int");

                    b.Property<string>("QuestionExam")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("Update_at")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("ExamId");

                    b.ToTable("Question");
                });

            modelBuilder.Entity("five_birds_be.Models.Result", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("AnswerId")
                        .HasColumnType("int");

                    b.Property<int?>("CandidateTestId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Create_at")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("ExamAnswer")
                        .HasColumnType("int");

                    b.Property<int>("ExamId")
                        .HasColumnType("int");

                    b.Property<bool>("Is_correct")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("QuestionId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Update_at")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AnswerId");

                    b.HasIndex("CandidateTestId");

                    b.HasIndex("ExamId");

                    b.HasIndex("QuestionId");

                    b.HasIndex("UserId");

                    b.ToTable("Result");
                });

            modelBuilder.Entity("five_birds_be.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("Create_at")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.Property<DateTime>("Update_at")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("UserId");

                    b.ToTable("User");
                });

            modelBuilder.Entity("five_birds_be.Models.User_Eaxam", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("Create_at")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("ExamDate")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("ExamId")
                        .HasColumnType("int");

                    b.Property<string>("ExamTime")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("TestStatus")
                        .HasColumnType("int");

                    b.Property<DateTime>("Update_at")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ExamId");

                    b.HasIndex("UserId");

                    b.ToTable("User_Exams");
                });

            modelBuilder.Entity("five_birds_be.Models.Answer", b =>
                {
                    b.HasOne("five_birds_be.Models.Question", "Question")
                        .WithMany("Answers")
                        .HasForeignKey("QuestionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Question");
                });

            modelBuilder.Entity("five_birds_be.Models.Candidate", b =>
                {
                    b.HasOne("five_birds_be.Models.User", "User")
                        .WithOne("Candidate")
                        .HasForeignKey("five_birds_be.Models.Candidate", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("five_birds_be.Models.CandidateTest", b =>
                {
                    b.HasOne("five_birds_be.Models.Exam", "Exam")
                        .WithMany("CandidateTests")
                        .HasForeignKey("ExamId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("five_birds_be.Models.User", "User")
                        .WithMany("CandidateTests")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Exam");

                    b.Navigation("User");
                });

            modelBuilder.Entity("five_birds_be.Models.Question", b =>
                {
                    b.HasOne("five_birds_be.Models.Exam", "Exam")
                        .WithMany("Question")
                        .HasForeignKey("ExamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Exam");
                });

            modelBuilder.Entity("five_birds_be.Models.Result", b =>
                {
                    b.HasOne("five_birds_be.Models.Answer", "Answer")
                        .WithMany()
                        .HasForeignKey("AnswerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("five_birds_be.Models.CandidateTest", null)
                        .WithMany("Results")
                        .HasForeignKey("CandidateTestId");

                    b.HasOne("five_birds_be.Models.Exam", "Exam")
                        .WithMany()
                        .HasForeignKey("ExamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("five_birds_be.Models.Question", "Questions")
                        .WithMany()
                        .HasForeignKey("QuestionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("five_birds_be.Models.User", "User")
                        .WithMany("Results")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Answer");

                    b.Navigation("Exam");

                    b.Navigation("Questions");

                    b.Navigation("User");
                });

            modelBuilder.Entity("five_birds_be.Models.User_Eaxam", b =>
                {
                    b.HasOne("five_birds_be.Models.Exam", "Exam")
                        .WithMany()
                        .HasForeignKey("ExamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("five_birds_be.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Exam");

                    b.Navigation("User");
                });

            modelBuilder.Entity("five_birds_be.Models.CandidateTest", b =>
                {
                    b.Navigation("Results");
                });

            modelBuilder.Entity("five_birds_be.Models.Exam", b =>
                {
                    b.Navigation("CandidateTests");

                    b.Navigation("Question");
                });

            modelBuilder.Entity("five_birds_be.Models.Question", b =>
                {
                    b.Navigation("Answers");
                });

            modelBuilder.Entity("five_birds_be.Models.User", b =>
                {
                    b.Navigation("Candidate")
                        .IsRequired();

                    b.Navigation("CandidateTests");

                    b.Navigation("Results");
                });
#pragma warning restore 612, 618
        }
    }
}