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
    [Migration("20241218164235_InitialCreate")]
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

                    b.Property<string>("CorrectAnswer")
                        .IsRequired()
                        .HasColumnType("longtext");

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

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Candidates");
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

                    b.Property<string>("Point")
                        .IsRequired()
                        .HasColumnType("longtext");

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

                    b.Property<int>("Answers")
                        .HasColumnType("int");

                    b.Property<DateTime>("Create_at")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("ExamAnswer")
                        .IsRequired()
                        .HasColumnType("longtext");

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

            modelBuilder.Entity("five_birds_be.Models.Answer", b =>
                {
                    b.HasOne("five_birds_be.Models.Question", "Question")
                        .WithMany("Answers")
                        .HasForeignKey("QuestionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Question");
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

                    b.HasOne("five_birds_be.Models.Exam", "exam")
                        .WithMany()
                        .HasForeignKey("ExamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("five_birds_be.Models.Question", "Auestions")
                        .WithMany()
                        .HasForeignKey("QuestionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("five_birds_be.Models.User", "user")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Answer");

                    b.Navigation("Auestions");

                    b.Navigation("exam");

                    b.Navigation("user");
                });

            modelBuilder.Entity("five_birds_be.Models.Exam", b =>
                {
                    b.Navigation("Question");
                });

            modelBuilder.Entity("five_birds_be.Models.Question", b =>
                {
                    b.Navigation("Answers");
                });
#pragma warning restore 612, 618
        }
    }
}